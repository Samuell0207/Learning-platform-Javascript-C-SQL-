using Microsoft.AspNetCore.Mvc;
using LearnMathAPI.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace LearnMathAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User newUser)
        {
            try
            {

                if (newUser.Chapters == null)
                {
                    newUser.Chapters = new List<string>();
                }

                if (_context.Users.Any(u => u.Email == newUser.Email))
                {
                    return BadRequest(new { error = "Email is already registered" });
                }

                newUser.ChaptersJson = JsonSerializer.Serialize(newUser.Chapters);

                newUser.Password = _passwordHasher.HashPassword(newUser, newUser.Password);

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return Ok(new
                {
                    user = new
                    {
                        newUser.Name,
                        newUser.Surname,
                        newUser.Email,
                        newUser.Grade,
                        newUser.Chapters
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // =========================
        // LOGIN
        // =========================
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == loginData.Email);

                if (user == null)
                {
                    return BadRequest(new { error = "Invalid email or password" });
                }

                var passwordHasher = new PasswordHasher<User>();

                var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginData.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    return BadRequest(new { error = "Invalid email or password" });
                }

                var chapters = string.IsNullOrEmpty(user.ChaptersJson)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(user.ChaptersJson);

                return Ok(new
                {
                    user = new
                    {
                        user.Name,
                        user.Surname,
                        user.Email,
                        user.Grade,
                        Chapters = chapters
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("user-chapters")]
        public IActionResult GetUserChapters([FromQuery] string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return NotFound(new { error = "User not found" });

            var chapters = JsonSerializer.Deserialize<List<string>>(user.ChaptersJson);

            return Ok(new { chapters });
        }
    }
}