using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using webapi.Domain.GameDetails;
using webapi.Domain.MessageDetails;
using webapi.Domain.PlayerConnectionDetails;
using webapi.Domain.PlayerDetails;
using webapi.Hubs;
using webapi.Repositories.Game;
using webapi.Repositories.Player;
using webapi.Repositories.PlayerConnection;
using static webapi.Domain.Enums;

namespace webapi.Controllers.PlayerConnection
{
    public class PlayerConnectionController : Controller, IPlayerConnectionController
    {
        private readonly ILogger<PlayerConnectionController> _logger;
        private readonly IPlayerStateController _playerStateController;
        private readonly IPlayerConnectionRepository _playerConnectionRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IHubContext<GameHub> _hubContext;

        public PlayerConnectionController(
            ILogger<PlayerConnectionController> logger,
            IPlayerStateController playerStateController,
            IPlayerConnectionRepository playerConnectionRepository,
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            IHubContext<GameHub> hubContext
        )
        {
            _logger = logger;
            _playerStateController = playerStateController;
            _playerConnectionRepository = playerConnectionRepository;
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _hubContext = hubContext;
        }

        public async Task<bool> JoinGame(IPlayerConnectionDetails playerConnectionDetails)
        {
            var gameName = playerConnectionDetails.GetGameName();
            var playerName = playerConnectionDetails.GetPlayerName();
            var connectionId = playerConnectionDetails.GetConnectionId();

            MessageDetails getMessageDetails(string content) => new(new GameDetails(gameName), MessageCode.Error, content);

            var existingClient = _playerConnectionRepository.GetPlayerConnection(playerConnectionDetails);
            if (existingClient != null)
            {
                await MessageClient(connectionId, getMessageDetails("Someone is already playing in that game under than name."));
                return false;
            }

            var game = _gameRepository.GetGame(gameName);

            var gameIsInitializing = game != null && game.GetPhase() == Phase.Initializing;

            var existingPlayer = _playerRepository.GetPlayer(playerConnectionDetails);

            if (game == null)
            {
                _gameRepository.CreateGame(gameName);
                _playerRepository.CreatePlayer(playerConnectionDetails);
            }
            else if (existingPlayer == null)
            {
                if (gameIsInitializing)
                {
                    _playerRepository.CreatePlayer(playerConnectionDetails);
                }
                else
                {
                    await MessageClient(connectionId, getMessageDetails("You cannot add a new player to a game already in progress."));
                    return false;
                }
            }

            _playerConnectionRepository.CreatePlayerConnection(playerConnectionDetails);

            await UpdateClients(playerConnectionDetails);

            return true;
        }

        public async Task UpdateClients(IGameDetails gameDetails)
        {
            foreach (var connection in _playerConnectionRepository.GetPlayerConnections(gameDetails))
            {
                try
                {
                    var playerState = _playerStateController.GetPlayerState(connection);
                    await _hubContext.Clients.Client(connection.GetConnectionId()).SendAsync("UpdatePlayerState", playerState);
                }
                catch (Exception ex)
                {
                    _logger.LogError(default, ex, "Error occurred while updating clients.");
                }
            }
        }

        public async Task MessageClients(IMessageDetails messageDetails)
        {
            var code = messageDetails.GetCode();
            var content = messageDetails.GetContent();

            foreach (var connection in _playerConnectionRepository.GetPlayerConnections(messageDetails))
            {
                try
                {
                    var playerState = _playerStateController.GetPlayerState(connection);
                    await _hubContext.Clients.Client(connection.GetConnectionId()).SendAsync("SendMessage", new { content, code });
                }
                catch (Exception ex)
                {
                    _logger.LogError(default, ex, "Error occurred while messaging the clients.");
                }
            }
        }

        public async Task MessageClient(string connectionId, IMessageDetails messageDetails)
        {
            var code = messageDetails.GetCode();
            var content = messageDetails.GetContent();

            try
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("SendMessage", new { content, code });
            }
            catch (Exception ex)
            {
                _logger.LogError(default, ex, "Error occurred while messaging a client.");
            }
        }

        public async Task MessageClient(IMessageDetails messageDetails, string playerName)
        {
            var code = messageDetails.GetCode();
            var content = messageDetails.GetContent();

            var playerDetails = new PlayerDetails(messageDetails.GetGameName(), playerName);

            try
            {
                var connection = _playerConnectionRepository.GetPlayerConnection(playerDetails);

                if (connection != null)
                {
                    await _hubContext.Clients.Client(connection.GetConnectionId()).SendAsync("SendMessage", new { content, code });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(default, ex, "Error occurred while messaging a client.");
            }
        }
    }
}
