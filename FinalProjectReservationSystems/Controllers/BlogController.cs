using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace FinalProjectReservationSystems.Controllers
{
    public class BlogController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public BlogController(SonaDb sonaDb, UserManager<AppUser> userManager, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            List<Blog> Blogs = await _sonaDb.Blogs.Include(x => x.BlogImages).Include(x => x.BlogTag).ToListAsync();

            return View(Blogs);
        }

        public async Task<IActionResult> Details(int id)
        {
            Blog? existBlog = await _sonaDb.Blogs.Include(x => x.BlogImages).Include(x => x.BlogTag).Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);

            if (existBlog == null) return NotFound();

            BlogViewModel blogViewModel = new BlogViewModel
            {
                blog = existBlog,
                RelatedBlogs = await _sonaDb.Blogs.Include(x => x.BlogImages).Include(x => x.BlogTag).Where(x => x.BlogTagId == existBlog.BlogTagId).ToListAsync(),
                BlogComments = await _sonaDb.Comments.Where(x => x.BlogId == existBlog.Id).ToListAsync(),
                BlogCommentsReply = await _sonaDb.CommentReplies.Where(x => x.BlogId == existBlog.Id).ToListAsync()

            };

            return View(blogViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Details(BlogViewModel blogVM, int id)
        {
            Blog? existBlog = await _sonaDb.Blogs.Include(x => x.BlogImages).Include(x => x.BlogTag).Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);

            if (existBlog == null) return NotFound();

            BlogViewModel blogViewModel = new BlogViewModel
            {
                blog = existBlog,
                RelatedBlogs = await _sonaDb.Blogs.Include(x => x.BlogImages).Include(x => x.BlogTag).Where(x => x.BlogTagId == existBlog.BlogTagId).ToListAsync(),
                BlogComments = await _sonaDb.Comments.Where(x => x.BlogId == existBlog.Id).ToListAsync(),
                BlogCommentsReply = await _sonaDb.CommentReplies.Where(x => x.BlogId == existBlog.Id).ToListAsync()
            };

            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("Email", "Please login or register");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                   ToastrOptions
                {
                    Title = "Opps!"
                });

                return Content("False");
            }

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                   ToastrOptions
                {
                    Title = "Opps!"
                });

                return Content("False");
            }
            
            string userName = HttpContext.User.Identity.Name;

            AppUser? user = null;

            user = await _userManager.FindByNameAsync(userName);

            if (user.Email == blogVM.Email)
            {
                Comment comment = new Comment
                {
                    BlogId = id,
                    Date = DateTime.Now,
                    Text = blogVM.Text,
                    Fullname = user.FullName,
                    ProfilPhoto = user.ProfilPhotoUrl,
                    Email = user.Email

                };

                await _sonaDb.Comments.AddAsync(comment);

                await _sonaDb.SaveChangesAsync();

                _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
                {
                    Title = "Successful"
                });

                return Json(comment);
            }
            else
            {
                ModelState.AddModelError("Email", "Please enter the email address you are currently registered with");

                _toastNotification.AddErrorToastMessage(message: "Please enter the email address you are currently registered with.", new
                  ToastrOptions
                {
                    Title = "Opps!"
                });

                return Content("False");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendReply(BlogViewModel blogVM, int id)
        {
            Comment existComment = await _sonaDb.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (existComment == null)
            {
                return NotFound();
            }

            if (blogVM.ReplyText == null || blogVM.ReplyText.Length > 174)
            {
                ModelState.AddModelError("ReplyText", "Maximum char limit 175 and reply text required");

                _toastNotification.AddErrorToastMessage(message: "Maximum char limit 175 and reply text required.", new
                   ToastrOptions
                {
                    Title = "Opps!"
                });

                //return RedirectToAction("details", "blog", new { id = existComment.BlogId });

                return Content("False");

            }

            AppUser user = null;

            string userName = User.Identity.Name;

            user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            CommentReply commentReply = new CommentReply
            {
                BlogId = existComment.BlogId,
                CommentId = existComment.Id,
                Date = DateTime.Now,
                Text = blogVM.ReplyText,
                Fullname = user.FullName,
                ProfilPhoto = user.ProfilPhotoUrl,
            };

            await _sonaDb.CommentReplies.AddAsync(commentReply);
            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return Json(commentReply);

        }
    }
}
