using FinalProjectReservationSystems.Abstractions.Services;
using FinalProjectReservationSystems.DAL;
using FinalProjectReservationSystems.Models;
using FinalProjectReservationSystems.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<SonaDb>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddDbContext<SonaDb>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//,
//sqlServerOptionsAction => sqlServerOptionsAction.CommandTimeout(60)));



// Add services to the container.
builder.Services.AddControllersWithViews().AddNToastNotifyToastr().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);


builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{

	opt.Password.RequiredLength = 0;
	opt.Password.RequireNonAlphanumeric = true;
	opt.Password.RequiredUniqueChars = 0;
	opt.Password.RequireUppercase = true;
	opt.Password.RequireLowercase = true;
	opt.Password.RequireDigit = true;
	opt.SignIn.RequireConfirmedEmail= true;
	opt.User.RequireUniqueEmail= true;
}).AddEntityFrameworkStores<SonaDb>().AddDefaultTokenProviders();



builder.Services.AddAuthentication()
        .AddGoogle(opts =>
        {
            opts.ClientId = "475669942174-plh4tjm5i8t3kqkf2vpvri3cp00q85qh.apps.googleusercontent.com";
            opts.ClientSecret = "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";
            //opts.SignInScheme = IdentityConstants.ExternalScheme;
        });






builder.Services.AddScoped<LayoutService>();

builder.Services.AddScoped<IEmailService, EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseNToastNotify();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
	  name: "areas",
	  pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
	);




app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
