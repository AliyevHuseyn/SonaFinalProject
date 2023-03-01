using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.ViewModels
{
    public class myAccountViewModel
    {
        [StringLength(maximumLength: 50)]
        public string? NewUserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? NewEmail { get; set; }


        [StringLength(maximumLength: 50)]
        public string? NewFullName { get; set; }

        public string? NewProfilPhotoUrl { get; set; }
        public IFormFile? newProfilPhotoFile { get; set; }


        [DataType(DataType.Password)]
        public string Security { get; set; }
    }
}
