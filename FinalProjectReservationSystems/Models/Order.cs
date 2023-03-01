using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int RoomId { get; set; }


        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        public DateTime? ScanDate { get; set; }

        [Required]
        public int Person { get; set; }

        public string? OrderSerialNumber { get; set; }

        public double? TotalPrice { get; set; }

        public string? UserMail { get; set; }

        public bool? Confirm { get; set; }

        public bool? Aviable { get; set; }

        public bool? Status { get; set; }

        public Room? Room { get; set; }

    }
}
