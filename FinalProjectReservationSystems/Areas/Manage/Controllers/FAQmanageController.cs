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
    public class FAQmanageController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IToastNotification _toastNotification;

        public FAQmanageController(SonaDb sonaDb, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            List<FAQ> fAQs = await _sonaDb.FAQs.ToListAsync(); 

            return View(fAQs);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FAQ faq)
        {
            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(faq);
            }

            await _sonaDb.FAQs.AddAsync(faq);

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            FAQ existFAQ = await _sonaDb.FAQs.FirstOrDefaultAsync(x => x.Id == id);

            if (existFAQ == null) return NotFound();

            return View(existFAQ);
        }

        [HttpPost]
        public async Task<IActionResult> Update(FAQ faq)
        {
            FAQ existFAQ = await _sonaDb.FAQs.FirstOrDefaultAsync(x => x.Id == faq.Id);

            if (existFAQ == null) return NotFound();

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(faq);
            }

            existFAQ.Title = faq.Title;
            existFAQ.Description = faq.Description;


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
            FAQ deleteFAQ = await _sonaDb.FAQs.FirstOrDefaultAsync(x => x.Id == id);

            if(deleteFAQ == null) return NotFound();    

            _sonaDb.FAQs.Remove(deleteFAQ);

            await _sonaDb.SaveChangesAsync();

            return Ok();
        }
    }
}
