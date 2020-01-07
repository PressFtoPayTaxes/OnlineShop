using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataAccess;
using OnlineShop.Domain;
using OnlineShop.DTO;
using OnlineShop.Services;
using OnlineShop.Services.Interfaces;

namespace OnlineShop.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISmsService smsService;
        private readonly OnlineShopContext context;
        private readonly JwtAuthenticationService userService;

        public AuthController(ISmsService smsService, OnlineShopContext context, JwtAuthenticationService userService)
        {
            this.smsService = smsService;
            this.context = context;
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserDTO userDTO)
        {
            var user = await context.Users.SingleOrDefaultAsync(usr => usr.PhoneNumber == userDTO.PhoneNumber);

            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = userDTO.PhoneNumber,
                    FullName = userDTO.FullName,
                    NotificationDeviceId = userDTO.NotificationDeviceId,
                };
                var userEntity = context.Users.Add(user);

                var cart = new Cart
                {
                    User = user,
                    UserId = user.Id
                };
                context.Carts.Add(cart);
                userEntity.Entity.Cart = cart;

                await context.SaveChangesAsync();
            }

            if (string.IsNullOrWhiteSpace(userDTO.VerificationCode))
            {
                Random random = new Random();
                var code = random.Next(1000, 9999).ToString();
                user.VerificationCode = code;

                await smsService.SendVerificationCode(user.PhoneNumber, user.VerificationCode);
                return Ok("We sent a verification code on your phone. Please send it back with your next request");
            }
            else
            {
                if (userDTO.VerificationCode == user.VerificationCode)
                {
                    user.VerificationCode = "";
                    return Ok(userService.Authenticate(user.PhoneNumber));
                }
                else
                {
                    return BadRequest("Invalid verification code");
                }
            }
        }
    }
}