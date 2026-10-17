using LGYTech_demo.Data;
using LGYTech_demo.Models;
using LGYTech_demo.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace LGYTech_demo.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        [HttpGet]
        public ActionResult Index(string search, int page = 1)
        {
            const int pageSize = 10;
            using (var db = new AppDbContext())
            {
                var query = db.Products.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(p => p.Name.Contains(search));
                }
                int totalCount = query.Count();

                page = Math.Max(1, page);
                var products = query
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.Search = search;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages =
                    (int)Math.Ceiling((double)totalCount / pageSize);

                return View(products);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new ProductFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductFormViewModel model)
        {
            if (model.Price <= 0)
            {
                ModelState.AddModelError(
                    "Price",
                    "need to be more then 0."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new AppDbContext())
            {
                var product = new Products
                {
                    Name = model.Name,
                    Price = model.Price,
                    Stock = model.Stock,
                    Sku = GenerateSku()
                };

                db.Products.Add(product);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var db = new AppDbContext())
            {
                var product = db.Products.Find(id);

                if (product == null)
                {
                    return HttpNotFound();
                }

                var model = new ProductFormViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock
                };

                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductFormViewModel model)
        {
            if (model.Price <= 0)
            {
                ModelState.AddModelError(
                    "Price",
                    "Fiyat sıfırdan büyük olmalıdır."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new AppDbContext())
            {
                var product = db.Products.Find(model.Id);

                if (product == null)
                {
                    return HttpNotFound();
                }

                product.Name = model.Name;
                product.Price = model.Price;
                product.Stock = model.Stock;

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private static string GenerateSku()
        {
            return "PRD-" + Guid.NewGuid()
                .ToString("N")
                .ToUpperInvariant();
        }
    }
}