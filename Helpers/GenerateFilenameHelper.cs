using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Helpers
{
  public class GenerateFilenameHelper
  {
    public static string GenerateFileRandomName()
    {
      DateTime currentDate = DateTime.Now;
      string fileNameSetting = currentDate.ToString("yyyyMMddHHmmssfff");
      return fileNameSetting;
    }
  }
}