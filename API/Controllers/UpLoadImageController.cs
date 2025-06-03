using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using DL.Entities;
using static System.Net.Mime.MediaTypeNames;
using System;
using BL.InterfaceServices;

namespace API.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UpLoadImageController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IImageService _imageService;

        public UpLoadImageController(IAmazonS3 s3Client, IImageService imageService)
        {
            _s3Client = s3Client;
            _imageService= imageService;
        }

        [HttpGet("presigned-url")]
        public async Task<IActionResult> GetPresignedUrl([FromQuery] string imageName, [FromQuery] int albumId, [FromQuery] int ownerId)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = "memoria-bucket-testpnoren",
                Key = imageName,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(5),
                //ContentType = "image/jpeg" // או סוג הקובץ המתאים
            };

            string url = _s3Client.GetPreSignedURL(request);

            //var image = new DL.Entities.Image
            //{
            //    Name = imageName,
            //    Type = 0, // יש להגדיר את סוג הקובץ בהתאם
            //    Size = 0, // יש לעדכן את הגודל לאחר ההעלאה
            //    AlbumId = albumId,
            //    S3URL = $"https://memoria-bucket-testpnoren.s3.amazonaws.com/{imageName}",
            //    IsDeleted = false,
            //    CreatedAt = DateTime.UtcNow,
            //    UpdatedAt = DateTime.UtcNow
            //};

            //await _imageService.AddImage(image);
            return Ok(new { url });
        }
    }
}
