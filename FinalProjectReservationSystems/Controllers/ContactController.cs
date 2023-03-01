using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace FinalProjectReservationSystems.Controllers
{
    public class ContactController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IToastNotification _toastNotification;

        public ContactController(SonaDb sonaDb, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ContactUsMessage contactUsMsg)
        {
            contactUsMsg.Seen = false;
            contactUsMsg.Status = false;

            if(!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(contactUsMsg);
            }

            _sonaDb.ContactUsMessages.Add(contactUsMsg);

            _sonaDb.SaveChanges();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("index");
        }
    }
}
