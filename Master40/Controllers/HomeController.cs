using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Master40.DB.Data.Context;
using Master40.DB.Data.Initializer;
using System;

namespace Master40.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductionDomainContext _context;
        public HomeController(ProductionDomainContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View();
        }


        [HttpGet("[Controller]/ReloadDb/{size}")]
        public async Task<IActionResult> ReloadDb(string size)
        {
            await Task.Run(() =>
                {
                    switch (size)
                    {
                        case "small":
                            _context.Database.EnsureDeleted();
                            MasterDBInitializerBasic.DbInitialize(_context);
                            break;
                            /* MARVIN 22.05.2018 Funktion deaktiviert
                        case "medium":
                            _context.Database.EnsureDeleted();
                            MasterDBInitializerMedium.DbInitialize(_context);
                            break;
                        default:
                            _context.Database.EnsureDeleted();
                            MasterDBInitializerLarge.DbInitialize(_context);
                            break;
                            */
                    }
                    
                }
            );

            return View("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
