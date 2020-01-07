using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataAccess;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Services
{
    public class CookieAuthenticationService
    {
        private readonly OnlineShopContext context;
        private readonly IHttpContextAccessor accessor;

        public CookieAuthenticationService(OnlineShopContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        public async Task<bool> Authenticate(string phoneNumber)
        {
            var existingUser = await context.Users.SingleOrDefaultAsync(user => user.PhoneNumber == phoneNumber);

            if (existingUser == null || existingUser.DeletedDate != null)
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                    new ClaimsPrincipal(claimsIdentity),
                                                    authProperties);

            return true;
        }
    }
}
