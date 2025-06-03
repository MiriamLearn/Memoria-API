using BL.InterfaceServices;
using DL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _albumService;


        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Album>>> GetAllAlbums()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("User is not authenticated or token is missing required claim.");
                }
                var userId = int.Parse(userIdClaim);

                //var userId =int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var albums = await _albumService.GetAllAlbums(userId);
                return Ok(albums);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Album>> GetAlbumById(int id)
        {
            try
            {
                var album = await _albumService.GetAlbumById(id);
                return Ok(album);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Album with ID {id} not found.");
            }
        }

        //[HttpGet("user/{userId}")]
        //public async Task<ActionResult<IEnumerable<Album>>> GetUserAlbums(int userId)
        //{
        //    var albums = await _albumService.GetUserAlbums(userId);
        //    return Ok(albums);
        //}

        [HttpPost]
        public async Task<ActionResult<Album>> AddAlbum([FromBody] Album album)
        {
            //await _albumService.AddAlbum(album);
            //return CreatedAtAction(nameof(GetAlbumById), new { id = album.Id }, album);
            if (album == null)
            {
                return BadRequest("Album is null");
            }

            try
            {
                var createdAlbum = await _albumService.AddAlbum(album);

                return CreatedAtAction(nameof(GetAlbumById), new { id = createdAlbum.Id }, createdAlbum);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(int id, [FromBody] Album album)
        {
            if (id != album.Id)
            {
                return BadRequest("Album ID mismatch.");
            }

            try
            {
                await _albumService.UpdateAlbum(id, album);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Album with ID {id} not found.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAlbum(int id)
        {
            try
            {
                await _albumService.RemoveAlbum(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Album with ID {id} not found.");
            }
        }
    }
}
