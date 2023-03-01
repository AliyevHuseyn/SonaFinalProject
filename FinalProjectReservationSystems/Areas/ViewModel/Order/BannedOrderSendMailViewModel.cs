using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Areas.ViewModel.Order
{
    public class BannedOrderSendMailViewModel
    {
        [Required]
        [StringLength(maximumLength: 35)]
        public string Subject { get; set; }


        [Required]
        [StringLength(maximumLength: 350)]
        public string Body { get; set; }

    }
}
