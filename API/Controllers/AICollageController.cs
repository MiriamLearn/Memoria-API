using BL.InterfaceServices;
using DL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class AICollageController : ControllerBase
    {
        private readonly IAICollageService _aiCollageService;

        public AICollageController(IAICollageService aiCollageService)
        {
            _aiCollageService = aiCollageService;
        }

        [HttpPost("design")]
        public async Task<IActionResult> GenerateCollageDesign([FromBody] CollagePromptRequest request)
        {
            try
            {
                var design = await _aiCollageService.GenerateCollageDesignAsync(request.Prompt);
                return Ok(design);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
