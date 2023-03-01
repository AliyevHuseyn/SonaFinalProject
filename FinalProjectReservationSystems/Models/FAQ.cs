using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class FAQ
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 56)]
        public string Title { get; set; }


        [Required]
        [StringLength(maximumLength: 700)]
        public string Description { get; set; }
    }
}
