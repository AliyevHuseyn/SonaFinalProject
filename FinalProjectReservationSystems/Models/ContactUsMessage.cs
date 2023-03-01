using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class ContactUsMessage
    {
        public int Id { get; set; }

        [StringLength(maximumLength: 50)]
        public string ClientFullName { get; set; }

        public string ClientEmail { get; set; }

        [StringLength(maximumLength: 35)]
        public string ClientMessageSubject { get; set; }

        [StringLength(maximumLength: 350)]
        public string ClientMessageText { get; set; }

        public bool Seen { get; set; }

        public bool Status { get; set; }
    }
}
