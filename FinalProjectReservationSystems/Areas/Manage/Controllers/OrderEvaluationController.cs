using FinalProjectReservationSystems.Abstractions.Services;
using FinalProjectReservationSystems.Areas.ViewModel.Order;
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
    public class OrderEvaluationController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IEmailService _emailService;
        private readonly IToastNotification _toastNotification;

        public OrderEvaluationController(SonaDb sonaDb, IEmailService emailService, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _emailService = emailService;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            List<Order> bannedOrders = await _sonaDb.Orders.Where(x => x.Status == false).Include(x => x.Room).ToListAsync();

            return View(bannedOrders);
        }

        public async Task<IActionResult> Send(int id)
        {
            Order bannedOrder = await _sonaDb.Orders.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == id);

            if(bannedOrder == null)
            {
                return NotFound();
            }

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Send(BannedOrderSendMailViewModel bannedOrderSendMailVM, int id)
        {
            Order bannedOrder = await _sonaDb.Orders.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == id);

            if(bannedOrder == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();

            }

            _emailService.Send(bannedOrder.UserMail, bannedOrderSendMailVM.Subject, bannedOrderSendMailVM.Body);

            _toastNotification.AddSuccessToastMessage(message: "Mail sent !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("index");
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> BannedOrder(int id)
        {

            Order bannedOrder = await _sonaDb.Orders.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == id);

            if (bannedOrder == null)
            {
                return NotFound();
            }


            bannedOrder.Status = null;

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Ban has been successfully removed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("index");
        }
    }
}
