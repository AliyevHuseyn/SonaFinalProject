using FinalProjectReservationSystems.Models;

namespace FinalProjectReservationSystems.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }

        public List<Service> Services { get; set; }

        public List<Room> Rooms { get; set; }

        public List<Room>? FindRoom { get; set; }

        public List<RoomService> roomServices { get; set; }

        public List<Blog> Blogs { get; set; }

        public List<BlogTag> BlogTags { get; set; }

        public List<Order> Orders { get; set; } 

        public List<SliderSetting> sliderSettings { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public byte Person { get; set; }
        public float minPrice { get; set; }
        public float maxPrice { get; set; }
 
    }
}
