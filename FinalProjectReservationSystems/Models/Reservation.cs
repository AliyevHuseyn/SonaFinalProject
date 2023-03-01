namespace FinalProjectReservationSystems.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public byte Guest { get; set; }

        public Room Room { get; set; }
    }
}
