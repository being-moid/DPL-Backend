using System;
using System.ComponentModel.DataAnnotations.Schema;
using global::DPL.Test.Api.ObjectModels.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DPL.Test.Api.ObjectModels.Entities
{
	public class Category
	{
		public Category()
		{
		}
		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; }
		public string Description { get; set; }
		public string ImageURL { get; set; }
        [JsonIgnore]
		public string ImageBinnary { get; set; }
		public string ImageName { get; set; }
		public string ImageFormat { get; set; }
		public Guid ParentGuidID { get; set; }
		public Category ParentCategory { get; set; }
		private ICollection<Category> _SubCategories;
		public ICollection<Category> SubCategories => _SubCategories ?? (_SubCategories = new List<Category>());
        private ICollection<Product> _products;
        public ICollection<Product> Products => _products ?? (_products= new List<Product>());
    }
}



namespace DPL.Test.Api.ObjectModels.Entities
{
    public class Product
    {
        public Product()
        {
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public string ImageURL { get; set; }
        public string Image64 { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

    }
}



namespace DPL.Test.Api.ObjectModels
{
  

    
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Category> Categories { get; set; }
            public DbSet<Product> Products { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Category>()
                    .HasMany(c => c.SubCategories)
                    .WithOne(c => c.ParentCategory)
                    .HasForeignKey(c => c.ParentGuidID)
                    .OnDelete(DeleteBehavior.Restrict);

                    modelBuilder.Entity<Product>()
                    .HasOne(o => o.Category)
                            .WithMany(o => o.Products)
                            .HasForeignKey(o => o.CategoryId);
            }
        }


}
    
namespace DPL.Test.Api.ObjectModels.DTOs
{
    public class CategoryDto
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public Guid? ParentGuidID { get; set; } // Nullable to allow top-level categories
    }
}
