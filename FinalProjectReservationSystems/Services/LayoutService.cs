using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectReservationSystems.Services
{
	public class LayoutService
	{
        private readonly SonaDb _sonaDb;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<AppUser> _userManager;

        public LayoutService(SonaDb sonaDb,IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
		{
            _sonaDb = sonaDb;
            _httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
        }

		public async Task<AppUser> GetUser()
		{
			AppUser user = null;

			if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
			{
				user = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
			}
			return user;
		}

		public async Task<List<ContactUsMessage>> GetMessage()
		{
			List<ContactUsMessage> contactUsMsg = null;

            contactUsMsg = await _sonaDb.ContactUsMessages.ToListAsync();

			return contactUsMsg;
		}


		public async Task<List<Setting>> GetSettingsAsync()
		{
			return await _sonaDb.Settings.ToListAsync();
		}

	}
}
