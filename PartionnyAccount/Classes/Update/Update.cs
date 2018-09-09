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
using System.Data.Entity.Migrations;
using System.Collections;
using System.Text.RegularExpressions;

namespace PartionnyAccount.Classes.Update
{
    public class Update //: DbMigration
    {
        #region Classes

        Models.DbConnectionLogin db = new Models.DbConnectionLogin("ConnStrMSSQL");
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Function.FunctionMSSQL.Jurn.JurnDispError jurnDispError = new Function.FunctionMSSQL.Jurn.JurnDispError();
        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();

        #endregion


        internal async Task<string> Start(string sMethod)
        {
            //return FreeStart(sMethod);

            return await ComercialStart(sMethod);
        }


        #region FreeStart

        private async Task<string[]> FreeStart(string sMethod)
        {
            try
            {
                string ConStr = connectionString.Return(0, null, false);
                return await UpdatingOne(ConStr, 0);
            }
            catch (Exception ex)
            {
                string[] ret = new string[2];
                ret[0] =  exceptionEntry.Return(ex);
                return ret;
            }
        }

        #endregion


        #region ComercialStart: Комерческое обновление (по циклу обновляются Бызы всех клиентов)

        private async Task<string> ComercialStart(string sMethod)
        {
            //Получаем список активных клиентов
            var query = db.DirCustomers.Where(x => x.Active == true).ToList();

            if (sMethod == "Update")
            {
                //Обновление сервиса

                #region var
                int iCountUpdate = 0, iCountUpdateNo = 0, iCountError = 0;
                string sError = "",
                    sOk = "", //Обновленно
                    sOkNo = "", //Уже было обновлено
                    sNo = ""; //Ошибка
                #endregion

                #region All
                try
                {
                    for (int i = 0; i < query.Count(); i++)
                    {
                        string ConStr = connectionString.Return(query[i].DirCustomersID, null, true);
                        string[] _result = await UpdatingOne(ConStr, query[i].DirCustomersID);

                        if (_result[0] == "true") { iCountUpdate++; sOk += query[i].Login + "<br />"; }
                        else if (_result[0] == "false") { iCountUpdateNo++; sOkNo += query[i].Login + "<br />"; }
                        else { iCountError++; sError = _result[0]; sNo += query[i].Login + "<br />"; }
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

            //RemParties - RemPartyMinuses

            else if (sMethod == "RemPartiesCheck")
            {
                //Проверить остатки

                #region var
                int iCountUpdate = 0, iCountUpdateNo = 0, iCountError = 0;
                string sError = "",
                    sOk = "", //Обновленно
                    sOkNo = "", //Уже было обновлено
                    sNo = ""; //Ошибка
                #endregion

                #region All
                for (int i = 0; i < query.Count(); i++)
                {
                    try
                    {
                        string ConStr = connectionString.Return(query[i].DirCustomersID, null, true);

                        string _result = await RemPartiesCheck(ConStr);
                        if (_result != "false")
                        {
                            iCountUpdateNo++; sOkNo += query[i].Login + " (К-во: " + _result + ")" + "<br />";
                        }
                        else
                        {
                            iCountUpdate++; sOk += query[i].Login + "<br />";
                        }
                    }
                    catch (Exception ex) { iCountError++; sError = ex.Message; sNo += query[i].Login + "<br />"; }
                }
                #endregion

                #region Return
                return
                "Без ошибок: " + iCountUpdate + "<br />" +
                "С ошибками остатков: " + iCountUpdateNo + "<br />" +
                "Ошибки подключения: " + iCountError + "<br />" +
                "Ошибки: " + sError + "<br /><br /><br />" +

                "Ошибки: " + sError + "<br /><br /><br />" +

                "Logins без ошибок:<br />" + sOk + "<br /><br />" + 
                "Logins с ошибками остаткив:<br />" + sOkNo + "<br /><br />" + 
                "Logins с ошибками соединения:<br />" + sNo + "<br /><br />";
                #endregion
            }
            else if (sMethod == "RemPartiesCorrect")
            {
                //Исправить остатки

                #region var
                int iCountUpdate = 0, iCountUpdateNo = 0, iCountError = 0;
                string sError = "",
                    sOk = "", //Обновленно
                    sOkNo = "", //Уже было обновлено
                    sNo = ""; //Ошибка

                string
                    SQL =
                    "SELECT " +
                    " RemParties.[DirNomenID], RemParties.[RemPartyID], " +
                    " RemParties.[Quantity], RemParties.[Remnant],  " +
                    " RemParties.[Remnant] - (RemParties.[Quantity] - RemPartyMinuses.[Quantity]) AS Error " +
                    "FROM RemParties, RemPartyMinuses " +
                    "WHERE " +
                    " RemParties.RemPartyID = RemPartyMinuses.RemPartyID and " +
                    " RemParties.[Remnant] - (RemParties.[Quantity] - RemPartyMinuses.[Quantity]) > 0 " +
                    "GROUP BY RemParties.[DirNomenID], RemParties.[RemPartyID]";
                #endregion

                #region All
                for (int i = 0; i < query.Count(); i++)
                {
                    try
                    {
                        string ConStr = connectionString.Return(query[i].DirCustomersID, null, true);
                        string _result = await RemPartiesCheck(ConStr);
                        if (_result != "false")
                        {
                            if (await RemPartiesCorrect(ConStr)) { iCountUpdateNo++; sOkNo += query[i].Login + " (К-во: " + _result + ")" + "<br />"; }
                        }
                        else
                        {
                            iCountUpdate++; sOk += query[i].Login + "<br />";
                        }
                    }
                    catch (Exception ex) { iCountError++; sError = ex.Message; sNo += query[i].Login + "<br />"; }
                }
                #endregion

                #region Return
                return
                "Без ошибок: " + iCountUpdate + "<br />" +
                "С исправлено: " + iCountUpdateNo + "<br />" +
                "Ошибки подключения: " + iCountError + "<br />" +
                "Ошибки: " + sError + "<br /><br /><br />" +

                "Ошибки: " + sError + "<br /><br /><br />" +

                "Logins без ошибок:<br />" + sOk + "<br /><br />" +
                "Logins исправлено:<br />" + sOkNo + "<br /><br />" +
                "Logins с ошибками подключения:<br />" + sNo + "<br /><br />";
                #endregion
            }

            //RemParties - RemRemnants

            else if (sMethod == "RemRemnantsCheck")
            {
                //Проверить остатки

                #region var
                int iCountUpdate = 0, iCountUpdateNo = 0, iCountError = 0;
                string sError = "",
                    sOk = "", //Обновленно
                    sOkNo = "", //Уже было обновлено
                    sNo = ""; //Ошибка
                #endregion

                #region All
                for (int i = 0; i < query.Count(); i++)
                {
                    try
                    {
                        string ConStr = connectionString.Return(query[i].DirCustomersID, null, true);

                        int _result = await RemRemnantsCheck(ConStr);
                        if (_result > 0)
                        {
                            iCountUpdateNo++; sOkNo += query[i].Login + " (К-во товара: " + _result.ToString() + ")" + "<br />";
                        }
                        else
                        {
                            iCountUpdate++; sOk += query[i].Login + "<br />";
                        }
                    }
                    catch (Exception ex) { iCountError++; sError = ex.Message; sNo += query[i].Login + "<br />"; }
                }
                #endregion

                #region Return
                return
                "Без ошибок: " + iCountUpdate + "<br />" +
                "С ошибками остатков: " + iCountUpdateNo + "<br />" +
                "Ошибки подключения: " + iCountError + "<br />" +
                "Ошибки: " + sError + "<br /><br /><br />" +

                "Ошибки: " + sError + "<br /><br /><br />" +

                "Logins без ошибок:<br />" + sOk + "<br /><br />" +
                "Logins с ошибками остаткив:<br />" + sOkNo + "<br /><br />" +
                "Logins с ошибками соединения:<br />" + sNo + "<br /><br />";
                #endregion
            }
            else if (sMethod == "RemRemnantsCorrect")
            {
                //Исправить остатки

                #region var
                int iCountUpdate = 0, iCountUpdateNo = 0, iCountError = 0;
                string sError = "",
                    sOk = "", //Обновленно
                    sOkNo = "", //Уже было обновлено
                    sNo = ""; //Ошибка

                string
                    SQL =
                    "SELECT " +
                    " RemParties.[DirNomenID], RemParties.[RemPartyID], " +
                    " RemParties.[Quantity], RemParties.[Remnant],  " +
                    " RemParties.[Remnant] - (RemParties.[Quantity] - RemPartyMinuses.[Quantity]) AS Error " +
                    "FROM RemParties, RemPartyMinuses " +
                    "WHERE " +
                    " RemParties.RemPartyID = RemPartyMinuses.RemPartyID and " +
                    " RemParties.[Remnant] - (RemParties.[Quantity] - RemPartyMinuses.[Quantity]) > 0 " +
                    "GROUP BY RemParties.[DirNomenID], RemParties.[RemPartyID]";
                #endregion

                #region All
                for (int i = 0; i < query.Count(); i++)
                {
                    try
                    {
                        string ConStr = connectionString.Return(query[i].DirCustomersID, null, true);
                        if (await RemRemnantsCheck(ConStr) > 0)
                        {
                            if (await RemRemnantsCorrect(ConStr)) { iCountUpdateNo++; sOkNo += query[i].Login + "<br />"; }
                        }
                        else
                        {
                            iCountUpdate++; sOk += query[i].Login + "<br />";
                        }
                    }
                    catch (Exception ex) { iCountError++; sError = ex.Message; sNo += query[i].Login + "<br />"; }
                }
                #endregion

                #region Return
                return
                "Без ошибок: " + iCountUpdate + "<br />" +
                "Исправлено ошибок: " + iCountUpdateNo + "<br />" +
                "Ошибки подключения: " + iCountError + "<br />" +
                "Ошибки: " + sError + "<br /><br /><br />" +

                "Ошибки: " + sError + "<br /><br /><br />" +

                "Logins без ошибок:<br />" + sOk + "<br /><br />" +
                "Logins исправлено:<br />" + sOkNo + "<br /><br />" +
                "Logins с ошибками подключения:<br />" + sNo + "<br /><br />";
                #endregion
            }

            else {
                return "Not found Method!";
            }
        }

        #endregion



        #region Remans (Check and Correct)

        //RemParties - RemPartyMinuses

        private async Task<string> RemPartiesCheck(string ConStr)
        {
            string ret = "false";

            string SQL =
                "SELECT " +
                " RemParties.[DirNomenID], RemParties.[RemPartyID], " +
                " RemParties.[Quantity], RemParties.[Remnant],  " +
                " SUM(RemPartyMinuses.[Quantity]), " +
                " (RemParties.[Quantity] - (RemParties.[Remnant]  + SUM(RemPartyMinuses.[Quantity]))) AS Error " + //" RemParties.[Remnant] - (RemParties.[Quantity] - RemPartyMinuses.[Quantity]) AS Error " +

                "FROM RemParties, RemPartyMinuses " +

                "WHERE " +
                " RemParties.RemPartyID = RemPartyMinuses.RemPartyID " +
                //" and RemParties.[Remnant] - (RemParties.[Quantity] - RemPartyMinuses.[Quantity]) != 0 " +

                "GROUP BY RemParties.[DirNomenID], RemParties.[RemPartyID],RemParties.[Quantity], RemParties.[Remnant] " +
                "HAVING (RemParties.[Quantity] - (RemParties.[Remnant]  + SUM(RemPartyMinuses.[Quantity]))) != 0 ";

            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                await con.OpenAsync(); //con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            //iCountUpdateNo++; sOkNo += query[i].Login + " (К-во: " + dr["Error"] + ")" + "<br />";
                            ret = dr["Error"].ToString();
                        }
                        /*else
                        {
                            //iCountUpdate++; sOk += query[i].Login + "<br />";
                            ret = false;
                        }*/
                    }
                }
                con.Close();
            }

            return ret;
        }

