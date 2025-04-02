using DL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DL
{
    public class DataContext: DbContext,IDataContext
    {
        //public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Album> Albums { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
               @"Server=bbr25qmtjwhzicpw3uro-mysql.services.clever-cloud.com;Port=3306;Database=bbr25qmtjwhzicpw3uro;User=undtetqpywbhigr5;Password=TepmHiOc7xJGmaXKzf6H",
                new MySqlServerVersion(new Version(9, 0, 0))
            );
        }
        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
