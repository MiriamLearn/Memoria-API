using DL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL
{
    public interface IDataContext
    {
        public DbSet<User> Users { get; set; }
        //public DbSet<File1> Files { get; set; }
        //public DbSet<Folder> Folders { get; set; }
        int SaveChanges();
    }
}
