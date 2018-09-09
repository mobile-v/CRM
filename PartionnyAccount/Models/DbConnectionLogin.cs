using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PartionnyAccount.Models
{
    public class DbConnectionLogin : DbContext
    {
        //Беспараметрический конструктор
        //Когда создаём Шаблон Контролера для DirNomen - надо раскомментировать! 
        //Т.к. Студия создаёт "безпараметрическое" подключение, а у нас сейчас обязателен параметр - строка подклчения (ниже)
        //Но, потом ОБЯЗАТЕЛЬНО закоментировать обратно!
        public DbConnectionLogin() { }

        //Передача строки соединения
        public DbConnectionLogin(string connString) : base(connString) { }
        //public DbConnectionSQLite(string connString) : base(new System.Data.SQLite.SQLiteConnection() { ConnectionString = connString }, true) { } 


        // === Login ===
        //Models.Login.Dir
        public DbSet<Login.Dir.DirCustomer> DirCustomers { get; set; }
        public DbSet<Login.Dir.DirCountry> DirCountries { get; set; }
        public DbSet<Login.Dir.DirLanguage> DirLanguages { get; set; }
        public DbSet<Login.Dir.DirPayCustomer> DirPayCustomers { get; set; }
        public DbSet<Login.Dir.DirPayService> DirPayServices { get; set; }
        public DbSet<Login.Dir.DirLoginNot> DirLoginNot { get; set; }

        //Models.Login.Jurn
        public DbSet<Login.Jurn.JurnDispLogining> JurnDispLoginings { get; set; }
        public DbSet<Login.Jurn.JurnDispError> JurnDispErrors { get; set; }

        //Models.Login.Sys
        public DbSet<Login.Sys.SysAdmin> SysAdmins { get; set; }
    }
}