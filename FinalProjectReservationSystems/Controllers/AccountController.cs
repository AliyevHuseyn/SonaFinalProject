using FinalProjectReservationSystems.Abstractions.Services;
using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Helpers;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;

namespace FinalProjectReservationSystems.Controllers
{
    public class AccountController : Controller
    {
        private readonly SonaDb _sonaDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;
        private readonly IToastNotification _toastNotification;

        public AccountController(SonaDb sonaDb, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env, IEmailService emailService, IToastNotification toastNotification)
        {
            _sonaDb = sonaDb;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _env = env;
            _emailService = emailService;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> CreateRole()
        {
            IdentityRole role1 = new IdentityRole("SuperAdmin");
            IdentityRole role2 = new IdentityRole("Admin");
            IdentityRole role3 = new IdentityRole("Member");

            await _roleManager.CreateAsync(role1);
            await _roleManager.CreateAsync(role2);
            await _roleManager.CreateAsync(role3);

            return Ok("Role Created");
        }

        public async Task<IActionResult> CreateSuperAdmin()
        {
            AppUser superAdmin = new AppUser
            {
                FullName = "Huseyn Aliyev",
                UserName = "Whosein_Ali",
                Email = "huseynali1119@gmail.com",
                ProfilPhotoUrl = "SuperAdminPP.jpeg",
                roleName = "SuperAdmin"
            };

            superAdmin.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(superAdmin, "Test-123");

            if(!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    return Content(item.Description);
                }
            }

            return Ok("Created");
        }