        private async Task<bool> RemPartiesCorrect(string ConStr)
        {
            string SQL = 
                "UPDATE RemParties SET Remnant = Quantity - " + 
                "(" + 
                " SELECT IFNULL(SUM(RemPartyMinuses.Quantity), 0) " + 
                " FROM RemPartyMinuses " + 
                " WHERE RemPartyMinuses.RemPartyID = RemParties.RemPartyID " + 
                " GROUP BY RemPartyMinuses.RemPartyID " + 
				") " + 
                "WHERE RemPartyID = " + 
                "(" + 
                " SELECT RemPartyID " + 
                " FROM RemPartyMinuses " + 
                " WHERE RemPartyMinuses.RemPartyID = RemParties.RemPartyID " + 
                " GROUP BY RemPartyMinuses.RemPartyID " + 
				")";

            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                await con.OpenAsync(); //con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
                {
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

            return true;
        }


        //RemParties - RemRemnants

        private async Task<int> RemRemnantsCheck(string ConStr)
        {
            int ret = 0;

            string SQL =
                "SELECT Count(DirNomenID) AS CountDirNomenID FROM " +
                "( " +
                " SELECT " +
                "  DirContractorIDOrg, DirWarehouseID, DirNomenID, SUM(Remnant), " +
                "  DirContractorIDOrg || '_' || DirWarehouseID || '_' || DirNomenID || '_' || round(SUM(Remnant), 2) AS X1 " +
                " FROM RemParties " +
                " GROUP BY DirContractorIDOrg, DirWarehouseID, DirNomenID " +
                ") " +
                "WHERE X1 NOT IN " +
                "( " +
                " SELECT " +
                "  DirContractorIDOrg || '_' || DirWarehouseID || '_' || DirNomenID || '_' || round(SUM(Quantity), 2) AS X1 " +
                " FROM RemRemnants " +
                " GROUP BY DirContractorIDOrg, DirWarehouseID, DirNomenID " +
                ")";

            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                await con.OpenAsync(); //con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            ret = Convert.ToInt32(dr["CountDirNomenID"].ToString());
                        }
                    }
                }
                con.Close();
            }

            return ret;
        }

        private async Task<bool> RemRemnantsCorrect(string ConStr)
        {
            string SQL = "";

            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                await con.OpenAsync(); //con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("", con))
                {
                    cmd.CommandText = "DELETE FROM RemRemnants;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText =
                        "INSERT INTO RemRemnants " +
                        "(DirContractorIDOrg, DirNomenID, DirWarehouseID, Quantity) " +
                        "SELECT DirContractorIDOrg, DirNomenID, DirWarehouseID, SUM(Remnant) " +
                        "FROM RemParties " +
                        "GROUP BY DirContractorIDOrg, DirWarehouseID, DirNomenID";
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

            return true;
        }

        #endregion



        #region Updating

        string _ConStr = "";
        internal async Task<string[]> UpdatingOne(string ConStr, int DirCustomersID)
        {
            string[] ret = { "true", "true" };
            _ConStr = ConStr;

            try
            {
                #region Update

                //1.Получаем версию БД
                int iSysVerNumber = 0;
                using (DbConnectionSklad db = new DbConnectionSklad(ConStr))
                {
                    iSysVerNumber = db.SysVers.Where(x => x.SysVerID == 1).ToList()[0].SysVerNumber;
                }

                if (iSysVerNumber == 90)
                {
                    ConStr = connectionString.Return(DirCustomersID, null, false);
                    _ConStr = ConStr;
                }

                using (DbConnectionSklad db = new DbConnectionSklad(ConStr))
                {
                    //1.Получаем версию БД
                    //int iSysVerNumber = db.SysVers.Where(x => x.SysVerID == 1).ToList()[0].SysVerNumber;

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        //2.Запускаем соответствующее обновление.
                        switch (iSysVerNumber)
                        {
                            case 0: await Task.Run(() => Update1(db)); break;
                            case 1: await Task.Run(() => Update2(db)); break;
                            case 2: await Task.Run(() => Update3(db)); break;
                            case 3: await Task.Run(() => Update4(db)); break;
                            case 4: await Task.Run(() => Update5(db)); break;
                            case 5: await Task.Run(() => Update6(db)); break;
                            case 6: await Task.Run(() => Update7(db)); break;
                            case 7: await Task.Run(() => Update8(db)); break;
                            case 8: await Task.Run(() => Update9(db)); break;
                            case 9: await Task.Run(() => Update10(db)); break;
                            case 10: await Task.Run(() => Update11(db)); break;
                            case 11: await Task.Run(() => Update12(db)); break;
                            case 12: await Task.Run(() => Update13(db)); break;
                            case 13: await Task.Run(() => Update14(db)); break;
                            case 14: await Task.Run(() => Update15(db)); break;
                            case 15: await Task.Run(() => Update16(db)); break;
                            case 16: await Task.Run(() => Update17(db)); break;
                            case 17: await Task.Run(() => Update18(db)); break;
                            case 18: await Task.Run(() => Update19(db)); break;
                            case 19: await Task.Run(() => Update20(db)); break;
                            case 20: await Task.Run(() => Update21(db)); break;
                            case 21: await Task.Run(() => Update22(db)); break;
                            case 22: await Task.Run(() => Update23(db)); break;
                            case 23: await Task.Run(() => Update24(db)); break;
                            case 24: await Task.Run(() => Update25(db)); break;
                            case 25: await Task.Run(() => Update26(db)); break;
                            case 26: await Task.Run(() => Update27(db)); break;
                            case 27: await Task.Run(() => Update28(db)); break;
                            case 28: await Task.Run(() => Update29(db)); break;
                            case 29: await Task.Run(() => Update30(db)); break;
                            case 30: await Task.Run(() => Update31(db)); break;
                            case 31: await Task.Run(() => Update32(db)); break;
                            case 32: await Task.Run(() => Update33(db)); break;
                            case 33: await Task.Run(() => Update34(db)); break;
                            case 34: await Task.Run(() => Update35(db)); break;
                            case 35: await Task.Run(() => Update36(db)); break;
                            case 36: await Task.Run(() => Update37(db)); break;
                            case 37: await Task.Run(() => Update38(db)); break;
                            case 38: await Task.Run(() => Update39(db)); break;
                            case 39: await Task.Run(() => Update40(db)); break;
                            case 40: await Task.Run(() => Update41(db)); break;
                            case 41: await Task.Run(() => Update42(db)); break;
                            case 42: await Task.Run(() => Update43(db)); break;
                            case 43: await Task.Run(() => Update44(db)); break;
                            case 44: await Task.Run(() => Update45(db)); break;
                            case 45: await Task.Run(() => Update46(db)); break;
                            case 46: await Task.Run(() => Update47(db)); break;
                            case 47: await Task.Run(() => Update48(db)); break;
                            case 48: await Task.Run(() => Update49(db)); break;
                            case 49: await Task.Run(() => Update50(db)); break;
                            case 50: await Task.Run(() => Update51(db)); break;
                            case 51: await Task.Run(() => Update52(db)); break;
                            case 52: await Task.Run(() => Update53(db)); break;
                            case 53: await Task.Run(() => Update54(db)); break;
                            case 54: await Task.Run(() => Update55(db)); break;
                            case 55: await Task.Run(() => Update56(db)); break;
                            case 56: await Task.Run(() => Update57(db)); break;

                                //ПФ "Инвентаризация"
                            case 57: await Task.Run(() => Update58(db)); break;
                            case 58: await Task.Run(() => Update59(db)); break;

                            case 59: await Task.Run(() => Update60(db)); break;
                            case 60: await Task.Run(() => Update61(db)); break;
                            case 61: await Task.Run(() => Update62(db)); break;

                            //Блядь!!! Блядь!!! Блядь!!! Блядь!!! Блядь!!! 
                            case 62: await Task.Run(() => Update63(db)); break;




                            //!!! Новый Релиз !!!


                            case 63: await Task.Run(() => Update64(db)); break;
                            case 64: await Task.Run(() => Update65(db)); break;
                            case 65: await Task.Run(() => Update66(db)); break;
                            case 66: await Task.Run(() => Update67(db)); break;
                            case 67: await Task.Run(() => Update68(db)); break;
                            case 68: await Task.Run(() => Update69(db)); break;
                            case 69: await Task.Run(() => Update70(db)); break;
                            case 70: await Task.Run(() => Update71(db)); break;
                            case 71: await Task.Run(() => Update72(db)); break;
                            case 72: await Task.Run(() => Update73(db)); break;
                            case 73: await Task.Run(() => Update74(db)); break;
                            case 74: await Task.Run(() => Update75(db)); break;
                            case 75: await Task.Run(() => Update76(db)); break;
                            case 76: await Task.Run(() => Update77(db)); break;
                            case 77: await Task.Run(() => Update78(db)); break;
                            case 78: await Task.Run(() => Update79(db)); break;
                            case 79: await Task.Run(() => Update80(db)); break;
                            case 80: await Task.Run(() => Update81(db)); break;
                            case 81: await Task.Run(() => Update82(db)); break;
                            case 82: await Task.Run(() => Update83(db)); break;
                            case 83: await Task.Run(() => Update84(db)); break;
                            case 84: await Task.Run(() => Update85(db)); break;
                            case 85: await Task.Run(() => Update86(db)); break;
                            case 86: await Task.Run(() => Update87(db)); break;
                            case 87: await Task.Run(() => Update88(db)); break;
                            case 88: await Task.Run(() => Update89(db)); break;
                            case 89: await Task.Run(() => Update90(db)); break;
                            case 90: await Task.Run(() => Update91(db)); break;
                            case 91: await Task.Run(() => Update92(db)); break;
                            case 92: await Task.Run(() => Update93(db)); break;
                            case 93: await Task.Run(() => Update94(db)); break;
                            case 94: await Task.Run(() => Update95(db)); break;
                            case 95: await Task.Run(() => Update96(db)); break;
                            case 96: await Task.Run(() => Update97(db)); break;
                            case 97: await Task.Run(() => Update98(db)); break;
                            case 98: await Task.Run(() => Update99(db)); break;
                            case 99: await Task.Run(() => Update100(db)); break;
                            case 100: await Task.Run(() => Update101(db)); break;
                            case 101: await Task.Run(() => Update102(db)); break;
                            case 102: await Task.Run(() => Update103(db)); break;




                            default: ret[0] = "false"; break;
                        }
                        if (Convert.ToBoolean(ret[0])) { ts.Commit(); }
                    }
                }

                #endregion


                #region Vacuum, Reindex, ANALYZE, integrity_check

                if (Convert.ToBoolean(ret[0]))
                {
                    Classes.Update.Prevention prevention = new Prevention();
                    ret[1] = prevention.UpdatingOne(ConStr, DirCustomersID);
                }

                #endregion

            }
            catch (Exception ex)
            {
                if (DirCustomersID > 0) try { jurnDispError.Write(db, exceptionEntry.Return(ex) + ". Class: PartionnyAccount.Controllers.Classes.Update.Update ", DirCustomersID); } catch { }
                ret[0] = exceptionEntry.Return(ex);
                //ret[1] = exceptionEntry.Return(ex);
            }


            return ret;
        }



        #region Update1 - СС - заполнение поля "DocServicePurches.Sums"

        private async Task<bool> Update1(DbConnectionSklad db)
        {
            //1. Алгоритм: 
            //Пробежатся по всем ремонтам.
            string SQL =
                "UPDATE DocServicePurches " +
                "SET " +
                " Sums = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurch1Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID) + " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurch2Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID) - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 1;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update2 - СС - заполнение поля "RemParties.DirEmployeeID.DocDate"

        private async Task<bool> Update2(DbConnectionSklad db)
        {
            //1. RemParties
            //1.1. DirEmployeeID из Docs
            string SQL =
                "UPDATE RemParties " +
                "SET DirEmployeeID = (SELECT DirEmployeeID FROM Docs WHERE RemParties.DocID = Docs.DocID);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));
            //1.2. DocDate из Docs
            SQL =
                "UPDATE RemParties " +
                "SET DocDate = (SELECT DocDate FROM Docs WHERE RemParties.DocID = Docs.DocID);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //2. RemPartyMinuses
            //2.1. DirEmployeeID из Docs
            SQL =
                "UPDATE RemPartyMinuses " +
                "SET DirEmployeeID = (SELECT DirEmployeeID FROM Docs WHERE RemPartyMinuses.DocID = Docs.DocID);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));



            //2.2. DirEmployeeID из DocServicePurch2Tabs
            #region DocServicePurch2Tabs

            /*
            SQL =
                "UPDATE RemPartyMinuses " +
                "SET DirEmployeeID = (SELECT DocServicePurch2Tabs.DirEmployeeID FROM DocServicePurches, DocServicePurch2Tabs WHERE RemPartyMinuses.DocID = DocServicePurches.DocID and DocServicePurches.DocServicePurchID=DocServicePurch2Tabs.DocServicePurchID);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));
            */

            var querySelect = 
                (
                    from x in db.DocServicePurch2Tabs
                    select x
                ).ToList();

            for (int i = 0; i < querySelect.Count(); i++)
            {
                int? DocID = querySelect[i].docServicePurch.DocID, DocServicePurch2TabID = querySelect[i].DocServicePurch2TabID;

                var queryUpdate = db.RemPartyMinuses.Where(x => x.DocID == DocID && x.FieldID == DocServicePurch2TabID).ToList();
                if (queryUpdate.Count() > 0)
                {
                    Models.Sklad.Rem.RemPartyMinus remPartyMinus = queryUpdate[0];
                    remPartyMinus.DocDate = querySelect[i].docServicePurch.doc.DocDate;
                    remPartyMinus.DirEmployeeID = querySelect[i].docServicePurch.doc.DirEmployeeID;

                    db.Entry(remPartyMinus).State = EntityState.Modified;
                    await Task.Run(() => db.SaveChangesAsync());
                }
            }

            #endregion


            //1.2. DocDate из Docs
            SQL =
                "UPDATE RemPartyMinuses " +
                "SET DocDate = (SELECT DocDate FROM Docs WHERE RemPartyMinuses.DocID = Docs.DocID);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 2;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update3 - СС - заполнение поля "DocServicePurches.Sums"

        private async Task<bool> Update3(DbConnectionSklad db)
        {
            //1. Алгоритм: 
            //Пробежатся по всем ремонтам.
            string SQL =
                "UPDATE DocServicePurches " +
                "SET " +
                " Sums = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurch1Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID) + " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurch2Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID) - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 3;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update4 - СС - заполнение поля "DocServicePurches.Sums"

        private async Task<bool> Update4(DbConnectionSklad db)
        {
            //1. Алгоритм: 
            //Пробежатся по всем ремонтам.
            string SQL =
                "UPDATE DocServicePurches " +
                "SET " +
                " Sums = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurch1Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID) + " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurch2Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID) - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 4;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update5 - Приход.Контрагенты: Перенести

        private async Task<bool> Update5(DbConnectionSklad db)
        {
            //Алгоритм: 

            //1. Перенести из "DirCharStyles.DirCharStyleName" в "DirContractors.DirContractorName, ..."

            //2. В "DocPurchTabs" создать поле "DirCharStyleName"
            //   Перенести в DocPurchTabs.DirCharStyleName = DocPurchTabs.docPurchTab.DirCharStyleName
            //3. В "DocPurchTabs" создать поле "DirContractorID FK ..."

            //4. Найти "DocPurchTabs.DirCharStyleName" в "DirContractors.DirContractorName" и 
            //   и записать найденный "DirContractors.DirContractorID" в "DocPurchTabs.DirContractorID"


            //Ну и заменить в коде "DirCharStyleID" на "DirContractorID"



            //1. Перенести из "DirCharStyles.DirCharStyleName" в "DirContractors.DirContractorName, ..."
            string SQL =
                "INSERT INTO DirContractors (DirContractor1TypeID, DirContractor2TypeID, DirContractorName) " +
                "SELECT 1, 2, DirCharStyles.DirCharStyleName FROM DirCharStyles WHERE DirCharStyles.DirCharStyleName not in (SELECT DirContractors.DirContractorName FROM DirContractors);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //2. В "DocPurchTabs" создать поле "DirCharStyleName"
            //   Перенести в DocPurchTabs.DirCharStyleName = DocPurchTabs.docPurchTab.DirCharStyleName - сделал ниже
            //3. В "DocPurchTabs" создать поле "DirContractorID FK ..."
            SQL =
                "ALTER TABLE DocPurchTabs ADD COLUMN DirCharStyleName TEXT(256);" +
                "ALTER TABLE DocPurchTabs ADD COLUMN [DirContractorID] INTEGER CONSTRAINT [FK_DocPurchTabs_DirContractorID] REFERENCES [DirContractors]([DirContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED;";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //   Перенести в DocPurchTabs.DirCharStyleName = DocPurchTabs.docPurchTab.DirCharStyleName
            SQL =
                "UPDATE DocPurchTabs SET DirCharStyleName=" +
                "(SELECT DirCharStyleName FROM DirCharStyles WHERE DocPurchTabs.DirCharStyleID = DirCharStyles.DirCharStyleID);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));



            //4. Найти "DocPurchTabs.DirCharStyleName = DirContractors.DirContractorName" и 
            //   записать найденный "DirContractors.DirContractorID" в "DocPurchTabs.DirContractorID"
            SQL =
                "UPDATE DocPurchTabs SET DirContractorID=" +
                "(SELECT DirContractorID FROM DirContractors WHERE DocPurchTabs.DirCharStyleName = DirContractors.DirContractorName);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 5;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update6 - Добавления поля "DateStatusChange" + 2 индекса

        private async Task<bool> Update6(DbConnectionSklad db)
        {
            string SQL =
                "ALTER TABLE DocServicePurches ADD COLUMN DateStatusChange DATE; " +
                "CREATE INDEX [IDX_DocServicePurches_DateStatusChange] ON [DocServicePurches] ([DateStatusChange]); " +
                "CREATE INDEX [IDX_DocServicePurches_IssuanceDate] ON [DocServicePurches] ([IssuanceDate]); ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            SQL = "UPDATE DocServicePurches SET DateStatusChange=DocServicePurches.IssuanceDate; ";
            SQLiteParameter parDateStatusChange = new SQLiteParameter("@DateStatusChange", System.Data.DbType.Date) { Value = DateTime.Now };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parDateStatusChange));


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 6;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update7 - Добавления поля "RightReportTotalTradePrice и RightReportTotalTradePriceCheck" в "DirEmployees"

        private async Task<bool> Update7(DbConnectionSklad db)
        {
            string SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightReportTotalTradePrice] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightReportTotalTradePriceCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightReportTotalTradePrice=1, RightReportTotalTradePriceCheck=1 WHERE DirEmployeeID=1; ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 7;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update8 - Добавления поля "DateStatusChange" в "SysSettings"

        private async Task<bool> Update8(DbConnectionSklad db)
        {
            string SQL =
                "ALTER TABLE SysSettings ADD COLUMN [DocServicePurchSmsAutoShow] BOOL NOT NULL DEFAULT 0; " +
                "INSERT INTO DirSmsTemplates (DirSmsTemplateID, DirSmsTemplateName, DirSmsTemplateMsg, DirSmsTemplateType, MenuID)values(6, 'Приёмка аппарата', 'Ваш аппарата принят в ремонт', 1, 1); ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 8;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update9 - Добавления поля "ServiceKPD" в "SysSettings"

        private async Task<bool> Update9(DbConnectionSklad db)
        {
            string SQL =
                "ALTER TABLE SysSettings ADD COLUMN [ServiceKPD] REAL NOT NULL DEFAULT 300; ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 9;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update10 - Добавления поля "Sums1 and Sums2" в "DocServicePurches"

        private async Task<bool> Update10(DbConnectionSklad db)
        {
            #region 1. Суммы выполенных работ и запчастей

            string SQL =
                "ALTER TABLE DocServicePurches ADD COLUMN [Sums1] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DocServicePurches ADD COLUMN [Sums2] REAL NOT NULL DEFAULT 0; ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. Удаляем старые тригерры

            SQL =
                "DROP TRIGGER TG_UPDATE_DocServicePurch1Tabs_DocServicePurches; " +
                "DROP TRIGGER TG_INSERT_DocServicePurch1Tabs_DocServicePurches; " +
                "DROP TRIGGER TG_DELETE_DocServicePurch1Tabs_DocServicePurches; " +

                "DROP TRIGGER TG_INSERT_DocServicePurch2Tabs_DocServicePurches; " +
                "DROP TRIGGER TG_UPDATE_DocServicePurch2Tabs_DocServicePurches; " +
                "DROP TRIGGER TG_DELETE_DocServicePurch2Tabs_DocServicePurches; ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 3. Создаём новые тригерры

            SQL =
                "CREATE TRIGGER[TG_UPDATE_DocServicePurch1Tabs_DocServicePurches] " +
                "AFTER UPDATE " +
                "ON[DocServicePurch1Tabs] " +
                "BEGIN " +

                " UPDATE DocServicePurches " +
                " SET " +

                " Sums1 = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                "  ), " +

                " Sums2 = " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                " ), " +

                " Sums = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                "  )    " +
                " + " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                " ) " +
                " - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0) " +

                " WHERE " +
                "  DocServicePurchID = (SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1); " +

                "END; " +


                "CREATE TRIGGER[TG_INSERT_DocServicePurch1Tabs_DocServicePurches] " +
                "AFTER INSERT " +
                "ON[DocServicePurch1Tabs] " +
                "BEGIN " +

                " UPDATE DocServicePurches " +
                " SET " +

                " Sums1 = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                "  ),  " +

                " Sums2 = " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                " ), " +

                " Sums = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                "  ) " +
                " + " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                " ) " +
                " - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0) " +

                " WHERE " +
                "  DocServicePurchID = (SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = NEW.DocServicePurch1TabID LIMIT 1); " +

                "END; " +


                "CREATE TRIGGER[TG_DELETE_DocServicePurch1Tabs_DocServicePurches] " +
                "BEFORE DELETE " +
                "ON[DocServicePurch1Tabs] " +
                "BEGIN " +

                " UPDATE DocServicePurches " +
                " SET " +

                " Sums1 = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                "  ), " +

                " Sums2 = " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                " ), " +

                " Sums = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                "  ) " +
                " + " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1 " +
                "       ) " +
                " ) " +
                " - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0) " +

                " WHERE " +
                "  DocServicePurchID = (SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1); " +

                "END; " +


                "CREATE TRIGGER[TG_INSERT_DocServicePurch2Tabs_DocServicePurches] " +
                "AFTER INSERT " +
                "ON[DocServicePurch2Tabs] " +
                "BEGIN " +

                " UPDATE DocServicePurches " +
                " SET " +

                " Sums1 = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                "  ), " +

                " Sums2 = " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                " ), " +

                " Sums = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                "  ) " +
                " + " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                " ) " +
                " - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0) " +

                " WHERE " +
                "  DocServicePurchID = (SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1); " +

                "END; " +


                "CREATE TRIGGER[TG_UPDATE_DocServicePurch2Tabs_DocServicePurches] " +
                "AFTER UPDATE " +
                "ON[DocServicePurch2Tabs] " +
                "BEGIN " +

                " UPDATE DocServicePurches " +
                " SET " +

                " Sums1 = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                "  ),  " +

                " Sums2 = " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                " ), " +


                " Sums = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                "  )  " +
                " + " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                " ) " +
                " - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0) " +

                " WHERE " +
                "  DocServicePurchID = (SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = NEW.DocServicePurch2TabID LIMIT 1); " +

                "END; " +


                "CREATE TRIGGER[TG_DELETE_DocServicePurch2Tabs_DocServicePurches] " +
                "BEFORE DELETE " +
                "ON[DocServicePurch2Tabs] " +
                "BEGIN " +

                " UPDATE DocServicePurches " +
                " SET " +

                " Sums1 = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                "  ),  " +

                " Sums2 = " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                " ), " +

                " Sums = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                "  ) " +
                " + " +
                " ( " +
                "  SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = " +
                "       ( " +
                "        SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1 " +
                "       ) " +
                " ) " +
                " - " +
                " IFNULL(DocServicePurches.PrepaymentSum, 0) " +

                " WHERE " +
                "  DocServicePurchID = (SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1); " +

                "END; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            #region 4. Пересчитываем суммы Sums1 и Sums2

            SQL =
                "UPDATE DocServicePurches " +
                "SET " +
                " Sums1 = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurch1Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID); " +

                "UPDATE DocServicePurches " +
                "SET " +
                " Sums2 = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurch2Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 10;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update11 - Б/У - 1

        private async Task<bool> Update11(DbConnectionSklad db)
        {
            #region 1. Права

            string SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHands0] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHands0=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandPurches] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandPurchesCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandPurches=1, RightDocSecondHandPurchesCheck=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandRetails] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandRetailsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandRetails=1, RightDocSecondHandRetailsCheck=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandsReport] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandsReportCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandsReport=1, RightDocSecondHandsReportCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. Документ "DocSecondHands"

            //...

            #endregion


            #region 3. ListObjects: 64, 3, DocSecondHands, Документ Б/У

            //...

            #endregion


            #region 4. ListObjectPFs

            //...

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 11;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update12 - Б/У - 2

        private async Task<bool> Update12(DbConnectionSklad db)
        {
  
            #region 1. Права

            //Добавить права:
            //RightDocSecondHandWorkshops, RightDocSecondHandWorkshopsCheck
            //RightDocSecondHandPurch1Tabs, RightDocSecondHandPurch1TabsCheck
            //RightDocSecondHandPurch2Tabs, RightDocSecondHandPurch2TabsCheck
            //RightDocSecondHandRetailReturns, RightDocSecondHandRetailReturnsCheck
            //RightDocSecondHandRetailActWriteOffs, RightDocSecondHandRetailActWriteOffsCheck, 

            string SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandWorkshops] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandWorkshopsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandWorkshops=1, RightDocSecondHandWorkshopsCheck=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandPurch1Tabs] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandPurch1TabsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandPurch1Tabs=1, RightDocSecondHandPurch1TabsCheck=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandPurch2Tabs] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandPurch2TabsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandPurch2Tabs=1, RightDocSecondHandPurch2TabsCheck=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandRetailReturns] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandRetailReturnsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandRetailReturns=1, RightDocSecondHandRetailReturnsCheck=1 WHERE DirEmployeeID=1; " +


                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandRetailActWriteOffs] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandRetailActWriteOffsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandRetailActWriteOffs=1, RightDocSecondHandRetailActWriteOffsCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 2. DirServiceContractors

            //В таблицу "DirServiceContractors" добавить 2-а поля "PassportSeries" and "PassportNumber"
            SQL =
                "ALTER TABLE DirServiceContractors ADD COLUMN [PassportSeries] TEXT(32); " +
                "ALTER TABLE DirServiceContractors ADD COLUMN [PassportNumber] TEXT(32); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 3. DirSecondHandStatuses

            //1. Создать таблицу "DirSecondHandStatuses"
            //2. Заполнить статусами

            SQL =
                "CREATE TABLE[DirSecondHandStatuses]( " +
                "  [DirSecondHandStatusID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "  [DirSecondHandStatusName] TEXT(256) NOT NULL, " +
                "  [SortNum] INTEGER NOT NULL); " +
                "CREATE UNIQUE INDEX[UDX_DirSecondHandStatuses_SortNum] ON[DirSecondHandStatuses]([SortNum]); " + 
            
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(1, 'Куплен', 1); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(2, 'Предпродажа (мастерская)', 2); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(3, 'На согласовании', 3); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(4, 'Согласовано', 4); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(5, 'Ожидает запчастей', 5); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(7, 'Готов для продажи', 6); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(8, 'На разбор', 7); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(9, 'Выдан', 8); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 4. DocSecondHandPurches

            //Создать таблицу "DocSecondHandPurches"
            //В таблицу "ListObjects" добавить 1 запись: (65, 3, DocSecondHandPurches, Документ Б/У)

            SQL =
                "CREATE TABLE [DocSecondHandPurches] ( " +
                "  [DocSecondHandPurchID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurches_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurches_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurches_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirSecondHandStatusID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurches_DirSecondHandStatusID] REFERENCES [DirSecondHandStatuses]([DirSecondHandStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirSecondHandStatusID_789] INTEGER CONSTRAINT [FK_DocSecondHandPurches_DirSecondHandStatusID_789] REFERENCES [DirSecondHandStatuses]([DirSecondHandStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [SerialNumberNo] BOOL DEFAULT 0,  " +
                "  [SerialNumber] TEXT(256),  " +
                "  [ComponentPasTextNo] BOOL DEFAULT 0,  " +
                "  [ComponentPasText] TEXT(256),  " +
                "  [ComponentOtherText] TEXT(256),  " +
                "  [ProblemClientWords] TEXT(2048), " +
                "  [Note] TEXT(2048),  " +
                "  [DirEmployeeIDMaster] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurches_DirEmployeeIDMaster] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceContractorName] TEXT(256),  " +
                "  [DirServiceContractorRegular] BOOL DEFAULT 0,  " +
                "  [DirServiceContractorID] INTEGER CONSTRAINT [FK_DocSecondHandPurches_DirServiceContractorID] REFERENCES [DirServiceContractors]([DirServiceContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceContractorAddress] TEXT,  " +
                "  [DirServiceContractorPhone] TEXT,  " +
                "  [DirServiceContractorEmail] TEXT,  " +
                "  [PassportSeries] TEXT,  " +
                "  [PassportNumber] TEXT,  " +
                "  [ServiceTypeRepair] INTEGER NOT NULL DEFAULT 1,  " +
                "  [PriceVAT] REAL NOT NULL,  " +
                "  [PriceCurrency] REAL NOT NULL,  " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurches_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1,  " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1,  " +
                "  [PriceRetailVAT] REAL NOT NULL,  " +
                "  [PriceRetailCurrency] REAL NOT NULL,  " +
                "  [PriceWholesaleVAT] REAL NOT NULL,  " +
                "  [PriceWholesaleCurrency] REAL NOT NULL,  " +
                "  [PriceIMVAT] REAL NOT NULL,  " +
                "  [PriceIMCurrency] REAL NOT NULL,  " +
                "  [ComponentOther] BOOL DEFAULT 0,  " +
                "  [DateDone] DATE NOT NULL,  " +
                "  [IssuanceDate] DATE,  " +
                "  [Summ_NotPre] REAL,  " +
                "  [Sums] REAL,  " +
                "  [DocDate_First] DATE,  " +
                "  [DateStatusChange] DATE,  " +
                "  [Sums1] REAL NOT NULL DEFAULT 0,  " +
                "  [Sums2] REAL NOT NULL DEFAULT 0); " +

                "CREATE INDEX [IDX_DocSecondHandPurches_DateStatusChange] ON [DocSecondHandPurches] ([DateStatusChange]); " +

                "CREATE INDEX [IDX_DocSecondHandPurches_DirSecondHandStatusID] ON [DocSecondHandPurches] ([DirSecondHandStatusID]); " +

                "CREATE INDEX [IDX_DocSecondHandPurches_DirSecondHandStatusID_789] ON [DocSecondHandPurches] ([DirSecondHandStatusID_789]); " +

                "CREATE INDEX [IDX_DocSecondHandPurches_DirServiceContractorID] ON [DocSecondHandPurches] ([DirServiceContractorID]); " +

                "CREATE INDEX [IDX_DocSecondHandPurches_DirServiceNomenID] ON [DocSecondHandPurches] ([DirServiceNomenID]); " +

                "CREATE INDEX [IDX_DocSecondHandPurches_DirWarehouseID] ON [DocSecondHandPurches] ([DirWarehouseID]); " +

                "CREATE INDEX [IDX_DocSecondHandPurches_IssuanceDate] ON [DocSecondHandPurches] ([IssuanceDate]); " +

                "CREATE INDEX [IDX_DocSecondHandPurches_SerialNumber] ON [DocSecondHandPurches] ([SerialNumber] COLLATE NOCASE); " +


                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(65, 3, 'DocSecondHandPurches', 'Документ Б/У'); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 4.1. DocSecondHandPurch1Tabs

            //Создать таблицу "DocSecondHandPurch1Tabs"

            SQL =
                "CREATE TABLE [DocSecondHandPurch1Tabs] ( " +
                "  [DocSecondHandPurch1TabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocSecondHandPurchID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurch1Tabs_DocSecondHandPurchID] REFERENCES [DocSecondHandPurches]([DocSecondHandPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirEmployeeID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurch1Tabs_DirEmployeeID] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceJobNomenID] INTEGER CONSTRAINT [FK_DocSecondHandPurch1Tabs_DirServiceJobNomenID] REFERENCES [DirServiceJobNomens]([DirServiceJobNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceJobNomenName] TEXT(256),  " +
                "  [PriceVAT] REAL NOT NULL,  " +
                "  [PriceCurrency] REAL NOT NULL,  " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSaleTab_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1,  " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1,  " +
                "  [DiagnosticRresults] TEXT(1024),  " +
                "  [TabDate] DATETIME,  " +
                "  [DirSecondHandStatusID] INTEGER); " +

                "CREATE INDEX [IDX_DocSecondHandPurch1Tabs_DirEmployeeID] ON [DocSecondHandPurch1Tabs] ([DirEmployeeID]); " +

                "CREATE TRIGGER [TG_UPDATE_DocSecondHandPurch1Tabs_DocSecondHandPurches] " +
                "AFTER UPDATE " +
                "ON [DocSecondHandPurch1Tabs] " +
                "BEGIN  UPDATE DocSecondHandPurches  SET  Sums1 =   (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )   ),  Sums2 =  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )  ),  Sums =   (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )   )     +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )  )  WHERE   DocSecondHandPurchID = (SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1); END; " +

                "CREATE TRIGGER [TG_INSERT_DocSecondHandPurch1Tabs_DocSecondHandPurches] " +
                "AFTER INSERT " +
                "ON [DocSecondHandPurch1Tabs] " +
                "BEGIN  UPDATE DocSecondHandPurches  SET  Sums1 =   (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )   ),   Sums2 =  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )  ),  Sums =   (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )   )  +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )  )  WHERE   DocSecondHandPurchID = (SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1); END; " +

                "CREATE TRIGGER [TG_DELETE_DocSecondHandPurch1Tabs_DocSecondHandPurches] " +
                "BEFORE DELETE " +
                "ON [DocSecondHandPurch1Tabs] " +

                "BEGIN " +


                " UPDATE DocSecondHandPurches " +
                " SET " +

                "  Sums1 = Sums1 - " +
                "  ( " +
                "    SELECT IFNULL(DocSecondHandPurch1Tabs.PriceCurrency, 0) " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1  " +
                "  ),  " +

                "  Sums2 = " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) " +
                "   FROM DocSecondHandPurch2Tabs " +
                "   WHERE DocSecondHandPurchID = " +
                "   ( " +
                "    SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 " +
                "   ) " +
                "  ), " +

                "  Sums = " +
                "  Sums1 - " +
                "  ( " +
                "    SELECT IFNULL(DocSecondHandPurch1Tabs.PriceCurrency, 0) " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 " +
                "  ) " +
                "  + " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) " +
                "   FROM DocSecondHandPurch2Tabs " +
                "   WHERE DocSecondHandPurchID =  " +
                "   ( " +
                "    SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID  " +
                "    FROM DocSecondHandPurch1Tabs  " +
                "    WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 " +
                "   ) " +
                "  ) " +

                " WHERE    " +
                "  DocSecondHandPurchID =  " +
                "  ( " +
                "   SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID  " +
                "   FROM DocSecondHandPurch1Tabs  " +
                "   WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 " +
                "  );  " +

                "END; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 4.2. DocSecondHandPurch2Tabs

            //Создать таблицу "DocSecondHandPurch1Tabs"

            SQL =
                "CREATE TABLE [DocSecondHandPurch2Tabs] ( " +
                "  [DocSecondHandPurch2TabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "  [DocSecondHandPurchID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurch1Tabs_DocSecondHandPurchID] REFERENCES [DocSecondHandPurches]([DocSecondHandPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirEmployeeID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurch2Tabs_DirEmployeeID] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandPurch1Tabs_DirSecondHandJobNomenID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [RemPartyID] INTEGER CONSTRAINT [FK_DocSecondHandPurch2Tabs_RemPartyID] REFERENCES [RemParties]([RemPartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [PriceVAT] REAL NOT NULL, " +
                "  [PriceCurrency] REAL NOT NULL, " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSaleTab_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1, " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1, " +
                "  [TabDate] DATETIME); " +

                "CREATE INDEX [IDX_DocSecondHandPurch2Tabs_DirEmployeeID] ON [DocSecondHandPurch2Tabs] ([DirEmployeeID]); " +

                "CREATE INDEX [IDX_DocSecondHandPurch2Tabs_RemPartyID] ON [DocSecondHandPurch2Tabs] ([RemPartyID]); " +

                "CREATE TRIGGER [TG_INSERT_DocSecondHandPurch2Tabs_DocSecondHandPurches] " +
                "AFTER INSERT " +
                "ON [DocSecondHandPurch2Tabs] " +
                "BEGIN  UPDATE DocSecondHandPurches  SET  Sums1 =   (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )   ),  Sums2 =  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )  ),  Sums =   (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )   )  +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )  )  WHERE   DocSecondHandPurchID = (SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1); END; " +

                "CREATE TRIGGER [TG_UPDATE_DocSecondHandPurch2Tabs_DocSecondHandPurches] " +
                "AFTER UPDATE " +
                "ON [DocSecondHandPurch2Tabs] " +
                "BEGIN  UPDATE DocSecondHandPurches  SET  Sums1 =   (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )   ),   Sums2 =  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )  ),  Sums =   (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )   )   +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )  )  WHERE   DocSecondHandPurchID = (SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1); END; " +

                "CREATE TRIGGER [TG_DELETE_DocSecondHandPurch2Tabs_DocSecondHandPurches] " +
                "BEFORE DELETE " +
                "ON [DocSecondHandPurch2Tabs] " +

                "BEGIN " +

                " UPDATE DocSecondHandPurches " +
                " SET " +

                "  Sums1 = " +
                "   ( " +
                "    SELECT IFNULL(SUM([DocSecondHandPurch1Tabs].PriceCurrency), 0) " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurchID = " +
                "    ( " +
                "     SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID " +
                "     FROM DocSecondHandPurch2Tabs " +
                "     WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "    ) " +
                "   ), " +

                "  Sums2 = " +
                "   Sums2 - " +
                "   ( " +
                "     SELECT IFNULL(DocSecondHandPurch2Tabs.PriceCurrency, 0) " +
                "     FROM DocSecondHandPurch2Tabs " +
                "     WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "   ), " +

                "  Sums = " +
                "   ( " +
                "    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurchID = " +
                "    ( " +
                "     SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID " +
                "     FROM DocSecondHandPurch2Tabs " +
                "     WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "    ) " +
                "   ) " +
                "   + " +
                "   Sums2 - " +
                "   ( " +
                "     SELECT IFNULL(DocSecondHandPurch2Tabs.PriceCurrency, 0) " +
                "     FROM DocSecondHandPurch2Tabs " +
                "     WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "   ) " +


                "  WHERE " +
                "   DocSecondHandPurchID = " +
                "   ( " +
                "    SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID " +
                "    FROM DocSecondHandPurch2Tabs " +
                "    WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "   ); " +

                " END;";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion
            
            #region 5. LogSecondHands and DirSecondHandLogTypes

            //Создать таблицу "DirSecondHandLogTypes"
            //Создать таблицу "LogSecondHands"

            SQL =
                "CREATE TABLE [DirSecondHandLogTypes] ( " +
                "  [DirSecondHandLogTypeID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "  [Del] BOOL NOT NULL DEFAULT 0, " +
                "  [DirSecondHandLogTypeName] TEXT(256) NOT NULL); " +

                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(1, 'Смена статуса'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(2, 'Смена инженера'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(3, 'Комментарии и заметки'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(4, 'Отправка СМС'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(5, 'Выполненная работа'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(6, 'Запчасть'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(7, 'Другое'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(8, 'Смена гарантии'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(9, 'Возврат по гарантии'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            SQL = 
                "CREATE TABLE [LogSecondHands] ( " +
                "  [LogSecondHandID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocSecondHandPurchID] INTEGER NOT NULL CONSTRAINT [FK_LogSecondHands_DocServicePurchID] REFERENCES [DocSecondHandPurches]([DocSecondHandPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirSecondHandLogTypeID] INTEGER NOT NULL CONSTRAINT [FK_LogSecondHands_DirServiceLogTypeID] REFERENCES [DirSecondHandLogTypes]([DirSecondHandLogTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirEmployeeID] INTEGER NOT NULL CONSTRAINT [FK_LogSecondHands_DirEmployees] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirSecondHandStatusID] INTEGER CONSTRAINT [FK_LogSecondHands_DirSecondHandStatusID] REFERENCES [DirSecondHandStatuses]([DirSecondHandStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [LogSecondHandDate] DATETIME DEFAULT (datetime(CURRENT_TIMESTAMP, 'localtime')),  " +
                "  [Msg] TEXT(1024)); " +

                "CREATE INDEX [IDX_LogSecondHands_DirEmployeeID] ON [LogSecondHands] ([DirEmployeeID]); " +

                "CREATE INDEX [IDX_LogSecondHands_DirSecondHandStatusID] ON [LogSecondHands] ([DirSecondHandStatusID]); " +

                "CREATE INDEX [IDX_LogSecondHands_DirServiceLogTypeID] ON [LogSecondHands] ([DirSecondHandLogTypeID]); " +

                "CREATE INDEX [IDX_LogSecondHands_DocServicePurchID] ON [LogSecondHands] ([DocSecondHandPurchID]);";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            #endregion

            #region 6. LogSecondHands, DirSecondHandLogTypes and ListObjects

            //Создать таблицу "DocSecondHandRetails"
            //Создать таблицу "DocSecondHandRetailTabs"
            //Создать запсь в "ListObjects" (66, 3, "DocSecondHandRetails", "Документ Б/У Розница", ...)

            SQL =
                "CREATE TABLE [DocSecondHandRetails] ( " +
                "  [DocSecondHandRetailID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetails_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetails_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Reserve] BOOL NOT NULL DEFAULT 1,  " +
                "  [OnCredit] BOOL NOT NULL DEFAULT 0); " +
                "CREATE INDEX [IDX_DocSecondHandRetails_DirWarehouseID] ON [DocSecondHandRetails] ([DirWarehouseID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetails_DocID] ON [DocSecondHandRetails] ([DocID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetails_OnCredit] ON [DocSecondHandRetails] ([OnCredit]); " +

                "CREATE TABLE [DocSecondHandRetailTabs] ( " +
                "  [DocSecondHandRetailTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocSecondHandRetailID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DocSecondHandRetailsID] REFERENCES [DocSecondHandRetails]([DocSecondHandRetailID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Rem2PartyID] INTEGER CONSTRAINT [FK_DocSecondHandRetailTabs_Rem2PartyID] REFERENCES [Rem2Parties]([Rem2PartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Quantity] REAL NOT NULL,  " +
                "  [DirPriceTypeID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTabs_DirPriceTypeID] REFERENCES [DirPriceTypes]([DirPriceTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [PriceVAT] REAL NOT NULL,  " +
                "  [PriceCurrency] REAL NOT NULL,  " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1,  " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1,  " +
                "  [DirReturnTypeID] INTEGER,  " +
                "  [DirDescriptionID] INTEGER " +
                "); " +
                "CREATE INDEX [IDX_DocSecondHandRetailTabs_DirServiceNomenID] ON [DocSecondHandRetailTabs] ([DirServiceNomenID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailTabs_DocSecondHandRetailID] ON [DocSecondHandRetailTabs] ([DocSecondHandRetailID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailTabs_Rem2PartyID] ON [DocSecondHandRetailTabs] ([Rem2PartyID]); " +

                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(66, 3, 'DocSecondHandRetails', 'Документ Б/У Розница'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));



            //Создать таблицу "DocSecondHandRetailReturns"
            //Создать таблицу "DocSecondHandRetailReturnTabs"
            //Создать запсь в "ListObjects" (67, 3, "DocSecondHandRetailReturns", "Документ Б/У Возврат", ...)

            SQL =
                "CREATE TABLE [DocSecondHandRetailReturns] ( " +
                "  [DocSecondHandRetailReturnID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT CONSTRAINT [FK_DocSecondHandRetailReturns_DocID] REFERENCES [DocSecondHandRetailReturns]([DocSecondHandRetailReturnID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DocID] INTEGER NOT NULL,  " +
                "  [DocSecondHandRetailID] INTEGER CONSTRAINT [FK_DocSecondHandRetailReturns_DocSecondHandRetailID] REFERENCES [DocSecondHandRetails]([DocSecondHandRetailID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailReturns_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Reserve] BOOL NOT NULL DEFAULT 1,  " +
                "  [OnCredit] BOOL NOT NULL DEFAULT 0); " +
                "CREATE INDEX [IDX_DocSecondHandRetailReturns_DirWarehouseID] ON [DocSecondHandRetailReturns] ([DirWarehouseID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailReturns_DocID] ON [DocSecondHandRetailReturns] ([DocID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailReturns_OnCredit] ON [DocSecondHandRetailReturns] ([OnCredit]); " +

                "CREATE TABLE [DocSecondHandRetailReturnTabs] ( " +
                "  [DocSecondHandRetailReturnTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocSecondHandRetailReturnID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailReturnTabs_DocSecondHandRetailReturnID] REFERENCES [DocSecondHandRetailReturns]([DocSecondHandRetailReturnID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailReturnTabs_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Rem2PartyMinusID] INTEGER CONSTRAINT [FK_DocSecondHandRetailReturnTabs_Rem2PartyMinusID] REFERENCES [Rem2PartyMinuses]([Rem2PartyMinusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Quantity] REAL NOT NULL,  " +
                "  [PriceVAT] REAL NOT NULL,  " +
                "  [PriceCurrency] REAL NOT NULL,  " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailReturnTabs_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCurrencyRate] REAL NOT NULL,  " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL,  " +
                "  [DirReturnTypeID] INTEGER CONSTRAINT [FK_DocSecondHandRetailReturnTabs_DirReturnTypeID] REFERENCES [DirPriceTypes]([DirPriceTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirDescriptionID] INTEGER CONSTRAINT [FK_DocSecondHandRetailReturnTabs_DirDescriptionID] REFERENCES [DirDescriptions]([DirDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED); " +
                "CREATE INDEX [IDX_DocSecondHandRetailReturnTabs_DirServiceNomenID] ON [DocSecondHandRetailReturnTabs] ([DirServiceNomenID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailReturnTabs_DocSecondHandRetailReturnID] ON [DocSecondHandRetailReturnTabs] ([DocSecondHandRetailReturnID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailReturnTabs_Rem2PartyMinusID] ON [DocSecondHandRetailReturnTabs] ([Rem2PartyMinusID]); " +

                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(67, 3, 'DocSecondHandRetailReturns', 'Документ Б/У Возврат'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));



            //Создать таблицу "DocSecondHandRetailActWriteOffs"
            //Создать таблицу "DocSecondHandRetailActWriteOffTabs"
            //Создать запсь в "ListObjects" (68, 3, "DocSecondHandRetailActWriteOffs", "Документ Б/У Списание", ...)

            SQL =
                "CREATE TABLE [DocSecondHandRetailActWriteOffs] ( " +
                "  [DocSecondHandRetailActWriteOffID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailActWriteOffs_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailActWriteOffs_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Reserve] BOOL NOT NULL DEFAULT 1 " +
                "); " +
                "CREATE INDEX [IDX_DocSecondHandRetailActWriteOffs_DirWarehouseID] ON [DocSecondHandRetailActWriteOffs] ([DirWarehouseID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailActWriteOffs_DocID] ON [DocSecondHandRetailActWriteOffs] ([DocID]); " +

                "CREATE TABLE [DocSecondHandRetailActWriteOffTabs] ( " +
                "  [DocSecondHandRetailActWriteOffTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocSecondHandRetailActWriteOffID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailActWriteOffTab_DocSecondHandRetailActWriteOffsID] REFERENCES [DocSecondHandRetailActWriteOffs]([DocSecondHandRetailActWriteOffID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailActWriteOffTab_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Rem2PartyID] INTEGER CONSTRAINT [FK_DocSecondHandRetailActWriteOffTabs_Rem2PartyID] REFERENCES [Rem2Parties]([Rem2PartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Quantity] REAL NOT NULL,  " +
                "  [DirPriceTypeID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailActWriteOffTabs_DirPriceTypeID] REFERENCES [DirPriceTypes]([DirPriceTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [PriceVAT] REAL NOT NULL,  " +
                "  [PriceCurrency] REAL NOT NULL,  " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailActWriteOffTab_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1,  " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1,  " +
                "  [DirReturnTypeID] INTEGER,  " +
                "  [DirDescriptionID] INTEGER); " +
                "CREATE INDEX [IDX_DocSecondHandRetailActWriteOffTabs_DirServiceNomenID] ON [DocSecondHandRetailActWriteOffTabs] ([DirServiceNomenID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailActWriteOffTabs_DocSecondHandRetailActWriteOffID] ON [DocSecondHandRetailActWriteOffTabs] ([DocSecondHandRetailActWriteOffID]); " +
                "CREATE INDEX [IDX_DocSecondHandRetailActWriteOffTabs_Rem2PartyID] ON [DocSecondHandRetailActWriteOffTabs] ([Rem2PartyID]); " +

                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(68, 3, 'DocSecondHandRetailActWriteOffs', 'Документ Б/У Списание'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            #endregion

            #region 7. DirCashOfficeSumTypes and DirBankSumTypes

            //Создать запсь в "DirCashOfficeSumTypes": 20, 21, 22, 23
            SQL =
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(21, 'Покупка Б/У №', -1); " +
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(22, 'Отмены покупки Б/У №', 1); " +

                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(23, 'Магазин продажа Б/У №', 1); " +
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(24, 'Отмены проведения продажи Б/У в магазине №', -1); " +
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(25, 'Магазин возврат Б/У №', -1); " +
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(26, 'Отмены проведения возврата Б/У в магазине №', 1); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            //Создать запсь в "DirBankSumTypes": 19, 20, 21, 22
            SQL =
                "INSERT INTO DirBankSumTypes (DirBankSumTypeID, DirBankSumTypeName, Sign)values(19, 'Покупка Б/У №', -1); " +
                "INSERT INTO DirBankSumTypes (DirBankSumTypeID, DirBankSumTypeName, Sign)values(20, 'Отмены покупки Б/У №', 1); " +

                "INSERT INTO DirBankSumTypes (DirBankSumTypeID, DirBankSumTypeName, Sign)values(21, 'Магазин продажа Б/У №', 1); " +
                "INSERT INTO DirBankSumTypes (DirBankSumTypeID, DirBankSumTypeName, Sign)values(22, 'Отмены проведения продажи в магазине №', -1); " +
                "INSERT INTO DirBankSumTypes (DirBankSumTypeID, DirBankSumTypeName, Sign)values(23, 'Магазин возврат Б/У №', -1); " +
                "INSERT INTO DirBankSumTypes (DirBankSumTypeID, DirBankSumTypeName, Sign)values(24, 'Отмены проведения возврата Б/У в магазине №', 1); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 8. Rem2Parties and Rem2PartyMinuses

            SQL =
                "CREATE TABLE [Rem2Parties] ( " +
                "  [Rem2PartyID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [DocID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DocDatePurches] DATE,  " +
                "  [DirContractorIDOrg] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirContractorIDOrg] REFERENCES [DirContractors]([DirContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceContractorID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirServiceContractorID] REFERENCES [DirServiceContractors]([DirServiceContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirWarehouseIDDebit] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirWarehouseIDDebit] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirWarehouseIDPurch] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirWarehouseIDPurch] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharColourID] INTEGER CONSTRAINT [FK_Rem2Parties_DirCharColourID] REFERENCES [DirCharColours]([DirCharColourID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCharMaterialID] INTEGER CONSTRAINT [FK_Rem2Parties_DirCharMaterialID] REFERENCES [DirCharMaterials]([DirCharMaterialID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCharNameID] INTEGER CONSTRAINT [FK_Rem2Parties_DirCharNameID] REFERENCES [DirCharNames]([DirCharNameID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCharSeasonID] INTEGER CONSTRAINT [FK_Rem2Parties_DirCharSeasonID] REFERENCES [DirCharSeasons]([DirCharSeasonID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCharSexID] INTEGER CONSTRAINT [FK_Rem2Parties_DirCharSexID] REFERENCES [DirCharSexes]([DirCharSexID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCharSizeID] INTEGER CONSTRAINT [FK_Rem2Parties_DirCharSizeID] REFERENCES [DirCharSizes]([DirCharSizeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCharStyleID] INTEGER CONSTRAINT [FK_Rem2Parties_DirCharStyleID] REFERENCES [DirCharStyles]([DirCharStyleID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCharTextureID] INTEGER CONSTRAINT [FK_Rem2Parties_DirCharTextureID] REFERENCES [DirCharTextures]([DirCharTextureID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [SerialNumber] TEXT(256),  " +
                "  [Barcode] TEXT(256),  " +
                "  [Quantity] REAL NOT NULL,  " +
                "  [Remnant] REAL NOT NULL,  " +
                "  [PriceVAT] REAL NOT NULL,  " +
                "  [DirVatValue] REAL NOT NULL,  " +
                "  [PriceCurrency] REAL NOT NULL,  " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [PriceRetailVAT] REAL NOT NULL DEFAULT 0,  " +
                "  [PriceRetailCurrency] REAL NOT NULL DEFAULT 0,  " +
                "  [PriceWholesaleVAT] REAL NOT NULL DEFAULT 0,  " +
                "  [PriceWholesaleCurrency] REAL NOT NULL DEFAULT 0,  " +
                "  [PriceIMVAT] REAL NOT NULL DEFAULT 0,  " +
                "  [PriceIMCurrency] REAL NOT NULL DEFAULT 0,  " +
                "  [FieldID] INTEGER NOT NULL,  " +
                "  [Rem2PartyMinusID] INTEGER,  " +
                "  [DirNomenMinimumBalance] REAL DEFAULT 0,  " +
                "  [DirReturnTypeID] INTEGER CONSTRAINT [FK_Rem2Parties_DirReturnTypeID] REFERENCES [DirReturnTypes]([DirReturnTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirDescriptionID] INTEGER CONSTRAINT [FK_Rem2Parties_DirDescriptionID] REFERENCES [DirDescriptions]([DirDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirEmployeeID] INTEGER CONSTRAINT [FK_Rem2Parties_DirEmployeeID] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DocDate] DATE); " +

                "CREATE INDEX [IDX_Rem2Parties_Barcode] ON [Rem2Parties] ([Barcode]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirServiceContractorID] ON [Rem2Parties] ([DirServiceContractorID]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirContractorIDOrg] ON [Rem2Parties] ([DirContractorIDOrg]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirContractorIDOrg_DirWarehouseID_DirServiceNomenID] ON [Rem2Parties] ([DirContractorIDOrg], [DirWarehouseID], [DirServiceNomenID]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirContractorIDOrg_DirWarehouseID_DirServiceNomenID_DirServiceContractorID] ON [Rem2Parties] ([DirContractorIDOrg], [DirWarehouseID], [DirServiceNomenID], [DirServiceContractorID]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirContractorIDOrg_DirWarehouseID_DirServiceNomenID_Remnant] ON [Rem2Parties] ([DirContractorIDOrg], [DirWarehouseID], [DirServiceNomenID], [Remnant]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirServiceNomenID] ON [Rem2Parties] ([DirServiceNomenID]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirNomenMinimumBalance] ON [Rem2Parties] ([DirNomenMinimumBalance]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirWarehouseID] ON [Rem2Parties] ([DirWarehouseID]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirWarehouseIDDebit] ON [Rem2Parties] ([DirWarehouseIDDebit]); " +
                "CREATE INDEX [IDX_Rem2Parties_DirWarehouseIDPurch] ON [Rem2Parties] ([DirWarehouseIDPurch]); " +
                "CREATE INDEX [IDX_Rem2Parties_DocID] ON [Rem2Parties] ([DocID]); " +
                "CREATE INDEX [IDX_Rem2Parties_FieldID] ON [Rem2Parties] ([FieldID]); " +
                "CREATE INDEX [IDX_Rem2Parties_Remnant] ON [Rem2Parties] ([Remnant] ASC); " +
                "CREATE INDEX [IDX_Rem2Parties_Remnant_DirContractorIDOrg] ON [Rem2Parties] ([Remnant], [DirContractorIDOrg]); " +
                "CREATE INDEX [IDX_Rem2Parties_Rem2PartyMinusID] ON [Rem2Parties] ([Rem2PartyMinusID]); " +
                "CREATE INDEX [IDX_Rem2Parties_SerialNumber] ON [Rem2Parties] ([SerialNumber]); " +





                "CREATE TABLE [Rem2PartyMinuses] ( " +
                "  [Rem2PartyMinusID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                "  [Rem2PartyID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_Rem2PartyID] REFERENCES [Rem2Parties]([Rem2PartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DocID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirContractorIDOrg] INTEGER NOT NULL CONSTRAINT [FK_Rem2PartyMinuses_DirContractorIDOrg] REFERENCES [DirContractors]([DirContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirServiceContractorID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirServiceContractorID] REFERENCES [DirServiceContractors]([DirServiceContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED,  " +
                "  [Quantity] REAL NOT NULL,  " +
                "  [PriceVAT] REAL NOT NULL,  " +
                "  [DirVatValue] REAL NOT NULL,  " +
                "  [PriceCurrency] REAL NOT NULL,  " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_Rem2Parties_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DirCurrencyRate] REAL NOT NULL,  " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL,  " +
                "  [Reserve] BOOL NOT NULL DEFAULT 0,  " +
                "  [FieldID] INTEGER NOT NULL,  " +
                "  [DirEmployeeID] INTEGER CONSTRAINT [FK_Rem2PartyMinuses_DirEmployeeID] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,  " +
                "  [DocDate] DATE); " +

                "CREATE INDEX [IDX_Rem2PartyMinuses_DirContractorIDOrg] ON [Rem2PartyMinuses] ([DirContractorIDOrg]); " +
                "CREATE INDEX [IDX_Rem2PartyMinuses_Rem2PartyID] ON [Rem2PartyMinuses] ([Rem2PartyID]); " +
                "CREATE INDEX [IDX_Rem2PartySales_DirServiceContractorID] ON [Rem2PartyMinuses] ([DirServiceContractorID]); " +
                "CREATE INDEX [IDX_Rem2PartySales_DirServiceNomenID] ON [Rem2PartyMinuses] ([DirServiceNomenID]); " +
                "CREATE INDEX [IDX_Rem2PartySales_DirWarehouseID] ON [Rem2PartyMinuses] ([DirWarehouseID]); " +
                "CREATE INDEX [IDX_Rem2PartySales_DocID] ON [Rem2PartyMinuses] ([DocID]); " +
                "CREATE INDEX [IDX_Rem2PartySales_Reserve] ON [Rem2PartyMinuses] ([Reserve]); " +

                "CREATE TRIGGER [TG_DELETE_Rem2PartyMinuses_Rem2Parties] " +
                "BEFORE DELETE " +
                "ON [Rem2PartyMinuses] " +
                "BEGIN " +
                " UPDATE Rem2Parties " +
                " SET Remnant = Remnant + (SELECT Rem2PartyMinuses.Quantity FROM Rem2PartyMinuses WHERE Rem2PartyMinusID = OLD.Rem2PartyMinusID) " +
                " WHERE Rem2PartyID = (SELECT Rem2PartyID FROM Rem2PartyMinuses WHERE Rem2PartyMinuses.Rem2PartyMinusID = OLD.Rem2PartyMinusID); " +
                "END; " +

                "CREATE TRIGGER [TG_INSERT_Rem2PartyMinuses] " +
                "AFTER INSERT " +
                "ON [Rem2PartyMinuses] " +
                "BEGIN " +
                " UPDATE Rem2Parties " +
                " SET Remnant = Remnant - (SELECT Rem2PartyMinuses.Quantity FROM Rem2PartyMinuses WHERE Rem2PartyMinusID = NEW.Rem2PartyMinusID) " +
                " WHERE Rem2PartyID = (SELECT Rem2PartyID FROM Rem2PartyMinuses WHERE Rem2PartyMinuses.Rem2PartyMinusID = NEW.Rem2PartyMinusID); " +
                "END;";



            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 12;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update13

        private async Task<bool> Update13(DbConnectionSklad db)
        {
            #region 1. DirWarehouses.Phone

            string SQL = "ALTER TABLE DirWarehouses ADD COLUMN [Phone] TEXT(128);";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 2. DirSmsTemplates.DirSmsTemplateMsg

            //Мало регистраций - исправлю вручную

            //1.Поменять в БД:
            //[[[АппаратИмя]]] => [[[ТоварНаименование]]]
            //[[[НомерПакета]]] => [[[ДокументНомер]]]
            //[[[Товар]]] => [[[ТоварНаименование]]]

            //2. Ваш [[[АппаратИмя]]] готов к выдаче (отремонтирован), стоимость [[[Сумма]]]
            //Ваш [[[ТоварНаименование]]] готов к выдаче (отремонтирован), стоимость [[[Сумма]]]

            //3. Ваш [[[АппаратИмя]]] готов к выдаче (без ремонта)
            //Ваш [[[ТоварНаименование]]] готов к выдаче (без ремонта)

            //4. Забрать пакет № [[[НомерПакета]]] с точки [[[ТочкаОт]]]
            //Забрать пакет № [[[ДокументНомер]]] с точки [[[ТочкаОт]]]

            //5. Ваш товар [[[Товар]]] доставлен на [[[ТочкаНа]]]
            //Ваш товар [[[ТоварНаименование]]] доставлен на [[[ТочкаНа]]]

            //6. Ваш [[[АппаратИмя]]] принят в ремонт, номер ремонта [[[ДокументНомер]]] Мастерская [[[Организация]]] тел: [[[ТочкаТелефон]]]
            //Ваш [[[ТоварНаименование]]] принят в ремонт, номер ремонта [[[ДокументНомер]]] Мастерская [[[Организация]]] тел: [[[ТочкаТелефон]]]

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 13;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update14: ПФ "Б/У"

        private async Task<bool> Update14(DbConnectionSklad db)
        {
            #region 1. ListObjectFieldNames: 2-а поля

            string SQL =
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)values(375, 'PassportSeries', 'Документ.ПаспортСерия', 'Документ.ПаспортСерія'); " +
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)values(376, 'PassportNumber', 'Документ.ПаспортНомер', 'Документ.ПаспортНомер'); " +
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)values(377, 'DocSecondHandPurchID', 'Документ.БУ.НомерВнутренний', 'Документ.БЖ.НомерВнутрішній'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. ListObjectFields: копируем поля из Серв.Центра

            SQL =
                //Переносим всё кроме поля "DocServicePurchID"
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID) " +
                "SELECT 65, lof.ListObjectFieldHeaderShow, lof.ListObjectFieldTabShow, lof.ListObjectFieldFooterShow, lof.ListObjectFieldNameID " +
                "FROM ListObjectFields lof, ListObjectFieldNames lofn " +
                "WHERE " +
                " lof.ListObjectID = 40 and " +
                " lof.ListObjectFieldNameID = lofn.ListObjectFieldNameID and " +
                " lofn.ListObjectFieldNameID <> 342; " +

                //DocSecondHandPurchID
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(65, 1, 0, 0, 377); " +
                //PassportSeries
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(65, 1, 0, 0, 375); " +
                //PassportNumber
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(65, 1, 0, 0, 376); " + 

                //370.SumTotal_InWords
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(65, 1, 0, 0, 370); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            //!!! Внимание !!!
            //Создать ПФ - вручную!!!




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 14;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update15: ПФ "Б/У"

        private async Task<bool> Update15(DbConnectionSklad db)
        {

            //66 - DocSecondHandRetails
            //67 - DocSecondHandRetailReturns
            //68 - DocSecondHandRetailActWriteOffs


            #region 1. ListObjectFieldNames: 2-а поля

            string SQL =
                //66 - 378
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)values(378, 'DocSecondHandRetailID', 'Документ.БУРозница.НомерВнутренний', 'Документ.БЖРоздріб.НомерВнутрішній'); " +
                //67 - 379;
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)values(379, 'DocSecondHandRetailReturnID', 'Документ.БУРозницаВозврат.НомерВнутренний', 'Документ.БЖРоздрібПовернення.НомерВнутрішній'); " +
                //68 - 380;
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)values(380, 'DocSecondHandRetailActWriteOffID', 'Документ.БУРозницаСписание.НомерВнутренний', 'Документ.БЖРоздрібСписання.НомерВнутрішній'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. ListObjectFields: копируем поля из Серв.Центра

            SQL =
                //66
                //Переносим всё кроме поля "DocServicePurchID"
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID) " +
                "SELECT 66, lof.ListObjectFieldHeaderShow, lof.ListObjectFieldTabShow, lof.ListObjectFieldFooterShow, lof.ListObjectFieldNameID " +
                "FROM ListObjectFields lof, ListObjectFieldNames lofn " +
                "WHERE " +
                " lof.ListObjectID = 40 and " +
                " lof.ListObjectFieldNameID = lofn.ListObjectFieldNameID and " +
                " lofn.ListObjectFieldNameID <> 342; " +
                //DocSecondHandPurchID
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(66, 1, 0, 0, 378); " +
                //370.SumTotal_InWords
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(66, 1, 0, 0, 370); " +


                //67
                //Переносим всё кроме поля "DocServicePurchID"
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID) " +
                "SELECT 67, lof.ListObjectFieldHeaderShow, lof.ListObjectFieldTabShow, lof.ListObjectFieldFooterShow, lof.ListObjectFieldNameID " +
                "FROM ListObjectFields lof, ListObjectFieldNames lofn " +
                "WHERE " +
                " lof.ListObjectID = 40 and " +
                " lof.ListObjectFieldNameID = lofn.ListObjectFieldNameID and " +
                " lofn.ListObjectFieldNameID <> 342; " +
                //DocSecondHandPurchID
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(67, 1, 0, 0, 379); " +
                //370.SumTotal_InWords
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(67, 1, 0, 0, 370); " +


                //68
                //Переносим всё кроме поля "DocServicePurchID"
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID) " +
                "SELECT 68, lof.ListObjectFieldHeaderShow, lof.ListObjectFieldTabShow, lof.ListObjectFieldFooterShow, lof.ListObjectFieldNameID " +
                "FROM ListObjectFields lof, ListObjectFieldNames lofn " +
                "WHERE " +
                " lof.ListObjectID = 40 and " +
                " lof.ListObjectFieldNameID = lofn.ListObjectFieldNameID and " +
                " lofn.ListObjectFieldNameID <> 342; " +
                //DocSecondHandPurchID
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(68, 1, 0, 0, 380); " +
                //370.SumTotal_InWords
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(68, 1, 0, 0, 370); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            //!!! Внимание !!!
            //Создать ПФ - вручную!!!
            //2-е ПФ
            //Сергею заменить ПФ на новую!




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 15;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update16: ПФ "Б/У"

        private async Task<bool> Update16(DbConnectionSklad db)
        {

            //66 - DocSecondHandRetails
            //67 - DocSecondHandRetailReturns
            //68 - DocSecondHandRetailActWriteOffs


            #region 1. ListObjectFieldNames: 2-а поля

            string SQL =
                //66 - 378
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)values(381, 'Phone', 'Точка.Телефон', 'Точка.Телефон'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. ListObjectFields: копируем поля из Серв.Центра

            SQL =
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(65, 1, 0, 0, 381); " +
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(66, 1, 0, 0, 381); " +
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(67, 1, 0, 0, 381); " +
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(68, 1, 0, 0, 381); ";
                

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 16;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update17: ЗП: DirBonus2ID2, "Б/У"

        private async Task<bool> Update17(DbConnectionSklad db)
        {
            #region 1. ЗП: DirBonus2ID2, "Б/У"

            string SQL =
                //1. Серв.Цент: изменяем существующие записи
                "ALTER TABLE DirEmployees ADD COLUMN [DirBonus2ID] INTEGER CONSTRAINT [FK_DirEmployees_DirBonus2ID] REFERENCES [DirBonuses]([DirBonusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "UPDATE DirEmployees SET DirBonus2ID=DirBonusID2; " +
                "UPDATE DirEmployees SET DirBonusID2=null; " +

                //3. Документ Зарплата: изменяем существующие записи
                //3.1. 
                "ALTER TABLE DocSalaryTabs ADD COLUMN [DirBonus2ID] INTEGER CONSTRAINT [FK_DocSalaryTabs_DirBonus2ID] REFERENCES [DirBonuses]([DirBonusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "UPDATE DocSalaryTabs SET DirBonus2ID=DirBonusID2; " +
                "UPDATE DocSalaryTabs SET DirBonusID2=null; " +
                //3.1. 
                "ALTER TABLE DocSalaryTabs ADD COLUMN [DirBonus2IDSalary] REAL NOT NULL DEFAULT 0; " +
                "UPDATE DocSalaryTabs SET DirBonus2IDSalary=DirBonusID2Salary; " +
                "UPDATE DocSalaryTabs SET DirBonusID2Salary=0; " +

                //2. Б/У: добавляем поля
                //2.1. Ремонт
                "ALTER TABLE DirEmployees ADD COLUMN [SalarySecondHandWorkshopCheck] BOOL DEFAULT 0; " +
                "ALTER TABLE DirEmployees ADD COLUMN [DirBonus3ID] INTEGER CONSTRAINT [FK_DirEmployees_DirBonus3ID] REFERENCES [DirBonuses]([DirBonusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "ALTER TABLE DirEmployees ADD COLUMN [SalaryFixedSecondHandWorkshopOne] REAL DEFAULT 0; " +
                //2.2. Продажа
                "ALTER TABLE DirEmployees ADD COLUMN [DirBonus4ID] INTEGER CONSTRAINT [FK_DirEmployees_DirBonus4ID] REFERENCES [DirBonuses]([DirBonusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "ALTER TABLE DirEmployees ADD COLUMN [SalaryFixedSecondHandRetailOne] REAL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 17;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update18: Права: RightDocServiceWorkshopsOnlyUsers, Что бы Мастер видел только свои аппараты

        private async Task<bool> Update18(DbConnectionSklad db)
        {
            #region 1. Права: RightDocServiceWorkshopsOnlyUsers

            string SQL =
                //1. Серв.Цент: изменяем существующие записи
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServiceWorkshopsOnlyUsers] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServiceWorkshopsOnlyUsersCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightDocServiceWorkshopsOnlyUsers=0; " +
                "UPDATE DirEmployees SET RightDocServiceWorkshopsOnlyUsersCheck=0; ";
                
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 18;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update19: Права: Admin, Что бы Мастер видел только свои аппараты

        private async Task<bool> Update19(DbConnectionSklad db)
        {
            #region 1. Права: Admin

            string SQL = "ALTER TABLE DirEmployeeWarehouses ADD COLUMN [IsAdmin] BOOL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 1. Права: Admin

            SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightReportSalaries] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightReportSalariesCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightReportSalaries=1, RightReportSalariesCheck=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightReportSalariesWarehouses] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightReportSalariesWarehousesCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightReportSalariesWarehouses=1, RightReportSalariesWarehousesCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 19;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update20: Права: Admin, Что бы Мастер видел только свои аппараты

        private async Task<bool> Update20(DbConnectionSklad db)
        {
            #region 1. Права: Admin

            string SQL =
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercentTrade] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercentService1Tabs] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercentService2Tabs] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercentSecond] REAL NOT NULL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion
            



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 20;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update21: Права: Admin, Что бы Мастер видел только свои аппараты

        private async Task<bool> Update21(DbConnectionSklad db)
        {
            #region 1. Права: Admin

            string SQL = "UPDATE Rem2Parties SET PriceCurrency=PriceVAT; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 21;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update22: DocInventoryTabs

        private async Task<bool> Update22(DbConnectionSklad db)
        {
            #region 1. DocInventoryTabs

            string SQL =
                "PRAGMA foreign_keys = OFF; " +

                "DROP INDEX IDX_DocInventoryTabs_DirNomenID; " +
                "DROP INDEX IDX_DocInventoryTabs_DocInventoryID; " +
                "DROP TABLE DocInventoryTabs; " +

                "CREATE TABLE[DocInventoryTabs]( \n" +
                "  [DocInventoryTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [DocInventoryID] INTEGER NOT NULL CONSTRAINT[FK_DocInventoryTabs_DocInventoryID] REFERENCES[DocInventories]([DocInventoryID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirNomenID] INTEGER NOT NULL CONSTRAINT[FK_DocInventoryTabs_DirNomenID] REFERENCES[DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [RemPartyID] INTEGER CONSTRAINT[FK_DocInventoryTabs_RemPartyID] REFERENCES[RemParties]([RemPartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCharColourID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirCharColourID] REFERENCES[DirCharColours]([DirCharColourID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCharMaterialID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirCharMaterialID] REFERENCES[DirCharMaterials]([DirCharMaterialID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCharNameID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirCharNameID] REFERENCES[DirCharNames]([DirCharNameID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCharSeasonID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirCharSeasonID] REFERENCES[DirCharSeasons]([DirCharSeasonID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCharSexID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirCharSexID] REFERENCES[DirCharSexes]([DirCharSexID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCharSizeID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirCharSizeID] REFERENCES[DirCharSizes]([DirCharSizeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCharStyleID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirCharStyleID] REFERENCES[DirCharStyles]([DirCharStyleID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCharTextureID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirCharTextureID] REFERENCES[DirCharTextures]([DirCharTextureID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [SerialNumber] TEXT(256), \n" +
                "  [Barcode] TEXT(256), \n" +

                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT[FK_DocInventoryTabs_DirCurrencyID] REFERENCES[DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCurrencyRate] REAL NOT NULL, \n" +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL, \n" +

                //Списание
                "  [Quantity_WriteOff] REAL NOT NULL, \n" +
                "  [PriceVAT] REAL NOT NULL, \n" +
                "  [PriceCurrency] REAL NOT NULL, \n" +

                "  [DirContractorID] INTEGER CONSTRAINT[FK_DocInventoryTabs_DirContractorID] REFERENCES[DirContractors]([DirContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

                //Приход
                "  [Quantity_Purch] REAL NOT NULL, \n" +
                "  [PriceRetailVAT] REAL NOT NULL, \n" +
                "  [PriceRetailCurrency] REAL NOT NULL, \n" +
                "  [PriceWholesaleVAT] REAL NOT NULL, \n" +
                "  [PriceWholesaleCurrency] REAL NOT NULL, \n" +
                "  [PriceIMVAT] REAL NOT NULL, \n" +
                "  [PriceIMCurrency] REAL NOT NULL, \n" +
                "  [DirNomenMinimumBalance] INTEGER NOT NULL); " + 

                "CREATE INDEX [IDX_DocInventoryTabs_DirNomenID] ON [DocInventoryTabs] ([DirNomenID]); " +
                "CREATE INDEX [IDX_DocInventoryTabs_DocInventoryID] ON [DocInventoryTabs] ([DocInventoryID]); " +

                "PRAGMA foreign_keys = ON; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 22;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update23: Хоз.расходы (DomesticExpenses)

        private async Task<bool> Update23(DbConnectionSklad db)
        {
            #region 1. DocInventoryTabs

            string SQL =
                //Справочник Хоз.Расчёты
                "CREATE TABLE[DirDomesticExpenses]( \n" +
                "  [DirDomesticExpenseID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [Del] BOOL NOT NULL DEFAULT 0, \n" +
                "  [DirDomesticExpenseName] TEXT NOT NULL DEFAULT 256, \n" +
                "  [DirDomesticExpenseType] INTEGER NOT NULL DEFAULT 0, \n" +
                "  [Sign] INTEGER NOT NULL DEFAULT -1); " +
                "INSERT INTO DirDomesticExpenses (DirDomesticExpenseID, DirDomesticExpenseName, DirDomesticExpenseType, Sign)values(1, 'Уборка', 1, -1); " +
                "INSERT INTO DirDomesticExpenses (DirDomesticExpenseID, DirDomesticExpenseName, DirDomesticExpenseType, Sign)values(2, 'Зарплата', 1, -1); " +
                "INSERT INTO DirDomesticExpenses (DirDomesticExpenseID, DirDomesticExpenseName, DirDomesticExpenseType, Sign)values(3, 'Инкасация', 1, -1); " +

                //Документ Хоз.Расчёты
                //1.
                "CREATE TABLE[DocDomesticExpenses]( " +
                "  [DocDomesticExpenseID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [DocID] INTEGER NOT NULL CONSTRAINT[FK_DocDomesticExpenses_DocID] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT[FK_DocDomesticExpenses_DirWarehouseID] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED); \n" +
                "CREATE INDEX[IDX_DocDomesticExpenses_DirWarehouseID] ON[DocDomesticExpenses] ([DirWarehouseID]); \n" +
                "CREATE INDEX[IDX_DocDomesticExpenses_DocID] ON[DocDomesticExpenses] ([DocID]); \n" +
                //2.
                "CREATE TABLE[DocDomesticExpenseTabs]( \n" +
                "  [DocDomesticExpenseTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [DocDomesticExpenseID] INTEGER NOT NULL CONSTRAINT[FK_DocDomesticExpenseTab_DocDomesticExpensesID] REFERENCES[DocDomesticExpenses]([DocDomesticExpenseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirDomesticExpenseID] INTEGER NOT NULL CONSTRAINT[FK_DocDomesticExpenseTab_DirNomenID] REFERENCES[DirDomesticExpenses]([DirDomesticExpenseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [PriceVAT] REAL NOT NULL, \n" +
                "  [PriceCurrency] REAL NOT NULL, \n" +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT[FK_DocDomesticExpenseTab_DirCurrencyID] REFERENCES[DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1, \n" +
                "[DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1); \n" +
                "CREATE INDEX[IDX_DocDomesticExpenseTabs_DirDomesticExpenseID] ON[DocDomesticExpenseTabs] ([DirDomesticExpenseID]); " +
                "CREATE INDEX[IDX_DocDomesticExpenseTabs_DocDomesticExpenseID] ON[DocDomesticExpenseTabs] ([DocDomesticExpenseID]); " + 

                //ListObjects
                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(69, 2, 'DirDomesticExpenses', 'Справочник Хоз.расходы'); " +
                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(70, 3, 'DocDomesticExpenses', 'Документ Хоз.расходы'); " +

                //DirCashOfficeSumTypes
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(27, 'Хоз.Расходы №', -1); " +
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(28, 'Отмены Хоз.Расходы №', 1); " +

                //DirBankSumTypes
                "INSERT INTO DirBankSumTypes (DirBankSumTypeID, DirBankSumTypeName, Sign)values(25, 'Хоз.Расходы №', -1); " +
                "INSERT INTO DirBankSumTypes (DirBankSumTypeID, DirBankSumTypeName, Sign)values(26, 'Отмены Хоз.Расходы №', 1); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 23;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update24: Права: Хоз.Расчеты

        private async Task<bool> Update24(DbConnectionSklad db)
        {
            #region 1. Права: Хоз.Расчеты

            string SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDirDomesticExpenses] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDirDomesticExpensesCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDirDomesticExpenses=1, RightDirDomesticExpensesCheck=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocDomesticExpenses] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocDomesticExpensesCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocDomesticExpenses=1, RightDocDomesticExpensesCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 24;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update25: Индекс "IDX_DocServicePurches_DirServiceContractorPhone"

        private async Task<bool> Update25(DbConnectionSklad db)
        {
            #region 1. INDEX

            string SQL = "CREATE INDEX [IDX_DocServicePurches_DirServiceContractorPhone] ON [DocServicePurches] ([DirServiceContractorPhone]); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 25;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update26: DirWarehouses

        private async Task<bool> Update26(DbConnectionSklad db)
        {
            #region 1. DirWarehouses

            string SQL =
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercentTradeType] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercentService2TabsType] INTEGER DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 26;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update27: TRIGGER DocServicePurchXTabs

        private async Task<bool> Update27(DbConnectionSklad db)
        {
            #region 1. TRIGGER DocServicePurchXTabs

            string SQL =

            "DROP TRIGGER TG_DELETE_DocServicePurch1Tabs_DocServicePurches; " +


            "CREATE TRIGGER[TG_DELETE_DocServicePurch1Tabs_DocServicePurches] \n" +
            "BEFORE DELETE ON[DocServicePurch1Tabs] \n" +
            "BEGIN \n" +

            "UPDATE DocServicePurches \n" +
            "SET \n" +

            "Sums1 = \n" +
            "( \n" +
            "   ( \n" +
            "      SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) \n" +
            "      FROM DocServicePurch1Tabs \n" +
            "      WHERE DocServicePurchID = \n" +
            "      ( \n" +
            "         SELECT DocServicePurch1Tabs.DocServicePurchID \n" +
            "         FROM DocServicePurch1Tabs \n" +
            "         WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1 \n" +
            "      ) \n" +
            "   ) \n" +
            "   - \n" +
            "   ( \n" +
            "      SELECT DocServicePurch1Tabs.PriceCurrency \n" +
            "      FROM DocServicePurch1Tabs \n" +
            "      WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1 \n" +
            "   ) \n" +
            "), \n" +

            "Sums2 = (SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = (SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1        )  ), \n" +
            "Sums = (SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = (SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1        )   )  +(SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = (SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1        )  )  -IFNULL(DocServicePurches.PrepaymentSum, 0)  WHERE DocServicePurchID = (SELECT DocServicePurch1Tabs.DocServicePurchID FROM DocServicePurch1Tabs WHERE DocServicePurch1TabID = OLD.DocServicePurch1TabID LIMIT 1); \n" +

            "END; " +



            "DROP TRIGGER TG_DELETE_DocServicePurch2Tabs_DocServicePurches; " +

            "CREATE TRIGGER[TG_DELETE_DocServicePurch2Tabs_DocServicePurches] \n" +
            "BEFORE DELETE ON[DocServicePurch2Tabs] \n" +
            "BEGIN \n" +

            "UPDATE DocServicePurches \n" +
            "SET \n" +

            "Sums1 = (SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = (SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1        )   ),    \n" +

            "Sums2 = \n" +
            "( \n" +
            "   ( \n" +
            "      SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) \n" +
            "      FROM DocServicePurch2Tabs \n" +
            "      WHERE DocServicePurchID = \n" +
            "      ( \n" +
            "         SELECT DocServicePurch2Tabs.DocServicePurchID \n" +
            "         FROM DocServicePurch2Tabs \n" +
            "         WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1 \n" +
            "      ) \n" +
            "   ) \n" +
            "   - \n" +
            "   ( \n" +
            "      SELECT DocServicePurch2Tabs.PriceCurrency \n" +
            "      FROM DocServicePurch2Tabs \n" +
            "      WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1 \n" +
            "   ) \n" +
            "), \n" +

            "Sums = (SELECT IFNULL(SUM(DocServicePurch1Tabs.PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurchID = (SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1        )   )  +(SELECT IFNULL(SUM(DocServicePurch2Tabs.PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurchID = (SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1        )  )  -IFNULL(DocServicePurches.PrepaymentSum, 0)  WHERE DocServicePurchID = (SELECT DocServicePurch2Tabs.DocServicePurchID FROM DocServicePurch2Tabs WHERE DocServicePurch2TabID = OLD.DocServicePurch2TabID LIMIT 1); \n" +

            "END; ";




            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            #region 4. Пересчитываем суммы Sums1 и Sums2

            SQL =
                "UPDATE DocServicePurches " +
                "SET " +
                " Sums1 = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch1Tabs WHERE DocServicePurch1Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID); " +

                "UPDATE DocServicePurches " +
                "SET " +
                " Sums2 = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocServicePurch2Tabs WHERE DocServicePurch2Tabs.DocServicePurchID = DocServicePurches.DocServicePurchID); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 27;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update28: TRIGGER DocSecondHandPurchXTabs

        private async Task<bool> Update28(DbConnectionSklad db)
        {
            #region 1. TRIGGER DocSecondHandPurchXTabs

            string SQL =

            "DROP TRIGGER TG_DELETE_DocSecondHandPurch1Tabs_DocSecondHandPurches; " +


            "CREATE TRIGGER[TG_DELETE_DocSecondHandPurch1Tabs_DocSecondHandPurches] \n" +
            "BEFORE DELETE ON[DocSecondHandPurch1Tabs] \n" +
            "BEGIN \n" +

            "UPDATE DocSecondHandPurches \n" +
            "SET \n" +

            "Sums1 = \n" +
            "( \n" +
            "   ( \n" +
            "      SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) \n" +
            "      FROM DocSecondHandPurch1Tabs \n" +
            "      WHERE DocSecondHandPurchID = \n" +
            "      ( \n" +
            "         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID \n" +
            "         FROM DocSecondHandPurch1Tabs \n" +
            "         WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 \n" +
            "      ) \n" +
            "   ) \n" +
            "   - \n" +
            "   ( \n" +
            "      SELECT DocSecondHandPurch1Tabs.PriceCurrency \n" +
            "      FROM DocSecondHandPurch1Tabs \n" +
            "      WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 \n" +
            "   ) \n" +
            "), \n" +

            "Sums2 = (SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID = (SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1        )  ), \n" +
            "Sums = (SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID = (SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1        )   )  +(SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID = (SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1        )  ) WHERE DocSecondHandPurchID = (SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1); \n" +

            "END; " +



            "DROP TRIGGER TG_DELETE_DocSecondHandPurch2Tabs_DocSecondHandPurches; " +

            "CREATE TRIGGER[TG_DELETE_DocSecondHandPurch2Tabs_DocSecondHandPurches] \n" +
            "BEFORE DELETE ON[DocSecondHandPurch2Tabs] \n" +
            "BEGIN \n" +

            "UPDATE DocSecondHandPurches \n" +
            "SET \n" +

            "Sums1 = (SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID = (SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1        )   ),    \n" +

            "Sums2 = \n" +
            "( \n" +
            "   ( \n" +
            "      SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) \n" +
            "      FROM DocSecondHandPurch2Tabs \n" +
            "      WHERE DocSecondHandPurchID = \n" +
            "      ( \n" +
            "         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID \n" +
            "         FROM DocSecondHandPurch2Tabs \n" +
            "         WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 \n" +
            "      ) \n" +
            "   ) \n" +
            "   - \n" +
            "   ( \n" +
            "      SELECT DocSecondHandPurch2Tabs.PriceCurrency \n" +
            "      FROM DocSecondHandPurch2Tabs \n" +
            "      WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 \n" +
            "   ) \n" +
            "), \n" +

            "Sums = (SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID = (SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1        )   )  +(SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID = (SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1        )  ) WHERE DocSecondHandPurchID = (SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1); \n" +

            "END; ";




            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            #region 4. Пересчитываем суммы Sums1 и Sums2

            SQL =
                "UPDATE DocSecondHandPurches " +
                "SET " +
                " Sums1 = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1Tabs.DocSecondHandPurchID = DocSecondHandPurches.DocSecondHandPurchID); " +

                "UPDATE DocSecondHandPurches " +
                "SET " +
                " Sums2 = " +
                " (SELECT IFNULL(SUM(PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2Tabs.DocSecondHandPurchID = DocSecondHandPurches.DocSecondHandPurchID); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 28;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update29: TRIGGER DocSecondHandPurchXTabs

        private async Task<bool> Update29(DbConnectionSklad db)
        {
            string SQL =
                "ALTER TABLE SysSettings ADD COLUMN [SmsAutoShow9] BOOL NOT NULL DEFAULT 0; " +
                "INSERT INTO DirSmsTemplates (DirSmsTemplateID, DirSmsTemplateName, DirSmsTemplateMsg, DirSmsTemplateType, MenuID)values(7, 'Выдан аппарат', 'Ваш [[[ТоварНаименование]]] выдан, гарантия до [[[Гарантия]]]', 4, 1); ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 29;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update30: DirWarehouses

        private async Task<bool> Update30(DbConnectionSklad db)
        {
            #region 1. DirWarehouses

            string SQL =
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercentService1TabsType] INTEGER DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 30;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update31: DirServiceJobNomens.DirServiceJobNomenType

        private async Task<bool> Update31(DbConnectionSklad db)
        {
            #region 1. DirWarehouses

            string SQL =
                //1 - СЦ
                //2 - БУ
                "ALTER TABLE DirServiceJobNomens ADD COLUMN [DirServiceJobNomenType] INTEGER DEFAULT 1; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 31;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update32: DocServicePurch1Tabs (PayDate, DocCashOfficeSumID, DocBankSumID)

        private async Task<bool> Update32(DbConnectionSklad db)
        {
            #region 1. ===

            string SQL =
                //1 - СЦ
                //1.1. DocServicePurch1Tabs
                "ALTER TABLE DocServicePurch1Tabs ADD COLUMN [PayDate] DATETIME; " +
                "ALTER TABLE DocServicePurch1Tabs ADD COLUMN [DocCashOfficeSumID] INTEGER; " +
                "ALTER TABLE DocServicePurch1Tabs ADD COLUMN [DocBankSumID] INTEGER; " +
                //1.1. DocServicePurch1Tabs
                "ALTER TABLE DocServicePurch2Tabs ADD COLUMN [PayDate] DATETIME; " +
                "ALTER TABLE DocServicePurch2Tabs ADD COLUMN [DocCashOfficeSumID] INTEGER; " +
                "ALTER TABLE DocServicePurch2Tabs ADD COLUMN [DocBankSumID] INTEGER; " +

                //2 - БУ
                //1.1. DocServicePurch1Tabs
                "ALTER TABLE DocSecondHandPurch1Tabs ADD COLUMN [PayDate] DATETIME; " +
                //1.1. DocServicePurch1Tabs
                "ALTER TABLE DocSecondHandPurch2Tabs ADD COLUMN [PayDate] DATETIME; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //1.1. DocServicePurch1Tabs.DocCashOfficeSums
            SQL =
                "UPDATE DocServicePurch1Tabs SET " +
                "PayDate=" +
                "(" +
                " SELECT MAX(DocCashOfficeSumDate) " +
                " FROM DocCashOfficeSums c, DocServicePurches p, DocServicePurch1Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocCashOfficeSumDate>x.TabDate and x.DocServicePurch1TabID=DocServicePurch1Tabs.DocServicePurch1TabID" +
                "), " +
                "DocCashOfficeSumID=" +
                "(" +
                " SELECT MAX(c.DocCashOfficeSumID) " +
                " FROM DocCashOfficeSums c, DocServicePurches p, DocServicePurch1Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocCashOfficeSumDate>x.TabDate and x.DocServicePurch1TabID=DocServicePurch1Tabs.DocServicePurch1TabID" +
                "); ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            //1.2. DocServicePurch1Tabs.DocBankSums
            SQL =
                "UPDATE DocServicePurch1Tabs SET " +
                "PayDate=" +
                "(" +
                " SELECT MAX(DocBankSumDate) " +
                " FROM DocBankSums c, DocServicePurches p, DocServicePurch1Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocBankSumDate>x.TabDate and x.DocServicePurch1TabID=DocServicePurch1Tabs.DocServicePurch1TabID" +
                "), " +
                "DocBankSumID=" +
                "(" +
                " SELECT MAX(c.DocBankSumID) " +
                " FROM DocBankSums c, DocServicePurches p, DocServicePurch1Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocBankSumDate>x.TabDate and x.DocServicePurch1TabID=DocServicePurch1Tabs.DocServicePurch1TabID" +
                ") " +
                "WHERE PayDate IS NULL; ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));



            //2.1. DocServicePurch2Tabs.DocCashOfficeSums
            SQL =
                "UPDATE DocServicePurch2Tabs SET " +
                "PayDate=" +
                "(" +
                " SELECT MAX(DocCashOfficeSumDate) " +
                " FROM DocCashOfficeSums c, DocServicePurches p, DocServicePurch2Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocCashOfficeSumDate>x.TabDate and x.DocServicePurch2TabID=DocServicePurch2Tabs.DocServicePurch2TabID" +
                "), " +
                "DocCashOfficeSumID=" +
                "(" +
                " SELECT MAX(c.DocCashOfficeSumID) " +
                " FROM DocCashOfficeSums c, DocServicePurches p, DocServicePurch2Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocCashOfficeSumDate>x.TabDate and x.DocServicePurch2TabID=DocServicePurch2Tabs.DocServicePurch2TabID" +
                "); ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            //2.2. DocServicePurch2Tabs.DocBankSums
            SQL =
                "UPDATE DocServicePurch2Tabs SET " +
                "PayDate=" +
                "(" +
                " SELECT MAX(DocBankSumDate) " +
                " FROM DocBankSums c, DocServicePurches p, DocServicePurch2Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocBankSumDate>x.TabDate and x.DocServicePurch2TabID=DocServicePurch2Tabs.DocServicePurch2TabID" +
                "), " +
                "DocBankSumID=" +
                "(" +
                " SELECT MAX(c.DocBankSumID) " +
                " FROM DocBankSums c, DocServicePurches p, DocServicePurch2Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocBankSumDate>x.TabDate and x.DocServicePurch2TabID=DocServicePurch2Tabs.DocServicePurch2TabID" +
                ") " +
                "WHERE PayDate IS NULL; ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 32;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update33: DocServicePurch1Tabs (PayDate, DocCashOfficeSumID, DocBankSumID)

        private async Task<bool> Update33(DbConnectionSklad db)
        {
            #region 1. ===

            string SQL = "";

            //1.1. DocServicePurch1Tabs.DocCashOfficeSums
            SQL =
                "UPDATE DocServicePurch1Tabs SET " +
                "PayDate=" +
                "(" +
                " SELECT MAX(DocCashOfficeSumDate) " +
                " FROM DocCashOfficeSums c, DocServicePurches p, DocServicePurch1Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocCashOfficeSumDate>x.TabDate and x.DocServicePurch1TabID=DocServicePurch1Tabs.DocServicePurch1TabID" +
                "), " +
                "DocCashOfficeSumID=" +
                "(" +
                " SELECT MAX(c.DocCashOfficeSumID) " +
                " FROM DocCashOfficeSums c, DocServicePurches p, DocServicePurch1Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocCashOfficeSumDate>x.TabDate and x.DocServicePurch1TabID=DocServicePurch1Tabs.DocServicePurch1TabID" +
                "); ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            //1.2. DocServicePurch1Tabs.DocBankSums
            SQL =
                "UPDATE DocServicePurch1Tabs SET " +
                "PayDate=" +
                "(" +
                " SELECT MAX(DocBankSumDate) " +
                " FROM DocBankSums c, DocServicePurches p, DocServicePurch1Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocBankSumDate>x.TabDate and x.DocServicePurch1TabID=DocServicePurch1Tabs.DocServicePurch1TabID" +
                "), " +
                "DocBankSumID=" +
                "(" +
                " SELECT MAX(c.DocBankSumID) " +
                " FROM DocBankSums c, DocServicePurches p, DocServicePurch1Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocBankSumDate>x.TabDate and x.DocServicePurch1TabID=DocServicePurch1Tabs.DocServicePurch1TabID" +
                ") " +
                "WHERE PayDate IS NULL; ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));



            //2.1. DocServicePurch2Tabs.DocCashOfficeSums
            SQL =
                "UPDATE DocServicePurch2Tabs SET " +
                "PayDate=" +
                "(" +
                " SELECT MAX(DocCashOfficeSumDate) " +
                " FROM DocCashOfficeSums c, DocServicePurches p, DocServicePurch2Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocCashOfficeSumDate>x.TabDate and x.DocServicePurch2TabID=DocServicePurch2Tabs.DocServicePurch2TabID" +
                "), " +
                "DocCashOfficeSumID=" +
                "(" +
                " SELECT MAX(c.DocCashOfficeSumID) " +
                " FROM DocCashOfficeSums c, DocServicePurches p, DocServicePurch2Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocCashOfficeSumDate>x.TabDate and x.DocServicePurch2TabID=DocServicePurch2Tabs.DocServicePurch2TabID" +
                "); ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            //2.2. DocServicePurch2Tabs.DocBankSums
            SQL =
                "UPDATE DocServicePurch2Tabs SET " +
                "PayDate=" +
                "(" +
                " SELECT MAX(DocBankSumDate) " +
                " FROM DocBankSums c, DocServicePurches p, DocServicePurch2Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocBankSumDate>x.TabDate and x.DocServicePurch2TabID=DocServicePurch2Tabs.DocServicePurch2TabID" +
                "), " +
                "DocBankSumID=" +
                "(" +
                " SELECT MAX(c.DocBankSumID) " +
                " FROM DocBankSums c, DocServicePurches p, DocServicePurch2Tabs x " +
                " WHERE c.DocID=p.DocID and p.DocServicePurchID=x.DocServicePurchID and c.DocBankSumDate>x.TabDate and x.DocServicePurch2TabID=DocServicePurch2Tabs.DocServicePurch2TabID" +
                ") " +
                "WHERE PayDate IS NULL; ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 33;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update34: DocServicePurch1Tabs (PayDate, DocCashOfficeSumID, DocBankSumID)

        private async Task<bool> Update34(DbConnectionSklad db)
        {
            #region 1. ===

            string SQL = "";

            //1.1. DocServicePurch1Tabs.DocCashOfficeSums
            SQL =
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercent2Second] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercent3Second] REAL NOT NULL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 34;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update35: DocSecondHandMovements and DocSecondHandMovementTabs

        private async Task<bool> Update35(DbConnectionSklad db)
        {
            #region DocSecondHandMovements and DocSecondHandMovementTabs

            string SQL =
                //DocSecondHandMovements
                "CREATE TABLE[DocSecondHandMovements](\n" +
                "  [DocSecondHandMovementID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,\n" +
                "  [DocID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovements_DocID] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED,\n" +
                "  [DirWarehouseIDFrom] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovements_DirWarehouseIDFrom] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED,\n" +
                "  [DirWarehouseIDTo] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovements_DirWarehouseIDTo] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED,\n" +
                "  [Reserve] BOOL NOT NULL DEFAULT 1,\n" +
                "  [DirMovementDescriptionID] INTEGER CONSTRAINT[FK_DocSecondHandMovements_DirMovementDescriptionID] REFERENCES[DirMovementDescriptions]([DirMovementDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
                "  [DirEmployeeIDCourier] INTEGER CONSTRAINT[FK_DocSecondHandMovements_DirEmployeeID] REFERENCES[DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
                "  [DirMovementStatusID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovements_DirMovementStatusID] REFERENCES[DirMovementStatuses]([DirMovementStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED DEFAULT 0 \n" +
                ");" +

                "CREATE INDEX[IDX_DocSecondHandMovements_DirEmployeeID] ON[DocSecondHandMovements] ([DirEmployeeIDCourier]);" +
                "CREATE INDEX[IDX_DocSecondHandMovements_DirEmployeeIDCourier] ON[DocSecondHandMovements] ([DirEmployeeIDCourier]);" +
                "CREATE INDEX[IDX_DocSecondHandMovements_DirMovementStatusID] ON[DocSecondHandMovements] ([DirMovementStatusID]);" +
                "CREATE INDEX[IDX_DocSecondHandMovements_DocID] ON[DocSecondHandMovements] ([DocID]); " +

                //DocSecondHandMovementTabs
                "CREATE TABLE[DocSecondHandMovementTabs](\n" +
                "  [DocSecondHandMovementTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [DocSecondHandMovementID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovementTabs_DocSecondHandMovementID] REFERENCES[DocSecondHandMovements]([DocSecondHandMovementID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE, \n" +
                "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRetailTab_DirServiceNomenID] REFERENCES[DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [Rem2PartyID] INTEGER CONSTRAINT[FK_DocSecondHandRetailTabs_Rem2PartyID] REFERENCES[Rem2Parties]([Rem2PartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [Quantity] REAL NOT NULL, \n" +
                "  [PriceVAT] REAL NOT NULL, \n" +
                "  [PriceCurrency] REAL NOT NULL, \n" +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovementTabs_DirCurrencyID] REFERENCES[DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCurrencyRate] REAL NOT NULL, \n" +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL, \n" +
                "  [PriceRetailVAT] REAL NOT NULL, \n" +
                "  [PriceRetailCurrency] REAL NOT NULL, \n" +
                "  [PriceWholesaleVAT] REAL NOT NULL, \n" +
                "  [PriceWholesaleCurrency] REAL NOT NULL, \n" +
                "  [PriceIMVAT] REAL NOT NULL, \n" +
                "  [PriceIMCurrency] REAL NOT NULL, \n" +
                "  [DirReturnTypeID] INTEGER CONSTRAINT[FK_DocSecondHandMovementTabs_DirReturnTypeID] REFERENCES[DirReturnTypes]([DirReturnTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirDescriptionID] INTEGER CONSTRAINT[FK_DocSecondHandMovementTabs_DirDescriptionID] REFERENCES[DirDescriptions]([DirDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED \n" +
                ");" +

                "CREATE INDEX[IDX_DocSecondHandMovementTabs_DirServiceNomenID] ON[DocSecondHandMovementTabs] ([DirServiceNomenID]);" +
                "CREATE INDEX[IDX_DocSecondHandMovementTabs_DocSecondHandMovementID] ON[DocSecondHandMovementTabs] ([DocSecondHandMovementID]);" +
                "CREATE INDEX[IDX_DocSecondHandMovementTabs_Rem2PartyID] ON[DocSecondHandMovementTabs] ([Rem2PartyID]);" +


                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(71, 3, 'DocSecondHandMovements', 'Документ Перемещение Б/У'); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Права: RightDocSecondHandMovements

            SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandMovements] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandMovementsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandMovements=1, RightDocSecondHandMovementsCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Поле: Rem2Parties.DocIDFirst

            SQL =
                "ALTER TABLE Rem2Parties ADD COLUMN [DocIDFirst] INTEGER CONSTRAINT [FK_Rem2Parties_DicIDFirst] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "UPDATE Rem2Parties SET DocIDFirst=Rem2Parties.DocID; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            //!!! НЕ НУЖНО !!!
            #region Права: RightDocMovementsLogisticsNew и RightDocSecondHandMovementsLogisticsNew

            /*
            SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocMovementsLogisticsNew] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocMovementsLogisticsNewCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocMovementsLogisticsNew=1, RightDocMovementsLogisticsNewCheck=1 WHERE DirEmployeeID=1; " +

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandMovementsLogisticsNew] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandMovementsLogisticsNewCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandMovementsLogisticsNew=1, RightDocSecondHandMovementsLogisticsNewCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));
            */

            #endregion


            #region DirLogisticLogTypes, DirLogisticStatuses и LogLogistics

            SQL =
                /*
                //DirLogisticLogTypes
                "CREATE TABLE[DirLogisticLogTypes]( \n" +
                "  [DirLogisticLogTypeID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [Del] BOOL NOT NULL DEFAULT 0, \n" +
                "  [DirLogisticLogTypeName] TEXT(256) NOT NULL \n" +
                "); " +

                "INSERT INTO DirLogisticLogTypes " +
                "(DirLogisticLogTypeID, Del, DirLogisticLogTypeName) " +
                "SELECT DirMovementLogTypeID, Del, DirMovementLogTypeName " +
                "FROM DirMovementLogTypes; " +


                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(72, 3, 'DirLogisticLogTypes', 'Справочник Логистика Тип Лога'); " +


                //DirLogisticStatuses
                "CREATE TABLE[DirLogisticStatuses]( \n" +
                "  [DirLogisticStatusID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [DirLogisticStatusName] TEXT(256) NOT NULL, \n" +
                "  [SortNum] INTEGER NOT NULL \n" +
                "); " +

                "CREATE UNIQUE INDEX[UDX_DirLogisticStatuses_SortNum] ON[DirLogisticStatuses]([SortNum]);" +

                "INSERT INTO DirLogisticStatuses " +
                "(DirLogisticStatusID, DirLogisticStatusName, SortNum) " +
                "SELECT DirMovementStatusID, DirMovementStatusName, SortNum " +
                "FROM DirMovementStatuses; " +


                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(73, 3, 'DirLogisticStatuses', 'Справочник Логистика Статус'); " +
                */

                //LogLogistics
                "CREATE TABLE[LogLogistics]( \n" +
                "  [LogLogisticID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [DocLogisticID] INTEGER NOT NULL,\n" +
                "  [DirMovementLogTypeID] INTEGER NOT NULL CONSTRAINT[FK_LogLogistics_DirMovementLogTypeID] REFERENCES[DirMovementLogTypes]([DirMovementLogTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
                "  [DirEmployeeID] INTEGER NOT NULL CONSTRAINT[FK_LogLogistics_DirEmployees] REFERENCES[DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
                "  [DirMovementStatusID] INTEGER CONSTRAINT[FK_LogLogistics_DirMovementStatusID] REFERENCES[DirMovementStatuses]([DirMovementStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
                "  [LogLogisticDate] DATETIME DEFAULT(datetime(CURRENT_TIMESTAMP, 'localtime')),\n" +
                "  [Msg] TEXT(1024) \n" +
                "); " +

                "CREATE INDEX[IDX_LogLogistics_DirMovementLogTypeID] ON[LogLogistics] ([DirMovementLogTypeID]); " +
                "CREATE INDEX[IDX_LogLogistics_DirMovementStatusID] ON[LogLogistics] ([DirMovementStatusID]); " +
                "CREATE INDEX[IDX_LogLogistics_DocLogisticID] ON[LogLogistics] ([DocLogisticID]); " +
                "CREATE INDEX[IDX_LogLogistics_DirEmployeeID] ON[LogLogistics] ([DirEmployeeID]); " +


                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(72, 3, 'LogLogistics', 'Лог Логистика'); ";


                await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion






            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 35;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update36: DocSecondHandMovements and DocSecondHandMovementTabs

        private async Task<bool> Update36(DbConnectionSklad db)
        {
            #region 1. Права: DocServicePurches

            string SQL =
                "ALTER TABLE DocServicePurches ADD COLUMN [InSecondHand] BOOL DEFAULT 0; " +

                "ALTER TABLE DocSecondHandPurches ADD COLUMN [FromService] BOOL DEFAULT 0; " +
                "ALTER TABLE DocSecondHandPurches ADD COLUMN [DocIDService] INTEGER CONSTRAINT [FK_DocSecondHandPurches_DocIDService] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "ALTER TABLE DocSecondHandPurches ADD COLUMN [Sums1Service] REAL DEFAULT 0; " +
                "ALTER TABLE DocSecondHandPurches ADD COLUMN [Sums2Service] REAL DEFAULT 0; " +

                "ALTER TABLE DocSecondHandPurch1Tabs ADD COLUMN [FromService] BOOL DEFAULT 0; " +
                "ALTER TABLE DocSecondHandPurch2Tabs ADD COLUMN [FromService] BOOL DEFAULT 0; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            
            #region 2. Удаляем старые тригерры

            SQL =
                "DROP TRIGGER TG_UPDATE_DocSecondHandPurch1Tabs_DocSecondHandPurches; " +
                "DROP TRIGGER TG_INSERT_DocSecondHandPurch1Tabs_DocSecondHandPurches; " +
                "DROP TRIGGER TG_DELETE_DocSecondHandPurch1Tabs_DocSecondHandPurches; " +

                "DROP TRIGGER TG_INSERT_DocSecondHandPurch2Tabs_DocSecondHandPurches; " +
                "DROP TRIGGER TG_UPDATE_DocSecondHandPurch2Tabs_DocSecondHandPurches; " +
                "DROP TRIGGER TG_DELETE_DocSecondHandPurch2Tabs_DocSecondHandPurches; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));
            
            #endregion

            #region 2.1. DocSecondHandPurch1Tabs

            SQL =

                "CREATE TRIGGER [TG_UPDATE_DocSecondHandPurch1Tabs_DocSecondHandPurches] " +
                "AFTER UPDATE " +
                "ON [DocSecondHandPurch1Tabs] " +
                "BEGIN  UPDATE DocSecondHandPurches  SET  Sums1 = Sums1Service +  (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )   ),  Sums2 = Sums2Service +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )  ),  Sums = Sums1Service + Sums2Service +  (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )   )     +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )  )  WHERE   DocSecondHandPurchID = (SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1); END; " +


                "CREATE TRIGGER [TG_INSERT_DocSecondHandPurch1Tabs_DocSecondHandPurches] " +
                "AFTER INSERT " +
                "ON [DocSecondHandPurch1Tabs] " +
                "BEGIN  UPDATE DocSecondHandPurches  SET  Sums1 = Sums1Service +  (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )   ),   Sums2 = Sums2Service + (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )  ),  Sums = Sums1Service + Sums2Service +  (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )   )  +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1        )  )  WHERE   DocSecondHandPurchID = (SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurch1TabID = NEW.DocSecondHandPurch1TabID LIMIT 1); END; " +


                "CREATE TRIGGER [TG_DELETE_DocSecondHandPurch1Tabs_DocSecondHandPurches] " +
                "BEFORE DELETE " +
                "ON [DocSecondHandPurch1Tabs] " +

                "BEGIN " +


                " UPDATE DocSecondHandPurches " +
                " SET " +

                "  Sums1 = Sums1Service + Sums1 - " +
                "  ( " +
                "    SELECT IFNULL(DocSecondHandPurch1Tabs.PriceCurrency, 0) " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1  " +
                "  ),  " +

                "  Sums2 = Sums2Service + " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) " +
                "   FROM DocSecondHandPurch2Tabs " +
                "   WHERE DocSecondHandPurchID = " +
                "   ( " +
                "    SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 " +
                "   ) " +
                "  ), " +

                "  Sums = " +
                "  Sums1Service + Sums1 - " +
                "  ( " +
                "    SELECT IFNULL(DocSecondHandPurch1Tabs.PriceCurrency, 0) " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 " +
                "  ) " +
                "  + " +
                "  ( " +
                "   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) " +
                "   FROM DocSecondHandPurch2Tabs " +
                "   WHERE DocSecondHandPurchID =  " +
                "   ( " +
                "    SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID  " +
                "    FROM DocSecondHandPurch1Tabs  " +
                "    WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 " +
                "   ) " +
                "  ) " +

                " WHERE    " +
                "  DocSecondHandPurchID =  " +
                "  ( " +
                "   SELECT DocSecondHandPurch1Tabs.DocSecondHandPurchID  " +
                "   FROM DocSecondHandPurch1Tabs  " +
                "   WHERE DocSecondHandPurch1TabID = OLD.DocSecondHandPurch1TabID LIMIT 1 " +
                "  );  " +

                "END; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 2.2. DocSecondHandPurch2Tabs
            
            SQL =
               
                "CREATE TRIGGER [TG_INSERT_DocSecondHandPurch2Tabs_DocSecondHandPurches] " +
                "AFTER INSERT " +
                "ON [DocSecondHandPurch2Tabs] " +
                "BEGIN  UPDATE DocSecondHandPurches  SET  Sums1 = Sums1Service +  (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )   ),  Sums2 = Sums2Service + (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )  ),  Sums = Sums1Service + Sums2Service +  (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )   )  +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )  )  WHERE   DocSecondHandPurchID = (SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1); END; " +


                "CREATE TRIGGER [TG_UPDATE_DocSecondHandPurch2Tabs_DocSecondHandPurches] " +
                "AFTER UPDATE " +
                "ON [DocSecondHandPurch2Tabs] " +
                "BEGIN  UPDATE DocSecondHandPurches  SET  Sums1 = Sums1Service + (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )   ),   Sums2 = Sums2Service + (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )  ),  Sums = Sums1Service + Sums2Service +  (    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) FROM DocSecondHandPurch1Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )   )   +  (   SELECT IFNULL(SUM(DocSecondHandPurch2Tabs.PriceCurrency), 0) FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurchID =        (         SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1        )  )  WHERE   DocSecondHandPurchID = (SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID FROM DocSecondHandPurch2Tabs WHERE DocSecondHandPurch2TabID = NEW.DocSecondHandPurch2TabID LIMIT 1); END; " +


                "CREATE TRIGGER [TG_DELETE_DocSecondHandPurch2Tabs_DocSecondHandPurches] " +
                "BEFORE DELETE " +
                "ON [DocSecondHandPurch2Tabs] " +

                "BEGIN " +

                " UPDATE DocSecondHandPurches " +
                " SET " +

                "  Sums1 = Sums1Service + " +
                "   ( " +
                "    SELECT IFNULL(SUM([DocSecondHandPurch1Tabs].PriceCurrency), 0) " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurchID = " +
                "    ( " +
                "     SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID " +
                "     FROM DocSecondHandPurch2Tabs " +
                "     WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "    ) " +
                "   ), " +

                "  Sums2 = " +
                "   Sums2Service + Sums2 - " +
                "   ( " +
                "     SELECT IFNULL(DocSecondHandPurch2Tabs.PriceCurrency, 0) " +
                "     FROM DocSecondHandPurch2Tabs " +
                "     WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "   ), " +

                "  Sums = " +
                " Sums1Service + Sums2Service + " + 
                "   ( " +
                "    SELECT IFNULL(SUM(DocSecondHandPurch1Tabs.PriceCurrency), 0) " +
                "    FROM DocSecondHandPurch1Tabs " +
                "    WHERE DocSecondHandPurchID = " +
                "    ( " +
                "     SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID " +
                "     FROM DocSecondHandPurch2Tabs " +
                "     WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "    ) " +
                "   ) " +
                "   + " +
                "   Sums2 - " +
                "   ( " +
                "     SELECT IFNULL(DocSecondHandPurch2Tabs.PriceCurrency, 0) " +
                "     FROM DocSecondHandPurch2Tabs " +
                "     WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "   ) " +


                "  WHERE " +
                "   DocSecondHandPurchID = " +
                "   ( " +
                "    SELECT DocSecondHandPurch2Tabs.DocSecondHandPurchID " +
                "    FROM DocSecondHandPurch2Tabs " +
                "    WHERE DocSecondHandPurch2TabID = OLD.DocSecondHandPurch2TabID LIMIT 1 " +
                "   ); " +

                " END;";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 36;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update37: IDX_DocServicePurches_DirServiceContractorID_Phone

        private async Task<bool> Update37(DbConnectionSklad db)
        {
            #region 1. IDX_DocServicePurches_DirServiceContractorID_Phone

            string SQL = "CREATE INDEX[IDX_DocServicePurches_DirServiceContractorID_Phone] ON[DocServicePurches]([DirServiceContractorID], [DirServiceNomenID]);";
                
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 37;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update38: SysSettings.SmsAutoShowServiceFromArchiv

        private async Task<bool> Update38(DbConnectionSklad db)
        {
            #region 1. IDX_DocServicePurches_DirServiceContractorID_Phone

            string SQL = "ALTER TABLE SysSettings ADD COLUMN [SmsAutoShowServiceFromArchiv] BOOL NOT NULL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 38;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update39: SysSettings.TabIdenty

        private async Task<bool> Update39(DbConnectionSklad db)
        {
            #region 1. SysSettings.TabIdenty

            string SQL = "ALTER TABLE SysSettings ADD COLUMN [TabIdenty] BOOL NOT NULL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 39;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update40: API10s

        private async Task<bool> Update40(DbConnectionSklad db)
        {
            #region 1. API10s

            string SQL =
                "CREATE TABLE[API10s]( \n" +

                " [API10ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                " [Del] BOOL NOT NULL DEFAULT 0, \n" +
                " [API10Key] TEXT(512) NOT NULL, \n" +

                //Export
                " [ExportDirNomens] BOOL NOT NULL DEFAULT 0, \n" + //DirNomenName
                " [ExportDirNomen_DirNomenNameFull] BOOL NOT NULL DEFAULT 0, \n" +
                " [ExportDirNomen_Description] BOOL NOT NULL DEFAULT 0, \n" +
                " [ExportDirNomen_DescriptionFull] BOOL NOT NULL DEFAULT 0, \n" +
                " [ExportDirNomen_ImageLink] BOOL NOT NULL DEFAULT 0, \n" +

                " [ExportDirChars] BOOL NOT NULL DEFAULT 0, \n" + //DirContractorName
                " [ExportDirContractors] BOOL NOT NULL DEFAULT 0, \n" + //DirContractorName
                " [ExportRemRemnants] BOOL NOT NULL DEFAULT 0, \n" +    //Остатки 
                " [ExportRemParties] BOOL NOT NULL DEFAULT 0, \n" +     //Партии-1
                " [ExportRem2Parties] BOOL NOT NULL DEFAULT 0, \n" +    //Партии-2

                //Import
                " [ImportDirNomens] BOOL NOT NULL DEFAULT 0, \n" +
                " [ImportDirContractors] BOOL NOT NULL DEFAULT 0, \n" +

                " [ImportDocOrderInts] BOOL NOT NULL DEFAULT 0 \n" +
                //" [ImportDocSales] BOOL NOT NULL DEFAULT 0, \n" +
                //" [ImportDocPurch] BOOL NOT NULL DEFAULT 0 \n" +

                ");";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Права: RightAPI10s

            SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightAPI10s] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightAPI10sCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightAPI10s=1, RightAPI10sCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region ListObjects

            SQL = "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(73, 1, 'API10s', 'API 1.0'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region DirOrderIntTypes

            SQL = "INSERT INTO DirOrderIntTypes (DirOrderIntTypeID, DirOrderIntTypeName)values(4, 'Интернет-Магазин');";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region DocOrderInts - переделать таблицу. ЕЩЁ НЕ СДЕЛАЛ !!!

            //DirNomenCategoryID => int?

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 40;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update41: DirServiceNomens.TabIdenty and ReCreate Trigger ON DirServiceNomens

        private async Task<bool> Update41(DbConnectionSklad db)
        {
            #region 1. DirServiceNomens.TabIdenty

            string SQL =
                //1. Замена дисплейного модуля (экран+сенсор в сборе)
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults1Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults1Price] REAL NOT NULL DEFAULT 0; " +
                //2. Замена сенсорного стекла (тачскрина)
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults2Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults2Price] REAL NOT NULL DEFAULT 0; " +
                //3. Замена разъема зарядки
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults3Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults3Price] REAL NOT NULL DEFAULT 0; " +
                //4. Замена разъема sim-карты
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults4Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults4Price] REAL NOT NULL DEFAULT 0; " +
                //5. Обновление ПО (прошивка)
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults5Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults5Price] REAL NOT NULL DEFAULT 0; " +
                //6. Замена динамика (слуховой)
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults6Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults6Price] REAL NOT NULL DEFAULT 0; " +
                //7. Замена микрофона
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults7Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults7Price] REAL NOT NULL DEFAULT 0; " +
                //8. Замена динамика (звонок)
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults8Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults8Price] REAL NOT NULL DEFAULT 0; " +
                //9. Восстановление после попадания жидкости
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults9Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults9Price] REAL NOT NULL DEFAULT 0; " +
                //10. Восстановление цепи питания
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults10Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults10Price] REAL NOT NULL DEFAULT 0; " +
                //11. Ремонт материнской платы
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults11Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults11Price] REAL NOT NULL DEFAULT 0; " +
                //12. Резерв-5
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults12Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults12Price] REAL NOT NULL DEFAULT 0; " +
                //13. Резерв-6
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults13Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults13Price] REAL NOT NULL DEFAULT 0; " +
                //14. Резерв-7
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults14Check] BOOL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirServiceNomens ADD COLUMN [Faults14Price] REAL NOT NULL DEFAULT 0; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. ReCreate Trigger: TG_INSERT_DirServiceNomens_DateUpdate and TG_UPDATE_DirServiceNomens_DateUpdate

            SQL =
                //1. DROP
                "DROP Trigger TG_INSERT_DirServiceNomens_DateUpdate; " +
                "DROP Trigger TG_UPDATE_DirServiceNomens_DateUpdate; " +

                //2. Create
                "CREATE TRIGGER[TG_INSERT_DirServiceNomens_DateUpdate] AFTER INSERT ON[DirServiceNomens] " + 
                "BEGIN " + 
                " UPDATE " +
                "  [DirServiceNomens] " +
                " SET " +
                "  [DateTimeUpdate] = (datetime(CURRENT_TIMESTAMP, 'localtime')) " +
                " WHERE " + 
                "  [DirServiceNomenID] = [new].[DirServiceNomenID]; " + 
                "END; " +

                "CREATE TRIGGER[TG_UPDATE_DirServiceNomens_DateUpdate] AFTER UPDATE ON[DirServiceNomens] " +
                "BEGIN " +
                " UPDATE " +
                "  [DirServiceNomens] " +
                " SET " +
                "  [DateTimeUpdate] = (datetime(CURRENT_TIMESTAMP, 'localtime')) " +
                " WHERE " +
                "  [DirServiceNomenID] = [new].[DirServiceNomenID]; " +
                "END;";
                


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 41;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update42: DirNomens.ImportToIM and DirServiceNomens.ImportToIM

        private async Task<bool> Update42(DbConnectionSklad db)
        {
            #region 1. DirNomens.TabIdenty and DirServiceNomens.ImportToIM

            string SQL =
                //1. 
                "ALTER TABLE DirNomens ADD COLUMN [ImportToIM] BOOL NOT NULL DEFAULT 1; " +
                //2. 
                "ALTER TABLE DirServiceNomens ADD COLUMN [ImportToIM] BOOL NOT NULL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. DirNomens.[CREATE INDEX]

            SQL =
                //1. 
                "CREATE INDEX [IDX_DirNomens_ImportToIM] ON [DirNomens] ([ImportToIM]); " +
                "CREATE INDEX [IDX_DirNomens_Sub_ImportToIM] ON [DirNomens] ([Sub], [ImportToIM]); " +
                "CREATE INDEX [IDX_DirNomens_Del_Sub] ON [DirNomens] ([Del], [Sub]); " + 
                "CREATE INDEX [IDX_DirNomens_Del_Sub_ImportToIM] ON [DirNomens] ([Del], [Sub], [ImportToIM]); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 3. DirNomens.[CREATE INDEX]

            SQL =
                //1. 
                "ALTER TABLE API10s ADD COLUMN [ExportDirServiceNomens] BOOL NOT NULL DEFAULT 0 ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 42;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update43: ККМ

        private async Task<bool> Update43(DbConnectionSklad db)
        {
            #region 1. SysSettings.ККМ

            string SQL =

                // Используется ККМСервер
                "ALTER TABLE SysSettings ADD COLUMN [KKMSActive] BOOL NOT NULL DEFAULT 0; " +

                // UrlServer: http://localhost:5893/
                "ALTER TABLE SysSettings ADD COLUMN [KKMSUrlServer] TEXT(64); " +

                // User: User
                "ALTER TABLE SysSettings ADD COLUMN [KKMSUser] TEXT(64); " +

                // Password: 30
                "ALTER TABLE SysSettings ADD COLUMN [KKMSPassword] TEXT(64); " +

                // Номер устройства. Если 0 то первое не блокированное на сервере
                "ALTER TABLE SysSettings ADD COLUMN [KKMSNumDevice] INTEGER DEFAULT 1; " +

                // ИНН продавца тег ОФД 1203
                "ALTER TABLE SysSettings ADD COLUMN [KKMSCashierVATIN] TEXT(32); " +

                // Система налогообложения (СНО) применяемая для чека
                // Если не указанно - система СНО настроенная в ККМ по умолчанию
                // 0: Общая ОСН
                // 1: Упрощенная УСН (Доход)
                // 2: Упрощенная УСН (Доход минус Расход)
                // 3: Единый налог на вмененный доход ЕНВД
                // 4: Единый сельскохозяйственный налог ЕСН
                // 5: Патентная система налогообложения
                // Комбинация разных СНО не возможна
                // Надо указывать если ККМ настроена на несколько систем СНО
                "ALTER TABLE SysSettings ADD COLUMN [KKMSTaxVariant] INTEGER DEFAULT 0; " +

                // НДС в процентах или ТЕГ НДС: 0 (НДС 0%), 10 (НДС 10%), 18 (НДС 18%), -1 (НДС не облагается), 118 (НДС 18/118), 110 (НДС 10/110)
                "ALTER TABLE SysSettings ADD COLUMN [KKMSTax] INTEGER DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            //Пока не будет использоваться (на будущее)
            #region 2. DirNomens.ККМ

            SQL =

                // НДС в процентах или ТЕГ НДС: 0 (НДС 0%), 10 (НДС 10%), 18 (НДС 18%), -1 (НДС не облагается), 118 (НДС 18/118), 110 (НДС 10/110)
                "ALTER TABLE DirNomens ADD COLUMN [KKMSTax] INTEGER DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 3. Docs.[EMail + Phone]

            SQL =

                "ALTER TABLE Docs ADD COLUMN [KKMSCheckNumber] INTEGER; " +
                "ALTER TABLE Docs ADD COLUMN [KKMSIdCommand] TEXT(64); " +

                // НДС в процентах или ТЕГ НДС: 0 (НДС 0%), 10 (НДС 10%), 18 (НДС 18%), -1 (НДС не облагается), 118 (НДС 18/118), 110 (НДС 10/110)
                "ALTER TABLE Docs ADD COLUMN [KKMSEMail] TEXT(32); " +
                "ALTER TABLE Docs ADD COLUMN [KKMSPhone] TEXT(32); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 4. DirEmployees.RightKKMXXX

            SQL =

                "ALTER TABLE DirEmployees ADD COLUMN [RightKKM0] BOOL DEFAULT 1; " +
                //"UPDATE DirEmployees SET RightKKM0=1 WHERE DirEmployeeID=1; " +

                //X-отчет
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMXReport] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMXReportCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMXReport=1, RightKKMXReportCheck=1 WHERE DirEmployeeID=1; " +
                //Открытие смены
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMOpen] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMOpenCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMOpen=1, RightKKMOpenCheck=1 WHERE DirEmployeeID=1; " +
                //Инкассация денег из кассы
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMEncashment] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMEncashmentCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMEncashment=1, RightKKMEncashmentCheck=1 WHERE DirEmployeeID=1; " +
                //Внесение денег в кассу
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMAdding] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMAddingCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMAdding=1, RightKKMAddingCheck=1 WHERE DirEmployeeID=1; " +
                //Закрытие смены
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMClose] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMCloseCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMClose=1, RightKKMCloseCheck=1 WHERE DirEmployeeID=1; " +
                //Печать состояния расчетов и связи с ОФД
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMPrintOFD] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMPrintOFDCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMPrintOFD=1, RightKKMPrintOFDCheck=1 WHERE DirEmployeeID=1; " +
                //Получить данные последнего чека из ФН.
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMCheckLastFN] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMCheckLastFNCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMCheckLastFN=1, RightKKMCheckLastFNCheck=1 WHERE DirEmployeeID=1; " +
                //Получить текущее состояние ККТ
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMState] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMStateCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMState=1, RightKKMStateCheck=1 WHERE DirEmployeeID=1; " +
                //Получение списка ККМ
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMList] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightKKMListCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightKKMList=1, RightKKMListCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region 5. DirWarehouses.KKMSActive

            SQL =
                //Используется ли на точка ККМ
                "ALTER TABLE DirWarehouses ADD COLUMN [KKMSActive] BOOL NOT NULL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 43;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update44: ККМ: Переделать поля "Docs.KKMSCheckNumber" и "Docs.KKMSIdCommand" => в Кассу и Банк 

        private async Task<bool> Update44(DbConnectionSklad db)
        {
            #region 1. DocCashOfficeSums.ККМ

            string SQL =
                "ALTER TABLE DocCashOfficeSums ADD COLUMN [KKMSCheckNumber] INTEGER; " +
                "ALTER TABLE DocCashOfficeSums ADD COLUMN [KKMSIdCommand] TEXT(64); " +
                "ALTER TABLE DocCashOfficeSums ADD COLUMN [KKMSEMail] TEXT(32); " +
                "ALTER TABLE DocCashOfficeSums ADD COLUMN [KKMSPhone] TEXT(32); " + 

                "ALTER TABLE DocBankSums ADD COLUMN [KKMSCheckNumber] INTEGER; " +
                "ALTER TABLE DocBankSums ADD COLUMN [KKMSIdCommand] TEXT(64); " +
                "ALTER TABLE DocBankSums ADD COLUMN [KKMSEMail] TEXT(32); " +
                "ALTER TABLE DocBankSums ADD COLUMN [KKMSPhone] TEXT(32); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 44;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update45: SysSettings -> Финансы -> Оплата документов [Касса + Банк, Касса, Банк]

        private async Task<bool> Update45(DbConnectionSklad db)
        {
            #region 1. SysSettings.ККМ

            string SQL =
                "ALTER TABLE SysSettings ADD COLUMN [PayType] INTEGER DEFAULT 0 NOT NULL; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 45;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update46: DirEmployees - SalaryPercentService1TabsType, SalaryPercentService1Tabs, SalaryPercentService2TabsType, SalaryPercentService2Tabs

        private async Task<bool> Update46(DbConnectionSklad db)
        {
            #region 1. DirEmployees

            string SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [SalaryPercentService1TabsType] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [SalaryPercentService1Tabs] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirEmployees ADD COLUMN [SalaryPercentService2TabsType] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [SalaryPercentService2Tabs] REAL NOT NULL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 46;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update47: DirEmployees - Права для сотрудника (не Админа точки) 

        private async Task<bool> Update47(DbConnectionSklad db)
        {
            #region 1. DirEmployees

            string SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServicePurchesExtradition] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServicePurchesExtraditionCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocServicePurchesExtradition=1, RightDocServicePurchesExtraditionCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 47;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update48: Витрина

        private async Task<bool> Update48(DbConnectionSklad db)
        {
            #region 1. DirNomens, DirOrdersStates, DirWebShopUO

            string SQL =

                // === DirNomens: Image ===
                "ALTER TABLE DirNomens ADD COLUMN [SysGen1ID] INTEGER; " +
                "ALTER TABLE DirNomens ADD COLUMN [SysGen2ID] INTEGER; " +
                "ALTER TABLE DirNomens ADD COLUMN [SysGen3ID] INTEGER; " +
                "ALTER TABLE DirNomens ADD COLUMN [SysGen4ID] INTEGER; " +
                "ALTER TABLE DirNomens ADD COLUMN [SysGen5ID] INTEGER; " +


                // === DirOrdersStates ===
                "CREATE TABLE[DirOrdersStates]( \n" +
                "    [DirOrdersStateID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "    [Del] BOOL NOT NULL DEFAULT 0, \n" +
                "    [SysRecord] BOOL NOT NULL DEFAULT 0, \n" +
                "    [DirOrdersStateName] TEXT NOT NULL, \n" +
                "    [CustomerPurch] INTEGER NOT NULL DEFAULT 1, \n" +
                "    [DirOrdersStateDesc] TEXT(1024) \n" +
                "); " +

                //ListObjects
                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(74, 2, 'DirOrdersStates', 'Справочник Заказы покупателя'); " +

                //CustomerPurch:
                //1 - Customer Doc
                //2 - Customer Nomen
                //3 - Purch Doc
                //4 - Purch Nomen
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(1, 1, 'Ожидает оплаты', 1); " +
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(2, 1, 'Оплачен', 1); " +
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(3, 1, 'Рассматривается', 1); " +
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(4, 1, 'Ожидается от поставщика', 1); " +
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(5, 1, 'На складе', 1); " +
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(6, 1, 'Частично отдан покупателю', 1); " +
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(7, 1, 'Отдан покупателю (выполнен)', 1); " +

                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(8, 1, 'Отправлена заявка поставщику', 2); " +
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(9, 1, 'Частично получен', 2); " +
                "INSERT INTO DirOrdersStates (DirOrdersStateID, SysRecord, DirOrdersStateName, CustomerPurch)values(10, 1, 'Получен (выполнен)', 2); " +

                //Права для заказов
                "ALTER TABLE DirEmployees ADD COLUMN [RightDirOrdersStates] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDirOrdersStatesCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDirOrdersStates=1, RightDirOrdersStatesCheck=1 WHERE DirEmployeeID=1; " +



                // === DirWebShopUO ===
                "CREATE TABLE [DirWebShopUOs] ( \n" +
                "    [DirWebShopUOID] INTEGER NOT NULL PRIMARY KEY, \n" +
                "    [Del] BOOL NOT NULL DEFAULT 0, \n" +
                "    [SysRecord] BOOL NOT NULL DEFAULT 0, \n" +
                "    [DirWebShopUOName] TEXT(256) NOT NULL, \n" +
                "    [DateCreate] DATE NOT NULL DEFAULT (datetime(CURRENT_TIMESTAMP, 'localtime')), \n" +

                "    [DomainName] TEXT(256) NOT NULL, \n" +

                "    [Nomen_DirPriceTypeID] INTEGER NOT NULL CONSTRAINT [FK_DirWebShopUO_Nomen_DirPriceTypeID] REFERENCES [DirPriceTypes]([DirPriceTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Nomen_Remains] BOOL NOT NULL DEFAULT 0, \n" +

                //Нужно поле "DirCurrencyNameShort"
                "    [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DirWebShopUO_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

                "    [Orders_Doc_DirOrdersStateID] INTEGER NOT NULL CONSTRAINT [FK_DirWebShopUO_Orders_Doc_DirOrdersStateID] REFERENCES [DirOrdersStates]([DirOrdersStateID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Orders_Nomen_DirOrdersStateID] INTEGER NOT NULL CONSTRAINT [FK_DirWebShopUO_Orders_Nomen_DirOrdersStateID] REFERENCES [DirOrdersStates]([DirOrdersStateID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Orders_DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DirWebShopUO_Orders_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Orders_DirContractorIDOrg] INTEGER NOT NULL CONSTRAINT [FK_DirWebShopUO_Orders_DirContractorIDOrg] REFERENCES [DirContractors]([DirContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Orders_DirContractorID] INTEGER NOT NULL CONSTRAINT [FK_DirWebShopUO_Orders_DirContractorID] REFERENCES [DirContractors]([DirContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Orders_Reserve] BOOL NOT NULL DEFAULT 0, \n" +

                //Слайдер
                "    [Slider_Quantity] int NOT NULL DEFAULT 0, \n" +
                //1.
                "    [Slider_DirNomen1ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Slider_DirNomen1ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [SysGen1ID] TEXT(1024), \n" +
                //2.
                "    [Slider_DirNomen2ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Slider_DirNomen2ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [SysGen2ID] TEXT(1024), \n" +
                //3.
                "    [Slider_DirNomen3ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Slider_DirNomen3ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [SysGen3ID] TEXT(1024), \n" +
                //4.
                "    [Slider_DirNomen4ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Slider_DirNomen4ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [SysGen4ID] TEXT(1024), \n" +

                //Рекомендованные
                "    [Recommended_Quantity] int NOT NULL DEFAULT 0, \n" +
                //4.
                "    [Recommended_DirNomen1ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen1ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen2ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen2ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen3ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen3ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen4ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen4ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                //8.
                "    [Recommended_DirNomen5ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen5ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen6ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen6ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen7ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen7ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen8ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen8ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                //12.
                "    [Recommended_DirNomen9ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen9ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen10ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen10ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen11ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen11ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen12ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen12ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                //16.
                "    [Recommended_DirNomen13ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen13ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen14ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen14ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen15ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen15ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "    [Recommended_DirNomen16ID] INTEGER CONSTRAINT [FK_DirWebShopUO_Recommended_DirNomen16ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +

                //HTML
                "    [Payment] TEXT(4096), \n" +
                "    [AboutUs] TEXT(4096), \n" +
                "    [DeliveryInformation] TEXT(4096), \n" +
                "    [PrivacyPolicy] TEXT(4096), \n" +
                "    [TermsConditions] TEXT(4096), \n" +
                "    [ContactUs] TEXT(4096), \n" +
                "    [Returns] TEXT(4096), \n" +
                "    [SiteMap] TEXT(4096), \n" +
                "    [Affiliate] TEXT(4096), \n" +
                "    [Specials] TEXT(4096), \n" +

                "    [DirNomenGroup_Top] INTEGER DEFAULT 1 \n" +

                "); " +


                //ListObjects
                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(75, 2, 'DirWebShopUOs', 'Справочник Интернет-Витрина'); " +

                //Права для заказов
                "ALTER TABLE DirEmployees ADD COLUMN [RightDirWebShopUOs] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDirWebShopUOsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDirWebShopUOs=1, RightDirWebShopUOsCheck=1 WHERE DirEmployeeID=1; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 48;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update49: Витрина

        private async Task<bool> Update49(DbConnectionSklad db)
        {
            #region 1. DirNomens.DirNomenNameLatin

            string SQL = "ALTER TABLE DirNomens ADD COLUMN [DirNomenNameURL] TEXT(256); ";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //Меняем: DirNomenNameLatin = PartionnyAccount.Classes.Function.Transliteration.Front(DirNomenName);
            SQL = "UPDATE DirNomens SET DirNomenNameURL=@DirNomenNameURL;";
            System.Collections.Generic.List<PartionnyAccount.Models.Sklad.Dir.DirNomen> dirNomenList = db.DirNomens.ToList();
            for (int i = 0; i < dirNomenList.Count(); i++)
            {
                PartionnyAccount.Models.Sklad.Dir.DirNomen dirNomen = dirNomenList[i];
                dirNomen.DirNomenNameURL = PartionnyAccount.Classes.Function.Transliteration.Front(dirNomen.DirNomenName).ToLower();

                db.Entry(dirNomen).State = EntityState.Modified;
                await db.SaveChangesAsync();

                /*
                SQLiteParameter parDirNomenNameURL = new SQLiteParameter("@DirNomenNameURL", System.Data.DbType.String) { Value = dirNomen.DirNomenNameURL };
                db.Database.ExecuteSqlCommandAsync(SQL, parDirNomenNameURL);
                */
            }


            SQL = "CREATE INDEX[IDX_DirNomens_DirNomenNameURL] ON[DirNomens]([DirNomenNameURL] COLLATE[NOCASE] ASC);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 49;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update50: Витрина

        private async Task<bool> Update50(DbConnectionSklad db)
        {
            #region 1. Docs ADD COLUMN and TRIGGER

            string SQL =

                "ALTER TABLE Docs ADD COLUMN [DocDateHeld] DATE; " +
                "ALTER TABLE Docs ADD COLUMN [DocDatePayment] DATE; " +


                "UPDATE Docs SET DocDateHeld=DocDate, DocDatePayment=DocDate; " +


                //Касса

                "DROP TRIGGER [TG_INSERT_DocCashOfficeSums_Docs]; " +

                "CREATE TRIGGER[TG_INSERT_DocCashOfficeSums_Docs] \n" +
                "AFTER INSERT \n" +
                "ON[DocCashOfficeSums] \n" +
                "BEGIN \n" +

                "  UPDATE Docs \n" +
                "  SET[Payment] = \n" +
                "  IFNULL( \n" +
                "   ( \n" +
                "    SELECT SUM(Tab1.Sums) \n" +
                "    FROM \n" +
                "      ( \n" +
                "        SELECT abs(round(SUM((DocCashOfficeSums.DocCashOfficeSumSum * 1.0 * DocCashOfficeSums.DirCurrencyRate * 1.0) / DocCashOfficeSums.DirCurrencyMultiplicity * 1.0), 2)) AS Sums \n" +
                "        FROM DocCashOfficeSums \n" +
                "        WHERE DocCashOfficeSums.DocID = NEW.DocID \n" +
                "        GROUP BY DocCashOfficeSums.DocID \n" +

                "        UNION ALL \n" +

                "        SELECT abs(round(SUM((DocBankSums.DocBankSumSum * 1.0 * DocBankSums.DirCurrencyRate * 1.0) / DocBankSums.DirCurrencyMultiplicity * 1.0), 2)) AS Sums \n" +
                "        FROM DocBankSums \n" +
                "        WHERE DocBankSums.DocID = NEW.DocID \n" +
                "        GROUP BY DocBankSums.DocID \n" +
                "      ) AS Tab1 \n" +
                "    ) \n" +
                "   , 0), \n" +
                "   [DocDatePayment] = strftime('%Y-%m-%d', date('now')) \n" +
                " WHERE DocID = NEW.DocID; \n" +
                "END;" +


                "DROP TRIGGER [TG_DELETE_DocCashOfficeSums_Docs]; " +

                "CREATE TRIGGER[TG_DELETE_DocCashOfficeSums_Docs] \n" +
                "AFTER DELETE \n" +
                "ON[DocCashOfficeSums] \n" +
                "BEGIN \n" +

                "  UPDATE Docs \n" +
                "  SET[Payment] = \n" +
                "  IFNULL( \n" +
                "  ( \n" +
                "    SELECT SUM(Tab1.Sums) \n" +
                "    FROM \n" +
                "      ( \n" +
                "        SELECT IFNULL(abs(round(SUM((DocCashOfficeSums.DocCashOfficeSumSum * 1.0 * DocCashOfficeSums.DirCurrencyRate * 1.0) / DocCashOfficeSums.DirCurrencyMultiplicity * 1.0), 2)), 0) AS Sums \n" +
                "        FROM DocCashOfficeSums \n" +
                "        WHERE DocCashOfficeSums.DocID = OLD.DocID \n" +
                "        GROUP BY DocCashOfficeSums.DocID \n" +

                "        UNION ALL \n" +

                "        SELECT IFNULL(abs(round(SUM((DocBankSums.DocBankSumSum * 1.0 * DocBankSums.DirCurrencyRate * 1.0) / DocBankSums.DirCurrencyMultiplicity * 1.0), 2)), 0) AS Sums \n" +
                "        FROM DocBankSums \n" +
                "        WHERE DocBankSums.DocID = OLD.DocID \n" +
                "        GROUP BY DocBankSums.DocID \n" +
                "      ) AS Tab1 \n" +
                "   ) \n" +
                "   , 0) \n" +

                //"   [DocDatePayment] =  Docs.[DocDate] \n" +

                " WHERE DocID = OLD.DocID; \n" +

                "END; " +


                //Банк

                "DROP TRIGGER [TG_INSERT_DocBankSums_Docs]; " +

                "CREATE TRIGGER[TG_INSERT_DocBankSums_Docs] \n" +
                "AFTER INSERT \n" +
                "ON[DocBankSums] \n" +
                "BEGIN \n" +

                "  UPDATE Docs \n" +
                "  SET[Payment] = \n" +
                "  IFNULL( \n" +
                "   ( \n" +
                "    SELECT SUM(Tab1.Sums) \n" +
                "    FROM \n" +
                "      ( \n" +
                "        SELECT abs(round(SUM((DocBankSums.DocBankSumSum * 1.0 * DocBankSums.DirCurrencyRate * 1.0) / DocBankSums.DirCurrencyMultiplicity * 1.0), 2)) AS Sums \n" +
                "        FROM DocBankSums \n" +
                "        WHERE DocBankSums.DocID = NEW.DocID \n" +
                "        GROUP BY DocBankSums.DocID \n" +

                "        UNION ALL \n" +

                "        SELECT abs(round(SUM((DocCashOfficeSums.DocCashOfficeSumSum * 1.0 * DocCashOfficeSums.DirCurrencyRate * 1.0) / DocCashOfficeSums.DirCurrencyMultiplicity * 1.0), 2)) AS Sums \n" +
                "        FROM DocCashOfficeSums \n" +
                "        WHERE DocCashOfficeSums.DocID = NEW.DocID \n" +
                "        GROUP BY DocCashOfficeSums.DocID \n" +
                "      ) AS Tab1 \n" +
                "    ) \n" +
                "   , 0), \n" +
                "   [DocDatePayment] = strftime('%Y-%m-%d', date('now')) \n" +
                " WHERE DocID = NEW.DocID; \n" +

                "END; " +


                "DROP TRIGGER [TG_DELETE_DocBankSums_Docs]; " +

                "CREATE TRIGGER[TG_DELETE_DocBankSums_Docs] \n" +
                "AFTER DELETE \n" +
                "ON[DocBankSums] \n" +
                "BEGIN \n" +

                "  UPDATE Docs \n" +
                "  SET[Payment] = \n" +
                "  IFNULL( \n" +
                "  ( \n" +
                "    SELECT SUM(Tab1.Sums) \n" +
                "    FROM \n" +
                "      ( \n" +
                "        SELECT IFNULL(abs(round(SUM((DocBankSums.DocBankSumSum * 1.0 * DocBankSums.DirCurrencyRate * 1.0) / DocBankSums.DirCurrencyMultiplicity * 1.0), 2)), 0) AS Sums \n" +
                "        FROM DocBankSums \n" +
                "        WHERE DocBankSums.DocID = OLD.DocID \n" +
                "        GROUP BY DocBankSums.DocID \n" +

                "        UNION ALL \n" +

                "        SELECT IFNULL(abs(round(SUM((DocCashOfficeSums.DocCashOfficeSumSum * 1.0 * DocCashOfficeSums.DirCurrencyRate * 1.0) / DocCashOfficeSums.DirCurrencyMultiplicity * 1.0), 2)), 0) AS Sums \n" +
                "        FROM DocCashOfficeSums \n" +
                "        WHERE DocCashOfficeSums.DocID = OLD.DocID \n" +
                "        GROUP BY DocCashOfficeSums.DocID \n" +
                "      ) AS Tab1 \n" +
                "   ) \n" +
                "   , 0) \n" +

                //"   [DocDatePayment] =  Docs.[DocDate] \n" +

                " WHERE DocID = OLD.DocID; \n" +

                "END; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. SysSettings ADD COLUMN "DocsSortField"

            SQL = "ALTER TABLE SysSettings ADD COLUMN [DocsSortField] INTEGER DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 50;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update51: Витрина

        private async Task<bool> Update51(DbConnectionSklad db)
        {
            #region 1. SysSettings ADD COLUMN "DocsSortField"

            string SQL =
                //1.Зарплата
                "ALTER TABLE DirEmployees ADD COLUMN [RightVitrina0] BOOL DEFAULT 1; " +
                //2.Витрина
                "ALTER TABLE DirEmployees ADD COLUMN [RightSalaries0] BOOL DEFAULT 1; " +
                //3.Аналитика
                "ALTER TABLE DirEmployees ADD COLUMN [RightAnalitics0] BOOL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 51;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update52: Витрина

        private async Task<bool> Update52(DbConnectionSklad db)
        {
            #region 1. SysSettings ADD COLUMN "DocsSortField"

            string SQL =
                "ALTER TABLE DocServicePurches ADD COLUMN [DiscountX] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DocServicePurches ADD COLUMN [DiscountY] REAL NOT NULL DEFAULT 0; " +

                //"ALTER TABLE DirEmployees ADD COLUMN [RightDocServicePurchesDiscount] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServicePurchesDiscountCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocServicePurchesDiscountCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 52;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update53: DocsSortField

        private async Task<bool> Update53(DbConnectionSklad db)
        {
            #region 1. SysSettings ADD COLUMN "DocsSortField"

            string SQL = 
                "ALTER TABLE SysSettings ADD COLUMN [DiscountPercentMarket] REAL NOT NULL DEFAULT 20; " +
                "ALTER TABLE SysSettings ADD COLUMN [DiscountPercentService] REAL NOT NULL DEFAULT 20; " +
                "ALTER TABLE SysSettings ADD COLUMN [DiscountPercentSecondHand] REAL NOT NULL DEFAULT 20; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 53;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update54: Витрина

        private async Task<bool> Update54(DbConnectionSklad db)
        {
            #region 1. SysSettings ADD COLUMN "DocsSortField"

            string SQL =
                "UPDATE Docs SET DocDatePayment = " + 

                "(" +
                " SELECT dat FROM " + 
                " (" + 
                "  SELECT DocCashOfficeSumDate AS dat FROM DocCashOfficeSums WHERE DocCashOfficeSums.DocID=Docs.DocID " +
                "  UNION " +
                "  SELECT DocBankSumDate AS dat FROM DocBankSums WHERE DocBankSums.DocID=Docs.DocID " +
                " ) AS Pay LIMIT 1 " + 
                ") " +

                "WHERE DocDatePayment IS NULL ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 54;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update55: Скидка: от суммы или от цены (если продали более 1 аппарата)

        private async Task<bool> Update55(DbConnectionSklad db)
        {
            #region 1. Скидка: от суммы

            string SQL = "ALTER TABLE SysSettings ADD COLUMN [DiscountMarketType] INTEGER NOT NULL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 55;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update56: БУ - Инвенторизация (DocSecondHandInventories - DocSecondHandInventoryID)

        private async Task<bool> Update56(DbConnectionSklad db)
        {
            #region 1. БУ - Инвенторизация

            string SQL =

            "CREATE TABLE[DocSecondHandInventories]( \n" +
            "  [DocSecondHandInventoryID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT CONSTRAINT[FK_DocSecondHandInventories_DocID] REFERENCES[DocSecondHandInventories]([DocSecondHandInventoryID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DocID] INTEGER NOT NULL, \n" +
            "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandInventories_DirWarehouseID] REFERENCES[DirWarehouses] ([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED \n" +
            "); " +
            "CREATE INDEX[IDX_DocSecondHandInventories_DirWarehouseID] ON[DocSecondHandInventories] ([DirWarehouseID]); " +
            "CREATE INDEX[IDX_DocSecondHandInventories_DocID] ON[DocSecondHandInventories] ([DocID]); " +


            "CREATE TABLE[DocSecondHandInventoryTabs] ( \n" +
            "  [DocSecondHandInventoryTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            "  [DocSecondHandInventoryID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandInventoryTabs_DocSecondHandInventoryID] REFERENCES[DocSecondHandInventories]([DocSecondHandInventoryID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

            "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [Rem2PartyID] INTEGER CONSTRAINT [FK_DocSecondHandInventoryTabs_Rem2PartyID] REFERENCES [Rem2Parties]([Rem2PartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [Quantity] REAL NOT NULL, \n" +
            "  [PriceVAT] REAL NOT NULL, \n" +
            "  [PriceCurrency] REAL NOT NULL, \n" +
            "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1, \n" +
            "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1, \n" +

            "  [Exist] BOOL NOT NULL DEFAULT 1 \n" +

            "); " +

            "CREATE INDEX [IDX_DocSecondHandInventoryTabs_DirServiceNomenID] ON [DocSecondHandInventoryTabs] ([DirServiceNomenID]); " +
            "CREATE INDEX [IDX_DocSecondHandInventoryTabs_DocSecondHandInventoryID] ON[DocSecondHandInventoryTabs] ([DocSecondHandInventoryID]); " +
            "CREATE INDEX [IDX_DocSecondHandInventoryTabs_Rem2PartyID] ON [DocSecondHandInventoryTabs] ([Rem2PartyID]); " +


            "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(76, 3, 'DocSecondHandInventories', 'Документ Б/У Инвентаризация'); " +


            "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandInventories] INTEGER DEFAULT 3; " +
            "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandInventoriesCheck] BOOL DEFAULT 0; " +
            "UPDATE DirEmployees SET RightDocSecondHandInventories=1, RightDocSecondHandInventoriesCheck=1 WHERE DirEmployeeID=1; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 56;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update57: БУ - Инвенторизация (Exist - change)

        private async Task<bool> Update57(DbConnectionSklad db)
        {
            #region 1. БУ - Инвенторизация

            string SQL =

            "PRAGMA foreign_keys = OFF; " +


            //БУ.Инвентаризация 


            "DROP TABLE[DocSecondHandInventories]; " +

            "CREATE TABLE[DocSecondHandInventories]( \n" +
            "  [DocSecondHandInventoryID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT CONSTRAINT[FK_DocSecondHandInventories_DocID] REFERENCES[DocSecondHandInventories]([DocSecondHandInventoryID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DocID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandInventories_DocID] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandInventories_DirWarehouseID] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

            // ВыпадающийСписок: "Списывать с ЗП": [1.Точка, 2.Сотрудник]
            "  [SpisatS] INTEGER NOT NULL DEFAULT 1, \n" +
            // ВыпадающийСписок: "Сотрудники" (если выбрали Сотрудник)
            "  [SpisatSDirEmployeeID] INTEGER CONSTRAINT[FK_DocSecondHandInventoryTabs_SpisatSDirEmployeeID] REFERENCES[DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED \n" +

            "); " +
            
            "CREATE INDEX[IDX_DocSecondHandInventories_DirWarehouseID] ON[DocSecondHandInventories] ([DirWarehouseID]); " +
            "CREATE INDEX[IDX_DocSecondHandInventories_DocID] ON[DocSecondHandInventories] ([DocID]); " +



            "DROP TABLE[DocSecondHandInventoryTabs]; " +

            "CREATE TABLE[DocSecondHandInventoryTabs] ( \n" +
            "  [DocSecondHandInventoryTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            "  [DocSecondHandInventoryID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandInventoryTabs_DocSecondHandInventoryID] REFERENCES[DocSecondHandInventories]([DocSecondHandInventoryID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

            "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [Rem2PartyID] INTEGER CONSTRAINT [FK_DocSecondHandInventoryTabs_Rem2PartyID] REFERENCES [Rem2Parties]([Rem2PartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [Quantity] REAL NOT NULL, \n" +
            "  [PriceVAT] REAL NOT NULL, \n" +
            "  [PriceCurrency] REAL NOT NULL, \n" +
            "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1, \n" +
            "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1, \n" +

            // Статус: Присутствует, Списывается с ЗП, На разбор
            "  [Exist] INTEGER NOT NULL DEFAULT 1 \n" +

            "); " +

            "CREATE INDEX [IDX_DocSecondHandInventoryTabs_DirServiceNomenID] ON [DocSecondHandInventoryTabs] ([DirServiceNomenID]); " +
            "CREATE INDEX [IDX_DocSecondHandInventoryTabs_DocSecondHandInventoryID] ON [DocSecondHandInventoryTabs] ([DocSecondHandInventoryID]); " +
            "CREATE INDEX [IDX_DocSecondHandInventoryTabs_Rem2PartyID] ON [DocSecondHandInventoryTabs] ([Rem2PartyID]); " +
            

            "PRAGMA foreign_keys = ON; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. Склады: добавление ещё одной Локации: №5 БУ.Разборка

            var query = await
                (
                    from x in db.DirWarehouses
                    where x.Sub == null
                    select x
                ).ToListAsync();

            if (query.Count() > 0)
            {
                for (int i = 0; i < query.Count(); i++)
                {
                    int? DirWarehouseID = query[i].DirWarehouseID;

                    //1.1. Списание
                    Models.Sklad.Dir.DirWarehouse dirWarehouse1 = new Models.Sklad.Dir.DirWarehouse();
                    dirWarehouse1.Sub = query[i].DirWarehouseID;
                    dirWarehouse1.DirBankID = query[i].DirBankID;
                    dirWarehouse1.DirCashOfficeID = query[i].DirCashOfficeID;
                    dirWarehouse1.DirWarehouseAddress = query[i].DirWarehouseAddress;
                    dirWarehouse1.Phone = query[i].Phone;
                    dirWarehouse1.DirWarehouseName = query[i].DirWarehouseName + " БУ.Разбор";
                    dirWarehouse1.DirWarehouseLoc = 5;

                    db.Entry(dirWarehouse1).State = EntityState.Added;
                    await Task.Run(() => db.SaveChangesAsync());
                }
            }

            #endregion


            //Не используется!!!
            #region 3. DirSecondHandRazborStatuses

            /*
            SQL =
            "CREATE TABLE[DirSecondHandRazborStatuses]( \n" +
            " [DirSecondHandRazborStatusID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            " [DirSecondHandRazborStatusName] TEXT(256) NOT NULL, \n" +
            " [SortNum] INTEGER NOT NULL \n" +
            "); " +

            "CREATE UNIQUE INDEX[UDX_DirSecondHandRazborStatuses_SortNum] ON[DirSecondHandRazborStatuses]([SortNum]); " +

            "INSERT INTO DirSecondHandRazborStatuses (DirSecondHandRazborStatusID, DirSecondHandRazborStatusName, SortNum)values(1, 'Перемещён', 1); " +
            "INSERT INTO DirSecondHandRazborStatuses (DirSecondHandRazborStatusID, DirSecondHandRazborStatusName, SortNum)values(2, 'Предразбор (мастерская)', 2); " +
            "INSERT INTO DirSecondHandRazborStatuses (DirSecondHandRazborStatusID, DirSecondHandRazborStatusName, SortNum)values(7, 'Готов для продажи', 3); " +
            "INSERT INTO DirSecondHandRazborStatuses (DirSecondHandRazborStatusID, DirSecondHandRazborStatusName, SortNum)values(9, 'Выдан', 4); " +
            */

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 4. БУ - Разбор

            //Учета по типу СЦ
            //DocSecondHandRazbors
            //DocServiceRazborTabs

            SQL =

            //Doc
            "CREATE TABLE[DocSecondHandRazbors](  \n" +
            "    [DocSecondHandRazborID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            "    [DocID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_DocID] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

            //DocSecondHandInventory.DocID;
            "    [DocIDFrom] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_DocIDFrom] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +


            //!!! Важно !!!
            //Поступление на разбор может быть и из БУ и из СЦ
            //ListObjects.ListObjectID - тип документа: БУ(65) или СЦ(40)
            "    [ListObjectIDFromType] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_ListObjectIDFromType] REFERENCES[ListObjects]([ListObjectID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            //Docs.DocID - по DocID можно вычислить ID-шник документа "Docs.NumberReal"
            "    [DocIDFromType] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_DocIDFrom] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +


            "    [DirWarehouseID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_DirWarehouseID] REFERENCES[DirWarehouses] ([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_DirServiceNomenID] REFERENCES[DirServiceNomens] ([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [DirSecondHandStatusID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_DirSecondHandStatusID] REFERENCES[DirSecondHandStatuses] ([DirSecondHandStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [DirSecondHandStatusID_789] INTEGER CONSTRAINT[FK_DocSecondHandRazbors_DirSecondHandStatusID_789] REFERENCES[DirSecondHandStatuses] ([DirSecondHandStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

            /*
            "    [SerialNumberNo] BOOL DEFAULT 0, \n" +
            "    [SerialNumber] TEXT(256), \n" +
            "    [ComponentPasTextNo] BOOL DEFAULT 0, \n" +
            "    [ComponentPasText] TEXT(256), \n" +
            "    [ComponentOtherText] TEXT(256), \n" +
            "    [ProblemClientWords] TEXT(2048), \n" +
            "    [Note] TEXT(2048), \n" +
            */

            "    [DirEmployeeIDMaster] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_DirEmployeeIDMaster] REFERENCES[DirEmployees] ([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

            /*
            "    [DirServiceContractorName] TEXT(256), \n" +
            "    [DirServiceContractorRegular] BOOL DEFAULT 0, \n" +
            "    [DirServiceContractorID] INTEGER CONSTRAINT[FK_DocSecondHandRazbors_DirServiceContractorID] REFERENCES[DirServiceContractors] ([DirServiceContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [DirServiceContractorAddress] TEXT, \n" +
            "    [DirServiceContractorPhone] TEXT, \n" +
            "    [DirServiceContractorEmail] TEXT, \n" +
            "    [PassportSeries] TEXT, \n" +
            "    [PassportNumber] TEXT, \n" +
            "    [ServiceTypeRepair] INTEGER NOT NULL DEFAULT 1, \n" +
            */

            "    [PriceVAT] REAL NOT NULL, \n" +
            "    [PriceCurrency] REAL NOT NULL, \n" +
            "    [DirCurrencyID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazbors_DirCurrencyID] REFERENCES[DirCurrencies] ([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [DirCurrencyRate] REAL NOT NULL DEFAULT 1, \n" +
            "    [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1, \n" +

            /*
            "    [PriceRetailVAT] REAL NOT NULL, \n" +
            "    [PriceRetailCurrency] REAL NOT NULL, \n" +
            "    [PriceWholesaleVAT] REAL NOT NULL, \n" +
            "    [PriceWholesaleCurrency] REAL NOT NULL, \n" +
            "    [PriceIMVAT] REAL NOT NULL, \n" +
            "    [PriceIMCurrency] REAL NOT NULL \n" +
            "    [ComponentOther] BOOL DEFAULT 0, \n" +
            "    [DateDone] DATE NOT NULL, \n" +
            "    [IssuanceDate] DATE, \n" +
            "    [Summ_NotPre] REAL, \n" +
            "    [Sums] REAL, \n" +
            */

            //"    [DocDate_First] DATE, \n" +
            "    [DateStatusChange] DATE, \n" +

            /*
            "    [Sums1] REAL NOT NULL DEFAULT 0, \n" +
            "    [Sums2] REAL NOT NULL DEFAULT 0, \n" +
            "    [FromService] BOOL DEFAULT 0, \n" +
            "    [DocIDService] INTEGER CONSTRAINT[FK_DocSecondHandRazbors_DocIDService] REFERENCES[Docs] ([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [Sums1Service] REAL DEFAULT 0, \n" +
            "    [Sums2Service] REAL DEFAULT 0 \n" +
            */



            //!!! Дополнительно !!!
            //1. С какой партии списываем (перемещение на Локальный склад, под-склад)
            //   В партии содержатся все необходимые параметры: 
            //   DocID - документ который создал партию (или DocSecondHandPurches или DocSecondHandMovements)
            //   DocIDFirst - документ создания аппарата (только DocSecondHandPurches)
            "  [Rem2PartyID] INTEGER CONSTRAINT [FK_DocSecondHandRazbors_Rem2PartyID] REFERENCES [Rem2Parties]([Rem2PartyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +

            "  [SumsDirNomen] REAL DEFAULT 0" + 


            ");" +

            //"CREATE INDEX[IDX_DocSecondHandRazbors_SerialNumber] ON[DocSecondHandRazbors] ([SerialNumber] COLLATE NOCASE); " +
            //"CREATE INDEX[IDX_DocSecondHandRazbors_IssuanceDate] ON[DocSecondHandRazbors] ([IssuanceDate]); " +
            "CREATE INDEX[IDX_DocSecondHandRazbors_DirWarehouseID] ON[DocSecondHandRazbors] ([DirWarehouseID]); " +
            "CREATE INDEX[IDX_DocSecondHandRazbors_DirSecondHandStatusID_789] ON[DocSecondHandRazbors] ([DirSecondHandStatusID_789]); " +
            "CREATE INDEX[IDX_DocSecondHandRazbors_DirSecondHandStatusID] ON[DocSecondHandRazbors] ([DirSecondHandStatusID]); " +
            "CREATE INDEX[IDX_DocSecondHandRazbors_DirServiceNomenID] ON[DocSecondHandRazbors] ([DirServiceNomenID]); " +
            "CREATE INDEX[IDX_DocSecondHandRazbors_DateStatusChange] ON[DocSecondHandRazbors] ([DateStatusChange]); " +
            //"CREATE INDEX[IDX_DocSecondHandRazbors_DirServiceContractorID] ON[DocSecondHandRazbors] ([DirServiceContractorID]); " +



            //Tab
            "CREATE TABLE[DocSecondHandRazborTabs] ( \n" +

            "    [DocSecondHandRazborTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            "    [DocSecondHandRazborID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRazborTabs_DocSecondHandRazborID] REFERENCES[DocSecondHandRazbors]([DocSecondHandRazborID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +

            "    [DirNomenID] INTEGER NOT NULL CONSTRAINT[FK_DocPurchTabs_DirNomenID] REFERENCES[DirNomens] ([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCharColourID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharColourID] REFERENCES[DirCharColours] ([DirCharColourID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCharMaterialID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharMaterialID] REFERENCES[DirCharMaterials] ([DirCharMaterialID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCharNameID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharNameID] REFERENCES[DirCharNames] ([DirCharNameID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCharSeasonID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharSeasonID] REFERENCES[DirCharSeasons] ([DirCharSeasonID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCharSexID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharSexID] REFERENCES[DirCharSexes] ([DirCharSexID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCharSizeID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharSizeID] REFERENCES[DirCharSizes] ([DirCharSizeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCharStyleID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharStyleID] REFERENCES[DirCharStyles] ([DirCharStyleID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCharTextureID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharTextureID] REFERENCES[DirCharTextures] ([DirCharTextureID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [SerialNumber] TEXT(256), \n" +
            "    [Barcode] TEXT(256), \n" +

            "    [Quantity] REAL NOT NULL, \n" +
            "    [PriceVAT] REAL NOT NULL, \n" +
            "    [PriceCurrency] REAL NOT NULL, \n" +
            "    [DirCurrencyID] INTEGER NOT NULL CONSTRAINT[FK_DocPurchTabs_DirCurrencyID] REFERENCES[DirCurrencies] ([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED,\n" +
            "    [DirCurrencyRate] REAL NOT NULL, \n" +
            "    [DirCurrencyMultiplicity] INTEGER NOT NULL, \n" +
            "    [PriceRetailVAT] REAL NOT NULL, \n" +
            "    [PriceRetailCurrency] REAL NOT NULL, \n" +
            "    [PriceWholesaleVAT] REAL NOT NULL, \n" +
            "    [PriceWholesaleCurrency] REAL NOT NULL, \n" +
            "    [PriceIMVAT] REAL NOT NULL, \n" +
            "    [PriceIMCurrency] REAL NOT NULL, \n" +

            "    DirNomenMinimumBalance REAL DEFAULT 0, \n" +
            "    DirCharStyleName TEXT(256), \n" +
            "    [DirContractorID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirContractorID] REFERENCES[DirContractors] ([DirContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED \n" +

            "); " +

            "CREATE INDEX[IDX_DocSecondHandRazborTabs_DirNomenID] ON[DocSecondHandRazborTabs] ([DirNomenID]); " +
            "CREATE INDEX[IDX_DocSecondHandRazborTabs_DocSecondHandRazborID] ON[DocSecondHandRazborTabs] ([DocSecondHandRazborID]); " +


            //Log
            "CREATE TABLE[LogSecondHandRazbors]( \n" +
            "    [LogSecondHandRazborID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            "    [DocSecondHandRazborID] INTEGER NOT NULL CONSTRAINT[FK_LogSecondHandRazbors_DocSecondHandRazborID] REFERENCES[DocSecondHandRazbors]([DocSecondHandRazborID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [DirSecondHandLogTypeID] INTEGER NOT NULL CONSTRAINT[FK_LogSecondHandRazbors_DirServiceLogTypeID] REFERENCES[DirSecondHandLogTypes] ([DirSecondHandLogTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [DirEmployeeID] INTEGER NOT NULL CONSTRAINT[FK_LogSecondHandRazbors_DirEmployees] REFERENCES[DirEmployees] ([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [DirSecondHandStatusID] INTEGER CONSTRAINT[FK_LogSecondHandRazbors_DirSecondHandStatusID] REFERENCES[DirSecondHandStatuses] ([DirSecondHandStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "    [LogSecondHandRazborDate] DATETIME DEFAULT(datetime(CURRENT_TIMESTAMP, 'localtime')), \n" +
            "    [Msg] TEXT(1024) \n" +
            "); " +

            "CREATE INDEX[IDX_LogSecondHandRazbors_DocSecondHandRazborID] ON[LogSecondHandRazbors] ([DocSecondHandRazborID]); " +
            "CREATE INDEX[IDX_LogSecondHandRazbors_DirServiceLogTypeID] ON[LogSecondHandRazbors] ([DirSecondHandLogTypeID]); " +
            "CREATE INDEX[IDX_LogSecondHandRazbors_DirSecondHandStatusID] ON[LogSecondHandRazbors] ([DirSecondHandStatusID]); " +
            "CREATE INDEX[IDX_LogSecondHandRazbors_DirEmployeeID] ON[LogSecondHandRazbors] ([DirEmployeeID]); " +   



            "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(77, 3, 'DocSecondHandRazbors', 'Документ Б/У Разбор аппарата'); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 5. Triggers for Sums

            SQL =

            "CREATE TRIGGER[TG_INSERT_DocSecondHandRazborTabs_DocSecondHandRazbors] \n" +
            "AFTER INSERT ON[DocSecondHandRazborTabs] \n" +
            "BEGIN \n" +

            " UPDATE DocSecondHandRazbors SET \n" +
            " SumsDirNomen = \n" +
            "   ( \n" +
            "    SELECT IFNULL(SUM(DocSecondHandRazborTabs.PriceCurrency), 0) \n" +
            "    FROM DocSecondHandRazborTabs \n" +
            "    WHERE DocSecondHandRazborTabID = NEW.DocSecondHandRazborTabID LIMIT 1 \n" +
            "   ) \n" +

            " WHERE \n" +
            "  DocSecondHandRazborID = \n" +
            "   ( \n" +
            "    SELECT DocSecondHandRazborTabs.DocSecondHandRazborID \n" +
            "    FROM DocSecondHandRazborTabs \n" +
            "    WHERE DocSecondHandRazborTabID = NEW.DocSecondHandRazborTabID LIMIT 1 \n" +
            "   ); \n" +

            "END;" +



            "CREATE TRIGGER[TG_UPDATE_DocSecondHandRazborTabs_DocSecondHandRazbors] \n" +
            "AFTER UPDATE ON[DocSecondHandRazborTabs] \n" +
            "BEGIN \n" +

            " UPDATE DocSecondHandRazbors SET \n" +
            " SumsDirNomen = \n" +
            "   ( \n" +
            "    SELECT IFNULL(SUM(DocSecondHandRazborTabs.PriceCurrency), 0) \n" +
            "    FROM DocSecondHandRazborTabs \n" +
            "    WHERE DocSecondHandRazborTabID = NEW.DocSecondHandRazborTabID LIMIT 1 \n" +
            "   ) \n" +

            " WHERE \n" +
            "  DocSecondHandRazborID = \n" +
            "   ( \n" +
            "    SELECT DocSecondHandRazborTabs.DocSecondHandRazborID \n" +
            "    FROM DocSecondHandRazborTabs \n" +
            "    WHERE DocSecondHandRazborTabID = NEW.DocSecondHandRazborTabID LIMIT 1 \n" +
            "   ); \n" +

            "END; " +



            "CREATE TRIGGER[TG_DELETE_DocSecondHandRazborTabs_DocSecondHandRazbors] \n" +
            "BEFORE DELETE ON[DocSecondHandRazborTabs] \n" +
            "BEGIN \n" +

            " UPDATE DocSecondHandRazbors SET \n" +
            " SumsDirNomen = \n" +
            "   ( \n" +
            "    SELECT IFNULL(SUM(DocSecondHandRazborTabs.PriceCurrency), 0) \n" +
            "    FROM DocSecondHandRazborTabs \n" +
            "    WHERE DocSecondHandRazborTabID = OLD.DocSecondHandRazborTabID LIMIT 1 \n" +
            "   ) \n" +

            " WHERE \n" +
            "  DocSecondHandRazborID = \n" +
            "   ( \n" +
            "    SELECT DocSecondHandRazborTabs.DocSecondHandRazborID \n" +
            "    FROM DocSecondHandRazborTabs \n" +
            "    WHERE DocSecondHandRazborTabID = OLD.DocSecondHandRazborTabID LIMIT 1 \n" +
            "   ); \n" +

            "END; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            #endregion


            #region 6. Права: RightDocSecondHandRazbors

            SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandRazbors] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocSecondHandRazborsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocSecondHandRazbors=1, RightDocSecondHandRazborsCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            #region 6. Исправление найденных ошибок

            SQL =
            "DROP INDEX IDX_LogSecondHands_DocServicePurchID; " +
            "CREATE INDEX [IDX_LogSecondHands_DocSecondHandPurchID] ON [LogSecondHands] ([DocSecondHandPurchID]);";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 57;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update58: БУ.Инвентаризация.ПФ

        private async Task<bool> Update58(DbConnectionSklad db)
        {
            #region ListObjectFieldNames

            string SQL =

                //DocSecondHandInventoryID
                "INSERT INTO ListObjectFieldNames " +
                "(ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)" +
                "values" +
                "(382, 'DocSecondHandInventoryID', 'Документ.БУИнвентаризация.НомерВнутренний', 'Документ.БЖІнвентаризація.НомерВнутрішній'); " +

                //SpisatS
                "INSERT INTO ListObjectFieldNames " +
                "(ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)" +
                "values" +
                "(383, 'SpisatS', 'Документ.БУИнвентаризация.СписатьСЗПТочкаСотрудник'); " +

                //SpisatS.SpisatSDirEmployeeName
                "INSERT INTO ListObjectFieldNames " +
                "(ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)" +
                "values" +
                "(384, 'SpisatSDirEmployeeName', 'Справочник.Сотрудник.Наименование.СписатьСЗП'); " +

                //ExistName
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(385, 'ExistName', 'Документ.БУИнвентаризация.Exist'); " +

                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(386, 'CountRecord1', 'Документ.ТабличнаяЧасть.КоличествоНаименованийПрисутствует'); " +
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(387, 'CountRecord_NumInWords1', 'Документ.ТабличнаяЧасть.КоличествоНаименованийПрисутствуетПрописью'); " +
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(388, 'Sums1', 'Документ.ТабличнаяЧасть.Сумма.Присутствует'); " +

                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(389, 'CountRecord2', 'Документ.ТабличнаяЧасть.КоличествоНаименованийСписываетсяСЗП'); " +
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(390, 'CountRecord_NumInWords2', 'Документ.ТабличнаяЧасть.КоличествоНаименованийСписываетсяСЗППрописью'); " +
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(391, 'Sums2', 'Документ.ТабличнаяЧасть.Сумма.СписываетсяСЗП'); " +

                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(392, 'CountRecord3', 'Документ.ТабличнаяЧасть.КоличествоНаименованийНаРазбор'); " +
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(393, 'CountRecord_NumInWords3', 'Документ.ТабличнаяЧасть.КоличествоНаименованийНаРазборПрописью'); " +
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(394, 'Sums3', 'Документ.ТабличнаяЧасть.Сумма.НаРазбор'); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region ListObjectFields

            SQL =

                //Шапка *** *** *** *** ***
                //DocSecondHandInventoryID
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 382); " +
                //DocDate (Документ.Дата)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 1, 73); " +
                //Base (Документ.Основание)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 72); " +
                //Held (Документ.Проведён)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 75); " +
                //NumberInt (Документ.НомерРучной)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 89); " +
                //DirWarehouseName (Справочник.Склад.Наименование)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 64); " +
                //Description (Документ.Описание)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 74); " +
                //DirWarehouseAddress (Справочник.Склад.Адрес)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 59); " +
                //DirWarehouseDesc (Справочник.Склад.Описание)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 61); " +
                //DocDate_InWords (Документ.ДатаПрописью)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 1, 180); " +
                //DirContractorNameOrg (Справочник.Организация.Наименование)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 167); " +
                //DirContractorEmailOrg (Справочник.Организация.EMail)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 255); " +
                //DirContractorWWWOrg (Справочник.Организация.WWW)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 254); " +
                //DirContractorAddressOrg (Справочник.Организация.Адрес)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 217); " +
                //DirContractorLegalTINOrg (Справочник.Организация.ИНН)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 220); " +
                //DirContractorLegalCATOrg (Справочник.Организация.КПП)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 222); " +
                //DirContractorLegalCertificateNumberOrg (Справочник.Организация.НомерСвидетельства)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 225); " +
                //DirContractorLegalBINOrg (Справочник.Организация.ОГРН)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 223); " +
                //DirContractorLegalOGRNIPOrg (Справочник.Организация.ОГРНИП)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 224); " +
                //DirContractorLegalRNNBOOrg (Справочник.Организация.ОКПО)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 221); " +
                //DirContractorPhoneOrg (Справочник.Организация.Телефон)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 251); " +
                //DirServiceStatusName (Справочник.СервисСтатусы.Наименование)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 363); " +


                //Футер *** *** *** *** ***
                //CountRecord (Документ.ТабличнаяЧасть.КоличествоНаименований)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 131); " +
                //SumCount (Документ.ТабличнаяЧасть.КоличествоНоменклатуры)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 132); " +
                //SumOfVATCurrency - Документ.СуммаСНДС.ВТекущейВалюте
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 157); " +
                //SumOfVATCurrency_InWords - Документ.СуммаСНДСПрописью.ВТекущейВалюте
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 184); " +


                //Табличная часть
                //DocSecondHandPurchID (номер документа)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 377); " +
                //Quantity (Документ.ТабличнаяЧасть.Количество)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 2); " +
                //DirCurrencyRate (Справочник.Валюта.Курс)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 38); " +
                //DirCurrencyMultiplicity (Справочник.Валюта.Кратность)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 35); " +
                //DirCurrencyName (Справочник.Валюта.Наименование)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 36); " +
                //PriceCurrency (Документ.ТабличнаяЧасть.ЦенаСНДС.ВТекущейВалюте)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 93); " +
                //SUMPurchPriceVATCurrency (Документ.ТабличнаяЧасть.СтоимостьПриходаСНДС.ВТекущейВалюте.КоличествоПриходнаяЦена)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 108); " +

                //MarkupRetail (Документ.ТабличнаяЧасть.РозничнаяНаценка)
                //"INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 87); " +
                //PriceRetailVAT (Документ.ТабличнаяЧасть.ЦенаРозничнаяСНДС.ВВалюте)
                //"INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 44); " +
                //PriceRetailCurrency (Документ.ТабличнаяЧасть.ЦенаРозничнаяСНДС.ВТекущейВалюте)
                //"INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 95); " +
                //MarkupWholesale (Документ.ТабличнаяЧасть.ОптоваяНаценка)
                //"INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 88); " +
                //PriceWholesaleVAT (Документ.ТабличнаяЧасть.ЦенаОптоваяСНДС.ВВалюте)
                //"INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 45); " +
                //PriceWholesaleCurrency (Документ.ТабличнаяЧасть.ЦенаОптоваяСНДС.ВТекущейВалюте)
                //"INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 1, 0, 0, 97); " +

                //DirServiceNomenID (Справочник.Аппарат.Код)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 359); " +
                //DirServiceNomenArticle (Справочник.Аппарат.Артикул)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 360); " +
                //DirServiceNomenName (Справочник.Аппарат.Наименование)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 361); " +
                //DirServiceNomenNameFull (Справочник.Аппарат.НаименованиеПолное)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 362); " +


                //SpisatS (Документ.БУИнвентаризация.СписатьСЗПТочкаСотрудник)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 383); " +
                //SpisatSDirEmployeeName (Справочник.Сотрудник.Наименование.СписатьСЗП)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 384); " +
                //ExistName (Документ.БУИнвентаризация.Exist)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 1, 0, 385); " +


                //Footer
                //CountRecord1 (Документ.ТабличнаяЧасть.КоличествоНаименованийПрисутствует)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 386); " +
                //CountRecord_NumInWords1 (Документ.ТабличнаяЧасть.КоличествоНаименованийПрисутствуетПрописью)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 387); " +
                //Sums1 (Документ.ТабличнаяЧасть.Сумма.Присутствует)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 388); " +

                //CountRecord2 (Документ.ТабличнаяЧасть.КоличествоНаименованийПрисутствует)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 389); " +
                //CountRecord_NumInWords2 (Документ.ТабличнаяЧасть.КоличествоНаименованийПрисутствуетПрописью)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 390); " +
                //Sums2 (Документ.ТабличнаяЧасть.Сумма.Присутствует)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 391); " +

                //CountRecord3 (Документ.ТабличнаяЧасть.КоличествоНаименованийПрисутствует)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 392); " +
                //CountRecord_NumInWords3 (Документ.ТабличнаяЧасть.КоличествоНаименованийПрисутствуетПрописью)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 393); " +
                //Sums3 (Документ.ТабличнаяЧасть.Сумма.Присутствует)
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(76, 0, 0, 1, 394); ";



            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region ListObjectPFs


            string[] Fieldname = { "ListObjectPFHtmlHeader", "ListObjectPFHtmlFooter" };
            SQL = "SELECT ListObjectPFHtmlHeader, ListObjectPFHtmlFooter FROM ListObjectPFs WHERE ListObjectPFID=43";
            ArrayList alField = await SelectDataFrom_EtalonDB(SQL, Fieldname);
            string[] mas = (string[])alField[0];



            SQL =
                "INSERT INTO ListObjectPFs " +
                "(" + 
                " ListObjectPFID, ListObjectPFIDSys, ListObjectID, ListLanguageID, ListObjectPFName, ListObjectPFHtmlHeaderUse, " +
                " ListObjectPFHtmlHeader, ListObjectPFHtmlTabUseTab, ListObjectPFHtmlTabEnumerate, " +
                " ListObjectPFHtmlTabFontSize, ListObjectPFHtmlFooterUse, " +
                " ListObjectPFHtmlFooter, MarginTop, MarginBottom, MarginLeft, MarginRight " + 
                ")" +
                "values" +
                "(" +
                " 43, 0, 76, 1, 'БУ: Акт Инвентаризации', 1, @ListObjectPFHtmlHeader, 1, 1, 0, 1, @ListObjectPFHtmlFooter, 0, 0, 0, 0 " + 
                ")";

            SQLiteParameter parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = mas[0] };
            SQLiteParameter parListObjectPFHtmlFooter = new SQLiteParameter("@ListObjectPFHtmlFooter", System.Data.DbType.String) { Value = mas[1] };

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader, parListObjectPFHtmlFooter));

            #endregion

            #region ListObjectPFTabs

            SQL =
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Код', 359, 1, 1, 15); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Наименование', 361, 1, 2, 0); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Кол-во', 2, 2, 3, 15); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Цена', 93, 3, 4, 15); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Сумма', 108, 3, 5, 15); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 58;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update59: БУ.Инвентаризация.ПФ

        private async Task<bool> Update59(DbConnectionSklad db)
        {
            #region ListObjectPFTabs

            string SQL =
                //0
                "DELETE FROM ListObjectPFTabs WHERE ListObjectPFID=43; " + 

                //1.
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Код', 377, 1, 1, 15); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Наименование', 361, 1, 1, 0); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Цена', 93, 3, 1, 15); " +
                //2. 
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Код', 377, 1, 2, 15); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Наименование', 361, 1, 2, 0); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Цена', 93, 3, 2, 15); " +
                //3
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Код', 377, 1, 3, 15); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Наименование', 361, 1, 3, 0); " +
                "INSERT INTO ListObjectPFTabs (ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID, TabNum, Width)values(43, 'Цена', 93, 3, 3, 15); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 59;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update60: Добавление статуса для Лог-а (СЦ и БУ) + Discount в (DocBankSums и DocCashOfficeSums)

        private async Task<bool> Update60(DbConnectionSklad db)
        {
            #region Добавление статуса для Лог-а (СЦ и БУ)

            string SQL =

                "INSERT INTO DirServiceLogTypes (DirServiceLogTypeID, DirServiceLogTypeName)values(10, 'Перемещён в модуль БУ'); " +
                "INSERT INTO DirServiceLogTypes (DirServiceLogTypeID, DirServiceLogTypeName)values(11, 'Перемещён из модуля БУ'); " +

                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(10, 'Перемещён в модуль СЦ'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(11, 'Перемещён из модуля СЦ'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Discount в (DocBankSums и DocCashOfficeSums)

            //Добавляем поле Discount
            SQL =
                "ALTER TABLE DocBankSums ADD COLUMN [Discount] REAL DEFAULT 0; " +
                "ALTER TABLE DocCashOfficeSums ADD COLUMN [Discount] REAL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //Меняем значения Discount из Документов
            SQL =
                "UPDATE DocBankSums " +
                "SET Discount = " +
                "(SELECT Discount FROM Docs WHERE DocBankSums.DocID = Docs.DocID); " +

                "UPDATE DocCashOfficeSums " +
                "SET Discount = " +
                "(SELECT Discount FROM Docs WHERE DocCashOfficeSums.DocID = Docs.DocID) ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 60;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update61: DocSecondHandPurches.DateRetail

        private async Task<bool> Update61(DbConnectionSklad db)
        {
            #region DocSecondHandPurches.DateRetail

            //Добавление поля
            string SQL =
                "ALTER TABLE DocSecondHandPurches ADD COLUMN [DateRetail] DATE; " +
                "ALTER TABLE DocSecondHandPurches ADD COLUMN [DateReturn] DATE; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //Проставляем значения
            SQL =
                "UPDATE DocSecondHandPurches " +
                "SET DateRetail = " +
                "(" +
                " SELECT MAX(Rem2PartyMinuses.DocDate) " +
                " FROM Rem2Parties, Rem2PartyMinuses " + 
                " WHERE " +
                " Rem2Parties.Rem2PartyID=Rem2PartyMinuses.Rem2PartyID and " +
                " DocSecondHandPurches.DocID=Rem2Parties.DocID " +
                ")";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 61;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update62: ListObjectFieldNames.DateStatusChange

        private async Task<bool> Update62(DbConnectionSklad db)
        {
            #region ListObjectFieldNames + ListObjectFields

            string SQL =

                //1
                "INSERT INTO ListObjectFieldNames (ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu)values(395, 'DateStatusChange', 'Документ.СЦ.ДатаСменыСтатуса'); " +

                //2
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(40, 1, 0, 0, 395); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region ListObjectPFs


            string[] Fieldname = { "ListObjectPFHtmlHeader" };
            SQL = "SELECT ListObjectPFHtmlHeader FROM ListObjectPFs WHERE ListObjectPFID=33";
            ArrayList alField = await SelectDataFrom_EtalonDB(SQL, Fieldname);
            string[] mas = (string[])alField[0];



            SQL = "UPDATE ListObjectPFs SET ListObjectPFHtmlHeader = @ListObjectPFHtmlHeader WHERE ListObjectPFID=33";

            SQLiteParameter parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = mas[0] };

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 62;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update63: ListObjectFieldNames.DateStatusChange
        int iHHHH = 0;
        private async Task<bool> Update63(DbConnectionSklad db)
        {
            string SQL = "";

            #region ListObjectPFs

            for (int i = 1; i <= 43; i++)
            {
                iHHHH = i;

                string[] Fieldname = { "ListObjectPFHtmlHeader" };
                SQL = "SELECT ListObjectPFHtmlHeader FROM ListObjectPFs WHERE ListObjectPFID=" + i.ToString();
                ArrayList alField = await SelectDataFrom_EtalonDB(SQL, Fieldname);
                if (alField.Count > 0)
                {
                    string[] mas = (string[])alField[0];

                    if (mas.Length > 0)
                    {
                        SQL = "UPDATE ListObjectPFs SET ListObjectPFHtmlHeader=@ListObjectPFHtmlHeader WHERE ListObjectPFID=" + i.ToString();
                        SQLiteParameter parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = mas[0] };

                        await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader));
                    }
                }

            }

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 63;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion





        //!!! Новый Релиз !!!


        #region Update64: ListObjectFieldNames.DateStatusChange

        private async Task<bool> Update64(DbConnectionSklad db)
        {

            //0. Подготовка === === === === === === === === === === === === === === === === ===

            #region а) В "LogSecondHands" добавить 2-а поля "С точки" и "На точку""

            string SQL =
                "ALTER TABLE LogSecondHands ADD COLUMN [DirWarehouseIDFrom] INTEGER; " +
                "ALTER TABLE LogSecondHands ADD COLUMN [DirWarehouseIDTo] INTEGER; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region б) Ввести новые статусы для аппарата "DirSecondHandStatuses": 

            SQL =

                //  - В продаже (9 - Выдан) - переименовать
                "UPDATE DirSecondHandStatuses SET DirSecondHandStatusName='В продаже' WHERE DirSecondHandStatusID=9; " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(10, 'Продан', 9); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(11, 'Списан с ЗП', 10); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(12, 'В разборе', 11); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(13, 'В разборе - Готов', 12); " +
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(14, 'Разобран', 13); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region в) Ввести новые типы для Лога "DirSecondHandLogTypes"

            SQL =
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(12, 'Перемещён'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(13, 'Продан'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(14, 'Возврат'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(15, 'Логистика (Перемещение)'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(16, 'Списан с ЗП'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(17, 'Перемещён на разбор'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(18, 'Перемещён на разбор - Готов'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(19, 'Разобран'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(20, 'Вернули с Разбора (отмена проводки документа Инвентаризация)'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(21, 'Вернули с Списания с ЗП (отмена проводки документа Инвентаризация)'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region г) Найти в Логе все статусы == 1 (LogSecondHandRazbors.DirSecondHandLogTypeID == 1) и в поле "Msg" записать на какую точку принят аппарат

            SQL =

                "UPDATE LogSecondHands " +
                "SET " +

                "Msg='Аппарат принят на точку №' ||  " +
                "(" +
                " SELECT DirWarehouseID " +
                " FROM DocSecondHandPurches " +
                " WHERE DocSecondHandPurches.DocSecondHandPurchID=LogSecondHands.DocSecondHandPurchID " +
                " LIMIT 1 " +
                ")" +

                ", " +

                "DirWarehouseIDFrom=" +
                "(" +
                " SELECT DirWarehouseID " +
                " FROM DocSecondHandPurches " +
                " WHERE DocSecondHandPurches.DocSecondHandPurchID=LogSecondHands.DocSecondHandPurchID " +
                " LIMIT 1 " + 
                ")" +

                "WHERE LogSecondHands.DirSecondHandLogTypeID=1";

            SQLiteParameter parMsg = new SQLiteParameter("@Msg", System.Data.DbType.String) { Value = "Аппарат принят на точку ..." };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg));

            #endregion



            //1. === === === === === === === === === === === === === === === === ===

            #region DocSecondHandPurches

            //Добавление полей
            SQL =
                "ALTER TABLE DocSecondHandPurches ADD COLUMN [DirReturnTypeID] INTEGER; " +
                "ALTER TABLE DocSecondHandPurches ADD COLUMN [DirDescriptionID] INTEGER; " +
                "ALTER TABLE DocSecondHandPurches ADD COLUMN [DirWarehouseIDPurches] INTEGER CONSTRAINT [FK_DocSecondHandPurches_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            //Изменение этих полей
            //" r.DocID, r.DocSecondHandRetailID, pur.DocSecondHandPurchID, r.DirWarehouseID, rt.DirServiceNomenID, rt.PriceVAT, rt.PriceCurrency, rt.DirCurrencyID, rt.DirCurrencyRate, rt.DirCurrencyMultiplicity, rt.DirReturnTypeID, rt.DirDescriptionID " +
            SQL =
                "UPDATE DocSecondHandPurches SET " +

                "DirReturnTypeID=" +
                "(" +
                "SELECT rt.DirReturnTypeID " +
                "FROM DocSecondHandRetailReturns AS r, DocSecondHandRetailReturnTabs AS rt, Rem2PartyMinuses AS rpm, Rem2Parties AS rp, DocSecondHandPurches AS pur " +
                "WHERE " +
                " r.DocSecondHandRetailReturnID=rt.DocSecondHandRetailReturnID and" +
                " rt.Rem2PartyMinusID=rpm.Rem2PartyMinusID and" +
                " rpm.Rem2PartyID=rp.Rem2PartyID and" +
                " pur.DocID=rp.DocIDFirst and" +
                " pur.DocSecondHandPurchID=DocSecondHandPurches.DocSecondHandPurchID " +
                "ORDER BY rt.DocSecondHandRetailReturnTabID DESC " +
                "LIMIT 1 " +
                "), " +

                "DirDescriptionID=" +
                "(" +
                "SELECT rt.DirDescriptionID " +
                "FROM DocSecondHandRetailReturns AS r, DocSecondHandRetailReturnTabs AS rt, Rem2PartyMinuses AS rpm, Rem2Parties AS rp, DocSecondHandPurches AS pur " +
                "WHERE " +
                " r.DocSecondHandRetailReturnID=rt.DocSecondHandRetailReturnID and" +
                " rt.Rem2PartyMinusID=rpm.Rem2PartyMinusID and" +
                " rpm.Rem2PartyID=rp.Rem2PartyID and" +
                " pur.DocID=rp.DocIDFirst and" +
                " pur.DocSecondHandPurchID=DocSecondHandPurches.DocSecondHandPurchID " +
                "ORDER BY rt.DocSecondHandRetailReturnTabID DESC " +
                "LIMIT 1 " +
                "), " +

                "DirWarehouseIDPurches=DocSecondHandPurches.DirWarehouseID";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //2. === === === === === === === === === === === === === === === === ===

            #region д1) По партиям:


            #region - Если Точка партии НЕ совпадает с аппаратом, то менять Точку аппарата на Точку партии
            //Может быть много партий для одного аппарата - это перемещения
            //Нужно партию на которой есть остаток

            SQL =
                "UPDATE DocSecondHandPurches "+
                "SET DirWarehouseID = " +
                "IFNULL " +
                "( " +
                " ( " +
                "  SELECT DirWarehouseID " +
                "  FROM Rem2Parties " +
                "  WHERE(Rem2Parties.DocIDFirst = DocSecondHandPurches.DocID) " +
                "  ORDER BY Rem2PartyID DESC " +
                "  LIMIT 1 " +
                " ), " +
                "  DocSecondHandPurches.DirWarehouseID " +
                ")";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg));

            #endregion


            #region - Нет остатка - менять статус на "Продан (10)"

            SQL =
                "UPDATE DocSecondHandPurches " +
                "SET DirSecondHandStatusID = " +
                "IFNULL " +
                "( " +

                " ( " +
                "  SELECT " +

                "  10 " + 
                //"  CASE Remnant " +
                //"  WHEN 0 THEN 10 " +
                //"  ELSE DocSecondHandPurches.DirSecondHandStatusID " +
                //"  END " +

                "  FROM Rem2Parties " +
                "  WHERE(Rem2Parties.DocIDFirst=DocSecondHandPurches.DocID)and(Rem2Parties.Remnant=0) " +
                "  ORDER BY Rem2PartyID DESC " +
                "  LIMIT 1 " +
                " ), " +
                "  DocSecondHandPurches.DirSecondHandStatusID " +
                ")";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg));

            #endregion


            #region - Есть остаток - менять статус на "В продаже (9 - Выдан)"

            SQL =
                "UPDATE DocSecondHandPurches " +
                "SET DirSecondHandStatusID = " +
                "IFNULL " +
                "( " +

                " ( " +
                "  SELECT " +

                "  9 " +
                //"  CASE Remnant " +
                //"  WHEN 1 THEN 9 " +
                //"  ELSE DocSecondHandPurches.DirSecondHandStatusID " +
                //"  END " + 

                "  FROM Rem2Parties " +
                "  WHERE(Rem2Parties.DocIDFirst=DocSecondHandPurches.DocID)and(Rem2Parties.Remnant>0) " +
                "  ORDER BY Rem2PartyID DESC " +
                "  LIMIT 1 " +
                " ), " +
                "  DocSecondHandPurches.DirSecondHandStatusID " +
                ")";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg));

            #endregion


            #endregion


            #region д2) Только "Лог" (т.к. все операции со статусами уже сделаны выше)


            #region д21) Перемещения
            //Находим все перемещения по каждому аппарату и пишем в Лог о них

            SQL =

                //, DirSecondHandStatusID
                "INSERT INTO LogSecondHands (DocSecondHandPurchID, DirSecondHandLogTypeID, DirEmployeeID, LogSecondHandDate, DirWarehouseIDFrom, DirWarehouseIDTo) " +

                //Все перемещения
                //Rem2Parties.DocIDFirst, DocSecondHandMovements.DirWarehouseIDFrom, DocSecondHandMovements.DirWarehouseIDTo
                "SELECT DocSecondHandPurches.DocSecondHandPurchID, 12, Docs.DirEmployeeID, Docs.DocDate, DocSecondHandMovements.DirWarehouseIDFrom, DocSecondHandMovements.DirWarehouseIDTo " +
                "FROM Docs, DocSecondHandMovements, DocSecondHandMovementTabs, Rem2Parties, DocSecondHandPurches " +
                "WHERE " +
                " (Docs.DocID=DocSecondHandMovements.DocID)and" + 
                " (DocSecondHandMovements.DocSecondHandMovementID=DocSecondHandMovementTabs.DocSecondHandMovementID)and" +
                " (DocSecondHandMovementTabs.Rem2PartyID=Rem2Parties.Rem2PartyID)and" +
                " (DocSecondHandPurches.DocID=Rem2Parties.DocIDFirst)" +
                "ORDER BY Rem2Parties.Rem2PartyID";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg));

            #endregion


            #region д22) Вовзраты
            //Находим все возвраты по каждому аппарату и пишем в Лог о них

            SQL =

                //, DirSecondHandStatusID
                "INSERT INTO LogSecondHands (DocSecondHandPurchID, DirSecondHandLogTypeID, DirEmployeeID, LogSecondHandDate) " +

                //Все перемещения
                "SELECT DocSecondHandPurches.DocSecondHandPurchID, 14, Docs.DirEmployeeID, Docs.DocDate " +
                "FROM Docs, DocSecondHandRetailReturns, DocSecondHandRetailReturnTabs, Rem2PartyMinuses, Rem2Parties, DocSecondHandPurches " +
                "WHERE " +
                " (Docs.DocID=DocSecondHandRetailReturns.DocID)and " +
                " (DocSecondHandRetailReturns.DocSecondHandRetailReturnID=DocSecondHandRetailReturnTabs.DocSecondHandRetailReturnID)and " +
                " (DocSecondHandRetailReturnTabs.Rem2PartyMinusID=Rem2PartyMinuses.Rem2PartyMinusID)and " +
                " (Rem2PartyMinuses.Rem2PartyID=Rem2Parties.Rem2PartyID)and " +
                " (DocSecondHandPurches.DocID=Rem2Parties.DocIDFirst) " +
                "ORDER BY Rem2Parties.Rem2PartyID ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg));

            #endregion


            #region д23) Продан
            //Если у аппарата статус 10, то записать об этом в Логе

            SQL =

                //, DirSecondHandStatusID
                "INSERT INTO LogSecondHands (DocSecondHandPurchID, DirSecondHandLogTypeID, DirEmployeeID, LogSecondHandDate) " +

                //Все перемещения
                "SELECT DocSecondHandPurches.DocSecondHandPurchID, 13, Docs.DirEmployeeID, Docs.DocDate " +
                "FROM Docs, DocSecondHandRetails, DocSecondHandRetailTabs, Rem2Parties, DocSecondHandPurches " +
                "WHERE " +
                " (Docs.DocID=DocSecondHandRetails.DocID)and" +
                " (DocSecondHandRetails.DocSecondHandRetailID=DocSecondHandRetailTabs.DocSecondHandRetailID)and" +
                " (DocSecondHandRetailTabs.Rem2PartyID=Rem2Parties.Rem2PartyID)and" +
                " (DocSecondHandPurches.DocID=Rem2Parties.DocIDFirst)" +
                "ORDER BY Rem2Parties.Rem2PartyID";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg));

            #endregion


            #endregion




            //3. === === === === === === === === === === === === === === === === ===

            //Перемещение
            #region DocSecondHandMovs (Создание таблицы Продажа)


            #region DocSecondHandMovs

            SQL =
            "CREATE TABLE [DocSecondHandMovs]( \n" +
            "  [DocSecondHandMovID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            "  [DocID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovs_DocID] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirWarehouseIDFrom] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovs_DirWarehouseIDFrom] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirWarehouseIDTo] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovs_DirWarehouseIDTo] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [Reserve] BOOL NOT NULL DEFAULT 1, \n" +
            "  [DirMovementDescriptionID] INTEGER CONSTRAINT[FK_DocSecondHandMovs_DirMovementDescriptionID] REFERENCES[DirMovementDescriptions]([DirMovementDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirEmployeeIDCourier] INTEGER CONSTRAINT[FK_DocSecondHandMovs_DirEmployeeID] REFERENCES[DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirMovementStatusID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovs_DirMovementStatusID] REFERENCES[DirMovementStatuses]([DirMovementStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED DEFAULT 0 \n" +
            "); \n" +

            "CREATE INDEX [IDX_DocSecondHandMovs_DirEmployeeID] ON[DocSecondHandMovs] ([DirEmployeeIDCourier]); " +
            "CREATE INDEX [IDX_DocSecondHandMovs_DirMovementStatusID] ON[DocSecondHandMovs] ([DirMovementStatusID]); " +
            "CREATE INDEX [IDX_DocSecondHandMovs_DirEmployeeIDCourier] ON[DocSecondHandMovs] ([DirEmployeeIDCourier]); " +
            "CREATE INDEX [IDX_DocSecondHandMovs_DocID] ON[DocSecondHandMovs] ([DocID]); " +


            "CREATE TABLE [DocSecondHandMovTabs]( \n" +
            "  [DocSecondHandMovTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            "  [DocSecondHandMovID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovTabs_DocSecondHandMovID] REFERENCES[DocSecondHandMovs]([DocSecondHandMovID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE, \n" +
            "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandRetailTab_DirServiceNomenID] REFERENCES[DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DocSecondHandPurchID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandMovTabs_DocSecondHandPurchID] REFERENCES [DocSecondHandPurches]([DocSecondHandPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n\n" +
            //"  [Quantity] REAL NOT NULL, \n" +
            "  [PriceVAT] REAL NOT NULL, \n" +
            "  [PriceCurrency] REAL NOT NULL, \n" +
            "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandMovTabs_DirCurrencyID] REFERENCES[DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirCurrencyRate] REAL NOT NULL, \n" +
            "  [DirCurrencyMultiplicity] INTEGER NOT NULL, \n" +
            "  [PriceRetailVAT] REAL NOT NULL, \n" +
            "  [PriceRetailCurrency] REAL NOT NULL, \n" +
            "  [PriceWholesaleVAT] REAL NOT NULL, \n" +
            "  [PriceWholesaleCurrency] REAL NOT NULL, \n" +
            "  [PriceIMVAT] REAL NOT NULL, \n" +
            "  [PriceIMCurrency] REAL NOT NULL, \n" +
            "  [DirReturnTypeID] INTEGER CONSTRAINT[FK_DocSecondHandMovTabs_DirReturnTypeID] REFERENCES[DirReturnTypes]([DirReturnTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirDescriptionID] INTEGER CONSTRAINT[FK_DocSecondHandMovTabs_DirDescriptionID] REFERENCES[DirDescriptions]([DirDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED \n" +
            "); " +

            "CREATE INDEX [IDX_DocSecondHandMovTabs_DirServiceNomenID] ON[DocSecondHandMovTabs] ([DirServiceNomenID]); " +
            "CREATE INDEX [IDX_DocSecondHandMovTabs_DocSecondHandPurchID] ON[DocSecondHandMovTabs] ([DocSecondHandPurchID]); " +
            "CREATE INDEX [IDX_DocSecondHandMovTabs_DocSecondHandMovID] ON[DocSecondHandMovTabs] ([DocSecondHandMovID]); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region DocSecondHandMovements => DocSecondHandMovs, DocSecondHandMovementTabs => DocSecondHandMovTabs (Перенос проданных аппаратов)

            SQL =
                //DocSecondHandMovs
                "INSERT INTO DocSecondHandMovs " +
                "(DocSecondHandMovID, DocID, DirWarehouseIDFrom, DirWarehouseIDTo, Reserve, DirMovementDescriptionID, DirEmployeeIDCourier, DirMovementStatusID) " +

                "SELECT DocSecondHandMovementID, DocID, DirWarehouseIDFrom, DirWarehouseIDTo, Reserve, DirMovementDescriptionID, DirEmployeeIDCourier, DirMovementStatusID " +
                "FROM DocSecondHandMovements; " +

                //DocSecondHandMovTabs
                "INSERT INTO DocSecondHandMovTabs " +
                "(DocSecondHandMovTabID, DocSecondHandMovID, DirServiceNomenID, DocSecondHandPurchID, PriceVAT, PriceCurrency, DirCurrencyID, DirCurrencyRate, DirCurrencyMultiplicity, PriceRetailVAT, PriceRetailCurrency, PriceWholesaleVAT, PriceWholesaleCurrency, PriceIMVAT, PriceIMCurrency, DirReturnTypeID, DirDescriptionID) " +

                "SELECT " +
                " mt.DocSecondHandMovementTabID, mt.DocSecondHandMovementID, mt.DirServiceNomenID, pur.DocSecondHandPurchID, mt.PriceVAT, mt.PriceCurrency, mt.DirCurrencyID, mt.DirCurrencyRate, mt.DirCurrencyMultiplicity, mt.PriceRetailVAT, mt.PriceRetailCurrency, mt.PriceWholesaleVAT, mt.PriceWholesaleCurrency, mt.PriceIMVAT, mt.PriceIMCurrency, mt.DirReturnTypeID, mt.DirDescriptionID " +
                "FROM DocSecondHandMovementTabs mt, Rem2Parties AS rp, DocSecondHandPurches AS pur " +
                "WHERE " +
                " mt.Rem2PartyID=rp.Rem2PartyID and" +
                " pur.DocID=rp.DocIDFirst ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion
                                  

            #endregion



            //4. === === === === === === === === === === === === === === === === ===

            //Продажа БУ
            #region DocSecondHandSales (Создание таблицы Продажа)


            #region DocSecondHandSales

            SQL =
            "CREATE TABLE [DocSecondHandSales] \n" +
            "( \n" +
            " [DocSecondHandSaleID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
            " [DocID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandSales_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DocSecondHandPurchID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandSales_DocSecondHandPurchID] REFERENCES [DocSecondHandPurches]([DocSecondHandPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandSales_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandSales_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [PriceVAT] REAL NOT NULL, \n" +
            " [PriceCurrency] REAL NOT NULL, \n" +
            " [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandSales_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DirCurrencyRate] REAL NOT NULL DEFAULT 1, \n" +
            " [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1, \n" +
            " [DirReturnTypeID] INTEGER, \n" +
            " [DirDescriptionID] INTEGER \n" +
            "); " +

            "CREATE INDEX [IDX_DocSecondHandSales_DirWarehouseID] ON [DocSecondHandSales] ([DirWarehouseID]); " +
            "CREATE INDEX [IDX_DocSecondHandSales_DocID] ON [DocSecondHandSales] ([DocID]); " +
            "CREATE INDEX [IDX_DocSecondHandSales_DocSecondHandPurchID] ON[DocSecondHandSales] ([DocSecondHandPurchID]); " +
            "CREATE INDEX [IDX_DocSecondHandSales_DirServiceNomenID] ON [DocSecondHandSales] ([DirServiceNomenID]); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region DocSecondHandRetails => DocSecondHandSales (Перенос проданных аппаратов)

            SQL =
                "INSERT INTO DocSecondHandSales " +
                "(DocSecondHandSaleID, DocID, DocSecondHandPurchID, DirWarehouseID, DirServiceNomenID, PriceVAT, PriceCurrency, DirCurrencyID, DirCurrencyRate, DirCurrencyMultiplicity, DirReturnTypeID, DirDescriptionID) " + 
                
                "SELECT " +
                " r.DocSecondHandRetailID, r.DocID, pur.DocSecondHandPurchID, r.DirWarehouseID, rt.DirServiceNomenID, rt.PriceVAT, rt.PriceCurrency, rt.DirCurrencyID, rt.DirCurrencyRate, rt.DirCurrencyMultiplicity, rt.DirReturnTypeID, rt.DirDescriptionID " +

                "FROM DocSecondHandRetails AS r, DocSecondHandRetailTabs AS rt, Rem2Parties AS rp, DocSecondHandPurches AS pur " + 

                "WHERE " +
                " r.DocSecondHandRetailID=rt.DocSecondHandRetailID and" +
                " rt.Rem2PartyID=rp.Rem2PartyID and" +
                " pur.DocID=rp.DocIDFirst ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #endregion

            //Возврат БУ
            #region DocSecondHandReturns (Создание таблицы Возврат)


            #region DocSecondHandReturns

            SQL =
            "CREATE TABLE [DocSecondHandReturns] \n" +
            "( \n" +
            " [DocSecondHandReturnID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT CONSTRAINT [FK_DocSecondHandReturns_DocID] REFERENCES [DocSecondHandReturns]([DocSecondHandReturnID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DocID] INTEGER NOT NULL, \n" +
            " [DocSecondHandSaleID] INTEGER CONSTRAINT [FK_DocSecondHandReturns_DocSecondHandSaleID] REFERENCES [DocSecondHandSales]([DocSecondHandSaleID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DocSecondHandPurchID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandSales_DocSecondHandPurchID] REFERENCES [DocSecondHandPurches]([DocSecondHandPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandReturns_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandReturns_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [PriceVAT] REAL NOT NULL, \n" +
            " [PriceCurrency] REAL NOT NULL, \n" +
            " [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandReturns_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DirCurrencyRate] REAL NOT NULL, \n" +
            " [DirCurrencyMultiplicity] INTEGER NOT NULL, \n" +
            " [DirReturnTypeID] INTEGER CONSTRAINT [FK_DocSecondHandReturns_DirReturnTypeID] REFERENCES [DirPriceTypes]([DirPriceTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            " [DirDescriptionID] INTEGER CONSTRAINT [FK_DocSecondHandReturns_DirDescriptionID] REFERENCES [DirDescriptions]([DirDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED \n" +
            "); " +

            "CREATE INDEX [IDX_DocSecondHandReturns_DocID] ON [DocSecondHandReturns] ([DocID]); " +
            "CREATE INDEX [IDX_DocSecondHandReturns_DirWarehouseID] ON [DocSecondHandReturns] ([DirWarehouseID]); " +
            "CREATE INDEX [IDX_DocSecondHandReturns_DocSecondHandPurchID] ON[DocSecondHandReturns] ([DocSecondHandPurchID]); " +
            "CREATE INDEX [IDX_DocSecondHandReturns_DirServiceNomenID] ON [DocSecondHandReturns] ([DirServiceNomenID]); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region DocSecondHandRetailReturns => DocSecondHandReturns (Перенос возвращённых аппаратов)

            SQL =
                "INSERT INTO DocSecondHandReturns " +
                "(DocID, DocSecondHandSaleID, DocSecondHandPurchID, DirWarehouseID, DirServiceNomenID, PriceVAT, PriceCurrency, DirCurrencyID, DirCurrencyRate, DirCurrencyMultiplicity, DirReturnTypeID, DirDescriptionID) " +

                "SELECT " +
                " r.DocID, r.DocSecondHandRetailID, pur.DocSecondHandPurchID, r.DirWarehouseID, rt.DirServiceNomenID, rt.PriceVAT, rt.PriceCurrency, rt.DirCurrencyID, rt.DirCurrencyRate, rt.DirCurrencyMultiplicity, rt.DirReturnTypeID, rt.DirDescriptionID " +

                "FROM DocSecondHandRetailReturns AS r, DocSecondHandRetailReturnTabs AS rt, Rem2PartyMinuses AS rpm, Rem2Parties AS rp, DocSecondHandPurches AS pur " +

                "WHERE " +
                " r.DocSecondHandRetailReturnID=rt.DocSecondHandRetailReturnID and" +
                " rt.Rem2PartyMinusID=rpm.Rem2PartyMinusID and" +
                " rpm.Rem2PartyID=rp.Rem2PartyID and" +
                " pur.DocID=rp.DocIDFirst ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #endregion



            //5. === === === === === === === === === === === === === === === === ===

            //Инвентаризация БУ
            #region DocSecondHandInvs (Создание таблицы Инвентаризация)
            
            SQL =
            "CREATE TABLE [DocSecondHandInvs]( " +
            "  [DocSecondHandInvID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT CONSTRAINT[FK_DocSecondHandInvs_DocID] REFERENCES[DocSecondHandInvs]([DocSecondHandInvID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DocID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandInvs_DocID] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandInvs_DirWarehouseID] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [LoadFrom] INTEGER NOT NULL DEFAULT 1, " + //[Продажа, Продажа + ППП На разбор]
            "  [SpisatS] INTEGER NOT NULL DEFAULT 1, " +
            "  [SpisatSDirEmployeeID] INTEGER CONSTRAINT[FK_DocSecondHandInvTabs_SpisatSDirEmployeeID] REFERENCES[DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED " +
            "); " +

            "CREATE INDEX [IDX_DocSecondHandInvs_DirWarehouseID] ON[DocSecondHandInvs] ([DirWarehouseID]); " +
            "CREATE INDEX [IDX_DocSecondHandInvs_DocID] ON[DocSecondHandInvs] ([DocID]); " +


            "CREATE TABLE [DocSecondHandInvTabs] ( " +
            "  [DocSecondHandInvTabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
            "  [DocSecondHandInvID] INTEGER NOT NULL CONSTRAINT[FK_DocSecondHandInvTabs_DocSecondHandInvID] REFERENCES[DocSecondHandInvs]([DocSecondHandInvID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DocSecondHandPurchID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandInvTabs_DocSecondHandPurchID] REFERENCES [DocSecondHandPurches]([DocSecondHandPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [PriceVAT] REAL NOT NULL, " +
            "  [PriceCurrency] REAL NOT NULL, " +
            "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRetailTab_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1, " +
            "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1, " +
            "  [Exist] INTEGER NOT NULL DEFAULT 1, " +
            "  [DirSecondHandStatusID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandInvTabs_DirSecondHandStatusID] REFERENCES [DirSecondHandStatuses]([DirSecondHandStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED " +
            "); " +

            "CREATE INDEX [IDX_DocSecondHandInvTabs_DirServiceNomenID] ON [DocSecondHandInvTabs] ([DirServiceNomenID]); " +
            "CREATE INDEX [IDX_DocSecondHandInvTabs_DocSecondHandInvID] ON [DocSecondHandInvTabs] ([DocSecondHandInvID]); " +
            "CREATE INDEX [IDX_DocSecondHandInvTabs_DocSecondHandPurchID] ON[DocSecondHandInvTabs] ([DocSecondHandPurchID]); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //6. === === === === === === === === === === === === === === === === ===

            //Разбор БУ - только табличная часть
            #region DocSecondHandRazbor2Tabs (Создание таблицы Разбор)

            SQL =

            "CREATE TABLE [DocSecondHandRazbor2Tabs] ( " +
            "  [DocSecondHandRazbor2TabID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
            "  [DocSecondHandPurchID] INTEGER NOT NULL CONSTRAINT [FK_DocSecondHandRazbor2Tabs_DocSecondHandPurchID] REFERENCES [DocSecondHandPurches]([DocSecondHandPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
            "  [DirNomenID] INTEGER NOT NULL CONSTRAINT[FK_DocPurchTabs_DirNomenID] REFERENCES[DirNomens] ([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCharColourID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharColourID] REFERENCES[DirCharColours] ([DirCharColourID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCharMaterialID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharMaterialID] REFERENCES[DirCharMaterials] ([DirCharMaterialID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCharNameID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharNameID] REFERENCES[DirCharNames] ([DirCharNameID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCharSeasonID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharSeasonID] REFERENCES[DirCharSeasons] ([DirCharSeasonID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCharSexID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharSexID] REFERENCES[DirCharSexes] ([DirCharSexID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCharSizeID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharSizeID] REFERENCES[DirCharSizes] ([DirCharSizeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCharStyleID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharStyleID] REFERENCES[DirCharStyles] ([DirCharStyleID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCharTextureID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirCharTextureID] REFERENCES[DirCharTextures] ([DirCharTextureID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [SerialNumber] TEXT(256), " +
            "  [Barcode] TEXT(256), " +
            "  [Quantity] REAL NOT NULL, " +
            "  [PriceVAT] REAL NOT NULL, " +
            "  [PriceCurrency] REAL NOT NULL, " +
            "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT[FK_DocPurchTabs_DirCurrencyID] REFERENCES[DirCurrencies] ([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
            "  [DirCurrencyRate] REAL NOT NULL, " +
            "  [DirCurrencyMultiplicity] INTEGER NOT NULL, " +
            "  [PriceRetailVAT] REAL NOT NULL, " +
            "  [PriceRetailCurrency] REAL NOT NULL, " +
            "  [PriceWholesaleVAT] REAL NOT NULL, " +
            "  [PriceWholesaleCurrency] REAL NOT NULL, " +
            "  [PriceIMVAT] REAL NOT NULL, " +
            "  [PriceIMCurrency] REAL NOT NULL, " +
            "  [DirNomenMinimumBalance] REAL DEFAULT 0, " +
            "  [DirCharStyleName] TEXT(256), " +
            "  [DirContractorID] INTEGER CONSTRAINT[FK_DocPurchTabs_DirContractorID] REFERENCES[DirContractors] ([DirContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED " +
            ");" +

            "CREATE INDEX [IDX_DocSecondHandRazbor2Tabs_DocSecondHandPurchID] ON[DocSecondHandRazbor2Tabs] ([DocSecondHandPurchID]); " +
            "CREATE INDEX [IDX_DocSecondHandRazbor2Tabs_DirNomenID] ON[DocSecondHandRazbor2Tabs] ([DirNomenID]); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));
            
            #endregion







            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 64;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update65: DirNomens и DirServiceNomens: удаление повторяющихся пробелов

        private async Task<bool> Update65(DbConnectionSklad db)
        {
            #region DirNomens: DirNomenName, NameLower, DirNomenNameFull, NameFullLower

            var queryDirNomens = await
                (
                    from x in db.DirNomens
                    select x
                ).ToListAsync();

            int DirNomenID = 0;
            string DirNomenName, NameLower, DirNomenNameFull, NameFullLower;
            for (int i = 0; i < queryDirNomens.Count(); i++)
            {
                DirNomenName = Regex.Replace(queryDirNomens[i].DirNomenName, @"\s+", " ");  
                NameLower = Regex.Replace(queryDirNomens[i].DirNomenName.Trim(), @"\s+", " ");
                DirNomenNameFull = Regex.Replace(queryDirNomens[i].DirNomenNameFull.Trim(), @"\s+", " ");
                NameFullLower = Regex.Replace(queryDirNomens[i].NameFullLower.Trim(), @"\s+", " ");


                DirNomenID = Convert.ToInt32(queryDirNomens[i].DirNomenID);
                Models.Sklad.Dir.DirNomen dirNomen = await db.DirNomens.FindAsync(DirNomenID);
                dirNomen.DirNomenName = DirNomenName;
                dirNomen.NameLower = NameLower;
                dirNomen.DirNomenNameFull = DirNomenNameFull;
                dirNomen.NameFullLower = NameFullLower;

                db.Entry(dirNomen).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            #endregion


            #region DirServiceNomens: DirNomenName, NameLower, DirNomenNameFull, NameFullLower

            var queryDirServiceNomens = await
                (
                    from x in db.DirServiceNomens
                    select x
                ).ToListAsync();

            int DirServiceNomenID = 0;
            for (int i = 0; i < queryDirServiceNomens.Count(); i++)
            {
                DirServiceNomenID = Convert.ToInt32(queryDirServiceNomens[i].DirServiceNomenID);

                Models.Sklad.Dir.DirServiceNomen dirServiceNomen = await db.DirServiceNomens.FindAsync(DirServiceNomenID);
                dirServiceNomen.DirServiceNomenName = queryDirServiceNomens[i].DirServiceNomenName.Trim();
                dirServiceNomen.NameLower = queryDirServiceNomens[i].NameLower.Trim();
                dirServiceNomen.DirServiceNomenNameFull = queryDirServiceNomens[i].DirServiceNomenNameFull.Trim();
                dirServiceNomen.NameFullLower = queryDirServiceNomens[i].NameFullLower.Trim();

                db.Entry(dirServiceNomen).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 65;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update66: DocSecondHandRazbor2Tabs ADD DirNomen1ID, DirNomen2ID, DirNomenCategoryID

        private async Task<bool> Update66(DbConnectionSklad db)
        {
            //Добавление поля
            string SQL =
                "ALTER TABLE DocSecondHandRazbor2Tabs ADD COLUMN [DirNomen1ID] INTEGER CONSTRAINT[FK_DocSecondHandRazbor2Tabs_DirNomen1ID] REFERENCES[DirNomens] ([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "ALTER TABLE DocSecondHandRazbor2Tabs ADD COLUMN [DirNomen2ID] INTEGER CONSTRAINT[FK_DocSecondHandRazbor2Tabs_DirNomen2ID] REFERENCES[DirNomens] ([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "ALTER TABLE DocSecondHandRazbor2Tabs ADD COLUMN [DirNomenCategoryID] INTEGER CONSTRAINT [FK_DocSecondHandRazbor2Tabs_DirNomenCategoryID] REFERENCES [DirNomenCategories]([DirNomenCategoryID]) ON DELETE SET NULL ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 66;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update67: Add INDEX in DocSecondHandPurches

        private async Task<bool> Update67(DbConnectionSklad db)
        {
            //Добавление индекса
            string SQL = "CREATE INDEX[IDX_DocSecondHandPurches_DocIDService] ON[DocSecondHandPurches]([DocIDService] COLLATE[NOCASE] ASC);";
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 67;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update68: RightDocServicePurchesWarehouseAllCheck

        private async Task<bool> Update68(DbConnectionSklad db)
        {
            #region 1. Права: RightDocServicePurchesWarehouseAllCheck

            string SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServicePurchesWarehouseAllCheck] BOOL DEFAULT 1; " +
                "UPDATE DirEmployees SET RightDocServicePurchesWarehouseAllCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 68;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update69: RightDocServicePurchesWarehouseAllCheck

        private async Task<bool> Update69(DbConnectionSklad db)
        {
            #region 1. Права: RightDocServicePurchesWarehouseAllCheck

            string SQL =
                "ALTER TABLE DirEmployeeWarehouses ADD COLUMN [WarehouseAll] BOOL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 69;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update70: DirWarehouses (SalaryPercent4Second, SalaryPercent5Second, SalaryPercent6Second)

        private async Task<bool> Update70(DbConnectionSklad db)
        {
            string SQL = "";

            //1.1. DocServicePurch1Tabs.DocCashOfficeSums
            SQL =
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercent4Second] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercent5Second] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercent6Second] REAL NOT NULL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));
            



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 70;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update71: DirWarehouses (SalaryPercent7Second, SalaryPercent8Second) + SysSettings.DocSecondHandSalesDiscount

        private async Task<bool> Update71(DbConnectionSklad db)
        {
            string SQL =
                //Добавить 4-е поле в справоник Точки: 
                //"добавляем % от стоимости аппарата"
                //учесть это поле в подсчёте ЗП
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercent7Second] REAL NOT NULL DEFAULT 0; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SalaryPercent8Second] REAL NOT NULL DEFAULT 0; " +
                //Разрешить скидку в для комиссионных БУ аппаратов
                "ALTER TABLE SysSettings ADD COLUMN [DocSecondHandSalesDiscount] BOOL NOT NULL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));
            


            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 71;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update72: ListObjectFieldNames.DateVIDACHI

        private async Task<bool> Update72(DbConnectionSklad db)
        {

            #region ListObjectFieldNames

            string SQL =
                "INSERT INTO ListObjectFieldNames " +
                "(ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)" +
                "values" +
                "(396, 'DateVIDACHI', 'Документ.СервисныйЦентр.ДатаВыдачи', 'Документ.СервіснийЦентр.ДатаВидачі'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));
            #endregion

            #region ListObjectFields

            SQL =
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(40, 1, 0, 1, 396); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region ListObjectPFs


            string[] Fieldname = { "ListObjectPFHtmlFooter" };
            SQL = "SELECT ListObjectPFHtmlFooter FROM ListObjectPFs WHERE ListObjectPFID=35";
            ArrayList alField = await SelectDataFrom_EtalonDB(SQL, Fieldname);
            string[] mas = (string[])alField[0];



            SQL = "UPDATE ListObjectPFs SET ListObjectPFHtmlFooter = @ListObjectPFHtmlFooter WHERE ListObjectPFID=35";

            SQLiteParameter parListObjectPFHtmlFooter = new SQLiteParameter("@ListObjectPFHtmlFooter", System.Data.DbType.String) { Value = mas[0] };

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlFooter));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 72;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update73: СЦ.Перемещение

        private async Task<bool> Update73(DbConnectionSklad db)
        {
            //0. Подготовка === === === === === === === === === === === === === === === === ===

            #region а) В "LogServices" добавить 2-а поля "С точки" и "На точку""

            string SQL =
                "ALTER TABLE LogServices ADD COLUMN [DirWarehouseIDFrom] INTEGER; " +
                "ALTER TABLE LogServices ADD COLUMN [DirWarehouseIDTo] INTEGER; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region б) Ввести новые статусы для аппарата "DirServiceStatuses": 

            SQL =
                "INSERT INTO DirServiceStatuses (DirServiceStatusID, DirServiceStatusName, SortNum)values(10, 'В разборе', 10); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #region в) Ввести новые типы для Лога "DirServiceLogTypes"

            SQL =
                "INSERT INTO DirServiceLogTypes (DirServiceLogTypeID, DirServiceLogTypeName)values(12, 'Перемещён на другую точку'); " +
                "INSERT INTO DirServiceLogTypes (DirServiceLogTypeID, DirServiceLogTypeName)values(13, 'Логистика (Перемещение)'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region г) Найти в Логе все статусы == 1 (LogServiceRazbors.DirServiceLogTypeID == 1) и в поле "Msg" записать на какую точку принят аппарат

            SQL =

                "UPDATE LogServices " +
                "SET " +

                "Msg='Аппарат принят на точку №' ||  " +
                "(" +
                " SELECT DirWarehouseID " +
                " FROM DocServicePurches " +
                " WHERE DocServicePurches.DocServicePurchID=LogServices.DocServicePurchID " +
                " LIMIT 1 " +
                ")" +

                ", " +

                "DirWarehouseIDFrom=" +
                "(" +
                " SELECT DirWarehouseID " +
                " FROM DocServicePurches " +
                " WHERE DocServicePurches.DocServicePurchID=LogServices.DocServicePurchID " +
                " LIMIT 1 " +
                ")" +

                "WHERE LogServices.DirServiceLogTypeID=1";

            SQLiteParameter parMsg = new SQLiteParameter("@Msg", System.Data.DbType.String) { Value = "Аппарат принят на точку ..." };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg));

            #endregion




            #region DocServiceMovs

            SQL =

                "CREATE TABLE [DocServiceMovs]( \n" +
                "  [DocServiceMovID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [DocID] INTEGER NOT NULL CONSTRAINT[FK_DocServiceMovs_DocID] REFERENCES[Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirWarehouseIDFrom] INTEGER NOT NULL CONSTRAINT[FK_DocServiceMovs_DirWarehouseIDFrom] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirWarehouseIDTo] INTEGER NOT NULL CONSTRAINT[FK_DocServiceMovs_DirWarehouseIDTo] REFERENCES[DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [Reserve] BOOL NOT NULL DEFAULT 1, \n" +
                "  [DirMovementDescriptionID] INTEGER CONSTRAINT[FK_DocServiceMovs_DirMovementDescriptionID] REFERENCES[DirMovementDescriptions]([DirMovementDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirEmployeeIDCourier] INTEGER CONSTRAINT[FK_DocServiceMovs_DirEmployeeID] REFERENCES[DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirMovementStatusID] INTEGER NOT NULL CONSTRAINT[FK_DocServiceMovs_DirMovementStatusID] REFERENCES[DirMovementStatuses]([DirMovementStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED DEFAULT 0 \n" +
                "); " +

                "CREATE INDEX [IDX_DocServiceMovs_DirMovementStatusID] ON[DocServiceMovs] ([DirMovementStatusID]);" +
                "CREATE INDEX [IDX_DocServiceMovs_DocID] ON[DocServiceMovs] ([DocID]);" +
                "CREATE INDEX [IDX_DocServiceMovs_DirEmployeeID] ON[DocServiceMovs] ([DirEmployeeIDCourier]);" +
                "CREATE INDEX [IDX_DocServiceMovs_DirEmployeeIDCourier] ON[DocServiceMovs] ([DirEmployeeIDCourier]); " +


                "CREATE TABLE [DocServiceMovTabs]( \n" +
                "  [DocServiceMovTabID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, \n" +
                "  [DocServiceMovID] INTEGER NOT NULL CONSTRAINT [FK_DocServiceMovTabs_DocServiceMovID] REFERENCES [DocServiceMovs]([DocServiceMovID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE, \n" +
                "  [DirServiceNomenID] INTEGER NOT NULL CONSTRAINT [FK_DocServiceRetailTab_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DocServicePurchID] INTEGER NOT NULL CONSTRAINT [FK_DocServiceMovTabs_DocServicePurchID] REFERENCES [DocServicePurches]([DocServicePurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirServiceStatusID] INTEGER NOT NULL CONSTRAINT [FK_DocServiceMovTabs_DirServiceStatusID] REFERENCES [DirServiceStatuses]([DirServiceStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirServiceStatusID_789] INTEGER \n" +
                "); " +

                "CREATE INDEX [IDX_DocServiceMovTabs_DocServicePurchID] ON[DocServiceMovTabs] ([DocServicePurchID]); " +
                "CREATE INDEX [IDX_DocServiceMovTabs_DirServiceNomenID] ON[DocServiceMovTabs] ([DirServiceNomenID]); " +
                "CREATE INDEX [IDX_DocServiceMovTabs_DocServiceMovID] ON[DocServiceMovTabs] ([DocServiceMovID]); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Добавление полей

            SQL = "ALTER TABLE DocServicePurches ADD COLUMN [DirWarehouseIDPurches] INTEGER CONSTRAINT [FK_DocServicePurches_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));


            SQL = "UPDATE DocServicePurches SET DirWarehouseIDPurches=DocServicePurches.DirWarehouseID";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            #region Права: RightDocServiceMovements

            SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServiceMovements] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServiceMovementsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocServiceMovements=1, RightDocServiceMovementsCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Права: ListObjects

            SQL =
                "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(78, 3, 'DocServiceMovements', 'Документ Перемещение СЦ'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion





            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 73;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update74: ПФ для 71 (БУ.Перемещение)

        private async Task<bool> Update74(DbConnectionSklad db)
        {

            #region ListObjectFields

            //ListObjectFields
            string SQL =
                "INSERT INTO ListObjectFields " +
                "(ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +

                "SELECT 71, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID " +
                "FROM ListObjectFields " +
                "WHERE ListObjectID=33 and ListObjectFieldTabShow=0 and ListObjectFieldNameID<>78; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));





            SQL =

                "INSERT INTO ListObjectFieldNames " +
                "(ListObjectFieldNameID, ListObjectFieldNameReal, ListObjectFieldNameRu, ListObjectFieldNameUa)" +
                "values" +
                "(397, 'DocSecondHandMovID', 'Документ.БУПеремещение.НомерВнутренний', 'Документ.БЖПереміщення.НомерВнутрішній'); " +


                "INSERT INTO ListObjectFields " +
                "(ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values"+
                "(71, 1, 0, 0, 397)";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            #region Табличная часть

            SQL =
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 361);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 362);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 42);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 359);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 96);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 38);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 35);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 36);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 167);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 93);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 289);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 288);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 107);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 106);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 44);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 95);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 111);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 110);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 45);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 97);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 113);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 112);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 338);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 339);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 340);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 341);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 144);" +

                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)" +
                "values" +
                "(71, 0, 1, 0, 85);";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion

            #endregion


            #region ListObjectPFs

            SQL =
                "INSERT INTO ListObjectPFs " +
                "(ListObjectPFID, ListObjectID, ListLanguageID, ListObjectPFHtmlCSSUse, ListObjectPFSys, ListObjectPFName, ListObjectPFHtmlHeaderUse, ListObjectPFHtmlDouble, ListObjectPFHtmlHeader, ListObjectPFHtmlTabUseCap, ListObjectPFHtmlTabCap, ListObjectPFHtmlTabUseTab, ListObjectPFHtmlTabEnumerate, ListObjectPFHtmlTabFont, ListObjectPFHtmlTabFontSize, ListObjectPFHtmlTabUseFooter, ListObjectPFHtmlTabFooter, ListObjectPFHtmlTabUseText, ListObjectPFHtmlTabText, ListObjectPFHtmlFooterUse, ListObjectPFHtmlFooter, ListObjectPFDesc, MarginTop, MarginBottom, MarginLeft, MarginRight) " +

                "SELECT 44, 71, ListLanguageID, ListObjectPFHtmlCSSUse, ListObjectPFSys, ListObjectPFName, ListObjectPFHtmlHeaderUse, ListObjectPFHtmlDouble, ListObjectPFHtmlHeader, ListObjectPFHtmlTabUseCap, ListObjectPFHtmlTabCap, ListObjectPFHtmlTabUseTab, ListObjectPFHtmlTabEnumerate, ListObjectPFHtmlTabFont, ListObjectPFHtmlTabFontSize, ListObjectPFHtmlTabUseFooter, ListObjectPFHtmlTabFooter, ListObjectPFHtmlTabUseText, ListObjectPFHtmlTabText, ListObjectPFHtmlFooterUse, ListObjectPFHtmlFooter, ListObjectPFDesc, MarginTop, MarginBottom, MarginLeft, MarginRight " +
                "FROM ListObjectPFs " +
                "WHERE ListObjectPFID=15; " +


                "INSERT INTO ListObjectPFs " +
                "(ListObjectPFID, ListObjectID, ListLanguageID, ListObjectPFHtmlCSSUse, ListObjectPFSys, ListObjectPFName, ListObjectPFHtmlHeaderUse, ListObjectPFHtmlDouble, ListObjectPFHtmlHeader, ListObjectPFHtmlTabUseCap, ListObjectPFHtmlTabCap, ListObjectPFHtmlTabUseTab, ListObjectPFHtmlTabEnumerate, ListObjectPFHtmlTabFont, ListObjectPFHtmlTabFontSize, ListObjectPFHtmlTabUseFooter, ListObjectPFHtmlTabFooter, ListObjectPFHtmlTabUseText, ListObjectPFHtmlTabText, ListObjectPFHtmlFooterUse, ListObjectPFHtmlFooter, ListObjectPFDesc, MarginTop, MarginBottom, MarginLeft, MarginRight) " +

                "SELECT 45, 71, ListLanguageID, ListObjectPFHtmlCSSUse, ListObjectPFSys, ListObjectPFName, ListObjectPFHtmlHeaderUse, ListObjectPFHtmlDouble, ListObjectPFHtmlHeader, ListObjectPFHtmlTabUseCap, ListObjectPFHtmlTabCap, ListObjectPFHtmlTabUseTab, ListObjectPFHtmlTabEnumerate, ListObjectPFHtmlTabFont, ListObjectPFHtmlTabFontSize, ListObjectPFHtmlTabUseFooter, ListObjectPFHtmlTabFooter, ListObjectPFHtmlTabUseText, ListObjectPFHtmlTabText, ListObjectPFHtmlFooterUse, ListObjectPFHtmlFooter, ListObjectPFDesc, MarginTop, MarginBottom, MarginLeft, MarginRight " +
                "FROM ListObjectPFs " +
                "WHERE ListObjectPFID=16; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            
            #region Замена 1
            //[[[Документ.НакладнаяНаПеремещение.НомерВнутренний]]] => [[[Документ.БУПеремещение.НомерВнутренний]]]

            string[] Fieldname1 = { "ListObjectPFHtmlHeader" };
            SQL = "SELECT ListObjectPFHtmlHeader FROM ListObjectPFs WHERE ListObjectPFID=44";
            ArrayList alField1 = await SelectDataFrom_EtalonDB(SQL, Fieldname1);
            string[] mas = (string[])alField1[0];

            string ListObjectPFHtmlHeader = mas[0].Replace("Документ.НакладнаяНаПеремещение.НомерВнутренний", "Документ.БУПеремещение.НомерВнутренний");

            SQL = "UPDATE ListObjectPFs SET ListObjectPFHtmlHeader=@ListObjectPFHtmlHeader WHERE ListObjectPFID=44";
            SQLiteParameter parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = mas[0] };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader));

            #endregion


            #region Замена 2
            //[[[Документ.НакладнаяНаПеремещение.НомерВнутренний]]] => [[[Документ.БУПеремещение.НомерВнутренний]]]

            string[] Fieldname2 = { "ListObjectPFHtmlHeader" };
            SQL = "SELECT ListObjectPFHtmlHeader FROM ListObjectPFs WHERE ListObjectPFID=45";
            ArrayList alField2 = await SelectDataFrom_EtalonDB(SQL, Fieldname2);
            mas = (string[])alField2[0];

            ListObjectPFHtmlHeader = mas[0].Replace("Документ.НакладнаяНаПеремещение.НомерВнутренний", "Документ.БУПеремещение.НомерВнутренний");

            SQL = "UPDATE ListObjectPFs SET ListObjectPFHtmlHeader=@ListObjectPFHtmlHeader WHERE ListObjectPFID=45";
            parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = mas[0] };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader));
            
            #endregion
            

            #endregion


            #region ListObjectPFTabs

            SQL =
                "INSERT INTO ListObjectPFTabs " +
                "(ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID) " +
                "values" +
                "(44, 'Наименование', 361, 1);" +


                "INSERT INTO ListObjectPFTabs " +
                "(ListObjectPFID, ListObjectPFTabName, ListObjectFieldNameID, PositionID) " +
                "values" +
                "(45, 'Наименование', 361, 1)," +
                "(45, 'Цена', 95, 2);";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 74;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update75: DirSecondHandStatuses and DirSecondHandLogTypes

        private async Task<bool> Update75(DbConnectionSklad db)
        {
            #region Ввести новые статусы для аппарата "DirSecondHandStatuses": 

            string SQL =
                "INSERT INTO DirSecondHandStatuses (DirSecondHandStatusID, DirSecondHandStatusName, SortNum)values(15, 'Отсутствует', 14); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Ввести новые типы для Лога "DirSecondHandLogTypes"

            SQL =
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(22, 'Отсутствует'); " +
                "INSERT INTO DirSecondHandLogTypes (DirSecondHandLogTypeID, DirSecondHandLogTypeName)values(23, 'Вернули с Отсутствия (отмена проводки документа Инвентаризация)'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 75;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update76: DocServiceInvs

        private async Task<bool> Update76(DbConnectionSklad db)
        {
            string SQL =
                "ALTER TABLE DocSecondHandInvs ADD COLUMN [DirEmployee1ID] INTEGER CONSTRAINT [FK_DocSecondHandInvs_DirEmployee1ID] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED;" +
                "ALTER TABLE DocSecondHandInvs ADD COLUMN [DirEmployee1Podpis] BOOL DEFAULT 0;" +
                "ALTER TABLE DocSecondHandInvs ADD COLUMN [DirEmployee2ID] INTEGER CONSTRAINT [FK_DocSecondHandInvs_DirEmployee2ID] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED;" +
                "ALTER TABLE DocSecondHandInvs ADD COLUMN [DirEmployee2Podpis] BOOL DEFAULT 0;";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 76;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update77: DocSecondHandInvs.DirEmployee1ID

        private async Task<bool> Update77(DbConnectionSklad db)
        {
            string SQL =
                "UPDATE DocSecondHandInvs SET DirEmployee1ID=1 WHERE DirEmployee1ID IS null";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 77;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update78: DocServiceInvs

        private async Task<bool> Update78(DbConnectionSklad db)
        {
            //[LoadFrom] INTEGER NOT NULL DEFAULT 1;
            string SQL =
                "ALTER TABLE DocSecondHandMovs ADD COLUMN [LoadFrom] INTEGER NOT NULL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 78;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update79: DocDomesticExpenses.DirEmployeeIDSpisat

        private async Task<bool> Update79(DbConnectionSklad db)
        {
            //[LoadFrom] INTEGER NOT NULL DEFAULT 1;
            string SQL =
                "ALTER TABLE DocDomesticExpenses ADD COLUMN [DirEmployeeIDSpisat] INTEGER CONSTRAINT [FK_DocDomesticExpenses_DirEmployeeIDSpisat] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; " +
                "UPDATE DocDomesticExpenses SET DirEmployeeIDSpisat=1";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 79;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update80: Update DocSecondHandInvs

        private async Task<bool> Update80(DbConnectionSklad db)
        {
            //[LoadFrom] INTEGER NOT NULL DEFAULT 1;
            string SQL =
                "UPDATE DocSecondHandInvs SET LoadFrom=1 WHERE LoadFrom=0 or LoadFrom is NULL; " +
                "UPDATE DocSecondHandInvs SET SpisatS=1 WHERE SpisatS=0 or SpisatS is NULL; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 80;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update81: Update DocSecondHandInvs

        private async Task<bool> Update81(DbConnectionSklad db)
        {
            //[LoadFrom] INTEGER NOT NULL DEFAULT 1;
            string SQL =
                "ALTER TABLE DirWarehouses ADD COLUMN [SmenaClose] BOOL DEFAULT 0; " +
                "ALTER TABLE DirWarehouses ADD COLUMN [SmenaCloseTime] TEXT; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 81;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update82: DocCashOfficeSumMovements и DocCashOfficeSumMovementTabs

        private async Task<bool> Update82(DbConnectionSklad db)
        {
            string SQL = "";


            #region DocCashOfficeSumMovements

            //1. DocCashOfficeSumMovements
            SQL = "CREATE TABLE [DocCashOfficeSumMovements] ( \n" +
                "  [DocCashOfficeSumMovementID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, \n" +
                "  [DocID] INTEGER NOT NULL CONSTRAINT [FK_DocCashOfficeSumMovements_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +

                "  [DirWarehouseIDFrom] INTEGER NOT NULL CONSTRAINT [FK_DocCashOfficeSumMovements_DirWarehouseIDFrom] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCashOfficeIDFrom] INTEGER NOT NULL CONSTRAINT [FK_DocCashOfficeSumMovements_DirCashOfficeIDFrom] REFERENCES [DirCashOffices]([DirCashOfficeID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +

                "  [DirWarehouseIDTo] INTEGER NOT NULL CONSTRAINT [FK_DocCashOfficeSumMovements_DirWarehouseIDTo] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCashOfficeIDTo] INTEGER NOT NULL CONSTRAINT [FK_DocCashOfficeSumMovements_DirCashOfficeIDTo] REFERENCES [DirCashOffices]([DirCashOfficeID]) ON DELETE RESTRICT ON UPDATE RESTRICT MATCH FULL DEFERRABLE INITIALLY DEFERRED, \n" +


                "  [Reserve] BOOL NOT NULL DEFAULT 1, \n" +

                "  [Sums] REAL NOT NULL, \n" +
                "  [SumsCurrency] REAL NOT NULL, \n" +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocCashOfficeSumMovementTabs_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirCurrencyRate] REAL NOT NULL, \n" +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL, \n" +


                "  [DirEmployeeIDCourier] INTEGER CONSTRAINT [FK_DocCashOfficeSumMovements_DirEmployeeIDCourier] REFERENCES [DirEmployees]([DirEmployeeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DirMovementStatusID] INTEGER NOT NULL CONSTRAINT [FK_DocCashOfficeSumMovements_DirMovementStatusID] REFERENCES [DirMovementStatuses]([DirMovementStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED DEFAULT 0 \n" +
                "); " +

                "CREATE INDEX [IDX_DocCashOfficeSumMovements_DocID] ON [DocCashOfficeSumMovements] ([DocID]); " +
                "CREATE INDEX [IDX_DocCashOfficeSumMovements_DirMovementStatusID] ON [DocCashOfficeSumMovements] ([DirMovementStatusID]); " +
                "CREATE INDEX [IDX_DocCashOfficeSumMovements_DirEmployeeIDCourier] ON [DocCashOfficeSumMovements] ([DirEmployeeIDCourier]); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region ListObjects

            SQL = "INSERT INTO ListObjects (ListObjectID, ListObjectTypeID, ListObjectNameSys, ListObjectNameRu)values(79, 3, 'DocCashOfficeSumMovements', 'Документ Касса Перемещение'); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region DirCashOfficeSumTypes

            SQL = 
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(29, 'Перемещение.Списание №', -1); " + 
                "INSERT INTO DirCashOfficeSumTypes (DirCashOfficeSumTypeID, DirCashOfficeSumTypeName, Sign)values(30, 'Перемещение.Внесение №', 1); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Права: RightDocCashOfficeSumMovements и RightDocSecondHandMovementsLogistics

            SQL =

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocCashOfficeSumMovements] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocCashOfficeSumMovementsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocCashOfficeSumMovements=1, RightDocCashOfficeSumMovementsCheck=1 WHERE DirEmployeeID=1; " + 

                "ALTER TABLE DirEmployees ADD COLUMN [RightDocCashOfficeSumMovementsLogistics] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocCashOfficeSumMovementsLogisticsCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocCashOfficeSumMovementsLogistics=1, RightDocCashOfficeSumMovementsLogisticsCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            #region Исправление предыдущих ошибок

            //"CREATE INDEX[IDX_DocSecondHandMovements_DirEmployeeID] ON[DocSecondHandMovements] ([DirEmployeeIDCourier]);" +
            SQL =
                "DROP INDEX IDX_DocSecondHandMovs_DirEmployeeID;";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 82;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update83: DocCashOfficeSumMovements [DirCashOfficeSumFrom, DirCashOfficeSumTo] + Права: Хоз.Расчеты

        private async Task<bool> Update83(DbConnectionSklad db)
        {
            string SQL = "";


            #region 2. DocCashOfficeSumMovements

            //1. DocCashOfficeSumMovements
            SQL =
                "ALTER TABLE DocCashOfficeSumMovements ADD COLUMN [DirCashOfficeSumFrom] REAL DEFAULT 0; " +
                "ALTER TABLE DocCashOfficeSumMovements ADD COLUMN [DirCashOfficeSumTo] REAL DEFAULT 0; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 3. Права: Хоз.Расчеты

            SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocDomesticExpenseSalaries] INTEGER DEFAULT 3; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocDomesticExpenseSalariesCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightDocDomesticExpenseSalaries=1, RightDocDomesticExpenseSalariesCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 83;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update84: LogSecondHands и LogServices

        private async Task<bool> Update84(DbConnectionSklad db)
        {
            string SQL = "";

            #region 1. Найти в Логе все статусы == 1 (LogSecondHandRazbors.DirSecondHandLogTypeID == 1) и в поле "Msg" записать на какую точку принят аппарат

            SQL =

                "UPDATE LogSecondHands " +
                "SET " +

                "Msg='Аппарат принят на точку ' ||  " +
                "(" +
                " SELECT DirWarehouses.DirWarehouseName " +
                " FROM DocSecondHandPurches, DirWarehouses " +
                " WHERE DocSecondHandPurches.DocSecondHandPurchID=LogSecondHands.DocSecondHandPurchID and DirWarehouses.DirWarehouseID=DocSecondHandPurches.DirWarehouseID " +
                " LIMIT 1 " +
                ") " +

                "WHERE LogSecondHands.DirSecondHandStatusID=1";

            SQLiteParameter parMsg1 = new SQLiteParameter("@Msg", System.Data.DbType.String) { Value = "Аппарат принят на точку ..." };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg1));

            #endregion


            #region 2. Найти в Логе все статусы == 1 (LogServiceRazbors.DirServiceLogTypeID == 1) и в поле "Msg" записать на какую точку принят аппарат

            SQL =

                "UPDATE LogServices " +
                "SET " +

                "Msg='Аппарат принят на точку ' ||  " +
                "(" +
                " SELECT DirWarehouses.DirWarehouseName " +
                " FROM DocServicePurches, DirWarehouses " +
                " WHERE DocServicePurches.DocServicePurchID=LogServices.DocServicePurchID and DirWarehouses.DirWarehouseID=DocServicePurches.DirWarehouseID " +
                " LIMIT 1 " +
                ") " +

                "WHERE LogServices.DirServiceStatusID=1";

            SQLiteParameter parMsg2 = new SQLiteParameter("@Msg", System.Data.DbType.String) { Value = "Аппарат принят на точку ..." };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parMsg2));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 84;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update85: ListObjectFields

        private async Task<bool> Update85(DbConnectionSklad db)
        {
            string SQL = "";

            #region 1. ListObjectFields

            SQL =
                "INSERT INTO ListObjectFields (ListObjectID, ListObjectFieldHeaderShow, ListObjectFieldTabShow, ListObjectFieldFooterShow, ListObjectFieldNameID)values(40, 1, 0, 1, 381); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region Замена 1
            //[[[Документ.НакладнаяНаПеремещение.НомерВнутренний]]] => [[[Документ.БУПеремещение.НомерВнутренний]]]

            string[] Fieldname1 = { "ListObjectPFHtmlHeader" };
            SQL = "SELECT ListObjectPFHtmlHeader FROM ListObjectPFs WHERE ListObjectPFID=31";
            ArrayList alField1 = await SelectDataFrom_EtalonDB(SQL, Fieldname1);
            string[] mas = (string[])alField1[0];

            string ListObjectPFHtmlHeader = mas[0].Replace("[[[Справочник.Склад.Адрес]]]", "[[[Справочник.Склад.Адрес]]]; &nbsp; [[[Точка.Телефон]]]");

            SQL = "UPDATE ListObjectPFs SET ListObjectPFHtmlHeader=@ListObjectPFHtmlHeader WHERE ListObjectPFID=31";
            SQLiteParameter parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = ListObjectPFHtmlHeader };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 85;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update86: ListObjectFields

        private async Task<bool> Update86(DbConnectionSklad db)
        {
            string SQL = "";

            #region 1. ListObjectFields

            SQL =
                "ALTER TABLE RemPartyMinuses ADD COLUMN [DirDescriptionID] INTEGER CONSTRAINT [FK_RemPartyMinuses_DirDescriptionID] REFERENCES [DirDescriptions]([DirDescriptionID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 86;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update87: ListObjectFields

        private async Task<bool> Update87(DbConnectionSklad db)
        {
            string SQL = "";

            #region 1. ListObjectFields

            SQL = "ALTER TABLE DirEmployees ADD COLUMN [RightDocServicePurchesDateDoneCheck] BOOL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 87;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update88: DocSecondHandSales

        private async Task<bool> Update88(DbConnectionSklad db)
        {
            string SQL = "";

            #region 1. DocSecondHandSales

            SQL = "ALTER TABLE DocSecondHandSales ADD COLUMN [ServiceTypeRepair] INTEGER NOT NULL DEFAULT 1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 88;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update89: DocSecondHandSales

        private async Task<bool> Update89(DbConnectionSklad db)
        {
            string SQL = "";

            #region Замена 1
            //[[[Документ.НакладнаяНаПеремещение.НомерВнутренний]]] => [[[Документ.БУПеремещение.НомерВнутренний]]]

            string[] Fieldname1 = { "ListObjectPFHtmlHeader" };
            SQL = "SELECT ListObjectPFHtmlHeader FROM ListObjectPFs WHERE ListObjectPFID=42";
            ArrayList alField1 = await SelectDataFrom_EtalonDB(SQL, Fieldname1);
            string[] mas1 = (string[])alField1[0];

            SQL = "UPDATE ListObjectPFs SET ListObjectPFName='Договор Купли-продажи', ListObjectPFHtmlHeader=@ListObjectPFHtmlHeader WHERE ListObjectPFID=42";
            SQLiteParameter parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = mas1[0] };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader));

            #endregion

            #region Замена 2
            //[[[Документ.НакладнаяНаПеремещение.НомерВнутренний]]] => [[[Документ.БУПеремещение.НомерВнутренний]]]

            string[] Fieldname2 = { "ListObjectPFHtmlHeader" };
            SQL = "SELECT ListObjectPFHtmlHeader FROM ListObjectPFs WHERE ListObjectPFID=46";
            ArrayList alField2 = await SelectDataFrom_EtalonDB(SQL, Fieldname2);
            string[] mas2 = (string[])alField2[0];


            SQL =
                "INSERT INTO ListObjectPFs " +
                "(" +
                " ListObjectPFID, ListObjectPFIDSys, ListObjectID, ListLanguageID, ListObjectPFName, ListObjectPFHtmlHeaderUse, ListObjectPFHtmlDouble, " +
                " ListObjectPFHtmlHeader, " +
                " ListObjectPFHtmlTabFontSize, " +
                " MarginTop, MarginBottom, MarginLeft, MarginRight " +
                ")" +
                "values" +
                "(" +
                " 46, 0, 66, 1, 'Продажа: Гарантийный талон', 1, 1, @ListObjectPFHtmlHeader, 0, 10, 10, 0, 0 " +
                ")";


            parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = mas2[0] };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 89;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update90: DocOrderInts + DirOrderIntTypes

        private async Task<bool> Update90(DbConnectionSklad db)
        {
            #region 1. DocOrderInts

            string SQL =
                "DELETE FROM LogOrderInts; " +
                "DELETE FROM DocOrderInts; " +

                "ALTER TABLE DocOrderInts ADD COLUMN [DirOrderIntContractorWWW] TEXT(256); " +
                "ALTER TABLE DocOrderInts ADD COLUMN [DirOrderIntContractorDesc] TEXT(256); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. DirOrderIntTypes

            SQL =
                "UPDATE DirOrderIntTypes SET DirOrderIntTypeName='Предзаказы (Лёгкий)' WHERE DirOrderIntTypeID=1; " +
                "UPDATE DirOrderIntTypes SET DirOrderIntTypeName='Впрок (Аналитика)' WHERE DirOrderIntTypeID=2; " +
                "UPDATE DirOrderIntTypes SET DirOrderIntTypeName='Мастерская' WHERE DirOrderIntTypeID=3; " +
                "UPDATE DirOrderIntTypes SET DirOrderIntTypeName='Б/У' WHERE DirOrderIntTypeID=4; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 3. Docs.DirOrderIntTypeID

            SQL =
                "ALTER TABLE Docs ADD COLUMN [DirOrderIntTypeID] INTEGER DEFAULT NULL; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 90;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update91: DocPurches.DocID - FK

        private async Task<bool> Update91(DbConnectionSklad db)
        {
            #region 1. DocPurches.DocID - FK

            string SQL =

                //0. 
                "DROP INDEX IDX_DocPurches_DocID; " +
                "DROP INDEX IDX_DocPurches_DirWarehouseID; " +
                "DROP INDEX IDX_DocPurches_NumberTax; " +
                "DROP INDEX IDX_DocPurches_NumberTT; " +



                //1. CREATE TEMPORARY TABLE
                "CREATE TEMPORARY TABLE [DocPurches_backup](" +
                " [DocPurchID] INTEGER, " +
                " [DocID] INTEGER NOT NULL, " +
                " [NumberTT] TEXT(128), " +
                " [NumberTax] TEXT(128), " +
                " [DirWarehouseID] INTEGER " +
                ");" +

                //2. INSERT INTO t1_backup SELECT a,b FROM t1;
                "INSERT INTO [DocPurches_backup] " +
                "SELECT [DocPurchID], [DocID], [NumberTT], [NumberTax], [DirWarehouseID] FROM [DocPurches]; " +

                //3. DROP TABLE t1;
                "DROP TABLE [DocPurches]; " +

                //4. CREATE TABLE t1(a,b);
                "CREATE TABLE [DocPurches] ( \n" +
                "  [DocPurchID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT CONSTRAINT [FK_DocPurches_DocID] REFERENCES [DocPurches]([DocPurchID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [DocID] INTEGER NOT NULL CONSTRAINT [FK_DocPurches_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, \n" +
                "  [NumberTT] TEXT(128), \n" +
                "  [NumberTax] TEXT(128), \n" +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DocPurches_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED \n" + 
                "); " +

                //5. INSERT INTO t1 SELECT a,b FROM t1_backup;
                "INSERT INTO [DocPurches] " +
                "SELECT [DocPurchID], [DocID], [NumberTT], [NumberTax], [DirWarehouseID] FROM [DocPurches_backup]; " +

                //3. DROP TABLE t1_backup;
                "DROP TABLE [DocPurches_backup]; " +



                //n. 
                "CREATE INDEX [IDX_DocPurches_DocID] ON [DocPurches] ([DocID]); " +
                "CREATE INDEX [IDX_DocPurches_DirWarehouseID] ON [DocPurches] ([DirWarehouseID]); " +
                "CREATE INDEX [IDX_DocPurches_NumberTax] ON [DocPurches] ([NumberTax] COLLATE NOCASE); " +
                "CREATE INDEX [IDX_DocPurches_NumberTT] ON [DocPurches] ([NumberTT] COLLATE NOCASE); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 91;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update92: DocOrderInts.DirNomenCategoryID - Is NULL

        private async Task<bool> Update92(DbConnectionSklad db)
        {
            #region 1. DocOrderInts.DirNomenCategoryID - Is NULL

            string SQL =

                //0. 
                "DROP INDEX IDX_DocOrderInts_DirOrderContractorID; " +
                "DROP INDEX IDX_DocOrderInts_DirOrderIntTypeID; " +



                //1. CREATE TEMPORARY TABLE
                "CREATE TEMPORARY TABLE[DocOrderInts_backup] ( " +
                "  [DocOrderIntID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "  [DocID] INTEGER NOT NULL, " +
                "  [DirOrderIntTypeID] INTEGER NOT NULL, " +
                "  [DocID2] INTEGER, " +
                "  [NumberReal] INTEGER, " +
                "  [DirWarehouseID] INTEGER NOT NULL, " +
                "  [DirOrderIntStatusID] INTEGER NOT NULL, " +
                "  [DirNomenID] INTEGER, " +
                "  [DirNomenName] TEXT, " +
                "  [PriceVAT] REAL NOT NULL, " +
                "  [PriceCurrency] REAL NOT NULL, " +
                "  [DirCurrencyID] INTEGER NOT NULL, " +
                "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1, " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1, " +
                "  [Quantity] REAL NOT NULL DEFAULT 1, " +
                "  [DirNomen1ID] INTEGER NOT NULL, " +
                "  [DirNomen2ID] INTEGER NOT NULL, " +
                "  [DirNomenCategoryID] INTEGER NOT NULL, " +
                "  [DirCharColourID] INTEGER, " +
                "  [DirCharMaterialID] INTEGER, " +
                "  [DirCharNameID] INTEGER, " +
                "  [DirCharSeasonID] INTEGER, " +
                "  [DirCharSexID] INTEGER, " +
                "  [DirCharSizeID] INTEGER, " +
                "  [DirCharStyleID] INTEGER, " +
                "  [DirCharTextureID] INTEGER, " +
                "  [PrepaymentSum] REAL NOT NULL DEFAULT 0, " +
                "  [DateDone] DATE NOT NULL, " +

                "  [DirOrderIntContractorName] TEXT(256), " +
                "  [DirOrderIntContractorID] INTEGER, " +
                "  [DirOrderIntContractorAddress] TEXT(256), " +
                "  [DirOrderIntContractorPhone] TEXT(256), " +
                "  [DirOrderIntContractorEmail] TEXT(256), " +
                "  [DirOrderIntContractorWWW] TEXT(256), " +
                "  [DirOrderIntContractorDesc] TEXT(256) " + 
                "); " +


                //2. INSERT INTO t1_backup SELECT a,b FROM t1;
                "INSERT INTO [DocOrderInts_backup] " +
                "SELECT " +
                " [DocOrderIntID], [DocID], [DirOrderIntTypeID], [DocID2], [NumberReal], [DirWarehouseID], [DirOrderIntStatusID], [DirNomenID], [DirNomenName], [PriceVAT], [PriceCurrency], " +
                " [DirCurrencyID], [DirCurrencyRate], [DirCurrencyMultiplicity], [Quantity], [DirNomen1ID], [DirNomen2ID], [DirNomenCategoryID], [DirCharColourID], [DirCharMaterialID], " +
                " [DirCharNameID], [DirCharSeasonID], [DirCharSexID], [DirCharSizeID], [DirCharStyleID], [DirCharTextureID], [PrepaymentSum], [DateDone], " +
                " [DirOrderIntContractorName], [DirOrderIntContractorID], [DirOrderIntContractorAddress], [DirOrderIntContractorPhone], [DirOrderIntContractorEmail], [DirOrderIntContractorWWW], [DirOrderIntContractorDesc] " +
                "FROM [DocOrderInts]; " +


                //3. DROP TABLE t1;
                "DROP TABLE [DocOrderInts]; " +


                //4. CREATE TABLE t1(a,b);
                "CREATE TABLE[DocOrderInts] ( " +
                "  [DocOrderIntID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "  [DocID] INTEGER NOT NULL CONSTRAINT [FK_DocOrderInts_DocID] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirOrderIntTypeID] INTEGER NOT NULL CONSTRAINT [FK_DocOrderInts_DirOrderIntTypeID] REFERENCES [DirOrderIntTypes]([DirOrderIntTypeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED DEFAULT 1, " +
                "  [DocID2] INTEGER CONSTRAINT [FK_DocOrderInts_DocID2] REFERENCES [Docs]([DocID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [NumberReal] INTEGER, " +
                "  [DirWarehouseID] INTEGER NOT NULL CONSTRAINT [FK_DocOrderInts_DirWarehouseID] REFERENCES [DirWarehouses]([DirWarehouseID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirOrderIntStatusID] INTEGER NOT NULL CONSTRAINT [FK_DocOrderInts_DirOrderIntStatusID] REFERENCES [DirOrderIntStatuses]([DirOrderIntStatusID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirNomenID] INTEGER CONSTRAINT [FK_DocOrderInts_DirNomenID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirNomenName] TEXT(256), " +
                "  [PriceVAT] REAL NOT NULL, " +
                "  [PriceCurrency] REAL NOT NULL, " +
                "  [DirCurrencyID] INTEGER NOT NULL CONSTRAINT [FK_DocSaleTab_DirCurrencyID] REFERENCES [DirCurrencies]([DirCurrencyID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCurrencyRate] REAL NOT NULL DEFAULT 1, " +
                "  [DirCurrencyMultiplicity] INTEGER NOT NULL DEFAULT 1, " +
                "  [Quantity] REAL NOT NULL DEFAULT 1, " +
                "  [DirNomen1ID] INTEGER NOT NULL CONSTRAINT [FK_DocOrderInts_DirNomen1ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirNomen2ID] INTEGER NOT NULL CONSTRAINT [FK_DocOrderInts_DirNomen2ID] REFERENCES [DirNomens]([DirNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirNomenCategoryID] INTEGER CONSTRAINT [FK_DocOrderInts_DirNomenCategoryID] REFERENCES [DirNomenCategories]([DirNomenCategoryID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharColourID] INTEGER CONSTRAINT [FK_DocOrderInts_DirCharColourID] REFERENCES [DirCharColours]([DirCharColourID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharMaterialID] INTEGER CONSTRAINT [FK_DocOrderInts_DirCharMaterialID] REFERENCES [DirCharMaterials]([DirCharMaterialID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharNameID] INTEGER CONSTRAINT [FK_DocOrderInts_DirCharNameID] REFERENCES [DirCharNames]([DirCharNameID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharSeasonID] INTEGER CONSTRAINT [FK_DocOrderInts_DirCharSeasonID] REFERENCES [DirCharSeasons]([DirCharSeasonID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharSexID] INTEGER CONSTRAINT [FK_DocOrderInts_DirCharSexID] REFERENCES [DirCharSexes]([DirCharSexID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharSizeID] INTEGER CONSTRAINT [FK_DocOrderInts_DirCharSizeID] REFERENCES [DirCharSizes]([DirCharSizeID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharStyleID] INTEGER CONSTRAINT [FK_DocOrderInts_DirCharStyleID] REFERENCES [DirCharStyles]([DirCharStyleID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirCharTextureID] INTEGER CONSTRAINT [FK_DocOrderInts_DirCharTextureID] REFERENCES [DirCharTextures]([DirCharTextureID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [PrepaymentSum] REAL NOT NULL DEFAULT 0, " +
                "  [DateDone] DATE NOT NULL, " +

                "  [DirOrderIntContractorName] TEXT(256), " +
                "  [DirOrderIntContractorID] INTEGER CONSTRAINT [FK_DocOrderInts_DirOrderContractorID] REFERENCES [DirOrderIntContractors]([DirOrderIntContractorID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED, " +
                "  [DirOrderIntContractorAddress] TEXT(256), " +
                "  [DirOrderIntContractorPhone] TEXT(256), " +
                "  [DirOrderIntContractorEmail] TEXT(256), " +
                "  [DirOrderIntContractorWWW] TEXT(256), " +
                "  [DirOrderIntContractorDesc] TEXT(256) " +
                "); " +


                //5. INSERT INTO t1 SELECT a,b FROM t1_backup;
                "INSERT INTO [DocOrderInts] " +
                "SELECT " +
                " [DocOrderIntID], [DocID], [DirOrderIntTypeID], [DocID2], [NumberReal], [DirWarehouseID], [DirOrderIntStatusID], [DirNomenID], [DirNomenName], [PriceVAT], [PriceCurrency], " +
                " [DirCurrencyID], [DirCurrencyRate], [DirCurrencyMultiplicity], [Quantity], [DirNomen1ID], [DirNomen2ID], [DirNomenCategoryID], [DirCharColourID], [DirCharMaterialID], " +
                " [DirCharNameID], [DirCharSeasonID], [DirCharSexID], [DirCharSizeID], [DirCharStyleID], [DirCharTextureID], [PrepaymentSum], [DateDone], " +
                " [DirOrderIntContractorName], [DirOrderIntContractorID], [DirOrderIntContractorAddress], [DirOrderIntContractorPhone], [DirOrderIntContractorEmail], [DirOrderIntContractorWWW], [DirOrderIntContractorDesc] " +
                "FROM [DocOrderInts_backup]; " +


                //3. DROP TABLE t1_backup;
                "DROP TABLE [DocOrderInts_backup]; " +



                //n. 
                "CREATE INDEX [IDX_DocOrderInts_DirOrderIntTypeID] ON [DocOrderInts] ([DirOrderIntTypeID]); " +
                "CREATE INDEX [IDX_DocOrderInts_DirOrderIntStatusID] ON [DirOrderIntStatuses] ([DirOrderIntStatusID]); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 92;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update93: DocOrderInts.DirNomenCategoryID - Is NULL

        private async Task<bool> Update93(DbConnectionSklad db)
        {
            #region Замена 1
            //[[[Документ.НакладнаяНаПеремещение.НомерВнутренний]]] => [[[Документ.БУПеремещение.НомерВнутренний]]]

            string[] Fieldname1 = { "ListObjectPFHtmlHeader" };
            string SQL = "SELECT ListObjectPFHtmlHeader FROM ListObjectPFs WHERE ListObjectPFID=35";
            ArrayList alField1 = await SelectDataFrom_EtalonDB(SQL, Fieldname1);
            string[] mas = (string[])alField1[0];

            string ListObjectPFHtmlHeader = mas[0].Replace("[[[Документ.Дата]]]", "[[[Документ.ДатаГотовности]]]");

            SQL = "UPDATE ListObjectPFs SET ListObjectPFHtmlHeader=@ListObjectPFHtmlHeader WHERE ListObjectPFID=35";
            SQLiteParameter parListObjectPFHtmlHeader = new SQLiteParameter("@ListObjectPFHtmlHeader", System.Data.DbType.String) { Value = ListObjectPFHtmlHeader };
            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parListObjectPFHtmlHeader));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 93;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update94: Права: RightDocServiceWorkshopsTab2Return

        private async Task<bool> Update94(DbConnectionSklad db)
        {
            #region 1. Права: RightDocServiceWorkshopsTab2Return

            string SQL =
                //1. Серв.Цент: изменяем существующие записи
                //"ALTER TABLE DirEmployees ADD COLUMN [RightDocServiceWorkshopsTab2Return] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServiceWorkshopsTab2ReturnCheck] BOOL DEFAULT 1; ";
                //"UPDATE DirEmployees SET RightDocServiceWorkshopsTab2Return=0; " +
                //"UPDATE DirEmployees SET RightDocServiceWorkshopsTab2ReturnCheck=0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 94;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update95: DirEmployees.RightDocServiceWorkshopsTab1AddCheck

        private async Task<bool> Update95(DbConnectionSklad db)
        {
            #region 1. DirEmployees.RightDocServiceWorkshopsTab1AddCheck

            string SQL =
                //1. Серв.Цент: изменяем существующие записи
                //"ALTER TABLE DirEmployees ADD COLUMN [RightDocServiceWorkshopsTab1Add] INTEGER DEFAULT 1; " +
                "ALTER TABLE DirEmployees ADD COLUMN [RightDocServiceWorkshopsTab1AddCheck] BOOL DEFAULT 1; ";
            //"UPDATE DirEmployees SET RightDocServiceWorkshopsTab1Add=0; " +
            //"UPDATE DirEmployees SET RightDocServiceWorkshopsTab1AddCheck=0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 95;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update96: Права: RightDocServiceWorkshopsTab1Add

        private async Task<bool> Update96(DbConnectionSklad db)
        {
            #region 1. Права: RightDocServiceWorkshopsTab1Add

            string SQL =
                "INSERT INTO DirSmsTemplates " + 
                "(DirSmsTemplateID, DirSmsTemplateName, DirSmsTemplateMsg, DirSmsTemplateType, MenuID)" + 
                "values" +
                "(8, 'При перемещении на точку', 'На Вашу точку перемещён док №[[[ДокументНомер]]] с точки [[[ТочкаОт]]]', 1, 1); ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 96;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update97: Права: RightDocServiceWorkshopsTab1Add

        private async Task<bool> Update97(DbConnectionSklad db)
        {
            #region 1. Права: RightDocServiceWorkshopsTab1Add

            string SQL =
                "ALTER TABLE DocOrderInts ADD COLUMN [DirServiceNomenID] INTEGER CONSTRAINT [FK_DocOrderInts_DirServiceNomenID] REFERENCES [DirServiceNomens]([DirServiceNomenID]) ON DELETE RESTRICT ON UPDATE RESTRICT DEFERRABLE INITIALLY DEFERRED; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 97;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update98: Права: RightDocServiceWorkshopsTab1Add

        private async Task<bool> Update98(DbConnectionSklad db)
        {
            #region 1. Права: RightDocServiceWorkshopsTab1Add

            string SQL =
                "ALTER TABLE SysSettings ADD COLUMN [SmsServiceMov] BOOL DEFAULT 0; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion



            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 98;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update99: Права: DirEmployees.RightReportSalariesEmpl: видит только свою ЗП

        private async Task<bool> Update99(DbConnectionSklad db)
        {
            #region 1. Права: Admin

            string SQL =
                "ALTER TABLE DirEmployees ADD COLUMN [RightReportSalariesEmplCheck] BOOL DEFAULT 0; " +
                "UPDATE DirEmployees SET RightReportSalariesEmplCheck=1 WHERE DirEmployeeID=1; ";

            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 99;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update100: DocServicePurch1Tabs and DocServicePurch2Tabs.RemontN

        private async Task<bool> Update100(DbConnectionSklad db)
        {
            #region 1. Права: Admin

            string SQL =
                "ALTER TABLE DocServicePurch1Tabs ADD COLUMN [RemontN] INTEGER; " +
                "UPDATE DocServicePurch1Tabs SET [RemontN]=1; " +
                "ALTER TABLE DocServicePurch2Tabs ADD COLUMN [RemontN] INTEGER; " +
                "UPDATE DocServicePurch2Tabs SET [RemontN]=1; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 100;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update101: DirSmsTemplates and SysSettings

        private async Task<bool> Update101(DbConnectionSklad db)
        {
            #region 1. DirSmsTemplates and SysSettings

            string SQL =
                "UPDATE DirSmsTemplates SET DirSmsTemplateName='Заказ принят', DirSmsTemplateMsg='Ваш заказ [[[ТоварНаименование]]] принят' WHERE DirSmsTemplateID=5; " +

                "INSERT INTO DirSmsTemplates " +
                "(DirSmsTemplateID, DirSmsTemplateName, DirSmsTemplateMsg, DirSmsTemplateType, MenuID)" +
                "values" +
                "(9, 'Заказ готов к выдачи', 'Ваш заказ [[[ТоварНаименование]]] готов к выдачи', 1, 3); " +

                "INSERT INTO DirSmsTemplates " +
                "(DirSmsTemplateID, DirSmsTemplateName, DirSmsTemplateMsg, DirSmsTemplateType, MenuID)" +
                "values" +
                "(10, 'Заказ исполнен', 'Ваш заказ [[[ТоварНаименование]]] исполнен', 1, 3); " +


                "ALTER TABLE SysSettings ADD COLUMN [SmsOrderInt5] BOOL DEFAULT 1; " +
                "ALTER TABLE SysSettings ADD COLUMN [SmsOrderInt9] BOOL DEFAULT 1; " +
                "ALTER TABLE SysSettings ADD COLUMN [SmsOrderInt10] BOOL DEFAULT 1; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 101;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update102: DirOrderIntStatuses - INSERT

        private async Task<bool> Update102(DbConnectionSklad db)
        {
            #region 1. DirSmsTemplates and SysSettings

            string SQL =

                "UPDATE DirOrderIntStatuses SET DirOrderIntStatusID=10 WHERE DirOrderIntStatusID=1; " +
                "UPDATE DirOrderIntStatuses SET DirOrderIntStatusID=20 WHERE DirOrderIntStatusID=2; " +
                "UPDATE DirOrderIntStatuses SET DirOrderIntStatusID=30 WHERE DirOrderIntStatusID=3; " +
                "UPDATE DirOrderIntStatuses SET DirOrderIntStatusID=40 WHERE DirOrderIntStatusID=4; " +
                "UPDATE DirOrderIntStatuses SET DirOrderIntStatusID=50 WHERE DirOrderIntStatusID=5; " +

                "INSERT INTO DirOrderIntStatuses " +
                "(DirOrderIntStatusID, DirOrderIntStatusName)" +
                "values" +
                "(35, 'Готов к выдаче'); " +


                "UPDATE DocOrderInts SET DirOrderIntStatusID=10 WHERE DirOrderIntStatusID=1; " +
                "UPDATE DocOrderInts SET DirOrderIntStatusID=20 WHERE DirOrderIntStatusID=2; " +
                "UPDATE DocOrderInts SET DirOrderIntStatusID=30 WHERE DirOrderIntStatusID=3; " +
                "UPDATE DocOrderInts SET DirOrderIntStatusID=40 WHERE DirOrderIntStatusID=4; " +
                "UPDATE DocOrderInts SET DirOrderIntStatusID=50 WHERE DirOrderIntStatusID=5; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 102;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion


        #region Update103: DocCashOfficeSums and DocBankSums

        private async Task<bool> Update103(DbConnectionSklad db)
        {
            #region 1. ALTER DocCashOfficeSums

            string SQL = "ALTER TABLE DocCashOfficeSums ADD COLUMN [DateOnly] DATE; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 2. ALTER DocBankSums

            SQL = "ALTER TABLE DocBankSums ADD COLUMN [DateOnly] DATE; ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 3. UPDATE DocCashOfficeSums

            SQL = "UPDATE DocCashOfficeSums SET DateOnly=datetime(date(DocCashOfficeSumDate)); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion


            #region 4. UPDATE DocBankSums

            SQL = "UPDATE DocBankSums SET DateOnly=datetime(date(DocBankSumDate)); ";


            await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL));

            #endregion




            //n.
            Models.Sklad.Sys.SysVer sysVer = await Task.Run(() => db.SysVers.FindAsync(1));
            sysVer.SysVerNumber = 103;
            sysVer.SysVerDate = DateTime.Now;

            db.Entry(sysVer).State = System.Data.Entity.EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            return true;
        }

        #endregion





        #endregion



        #region Etalon

        private async Task<ArrayList> SelectDataFrom_EtalonDB(string SQL, string[] Fieldname)
        {
            Classes.Function.Variables.ConnectionString connectionString = new Function.Variables.ConnectionString();
            string ConStr = "Data Source=" + connectionString.SQLitePathEtalon() + ";New=False;Version=3;foreign keys=false;"; // connectionString.SQLitePathEtalon();

            ArrayList ret = new ArrayList();

            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                con.Open();  //await con.OpenAsync(); //con.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string[] mas = new string[Fieldname.Length];
                            for (int i = 0; i < Fieldname.Length; i++)
                            {
                                mas[i] = dr[Fieldname[i]].ToString();
                            }
                            ret.Add(mas);
                        }
                    }
                }
            }

            return ret;
        }

        #endregion

    }
}