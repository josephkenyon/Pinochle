using Microsoft.AspNetCore.SignalR;
using webapi.Controllers.GameHub;
using webapi.Domain.PlayerConnectionDetails;
using webapi.Domain.PlayerDetails;

namespace webapi.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameHubController _gameHubController;

        public GameHub(IGameHubController gameHubController) {
            _gameHubController = gameHubController;
        }
    
        public async Task JoinGame(PlayerDetails playerDetails)
        {
            var playerConnectionDetails = new PlayerConnectionDetails(
                playerDetails.GameName,
                playerDetails.PlayerName,
                Context.ConnectionId
            );

            var success = await _gameHubController.JoinGame(playerConnectionDetails);

            if (!success)
            {
                Context.Abort();
            }
        }

        public async Task Bid(int bid)
        {
            await _gameHubController.OnBid(Context.ConnectionId, bid);
        }

        public async Task DeclareTrump(int trumpSuitIndex)
        {
            await _gameHubController.DeclareTrump(Context.ConnectionId, trumpSuitIndex);
        }

        public async Task SwapPlayerPosition(string playerName)
        {
            await _gameHubController.SwapPlayerPosition(Context.ConnectionId, playerName);
        }

        //public async Task DeclareReady(bool ready)
        //{

        //    var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

        //    var players = _gameContext.Players.Where(player => player.GameName == playerConnectionData.GameName).ToList();

        //    var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
        //    if (game == null)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
        //        return;
        //    }

        //    var player = players.Where(player => player.GameName == playerConnectionData.GameName).Where(player => player.Name == playerConnectionData.PlayerName).Single();
        //    if (player == null)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "Player does not exist.");
        //        return;
        //    }

        //    var initializing = game.Phase == Phase.Initializing;
        //    var meld = game.Phase == Phase.Meld;
        //    var roundEnd = game.Phase == Phase.RoundEnd;
        //    if (!(initializing || meld || roundEnd))
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "Invalid phase for ready declaration.");
        //        return;
        //    }

        //    player.Ready = ready;

        //    if (players.Count == 4 && players.All(player => player.Ready))
        //    {
        //        if (initializing || roundEnd)
        //        {
        //            StartNewRound(game, players);
        //        }
        //        else if (meld)
        //        {
        //            game.Phase = Phase.Playing;

        //            var leadingPlayer = _gameContext.Players.Single(player => player.GameName == game.Name && player.PlayerIndex == game.PlayerTurnIndex);
        //            await MessageClients(game.Name, $"The hand has started! {leadingPlayer.Name} has the lead.", (leadingPlayer.PlayerIndex == 0 || leadingPlayer.PlayerIndex == 2) ? MessageCode.TeamOne : MessageCode.TeamTwo);
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("Game was in an unexpected phase.");
        //        }

        //        players.ForEach(player => player.Ready = false);
        //    }

        //    _gameContext.SaveChanges();

        //    await UpdateClients(game.Name);
        //}

        //private void StartNewRound(Game game, List<Player> players)
        //{
        //    DealCards(players);

        //    game.Phase = Phase.Bidding;
        //    game.PlayerTurnIndex = game.IncrementAndGetStartingPlayerTurnIndex();
        //    game.CurrentBid = 14;
        //    game.TeamOneCardsTakenIds = "";
        //    game.TeamTwoCardsTakenIds = "";

        //    var playerName = players.Single(player => player.PlayerIndex == game.PlayerTurnIndex).Name;

        //    var teamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;

        //    foreach (var player in players)
        //    {
        //        player.Passed = false;
        //        player.LastBid = 0;
        //    }

        //    _ = MessageClients(game.Name, $"The round has started! Bidding starts with {playerName}.", teamIndex == 0 ? MessageCode.TeamOne : MessageCode.TeamTwo);
        //}

        //private static void DealCards(List<Player> players)
        //{
        //    var cards = new List<Card>();

        //    var index = 0;
        //    var rng = new Random();
        //    var shuffleCount = rng.Next(20, 30);

        //    Enum.GetValues<Suit>().ToList().ForEach(suit =>
        //    {
        //        Enum.GetValues<Rank>().ToList().ForEach(rank =>
        //        {
        //            cards.Add(new Card(index++, suit, rank));
        //            cards.Add(new Card(index++, suit, rank));
        //        });
        //    });

        //    for (int i = 0; i < shuffleCount; i++)
        //    {
        //        cards = cards.OrderBy(card => rng.Next()).ToList();
        //    }

        //    index = 0;
        //    var startingIndex = 0;
        //    foreach (var player in players)
        //    {
        //        var hand = new List<Card>();
        //        for (int i = startingIndex; i < startingIndex + 12; i++)
        //        {
        //            hand.Add(cards[i]);
        //        }

        //        index++;
        //        if (index > 3)
        //        {
        //            index = 0;
        //        }

        //        hand.Sort((a, b) =>
        //        {
        //            if ((int) a.Suit > (int) b.Suit)
        //            {
        //                return 1;
        //            }
        //            else if ((int) a.Suit < (int) b.Suit)
        //            {
        //                return -1;
        //            }

        //            if ((int) a.Rank > (int) b.Rank)
        //            {
        //                return 1;
        //            }
        //            else if ((int) a.Rank < (int) b.Rank)
        //            {
        //                return -1;
        //            }

        //            return 0;
        //        });

        //        player.SetHand(hand);

        //        startingIndex += 12;
        //    }
        //}

        //public async Task CollectTrick(bool updateClients = true)
        //{
        //    var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

        //    var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
        //    if (game == null)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
        //        return;
        //    }

        //    var currentTrick = _gameContext.Tricks.SingleOrDefault(trick => trick.GameName == game.Name);
        //    if (currentTrick == null)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "There's no trick to collect.");
        //        return;
        //    }

        //    var trickPlays = currentTrick.GetTrickPlays();
        //    if (trickPlays.Count != 4)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "Trick is not complete.");
        //        return;
        //    }

        //    var teamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;
        //    game.AddCardIds(teamIndex, trickPlays.Select(play => play.Card.Id).ToList());

        //    var player = _gameContext.Players.Single(player => player.GameName == game.Name && player.Name == playerConnectionData.PlayerName);

        //    _gameContext.Tricks.Remove(currentTrick);

        //    if (player.GetHand().Count == 0)
        //    {
        //        game.Phase = Phase.RoundEnd;
        //        ProcessRoundEnd(game);

        //        var teamOneScore = game.GetTotalScore(0);
        //        var teamTwoScore = game.GetTotalScore(1);
        //        if (game.TookBidTeamIndex == 0 && teamOneScore > 100)
        //        {
        //            game.Phase = Phase.Game_Over;
        //        }
        //        else if (game.TookBidTeamIndex == 1 && teamTwoScore > 100)
        //        {
        //            game.Phase = Phase.Game_Over;
        //        }
        //        else if (teamOneScore < 0 && teamTwoScore > 100)
        //        {
        //            game.Phase = Phase.Game_Over;
        //        }
        //        else if (teamTwoScore < 0 && teamOneScore > 100)
        //        {
        //            game.Phase = Phase.Game_Over;
        //        }
        //    }

        //    _gameContext.SaveChanges();

        //    if (updateClients)
        //    {
        //        await UpdateClients(game.Name);
        //    }
        //}

        //private void ProcessRoundEnd(Game game)
        //{
        //    var teamIndex = (game.PlayerTurnIndex == 0 || game.PlayerTurnIndex == 2) ? 0 : 1;

        //    var teamOnePoints = game.GetCardIds(0).Select(Utils.GetCardFromId).Where(card => (int) card.Rank < 3).Count();
        //    var teamTwoPoints = game.GetCardIds(1).Select(Utils.GetCardFromId).Where(card => (int) card.Rank < 3).Count();

        //    if (teamIndex == 0)
        //    {
        //        teamOnePoints += 2;
        //    }
        //    else
        //    {
        //        teamTwoPoints += 2;
        //    }

        //    var someoneSet = false;

        //    var playerNames = _gameContext.Players.Where(player => player.GameName == game.Name).OrderBy(player => player.PlayerIndex).Select(player => player.Name).ToList();

        //    var teamOneName = $"{playerNames.ElementAtOrDefault(0) ?? ""} and {playerNames.ElementAtOrDefault(2) ?? ""}";
        //    var teamTwoName = $"{playerNames.ElementAtOrDefault(1) ?? ""} and {playerNames.ElementAtOrDefault(3) ?? ""}";

        //    if (game.TookBidTeamIndex == 0 && (teamOnePoints + game.GetLastMeld(0)) < game.CurrentBid)
        //    {
        //        game.NullifyMeld(0);
        //        game.AddScore(0, -game.CurrentBid);

        //        _ = MessageClients(game.Name, $"{teamOneName} have been set with only {teamOnePoints} points.", MessageCode.TeamOne);
        //        someoneSet = true;
        //    }
        //    else
        //    {
        //        game.AddScore(0, teamOnePoints);
        //    }

        //    if (game.TookBidTeamIndex == 1 && (teamTwoPoints + game.GetLastMeld(1)) < game.CurrentBid)
        //    {
        //        game.NullifyMeld(1);
        //        game.AddScore(1, -game.CurrentBid);
        //        _ = MessageClients(game.Name, $"{teamTwoName} have been set with only {teamTwoPoints} points.", MessageCode.TeamTwo);
        //        someoneSet = true;
        //    }
        //    else
        //    {
        //        game.AddScore(1, teamTwoPoints);
        //    }

        //    if (!someoneSet)
        //    {
        //        var teamName = game.TookBidTeamIndex == 0 ? teamOneName : teamTwoName;
        //        var teamPoints = game.TookBidTeamIndex == 0 ? teamOnePoints : teamTwoPoints;
        //        _ = MessageClients(game.Name, $"{teamName} has made their bid with {teamPoints} points.", game.TookBidTeamIndex == 0 ? MessageCode.TeamOne : MessageCode.TeamTwo);
        //    }
        //}

        //public async Task PlayCard(int sentCardId)
        //{
        //    var playerConnectionData = _gameContext.PlayerConnections.Single(connection => connection.Id == Context.ConnectionId);

        //    var game = _gameContext.Games.Where(game => game.Name == playerConnectionData.GameName).SingleOrDefault();
        //    if (game == null)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "A game with that name does not exist.");
        //        return;
        //    }

        //    var gameIsPlaying = game.Phase == Phase.Playing;
        //    if (!gameIsPlaying)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "Game is not currently in a playing phase.");
        //        return;
        //    }

        //    var player = _gameContext.Players.Single(player => player.GameName == game.Name && player.Name == playerConnectionData.PlayerName);
        //    if (game.PlayerTurnIndex != player.PlayerIndex)
        //    {
        //        await MessageClient(game.Name, player.Name, "Its not your turn.", MessageCode.Error);
        //        return;
        //    }

        //    var hand = player.GetHand();

        //    var cardId = sentCardId;
        //    if (hand.Count != 1 && cardId == -1)
        //    {
        //        await MessageClient(game.Name, player.Name, "You must select a card.", MessageCode.Error);
        //        return;
        //    }
        //    else if (hand.Count == 1)
        //    {
        //        cardId = hand.Single().Id;
        //    }

        //    var card = hand.Where(card => card.Id == cardId).FirstOrDefault();
        //    if (card == null)
        //    {
        //        await Clients.Caller.SendAsync("ErrorMessage", "You do not have that card to play.");
        //        return;
        //    }

        //    var currentTrick = _gameContext.Tricks.SingleOrDefault(trick => trick.GameName == game.Name);
        //    if (currentTrick != null && currentTrick.GetCards().Count == 4)
        //    {
        //        await CollectTrick(false);
        //    }

        //    currentTrick = _gameContext.Tricks.SingleOrDefault(trick => trick.GameName == game.Name);
        //    if (currentTrick == null)
        //    {
        //        currentTrick = new Trick
        //        {
        //            LedSuit = card.Suit,
        //            TrumpSuit = game.TrumpSuit,
        //            GameName = game.Name
        //        };
        //        _gameContext.Tricks.Add(currentTrick);
        //    }
        //    else
        //    {
        //        int index = 0;
        //        var playedCards = currentTrick.GetCards().Select(card => new TrickCard(card.Id, card.Suit, card.Rank, index++)).ToList();

        //        var validPlays = Utils.GetValidPlays(playedCards, hand, game.TrumpSuit);

        //        var canPlayCard = validPlays.Any(c => c.Suit == card.Suit && c.Rank == card.Rank);

        //        if (!canPlayCard)
        //        {
        //            await MessageClient(game.Name, player.Name, "You can't play that card.", MessageCode.Error);
        //            return;
        //        }
        //    }

        //    currentTrick.PlayCard(card, player.PlayerIndex);
        //    player.RemoveCard(card.Id);

        //    var cardPronoun = card.Rank == Rank.Ace ? "an" : "a";
        //    await MessageClients(game.Name, $"{player.Name} played {cardPronoun} {card.Rank} of {card.Suit}s!", (player.PlayerIndex == 0 || player.PlayerIndex == 2) ? MessageCode.TeamOne : MessageCode.TeamTwo);

        //    var trickPlays = currentTrick.GetTrickPlays();
        //    if (trickPlays.Count == 4)
        //    {
        //        int index = 0;
        //        var playedCards = currentTrick.GetCards().Select(card => new TrickCard(card.Id, card.Suit, card.Rank, index++)).ToList();

        //        var winningCardId = Utils.GetWinningCardId(currentTrick.TrumpSuit, playedCards);

        //        var winningCard = Utils.GetCardFromId(winningCardId);

        //        var winningCardPronoun = winningCard.Rank == Rank.Ace ? "an" : "a";

        //        var winningPlayerIndex = trickPlays.Single(card => card.Card.Id == winningCardId).PlayerIndex;

        //        game.PlayerTurnIndex = winningPlayerIndex;

        //        var winningPlayerName = _gameContext.Players.Single(player => player.GameName == game.Name && player.PlayerIndex == winningPlayerIndex).Name;
        //        await MessageClients(game.Name, $"{winningPlayerName} has won the trick with {winningCardPronoun} {winningCard.Rank} of {winningCard.Suit}s!", (winningPlayerIndex == 0 || winningPlayerIndex == 2) ? MessageCode.TeamOne : MessageCode.TeamTwo);
        //    }
        //    else
        //    {
        //        game.PlayerTurnIndex++;
        //        if (game.PlayerTurnIndex > 3)
        //        {
        //            game.PlayerTurnIndex = 0;
        //        }
        //    }

        //    _gameContext.SaveChanges();

        //    await UpdateClients(game.Name);
        //}

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _gameHubController.OnClientDisconnected(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
