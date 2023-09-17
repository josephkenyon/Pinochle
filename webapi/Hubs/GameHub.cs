using Microsoft.AspNetCore.SignalR;
using webapi.Data;
using webapi.Domain;
using static webapi.Domain.Enums;

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
                await MessageClient(playerData.GameName, playerData.PlayerName, "Someone is already playing in that game under than name.", MessageCode.Error, Context.ConnectionId);
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
                    await MessageClient(playerData.GameName, playerData.PlayerName, "You cannot add a new player to a game already in progress.", MessageCode.Error, Context.ConnectionId);
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

        private async Task MessageClients(string gameName, string messageContent, MessageCode messageCode)
        {
            foreach (var connection in _gameContext.PlayerConnections.Where(playerConnection => playerConnection.GameName == gameName && playerConnection.Id != null))
            {
                try
                {
                    var playerState = GetPlayerState(connection);
                    await Clients.Client(connection.Id!).SendAsync("SendMessage", new { content = messageContent, code = messageCode });
                }
                catch (Exception ex)
                {
                    _logger.LogError(default, ex, "Error occurred while messaging the clients.");
                }
            }
        }

        private async Task MessageClient(string gameName, string playerName, string messageContent, MessageCode messageCode, string? connectionId = null)
        {
            try
            {
                if (connectionId != null)
                {
                    await Clients.Client(connectionId).SendAsync("SendMessage", new { content = messageContent, code = messageCode });
                    return;
                }

                var connection = _gameContext.PlayerConnections.Single(playerConnection => playerConnection.GameName == gameName && playerConnection.PlayerName == playerName && playerConnection.Id != null);
                
                await Clients.Client(connection.Id!).SendAsync("SendMessage", new { content = messageContent, code = messageCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(default, ex, "Error occurred while messaging a client.");
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

            playerState.Hand.AddRange(_gameContext.Players.Single(player => player.GameName == game.Name && player.Name == playerConnectionData.PlayerName).GetHand());

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
            var isPlaying = game.Phase == Phase.Playing;
            var isDeclaringTrump = game.Phase == Phase.Declaring_Trump;
            var isRoundEnd = game.Phase == Phase.RoundEnd;
            var showLastBid = isBidding || isDeclaringTrump;
            var highLightPlayer = showLastBid || isPlaying;
            var showReady = isInitializing || isMeld || isRoundEnd;

            playerState.ShowSwapPosition = isInitializing;

            playerState.ShowTrumpIndicator = isMeld || isPlaying;
            playerState.ShowTricksTaken = isPlaying;

            playerState.TeamOneTricksTaken = game.GetCardIds(0).Count / 4;
            playerState.TeamTwoTricksTaken = game.GetCardIds(1).Count / 4;

            playerState.ShowReady = showReady;
            playerState.IsReady = player.Ready;

            playerState.TeamIndex = (player.PlayerIndex == 0 || player.PlayerIndex == 2) ? 0 : 1;

            playerState.ShowBiddingBox = isBidding && isPlayersTurn;
            playerState.CurrentBid = game.CurrentBid;
            playerState.LastBid = player.LastBid;
            playerState.ShowLastBid = showLastBid && playerState.LastBid != 0;

            var currentTrick = _gameContext.Tricks.SingleOrDefault(trick => trick.GameName == game.Name);
            var collectCards = currentTrick != null && currentTrick.GetCards().Count == 4;

            playerState.ShowPlayButton = isPlaying && isPlayersTurn && !collectCards;
            playerState.ShowCollectButton = isPlaying && isPlayersTurn && collectCards;

            playerState.ShowTrumpSelection = isDeclaringTrump && isPlayersTurn;

            if (ally != null)
            {
                playerState.AllyState.Name = ally.Name;

                playerState.AllyState.ShowReady = showReady;
                playerState.AllyState.IsReady = ally.Ready;

                playerState.AllyState.ShowLastBid = showLastBid && ally.LastBid != 0;
                playerState.AllyState.LastBid = ally.LastBid;

                playerState.AllyState.HighlightPlayer = highLightPlayer && game.PlayerTurnIndex == ally.PlayerIndex;

                if (isMeld)
                {
                    playerState.AllyState.DisplayedCards = new MeldResult(ally.GetHand(), game.TrumpSuit).MeldCards;
                }
            }

            if (leftOpponent != null)
            {
                playerState.LeftOpponentState.Name = leftOpponent.Name;

                playerState.LeftOpponentState.ShowReady = showReady;
                playerState.LeftOpponentState.IsReady = leftOpponent.Ready;

                playerState.LeftOpponentState.ShowLastBid = showLastBid && leftOpponent.LastBid != 0;
                playerState.LeftOpponentState.LastBid = leftOpponent.LastBid;

                playerState.LeftOpponentState.HighlightPlayer = highLightPlayer && game.PlayerTurnIndex == leftOpponent.PlayerIndex;

                if (isMeld)
                {
                    playerState.LeftOpponentState.DisplayedCards = new MeldResult(leftOpponent.GetHand(), game.TrumpSuit).MeldCards;
                }
            }

            if (rightOpponent != null)
            {
                playerState.RightOpponentState.Name = rightOpponent.Name;

                playerState.RightOpponentState.ShowReady = showReady;
                playerState.RightOpponentState.IsReady = rightOpponent.Ready;

                playerState.RightOpponentState.ShowLastBid = showLastBid && rightOpponent.LastBid != 0;
                playerState.RightOpponentState.LastBid = rightOpponent.LastBid;

                playerState.RightOpponentState.HighlightPlayer = highLightPlayer && game.PlayerTurnIndex == rightOpponent.PlayerIndex;

                if (isMeld)
                {
                    playerState.RightOpponentState.DisplayedCards = new MeldResult(rightOpponent.GetHand(), game.TrumpSuit).MeldCards;
                }
            }

            playerState.HighlightPlayer = highLightPlayer && game.PlayerTurnIndex == player.PlayerIndex;

            if (isMeld)
            {
                playerState.DisplayedCards = new MeldResult(player.GetHand(), game.TrumpSuit).MeldCards;
            }

            var playerNames = _gameContext.Players.Where(player => player.GameName == game.Name).OrderBy(player => player.PlayerIndex).Select(player => player.Name).ToList();

            var teamOneName = $"{playerNames.ElementAtOrDefault(0) ?? ""}/{playerNames.ElementAtOrDefault(2) ?? ""}";
            var teamTwoName = $"{playerNames.ElementAtOrDefault(1) ?? ""}/{playerNames.ElementAtOrDefault(3) ?? ""}";

            var trick = _gameContext.Tricks.Where(trick => trick.GameName == game.Name).SingleOrDefault();
            if (trick != null)
            {
                playerState.TrickState = Utils.GetTrickState(trick, player.PlayerIndex);
            }

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
                await MessageClient(game.Name, playerConnectionData.PlayerName, "This is not a valid bid.", MessageCode.Error);
                return;
            }

            var unPassedPlayers = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName && !player.Passed).OrderBy(player => player.PlayerIndex).ToList();
            var teamIndex = (biddingPlayer.PlayerIndex == 0 || biddingPlayer.PlayerIndex == 2) ? 0 : 1;
            if (bidIsPass)
            {
                biddingPlayer.Passed = true;
                unPassedPlayers.Remove(biddingPlayer);

                await MessageClients(game.Name, $"{biddingPlayer.Name} has passed!", teamIndex == 0 ? MessageCode.TeamOne : MessageCode.TeamTwo);
            }
            else
            {
                game.CurrentBid = bid;
                await MessageClients(game.Name, $"{biddingPlayer.Name} has bid {bid}!", teamIndex == 0 ? MessageCode.TeamOne : MessageCode.TeamTwo);
            }

            biddingPlayer.LastBid = bid;

            if (unPassedPlayers.Count == 1)
            {
                game.Phase = Phase.Declaring_Trump;
                game.PlayerTurnIndex = unPassedPlayers.Single().PlayerIndex;
                
                game.TookBidTeamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;

                var playerName = unPassedPlayers.Single().Name;
                await MessageClients(game.Name, $"{playerName} has won the bid and is declaring trump!", game.TookBidTeamIndex == 0 ? MessageCode.TeamOne : MessageCode.TeamTwo);

                foreach (var player in _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName))
                {
                    player.LastBid = 0;
                }
            }
            else
            {
                var index = game.PlayerTurnIndex + 1;
                var nextPlayerIndex = -1;
                while (nextPlayerIndex == -1)
                {
                    if (unPassedPlayers.Any(player => player.PlayerIndex == index))
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

            var player = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName && player.Name == playerConnectionData.PlayerName).Single();
            if (player.PlayerIndex != game.PlayerTurnIndex)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "This player cannot declare trump.");
                return;
            }

            game.TrumpSuit = (Suit) trumpSuitIndex;
            game.Phase = Phase.Meld;

            int teamOneScore = 0;
            int teamTwoScore = 0;

            var players = _gameContext.Players.Where(player => player.GameName == game.Name).ToList();
            players.ForEach(player =>
            {
                var meldResult = new MeldResult(player.GetHand(), game.TrumpSuit);

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


            game.TookBidTeamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;

            await MessageClients(game.Name, $"Trump is {game.TrumpSuit}s!", teamIndex == 0 ? MessageCode.TeamOne : MessageCode.TeamTwo);

            _gameContext.SaveChanges();

            await UpdateClients(game.Name);
        }

        public async Task SwapPlayerPosition(string playerName)
        {

            var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

            var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
            if (game == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
                return;
            }

            var initializing = game.Phase == Phase.Initializing;
            if (!initializing)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Game is not initializing.");
                return;
            }

            var players = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName).ToList();
            var player = players.Where(player => player.GameName == playerConnectionData.GameName).Where(player => player.Name == playerConnectionData.PlayerName).Single();
            if (player == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Player does not exist.");
                return;
            }

            var swapPlayer = players.Where(player => player.GameName == playerConnectionData.GameName).Where(player => player.Name == playerName).SingleOrDefault();
            if (swapPlayer == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Swap player does not exist.");
                return;
            }

            var index = player.PlayerIndex;

            player.PlayerIndex = swapPlayer.PlayerIndex;
            swapPlayer.PlayerIndex = index;

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
                await Clients.Caller.SendAsync("ErrorMessage", "Player does not exist.");
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

                    var leadingPlayer = _gameContext.Players.Single(player => player.GameName == game.Name && player.PlayerIndex == game.PlayerTurnIndex);
                    await MessageClients(game.Name, $"The hand has started! {leadingPlayer.Name} has the lead.", (leadingPlayer.PlayerIndex == 0 || leadingPlayer.PlayerIndex == 2) ? MessageCode.TeamOne : MessageCode.TeamTwo);
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
            DealCards(players);

            game.Phase = Phase.Bidding;
            game.PlayerTurnIndex = game.IncrementAndGetStartingPlayerTurnIndex();
            game.CurrentBid = 14;
            game.TeamOneCardsTakenIds = "";
            game.TeamTwoCardsTakenIds = "";

            var playerName = players.Single(player => player.PlayerIndex == game.PlayerTurnIndex).Name;

            var teamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;

            foreach (var player in players)
            {
                player.Passed = false;
                player.LastBid = 0;
            }

            _ = MessageClients(game.Name, $"The round has started! Bidding starts with {playerName}.", teamIndex == 0 ? MessageCode.TeamOne : MessageCode.TeamTwo);
        }

        private static void DealCards(List<Player> players)
        {
            var cards = new List<Card>();

            var index = 0;
            var rng = new Random();
            var shuffleCount = rng.Next(20, 30);

            Enum.GetValues<Suit>().ToList().ForEach(suit =>
            {
                Enum.GetValues<Rank>().ToList().ForEach(rank =>
                {
                    cards.Add(new Card(index++, suit, rank));
                    cards.Add(new Card(index++, suit, rank));
                });
            });

            for (int i = 0; i < shuffleCount; i++)
            {
                cards = cards.OrderBy(card => rng.Next()).ToList();
            }

            index = 0;
            var startingIndex = 0;
            foreach (var player in players)
            {
                var hand = new List<Card>();
                for (int i = startingIndex; i < startingIndex + 12; i++)
                {
                    hand.Add(cards[i]);
                }

                index++;
                if (index > 3)
                {
                    index = 0;
                }

                hand.Sort((a, b) =>
                {
                    if ((int) a.Suit > (int) b.Suit)
                    {
                        return 1;
                    }
                    else if ((int) a.Suit < (int) b.Suit)
                    {
                        return -1;
                    }

                    if ((int) a.Rank > (int) b.Rank)
                    {
                        return 1;
                    }
                    else if ((int) a.Rank < (int) b.Rank)
                    {
                        return -1;
                    }

                    return 0;
                });

                player.SetHand(hand);

                startingIndex += 12;
            }
        }

        public async Task CollectTrick(bool updateClients = true)
        {
            var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

            var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
            if (game == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
                return;
            }

            var currentTrick = _gameContext.Tricks.SingleOrDefault(trick => trick.GameName == game.Name);
            if (currentTrick == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "There's no trick to collect.");
                return;
            }

            var trickPlays = currentTrick.GetTrickPlays();
            if (trickPlays.Count != 4)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Trick is not complete.");
                return;
            }

            var teamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;
            game.AddCardIds(teamIndex, trickPlays.Select(play => play.Card.Id).ToList());

            var player = _gameContext.Players.Single(player => player.GameName == game.Name && player.Name == playerConnectionData.PlayerName);

            _gameContext.Tricks.Remove(currentTrick);

            if (player.GetHand().Count == 0)
            {
                game.Phase = Phase.RoundEnd;
                ProcessRoundEnd(game);

                var teamOneScore = game.GetTotalScore(0);
                var teamTwoScore = game.GetTotalScore(1);
                if (game.TookBidTeamIndex == 0 && teamOneScore > 100)
                {
                    game.Phase = Phase.Game_Over;
                }
                else if (game.TookBidTeamIndex == 1 && teamTwoScore > 100)
                {
                    game.Phase = Phase.Game_Over;
                }
                else if (teamOneScore < 0 && teamTwoScore > 100)
                {
                    game.Phase = Phase.Game_Over;
                }
                else if (teamTwoScore < 0 && teamOneScore > 100)
                {
                    game.Phase = Phase.Game_Over;
                }
            }

            _gameContext.SaveChanges();

            if (updateClients)
            {
                await UpdateClients(game.Name);
            }
        }

        private void ProcessRoundEnd(Game game)
        {
            var teamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;

            var teamOnePoints = game.GetCardIds(0).Select(Utils.GetCardFromId).Where(card => (int) card.Rank < 3).Count();
            var teamTwoPoints = game.GetCardIds(1).Select(Utils.GetCardFromId).Where(card => (int) card.Rank < 3).Count();

            if (teamIndex == 0)
            {
                teamOnePoints += 2;
            }
            else
            {
                teamTwoPoints += 2;
            }

            var someoneSet = false;

            var playerNames = _gameContext.Players.Where(player => player.GameName == game.Name).OrderBy(player => player.PlayerIndex).Select(player => player.Name).ToList();

            var teamOneName = $"{playerNames.ElementAtOrDefault(0) ?? ""} and {playerNames.ElementAtOrDefault(2) ?? ""}";
            var teamTwoName = $"{playerNames.ElementAtOrDefault(1) ?? ""} and {playerNames.ElementAtOrDefault(3) ?? ""}";

            if (game.TookBidTeamIndex == 0 && (teamOnePoints + game.GetLastMeld(0)) < game.CurrentBid)
            {
                game.NullifyMeld(0);
                game.AddScore(0, -game.CurrentBid);

                _ = MessageClients(game.Name, $"{teamOneName} have been set with only {teamOnePoints} points.", MessageCode.TeamOne);
                someoneSet = true;
            }
            else
            {
                game.AddScore(0, teamOnePoints);
            }

            if (game.TookBidTeamIndex == 1 && (teamTwoPoints + game.GetLastMeld(1)) < game.CurrentBid)
            {
                game.NullifyMeld(1);
                game.AddScore(1, -game.CurrentBid);
                _ = MessageClients(game.Name, $"{teamTwoName} have been set with only {teamTwoPoints} points.", MessageCode.TeamTwo);
                someoneSet = true;
            }
            else
            {
                game.AddScore(1, teamTwoPoints);
            }

            if (!someoneSet)
            {
                var teamName = game.TookBidTeamIndex == 0 ? teamOneName : teamTwoName;
                var teamPoints = game.TookBidTeamIndex == 0 ? teamOnePoints : teamTwoPoints;
                _ = MessageClients(game.Name, $"{teamName} has made their bid with {teamPoints} points.", game.TookBidTeamIndex == 0 ? MessageCode.TeamOne : MessageCode.TeamTwo);
            }
        }

        public async Task PlayCard(int sentCardId)
        {
            var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

            var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
            if (game == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
                return;
            }

            var gameIsPlaying = game.Phase == Phase.Playing;
            if (!gameIsPlaying)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Game is not currently in a playing phase.");
                return;
            }

            var player = _gameContext.Players.Single(player => player.GameName == game.Name && player.Name == playerConnectionData.PlayerName);
            if (game.PlayerTurnIndex != player.PlayerIndex)
            {
                await MessageClient(game.Name, player.Name, "Its not your turn.", MessageCode.Error);
                return;
            }

            var hand = player.GetHand();

            var cardId = sentCardId;
            if (hand.Count != 1 && cardId == -1)
            {
                await MessageClient(game.Name, player.Name, "You must select a card.", MessageCode.Error);
                return;
            }
            else if (hand.Count == 1)
            {
                cardId = hand.Single().Id;
            }

            var card = hand.Where(card => card.Id == cardId).FirstOrDefault();
            if (card == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "You do not have that card to play.");
                return;
            }

            var currentTrick = _gameContext.Tricks.SingleOrDefault(trick => trick.GameName == game.Name);
            if (currentTrick != null && currentTrick.GetCards().Count == 4)
            {
                await CollectTrick(false);
            }

            currentTrick = _gameContext.Tricks.SingleOrDefault(trick => trick.GameName == game.Name);
            if (currentTrick == null)
            {
                currentTrick = new Trick
                {
                    LedSuit = card.Suit,
                    TrumpSuit = game.TrumpSuit,
                    GameName = game.Name
                };
                _gameContext.Tricks.Add(currentTrick);
            }
            else
            {
                int index = 0;
                var playedCards = currentTrick.GetCards().Select(card => new TrickCard(card.Id, card.Suit, card.Rank, index++)).ToList();

                var validPlays = Utils.GetValidPlays(playedCards, hand, game.TrumpSuit);

                var canPlayCard = validPlays.Any(c => c.Suit == card.Suit && c.Rank == card.Rank);

                if (!canPlayCard)
                {
                    await MessageClient(game.Name, player.Name, "You can't play that card.", MessageCode.Error);
                    return;
                }
            }

            currentTrick.PlayCard(card, player.PlayerIndex);
            player.RemoveCard(card.Id);

            var cardPronoun = card.Rank == Rank.Ace ? "an" : "a";
            await MessageClients(game.Name, $"{player.Name} played {cardPronoun} {card.Rank} of {card.Suit}s!", (player.PlayerIndex == 0 || player.PlayerIndex == 2) ? MessageCode.TeamOne : MessageCode.TeamTwo);

            var trickPlays = currentTrick.GetTrickPlays();
            if (trickPlays.Count == 4)
            {
                int index = 0;
                var playedCards = currentTrick.GetCards().Select(card => new TrickCard(card.Id, card.Suit, card.Rank, index++)).ToList();

                var winningCardId = Utils.GetWinningCardId(currentTrick.TrumpSuit, playedCards);

                var winningCard = Utils.GetCardFromId(winningCardId);

                var winningCardPronoun = winningCard.Rank == Rank.Ace ? "an" : "a";

                var winningPlayerIndex = trickPlays.Single(card => card.Card.Id == winningCardId).PlayerIndex;

                game.PlayerTurnIndex = winningPlayerIndex;

                var winningPlayerName = _gameContext.Players.Single(player => player.GameName == game.Name && player.PlayerIndex == winningPlayerIndex).Name;
                await MessageClients(game.Name, $"{winningPlayerName} has won the trick with {winningCardPronoun} {winningCard.Rank} of {winningCard.Suit}s!", (winningPlayerIndex == 0 || winningPlayerIndex == 2) ? MessageCode.TeamOne : MessageCode.TeamTwo);
            }
            else
            {
                game.PlayerTurnIndex++;
                if (game.PlayerTurnIndex > 3)
                {
                    game.PlayerTurnIndex = 0;
                }
            }

            _gameContext.SaveChanges();

            await UpdateClients(game.Name);
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
