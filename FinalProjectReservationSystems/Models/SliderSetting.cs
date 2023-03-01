using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectReservationSystems.Models
{
    public class SliderSetting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 45)]
        public string Title { get; set; }

        [Required]
        [StringLength(maximumLength: 250)]
        public string Description { get; set; }

        [Required]
        [StringLength(maximumLength: 15)]
        public string ButtonText { get; set; }

        [Required]
        public string ButtonUrl { get; set; }

    }
}
