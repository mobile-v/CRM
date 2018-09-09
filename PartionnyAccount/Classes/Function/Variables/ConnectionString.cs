using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartionnyAccount.Classes.Function.Variables
{
    internal class ConnectionString
    {
        #region Union

        //Рабочий метод, но не используется
        //Параметр - Куки
        /*internal string Return(HttpCookie CookieIPOL)
        {
            //Если Куки null
            if (CookieIPOL == null) throw new System.ArgumentException(Classes.Language.Language.msg4, "original");

            #region Декодируем Логин

            Account.EncodeDecode encode = new Account.EncodeDecode();
            string DirEmployeesLogin = encode.UnionDecode(CookieIPOL["CookieU"]);

            #endregion


            //return Free();

            return Comercial(0, DirEmployeesLogin, true);
        }*/

        //Параметр - Готовые данные
        internal string Return(int DirCustomersID, string Login, bool FK)
        {
            //return Free();

            return Comercial(DirCustomersID, Login, FK);
        }

        //Путь к папке БД
        internal string SQLitePathUser()
        {
            //return Free_SQLitePathUser();

            return Comercial_SQLitePathUser();
        }
        //Путь к папке БД Etalon
        internal string SQLitePathEtalon()
        {
            //return Free_Etalon();

            return Comercial_Etalon();
        }

        //Путь к папке файлов (например изображения)
        internal string FilePathUser()
        {
            //return Free_SQLitePathUser();

            return Comercial_FilePathUser();
        }

        #endregion


        #region Free

        //Путь к директории "User"
        public string Free_SQLitePathUser()
        {
            return AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\Free\Users\";
        }
        //Путь к базе "Etalon"
        //Для Free Эталонная БД не нужна, но может позже реализую "что-то" с ней
        internal string Free_Etalon()
        {
            return AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\Free\UsersEtalon\Base\basic.db";
        }

        public string Free_FilePathUser()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/") + @"Users\";
        }


        internal string Free()
        {
            return "ConnStr";
        }

        #endregion


        #region Comercial

        internal string Comercial(int DirCustomersID, string Login, bool FK)
        {
            if (!String.IsNullOrEmpty(Login) && FK) return GetSQLiteBasicConnStr(Login);
            else if (!String.IsNullOrEmpty(Login) && !FK) return GetSQLiteBasicConnStr_FK_OFF(Login);

            else if (DirCustomersID > 0 && FK) return GetSQLiteBasicConnStr_DirCustomersID(DirCustomersID);
            else if (DirCustomersID > 0 && !FK) return GetSQLiteBasicConnStr_DirCustomersID_FK_OFF(DirCustomersID);

            throw new System.ArgumentException("ConnectionString cannot be Null Or Empty", "original");
        }



        //Метод: получаем ID-шник по Логину (в MS SQL)
        internal int mDirCustomersID(string pLogin)
        {
            int DirCustomersID = -1;
            //-1 - Не найден пользователь
            //0  - Найден пользователь, но Не активный
            //>=1  - Найден пользователь и активный

            using (Models.DbConnectionLogin db = new Models.DbConnectionLogin("ConnStrMSSQL"))
            {
                var query = db.DirCustomers.Where(x => x.Login == pLogin && x.Active == true).ToList();
                if (query.Count() > 0)
                {
                    if (Convert.ToBoolean(query[0].Active)) DirCustomersID = query[0].DirCustomersID;
                    else DirCustomersID = 0;
                }
            }
            return DirCustomersID;
        }

        //Метод: получаем ID-шник по Логину (в MS SQL)
        internal string mDLogin(int iDirCustomersID)
        {
            string Login = null;
            //-1 - Не найден пользователь
            //0  - Найден пользователь, но Не активный
            //>=1  - Найден пользователь и активный

            using (Models.DbConnectionLogin db = new Models.DbConnectionLogin("ConnStrMSSQL"))
            {
                var query = db.DirCustomers.Where(x => x.DirCustomersID == iDirCustomersID && x.Active == true).ToList();
                if (query.Count() > 0)
                {
                    if (Convert.ToBoolean(query[0].Active)) Login = query[0].Login;
                    else Login = null;
                }
            }
            return Login;
        }


        #region Строки соединения

        //Путь к директории App_Data - "Users"
        public string Comercial_SQLitePathUser()
        {
            return AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\Comercial\Users\";
        }
        //Путь к директории "Users"
        public string Comercial_Etalon()
        {
            return AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\Comercial\Users\Etalon\Base\basic.db";
        }


        //Путь к директории "Users"
        public string Comercial_FilePathUser()
        {
            return AppDomain.CurrentDomain.BaseDirectory + @"Users\";
        }


        //Путь к Базе "Users"
        internal string GetSQLiteBasicConnStr(string Login)
        {
            return String.Format("Data Source={0};New=False;Version=3;foreign keys=true;", Comercial_SQLitePathUser() + "user_" + mDirCustomersID(Login) + @"Base\basic.db"); //EnforceFKConstraints=1
        }
        //Путь к Базе "Users" без FK
        internal string GetSQLiteBasicConnStr_FK_OFF(string Login)
        {
            return String.Format("Data Source={0};New=False;Version=3;foreign keys=false;", Comercial_SQLitePathUser() + "user_" + mDirCustomersID(Login) + @"Base\basic.db"); //EnforceFKConstraints=1
        }

        //Прямой коннект к БД
        internal string GetSQLiteBasicConnStr_DirCustomersID(int DirCustomersID)
        {
            return String.Format("Data Source={0};New=False;Version=3;foreign keys=true;", Comercial_SQLitePathUser() + "user_" + DirCustomersID + @"\Base\basic.db"); //EnforceFKConstraints=1
        }
        //Прямой коннект к БД без FK
        internal string GetSQLiteBasicConnStr_DirCustomersID_FK_OFF(int DirCustomersID)
        {
            return String.Format("Data Source={0};New=False;Version=3;foreign keys=false;", Comercial_SQLitePathUser() + "user_" + DirCustomersID + @"\Base\basic.db"); //EnforceFKConstraints=1
        }

        //Путь к "BackUp" базе
        internal string SQLitePathBackUp()
        {
            return AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\Comercial\BackUp\";
        }
        //Строка соединения к базе "Etalon"
        internal string GetSQLiteBasicConnStr_Etalon()
        {
            return String.Format("Data Source={0};New=False;Version=3;foreign keys=true;", Comercial_SQLitePathUser() + @"\Comercial\Users\Etalon\Base\basic.db"); //EnforceFKConstraints=1
        }

        #endregion

        #endregion

    }
}