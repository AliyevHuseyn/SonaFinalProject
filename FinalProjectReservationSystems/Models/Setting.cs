using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class Setting
    {
        public int Id { get; set; }

        [StringLength(maximumLength: 50)]
        public string Key { get; set; }

        [StringLength(maximumLength: 250)]
        public string Value { get; set; }
    }
}
