using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using PartionnyAccount.Models;
using PartionnyAccount.Models.Sklad.Dir;
using System.Data.SQLite;

namespace PartionnyAccount.Classes.Update
{
    public class Prevention
    {
        #region Classes

        Models.DbConnectionLogin dbLogin = new Models.DbConnectionLogin("ConnStrMSSQL");
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Function.FunctionMSSQL.Jurn.JurnDispError jurnDispError = new Function.FunctionMSSQL.Jurn.JurnDispError();
        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();

        #endregion

        internal string Start()
        {
            //return FreeStart();

            return ComercialStart();
        }


        #region FreeStart

        private string FreeStart()
        {
            try
            {
                string ConStr = connectionString.Return(0, null, false);
                return UpdatingOne(ConStr, 0);
            }
            catch (Exception ex)
            {
                return exceptionEntry.Return(ex);
            }
        }

        #endregion


        #region ComercialStart: Комерческое обновление (по циклу обновляются Бызы всех клиентов)

        private string ComercialStart()
        {
            #region var
            int iCountUpdate = 0, iCountUpdateNo = 0, iCountError = 0;
            string sError = "",
                sOk = "", //Обновленно
                sOkNo = "", //Уже было обновлено
                sNo = ""; //Ошибка
            #endregion

            #region Update All
            try
            {
                //Получаем список активных клиентов
                var query = dbLogin.DirCustomers.Where(x => x.Active == true).ToList();

                for (int i = 0; i < query.Count(); i++)
                {
                    string ConStr = connectionString.Return(query[i].DirCustomersID, null, true);
                    string _result = UpdatingOne(ConStr, query[i].DirCustomersID);

                    if (_result == "true") { iCountUpdate++; sOk += query[i].Login + "<br />"; }
                    else if (_result == "false") { iCountUpdateNo++; sOkNo += query[i].Login + "<br />"; }
                    else { iCountError++; sError = _result; sNo += query[i].Login + "<br />"; }
                }

            }
            catch (Exception ex) { return exceptionEntry.Return(ex); }
            #endregion

            #region Return
            return
            "Обновленно: " + iCountUpdate + "<br />" +
            "Уже было Обновленно: " + iCountUpdateNo + "<br />" +
            "Не обновленно: " + iCountError + "<br />" +
            "Ошибки: " + sError + "<br /><br /><br />" +

            "Обновленно Logins:<br />" + sOk + "<br /><br />" +
            "Уже было Обновленно Logins:<br />" + sOkNo + "<br /><br />" +
            "Не обновленно Logins:<br />" + sNo + "<br /><br />";
            #endregion
        }

        #endregion


        class FieldX
        {
            public string str1 { get; set; }
            public string str2 { get; set; }
            public string str3 { get; set; }
            public string str4 { get; set; }
        }

        internal string UpdatingOne(string ConStr, int DirCustomersID)
        {
            string ret = "true";

            try
            {
                /*
                using (DbConnectionSklad db = new DbConnectionSklad(ConStr))
                {
                    //1. Check
                    var queryCheck = db.Database.SqlQuery<string>("pragma integrity_check;").ToList();
                    if (queryCheck.Count() > 0)
                    {
                        if (queryCheck[0].ToLower() != "ok")
                        {
                            try { jurnDispError.Write(dbLogin, "pragma integrity_check = " + queryCheck.ToString().ToLower() + ". Class: DocumentManagement.Class.UPDATE.Update", DirCustomersID); }
                            catch { }
                            return "Check Error!";
                        }
                    }

                    //2. Vacuum
                    //var queryVacuum = db.Database.ExecuteSqlCommand("Vacuum;"); // - Not Work ???
                    var queryVacuum = db.Database.SqlQuery<FieldX>("Vacuum;").ToList();

                    //3. Reindex
                    var queryReindex1 = db.Database.ExecuteSqlCommand("Reindex;");
                    //var queryReindex2 = db.Database.SqlQuery<FieldX>("Reindex;").ToList();

                    //4. ANALYZE
                    var queryANALYZE1 = db.Database.ExecuteSqlCommand("ANALYZE;");
                    //var queryANALYZE2 = db.Database.SqlQuery<FieldX>("ANALYZE;").ToList();
                }
                */



                using (SQLiteConnection con = new SQLiteConnection(ConStr))
                {
                    con.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand("", con))
                    {
                        cmd.CommandText = "pragma integrity_check;";
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (dr[0].ToString().ToLower() != "ok")
                                {
                                    Function.FunctionMSSQL.Jurn.JurnDispError jurnDispError = new Function.FunctionMSSQL.Jurn.JurnDispError();
                                    jurnDispError.Write("pragma integrity_check = " + dr[0].ToString() + ". Class: DocumentManagement.Class.UPDATE.Update", DirCustomersID);
                                }
                            }
                        }

                        cmd.CommandText = "Vacuum"; cmd.ExecuteNonQuery();
                        cmd.CommandText = "Reindex"; cmd.ExecuteNonQuery();
                        cmd.CommandText = "ANALYZE"; cmd.ExecuteNonQuery();


                        //Тормозит выборка данных
                        //Исправление: удалить записи в таблице "sqlite_statХ"
                        //5-ть запусков делаем, если ошибка.
                        /*
                        if (!sqlite_statX(con, cmd))
                            if (!sqlite_statX(con, cmd))
                                if (!sqlite_statX(con, cmd))
                                    if (!sqlite_statX(con, cmd))
                                        if (!sqlite_statX(con, cmd)) { }
                        */

                    }
                }


            }
            catch (Exception ex)
            {
                if (DirCustomersID > 0) try { jurnDispError.Write(dbLogin, exceptionEntry.Return(ex) + ". Class: PartionnyAccount.Controllers.Classes.Update.Update ", DirCustomersID); } catch { }

                return exceptionEntry.Return(ex);
            }


            return ret;
        }

        private bool sqlite_statX(SQLiteConnection con, SQLiteCommand cmd)
        {
            try
            {
                /*
                cmd.CommandText =
                    "DELETE FROM sqlite_stat1 WHERE tbl = 'DocPurchTab'; " +
                    "DELETE FROM sqlite_stat3 WHERE tbl = 'DocPurchTab'; " +
                    "DELETE FROM sqlite_stat4 WHERE tbl = 'DocPurchTab'; ";
                cmd.ExecuteNonQuery();
                */
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}