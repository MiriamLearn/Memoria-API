using BL.IterfaceServices;
using DL.Entities;
using DL;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace BL.services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IDataContext _dataContext;

        public UserService(IDataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public string GenerateJwtToken(string username, string[] roles)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var user = _dataContext.Users.FirstOrDefault(u => u.Name == username);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            var userId = user.Id;

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

            // הוספת תפקידים כ-Claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<List<User>> GetAllUsers()
        {
            //return _dataContext.Users.ToList();
            var list = await _dataContext.Users.ToListAsync();
            return list;
        }
        public async Task<User> GetUserById(int id)
        {
            //BabyValidation.ValidateBabyId(id);
            return _dataContext.Users.Where(u => u.Id == id).FirstOrDefault();
        }
        public async Task AddUser(User user)
        {
           
                _dataContext.Users.Add(user);
              await  _dataContext.SaveChangesAsync();
            
        }

        public async Task UpdateUser(int id, User user)
        {
            var newUser = _dataContext.Users.Where(user => user.Id == id).FirstOrDefault();
            if (newUser != null)
            {
                newUser.Name = user.Name;
                newUser.Email = user.Email;
                newUser.Password = user.Password;
                newUser.UpdatedAt = DateTime.Now;
                newUser.UpdatedBy = user.UpdatedBy;
                await _dataContext.SaveChangesAsync();
            }

        }
        public async Task RemoveUser(int id)
        {
            var userToDelete = _dataContext.Users.FirstOrDefault(user => user.Id == id);
            if (userToDelete != null)
            {
                _dataContext.Users.Remove(userToDelete);
                await _dataContext.SaveChangesAsync();
            }

        }

        public async Task<User> Login(string email, string password)
        {
            User user =await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            return user;
        }
    }
}
