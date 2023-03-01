using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string Icon { get; set; }

        [Required]
        [StringLength(maximumLength: 25)]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 150)]
        public string Description { get; set; }

        public List<RoomService>? roomServices { get; set; }


    }
}
