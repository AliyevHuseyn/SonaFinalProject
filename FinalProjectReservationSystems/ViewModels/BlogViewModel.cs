using FinalProjectReservationSystems.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.ViewModels
{
    public class BlogViewModel
    {

        public Blog? blog { get; set; }

        [Required]
        [StringLength(maximumLength: 55)]
        public string Fullname { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(maximumLength: 175)]
        public string Text { get; set; }

        [StringLength(maximumLength: 175)]
        public string? ReplyText { get; set; }


        public List<Blog>? RelatedBlogs { get; set; }

        public List<Comment>? BlogComments { get; set; }

        public List<CommentReply>? BlogCommentsReply { get; set; }

    }
}
