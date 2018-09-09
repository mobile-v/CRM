using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace PartionnyAccount.Classes.Function.FunctionMSSQL.Jurn
{
    public class JurnDispError
    {
        //Insert
        public void Write(Models.DbConnectionLogin dbLogin, string pEx, int DirCustomersID)
        {
            try
            {
                Models.Login.Jurn.JurnDispError jurnDispError = new Models.Login.Jurn.JurnDispError();
                jurnDispError.DirCustomersID = DirCustomersID;
                jurnDispError.JurnDispErrorText = pEx;
                dbLogin.Entry(jurnDispError).State = System.Data.Entity.EntityState.Added;
                dbLogin.SaveChanges();
            }
            catch (Exception ex) { string extxt = ex.Message; }
        }

        public void Write(string pEx, int DirCustomersID)
        {
            try
            {
                Models.DbConnectionLogin dbLogin = new Models.DbConnectionLogin("ConnStrMSSQL");

                Models.Login.Jurn.JurnDispError jurnDispError = new Models.Login.Jurn.JurnDispError();
                jurnDispError.DirCustomersID = DirCustomersID;
                jurnDispError.JurnDispErrorText = pEx;
                dbLogin.Entry(jurnDispError).State = System.Data.Entity.EntityState.Added;
                dbLogin.SaveChanges();
            }
            catch { }
        }
    }
}