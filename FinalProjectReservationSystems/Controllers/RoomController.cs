using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace FinalProjectReservationSystems.Controllers
{
    public class RoomController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public RoomController(SonaDb sonaDb, UserManager<AppUser> userManager, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            List<Room> rooms = await _sonaDb.Rooms.Include(x => x.roomServices).ThenInclude(x => x.Service).ToListAsync();

            return View(rooms);
        }

        public async Task<IActionResult> Details(int id)
        {
            RoomViewModel roomVM = new RoomViewModel
            {
                Room = await _sonaDb.Rooms.Include(x => x.roomServices).ThenInclude(x => x.Service).FirstOrDefaultAsync(x => x.Id == id),
                Reviews = await _sonaDb.Reviews.Where(x => x.RoomId == id).ToListAsync(),
            };

            if (roomVM.Room == null) return NotFound();

            return View(roomVM);
        }

        [HttpPost]
        public async Task<IActionResult> Details(RoomViewModel roomVM, int id)
        {
            RoomViewModel existRoom = new RoomViewModel
            {
                Room = await _sonaDb.Rooms.Include(x => x.roomServices).ThenInclude(x => x.Service).FirstOrDefaultAsync(x => x.Id == id),
                Reviews = await _sonaDb.Reviews.Where(x => x.RoomId == id).ToListAsync(),
            };

            if (existRoom.Room == null) return NotFound();

            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("Email", "Please login or register");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(existRoom);
            }

            if (roomVM.Rating < 1 || roomVM.Rating > 5)
            {
                ModelState.AddModelError("Rating", "The Rating field is required");
            }

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(existRoom);
            }

            AppUser? user = await _userManager.FindByEmailAsync(roomVM.Email);

            if(user?.UserName != User.Identity.Name || user == null)
            {
                ModelState.AddModelError("Email", "Please enter the email address you are currently registered with");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(existRoom);
            }

            Review newReview = new Review
            {
                dateTime = DateTime.Now,
                FullName = roomVM.FullName,
                Email = roomVM.Email,
                Description = roomVM.Description,
                ProfilPhotoUrl = user?.ProfilPhotoUrl,
                Rating = roomVM.Rating,
                RoomId = existRoom.Room.Id
            };

            await _sonaDb.Reviews.AddAsync(newReview);

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("details", "room");
        }
    }
}
