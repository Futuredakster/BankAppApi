using DiaryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiaryApp.Controllers
{
    public class LoginRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _service.Login(request.Name, request.Password);
            if (token == null)
                return Unauthorized(new { message = "Invalid name or password." });

            return Ok(new { token });
        }
    }
}
