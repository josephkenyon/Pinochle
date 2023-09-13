﻿using Microsoft.AspNetCore.SignalR;
using webapi.Data;
using webapi.Domain;
using static webapi.Domain.Enums;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace webapi.Hubs
{
    public class GameHub : Hub
    {
        private readonly ILogger<GameHub> _logger;
        private readonly GameContext _gameContext;

        public GameHub(ILogger<GameHub> logger, GameContext gameContext) {
            _logger = logger;
            _gameContext = gameContext;
        }
    
        public async Task JoinGame(PlayerConnectionData playerData)
        {
            var clientAlreadyExists = _gameContext.PlayerConnections.Any(client => client.GameName == playerData.GameName && client.PlayerName == playerData.PlayerName);
            if (clientAlreadyExists)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Someone is already playing in that game under than name.");
                Context.Abort();
                return;
            }

            var addToGroup = Groups.AddToGroupAsync(Context.ConnectionId, playerData.GameName);

            var game = _gameContext.Games.Where(game => game.Name == playerData.GameName).SingleOrDefault();

            var gameIsInitializing = game != null && game.Phase == Phase.Initializing;

            var playerExists = _gameContext.Players.Where(player => player.GameName == playerData.GameName && player.Name == playerData.PlayerName).Any();

            if (game == null)
            {
                game = new Game(playerData.GameName, playerData.PlayerName);
                gameIsInitializing = true;

                _gameContext.Games.Add(game);
            }
            else if (!playerExists)
            {
                if (gameIsInitializing)
                {
                    var players = _gameContext.Players.Where(player => player.GameName == playerData.GameName).ToList();

                    var player = new Player(playerData.PlayerName, playerData.GameName)
                    {
                        PlayerIndex = players.Count()
                    };

                    game.Players.Add(player);
                }
                else
                {
                    await Clients.Caller.SendAsync("ErrorMessage", "You cannot add a new player to a game already in progress.");
                    Context.Abort();
                    return;
                }
            }


            _gameContext.PlayerConnections.Add(new PlayerConnectionData(playerData.GameName, playerData.PlayerName, Context.ConnectionId));

            _gameContext.SaveChanges();

            addToGroup.Wait();

            await UpdateClients(game.Name);
        }

        private async Task UpdateClients(string gameName)
        {
            foreach (var connection in _gameContext.PlayerConnections.Where(playerConnection => playerConnection.GameName == gameName && playerConnection.Id != null))
            {
                try
                {
                    var playerState = GetPlayerState(connection);
                    await Clients.Client(connection.Id!).SendAsync("UpdatePlayerState", playerState);
                }
                catch (Exception ex)
                {
                    _logger.LogError(default, ex, "Error occurred while updating clients.");
                }
            }
        }

        private PlayerState GetPlayerState(PlayerConnectionData playerConnectionData)
        {
            var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).Single();
            var player = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName && player.Name == playerConnectionData.PlayerName).Single();

            var isPlayersTurn = player.PlayerIndex == game.PlayerTurnIndex;

            var playerState = new HeroPlayerState
            {
                Name = playerConnectionData.PlayerName
            };

            playerState.Hand.AddRange(_gameContext.Cards.Where(card => card.GameName == playerConnectionData.GameName && card.PlayerName == playerConnectionData.PlayerName).Select(card => card.CreateCardState()));

            var allyIndex = player.PlayerIndex + 2;
            if (allyIndex > 3)
            {
                allyIndex -= 4;
            }
            var ally = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName && player.PlayerIndex == allyIndex).SingleOrDefault();

            var leftOpponentIndex = player.PlayerIndex + 1;
            if (leftOpponentIndex > 3)
            {
                leftOpponentIndex -= 4;
            }
            var leftOpponent = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName && player.PlayerIndex == leftOpponentIndex).SingleOrDefault();

            var rightOpponentIndex = player.PlayerIndex + 3;
            if (rightOpponentIndex > 3)
            {
                rightOpponentIndex -= 4;
            }
            var rightOpponent = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName && player.PlayerIndex == rightOpponentIndex).SingleOrDefault();

            var isInitializing = game.Phase == Phase.Initializing;
            var isBidding = game.Phase == Phase.Bidding;
            var isMeld = game.Phase == Phase.Meld;
            var isDeclaringTrump = game.Phase == Phase.Declaring_Trump;

            var showReady = isInitializing || isMeld;

            playerState.ShowReady = showReady;
            playerState.IsReady = player.Ready;

            playerState.ShowBiddingBox = isBidding && isPlayersTurn;
            playerState.LastBid = game.CurrentBid;
            playerState.CurrentBid = game.CurrentBid + 1;

            playerState.ShowTrumpSelection = isDeclaringTrump && isPlayersTurn;

            if (ally != null)
            {
                playerState.AllyState.Name = ally.Name;

                playerState.AllyState.ShowReady = showReady;
                playerState.AllyState.IsReady = ally.Ready;

                playerState.AllyState.ShowLastBid = isBidding && ally.LastBid != 0;
                playerState.AllyState.LastBid = ally.LastBid;

                if (isMeld)
                {
                    var cards = _gameContext.Cards.Where(card => card.GameName == game.Name && card.PlayerName == ally.Name).ToList();
                    playerState.AllyState.DisplayedCards = new MeldResult(cards, game.TrumpSuit).MeldCards.Select(card => card.CreateCardState()).ToList();
                }
            }

            if (leftOpponent != null)
            {
                playerState.LeftOpponentState.Name = leftOpponent.Name;

                playerState.LeftOpponentState.ShowReady = showReady;
                playerState.LeftOpponentState.IsReady = leftOpponent.Ready;

                playerState.LeftOpponentState.ShowLastBid = isBidding && leftOpponent.LastBid != 0;
                playerState.LeftOpponentState.LastBid = leftOpponent.LastBid;

                if (isMeld)
                {
                    var cards = _gameContext.Cards.Where(card => card.GameName == game.Name && card.PlayerName == leftOpponent.Name).ToList();
                    playerState.LeftOpponentState.DisplayedCards = new MeldResult(cards, game.TrumpSuit).MeldCards.Select(card => card.CreateCardState()).ToList();
                }
            }

            if (rightOpponent != null)
            {
                playerState.RightOpponentState.Name = rightOpponent.Name;

                playerState.RightOpponentState.ShowReady = showReady;
                playerState.RightOpponentState.IsReady = rightOpponent.Ready;

                playerState.RightOpponentState.ShowLastBid = isBidding && rightOpponent.LastBid != 0;
                playerState.RightOpponentState.LastBid = rightOpponent.LastBid;

                if (isMeld)
                {
                    var cards = _gameContext.Cards.Where(card => card.GameName == game.Name && card.PlayerName == rightOpponent.Name).ToList();
                    playerState.RightOpponentState.DisplayedCards = new MeldResult(cards, game.TrumpSuit).MeldCards.Select(card => card.CreateCardState()).ToList();
                }
            }

            var playerNames = _gameContext.Players.Where(player => player.GameName == game.Name).Select(player => player.Name).ToList();

            var teamOneName = $"{playerNames.ElementAtOrDefault(0) ?? "Waiting for player"}/{playerNames.ElementAtOrDefault(2) ?? "Waiting for player"}";
            var teamTwoName = $"{playerNames.ElementAtOrDefault(1) ?? "Waiting for player"}/{playerNames.ElementAtOrDefault(3) ?? "Waiting for player"}";

            playerState.TeamOneScoreList = game.GetScoreLog(teamOneName, 0);
            playerState.TeamTwoScoreList = game.GetScoreLog(teamTwoName, 1);
            playerState.RoundBidResults = game.GetRoundBidResults();

            return playerState;
        }

        public async Task Bid(int bid)
        {
            var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

            var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
            if (game == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
                return;
            }

            var gameIsBidding = game.Phase == Phase.Bidding;
            if (!gameIsBidding)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Game is not currently in a bidding phase.");
                return;
            }

            var biddingPlayer = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName).Where(player => player.Name == playerConnectionData.PlayerName).Single();
            if (biddingPlayer == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "A player with that name does exist in this game.");
                return;
            }

            var playersTurn = biddingPlayer.PlayerIndex == game.PlayerTurnIndex;
            if (!playersTurn)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "It is not that player's turn.");
                return;
            }

            var bidIsPass = bid == -1;

            var validBid = bidIsPass || (bid > game.CurrentBid && (game.CurrentBid < 30 || (bid % 5 == 0)));
            if (!validBid)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "This is not a valid bid.");
                return;
            }

            var unPassedPlayers = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName && !player.Passed).OrderBy(player => player.PlayerIndex).ToList();
            if (bidIsPass)
            {
                biddingPlayer.Passed = true;
                unPassedPlayers.Remove(biddingPlayer);
            }
            else
            {
                game.CurrentBid = bid;
            }

            biddingPlayer.LastBid = bid;

            if (unPassedPlayers.Count == 1)
            {
                game.Phase = Phase.Declaring_Trump;
                game.PlayerTurnIndex = unPassedPlayers.Single().PlayerIndex;

                foreach (var player in _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName))
                {
                    player.LastBid = 0;
                }
            }
            else
            {
                var max = unPassedPlayers.Last().PlayerIndex;
                var nextPlayerIndex = game.PlayerTurnIndex + 1;
                if (nextPlayerIndex > max)
                {
                    nextPlayerIndex = unPassedPlayers.First().PlayerIndex;
                }

                game.PlayerTurnIndex = nextPlayerIndex;
            }

            _gameContext.SaveChanges();

            await UpdateClients(game.Name);
        }

        public async Task DeclareTrump(int trumpSuitIndex)
        {
            var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

            var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
            if (game == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
                return;
            }

            var gameIsDeclaringTrump = game.Phase == Phase.Declaring_Trump;
            if (!gameIsDeclaringTrump)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Game is not currently in a declaring trump phase.");
                return;
            }

            if (trumpSuitIndex < 0 || trumpSuitIndex > 3)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "That is not a valid suit.");
                return;
            }

            game.TrumpSuit = (Suit) trumpSuitIndex;
            game.Phase = Phase.Meld;

            int teamOneScore = 0;
            int teamTwoScore = 0;

            var players = _gameContext.Players.Where(player => player.GameName == game.Name).ToList();
            players.ForEach(player =>
            {
                var cards = _gameContext.Cards.Where(card => card.GameName == game.Name && card.PlayerName == player.Name).ToList();
                var meldResult = new MeldResult(cards, game.TrumpSuit);

                if (player.PlayerIndex == 0 || player.PlayerIndex == 2)
                {
                    teamOneScore += meldResult.MeldValue;
                }
                else
                {
                    teamTwoScore += meldResult.MeldValue;
                }
            });

            game.AddScore(0, teamOneScore);
            game.AddScore(1, teamTwoScore);

            var teamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;

            game.AddRoundBidResult(game.TrumpSuit, teamIndex, game.CurrentBid);

            _gameContext.SaveChanges();

            await UpdateClients(game.Name);
        }

        public async Task DeclareReady(bool ready)
        {
            var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

            var players = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName).ToList();

            var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
            if (game == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
                return;
            }

            var player = players.Where(player => player.GameName == playerConnectionData.GameName).Where(player => player.Name == playerConnectionData.PlayerName).Single();
            if (player == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Game is not currently in a declaring trump phase.");
                return;
            }

            var initializing = game.Phase == Phase.Initializing;
            var meld = game.Phase == Phase.Meld;
            var roundEnd = game.Phase == Phase.RoundEnd;
            if (!(initializing || meld || roundEnd))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Invalid phase for ready declaration.");
                return;
            }

            player.Ready = ready;

            if (players.Count == 4 && players.All(player => player.Ready))
            {
                if (initializing || roundEnd)
                {
                    StartNewRound(game, players);
                }
                else if (meld)
                {
                    game.Phase = Phase.Playing;
                }
                else
                {
                    throw new InvalidOperationException("Game was in an unexpected phase.");
                }

                players.ForEach(player => player.Ready = false);
            }

            _gameContext.SaveChanges();

            await UpdateClients(game.Name);
        }

        private void StartNewRound(Game game, List<Player> players)
        {
            DealCards(game.Name, players);

            game.Phase = Phase.Bidding;
            game.PlayerTurnIndex = game.IncrementAndGetStartingPlayerTurnIndex();
            game.CurrentBid = 14;
        }

        private void DealCards(string gameName, List<Player> players)
        {
            var index = 0;
            var rng = new Random();
            var shuffleCount = rng.Next(20, 30);
            var cards = _gameContext.Cards.Where(card => card.GameName == gameName).ToList();

            for (int i = 0; i < shuffleCount; i++)
            {
                cards = cards.OrderBy(card => rng.Next()).ToList();
            }

            foreach (var card in cards)
            {
                var player = players.Where(player => player.PlayerIndex == index).Single();
                card.PlayerName = player.Name;

                index++;
                if (index > 3)
                {
                    index = 0;
                }
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = _gameContext.PlayerConnections.SingleOrDefault(playerConnection => playerConnection.Id == Context.ConnectionId);

            if (connection != null)
            {
                _gameContext.PlayerConnections.Remove(connection);
                _gameContext.SaveChanges();
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
