using DL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.IterfaceServices
{
    public interface IUserService
    {
        public string GenerateJwtToken(string username, string[] roles);

        public Task<List<User>> GetAllUsers();
        public Task<User> GetUserById(int id);
        public Task AddUser(User user);
        public Task UpdateUser(int id, User user);
        public Task RemoveUser(int id);
        public Task<User> Login(string email, string password);
    }
}
