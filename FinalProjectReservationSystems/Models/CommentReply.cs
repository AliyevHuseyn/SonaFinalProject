using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class CommentReply
    {
        public int? Id { get; set; }
        public int? CommentId { get; set; }

        public int? BlogId { get; set; }

        [StringLength(maximumLength: 55)]
        public string? Fullname { get; set; }

        [Required]
        [StringLength(maximumLength: 275)]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string? ProfilPhoto { get; set; }
        public Comment? Comment { get; set; } 

        public Blog? Blog { get; set; }
    }
}
