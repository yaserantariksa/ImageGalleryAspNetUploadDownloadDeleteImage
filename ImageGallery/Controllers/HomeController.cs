using ImageGallery.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace ImageGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private readonly string wwwrootDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
        
        public IActionResult Index()
        {
            List<string> images = Directory.GetFiles(wwwrootDir, "*.png").Select(Path.GetFileName).ToList();
            return View(images);
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile imageFile)
        {
            if (imageFile != null)
            {
                var path = Path.Combine(wwwrootDir, DateTime.Now.Ticks.ToString() + Path.GetExtension(imageFile.FileName));

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> DownloadImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images",imagePath);

                var memory = new MemoryStream();
                using(var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var contentType = "APPLICATION/octet-stream";
                var fileName = Path.GetFileName(path);
                return File(memory, contentType, fileName);
            }
            return View();
        }

        public async Task<IActionResult> DeleteImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", imagePath);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return RedirectToAction("Index");
            }
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