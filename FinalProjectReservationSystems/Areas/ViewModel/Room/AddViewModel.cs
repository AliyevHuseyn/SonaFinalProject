using FinalProjectReservationSystems.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectReservationSystems.Areas.ViewModel.Room
{
    public class AddViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string Name { get; set; } //

        [Required]
        public double Price { get; set; } //

        public string? PosterImgUrl { get; set; }

        public string? DetailImgUrl { get; set; }

        [Required]
        public double Size { get; set; } //

        [Required]
        public byte Capacity { get; set; } //

        [Required]
        [StringLength(maximumLength: 30)]
        public string BedType { get; set; } //

        [Required]
        [StringLength(maximumLength: 500)]
        public string Description { get; set; } //

        [NotMapped]
        public IFormFile? PosterFile { get; set; } //

        [NotMapped]
        public IFormFile? DetailFile { get; set; } //

        public List<int> ServiceIds { get; set; } //?
        public List<Service>? Services { get; set; }

        public List<RoomService>? roomServices { get; set; }
    }
}
