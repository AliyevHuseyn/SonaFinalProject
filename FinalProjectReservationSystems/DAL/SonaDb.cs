using FinalProjectReservationSystems.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectReservationSystems.DAL
{
    public class SonaDb : IdentityDbContext
    {


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Comment>()
                .HasOne(b => b.Blog)
                .WithMany(b => b.Comments)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CommentReply>()
                .HasOne(c=>c.Blog)
                .WithMany(c=>c.CommentReplies)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
        }



        public SonaDb(DbContextOptions<SonaDb> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<ContactUsMessage> ContactUsMessages { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<CommentReply> CommentReplies { get; set; }

        public DbSet<BlogImage> BlogImages { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<RoomService> RoomServices { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<FAQ> FAQs { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<SliderSetting> sliderSetting { get; set; }
    }
}
