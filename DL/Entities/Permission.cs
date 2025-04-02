using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Permission(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

    }
}
