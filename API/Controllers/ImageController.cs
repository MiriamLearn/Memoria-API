using Microsoft.AspNetCore.Mvc;
using BL.InterfaceServices;
using DL.Entities;
using BL.services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        public ImageController(IImageService fileService)
        {
            _imageService = fileService;
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
