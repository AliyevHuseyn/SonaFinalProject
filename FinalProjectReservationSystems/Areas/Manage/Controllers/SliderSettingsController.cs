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
    public class SliderSettingsController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IToastNotification _toastNotification;

        public SliderSettingsController(SonaDb sonaDb, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            SliderSetting sliderSetting = await _sonaDb.sliderSetting.FirstOrDefaultAsync(x => x.Id == x.Id);

            return View(sliderSetting);
        }

        public async Task<IActionResult> Update(int id)
        {
            SliderSetting existSliderStng = await _sonaDb.sliderSetting.FirstOrDefaultAsync(x => x.Id == id);

            if (existSliderStng == null)
            {
                return NotFound();
            }


            return View(existSliderStng);
        }

        [HttpPost]
        public async Task<IActionResult> Update(SliderSetting sliderSetting)
        {
            SliderSetting existSliderStng = await _sonaDb.sliderSetting.FirstOrDefaultAsync(x => x.Id == sliderSetting.Id);

            if(existSliderStng == null)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }

            existSliderStng.Title = sliderSetting.Title;
            existSliderStng.Description = sliderSetting.Description;
            existSliderStng.ButtonText = sliderSetting.ButtonText;
            existSliderStng.ButtonUrl = sliderSetting.ButtonUrl;

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            await _sonaDb.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
