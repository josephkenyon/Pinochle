using Microsoft.EntityFrameworkCore;
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

            modelBuilder.Entity<Game>()
                .HasKey(game => game.Name);

            modelBuilder.Entity<Game>()
                .HasMany(game => game.Players)
                .WithOne(player => player.Game)
                .HasForeignKey(player => player.GameName);

            modelBuilder.Entity<PlayerConnectionData>()
                .HasKey(playerConnection => new { playerConnection.GameName, playerConnection.PlayerName });

            modelBuilder.Entity<Player>()
                .HasKey(player => new { player.GameName, player.Name });

            modelBuilder.Entity<Player>()
                .HasOne(player => player.Game)
                .WithMany(game => game.Players)
                .HasForeignKey(player => player.GameName);

            modelBuilder.Entity<Trick>()
                .HasKey(trick => trick.GameName);

            modelBuilder.Entity<Card>()
                .HasKey(card => new { card.GameName, card.Id, card.Suit, card.Rank });

            modelBuilder.Entity<Card>()
                .HasOne(card => card.Game)
                .WithMany(game => game.Cards)
                .HasForeignKey(card => card.GameName);
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerConnectionData> PlayerConnections { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
}
