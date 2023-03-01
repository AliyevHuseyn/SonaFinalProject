using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectReservationSystems.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public int BlogTagId { get; set; }

        [Required]
        [StringLength(maximumLength: 50)]
        public string Title { get; set; }       
        public DateTime Date { get; set; }
        [Required]
        [StringLength(maximumLength: 500)]
        public string Description { get; set; }


        [StringLength(maximumLength: 35)]
        public string? Author { get; set; }

        [StringLength(maximumLength: 50)]
        public string? SubTitle { get; set; }

        [StringLength(maximumLength: 350)]
        public string? SubDescription { get; set; }
        public BlogTag? BlogTag { get; set; }

        public List<BlogImage>? BlogImages { get; set; }

        public List<Comment>? Comments { get; set; }
        public List<CommentReply>? CommentReplies { get; set; }


        [NotMapped]
        public IFormFile? PosterImage { get; set; }


        [NotMapped]

        public IFormFile? SliderImage { get; set; }


        [NotMapped]

        public List<IFormFile>? Images { get; set; }


        [NotMapped]
        public List<int>? BlogImageIds { get; set; }

    }
}
