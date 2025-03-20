using BL.IterfaceServices;
using DL.Entities;
using Microsoft.AspNetCore.Authorization;
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
        public List<User> GetAllUsers()
        {
            return userService.GetAllUsers();
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public User GetUserById(int id)
        {
            return userService.GetUserById(id);
        }

        [HttpPost]
        public void AddUser([FromBody] User user)
        {
            userService.AddUser(user);
        }

        [HttpPut("{id}")]
        //[Authorize]
        public void UpDateUser(int id, [FromBody] User user)
        {
            userService.UpdateUser(id, user);
        }

        [HttpDelete("{id}")]
        //[Authorize(Policy = "AdminOnly")]
        public void RemoveUser(int id)
        {
            userService.RemoveUser(id);
        }

        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] DL.Entities.Login login)
        {
            try
            {
                userService.Login(login.Email, login.Password);
                return Ok("Login successful");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid email or password");
            }
        }
    }
}
