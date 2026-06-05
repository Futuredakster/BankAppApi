using BankApp.Models;
using DiaryApp.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DiaryApp.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public UserService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // Hashes and saves a new user — called from your EF seed, not a public endpoint
        public Users Register(string name, string plainPassword)
        {
            var user = new Users
            {
                Name = name,
                Password = BCrypt.Net.BCrypt.HashPassword(plainPassword)
            };
            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }

        // Verifies credentials — returns a JWT string on success, null on failure
        public string? Login(string name, string plainPassword)
        {
            var user = _db.Users.FirstOrDefault(u => u.Name == name);
            if (user == null) return null;

            bool passwordMatches = BCrypt.Net.BCrypt.Verify(plainPassword, user.Password);
            if (!passwordMatches) return null;

            return GenerateToken(user);
        }

        private string GenerateToken(Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_config["Jwt:ExpiresInMinutes"]!));

            // Claims are the data embedded inside the token — readable on the client
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
