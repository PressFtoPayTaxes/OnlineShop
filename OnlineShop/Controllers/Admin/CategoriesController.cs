using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataAccess;
using OnlineShop.Domain;
using OnlineShop.DTO;

namespace OnlineShop.Web.Controllers.Admin
{
    [Route("admin/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly OnlineShopContext context;

        public CategoriesController(OnlineShopContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await context.Categories.Where(category => category.DeletedDate == null).ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var category = await context.Categories.SingleOrDefaultAsync(cat => cat.Id == id);

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDTO categoryDTO)
        {
            var products = new List<Product>();

            foreach (var id in categoryDTO.Products)
            {
                products.Add(await context.Products.SingleOrDefaultAsync(product => product.Id == id));
            }

            var category = new Category
            {
                ImageUrl = categoryDTO.ImageUrl,
                Name = categoryDTO.Name,
                Products = products
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditCategory(CategoryDTO categoryDTO)
        {
            var category = await context.Categories.SingleOrDefaultAsync(cat => cat.Id == categoryDTO.Id);

            if (categoryDTO.Name != null && categoryDTO.Name != "")
                category.Name = categoryDTO.Name;
            if (categoryDTO.ImageUrl != null && categoryDTO.ImageUrl != "")
                category.ImageUrl = categoryDTO.ImageUrl;

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("admin/categories/addproduct")]
        public async Task<IActionResult> AddProduct(ProductCategoryDTO dto)
        {
            var category = await context.Categories.SingleOrDefaultAsync(cat => cat.Id == dto.CategoryId);
            var product = await context.Products.SingleOrDefaultAsync(prod => prod.Id == dto.ProductId);

            category.Products.Add(product);

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("admin/categories/removeproduct")]
        public async Task<IActionResult> RemoveProduct(ProductCategoryDTO dto)
        {
            var category = await context.Categories.SingleOrDefaultAsync(cat => cat.Id == dto.CategoryId);
            var product = await context.Products.SingleOrDefaultAsync(prod => prod.Id == dto.ProductId);

            category.Products.Remove(product);

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await context.Categories.SingleOrDefaultAsync(cat => cat.Id == id);
            category.DeletedDate = DateTime.Now;

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}