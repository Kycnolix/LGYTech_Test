using System.Data.Entity;
using LGYTech_demo.Models;

namespace LGYTech_demo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=AppDbContext")
        {
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLines> OrderLines { get; set; }
        public DbSet<StockMovements> StockMovements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Products>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderLines>()
                .Property(ol => ol.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderLines>()
                .HasRequired(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .HasForeignKey(ol => ol.OrderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderLines>()
                .HasRequired(ol => ol.Product)
                .WithMany(p => p.OrderLines)
                .HasForeignKey(ol => ol.ProductId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StockMovements>()
                .HasRequired(sm => sm.Product)
                .WithMany(p => p.StockMovements)
                .HasForeignKey(sm => sm.ProductId)
                .WillCascadeOnDelete(false);
        }
    }
}