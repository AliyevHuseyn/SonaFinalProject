using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Data;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ServiceController : Controller
    {

        private readonly SonaDb _sonaDb;
        private readonly IToastNotification _toastNotification;

        public ServiceController(SonaDb sonaDb, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            List<Service> services = await _sonaDb.Services.ToListAsync();

            return View(services);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Service service)
        {
            if(!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }


            await _sonaDb.Services.AddAsync(service);

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            Service existService = await _sonaDb.Services.FirstOrDefaultAsync(x => x.Id == id);

            if(existService == null) { return NotFound(); }

            return View(existService);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Service service)
        {
            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                   ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }

            Service existService = await _sonaDb.Services.FirstOrDefaultAsync(x => x.Id == service.Id);

            if(existService == null) { return NotFound(); }

            existService.Name = service.Name;
            existService.Description = service.Description;
            existService.Icon = service.Icon;

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                   ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Service deleteService = await _sonaDb.Services.FirstOrDefaultAsync(x => x.Id == id);

            if(deleteService == null) { return NotFound(); }

            _sonaDb.Services.Remove(deleteService);

            await _sonaDb.SaveChangesAsync();

            return Ok();
        }

    }
}
