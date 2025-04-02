using BL.IterfaceServices;
using BL.services;
using DL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<List<User>> GetAllUsers()
        {
            return await userService.GetAllUsers();
        }

        [HttpGet("{id}")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<User> GetUserById(int id)
        {
            return await userService.GetUserById(id);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            try
            {
                await userService.AddUser(user);
                return Ok(new { success = true, User = user });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid email or password");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task UpDateUser(int id, [FromBody] User user)
        {
           await userService.UpdateUser(id, user);
        }

        [HttpDelete("{id}")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task RemoveUser(int id)
        {
           await userService.RemoveUser(id);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] DL.Entities.Login login)
        {
            try
            {
                //await userService.Login(login.Email, login.Password);
                // return Ok(new { success = true, message = "Login successful", User = new { user.Id, user.Name, user.Email } });
                var user = await userService.Login(login.Email, login.Password);

                if (user == null)
                    return Unauthorized(new { Message = "Invalid email or password" });
                var token = userService.GenerateJwtToken(user.Name, new[] { "User" });
                return Ok(new
                {
                    success = true,
                    Message = "Login successful",
                    Token = token,
                    User = new { user.Id, user.Name, user.Email }
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "Invalid credentials" });
            }
        }
    }
}
