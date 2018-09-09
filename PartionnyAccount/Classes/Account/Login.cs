using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PartionnyAccount.Models;

namespace PartionnyAccount.Classes.Account
{
    internal class Login // : HttpApplication
    {
        //Проверка 2-х типов:
        //Free -      Проверяем только из SQLite
        //Comercial - Проверяем Логин из БД MS SQL и SQLite


        #region Union

        #region Service

        internal class Field
        {
            public bool Access;        // true or false. If false then see a "Message";
            public string Msg;         // If false

            //Раскодированыый Логин: admin or admin@intradecloud.com
            public string LoginFull;

            //Models.Sklad.Dir.DirEmployees (SQLite)
            public int DirEmployeeID; // Employees
            //public int SysDirRightsID; // Employees Rights
            //public bool RetailOnly;

            //Models.Login.Dir.DirCustomerLogin (MS SQL)
            public string Login;       // LoginName
            public int DirCustomersID; // LoginID
        }

        internal Field Return(HttpCookie CookieIPOL, bool bEncode)
        {
            //Если Куки null
            if (CookieIPOL == null)
            {
                
                Field field = new Field();
                field.Access = false;
                field.Msg = Classes.Language.Sklad.Language.msg4;

                return field;
                

                //return Comercial("admin@Project", "citi", false);
            }

            //Free
            //return Free(CookieIPOL["CookieU"], CookieIPOL["CookieP"], bEncode);

            //Comercial
            return Comercial(CookieIPOL["CookieU"], CookieIPOL["CookieP"], bEncode);
        }

        internal Field Return(string DirEmployeeLogin, string DirEmployeePswd, bool bEncode)
        {
            //Free
            //return Free(DirEmployeeLogin, DirEmployeePswd, bEncode);

            //Comercial
            return Comercial(DirEmployeeLogin, DirEmployeePswd, bEncode);
        }

        #endregion


        #region Update

        internal bool ReturnUpdate(HttpCookie CookieUpdateIPOL, bool bEncode)
        {
            //Если Куки null
            if (CookieUpdateIPOL == null) return false;

            //Free - SQLite (or MS SQL) Login: admin
            //return FreeUpdate(CookieIPOL["CookieU"], CookieIPOL["CookieP"], bEncode);

            //Comercial
            return ComercialUpdate(CookieUpdateIPOL["CookieUpdateU"], CookieUpdateIPOL["CookieUpdateP"], bEncode);
        }

        internal bool ReturnUpdate(string AdminsLogin, string AdminsPswd, bool bEncode)
        {
            //Free - SQLite (or MS SQL) Login: admin
            //return FreeUpdate(AdminsLogin, AdminsPswd, bEncode);

            //Comercial
            return ComercialUpdate(AdminsLogin, AdminsPswd, bEncode);
        }

        #endregion

        #endregion


        #region Free

        #region Service

        private Field Free(string DirEmployeeLogin, string DirEmployeePswd, bool bEncode)
        {
            Field field = new Field();


            #region Проверяем

            if (String.IsNullOrEmpty(DirEmployeeLogin) && String.IsNullOrEmpty(DirEmployeePswd))
            {
                field.Access = false; field.Msg = Classes.Language.Sklad.Language.msg4;
                return field;
            }

            #endregion


            #region Декодируем

            if (bEncode)
            {
                Account.EncodeDecode encode = new Account.EncodeDecode();
                DirEmployeeLogin = encode.UnionDecode(DirEmployeeLogin);
                field.LoginFull = DirEmployeeLogin;
                DirEmployeePswd = encode.UnionDecode(DirEmployeePswd);
            }

            #endregion


            #region Проверяем Логин и Пароль

            using (DbConnectionSklad db = new DbConnectionSklad("ConnStr"))
            {
                db.Database.Connection.Open();

                var query = db.DirEmployees.Where(x => x.DirEmployeeLogin == DirEmployeeLogin && x.DirEmployeePswd == DirEmployeePswd && x.Del == false && x.DirEmployeeActive == true).ToList();
                if (query.Count > 0)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = query[0];

                    field.Access = true;
                    field.DirEmployeeID = Convert.ToInt32(dirEmployee.DirEmployeeID);
                    //field.SysDirRightsID = dirEmployee.SysDirRightsID;
                }
                else
                {
                    field.Access = false; field.Msg = Classes.Language.Sklad.Language.msg4;
                    return field;
                }
            }

            #endregion


            return field;
        }

        #endregion


        #region Update

        private bool FreeUpdate(string AdminsLogin, string AdminsPswd, bool bEncode)
        {
            #region Проверяем

            if (String.IsNullOrEmpty(AdminsLogin) && String.IsNullOrEmpty(AdminsPswd)) return false;

            #endregion


            #region Декодируем

            if (bEncode)
            {
                Account.EncodeDecode encode = new Account.EncodeDecode();
                AdminsLogin = encode.UnionDecode(AdminsLogin);
                AdminsPswd = encode.UnionDecode(AdminsPswd);
            }

            #endregion


            #region Проверяем Логин и Пароль

            using (DbConnectionSklad db = new DbConnectionSklad("ConnStr"))
            {
                db.Database.Connection.Open();

                var query = db.DirEmployees.Where(x => x.DirEmployeeLogin == AdminsLogin && x.DirEmployeePswd == AdminsPswd && x.Del == false && x.DirEmployeeActive == true).ToList();
                if (query.Count > 0)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = query[0];

                    //Только с правами Администратора можно обновлять!
                    /*if (dirEmployee.SysDirRightsID == 1) return true;
                    else return false;*/

                    return true;
                }
                else
                {
                    return false;
                }
            }

