namespace LGYTech_demo.Migrations
{
    using LGYTech_demo.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LGYTech_demo.Data.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LGYTech_demo.Data.AppDbContext context)
        {

            var products = new[]
{
                new Products
                {
                    Name = "Kalem",
                    Sku = "KLM-001",
                    Price = 10m,
                    Stock = 100
                },
                new Products
                {
                    Name = "Defter",
                    Sku = "DFT-001",
                    Price = 35m,
                    Stock = 50
                },
                new Products
                {
                    Name = "Silgi",
                    Sku = "SLG-001",
                    Price = 5m,
                    Stock = 3
                },
                new Products
                {
                    Name = "Cetvel",
                    Sku = "CTV-001",
                    Price = 15m,
                    Stock = 1
                },
                new Products
                {
                    Name = "Çanta",
                    Sku = "CNT-001",
                    Price = 250m,
                    Stock = 0
                }
            };

            foreach (var product in products)
            {
                bool exists = context.Products
                    .Any(p => p.Sku == product.Sku);

                if (!exists)
                {
                    context.Products.Add(product);
                }
            }

            context.SaveChanges();

        }
    }
}
