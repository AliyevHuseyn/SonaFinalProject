using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class BlogTag
    {
        public int Id { get; set; }

        [Required]
        [StringLength (maximumLength: 25)]
        public string Name { get; set; }

        public List<Blog>? Blogs { get; set; }
    }
}