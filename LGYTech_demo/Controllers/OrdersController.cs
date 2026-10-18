using System;
using System.Web.Mvc;
using LGYTech_demo.Services;
using LGYTech_demo.ViewModels;
using System.Linq;
using LGYTech_demo.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace LGYTech_demo.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderService _orderService = new OrderService();

        [HttpGet]
        public ActionResult Index()
        {
            using (var db = new AppDbContext())
            {
                // Load orders and their related products
                var orders = db.Orders
                    .Include(o => o.OrderLines.Select(l => l.Product))
                    .OrderByDescending(o => o.CreatedAt)
                    .ThenByDescending(o => o.Id)
                    .ToList();

                return View(orders);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            using (var db = new AppDbContext())
            {
                ViewBag.Products = db.Products
                    .OrderBy(p => p.Name)
                    .Select(p => new ProductOptionViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Stock = p.Stock
                    })
                    .ToList();
            }

            return View(new OrderCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(OrderCreateViewModel model)
        {
            if (model == null || !ModelState.IsValid ||
                model.Items == null || model.Items.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid order information."
                });
            }

            try
            {
                int orderId = _orderService.CreateOrder(model);

                return Json(new
                {
                    success = true,
                    message = $"Order #{orderId} created successfully.",
                    orderId = orderId
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (SqlException ex)
            {
                if (ex.Number >= 50001 && ex.Number <= 50005)
                {
                    return Json(new
                    {
                        success = false,
                        message = ex.Message
                    });
                }

                // Unexpected database errors must not be exposed to clients
                throw;
            }
        }
    }
}