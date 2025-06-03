using Microsoft.AspNetCore.Mvc;
using BL.InterfaceServices;
using DL.Entities;
using BL.services;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ImageController> _logger;
        public ImageController(IImageService fileService, HttpClient httpClient, ILogger<ImageController> logger)
        {
            _imageService = fileService;
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Image>>> GetAllImages()
        {
            var images = await _imageService.GetAllImages();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Image>> GetImageById(int id)
        {
            try
            {
                var image = await _imageService.GetImageById(id);
                return Ok(image);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpGet("proxy")]
        [Authorize]
        public async Task<IActionResult> ProxyImage([FromQuery] string url)
        {
            try
            {
                // בדיקת אבטחה - רק URLs מה-S3 bucket שלך
                if (string.IsNullOrEmpty(url) || !url.Contains("memoria-bucket-testpnoren.s3.us-east-1.amazonaws.com"))
                {
                    _logger.LogWarning($"Invalid image URL attempted: {url}");
                    return BadRequest("Invalid image URL");
                }

                _logger.LogInformation($"Proxying image: {url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";

                    // הוספת CORS headers
                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    Response.Headers.Add("Access-Control-Allow-Methods", "GET");
                    Response.Headers.Add("Access-Control-Allow-Headers", "*");

                    return File(content, contentType);
                }

                _logger.LogWarning($"Failed to fetch image: {response.StatusCode} for URL: {url}");
                return NotFound($"Image not found: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching image from URL: {url}");
                return BadRequest($"Error fetching image: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddImage([FromBody] Image image)
        {
            await _imageService.AddImage(image);
            return CreatedAtAction(nameof(GetImageById), new { id = image.Id }, image);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateImage(int id, [FromBody] Image image)
        {
            try
            {
                await _imageService.UpdateImage(id, image);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveImage(int id)
        {
            try
            {
                await _imageService.RemoveImage(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
