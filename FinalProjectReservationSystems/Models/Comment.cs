using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class Comment
    {
        public int? Id { get; set; }
        public int? BlogId { get; set; }

        [Required]
        [StringLength(maximumLength: 55)]
        public string Fullname { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(maximumLength: 250)]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string? ProfilPhoto { get; set; }
        public Blog? Blog { get; set; }

        public List<CommentReply>? commentReplies { get; set; }
    }
}
