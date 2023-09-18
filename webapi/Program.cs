using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Hubs;
using webapi.Repository.Game;
using webapi.Repository.Player;
using webapi.Repository.PlayerConnection;
using webapi.Repository.Trick;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<GameContext>(options => options.UseInMemoryDatabase("PinochleGames"));
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
