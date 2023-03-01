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
    public class BlogTagController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IToastNotification _toastNotification;

        public BlogTagController(SonaDb sonaDb, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {

            List<BlogTag> blogTags = await _sonaDb.BlogTags.ToListAsync();

            return View(blogTags);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(BlogTag blogTag)
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

            await _sonaDb.BlogTags.AddAsync(blogTag);

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
            BlogTag existTags = await _sonaDb.BlogTags.FirstOrDefaultAsync(x => x.Id == id);

            if (existTags == null) return NotFound();

            return View(existTags);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BlogTag tag)
        {

            BlogTag existTag = await _sonaDb.BlogTags.FirstOrDefaultAsync(x => x.Id == tag.Id);

            if (existTag == null) return NotFound();

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View();

            }

            existTag.Name = tag.Name;

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index");

        }

        public IActionResult Delete(int id)
        {
            BlogTag deleteTag = _sonaDb.BlogTags.FirstOrDefault(x => x.Id == id);

            if (deleteTag == null) return NotFound();

            _sonaDb.BlogTags.Remove(deleteTag);

            _sonaDb.SaveChanges();


            return Ok();
        }
    }
}
