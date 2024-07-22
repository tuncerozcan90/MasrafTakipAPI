using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MasrafTakipAPI.Models;
using MasrafTakipAPI.Entity;
using Swashbuckle.AspNetCore.Annotations;

namespace MasrafTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegister">The user registration details.</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">Username already exists.</response>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registers a new user.")]
        [SwaggerResponse(200, "User registered successfully.")]
        [SwaggerResponse(400, "Username already exists.")]
        public IActionResult Register([FromBody] UserRegister userRegister)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == userRegister.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            var person = new Person
            {
                Name = userRegister.FirstName,
                Email = userRegister.Email
            };

            var user = new User
            {
                Username = userRegister.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userRegister.Password), // Şifreleme
                Person = person
            };

            _context.Persons.Add(person);
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully.");
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="userLogin">The user login details.</param>
        /// <returns>A JWT token.</returns>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="401">Unauthorized if the login details are incorrect.</response>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Authenticates a user and returns a JWT token.")]
        [SwaggerResponse(200, "Returns the JWT token.")]
        [SwaggerResponse(401, "Unauthorized if the login details are incorrect.")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userLogin.Username);
            if (user != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                var token = Generate(user);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserLogin
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }
    }

    public class UserRegister
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; set; }
    }
}
