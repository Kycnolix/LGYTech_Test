using LGYTech_demo.Data;
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
        public ActionResult Index(string search, int page =1)
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
    }
}