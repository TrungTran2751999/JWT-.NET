using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace app.Controllers;

[ApiController]
[Route("api/login")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private IHttpContextAccessor contextAccessor;
    private readonly JwtSettings jwtSetting;
    public UserController(ApplicationDbContext dbContext, IOptions<JwtSettings> options, IHttpContextAccessor contextAccessor){
        this.dbContext = dbContext;
        this.jwtSetting = options.Value;
        this.contextAccessor = contextAccessor;
    }
    [HttpPost]
    public async Task<IActionResult> Authenticate([FromBody] UserCred user){
        var result = await dbContext.Users.FirstOrDefaultAsync(param=>param.username==user.username && param.password==user.password);
        if(result==null){
            return Unauthorized();
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(this.jwtSetting.securitykey);
        var tokendesc = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new Claim[]{new Claim(ClaimTypes.Name, result.username), new Claim(ClaimTypes.Role, result.role)}
            ),
            Expires = DateTime.Now.AddDays(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokendesc);
        string finalToken = tokenHandler.WriteToken(token);
        
        contextAccessor.HttpContext.Response.Cookies.Append("token",finalToken, new CookieOptions{HttpOnly=true});
        return Ok(finalToken);
    }
}