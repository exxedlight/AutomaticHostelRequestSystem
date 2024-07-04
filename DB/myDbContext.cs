using Microsoft.EntityFrameworkCore;
using HostelRequest.DB.Entityes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HostelRequest.DB
{
    //  контекст БД для ентіті фреймворка
    internal class myDbContext : DbContext
    {
        private string con_str = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""D:\zDocs\C_Sharp\ASP NET\HostelRequest\HostelRequest\DB\Posellenia.mdf"";Integrated Security=True;Connect Timeout=30";
        

        public DbSet<HostelEntity> Hostels { get; set; }                        //  Гуртожитки
        public DbSet<RoomEntity> Rooms { get; set; }                            //  Кімнати
        public DbSet<StudentEntity> Students { get; set; }                      //  Студенти
        public DbSet<ApplicationSettlementEntity> Applications { get; set; }    //  Заяви
        public DbSet<AdminEntity> Admins { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {   //  конфігуратор підключення
            optionsBuilder.UseSqlServer(con_str);
        }
    }
}
