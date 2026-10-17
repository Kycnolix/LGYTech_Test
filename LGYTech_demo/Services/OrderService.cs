using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using LGYTech_demo.Data;
using LGYTech_demo.Models;
using LGYTech_demo.ViewModels;

namespace LGYTech_demo.Services
{
    public class OrderService
    {
        public int CreateOrder(OrderCreateViewModel model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.CustomerName) ||
                model.Items == null ||
                model.Items.Count == 0)
            {
                throw new ArgumentException("Invalid order information.");
            }

            if (model.Items.Any(item =>
                item.ProductId <= 0 || item.Quantity <= 0))
            {
                throw new ArgumentException(
                    "Product and quantity information must be valid.");
            }

            if (model.Items.GroupBy(item => item.ProductId)
                           .Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "The same product cannot be added more than once.");
            }

            using (var db = new AppDbContext())
            using (var transaction =
                db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var items = model.Items
                        .OrderBy(item => item.ProductId)
                        .ToList();

                    var productIds = items
                        .Select(item => item.ProductId)
                        .ToList();

                    var products = db.Products
                        .AsNoTracking()
                        .Where(product => productIds.Contains(product.Id))
                        .ToDictionary(product => product.Id);

                    if (products.Count != productIds.Count)
                    {
                        throw new InvalidOperationException(
                            "One or more products could not be found.");
                    }

                    decimal totalAmount = 0;
                    var orderLines = new List<OrderLines>();
                    var stockMovements = new List<StockMovements>();

                    foreach (var item in items)
                    {
                        var product = products[item.ProductId];

                        // Atomically check and deduct available stock
                        int affectedRows = db.Database.ExecuteSqlCommand(
                            @"UPDATE dbo.Products
                              SET Stock = Stock - @Quantity
                              WHERE Id = @ProductId
                                AND Stock >= @Quantity",
                            new SqlParameter("@Quantity", item.Quantity),
                            new SqlParameter("@ProductId", item.ProductId)
                        );

                        if (affectedRows != 1)
                        {
                            throw new InvalidOperationException(
                                "Insufficient stock for product: " +
                                product.Name);
                        }

                        decimal unitPrice = product.Price;

                        totalAmount += unitPrice * item.Quantity;

                        orderLines.Add(new OrderLines
                        {
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitPrice = unitPrice
                        });

                        stockMovements.Add(new StockMovements
                        {
                            ProductId = product.Id,
                            Change = -item.Quantity,
                            Reason = "Order created",
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    var order = new Order
                    {
                        CustomerName = model.CustomerName.Trim(),
                        CreatedAt = DateTime.UtcNow,
                        Total = totalAmount,
                        OrderLines = orderLines
                    };

                    db.Orders.Add(order);

                    foreach (var movement in stockMovements)
                    {
                        db.StockMovements.Add(movement);
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return order.Id;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}