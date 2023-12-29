using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Slugify;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity and Email Service
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
	opt.Password.RequiredLength = 6;
	opt.Password.RequireDigit = true;
	opt.Password.RequireUppercase = true;
	opt.Password.RequireNonAlphanumeric = true;
	opt.Password.RequireLowercase = true;
	opt.User.RequireUniqueEmail = true;
	opt.Tokens.ProviderMap.Add("Default", new TokenProviderDescriptor(
		typeof(DataProtectorTokenProvider<ApplicationUser>)));
})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

// SlugHelper
builder.Services.AddSingleton<ISlugHelper, SlugHelper>();

// Email service
var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
  .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Configure TokenProviderOptions and Session
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
   opt.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(2); // Thời gian phiên làm việc
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// Configure Notyf
builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });

var app = builder.Build();



builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseNotyf();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "area",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

DataSeeding();

app.Run();

void DataSeeding()
{
	using (var scope = app.Services.CreateScope())
	{
		var DbInitialize = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		DbInitialize.Initialize();
	}
}



/*Lê Đức Tài, Bùi Ngọc Na*/