using BlogWebsite.Data;
using BlogWebsite.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebsite.Utilites
{
	public class DbInitializer : IDbInitializer
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public DbInitializer(ApplicationDbContext context,
							 UserManager<ApplicationUser> userManager,
							 RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public void Initialize()
		{
			if (!_roleManager.RoleExistsAsync(WebsiteRole.WebisteAdmin).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(WebsiteRole.WebisteAdmin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(WebsiteRole.WebisteAuthor)).GetAwaiter().GetResult();
				_userManager.CreateAsync(new ApplicationUser()
				{
					UserName = "admin@gmail.com",
					Email = "admin@gmail.com",
					FirstName = "Super",
					LastName = "Admin"
				}, "Admin@0011").Wait();

				var appUser = _context.applicationUsers!.FirstOrDefault(x => x.Email == "admin@gmail.com");
				if (appUser != null)
				{
					_userManager.AddToRoleAsync(appUser, WebsiteRole.WebisteAdmin).GetAwaiter().GetResult();
				}


				var listOfPages = new List<Page>()
				{
					new Page()
					{
						Title = "About Us",
						Slug = "about"
					},
				 };

				_context.pages!.AddRange(listOfPages);
				var setting = new Setting
				{
					SiteName = "Site Name",
					Title = "Site Title",
				};
				_context.settings!.Add(setting);
				_context.SaveChanges();
			}
		}
	}
}
