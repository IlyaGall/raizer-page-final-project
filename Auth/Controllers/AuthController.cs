using Auth;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly UserService _userService;

    public AuthController(JwtService jwtService, UserService userService)
    {
        _jwtService = jwtService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {

        User user = await _userService.AuthenticateAsync(request.Username, request.Password);
      //   user = new User();
      //  user.TestIngData();//заполняем  тестовыми данными не из бд, так как её сейчас неть(
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                user.Id,
                user.Username,
                user.FullName,
                user.Email,
                user.Roles
            }
        });
    }

    [Authorize]
    [HttpGet("userinfo")]
    public IActionResult GetUserInfo()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.Identity.Name;
        var fullName = User.FindFirst("FullName")?.Value;
        var email = User.FindFirst("Email")?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

        return Ok(new
        {
            Id = userId,
            Username = username,
            FullName = fullName,
            Email = email,
            Roles = roles
        });
    }
}