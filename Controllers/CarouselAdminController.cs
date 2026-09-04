using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoolCompanyEstore.Data;
using CoolCompanyEstore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace CoolCompanyEstore.Controllers
{
    [Authorize(Roles = "SuperAdmin,ContentManager")]
    public class CarouselAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarouselAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var slides = await _context.CarouselImages.OrderBy(c => c.DisplayOrder).ToListAsync();

            var viewModel = new CarouselAdminViewModel
            {
                Slides = slides,
                NewSlide = new CarouselImage()
            };

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(CarouselAdminViewModel viewModel, IFormFile ImageFile)
        {
            var model = viewModel.NewSlide;

            if (!ModelState.IsValid)
            {
                viewModel.Slides = await _context.CarouselImages.OrderBy(c => c.DisplayOrder).ToListAsync();
                return View("Index", viewModel);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await ImageFile.CopyToAsync(ms);
                model.ImageData = ms.ToArray();
                model.ImageContentType = ImageFile.ContentType;
            }

            _context.CarouselImages.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Slide added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var slide = await _context.CarouselImages.FindAsync(id);
            if (slide != null)
            {
                _context.CarouselImages.Remove(slide);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Slide deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Slide not found!";
            }

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetCarouselImage(int id)
        {
            var slide = await _context.CarouselImages.FindAsync(id);
            if (slide == null || slide.ImageData == null)
                return NotFound();

            return File(slide.ImageData, slide.ImageContentType ?? "image/jpeg");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var slide = await _context.CarouselImages.FindAsync(id);
            if (slide == null)
                return NotFound();

            var viewModel = new CarouselAdminViewModel
            {
                NewSlide = slide,
                Slides = await _context.CarouselImages.OrderBy(c => c.DisplayOrder).ToListAsync()
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarouselAdminViewModel viewModel, IFormFile ImageFile)
        {
            var updatedSlide = viewModel.NewSlide;

            var existingSlide = await _context.CarouselImages.FindAsync(updatedSlide.Id);
            if (existingSlide == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                viewModel.Slides = await _context.CarouselImages.OrderBy(c => c.DisplayOrder).ToListAsync();
                return View("Edit", viewModel);
            }

            existingSlide.Title = updatedSlide.Title;
            existingSlide.ShortParagraph = updatedSlide.ShortParagraph;
            existingSlide.ButtonLabel = updatedSlide.ButtonLabel;
            existingSlide.ButtonLink = updatedSlide.ButtonLink;
            existingSlide.Subtitle = updatedSlide.Subtitle;
            existingSlide.Description = updatedSlide.Description;
            existingSlide.ButtonText = updatedSlide.ButtonText;
            existingSlide.DisplayOrder = updatedSlide.DisplayOrder;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await ImageFile.CopyToAsync(ms);
                existingSlide.ImageData = ms.ToArray();
                existingSlide.ImageContentType = ImageFile.ContentType;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Slide updated successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
