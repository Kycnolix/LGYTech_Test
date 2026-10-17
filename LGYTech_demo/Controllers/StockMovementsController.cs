using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LGYTech_demo.Data;

namespace LGYTech_demo.Controllers
{
    public class StockMovementsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            using (var db = new AppDbContext())
            {
                // Load stock movements with related products
                var movements = db.StockMovements
                    .Include(m => m.Product)
                    .OrderByDescending(m => m.CreatedAt)
                    .ThenByDescending(m => m.Id)
                    .ToList();

                return View(movements);
            }
        }
    }
}