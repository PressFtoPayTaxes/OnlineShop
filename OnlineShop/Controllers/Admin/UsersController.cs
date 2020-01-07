using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataAccess;
using OnlineShop.DTO;

namespace OnlineShop.Web.Controllers.Admin
{
    [Route("admin/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly OnlineShopContext context;

        public UsersController(OnlineShopContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await context.Users.Where(user => user.DeletedDate == null).ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Id == id);
            return Ok(user);
        } 

        [HttpPut]
        public async Task<IActionResult> EditUser(UserEditDTO userDTO)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Id == userDTO.Id);

            if (userDTO.FullName != null && userDTO.FullName != "")
                user.FullName = userDTO.FullName;
            if (userDTO.PhoneNumber != null && userDTO.PhoneNumber != "")
                user.PhoneNumber = userDTO.PhoneNumber;
            if (userDTO.NotificationDeviceId != null && userDTO.NotificationDeviceId != "")
                user.NotificationDeviceId = userDTO.NotificationDeviceId;

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Id == id);
            user.DeletedDate = DateTime.Now;
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}