using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectReservationSystems.Controllers
{
    public class FAQController : Controller
    {
        private readonly SonaDb _sonaDb;

        public FAQController(SonaDb sonaDb)
        {
            _sonaDb = sonaDb;
        }

        public async Task<IActionResult> Index()
        {
            List<FAQ> FAQs = await _sonaDb.FAQs.ToListAsync(); 

            return View(FAQs);
        }
    }
}
