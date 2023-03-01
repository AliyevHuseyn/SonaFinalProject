using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Helpers;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Data;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class BlogController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public BlogController(SonaDb sonaDb, IWebHostEnvironment env, UserManager<AppUser> userManager, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _env = env;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {

            List<Blog> blogs = await _sonaDb.Blogs.Include(x => x.BlogImages).Include(x => x.BlogTag).ToListAsync();

            return View(blogs);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Tags = await _sonaDb.BlogTags.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Blog blog)
        {
            ViewBag.Tags = await _sonaDb.BlogTags.ToListAsync();

            blog.Date = DateTime.Now;

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });


                return View(blog);

            }
                if (blog.PosterImage != null)
            {
                if (blog.PosterImage.ContentType != "image/png" && blog.PosterImage.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterImage", "Only png or jpeg!");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }
                if (blog.PosterImage.Length > 3145728)
                {
                    ModelState.AddModelError("PosterImage", "Maximum size 3mb!");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("PosterImage", "Poster image required!");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(blog);
            }


            if (blog.SliderImage != null)
            {
                if (blog.SliderImage.ContentType != "image/png" && blog.SliderImage.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("SliderImage", "Only png or jpeg!");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                   ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }
                if (blog.SliderImage.Length > 3145728)
                {
                    ModelState.AddModelError("SliderImage", "Maximum size 3mb!");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                   ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View();
                }
                
            }
            else
            {
                ModelState.AddModelError("SliderImage", "Slider image required!");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(blog);
            }



            if (blog.Images != null)
            {
                foreach (IFormFile imageFile in blog.Images)
                {
                    if (imageFile.ContentType != "image/png" && imageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("Images", "Only png or jpeg!");

                        _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                        {
                            Title = "Opps!"
                        });

                        return View();
                    }
                    if (imageFile.Length > 3145728)
                    {
                        ModelState.AddModelError("Images", "Maximum size 3mb!");

                        _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                        {
                            Title = "Opps!"
                        });

                        return View();
                    }
                    BlogImage blogImage = new BlogImage
                    {
                        Blog = blog,
                        ImageUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/blogs", imageFile),
                        IsPoster = null
                    };
                    await _sonaDb.BlogImages.AddAsync(blogImage);
                }
            }
            else
            {
                ModelState.AddModelError("Images", "Blog images required!");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(blog);
            }




            BlogImage posterImage = new BlogImage
            {
                Blog = blog,
                ImageUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/blogs", blog.PosterImage),
                IsPoster = true
            };
            await _sonaDb.BlogImages.AddAsync(posterImage);


            BlogImage sliderImage = new BlogImage
            {
                Blog = blog,
                ImageUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/blogs", blog.SliderImage),
                IsPoster = false
            };
            await _sonaDb.BlogImages.AddAsync(sliderImage);



            AppUser user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if (user == null)
            {
                blog.Author = "Anonim";
            }
            else
            {
                blog.Author = user.FullName;
            }


            await _sonaDb.Blogs.AddAsync(blog);
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
            Blog blog = _sonaDb.Blogs
                .Include(x => x.BlogImages)
                .FirstOrDefault(x => x.Id == id);


            if (blog == null) return NotFound();
            ViewBag.BlogTags = _sonaDb.BlogTags.ToList();
            //ViewBag.Comments = _sonaDb.Comments.ToList();
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Blog blog)
        {
            ViewBag.BlogTags = await _sonaDb.BlogTags.ToListAsync();

            Blog? existBlog = await _sonaDb?.Blogs?.
                Include(x => x.BlogImages)?.
                FirstOrDefaultAsync(x => x.Id == blog.Id);
            if (existBlog == null) return NotFound();


            existBlog.Date = DateTime.Now;

            if (!ModelState.IsValid)
            {

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(existBlog);
            }

            if(blog.BlogImageIds == null && blog.Images == null) 
            {

                ModelState.AddModelError("Images", "Blog Image required!");

                _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(existBlog);
            
            }
            else
            {
                if (blog.BlogImageIds == null)
                {
                    blog.BlogImageIds = new List<int>();

                    var imagesDelete = existBlog.BlogImages.Where(x => !blog.BlogImageIds.Contains(x.Id) && x.IsPoster == null);

                    foreach (var item in imagesDelete)
                    {
                        FileManager.DeleteFile(_env.WebRootPath, "uploads/blogs", item.ImageUrl);
                    }
                    existBlog.BlogImages.RemoveAll(x => !blog.BlogImageIds.Contains(x.Id) && x.IsPoster == null);
                }
                else
                {

                    var imagesDelete = existBlog.BlogImages.Where(x => !blog.BlogImageIds.Contains(x.Id) && x.IsPoster == null);

                    foreach (var item in imagesDelete)
                    {
                        FileManager.DeleteFile(_env.WebRootPath, "uploads/blogs", item.ImageUrl);
                    }
                    existBlog.BlogImages.RemoveAll(x => !blog.BlogImageIds.Contains(x.Id) && x.IsPoster == null);


                }
                    
            }

            if (blog.PosterImage != null)
            {
                if (blog.PosterImage.ContentType != "image/png" && blog.PosterImage.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterImage", "Only png or jpeg!");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(existBlog);
                }
                if (blog.PosterImage.Length > 3145728)
                {
                    ModelState.AddModelError("PosterImage", "Maximum size 3mb!");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(existBlog);
                }

                FileManager.DeleteFile(_env.WebRootPath, "uploads/blogs", existBlog.BlogImages.FirstOrDefault(x => x.IsPoster == true).ImageUrl);
                BlogImage blogImage = new BlogImage
                {
                    BlogId = blog.Id,
                    ImageUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/blogs", blog.PosterImage),
                    IsPoster = true
                };

                existBlog.BlogImages.FirstOrDefault(x => x.IsPoster == true).ImageUrl = blogImage.ImageUrl;

            }


            if (blog.SliderImage != null)
            {
                if (blog.SliderImage.ContentType != "image/png" && blog.SliderImage.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("SliderImage", "Only png or jpeg!");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(existBlog);
                }
                if (blog.SliderImage.Length > 3145728)
                {
                    ModelState.AddModelError("SliderImage", "Maximum size 3mb!");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(existBlog);
                }

                FileManager.DeleteFile(_env.WebRootPath, "uploads/blogs", existBlog.BlogImages.FirstOrDefault(x => x.IsPoster == false).ImageUrl);

                BlogImage blogImage = new BlogImage
                {
                    BlogId = blog.Id,
                    ImageUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/blogs", blog.SliderImage),
                    IsPoster = false
                };
                existBlog.BlogImages.FirstOrDefault(x => x.IsPoster == false).ImageUrl = blogImage.ImageUrl;

            }


            if (blog.Images != null)
            {
                foreach (var imageFile in blog.Images)
                {
                    
                    if (imageFile.ContentType != "image/png" && imageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("Images", "Only png or jpeg!");

                        _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                        {
                            Title = "Opps!"
                        });

                        return View(existBlog);
                    }
                    if (imageFile.Length > 3145728)
                    {
                        ModelState.AddModelError("Images", "Maximum size 3mb!");

                        _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                        {
                            Title = "Opps!"
                        });

                        return View(existBlog);
                    }
                    BlogImage blogImage = new BlogImage
                    {
                        Blog = blog,
                        ImageUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/blogs", imageFile),
                        IsPoster = null
                    };
                    existBlog.BlogImages.Add(blogImage);
                }
            }



            AppUser user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if (user == null)
            {
                blog.Author = "Anonim";
            }
            else
            {
                blog.Author = user.FullName;
            }

            existBlog.Title = blog.Title;
            existBlog.Description = blog.Description;
            existBlog.SubTitle = blog.SubTitle;
            existBlog.SubDescription = blog.SubDescription;
            existBlog.Author = blog.Author;
            existBlog.BlogTagId = blog.BlogTagId;
           
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
            Blog blog = _sonaDb.Blogs.Include(x => x.BlogImages).FirstOrDefault(x => x.Id == id);
            if (blog == null) return NotFound(); //404

            if (blog.BlogImages != null)
            {
                foreach (var blogImage in blog.BlogImages)
                {
                    FileManager.DeleteFile(_env.WebRootPath, "uploads/blogs", blogImage.ImageUrl);
                }
            }

            _sonaDb.Blogs.Remove(blog);
            await _sonaDb.SaveChangesAsync();

            return Ok(); //200
        }
    }
}