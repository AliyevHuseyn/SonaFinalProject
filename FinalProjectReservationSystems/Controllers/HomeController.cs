using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FinalProjectReservationSystems.Controllers
{
    public class HomeController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IToastNotification _toastNotification;

        public HomeController(SonaDb sonaDb, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _toastNotification = toastNotification;
        }


        public async Task<IActionResult> Index()
        {
            HomeViewModel homeVM = new HomeViewModel
            {
                Sliders = await _sonaDb.Sliders.ToListAsync(),
                Services = await _sonaDb.Services.ToListAsync(),
                Rooms = await _sonaDb.Rooms.Include(x => x.roomServices).ToListAsync(),
                roomServices = await _sonaDb.RoomServices.ToListAsync(),
                Blogs = await _sonaDb.Blogs.Include(x => x.BlogTag).Include(x => x.BlogImages).ToListAsync(),
                sliderSettings = await _sonaDb.sliderSetting.ToListAsync(),
                FindRoom = new List<Room>(),
            };

            return View(homeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeViewModel homeVM)
        {
            HomeViewModel home = new HomeViewModel
            {
                Sliders = await _sonaDb.Sliders.ToListAsync(),
                Services = await _sonaDb.Services.ToListAsync(),
                Rooms = await _sonaDb.Rooms.Include(x => x.roomServices).ToListAsync(),
                roomServices = await _sonaDb.RoomServices.ToListAsync(),
                Blogs = await _sonaDb.Blogs.Include(x => x.BlogTag).Include(x => x.BlogImages).ToListAsync(),
                sliderSettings = await _sonaDb.sliderSetting.ToListAsync(),
            };

            if(homeVM.CheckIn >= homeVM.CheckOut || homeVM.CheckIn <= DateTime.Now)
            {
                _toastNotification.AddErrorToastMessage(message: "Please enter CheckIN and CheckOUT date.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });
                return View(home);
            }

            if (homeVM.Person < 0 || homeVM.minPrice < 0 || homeVM.maxPrice < 0)
            {
                _toastNotification.AddErrorToastMessage(message: "Please check Guest/Minimum and Maximum price.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(home);
            }

            List<Room> findRooms = await _sonaDb.Rooms
                                               .Where(x => x.Price >= homeVM.minPrice && x.Price <= homeVM.maxPrice)
                                               .Where(x => x.Capacity >= homeVM.Person)
                                               .Include(x => x.roomServices)
                                               .ToListAsync();

            List<Room> findRms = new List<Room>();
            Room checkRoom = new Room();

            if (findRooms != null)
            {
                foreach (var findRoom in findRooms)
                {
                    List<Order> existOrders = await _sonaDb.Orders.Where(x => x.RoomId == findRoom.Id).ToListAsync();

                    if (existOrders.Count() > 0)
                    {
                        foreach (var existOrder in existOrders)
                        {
                            //  existOrder.CheckOut > checkoutVM.order.CheckIn || checkoutVM.order.CheckOut < existOrder.CheckIn
                            if (!(existOrder.CheckOut < homeVM.CheckIn) && !(homeVM.CheckOut > existOrder.CheckIn))
                            {

                                checkRoom = findRms.Find(x => x.Id == findRoom.Id);

                                if (checkRoom == null)
                                {
                                    findRms.Add(findRoom);
                                }
                            }
                        }
                    }
                    else
                    {
                        findRms.Add(findRoom);
                    }
                    
                }
            }

            home.FindRoom = findRms;

            return View(home);
        }



        [HttpPost]
        public async Task<IActionResult> Search(string? search)
        {
            List<Room> existRoom = await _sonaDb.Rooms.Include(x => x.roomServices).ThenInclude(x => x.Service).ToListAsync();

            List<Room> foundedRooms = new List<Room>();

            if(String.IsNullOrEmpty(search))
            {
                return View(foundedRooms);
            }
            foreach (var room in existRoom)
            {
                if(room.Name.ToUpper().Contains(search?.ToUpper()))
                {
                    foundedRooms.Add(room);
                }
            }

            return View(foundedRooms);
        }
    }
}