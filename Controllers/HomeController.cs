using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsAndCategories.Context;
using ProductsAndCategories.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return Redirect("/products");
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }
        [HttpPost("product/new")]
        public IActionResult CreateProduct(Product newP)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newP);
                _context.SaveChanges();
                return RedirectToAction("Products");
            }
            else
            {
                return View("Products");
            }
        }

        [HttpPost("addcat")]
        public IActionResult AddCat(Association newAss)
        {
            _context.Add(newAss);
            _context.SaveChanges();

            return Redirect($"/product/{newAss.ProductId}");
        }
        [HttpPost("addprod")]

        public IActionResult AddProd(Association newAss)
        {
            _context.Add(newAss);
            _context.SaveChanges();

            return Redirect($"/product/{newAss.ProductId}");
        }

        [HttpGet("categories")]
        public IActionResult Categories()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        [HttpPost("category/new")]
        public IActionResult CreateCategory(Category newC)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newC);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            else
            {
                return View("Categories");
            }
        }
        [HttpGet("product/{pID}")]
        public IActionResult ShowProduct(int pID)
        {
            ViewBag.SP = _context.Products
                .Include(c => c.Categories)
                .ThenInclude(a => a.Category)
                .FirstOrDefault(p => p.ProductId == pID);
            ViewBag.Categories = _context.Categories
                .Include(p => p.Products)
                .ThenInclude(a => a.Product)
                .Where(p => p.Products.All(a => a.ProductId != pID))
                .ToList();
            return View();
        }
        [HttpGet("category/{cID}")]
        public IActionResult ShowCategory(int cID)
        {
            ViewBag.SC = _context.Categories
                .Include(p => p.Products)
                .ThenInclude(a => a.Product)
                .FirstOrDefault(c => c.CategoryId == cID);
            ViewBag.Products = _context.Products
                .Include(p => p.Categories)
                .ThenInclude(a => a.Category)
                .Where(p => p.Categories.All(a => a.CategoryId != cID))
            .ToList();
            return View();
        }

        public IActionResult Privacy()
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
