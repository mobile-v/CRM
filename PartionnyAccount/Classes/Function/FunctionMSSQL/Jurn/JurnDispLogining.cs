using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace PartionnyAccount.Classes.Function.FunctionMSSQL.Jurn
{
    public class JurnDispLogining
    {
        //Insert
        public void Write(int DirCustomersID, string pLogin, string pPswd, string pJurnDispLoginingDesc)
        {
            try
            {
                Models.DbConnectionLogin db = new Models.DbConnectionLogin("ConnStrMSSQL");

                Models.Login.Jurn.JurnDispLogining jurnDispLogining = new Models.Login.Jurn.JurnDispLogining();
                jurnDispLogining.JurnDispLoginingDate = DateTime.Now;
                jurnDispLogining.DirCustomersID = DirCustomersID;

                jurnDispLogining.Login = pLogin;
                jurnDispLogining.Pswd = pPswd;
                jurnDispLogining.JurnDispLoginingDesc = pJurnDispLoginingDesc;

                db.Entry(jurnDispLogining).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
            }
            catch { }
        }
    }
}