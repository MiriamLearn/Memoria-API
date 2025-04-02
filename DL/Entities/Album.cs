using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Image> imageList { get; set; }=new List<Image>();
        public DateTime CreatedAt { get; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }=false;
     

        public Album(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
