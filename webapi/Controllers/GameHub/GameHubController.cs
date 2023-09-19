using webapi.Controllers.Game;
using webapi.Controllers.Player;
using webapi.Controllers.PlayerConnection;
using webapi.Data;
using webapi.Domain.Game;
using webapi.Domain.GameDetails;
using webapi.Domain.Meld;
using webapi.Domain.PlayerConnectionDetails;
using webapi.Domain.PlayerDetails;
using static webapi.Domain.Statics.Enums;

namespace webapi.Controllers.GameHub
{
    public class GameHubController : IGameHubController
    {
        private readonly IGameController _gameController;
        private readonly IPlayerController _playerController;
        private readonly IPlayerConnectionController _playerConnectionController;

        public GameHubController(
            IGameController gameController,
            IPlayerController playerController,
            IPlayerConnectionController playerConnectionController
        ) {
            _gameController = gameController;
            _playerController = playerController;
            _playerConnectionController = playerConnectionController;
        }

        public async Task<bool> JoinGame(IPlayerConnectionDetails playerConnectionDetails)
        {
            return await _playerConnectionController.JoinGame(playerConnectionDetails);
        }

        public async Task OnBid(string connectionId, int bid)
        {
            var playerDetails = GetVerifiedPlayerDetails(connectionId);

            var playerName = playerDetails.GetPlayerName();

            var game = GetVerifiedGame(playerDetails);

            ValidatePhase(playerDetails, game, Phase.Bidding);

            var players = _playerController.GetPlayers(playerDetails);

            var biddingPlayer = players.Single(player => player.GetName() == playerName);

            var biddingPlayerIndex = biddingPlayer.GetIndex();
            var playerTurnIndex = game.GetPlayerTurnIndex();

            if (playerTurnIndex != biddingPlayerIndex)
            {
                await _playerConnectionController.MessageErrorToClient(playerDetails, "It is not that player's turn.");
                return;
            }

            var currentBid = game.GetCurrentBid();
            var bidIsPass = bid == -1;

            if (currentBid > 30 && (bid % 5 != 0))
            {
                await _playerConnectionController.MessageErrorToClient(playerDetails, "Above 30 you must bid up by increments of 5.");
                return;
            }
            else if (bid < currentBid)
            {
                await _playerConnectionController.MessageErrorToClient(playerDetails, "You must bid higher than the current bid.");
                return;
            }

            var unPassedPlayers = players.Where(player => !player.GetPassed()).OrderBy(player => player.GetIndex()).ToList();
            var teamIndex = biddingPlayer.GetTeamIndex();
            if (bidIsPass)
            {
                unPassedPlayers.Remove(biddingPlayer);

                await _playerConnectionController.MessageClients(playerDetails, $"{playerName} has passed!", teamIndex);
            }
            else
            {
                game.SetCurrentBid(bid);
                await _playerConnectionController.MessageClients(playerDetails, $"{playerName} has bid {bid}!", teamIndex);
            }

            biddingPlayer.Bid(bid);
            _playerController.UpdatePlayer(playerDetails, biddingPlayer);

            if (unPassedPlayers.Count == 1)
            {
                game.IncrementPhase();

                var newPlayerTurnIndex = unPassedPlayers.Single().GetIndex();
                game.SetPlayerTurnIndex(newPlayerTurnIndex);

                var newTeamIndex = (newPlayerTurnIndex == 0 || newPlayerTurnIndex == 2) ? 0 : 1;
                game.SetTookBidTeamIndex(newTeamIndex);

                var newPlayerName = unPassedPlayers.Single().GetName();
                await _playerConnectionController.MessageClients(playerDetails, $"{newPlayerName} has won the bid and is declaring trump!", newTeamIndex);

                _playerController.ResetBidState(playerDetails);
            }
            else
            {
                var index = game.GetPlayerTurnIndex() + 1;
                var nextPlayerIndex = -1;
                while (nextPlayerIndex == -1)
                {
                    if (unPassedPlayers.Any(player => player.GetIndex() == index))
                    {
                        nextPlayerIndex = index;
                        break;
                    }

                    index++;
                    if (index > 3)
                    {
                        index = 0;
                    }

                }

                game.SetPlayerTurnIndex(nextPlayerIndex);
            }

            _gameController.UpdateGame(playerDetails, game);

            await _playerConnectionController.UpdateClients(playerDetails);
        }

