using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectReservationSystems.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }

        public string? ProfilPhotoUrl { get;set; }

        public string? roleName { get; set; }

        [NotMapped]
        public IFormFile? ProfilPhoto { get; set; }
    }
}
