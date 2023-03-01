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
    public class CommentManageController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IToastNotification _toastNotification;

        public CommentManageController(SonaDb sonaDb, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {

            List<Comment> Comments = await _sonaDb.Comments.ToListAsync();

            return View(Comments);
        }

        public async Task<IActionResult> Details(int id)
        {
            Comment foundedComment = await _sonaDb.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (foundedComment == null) return NotFound();

            return View(foundedComment);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Comment deleteComment = await _sonaDb.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (deleteComment == null) return NotFound();

            List<CommentReply>? replies = await _sonaDb.CommentReplies.Where(x => x.CommentId == id)?.ToListAsync();

            if(replies != null && replies.Count > 0)
            {
                foreach (var reply in replies)
                {
                    _sonaDb.CommentReplies.Remove(reply);
                }
            }

            _sonaDb.Comments.Remove(deleteComment);

            await _sonaDb.SaveChangesAsync();

            return Ok();
        }

    }
}
