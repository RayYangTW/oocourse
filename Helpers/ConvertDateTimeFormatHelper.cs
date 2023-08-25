using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Helpers
{
  public class ConvertDateTimeFormatHelper
  {
    public static string ConvertDateTimeFormat(string originalDate)
    {
      if (DateTime.TryParse(originalDate, out DateTime parsedDate))
      {
        return parsedDate.ToString("yyyy/MM/dd - HH:mm");
      }
      return originalDate;
    }
  }
}