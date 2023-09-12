using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using personal_project.Data;
using personal_project.Models.Domain;
using personal_project.Models.User;

namespace personal_project.Helpers
{
  public class JwtHelper
  {
    public static string CreateJWT(BaseUser user)
    {
      // put the info that you want to store in the JWT
      List<Claim> claims = new List<Claim>
      {
        new Claim("email", user.email),
        new Claim("role", user.role)
      };
      // store the key from the .env
      DotNetEnv.Env.Load();
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(System.Environment.GetEnvironmentVariable("JWT_KEY")));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken(
        issuer: System.Environment.GetEnvironmentVariable("JWT_ISSUER"),
        audience: System.Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        claims: claims,
        // jwt expire time
        expires: DateTime.Now.AddHours(8),
        signingCredentials: creds
      );
      var jwt = new JwtSecurityTokenHandler().WriteToken(token);
      return jwt;
    }
    public static long GetTokenExpirationTime(string token)
    {
      var handler = new JwtSecurityTokenHandler();
      var jwtSecurityToken = handler.ReadJwtToken(token);
      var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
      var ticks = long.Parse(tokenExp);
      return ticks;
    }
    //// Method: to get the user from JWT
    public static async Task<User> GetUserFromJWTAsync(HttpContext httpContext, WebDbContext _db)
    {
      string token = httpContext.Request.Headers["Authorization"];
      if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
      {
        return null;
      }

      token = token.Substring("Bearer ".Length);

      var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

      var userEmail = jwt.Claims.FirstOrDefault(a => a.Type == "email")?.Value;

      if (userEmail is not null)
      {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.email == userEmail);
        return user;
      }

      return null;
    }
  }
}