using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.DataAccess;
using OnlineShop.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Services
{
    public class JwtAuthenticationService
    {
        private readonly OnlineShopContext context;
        private readonly string jwtSecret;

        public JwtAuthenticationService(OnlineShopContext context, IOptions<SecretOptions> jwtSecret)
        {
            this.context = context;
            this.jwtSecret = jwtSecret.Value.Secret;
        }
        public async Task<string> Authenticate(string phoneNumber)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber);

            if (existingUser == null || existingUser.DeletedDate != null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.MobilePhone, existingUser.PhoneNumber),
                    new Claim(ClaimTypes.Name, existingUser.FullName)
                }),
                Expires = DateTime.UtcNow.AddYears(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
