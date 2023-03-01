using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.ViewModels
{
    public class RoomViewModel
    {
        public Room? Room { get; set; }

        public List<Review>? Reviews { get; set; }

        [Required]
        [StringLength(maximumLength: 150)]
        public string FullName { get; set; }

        [Required]
        [StringLength(maximumLength: 150)]
        public string Description { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        public string Email { get; set; }

        public int Rating { get; set; }

    }
}
