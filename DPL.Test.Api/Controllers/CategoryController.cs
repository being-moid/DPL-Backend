using DPL.Test.Api.ObjectModels;
using DPL.Test.Api.ObjectModels.DTOs;
using DPL.Test.Api.ObjectModels.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DPL.Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment; // Add this field

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment) // Update constructor
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment; // Initialize the hosting environment
        }
        // GET: api/category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.Include(c => c.SubCategories).ToListAsync();
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([FromForm] CategoryDto categoryDto, IFormFile image)
        {
            // Validate the DTO
            if (string.IsNullOrWhiteSpace(categoryDto.CategoryName))
            {
                return BadRequest("Category name is required.");
            }

            // Create the directory in the application root if it doesn't exist
            var uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            // Create the Category entity
            var category = new Category
            {
                CategoryId = Guid.NewGuid(), // Set a new GUID for the entity
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                ParentGuidID = categoryDto.ParentGuidID ?? Guid.Empty, // Handle top-level categories
                // Initialize other properties
                ImageBinnary = null,
                ImageName = null,
                ImageFormat = null,
                ImageURL = null // Initialize URL as null initially
            };

            if (image != null && image.Length > 0)
            {
                // Generate the file path
                var filePath = Path.Combine(uploadsFolder, category.CategoryId + Path.GetExtension(image.FileName));

                // Save the image file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }


                // Populate category properties with image information
                category.ImageBinnary = Convert.ToBase64String(await System.IO.File.ReadAllBytesAsync(filePath));
                category.ImageName = image.FileName;
                category.ImageFormat = image.ContentType;

                // Construct the full URL for the saved image
                var request = HttpContext.Request;
                category.ImageURL = $"{request.Scheme}://{request.Host}/uploads/{category.CategoryId}{Path.GetExtension(image.FileName)}"; // Adjust according to your routing
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok( new { id = category.CategoryId , category });
        }

        // GET: api/category/autocomplete?prefix=somePrefix
        [HttpGet("autocomplete")]
        public async Task<ActionResult<IEnumerable<Category>>> Autocomplete(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return BadRequest("Prefix cannot be null or empty.");
            }

            var categories = await _context.Categories
                .Where(c => c.CategoryName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return Ok(categories);
        }
    }
}
