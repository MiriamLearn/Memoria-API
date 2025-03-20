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
        public DbSet<User> Users { get; set; }

        //public DataContext(DbContextOptions<DataContext> options) : base(options)
        //{
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
               @"Server=bbr25qmtjwhzicpw3uro-mysql.services.clever-cloud.com;Port=3306;Database=bbr25qmtjwhzicpw3uro;User=undtetqpywbhigr5;Password=TepmHiOc7xJGmaXKzf6H",
                new MySqlServerVersion(new Version(9, 0, 0))
            );
        }
        public int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
