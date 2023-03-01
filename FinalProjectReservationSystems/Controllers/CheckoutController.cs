using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace FinalProjectReservationSystems.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public CheckoutController(SonaDb sonaDb, UserManager<AppUser> userManager, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index(int id)
        {
            CheckoutViewModel checkoutVM = new CheckoutViewModel
            {
                room = await _sonaDb.Rooms.Include(x => x.roomServices).ThenInclude(x => x.Service).FirstOrDefaultAsync(r => r.Id == id),
                order = new Order()
            };

            if (checkoutVM.room == null) return NotFound();

            return View(checkoutVM);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel newOrder, int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }

            CheckoutViewModel checkoutVM = new CheckoutViewModel
            {
                room = await _sonaDb.Rooms.Include(x => x.roomServices).ThenInclude(x => x.Service).FirstOrDefaultAsync(x => x.Id == id),
                order = newOrder.order
            };

            if (checkoutVM.room == null) return NotFound();

            if (checkoutVM.order.CheckIn >= checkoutVM.order.CheckOut || checkoutVM.order.CheckIn <= DateTime.Now)
            {
                ModelState.AddModelError("order.CheckIn", "Please check date information");
                ModelState.AddModelError("order.CheckOut", "Please check date information");
            }

            if (checkoutVM.order.Person > checkoutVM.room.Capacity || checkoutVM.order.Person < 1)
            {
                ModelState.AddModelError("order.Person", "Please check person information");
            }

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Check your informations", new
                    ToastrOptions
                {
                    Title = "Invalid Information"
                });

                return View(checkoutVM);
            }

            List<Order> existOrders = await _sonaDb.Orders.Where(x => x.RoomId == id).ToListAsync();

            if (existOrders.Count() > 0)
            {
                foreach (var existOrder in existOrders)
                {
                    if (existOrder.CheckIn.Month == checkoutVM.order.CheckIn.Month && existOrder.CheckOut.Month == checkoutVM.order.CheckOut.Month)
                    {
                        if (existOrder.CheckOut > checkoutVM.order.CheckIn || checkoutVM.order.CheckOut < existOrder.CheckIn)
                        {
                            checkoutVM.order.Aviable = false;
                            ModelState.AddModelError("order.Aviable", "This reservation not aviable");

                            _toastNotification.AddWarningToastMessage(message: "Sorry, there are reservations for this date.", new
                            ToastrOptions
                            {
                                Title = "Please choose another date"
                            });

                            return View(checkoutVM);
                        }
                    }
                    else
                    {
                        if(existOrder.CheckIn.Month < checkoutVM.order.CheckIn.Month && existOrder.CheckOut.Month < checkoutVM.order.CheckOut.Month)
                        {
                            if (existOrder.CheckOut > checkoutVM.order.CheckIn || checkoutVM.order.CheckOut < existOrder.CheckIn)
                            {
                                checkoutVM.order.Aviable = false;
                                ModelState.AddModelError("order.Aviable", "This reservation not aviable");

                                _toastNotification.AddWarningToastMessage(message: "Sorry, there are reservations for this date.", new
                                ToastrOptions
                                {
                                    Title = "Please choose another date"
                                });

                                return View(checkoutVM);
                            }
                        }
                        else
                        {
                            if (existOrder.CheckOut < checkoutVM.order.CheckIn || checkoutVM.order.CheckOut > existOrder.CheckIn)
                            {
                                checkoutVM.order.Aviable = false;
                                ModelState.AddModelError("order.Aviable", "This reservation not aviable");

                                _toastNotification.AddWarningToastMessage(message: "Sorry, there are reservations for this date.", new
                                ToastrOptions
                                {
                                    Title = "Please choose another date"
                                });

                                return View(checkoutVM);
                            }
                        }
                    }
                }
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest();
            }

            _toastNotification.AddSuccessToastMessage(message: "You can make reservations on these dates.", new
                        ToastrOptions
            {
                Title = "Available."
            });

            return View(checkoutVM);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(CheckoutViewModel newOrder, int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }

            CheckoutViewModel checkoutVM = new CheckoutViewModel
            {
                room = await _sonaDb.Rooms.Include(x => x.roomServices).ThenInclude(x => x.Service).FirstOrDefaultAsync(x => x.Id == id),
                order = newOrder.order
            };

            if (checkoutVM.room == null) return NotFound();

            if (checkoutVM.order.CheckIn >= checkoutVM.order.CheckOut || checkoutVM.order.CheckIn <= DateTime.Now)
            {
                ModelState.AddModelError("order.CheckIn", "Please check date information");
                ModelState.AddModelError("order.CheckOut", "Please check date information");
            }

            if (checkoutVM.order.Person > checkoutVM.room.Capacity || checkoutVM.order.Person < 1)
            {
                ModelState.AddModelError("order.Person", "Please check person information");
            }

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage(message: "Check your informations", new
                    ToastrOptions
                {
                    Title = "Invalid Information"
                });

                return RedirectToAction("Index", "Checkout", new { newOrder = checkoutVM, id = id });

            }

            List<Order> existOrders = await _sonaDb.Orders.Where(x => x.RoomId == id).ToListAsync();

            if (existOrders.Count() > 0)
            {
                foreach (var existOrder in existOrders)
                {
                    if (existOrder.CheckIn.Month == checkoutVM.order.CheckIn.Month && existOrder.CheckOut.Month == checkoutVM.order.CheckOut.Month)
                    {
                        if (existOrder.CheckOut > checkoutVM.order.CheckIn || checkoutVM.order.CheckOut < existOrder.CheckIn)
                        {
                            checkoutVM.order.Aviable = false;
                            ModelState.AddModelError("order.Aviable", "This reservation not aviable");

                            _toastNotification.AddWarningToastMessage(message: "Sorry, there are reservations for this date.", new
                            ToastrOptions
                            {
                                Title = "Please choose another date"
                            });

                            return View(checkoutVM);
                        }
                    }
                    else
                    {
                        if (existOrder.CheckIn.Month < checkoutVM.order.CheckIn.Month && existOrder.CheckOut.Month < checkoutVM.order.CheckOut.Month)
                        {
                            if (existOrder.CheckOut > checkoutVM.order.CheckIn || checkoutVM.order.CheckOut < existOrder.CheckIn)
                            {
                                checkoutVM.order.Aviable = false;
                                ModelState.AddModelError("order.Aviable", "This reservation not aviable");

                                _toastNotification.AddWarningToastMessage(message: "Sorry, there are reservations for this date.", new
                                ToastrOptions
                                {
                                    Title = "Please choose another date"
                                });
                                return View(checkoutVM);
                            }
                        }
                        else
                        {
                            if (existOrder.CheckOut < checkoutVM.order.CheckIn || checkoutVM.order.CheckOut > existOrder.CheckIn)
                            {
                                checkoutVM.order.Aviable = false;
                                ModelState.AddModelError("order.Aviable", "This reservation not aviable");

                                _toastNotification.AddWarningToastMessage(message: "Sorry, there are reservations for this date.", new
                                ToastrOptions
                                {
                                    Title = "Please choose another date"
                                });

                                return View(checkoutVM);
                            }
                        }
                    }
                }
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest();
            }

            var days = checkoutVM.order.CheckOut - checkoutVM.order.CheckIn;

            int dayCount = Convert.ToInt32(days.TotalDays);

            Order order = new Order
            {
                Person = checkoutVM.order.Person,
                Aviable = true,
                CheckIn = checkoutVM.order.CheckIn,
                CheckOut = checkoutVM.order.CheckOut,
                Confirm = false,
                RoomId = id,
                TotalPrice = dayCount * checkoutVM.room.Price,
                UserMail = user.Email
            };

            await _sonaDb.Orders.AddAsync(order);

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "After the review, you will receive a notification in your e-mail.", new
                        ToastrOptions
            {
                Title = "Your order is complete."
            });

            return RedirectToAction("Index", "Checkout", new { newOrder = checkoutVM, id = id });
        }

    }
}
