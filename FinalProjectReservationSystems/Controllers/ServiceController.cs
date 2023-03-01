using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectReservationSystems.Controllers
{
    public class ServiceController : Controller
    {
        private readonly SonaDb _sonaDb;

        public ServiceController(SonaDb sonaDb)
        {
            _sonaDb = sonaDb;
        }

        public async Task<IActionResult> Index()
        {
            List<Service> services = await _sonaDb.Services.ToListAsync();

            return View(services);
        }
    }
}
