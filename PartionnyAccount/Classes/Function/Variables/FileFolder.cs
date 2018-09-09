using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartionnyAccount.Classes.Function.Variables
{
    public class FileFolder
    {
        internal string Return(int DirCustomersID, string sTypeFilder)
        {
            //return Free(iTypeFilder);

            return Comercial(DirCustomersID, sTypeFilder);
        }


        #region Free

        private string Free(string sTypeFilder)
        {
            Classes.Function.Variables.ConnectionString connectionString = new ConnectionString();
            switch (sTypeFilder)
            {
                case "BackUp": return System.Web.HttpContext.Current.Server.MapPath("~/") + @"\UsersTemp\BackUp\";
                case "FileStock": return System.Web.HttpContext.Current.Server.MapPath("~/") + @"\UsersTemp\FileStock\";

                case "Export": return connectionString.Free_SQLitePathUser() + @"Users\File\Export\";
                case "Logo": return connectionString.Free_SQLitePathUser() + @"Users\File\Logo\";
                case "Photo": return connectionString.Free_SQLitePathUser() + @"Users\File\Photo\";

                default: return "Папка '" + sTypeFilder + "' не найдена!";
            }
        }

        #endregion


        #region Comercial

        private string Comercial(int DirCustomersID, string sTypeFilder)
        {
            Classes.Function.Variables.ConnectionString connectionString = new ConnectionString();
            switch (sTypeFilder)
            {
                case "BackUp": return System.Web.HttpContext.Current.Server.MapPath("~/") + @"\UsersTemp\BackUp\";
                case "FileStock": return System.Web.HttpContext.Current.Server.MapPath("~/") + @"\UsersTemp\FileStock\";

                case "Export": return connectionString.Comercial_SQLitePathUser() + "user_" + DirCustomersID + @"\File\Export\";
                case "Logo": return connectionString.Comercial_SQLitePathUser() + "user_" + DirCustomersID + @"\File\Logo\";
                case "Photo": return connectionString.Comercial_SQLitePathUser() + "user_" + DirCustomersID + @"\File\Photo\";

                default: return "Папка '" + sTypeFilder + "' не найдена!";
            }
        }

        #endregion

    }
}