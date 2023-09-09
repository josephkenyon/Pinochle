using Microsoft.AspNetCore.Mvc;
using webapi.Data;
using webapi.Domain;
using static webapi.Domain.Enums;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly GameContext _context;

    public GameController(ILogger<GameController> logger, GameContext context)
    {
        _logger = logger;
        _context = context;
    }


    [HttpGet(Name = "GetWeatherForecast")]
    public ActionResult Get() //[FromHeader] string gameName, [FromHeader] string playerName
    {
        var gameName = "test";
        var playerName = "1";

        var game = _context.Games.Where(game => game.Name == gameName).SingleOrDefault();
        if (game == null)
        {
            var gameAlreadyExists = _context.Games.Where(game => game.Name == gameName).Any();
            if (gameAlreadyExists)
            {
                return StatusCode(400, "A game with that name already exists.");
            }
            else
            {
                game = new Game(gameName, playerName);

                _context.Games.Add(game);
                _context.SaveChanges();
            }

            //return StatusCode(400, "A game with that name does not exist.");
        }

        var player = _context.Players.Where(player => player.GameName == gameName && player.Name == playerName).Single();
        if (player == null)
        {
            return StatusCode(400, "A player with that name does exist in this game.");
        }

        var isPlayersTurn = player.PlayerIndex == game.PlayerTurnIndex;

        var playerState = new HeroPlayerState();
        playerState.Hand.AddRange(_context.Cards.Where(card => card.PlayerName == playerName));

        var allyIndex = player.PlayerIndex += 2;
        if (allyIndex > 3)
        {
            allyIndex -= 4;
        }
        var ally = _context.Players.Where(player => player.GameName == gameName && player.PlayerIndex == allyIndex).SingleOrDefault();

        var leftOpponentIndex = player.PlayerIndex += 1;
        if (leftOpponentIndex > 3)
        {
            leftOpponentIndex -= 4;
        }
        var leftOpponent = _context.Players.Where(player => player.GameName == gameName && player.PlayerIndex == leftOpponentIndex).SingleOrDefault();

        var rightOpponentIndex = player.PlayerIndex += 3;
        if (rightOpponentIndex > 3)
        {
            rightOpponentIndex -= 4;
        }
        var rightOpponent = _context.Players.Where(player => player.GameName == gameName && player.PlayerIndex == rightOpponentIndex).SingleOrDefault();

        Action showReady = () =>
        {
            playerState.ShowReady = true;
            playerState.IsReady = player.Ready;

            if (ally != null)
            {
                playerState.AllyState.ShowReady = true;
                playerState.AllyState.IsReady = ally.Ready;
            }

            if (leftOpponent != null)
            {
                playerState.LeftOpponentState.ShowReady = true;
                playerState.LeftOpponentState.IsReady = leftOpponent.Ready;
            }

            if (rightOpponent != null)
            {
                playerState.RightOpponentState.ShowReady = true;
                playerState.RightOpponentState.IsReady = rightOpponent.Ready;
            }
        };

        if (game.Phase == Phase.Initializing)
        {
            showReady();
        }
        else if (game.Phase == Phase.Bidding)
        {
            playerState.ShowBiddingBox = isPlayersTurn;

            if (ally != null)
            {
                playerState.AllyState.ShowLastBid = ally.LastBid != 0;
                playerState.AllyState.LastBid = ally.LastBid;
            }

            if (leftOpponent != null)
            {
                playerState.LeftOpponentState.ShowLastBid = leftOpponent.LastBid != 0;
                playerState.LeftOpponentState.LastBid = leftOpponent.LastBid;
            }

            if (rightOpponent != null)
            {
                playerState.RightOpponentState.ShowLastBid = rightOpponent.LastBid != 0;
                playerState.RightOpponentState.LastBid = rightOpponent.LastBid;
            }
        }
        else if (game.Phase == Phase.Declaring_Trump)
        {
            playerState.ShowTrumpSelection = isPlayersTurn;
        }
        else if (game.Phase == Phase.Meld)
        {
            showReady();

            var meldResults = _context.MeldResults.Where(meldResult => meldResult.GameName == gameName).ToList();

            if (ally != null)
            {
                playerState.AllyState.DisplayedCards = meldResults.Single(result => result.PlayerIndex == allyIndex).MeldCards;
            }

            if (leftOpponent != null)
            {
                playerState.LeftOpponentState.DisplayedCards = meldResults.Single(result => result.PlayerIndex == leftOpponentIndex).MeldCards;
            }

            if (rightOpponent != null)
            {
                playerState.RightOpponentState.DisplayedCards = meldResults.Single(result => result.PlayerIndex == rightOpponentIndex).MeldCards;
            }
        }

        return StatusCode(200, playerState);
    }

    //[Route("GetGame")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[HttpGet()]
    //public ActionResult GetGame([FromHeader] string gameName)
    //{
    //    var game = _context.Games.Where(game => game.Name == gameName).SingleOrDefault();
    //    if (game != null)
    //    {
    //        game.Players = _context.Players.Where(player => player.GameName == gameName).ToList();
    //        game.Cards = _context.Cards.Where(card => card.GameName == gameName).OrderBy(card => card.PlayerName).ToList();
    //        return StatusCode(200, game);
    //    }
    //    else
    //    {
    //        return StatusCode(400, "No game was found with that name.");
    //    }
    //}

    [Route("CreateGame")]
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult CreateGame([FromHeader] string gameName, [FromHeader] string hostingPlayerName)
    {
        var gameAlreadyExists = _context.Games.Where(game => game.Name == gameName).Any();
        if (gameAlreadyExists)
        {
            return StatusCode(400, "A game with that name already exists.");
        }
        else
        {
            var game = new Game(gameName, hostingPlayerName);

            _context.Games.Add(game);
            _context.SaveChanges();

            return StatusCode(201, "Game created successfully.");
        }
    }

    [Route("AddPlayer")]
    [HttpPost]
    public ActionResult AddPlayer([FromHeader] string gameName, [FromHeader] string playerName)
    {
        var game = _context.Games.Where(game => game.Name == gameName).SingleOrDefault();
        if (game == null)
        {
            return StatusCode(400, "A game with that name does not exist.");
        }

        var gameIsInitializing = game.Phase == Enums.Phase.Initializing;
        if (!gameIsInitializing)
        {
            return StatusCode(400, "Game has already started.");
        }

        var playerExists = _context.Players.Where(player => player.GameName == gameName).Where(player => player.Name == playerName).Any();
        if (playerExists)
        {
            return StatusCode(400, "A player with that name within that game already exists.");
        }

        var spaceForPlayer = _context.Players.Where(player => player.GameName == gameName).Count() < 4;
        if (!spaceForPlayer)
        {
            return StatusCode(400, "Game is already full.");
        }

        var players = _context.Players.Where(player => player.GameName == gameName).ToList();

        var player = new Player(playerName, gameName)
        {
            PlayerIndex = players.Count()
        };

        game.Players.Add(player);
        _context.SaveChanges();

        return StatusCode(201, "Player created successfully.");
    }

    private void DealCards(string gameName, List<Player> players)
    {
        var index = 0;
        var rng = new Random();
        var cards = _context.Cards.Where(card => card.GameName == gameName).OrderBy(card => rng.Next()).ToList();
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

    [Route("Bid")]
    [HttpPost]
    public ActionResult Bid([FromHeader] string gameName, [FromHeader] string playerName, [FromHeader] int bid)
    {
        var game = _context.Games.Where(game => game.Name == gameName).SingleOrDefault();
        if (game == null)
        {
            return StatusCode(400, "A game with that name does not exist.");
        }

        var gameIsBidding = game.Phase == Phase.Bidding;
        if (!gameIsBidding)
        {
            return StatusCode(400, "Game is not currently in a bidding phase.");
        }

        var biddingPlayer = _context.Players.Where(player => player.GameName == gameName).Where(player => player.Name == playerName).Single();
        if (biddingPlayer == null)
        {
            return StatusCode(400, "A player with that name does exist in this game.");
        }

        var playersTurn = biddingPlayer.PlayerIndex == game.PlayerTurnIndex;
        if (!playersTurn)
        {
            return StatusCode(400, "It is not that player's turn.");
        }

        var bidIsPass = bid == -1;

        var validBid = bidIsPass || (bid > game.CurrentBid && (game.CurrentBid < 30 || (bid % 5 == 0)));
        if (!validBid)
        {
            return StatusCode(400, "This is not a valid bid.");
        }

        var unPassedPlayers = _context.Players.Where(player => player.GameName == gameName && !player.Passed).OrderBy(player => player.PlayerIndex).ToList();
        if (bidIsPass)
        {
            biddingPlayer.Passed = true;
            unPassedPlayers.Remove(biddingPlayer);
        } else
        {
            game.CurrentBid = bid;
        }

        if (unPassedPlayers.Count == 1)
        {
            game.Phase = Phase.Declaring_Trump;
            game.PlayerTurnIndex = unPassedPlayers.Single().PlayerIndex;

            foreach (var player in _context.Players.Where(player => player.GameName == gameName))
            {
                player.LastBid = 0;
            }
        } else
        {
            var max = unPassedPlayers.Last().PlayerIndex;
            var nextPlayerIndex = game.PlayerTurnIndex + 1;
            if (nextPlayerIndex > max)
            {
                nextPlayerIndex = unPassedPlayers.First().PlayerIndex;
            }

            game.PlayerTurnIndex = nextPlayerIndex;
        }

        _context.SaveChanges();
        return StatusCode(200, "Bid completed successfully.");
    }

    [Route("DeclareTrump")]
    [HttpPost]
    public ActionResult DeclareTrump([FromHeader] string gameName, [FromHeader] Suit suit)
    {
        var game = _context.Games.Where(game => game.Name == gameName).SingleOrDefault();
        if (game == null)
        {
            return StatusCode(400, "A game with that name does not exist.");
        }

        var gameIsDeclaringTrump = game.Phase == Phase.Declaring_Trump;
        if (!gameIsDeclaringTrump)
        {
            return StatusCode(400, "Game is not currently in a declaring trump phase.");
        }

        game.TrumpSuit = suit;
        game.Phase = Phase.Meld;

        _context.SaveChanges();
        return StatusCode(200, "Trump declared successfully.");
    }

    [Route("DeclareReady")]
    [HttpPost]
    public ActionResult DeclareReady([FromHeader] string gameName, [FromHeader] string playerName, [FromHeader] bool ready)
    {
        var players = _context.Players.Where(player => player.GameName == gameName).ToList();

        var game = _context.Games.Where(game => game.Name == gameName).SingleOrDefault();
        if (game == null)
        {
            return StatusCode(400, "A game with that name does not exist.");
        }

        var player = players.Where(player => player.GameName == gameName).Where(player => player.Name == playerName).Single();
        if (player == null)
        {
            return StatusCode(400, "A player with that name does exist in this game.");
        }

        var initializing = game.Phase == Phase.Initializing;
        var meld = game.Phase == Phase.Meld;
        var roundEnd = game.Phase == Phase.RoundEnd;
        if (!(initializing || meld || roundEnd))
        {
            return StatusCode(400, "Invalid phase for ready declaration.");
        }

        player.Ready = ready;

        if (players.All(player => player.Ready))
        {
            if (initializing || roundEnd)
            {
                StartNewRound(gameName, game, players);
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

        _context.SaveChanges();
        return StatusCode(200, "Ready declared successfully.");
    }

    private void StartNewRound(string gameName, Game game, List<Player> players)
    {
        DealCards(gameName, players);

        players.ForEach(player =>
        {
            var playerCards = _context.Cards.Where(card => card.GameName == gameName && card.PlayerName == player.Name).ToList();
            var newMeldResult = new MeldResult(gameName, playerCards, game.TrumpSuit);

            var meldResult = _context.MeldResults.Where(meldResult => meldResult.GameName == gameName && meldResult.PlayerIndex == player.PlayerIndex).FirstOrDefault();
            if (meldResult != null)
            {
                meldResult = newMeldResult;
            }
            else
            {
                game.MeldResults.Add(newMeldResult);
            }
        });

        game.Phase = Phase.Bidding;
        game.PlayerTurnIndex = game.IncrementAndGetStartingPlayerTurnIndex();
        game.CurrentBid = 14;
    }
}
