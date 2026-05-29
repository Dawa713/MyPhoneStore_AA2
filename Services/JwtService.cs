using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api_clase.Models;
using Microsoft.IdentityModel.Tokens;

namespace api_clase.Services
{
    /// <summary>
    /// Genera tokens JWT para los usuarios autenticados.
    /// El token contiene: id del usuario, email y rol.
    /// </summary>
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Customer customer)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims: datos que se guardan dentro del token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, customer.Email),
                new Claim(ClaimTypes.Role, customer.Role),        // "ADMIN" o "CLIENT"
                new Claim("name", customer.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiration = DateTime.UtcNow.AddHours(
                _config.GetValue<int>("Jwt:ExpirationHours"));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