        public async Task<IActionResult> AddToRole()
        {
            AppUser user = await _userManager.FindByNameAsync("Whosein_Ali");

            var res = await _userManager.AddToRoleAsync(user, "SuperAdmin");

            return Ok(res);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(registerVM.UserName);

            if (user != null)
            {
                ModelState.AddModelError("UserName", "user name has taken");
                return View();
            }


			user = await _userManager.FindByEmailAsync(registerVM.Email);
			if (user != null)
			{
				ModelState.AddModelError("Email", "This Email has taken!");
				return View();
			}


            if(registerVM.ProfilPhoto == null)
            {
                user = new AppUser
                {
                    UserName = registerVM.UserName,
                    FullName = registerVM.FullName,
                    Email = registerVM.Email,
                    ProfilPhotoUrl = "anonymousUserPP.png",
                    roleName = "Member"
                };
            }
            else
            {
                if (registerVM.ProfilPhoto.ContentType != "image/png" && registerVM.ProfilPhoto.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ProfilPhoto", "Only png ve jpeg file type");

                    return View();
                }

                if (registerVM.ProfilPhoto.Length > 5300000)
                {
                    ModelState.AddModelError("ProfilPhoto", "Profil Photo image max size 5 mb");

                    return View();
                }

                user = new AppUser
                {
                    UserName = registerVM.UserName,
                    FullName = registerVM.FullName,
                    Email = registerVM.Email,
                    ProfilPhotoUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/profilPhotos", registerVM.ProfilPhoto),
                    roleName = "Member"
                };
            }

            var res = await _userManager.CreateAsync(user, registerVM.Password);

            if (!res.Succeeded)
            {
                foreach (var err in res.Errors)
                {
                    ModelState.AddModelError("Password", err.Description);
                    return View();
                }
            }

            await _userManager.AddToRoleAsync(user, "Member");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, Email = user.Email }, Request.Scheme);

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(Path.Combine(_env.WebRootPath, "additional/fileReader", "confirmationEmail.html"), encoding: Encoding.UTF8))
            {
                html = sr.ReadToEnd();
            }
            html = Regex.Replace(html, "confirmlink", confirmationLink);

            _emailService.Send(user.Email, "Account Confirmation", html);

            _toastNotification.AddAlertToastMessage(message: "Last Step Confirm Your Email!", new
                    ToastrOptions
            {
                Title = "Please check email"
            });

            return View();
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if(user==null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if(!result.Succeeded)
            {
                return NotFound();
            }

            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("index", "home");
        }

        public  async Task<IActionResult> SuccessfullyRegistered()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM, string? returnUrl)
		{
            if(!ModelState.IsValid)
            {
                return View();
            }

			AppUser user = await _userManager.FindByNameAsync(loginVM.userName);

			if (user == null)
			{
				ModelState.AddModelError("", "Username or password incorrect");
				return View();
			}

            if(!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please comfirm your email (Check email)");
                return View();
            }

			var res = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, false);

			if (!res.Succeeded)
			{
				ModelState.AddModelError("", "Username or password incorrect");
				return View();
			}

			return RedirectToAction("Index", "home");
		}

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordVM);
            }

            var appUser = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("Email", "Email not found");
                return View(forgotPasswordVM);
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

            string link = Url.Action("ResetPassword", "Account", new { userid = appUser.Id, token = token }, HttpContext.Request.Scheme);

            _emailService.Send(forgotPasswordVM.Email, "Reset Password", link);

            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> ResetPassword(string userid, string token)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(token)) { return BadRequest(); }
            AppUser appUser = await _userManager.FindByIdAsync(userid);
            if (appUser == null) { return BadRequest(); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordVM, string userid, string token)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(token)) { return BadRequest(); }
            if(!ModelState.IsValid) { return View(resetPasswordVM); }
            AppUser appUser = await _userManager.FindByIdAsync(userid);
            if (appUser == null) { return BadRequest(); }

            var res = await _userManager.ResetPasswordAsync(appUser, token, resetPasswordVM.NewPassword);
            if (res.Succeeded) { return RedirectToAction("Login"); }
            return BadRequest();
        }
        public async Task<IActionResult> myAccount()
        {
            if(User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                if(user == null)
                {
                    return BadRequest();
                }

                _toastNotification.AddInfoToastMessage(message: "Do you want change your information?", new
                    ToastrOptions
                {
                    Title = "Welcome !"
                });

                return View();

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> myAccount(myAccountViewModel newInfo)
        {
            if (newInfo.Security == null)
            {

                _toastNotification.AddErrorToastMessage(message: "Please enter your currently password.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });

                return View(newInfo);
            }

            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await _signInManager.PasswordSignInAsync(user, newInfo.Security, false, false);

            if (!result.Succeeded)
            {

                ModelState.AddModelError("Security", "Password incorrect");

                _toastNotification.AddErrorToastMessage(message: "Please enter correctly.", new
                    ToastrOptions
                {
                    Title = "Error"
                });

                return View(newInfo);
            }

            if (newInfo.newProfilPhotoFile != null)
            {
                if (newInfo.newProfilPhotoFile.ContentType != "image/png" && newInfo.newProfilPhotoFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("newProfilPhotoFile", "Only png ve jpeg file type");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                        ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(newInfo);
                }

                if (newInfo.newProfilPhotoFile.Length > 5300000)
                {
                    ModelState.AddModelError("newProfilPhotoFile", "Profile Photo image max size 5 mb");

                    _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                        ToastrOptions
                    {
                        Title = "Opps!"
                    });

                    return View(newInfo);
                }

                if (user.ProfilPhotoUrl == "anonymousUserPP.png")
                {
                    user.ProfilPhotoUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/profilPhotos", newInfo.newProfilPhotoFile);
                }
                else
                {
                    FileManager.DeleteFile(_env.WebRootPath, "uploads/profilPhotos", user.ProfilPhotoUrl);

                    user.ProfilPhotoUrl = FileManager.SaveFile(_env.WebRootPath, "uploads/profilPhotos", newInfo.newProfilPhotoFile);
                }
            }

            if(newInfo.NewEmail != null)
            {
                user.Email = newInfo.NewEmail;
                user.NormalizedEmail= newInfo.NewEmail.ToUpper();
            }

            if (newInfo.NewUserName != null)
            {
                user.UserName = newInfo.NewUserName;
                user.NormalizedUserName = newInfo.NewUserName.ToUpper();
            }
            if(newInfo.NewFullName != null)
            {
                user.FullName = newInfo.NewFullName;
            }

            await _sonaDb.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage(message: "Your informations changed!", new
                    ToastrOptions
            {
                Title = "Successful"
            });

            return RedirectToAction("Index", "Home");
        }

        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Account", new { ReturnUrl = ReturnUrl });
            //Google giris ucun user'e url yaratmaq
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            //Baglanti funksiyalari elde eleyib, teyin edir
            return new ChallengeResult("Google", properties);
            //ChallengeResult; hesab dogrulama eleyir
        }

        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            //Xarici giris bilgilerini ve hansi platformadan oldugunu ozunde saxlayan obyektdi
            if (loginInfo == null)
                return RedirectToAction("Login");
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult loginResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                //Giriş etmek
                if (loginResult.Succeeded)
                    return Redirect(ReturnUrl);
                else
                {
                    AppUser HasUser = await _userManager.FindByEmailAsync(loginInfo.Principal.FindFirst(ClaimTypes.Email).Value);
                    if (HasUser is not null)
                    {
                        _toastNotification.AddInfoToastMessage(message: "Please login with password", new ToastrOptions
                        {
                            Title = "There is an account in this mail"
                        });
                        return RedirectToAction(nameof(Login));
                    }
                    //Eger istifadeci login ola bilmirse demek ki, hesabi yoxdu
                    //Register eleyib yeni bir hesab yaradib, login etmek lazimdi
                    AppUser user = new AppUser
                    {
                        Email = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value,
                        UserName = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value,
                        FullName = loginInfo.Principal.FindFirst(ClaimTypes.Name).Value,
                        roleName = "Member",
                        EmailConfirmed = true,
                        ProfilPhotoUrl = "anonymousUserPP.png"
                    };

                    //Gelen deyerleri menimsedir
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    //Register edirik
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Member");

                        //Users Table'da datalar dusur
                        IdentityResult addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                        //User melumatlari AspNetUserLogins table'de yadda saaxlanir
                        if (addLoginResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, true);
                            //await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                            return Redirect(ReturnUrl);
                        }
                    }
                }
            }
            return RedirectToAction("register", "account");
        }
    }
}
