using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data;  
using System.Security.Policy;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using NToastNotify;
using FinalProjectReservationSystems.DAL;
using static QRCoder.PayloadGenerator;
using System.Text.RegularExpressions;
using System.Text;

namespace FinalProjectReservationSystems.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class DashboardController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _env;

        public DashboardController(IEmailService emailService, IToastNotification toastNotification, IWebHostEnvironment env)
        {
            _emailService = emailService;
            _toastNotification = toastNotification;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
