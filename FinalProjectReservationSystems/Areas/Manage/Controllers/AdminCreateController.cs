using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Helpers;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Security.Cryptography.X509Certificates;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminCreateController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IToastNotification _toastNotification;

        public AdminCreateController(SonaDb sonaDb, UserManager<AppUser> userManager, IWebHostEnvironment env, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _userManager = userManager;
            _env = env;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {

            List<AppUser> appUsers = await _sonaDb.AppUsers.ToListAsync();

            return View(appUsers);
        }


        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel registerVM)
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

            AppUser user = await _userManager.FindByNameAsync(registerVM.UserName);

            if (user != null)
            {
                ModelState.AddModelError("UserName", "User Name has taken");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }


            user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                ModelState.AddModelError("Email", "This Email has taken!");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }


            if (registerVM.ProfilPhoto == null)
            {
                user = new AppUser
                {
                    UserName = registerVM.UserName,
                    FullName = registerVM.FullName,
                    Email = registerVM.Email,
                    ProfilPhotoUrl = "anonymousUserPP.png",
                    roleName = "Admin",
                    EmailConfirmed = true
                };
            }
            else
            {
                if (registerVM.ProfilPhoto.ContentType != "image/png" && registerVM.ProfilPhoto.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ProfilPhoto", "Only png ve jpeg file type");

                    _toastNotification.AddErrorToastMessage(message: "Only png ve jpeg file type", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }


                if (registerVM.ProfilPhoto.Length > 5300000)
                {
                    ModelState.AddModelError("ProfilPhoto", "Profil Photo image max size 5 mb");

                    _toastNotification.AddErrorToastMessage(message: "Profil Photo image max size 5 mb", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }

                user = new AppUser
                {
                    UserName = registerVM.UserName,
                    FullName = registerVM.FullName,
                    Email = registerVM.Email,
                    ProfilPhotoUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/profilPhotos", registerVM.ProfilPhoto),
                    roleName = "Admin",
                    EmailConfirmed = true
                };
            }
            


            var res = await _userManager.CreateAsync(user, registerVM.Password);

            if (!res.Succeeded)
            {
                foreach (var err in res.Errors)
                {
                    ModelState.AddModelError("Password", err.Description);

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }
            }


            await _userManager.AddToRoleAsync(user, "Admin");

            _toastNotification.AddSuccessToastMessage(message: "Something went wrong.", new
                    ToastrOptions
            {
                Title = "Opps!"
            });

            return RedirectToAction("index");
        }


        public async Task<IActionResult> Delete(string id)
        {
            AppUser deleteUser = await _sonaDb.AppUsers.FirstOrDefaultAsync(x => x.Id == id);

            if (deleteUser == null || deleteUser.roleName == "SuperAdmin") return NotFound();

            await _userManager.DeleteAsync(deleteUser);

            return Ok();

        }
    }
}
