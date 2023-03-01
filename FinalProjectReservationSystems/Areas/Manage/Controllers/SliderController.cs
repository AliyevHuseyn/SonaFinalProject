using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Helpers;
using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Diagnostics.Metrics;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]

    public class SliderController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IWebHostEnvironment _env;
        private readonly IToastNotification _toastNotification;

        public SliderController(SonaDb sonaDb, IWebHostEnvironment env, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _env = env;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            List<Slider> sliders = _sonaDb.Sliders.ToList();

            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
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


            if (slider.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Slider image required!");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }

            if (slider.ImageFile.ContentType != "image/png" && slider.ImageFile.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("ImageFile", "Only png ve jpeg file type");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }


            if (slider.ImageFile.Length > 5300000)
            {
                ModelState.AddModelError("ImageFile", "Slider image max size 5 mb");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }


            slider.ImageUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/sliders", slider.ImageFile);


            await _sonaDb.Sliders.AddAsync(slider);

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            Slider existSlider = _sonaDb.Sliders.FirstOrDefault(x => x.Id == id);

            if (existSlider == null) return NotFound();

            return View(existSlider);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Slider slider)
        {
            Slider existSlider = _sonaDb.Sliders.FirstOrDefault(x => x.Id == slider.Id);

            if (existSlider == null) return NotFound();

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();
            }

            if (slider.ImageFile != null)
            {
                FileManager.DeleteFile(_env.WebRootPath, "uploads/sliders", existSlider.ImageUrl);

                if (slider.ImageFile.ContentType != "image/png" && slider.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "Only png ve jpeg file type");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }


                if (slider.ImageFile.Length > 5300000)
                {
                    ModelState.AddModelError("ImageFile", "Slider image max size 5 mb");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }

                existSlider.ImageUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            }

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Slider deleteSlider = _sonaDb.Sliders.FirstOrDefault(x => x.Id == id);

            if (deleteSlider == null) return NotFound();

            FileManager.DeleteFile(_env.WebRootPath, "uploads/sliders", deleteSlider.ImageUrl);

            _sonaDb.Sliders.Remove(deleteSlider);

            await _sonaDb.SaveChangesAsync();

            return Ok();
        }
    }

}
