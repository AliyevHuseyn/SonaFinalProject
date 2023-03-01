using FinalProjectReservationSystems.Abstractions.Services;
using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Data;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class OrderController : Controller
    {

        private readonly IEmailService _emailService;
        private readonly IToastNotification _toastNotification;
        private readonly SonaDb _sonaDb;

        public OrderController(SonaDb sonaDb, IEmailService emailService, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _emailService = emailService;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {

            List<Order> orders = await _sonaDb.Orders.Include(x => x.Room).OrderBy(x => x.CheckIn).ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Confirm(int id)
        {
            Order order = await _sonaDb.Orders.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == id);

            if(order == null)
            {
                return BadRequest();
            }

            order.OrderSerialNumber = Guid.NewGuid().ToString();
            order.Confirm = true;

            var ticketUrl = "https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=" + order.OrderSerialNumber;

            _emailService.Send(order.UserMail, $"Your reservation has been confirmed",

                $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n<style>\r\ntable, th, td {{\r\n  border: 1px solid black;\r\n  border-collapse: collapse;\r\n}}\r\nth, td {{\r\n  padding: 5px;\r\n  text-align: left;\r\n}}\r\n</style>\r\n</head>\r\n<body>\r\n\r\n<h2>Your reservation information:</h2>\r\n\r\n<table style=\"width:100%\">\r\n  <tr>\r\n    <th>From:</th>\r\n    <td>{order.UserMail}</td>\r\n  </tr>\r\n  <tr>\r\n    <th>Checkin Date</th>\r\n    <td>{order.CheckIn.ToString("dd MMMM yyyy")}</td>\r\n  </tr>\r\n  <tr>\r\n    <th>CheckOut Date:</th>\r\n    <td>{order.CheckOut.ToString("dd MMMM yyyy")}</td>\r\n  </tr>\r\n    <tr>\r\n    <th>Room Name:</th>\r\n    <td>{order.Room.Name}</td>\r\n  </tr>\r\n    <tr>\r\n    <th>Person:</th>\r\n    <td>{order.Person}</td>\r\n  </tr>\r\n  </tr>\r\n    <tr>\r\n    <th>Total Price:</th>\r\n    <td>{order.TotalPrice}</td>\r\n  </tr>\r\n  </tr>\r\n    <tr>\r\n   <th>Ticket ID <span style=\"color:red\">(Don't share)</span>:</th>\r\n    <td>{order.OrderSerialNumber}</td>\r\n  </tr>\r\n  </tr>\r\n    <tr>\r\n   <th>Ticket ID <span style=\"color:red\">(Don't share)</span>:</th>\r\n    <td><img src='{ticketUrl}'></td>\r\n  </tr>\r\n</table>\r\n\r\n</body>\r\n</html>\r\n");

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Ban(int id)
        {

            Order foundedOrder = await _sonaDb.Orders.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == id);

            if (foundedOrder == null) return NotFound();

            foundedOrder.Status = false;

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddAlertToastMessage(message: "Reservation Banned !", new
                    ToastrOptions
            {
                Title = "Process Completed"
            });

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Scan()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Scan([FromForm] string qrCode)
        {

            Order order = await _sonaDb.Orders.Where(x => x.Confirm == true).FirstOrDefaultAsync(x => x.OrderSerialNumber == qrCode);


            if (order == null)
            {
                _toastNotification.AddErrorToastMessage(message: "There is no matching information in the database !", new
                    ToastrOptions
                {
                    Title = "Not Found!"
                });

                return View();
            }
            if (order.Status == false)
            {
                _toastNotification.AddErrorToastMessage(message: "This ticket has been banned !", new
                    ToastrOptions
                {
                    Title = "Banned!"
                });

                return View();
            }
            if(order.Status == true)
            {
                _toastNotification.AddWarningToastMessage(message: "This ticket has been scanned !", new
                    ToastrOptions
                {
                    Title = "Scanned!"
                });

                return View();
            }
            else
            {
                order.Status = true;
                order.ScanDate = DateTime.Now;
                await _sonaDb.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage(message: "Process completed successfully !", new
                    ToastrOptions
                {
                    Title = "Valid !"
                });

                return RedirectToAction("index");
            }
        }
    }
}
