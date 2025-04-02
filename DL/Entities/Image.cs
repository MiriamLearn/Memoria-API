using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public long Size { get; set; }
        public string S3URL { get; set; }
        public int AlbumId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
      
        public Image() { }
        public Image(int id, string name, int type, long size, string s3URL)
        {
            Id = id;
            Name = name;
            Type = type;
            Size = size;
            S3URL = s3URL;
        }
    }
}
