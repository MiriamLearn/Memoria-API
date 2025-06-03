using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Album> albumList { get; set; } = new List<Album>();
        public DateTime CreatedAt { get; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int UpdatedBy { get; set; } = 0;

        public User() { }

        public User(int id, string name, string email, string password)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            CreatedBy = id;
        }
    }
}
