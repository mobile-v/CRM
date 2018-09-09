using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PartionnyAccount.Models;
using PartionnyAccount.Models.Sklad.Doc;
using System.Data.SQLite;
using System.Web.Script.Serialization;

namespace PartionnyAccount.Classes.Function
{
    public class GenGenerate
    {
        internal async Task<string> ReturnGenID(DbConnectionSklad db)
        //internal string ReturnGenID(DbConnectionSklad db)
        {
            Models.Sklad.Sys.SysGen sysGen = new Models.Sklad.Sys.SysGen();
            sysGen.SysGenID = null;
            sysGen.SysGenTemp = true;
            db.Entry(sysGen).State = EntityState.Added;
            await db.SaveChangesAsync();
            //db.SaveChangesAsync();

            string SysGenID = sysGen.SysGenID.ToString();
            return SysGenID;
        }

        internal string ReturnTrash()
        {
            string ret = "";

            Random random = new Random();
            int randNum = 0;
            for (int i = 0; i < 3; i++)
            {
                randNum = random.Next(0, 9);
                ret += randNum.ToString();
            }

            return ret;
        }
    }
}