        public async Task DeclareTrump(string connectionId, int trumpSuitIndex)
        {
            var playerDetails = GetVerifiedPlayerDetails(connectionId);

            var playerName = playerDetails.GetPlayerName();

            var game = GetVerifiedGame(playerDetails);

            var playerTurnIndex = game.GetPlayerTurnIndex();

            ValidatePhase(playerDetails, game, Phase.Declaring_Trump);

            var players = _playerController.GetPlayers(playerDetails);

            if (trumpSuitIndex < 0 || trumpSuitIndex > 3)
            {
                await _playerConnectionController.MessageErrorToClient(playerDetails, "Invalid suit.");
                return;
            }

            var playerIndex = players.Single(player => player.GetName() == playerDetails.GetPlayerName()).GetIndex();
            if (playerIndex != playerTurnIndex)
            {
                await _playerConnectionController.MessageErrorToClient(playerDetails, "This player cannot declare trump.");
                return;
            }

            var trumpSuit = (Suit) trumpSuitIndex;
            game.SetTrumpSuit(trumpSuit);
            game.IncrementPhase();

            int teamOneScore = 0;
            int teamTwoScore = 0;

            foreach (var player in players)
            {
                var meldResult = new MeldResult(player.GetHand(), trumpSuit);

                var index = player.GetIndex();
                if (index == 0 || index == 2)
                {
                    teamOneScore += meldResult.MeldValue;
                }
                else
                {
                    teamTwoScore += meldResult.MeldValue;
                }
            }

            game.AddScore(0, teamOneScore);
            game.AddScore(1, teamTwoScore);

            var teamIndex = (playerTurnIndex == 0 || playerTurnIndex == 2) ? 0 : 1;

            game.AddRoundBidResult(trumpSuit, teamIndex, game.GetCurrentBid());


            var tookBidTeamIndex = (playerTurnIndex == 0 || playerTurnIndex == 2) ? 0 : 1;
            game.SetTookBidTeamIndex(tookBidTeamIndex);

            await _playerConnectionController.MessageClients(playerDetails, $"Trump is {trumpSuit}s!", teamIndex);

            _gameController.UpdateGame(playerDetails, game);
            _playerController.UpdatePlayers(playerDetails, players);

            await _playerConnectionController.UpdateClients(playerDetails);
        }

        public async Task SwapPlayerPosition(string connectionId, string swapPlayerName)
        {
            var playerDetails = GetVerifiedPlayerDetails(connectionId);

            var game = GetVerifiedGame(playerDetails);

            ValidatePhase(playerDetails, game, Phase.Initializing);

            var players = _playerController.GetPlayers(playerDetails);

            var player = players.Where(player => player.GetName() == playerDetails.GetPlayerName()).Single();
            if (player == null)
            {
                await _playerConnectionController.MessageErrorToClient(playerDetails, "Player does not exist.");
                return;
            }

            var swapPlayer = players.Where(player => player.GetName() == swapPlayerName).SingleOrDefault();
            if (swapPlayer == null)
            {
                await _playerConnectionController.MessageErrorToClient(playerDetails, "Swap player does not exist.");
                return;
            }

            _playerController.SwapPlayerIndices(player, swapPlayer);

            await _playerConnectionController.UpdateClients(playerDetails);
        }

        private IPlayerDetails GetVerifiedPlayerDetails(string connectionId)
        {
            return _playerConnectionController.GetPlayerDetails(connectionId)
                ?? throw new InvalidOperationException($"Player details were null for {connectionId}.");
        }

        private IGame GetVerifiedGame(IPlayerDetails playerDetails)
        {
            var game = _gameController.GetGame(playerDetails);
            if (game == null)
            {
                _playerConnectionController.MessageErrorToClient(playerDetails, "A game with that name does not exist.");
                throw new InvalidOperationException("A game with that name does not exist.");
            }
            else
            {
                return game;
            }
        }

        private void ValidatePhase(IPlayerDetails playerDetails, IGame game, Phase phase)
        {
            var gamePhase = game.GetPhase();
            if (gamePhase != phase)
            {
                _playerConnectionController.MessageErrorToClient(playerDetails, "Invalid game phase.");
                throw new InvalidOperationException("Invalid phase.");
            }
        }

        public void OnClientDisconnected(string connectionId)
        {
            _playerConnectionController.RemoveConnection(connectionId);
        }
    }
}
