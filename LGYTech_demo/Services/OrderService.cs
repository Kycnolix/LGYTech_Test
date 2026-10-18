using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using LGYTech_demo.Data;
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

            if (model.CustomerName.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "Customer name cannot exceed 50 characters.");
            }

            if (model.Items.Any(item =>
                item.ProductId <= 0 || item.Quantity <= 0))
            {
                throw new ArgumentException(
                    "Invalid product or quantity.");
            }
            


            if (model.Items.GroupBy(item => item.ProductId)
                           .Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "Duplicate products are not allowed.");
            }

            // Convert order items into a table-valued parameter
            var itemsTable = new DataTable();
            itemsTable.Columns.Add("ProductId", typeof(int));
            itemsTable.Columns.Add("Quantity", typeof(int));

            foreach (var item in model.Items)
            {
                itemsTable.Rows.Add(item.ProductId, item.Quantity);
            }

            var customerParameter = new SqlParameter(
                "@CustomerName", SqlDbType.NVarChar, 50)
            {
                Value = model.CustomerName.Trim()
            };

            var itemsParameter = new SqlParameter(
                "@Items", SqlDbType.Structured)
            {
                TypeName = "dbo.OrderItemType",
                Value = itemsTable
            };

            var orderIdParameter = new SqlParameter(
                "@OrderId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };




            using (var db = new AppDbContext())
            {
                foreach (var item in model.Items)
                {
                    var product = db.Products
                        .FirstOrDefault(p => p.Id == item.ProductId);

                    if (product == null)
                    {
                        throw new ArgumentException("Product not found.");
                    }

                    
                    item.ProductName = product.Name;
                    item.Stock = product.Stock;

                   
                    if (item.Stock < item.Quantity)
                    {
                        throw new ArgumentException(
                            $"Insufficient stock for '{item.ProductName}'. " +
                            $"Available: {item.Stock}, " +
                            $"Requested: {item.Quantity}."
                        );
                    }
                }

                db.Database.ExecuteSqlCommand(
                    "EXEC dbo.CreateOrder @CustomerName, @Items, @OrderId OUTPUT",
                    customerParameter,
                    itemsParameter,
                    orderIdParameter
                );
            }

            return Convert.ToInt32(orderIdParameter.Value);
        }
    }
}