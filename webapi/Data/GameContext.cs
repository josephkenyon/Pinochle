using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using webapi.Domain;

namespace webapi.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var converter = new ValueConverter<List<int>, string>(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => int.Parse(val)).ToList());

            modelBuilder.Entity<Game>()
                .HasKey(game => game.Name);

            modelBuilder.Entity<Game>()
                .Property(game => game.Scores)
                .HasConversion(converter);

            modelBuilder.Entity<Game>()
                .HasMany(game => game.Players)
                .WithOne(player => player.Game)
                .HasForeignKey(player => player.GameName);

            modelBuilder.Entity<Game>()
                .HasMany(game => game.MeldResults)
                .WithOne(meldResult => meldResult.Game)
                .HasForeignKey(meldResult => meldResult.GameName);

            modelBuilder.Entity<PlayerConnectionData>()
                .HasKey(playerConnection => new { playerConnection.GameName, playerConnection.PlayerName });

            modelBuilder.Entity<Player>()
                .HasKey(player => new { player.GameName, player.Name });

            modelBuilder.Entity<Player>()
                .HasOne(player => player.Game)
                .WithMany(game => game.Players)
                .HasForeignKey(player => player.GameName);

            modelBuilder.Entity<Card>()
                .HasKey(card => new { card.GameName, card.Id, card.Suit, card.Rank });

            modelBuilder.Entity<Card>()
                .HasOne(card => card.Game)
                .WithMany(game => game.Cards)
                .HasForeignKey(card => card.GameName);

            modelBuilder.Entity<MeldResult>()
                .HasKey(meldResult => new { meldResult.GameName, meldResult.PlayerIndex });
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerConnectionData> PlayerConnections { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<MeldResult> MeldResults { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
}
