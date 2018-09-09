using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace PartionnyAccount.Classes.Function.Exceptions
{
    internal class ExceptionEntry
    {
        PartionnyAccount.Classes.Function.Function function = new Function();

        internal string Return(Exception ex)
        {
            //return ex.InnerException.InnerException.ToString();

            string exMsg = ex.Message;

            //Если есть кириллица, то возвращаем её!
            if (function.IsStringLatin(exMsg)) { return exMsg; }

            if (ex.Message != null || (ex.InnerException != null && ex.InnerException.Message != null))
            {
                if (ex.Message != null)
                {
                    exMsg = ex.Message;
                }
                else
                {
                    exMsg = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message != null)
                    {
                        exMsg = ex.InnerException.InnerException.Message;
                    }
                }
            }
            else if (((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors != null)
            {
                System.Data.Entity.Validation.DbEntityValidationException ex2 = ((System.Data.Entity.Validation.DbEntityValidationException)ex); //.EntityValidationErrors;

                if (ex2.EntityValidationErrors.Count() > 0)
                {
                    exMsg +=
                        "<br>" +
                        "Найдено ошибок в: " + ex2.EntityValidationErrors.Count().ToString() + " записях<br>" +
                        "Ошибки:<br>";

                    foreach (var eve in ex2.EntityValidationErrors)
                    {
                        //string Name = eve.Entry.Entity.GetType().Name; string State = eve.Entry.State.ToString();
                        foreach (var ve in eve.ValidationErrors)
                        {
                            //string PropertyName = ve.PropertyName; string ErrorMessage = ve.ErrorMessage;
                            exMsg += "Поле: " + ve.ErrorMessage + " (" + ve.PropertyName + ")" + "<br>";
                        }
                    }
                }
            }

            //return Replacement(exMsg) +"<br />"+ exMsg;
            //return Replacement(exMsg) + " - " + exMsg;
            return exMsg;
        }

        internal string Return2(Exception ex)
        {
            string exMsg = ex.Message;
            if (ex.InnerException != null && ex.InnerException.Message != null)
            {
                exMsg = ex.InnerException.Message;
                if (ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message != null)
                {
                    exMsg = ex.InnerException.InnerException.Message;
                }
            }

            return Replacement(exMsg) + "<br />" + exMsg;
            //return Replacement(exMsg);
        }

        //constraint failed FOREIGN KEY constraint failed
        private string Replacement(string sMsg)
        {
            string msg = sMsg.ToLower();

            //Oracle, Informix, DB2, FireBird, SQLite, ... === === === === === === === === === === === === === === === === ===
            if (msg.IndexOf("constraint failed") > -1)
            {

                if (
                    (msg.IndexOf("column") > -1 && msg.IndexOf("is") > -1 && msg.IndexOf("not") > -1 && msg.IndexOf("unique") > -1) ||
                    (msg.ToLower().IndexOf("unique") > -1 && msg.IndexOf("constraint") > -1 && msg.IndexOf("failed") > -1)
                    )
                {
                    msg = Classes.Language.Sklad.Language.msg35_1;
                }
                else if (msg.IndexOf("foreign") > -1 && msg.IndexOf("key") > -1 && msg.IndexOf("constraint") > -1 && msg.IndexOf("failed") > -1)
                {
                    msg = Classes.Language.Sklad.Language.msg35_2;
                }
            }
            else if (msg.IndexOf("missing") > -1 && msg.IndexOf("Insufficient") > -1 && msg.IndexOf("parameters") > -1 && msg.IndexOf("supplied") > -1 && msg.IndexOf("command") > -1)
            {
                msg = Classes.Language.Sklad.Language.msg35_3;
            }
            else if (msg.IndexOf("database") > -1 && msg.IndexOf("locked") > -1)
            {
                msg = Classes.Language.Sklad.Language.msg35_4;
            }
            else if (msg.IndexOf("disk") > -1 && msg.IndexOf("I/O") > -1 && msg.IndexOf("error") > -1)
            {
                msg = Classes.Language.Sklad.Language.msg35_4;
            }

            //MS SQL === === === === === === === === === === === === === === === === ===
            if (msg.IndexOf("error") > -1 && msg.IndexOf("occurred") > -1 && msg.IndexOf("establishing") > -1 && msg.IndexOf("connection") > -1 && msg.IndexOf("SQL") > -1 && msg.IndexOf("Server") > -1)
            {
                msg = Classes.Language.Sklad.Language.msg39 + "<BR>" + Classes.Language.Sklad.Language.msg39_1 + "<BR><HR>";
            }

            //Убираем символ "
            msg = msg.Replace("\"", "'");

            //Убираем символ "новой строки"
            msg = msg.Replace("\n", ". ");

            if (msg == sMsg.ToLower()) msg = sMsg;

            return msg;
        }
    }
}