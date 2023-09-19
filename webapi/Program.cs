using Microsoft.EntityFrameworkCore;
using webapi.Controllers.Game;
using webapi.Controllers.GameHub;
using webapi.Controllers.Player;
using webapi.Controllers.PlayerConnection;
using webapi.Controllers.PlayerState;
using webapi.Data;
using webapi.Hubs;
using webapi.Repositories.Game;
using webapi.Repositories.Player;
using webapi.Repositories.PlayerConnection;
using webapi.Repositories.Trick;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<GameContext>(options => options.UseInMemoryDatabase("PinochleGames"));

builder.Services.AddScoped<IGameController, GameController>();
builder.Services.AddScoped<IPlayerController, PlayerController>();
builder.Services.AddScoped<IPlayerConnectionController, PlayerConnectionController>();
builder.Services.AddScoped<IPlayerStateController, PlayerStateController>();
builder.Services.AddScoped<IGameHubController, GameHubController>();

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerConnectionRepository, PlayerConnectionRepository>();
builder.Services.AddScoped<ITrickRepository, TrickRepository>();

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

app.UseCors(x => x.AllowAnyMethod()
           .AllowAnyHeader()
           .SetIsOriginAllowed(origin => true)
           .AllowCredentials());

app.UseAuthorization();

app.MapHub<GameHub>("/game");
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
