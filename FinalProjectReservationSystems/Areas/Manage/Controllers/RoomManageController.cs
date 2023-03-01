using FinalProjectReservationSystems.Areas.ViewModel.Room;
using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Helpers;
using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class RoomManageController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IWebHostEnvironment _env;
        private readonly IToastNotification _toastNotification;

        public RoomManageController(SonaDb sonaDb, IWebHostEnvironment env, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _env = env;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            List<Room> rooms = await _sonaDb.Rooms.Include(r => r.roomServices).ToListAsync();

            return View(rooms);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.RoomServices = await _sonaDb.Services.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddViewModel room)
        {
            ViewBag.RoomServices = await _sonaDb.Services.ToListAsync();

            if (room.Capacity < 1 || room.Price < 1 || room.Size < 1)
            {
                ModelState.AddModelError("Price", "Price minimum value 1");
                ModelState.AddModelError("Size", "Size minimum value 1");
                ModelState.AddModelError("Capacity", "Capacity minimum value 1");
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


            if(room.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "Image required");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }
            else
            {
                if (room.PosterFile.Length > 3500000)
                {
                    ModelState.AddModelError("PosterFile", "Max size 3mb");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }



                if (room.PosterFile.ContentType != "image/png" && room.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterFile", "Only jpeg or png");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }
            }

            if (room.DetailFile == null)
            {
                ModelState.AddModelError("DetailFile", "Image required");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }
            else
            {
                if (room.DetailFile.Length > 3500000)
                {
                    ModelState.AddModelError("DetailFile", "Max size 3mb");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }



                if (room.DetailFile.ContentType != "image/png" && room.DetailFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("DetailFile", "Only jpeg or png");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }
            }

            var roomModel = new Room
            {
                Name = room.Name,
                Description = room.Description,
                Capacity = room.Capacity,
                Size = room.Size,
                DetailImgUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/rooms", room.DetailFile),
                PosterImgUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/rooms", room.PosterFile),
                BedType = room.BedType,
                Price = room.Price,

            };

             _sonaDb.Rooms.Add(roomModel);

            foreach (var service in room.ServiceIds)
            {
                var roomService = new RoomService
                {
                    Room = roomModel,
                    ServiceId = service
                };

                 _sonaDb.RoomServices.Add(roomService);
            }

            _sonaDb.SaveChanges();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? roomId)
        {
            ViewBag.RoomServices = _sonaDb.Services.ToList();

            if (roomId == null || roomId <= 0) return BadRequest();

            var room = await _sonaDb.Rooms.Include(r => r.roomServices).FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) return NotFound();


            var model = new AddViewModel
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                Capacity = room.Capacity,
                Size = room.Size,
                DetailImgUrl = room.DetailImgUrl != null ? room.DetailImgUrl : null,
                PosterImgUrl = room.PosterImgUrl != null ? room.PosterImgUrl : null,
                BedType = room.BedType,
                Price = room.Price,
                ServiceIds = room.roomServices.Select(r => r.ServiceId).ToList(),
                Services = await _sonaDb.Services.ToListAsync(),

            };


            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int? roomId , AddViewModel model)
        {

            ViewBag.RoomServices = _sonaDb.Services.ToList();

            if (roomId == null || roomId <= 0) return BadRequest();

            var existRoom = await _sonaDb.Rooms.Include(r => r.roomServices).FirstOrDefaultAsync(r => r.Id == roomId);

            if (existRoom == null) return NotFound();


            var newModel = new AddViewModel
            {
                Id = existRoom.Id,
                Name = existRoom.Name,
                Description = existRoom.Description,
                Capacity = existRoom.Capacity,
                Size = existRoom.Size,
                DetailImgUrl = existRoom.DetailImgUrl != null ? existRoom.DetailImgUrl : null,
                PosterImgUrl = existRoom.PosterImgUrl != null ? existRoom.PosterImgUrl : null,
                BedType = existRoom.BedType,
                Price = existRoom.Price,
                ServiceIds = existRoom.roomServices.Select(r => r.ServiceId).ToList(),
                Services = await _sonaDb.Services.ToListAsync(),

            };

            if (model.Capacity < 1 || model.Price < 1 || model.Size < 1)
            {
                ModelState.AddModelError("Price", "Price minimum value 1");
                ModelState.AddModelError("Size", "Size minimum value 1");
                ModelState.AddModelError("Capacity", "Capacity minimum value 1");
            }

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(newModel);
            }
                
            if (roomId == null || roomId <= 0) return BadRequest();

            var room = await _sonaDb.Rooms.Include(r => r.roomServices).FirstOrDefaultAsync(r => r.Id == roomId);

            if (room is null) return NotFound();


            if (model.PosterFile != null)
            {
                if (model.PosterFile.Length > 3500000)
                {
                    ModelState.AddModelError("PosterFile", "Max size 3mb");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(newModel);
                }

                if (model.PosterFile.ContentType != "image/png" && model.PosterFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterFile", "Only jpeg or png");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(newModel);
                }

                FileManager.DeleteFile(_env.WebRootPath, "uploads/rooms", room.PosterImgUrl);
                room.PosterImgUrl = model.PosterFile != null ? FileManager.SaveFile(_env.WebRootPath, "uploads/rooms", model.PosterFile) : null;
            }


            if (model.DetailFile != null)
            {
                if (model.DetailFile.Length > 3500000)
                {
                    ModelState.AddModelError("DetailFile", "Max size 3mb");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(newModel);
                }

                if (model.DetailFile.ContentType != "image/png" && model.DetailFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("DetailFile", "Only jpeg or png");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(newModel);
                }

                FileManager.DeleteFile(_env.WebRootPath, "uploads/rooms", room.DetailImgUrl);
                room.DetailImgUrl = model.DetailFile != null ? FileManager.SaveFile(_env.WebRootPath, "uploads/rooms", model.DetailFile) : null;
            }


            foreach (var roomservice in model.ServiceIds)
            {
                if(!_sonaDb.Services.Any(rs => rs.Id == roomservice))
                {
                    ModelState.AddModelError("Services", "Service not found!");
                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(newModel);
                }
            }
            room.Name= model.Name;
            room.Description= model.Description;
            room.Capacity= model.Capacity;
            room.Size= model.Size;
            room.Price = model.Price;
            UpdateRoomService();

            void UpdateRoomService()
             {
                //roomun databasada olan servicelerinin idlerini tapmaq
                var roomServiceInDb = room.roomServices.Select(rs => rs.ServiceId).ToList();

                //roomun databasada olan kohne servicelerinin idlerini yigmaq (silmek ucun)

                var roomServiceToRemove = roomServiceInDb.Except(model.ServiceIds).ToList();

                //roomun databasada olan ve ya olmayan userdan gelen yeni service id leri (elave etmek ucun)

                var roomServiceIdsToAdd = model.ServiceIds.Except(roomServiceInDb).ToList();

                room.roomServices.RemoveAll(rs => roomServiceToRemove.Contains(rs.ServiceId));

                foreach (var roomServiceIdToAdd in roomServiceIdsToAdd)
                {
                    var roomservice = new RoomService
                    {
                        Room = room,
                        ServiceId = roomServiceIdToAdd
                    };

                    _sonaDb.RoomServices.Add(roomservice);
                }

                 _sonaDb.SaveChanges();
            }

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? roomId)
        {
            if (roomId == null || roomId <= 0) return BadRequest();

            var room = await _sonaDb.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) return NotFound();

            FileManager.DeleteFile(_env.WebRootPath, "uploads/rooms", room.PosterImgUrl);
            FileManager.DeleteFile(_env.WebRootPath, "uploads/rooms", room.DetailImgUrl);

            _sonaDb.Rooms.Remove(room);

            await _sonaDb.SaveChangesAsync();

            return Ok();
        }
    }
}
