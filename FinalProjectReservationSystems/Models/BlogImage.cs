using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Models
{
    public class BlogImage
    {
        public int Id { get; set; }

        public int BlogId { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        public string? ImageUrl { get; set; }
        public bool? IsPoster { get; set; }
        public Blog Blog { get; set; }
    }
}
