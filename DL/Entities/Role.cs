using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public int UpdatedBy { get; set; } = 0;

        public Role(int id, string name, string description, int createdBy)
        {
            Id = id;
            Name = name;
            Description = description;
            CreatedBy = createdBy;
        }
    }
}
