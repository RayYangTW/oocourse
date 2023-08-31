using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Helpers
{
  public class GenerateRandomStringHelper
  {
    public static string GenerateRandomString(int length)
    {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
      var random = new Random();

      var randomString = new char[length];
      for (int i = 0; i < length; i++)
      {
        randomString[i] = chars[random.Next(chars.Length)];
      }

      return new string(randomString);
    }
  }
}