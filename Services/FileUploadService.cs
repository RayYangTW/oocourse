using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using personal_project.Data;
using personal_project.Helpers;
using personal_project.Models.Domain;
using personal_project.Models.FormModels;

namespace personal_project.Services
{
  public class FileUploadService : IFileUploadService
  {
    private readonly WebDbContext _db;
    private readonly IAmazonS3 _s3Client;

    public FileUploadService(IAmazonS3 s3Client, WebDbContext db)
    {
      _s3Client = s3Client;
      _db = db;
    }

    public async Task<string> UploadFileToS3Async(IFormFile file, string bucketName, string fileToS3Path)
    {
      var ext = Path.GetExtension(file.FileName);

      var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
      if (!bucketExists)
      {
        var bucketRequest = new PutBucketRequest()
        {
          BucketName = bucketName,
          UseClientRegion = true
        };
        await _s3Client.PutBucketAsync(bucketRequest);
      }

      var fileToS3 = fileToS3Path + GenerateFilenameHelper.GenerateFileRandomName() + ext;
      var objectRequest = new PutObjectRequest()
      {
        BucketName = bucketName,
        Key = fileToS3,
        InputStream = file.OpenReadStream()
      };
      await _s3Client.PutObjectAsync(objectRequest);

      return fileToS3;
    }

    public async Task<TeacherApplication> UploadCertificationsAsync(List<IFormFile> uploadCertifications, TeacherApplication application, User user, string bucketName, string certificationFileToS3Path, string filesLocateDomain)
    {
      if (uploadCertifications is not null)
      {
        foreach (var formFile in uploadCertifications)
        {
          if (formFile.Length > 0)
          {
            var ext = Path.GetExtension(formFile.FileName);
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);

            if (!bucketExists)
            {
              var bucketRequest = new PutBucketRequest()
              {
                BucketName = bucketName,
                UseClientRegion = true
              };
              await _s3Client.PutBucketAsync(bucketRequest);
            }
            else
            {
              var fileToS3 = certificationFileToS3Path + GenerateFilenameHelper.GenerateFileRandomName() + application.name + ext;
              var objectRequest = new PutObjectRequest()
              {
                BucketName = bucketName,
                Key = fileToS3,
                InputStream = formFile.OpenReadStream()
              };
              await _s3Client.PutObjectAsync(objectRequest);

              var newCertifications = new Certification
              {
                certification = filesLocateDomain + fileToS3,
                userId = user.id
              };
              application.certifications.Add(newCertifications);
            }
          }
        }
        return application;
      }
      return null;
    }
  }
}