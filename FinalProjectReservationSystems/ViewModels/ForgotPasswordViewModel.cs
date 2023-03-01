using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [StringLength(maximumLength: 100, ErrorMessage = "Maksimum size 100 symbols!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
