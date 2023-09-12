using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using personal_project.Helpers;

namespace personal_project.Services
{
  public class FileUploadService : IFileUploadService
  {
    private readonly IAmazonS3 _s3Client;

    public FileUploadService(IAmazonS3 s3Client)
    {
      _s3Client = s3Client;
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
  }
}