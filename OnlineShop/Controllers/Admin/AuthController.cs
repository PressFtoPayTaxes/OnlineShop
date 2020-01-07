using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataAccess;
using OnlineShop.DTO;
using OnlineShop.Services;
using OnlineShop.Services.Interfaces;

namespace OnlineShop.Web.Controllers.Admin
{
    [Route("admin/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly OnlineShopContext context;
        private readonly ISmsService smsService;
        private readonly CookieAuthenticationService userService;

        public AuthController(OnlineShopContext context, ISmsService smsService, CookieAuthenticationService userService)
        {
            this.context = context;
            this.smsService = smsService;
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(AuthDTO userDTO)
        {
            var user = await context.Users.SingleOrDefaultAsync(usr => usr.PhoneNumber == userDTO.PhoneNumber);


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
                    if (await userService.Authenticate(user.PhoneNumber))
                        return Ok();
                    else
                        return Unauthorized();
                }
                else
                {
                    return BadRequest("Invalid verification code");
                }
            }
        }
    }
}