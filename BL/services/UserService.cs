using BL.IterfaceServices;
using DL.Entities;
using DL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        //public string GenerateJwtToken(string username, string[] roles)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, username)
        //};

        //    // הוספת תפקידים כ-Claims
        //    foreach (var role in roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(30),
        //        signingCredentials: credentials
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}


        public List<User> GetAllUsers()
        {
            return _dataContext.Users.ToList();
        }
        public User GetUserById(int id)
        {
            //BabyValidation.ValidateBabyId(id);
            return _dataContext.Users.Where(u => u.Id == id).FirstOrDefault();
        }
        public void AddUser(User user)
        {
            {
                _dataContext.Users.Add(user);
                _dataContext.SaveChanges();
            }
        }

        public void UpdateUser(int id, User user)
        {
            //BabyValidation.ValidateBabyId(baby.Id);
            //BabyValidation.ValidateBabyId(id);
            //BabyValidation.ValidateBabyName(baby.Name);
            var newUser = _dataContext.Users.Where(user => user.Id == id).FirstOrDefault();
            if (newUser != null)
            {
                newUser.Name = user.Name;
                newUser.Email = user.Email;
                newUser.Password = user.Password;
                newUser.UpdatedAt = DateTime.Now;
                newUser.UpdatedBy = user.UpdatedBy;
                _dataContext.SaveChanges();
            }

        }
        public void RemoveUser(int id)
        {
            var userToDelete = _dataContext.Users.FirstOrDefault(user => user.Id == id);
            if (userToDelete != null)
            {
                _dataContext.Users.Remove(userToDelete);
                _dataContext.SaveChanges();
            }

        }

        public void Login(string email, string password)
        {
            User user = _dataContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
        }
    }
}
