using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ReplyManageController : Controller
    {
        private readonly SonaDb _sonaDb;

        public ReplyManageController(SonaDb sonaDb)
        {
            _sonaDb = sonaDb;
        }

        public async Task<IActionResult> Index()
        {
            List<CommentReply> Replies = await _sonaDb.CommentReplies.ToListAsync();

            return View(Replies);
        }

        public async Task<IActionResult> Details(int id)
        {
            CommentReply foundedReply = await _sonaDb.CommentReplies.FirstOrDefaultAsync(x => x.Id == id);

            if (foundedReply == null) return NotFound();

            return View(foundedReply);
        }

        public async Task<IActionResult> Delete(int id)
        {
            CommentReply deleteReply = await _sonaDb.CommentReplies.FirstOrDefaultAsync(x => x.Id == id);

            if (deleteReply == null) return NotFound();

            _sonaDb.CommentReplies.Remove(deleteReply);

            await _sonaDb.SaveChangesAsync();

            return Ok();
        }

    }
}
