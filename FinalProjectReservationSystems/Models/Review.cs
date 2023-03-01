using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public string? ProfilPhotoUrl { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(maximumLength: 350)]
        public string Description { get; set; }


        [Required]
        [StringLength(maximumLength: 100)]
        public string Email { get; set; }


        public float Rating { get; set; }

        public DateTime dateTime { get; set; }

        public Room Room { get; set; }
    }
}
