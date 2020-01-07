using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.DataAccess;
using OnlineShop.Domain;
using OnlineShop.Options;

namespace OnlineShop.Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ShopController : ControllerBase
    {
        private readonly OnlineShopContext context;
        private readonly string jwtSecret;
        private readonly User user;

        public ShopController(OnlineShopContext context, IOptions<SecretOptions> jwtSecret)
        {
            this.context = context;
            this.jwtSecret = jwtSecret.Value.Secret;
            user = GetCurrentUser();
        }

        private User GetCurrentUser()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };

            var principal = tokenHandler.ValidateToken(Request.Headers["Authentications"].ToString().Split(" ")[1], 
                                                       validationParameters,
                                                       out _);
            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            return context.Users.SingleOrDefault(usr => usr.Id == Guid.Parse(userId));
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var category = await context.Categories.SingleOrDefaultAsync(cat => cat.Id == id);
            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1)
        {
            int startIndex = (page - 1) * 10;
            int endIndex = startIndex + 9;
            var products = new List<Product>();
            var allProducts = await context.Products.ToListAsync();

            for(int i = startIndex; i <= endIndex; i++)
            {
                try
                {
                    products.Add(allProducts[i]);
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }

            return Ok(new { page = page, products = products });
        }

        [HttpPut]
        public async Task<IActionResult> PutProductInCart(Guid id, int count)
        {
            var product = await context.Products.SingleOrDefaultAsync(prod => prod.Id == id);

            context.ProductsInCarts.Add(new ProductInCart
            {
                Cart = user.Cart,
                Count = count,
                Product = product
            });
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> PutProductToFavorites(Guid id)
        {
            var product = await context.Products.SingleOrDefaultAsync(prod => prod.Id == id);

            context.FavoriteProducts.Add(new FavoriteProduct
            {
                Product = product,
                User = user
            });

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var favorites = await context.FavoriteProducts.Where(product => product.User == user).ToListAsync();

            return Ok(favorites);
        }
    }
}