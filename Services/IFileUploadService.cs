using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personal_project.Services
{
  public interface IFileUploadService
  {
    Task<string> UploadFileToS3Async(IFormFile file, string bucketName, string fileToS3Path);
  }
}