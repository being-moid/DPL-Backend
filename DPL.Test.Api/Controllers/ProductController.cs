using DPL.Test.Api.ObjectModels;
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
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] ProductDto productDto)
        {
            // Validate product data
            if (string.IsNullOrWhiteSpace(productDto.ProductName))
            {
                return BadRequest("Product name is required.");
            }

            if (productDto.Amount <= 0)
            {
                return BadRequest("Amount should be greater than 0.");
            }
            IFormFile image = productDto.Image;
            // Map DTO to Entity
            var product = new Product
            {
                ProductID = Guid.NewGuid(),
                ProductName = productDto.ProductName,
                Amount = productDto.Amount,
                CategoryId = productDto.CategoryId,
                
            };

            // Handle image upload
            if (image != null && image.Length > 0)
            {
                // Get the application's root folder
                var appRootPath = Path.Combine(Directory.GetCurrentDirectory(),  "uploads");

                // Create the uploads directory if it doesn't exist
                if (!Directory.Exists(appRootPath))
                {
                    Directory.CreateDirectory(appRootPath);
                }

                // Generate the file path
                var fileExtension = Path.GetExtension(image.FileName);
                var fileName = $"{product.ProductID}{fileExtension}";
                var filePath = Path.Combine(appRootPath, fileName);

                // Save the image file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Set the image URL
                product.ImageURL = $"http://localhost:5000/uploads/{fileName}";
                // Optionally, set the base64 encoded image string if needed
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    product.Image64 = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            else
            {
                // Handle scenario where no image is provided (set a default image or null)
                product.ImageURL = null; // or a default URL if applicable
                product.Image64 = null;
            }

            // Save product to database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Return the newly created product
            return Ok(new { id = product.ProductID, product });
        }

    }
}
public class ProductDto
{
    public string ProductName { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    
    public IFormFile Image { get; set; }
}
