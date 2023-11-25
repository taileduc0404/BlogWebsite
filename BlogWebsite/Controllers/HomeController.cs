﻿using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace BlogWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context,
                        ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //     public async Task<IActionResult> Index(string keyword)
        //     {
        //var vm = new HomeVM();
        //         //var setting = _context.Settings!.ToList();
        //         //vm.Title = setting[0].Title;
        //         //vm.ThumbnailUrl = setting[0].ThumbnailUrl;
        //if (string.IsNullOrEmpty(keyword))
        //{
        //             vm.posts = await _context.posts!.Include(x => x.ApplicationUsers).OrderByDescending(x => x.CreatedDate).ToListAsync();
        //         }
        //         else
        //{
        //	keyword = keyword.ToLower();
        //             vm.posts = await _context.posts!.Where(p => p.Title!.ToLower().Contains(keyword)).Include(x => x.ApplicationUsers).OrderByDescending(x => x.CreatedDate).ToListAsync();
        //}
        //return View(vm);
        //     }

        public async Task<IActionResult> Index(string keyword, int? page)
        {
            var vm = new HomeVM();

            IQueryable<Post> postsQuery = _context.posts!.Include(x => x.ApplicationUsers).OrderByDescending(x => x.CreatedDate);

            if (!string.IsNullOrEmpty(keyword))
            {
                string lowerKeyword = keyword.ToLower();
                postsQuery = postsQuery.Where(p => p.Title!.ToLower().Contains(lowerKeyword));
            }

            int pageNumber = page ?? 1;
            int pageSize = 6;

            vm.posts = await postsQuery.ToPagedListAsync(pageNumber, pageSize);

            ViewData["keyword"] = keyword; // Pass the keyword to the view

            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}