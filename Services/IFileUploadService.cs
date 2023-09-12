using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personal_project.Models.Domain;
using personal_project.Models.FormModels;

namespace personal_project.Services
{
  public interface IFileUploadService
  {
    Task<string> UploadFileToS3Async(IFormFile file, string bucketName, string fileToS3Path);
    Task<TeacherApplication> UploadCertificationsAsync(List<IFormFile> uploadCertifications, TeacherApplication application, User user, string bucketName, string certificationFileToS3Path, string filesLocateDomain);
  }
}