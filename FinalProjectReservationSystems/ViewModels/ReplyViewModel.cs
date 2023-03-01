using FinalProjectReservationSystems.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.ViewModels
{
    public class ReplyViewModel
    {
        [Required]
        [StringLength (maximumLength: 35)]
        public string Subject { get; set;}

        [Required]
        [StringLength (maximumLength: 350)]
        public string MessageText { get; set; }
    }
}
