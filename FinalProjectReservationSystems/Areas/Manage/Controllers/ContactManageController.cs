using FinalProjectReservationSystems.Abstractions.Services;
using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Helpers;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Data;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ContactManageController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IEmailService _emailService;
        private readonly IToastNotification _toastNotification;

        public ContactManageController(SonaDb sonaDb, IEmailService emailService, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _emailService = emailService;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            List<ContactUsMessage> messages = await _sonaDb.ContactUsMessages.ToListAsync();

            return View(messages);
        }

        public async Task<IActionResult> Details(int id)
        {
            ContactUsMessage foundedMessage = await _sonaDb.ContactUsMessages.FirstOrDefaultAsync(x => x.Id == id);

            if (foundedMessage == null) return NotFound();

            foundedMessage.Seen = true;

            await _sonaDb.SaveChangesAsync();

            return View(foundedMessage);
        }

        public async Task<IActionResult> SendMessage(int id)
        {

            ContactUsMessage foundedMessage = await _sonaDb.ContactUsMessages.FirstOrDefaultAsync(x => x.Id == id);

            if (foundedMessage == null) return NotFound();


            return View();
        }

        [HttpPost]

        public async Task<IActionResult> SendMessage(ReplyViewModel replyVM, int id)
        {

            ContactUsMessage foundedMessage = await _sonaDb.ContactUsMessages.FirstOrDefaultAsync(x => x.Id == id);

            if (foundedMessage == null) return NotFound();

            foundedMessage.Status = true;

            await _sonaDb.SaveChangesAsync();

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();

            }

            _emailService.Send(foundedMessage.ClientEmail, replyVM.Subject, replyVM.MessageText);


            _toastNotification.AddSuccessToastMessage(message: "Mail sent !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            ContactUsMessage deleteMessage = await _sonaDb.ContactUsMessages.FirstOrDefaultAsync(x => x.Id == id);

            if (deleteMessage == null) return NotFound();

            _sonaDb.ContactUsMessages.Remove(deleteMessage);

            await _sonaDb.SaveChangesAsync();

            return Ok();
        }
    }
}
