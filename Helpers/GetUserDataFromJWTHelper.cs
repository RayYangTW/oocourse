using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using personal_project.Data;
using personal_project.Models.Domain;

namespace personal_project.Helpers
{
  public class GetUserDataFromJWTHelper
  {
    private readonly WebDbContext _db;


    public GetUserDataFromJWTHelper(WebDbContext db)
    {
      _db = db;
    }

    // To use : GetUserDataFromJWTAsync(Request.Headers["Authorization"])
    public async Task<User> GetUserDataFromJWTAsync(string authorizationHeader)
    {
      if (string.IsNullOrEmpty(authorizationHeader))
      {
        return null;
      }

      string token = authorizationHeader.Substring("Bearer ".Length);

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