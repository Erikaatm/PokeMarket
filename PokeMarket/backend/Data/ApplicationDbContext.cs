using Microsoft.EntityFrameworkCore;
using PokeMarket.Models;

namespace PokeMarket.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Grading> Gradings { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CardTag> CardTags { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        // OnModelCreating sirve para configurar las relaciones entre los modelos, establecer resxtricciones,
        // definir claves compuestas y cotras configuraciones 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación muchos a muchos entre las tablas Card y Tag con la tabla intermedia CardTag

            // Definimos la clave primaria, la cual es combinada 
            modelBuilder.Entity<CardTag>()
                .HasKey(ct => new { ct.CardId, ct.TagId });

            // Relacion entre CardTag y Card
            modelBuilder.Entity<CardTag>()
                .HasOne(ct => ct.Card)
                .WithMany(c => c.CardTags)
                .HasForeignKey(ct => ct.CardId);

            // Relacion entre CardTag y Tag
            modelBuilder.Entity<CardTag>()
                .HasOne(ct => ct.Tag)
                .WithMany(t => t.CardTags)
                .HasForeignKey(ct => ct.TagId);


            // Configuración de relaciones del modelo Trade

            // Relacion entre Trade y Requester, es el usuario que inicia el intercambio
            modelBuilder.Entity<Trade>()
                .HasOne(t => t.Requester)
                .WithMany()
                .HasForeignKey(t => t.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Trade y Receiver, usuario que recibe el intercambio 
            modelBuilder.Entity<Trade>()
                .HasOne(t => t.Receiver)
                .WithMany()
                .HasForeignKey(t => t.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Trade y OfferedCard, la carta que ofrece el usuario que inicia el intercambio
            modelBuilder.Entity<Trade>()
                .HasOne(t => t.OfferedCard)
                .WithMany()
                .HasForeignKey(t => t.OfferedCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Trade y RequestedCard, la carta que quiere el usuario que inicia el intercambio
            modelBuilder.Entity<Trade>()
                .HasOne(t => t.RequestedCard)
                .WithMany()
                .HasForeignKey(t => t.RequestedCardId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configuración de relaciones para CartItem (Carrito)

            // Relación entre CartItem y User
            modelBuilder.Entity<CartItem>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre CartItem y Cards
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Card)
                .WithMany()
                .HasForeignKey(c => c.CardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
