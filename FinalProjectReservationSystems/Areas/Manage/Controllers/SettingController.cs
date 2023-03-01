using FinalProjectReservationSystems.Areas.ViewModel.Setting;
using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Helpers;
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
    public class SettingController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _env;

        public SettingController(SonaDb sonaDb, IToastNotification toastNotification, IWebHostEnvironment env)
        {
            _sonaDb = sonaDb;
            _toastNotification = toastNotification;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Setting> settings = await _sonaDb.Settings.ToListAsync();

            return View(settings);
        }

        public async Task<IActionResult> Update()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(SettingViewModel settingVM)
        {
            //Setting existSetting = await _sonaDb.Settings.FirstOrDefaultAsync(x => x.Id == setting.Id);

            //if (existSetting == null) return NotFound();

            List<Setting> existSettings = await _sonaDb.Settings.ToListAsync();

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }



            existSettings.FirstOrDefault(x => x.Key == "FacebookUrl").Value = settingVM.FacebookUrl;
            existSettings.FirstOrDefault(x => x.Key == "TwitterUrl").Value = settingVM.TwitterUrl;
            existSettings.FirstOrDefault(x => x.Key == "InstagramUrl").Value = settingVM.InstagramUrl;
            existSettings.FirstOrDefault(x => x.Key == "YoutubeUrl").Value = settingVM.YoutubeUrl;
            existSettings.FirstOrDefault(x => x.Key == "SupportPhone").Value = settingVM.SupportPhone;
            existSettings.FirstOrDefault(x => x.Key == "SupportEmail").Value = settingVM.SupportEmail;
            existSettings.FirstOrDefault(x => x.Key == "Address").Value = settingVM.Address;
            existSettings.FirstOrDefault(x => x.Key == "FooterText").Value = settingVM.FooterText;
            existSettings.FirstOrDefault(x => x.Key == "FAX").Value = settingVM.FAX;

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                   ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");
        }
    }
}
