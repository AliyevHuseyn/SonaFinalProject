using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.ViewModels
{
	public class LoginViewModel
	{
		[Required]
		[StringLength(maximumLength: 35)]
		public string userName { get; set; }


		[Required]
		[StringLength(maximumLength: 50, MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool RememberMe { get; set; } = false;	
	}
}
