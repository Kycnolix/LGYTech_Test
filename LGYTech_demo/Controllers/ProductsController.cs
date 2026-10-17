using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LGYTech_demo.Data;

namespace LGYTech_demo.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        [HttpGet]
        public ActionResult Index()
        {
            using (var db = new AppDbContext())
            {

                var products = db.Products
                .OrderBy(p => p.Id)
                .ToList();

                return View(products);

            }
        }
    }
}