            #endregion
        }

        #endregion

        #endregion


        #region Comercial

        #region Service

        private Field Comercial(string DirEmployeeLogin, string DirEmployeePswd, bool bEncode)
        {
            Function.Variables.ConnectionString connectionString = new Function.Variables.ConnectionString();
            Field field = new Field();


            #region Проверяем "IsNullOrEmpty"

            if (String.IsNullOrEmpty(DirEmployeeLogin) && String.IsNullOrEmpty(DirEmployeePswd))
            {
                field.Access = false; field.Msg = Classes.Language.Sklad.Language.msg4;
                return field;
            }

            #endregion


            #region Декодируем Логин и Пароль

            if (bEncode)
            {
                Account.EncodeDecode encode = new Account.EncodeDecode();
                DirEmployeeLogin = encode.UnionDecode(DirEmployeeLogin);
                field.LoginFull = DirEmployeeLogin;
                DirEmployeePswd = encode.UnionDecode(DirEmployeePswd);
            }

            #endregion


            #region Разщепляем Логин на 2-е составные части Логин@МояКомпания

            if (DirEmployeeLogin.IndexOf("@") == -1)
            {
                field.Access = false; field.Msg = Classes.Language.Sklad.Language.msg26;
                return field;
            }
            else
            {
                string _Login = DirEmployeeLogin;
                int p1 = _Login.IndexOf("@");
                DirEmployeeLogin = _Login;
                DirEmployeeLogin = DirEmployeeLogin.Remove(p1, DirEmployeeLogin.Length - p1);
                field.Login = _Login.Remove(0, p1 + 1);
            }

            #endregion


            #region Получаем DirCustomersID (из MS SQL)

            //Получаем ID-шник в БД MS SQL
            field.DirCustomersID = connectionString.mDirCustomersID(field.Login);
            //Полученные варианты
            if (field.DirCustomersID == -1) //Не найден такой User.
            {
                field.Access = false; field.Msg = Classes.Language.Sklad.Language.msg1 + DirEmployeeLogin;
                return field;
            }
            else if (field.DirCustomersID == 0) //Найден такой User, но не активен (долго не заходил в свой аккаунт)
            {
                field.Access = false; field.Msg = Classes.Language.Sklad.Language.msg2();
                return field;
            }

            #endregion


            #region Проверяем Логин и Пароль

            //using (DbConnectionSklad db = new DbConnectionSklad(GetSQLiteBasicConnStr_DirCustomersID(field.DirCustomersID)))
            using (DbConnectionSklad db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true)))
            {
                db.Database.Connection.Open();

                var query = db.DirEmployees.Where(x => x.DirEmployeeLogin == DirEmployeeLogin && x.DirEmployeePswd == DirEmployeePswd && x.Del == false && x.DirEmployeeActive == true).ToList();
                if (query.Count > 0)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = query[0];

                    field.Access = true;
                    field.DirEmployeeID = Convert.ToInt32(dirEmployee.DirEmployeeID);

                    //field.SysDirRightsID = dirEmployee.SysDirRightsID;
                    //field.RetailOnly = Convert.ToBoolean(dirEmployee.RetailOnly);
                }
                else
                {
                    field.Access = false; field.Msg = Classes.Language.Sklad.Language.msg4;
                    return field;
                }
            }

            #endregion


            return field;
        }

        #endregion


        #region Update

        private bool ComercialUpdate(string AdminsLogin, string AdminsPswd, bool bEncode)
        {
            Function.Variables.ConnectionString connectionString = new Function.Variables.ConnectionString();


            #region Проверяем "IsNullOrEmpty"

            if (String.IsNullOrEmpty(AdminsLogin) && String.IsNullOrEmpty(AdminsPswd)) return false;

            #endregion


            #region Декодируем Логин и Пароль

            if (bEncode)
            {
                Account.EncodeDecode encode = new Account.EncodeDecode();
                AdminsLogin = encode.UnionDecode(AdminsLogin);
                AdminsPswd = encode.UnionDecode(AdminsPswd);
            }

            #endregion


            #region Проверяем Логин и Пароль

            using (DbConnectionLogin db = new DbConnectionLogin("ConnStrMSSQL"))
            {
                db.Database.Connection.Open();

                var query = db.SysAdmins.Where(x => x.AdminsLogin == AdminsLogin && x.AdminsPswd == AdminsPswd && x.AdminsActive == true).ToList();
                if (query.Count > 0) return true;
                else return false;
            }

            #endregion
        }

        #endregion

        #endregion

    }
}