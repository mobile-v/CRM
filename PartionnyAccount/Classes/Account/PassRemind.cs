using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PartionnyAccount.Models;
using System.Data.Entity;

namespace PartionnyAccount.Classes.Account
{
    public class PassRemind
    {
        #region var
        public int CountRecord = 0;
        int DirCustomersID = 0;
        public string SendMsg = "", pDirLanguageID;
        //Class.Settings.Variables _var = new Class.Settings.Variables();
        //Class.Functions.Functions fun = new Class.Functions.Functions();
        #endregion

        public void ConfirmAndSendMail(string Email)
        {
            /*
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnStrMSSQL"].ConnectionString))
            {
                con.Open();
                using (SqlConnection con2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnStrMSSQL"].ConnectionString))
                {
                    con2.Open();

                    //Читаем БД
                    bool Confirmed = false;
                    using (SqlCommand cmd = new SqlCommand("SELECT DirCustomersID, Login, Pswd, DirLanguageID, Confirmed, Active FROM DirCustomers WHERE (Email LIKE @Email)", con))
                    {
                        SQLiteParameter parEmail = new SQLiteParameter("@Email", System.Data.SqlDbType.NVarChar); cmd.Parameters.Add(parEmail).Value = Email;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                DirCustomersID = Convert.ToInt32(dr["DirCustomersID"].ToString());
                                Confirmed = Convert.ToBoolean(dr["Confirmed"].ToString());
                                pDirLanguageID = dr["DirLanguageID"].ToString();

                                //К-во Логинов пренадлежащих введённому ЕМайлу
                                CountRecord++;

                                SendMsg += CountRecord + ".<hr>";

                                //Пароль в сервис (из БД СкуЛайт)
                                if (Convert.ToBoolean(dr["Active"].ToString()))
                                {
                                    //Try нужет, если нет папки с БД.
                                    try { SQLitePswd(DirCustomersID, dr["Login"].ToString(), Convert.ToInt32(dr["DirLanguageID"].ToString())); }
                                    catch { SendMsg += "База Данных на прфилактеке ...<br />"; }

                                    //Апдейтим историю
                                    UpdateMsSQL(con2, DirCustomersID);
                                }
                                else
                                {
                                    if (!Confirmed) { SendMsg += "Логин " + dr["Login"].ToString() + " <b>Не Подтверждён</b>!"; }
                                    else SendMsg += "Логин '" + dr["Login"].ToString() + "' <b>Не Активен</b>! Возможно, Вы не заходили в сервис под эти Логином более 3-х месяцев. Если желаете разблокировать его, напишите на <a href='mailto:support@intradecloud.com'>support@intradecloud.com</a>";

                                    SendMsg += "<hr>";
                                }

                                //Для "IdToString(pDirLanguageID)"
                                PartionnyAccount.Controllers.Classes.Account.RegConf regConf = new PartionnyAccount.Controllers.Classes.Account.RegConf();

                                SendMsg += "------------------------------------<br />" +
                                "<img src='https://intradecloud.com/images/partner_work.png' alt='Партнёрская программа' align='left' width='5%' height='5%' />" +
                                "<b>Партнёрская программа</b>!<br /> " +
                                //"Вы получаете <b>35%</b> комиссии со всех оплат клиентов, которых Вы привели.<br /> " +

                                "Ваша партнерская ссылка для заработка на новых клиентах:<br /> " +

                                "<a href='https://intradecloud.com/" + regConf.IdToString(pDirLanguageID) + "registration?ref=" + fun.RoundID() + DirCustomersID.ToString() + "'>" +
                                "https://intradecloud.com/" + regConf.IdToString(pDirLanguageID) + "registration?ref=" + fun.RoundID() + DirCustomersID.ToString() +
                                "</a><br /><br /> " +

                                "Сcылка в панель 'Партнёрской программы':<br />" +
                                "<a href='https://sklad.intradecloud.com/PanelCustomers/'>https://sklad.intradecloud.com/PanelCustomers/</a>" + "(Логин=" + dr["Login"].ToString() + ", Пароль=" + dr["Pswd"].ToString() + ")<br /><br /> " +

                                "Желаем Вам больших и стабильных заработков вместе с нами!<br /><br /><hr> ";


                            }
                        }
                    }


                    //Отправляем по Мейлу.
                    SendMail(Email);

                    con2.Close(); con2.Dispose();
                }
                con.Close(); con.Dispose();
            }
            */


            using (DbConnectionLogin con = new DbConnectionLogin("ConnStrMSSQL"))
            {
                //1. Получаем список Логинов зарегистрированных на данный ЕМайл-у
                var query =
                    (
                        from dirCustomers in con.DirCustomers
                        where dirCustomers.Email == Email
                        select new
                        {
                            DirCustomersID = dirCustomers.DirCustomersID,
                            Login = dirCustomers.Login,
                            Pswd = dirCustomers.Pswd,
                            DirLanguageID = dirCustomers.DirLanguageID,
                            Confirmed = dirCustomers.Confirmed,
                            Active = dirCustomers.Active
                        }
                    ).ToList();

                bool Confirmed = false;
                for (int i = 0; i < query.Count(); i++)
                {
                    DirCustomersID = query[i].DirCustomersID;
                    Confirmed = Convert.ToBoolean(query[i].Confirmed);
                    pDirLanguageID = query[i].DirLanguageID.ToString();

                    //К-во Логинов пренадлежащих введённому ЕМайлу
                    CountRecord++;

                    SendMsg += CountRecord + ".<hr>";

                    //Пароль в сервис (из БД СкуЛайт)
                    if (Convert.ToBoolean(query[i].Active))
                    {
                        //Try нужет, если нет папки с БД.
                        try { SQLitePswd(DirCustomersID, query[i].Login, query[i].DirLanguageID); }
                        catch { SendMsg += "База Данных на прфилактеке ...<br />"; }

                        //Апдейтим историю
                        //UpdateMsSQL(con2, DirCustomersID);
                    }
                    else
                    {
                        if (!Confirmed) { SendMsg += "Логин " + query[i].Login + " <b>Не Подтверждён</b>!"; }
                        else SendMsg += "Логин '" + query[i].Login + "' <b>Не Активен</b>! Возможно, Вы не заходили в сервис под эти Логином более 3-х месяцев. Если желаете разблокировать его, напишите на <a href='mailto:support@intradecloud.com'>support@intradecloud.com</a>";

                        SendMsg += "<hr>";
                    }

                    //Для "IdToString(pDirLanguageID)"
                    PartionnyAccount.Classes.Account.RegConf regConf = new PartionnyAccount.Classes.Account.RegConf();

                    /*
                    SendMsg += "------------------------------------<br />" +
                    "<img src='https://intradecloud.com/images/partner_work.png' alt='Партнёрская программа' align='left' width='5%' height='5%' />" +
                    "<b>Партнёрская программа</b>!<br /> " +
                    //"Вы получаете <b>35%</b> комиссии со всех оплат клиентов, которых Вы привели.<br /> " +

                    "Ваша партнерская ссылка для заработка на новых клиентах:<br /> " +

                    "<a href='https://intradecloud.com/" + regConf.IdToString(pDirLanguageID) + "registration?ref=" + regConf.RoundID() + DirCustomersID.ToString() + "'>" +
                    "https://intradecloud.com/" + regConf.IdToString(pDirLanguageID) + "registration/?ref=" + regConf.RoundID() + DirCustomersID.ToString() +
                    "</a><br /><br /> " +

                    "Сcылка в панель 'Партнёрской программы':<br />" +
                    "<a href='https://sklad.intradecloud.com/PanelCustomers/'>https://sklad.intradecloud.com/PanelCustomers/</a>" + "(Логин=" + query[i].Login + ", Пароль=" + query[i].Pswd + ")<br /><br /> " +

                    "Желаем Вам больших и стабильных заработков вместе с нами!<br /><br /><hr> ";
                    */
                }

                //Отправляем по Мейлу.
                SendMail(Email);

            }
        }


        private void SQLitePswd(int DirCustomersID, string Login, int DirLanguageID)
        {
            /*
            using (SQLiteConnection con = new SQLiteConnection(_var.GetSQLiteBasicConnStr_DirCustomersID(DirCustomersID)))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT DirEmployeeLogin, DirEmployeePswd FROM DirEmployees WHERE DirEmployeeID=1", con))
                {
                    //SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirCustomersID }; cmd.Parameters.Add(parDirEmployeeID);
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            SendMsg +=
                            "Данные для входа:<br /><br />" +

                            "Web-сервис:<br />" +
                            "Ваш Логин: <b>" + dr["DirEmployeeLogin"].ToString() + "@" + Login + "</b><br />" +
                            "Ваш Пароль: <b>" + dr["DirEmployeePswd"].ToString() + "</b> (можно поменять в сервисе)<br /><br />" +

                            "<b>Прямая гиперссылка для входа в сервис:</b><br />" +
                            "<a href='https://sklad.intradecloud.com/Login.aspx?username=" + dr["DirEmployeeLogin"].ToString() + "@" + Login + "&password=" + dr["DirEmployeePswd"].ToString() + "&language=" + DirLanguageID + "&theme=1'>" +
                            "https://sklad.intradecloud.com/Login.aspx?username=" + dr["DirEmployeeLogin"].ToString() + "@" + Login + "&password=" + dr["DirEmployeePswd"].ToString() + "&language=" + DirLanguageID + "&theme=1</a><br /><br />" +
                            "<br />";
                        }
                    }
                }
                con.Close(); con.Dispose();
            }
            */

            Classes.Function.Variables.ConnectionString connectionString = new Function.Variables.ConnectionString();
            using (DbConnectionSklad db = new DbConnectionSklad(connectionString.Return(DirCustomersID, null, true)))
            {
                //"SELECT DirEmployeeLogin, DirEmployeePswd FROM DirEmployee WHERE DirEmployeeID=1"
                var query =
                    (
                        from dirEmployees in db.DirEmployees
                        where dirEmployees.DirEmployeeID == 1
                        select new
                        {
                            DirEmployeeLogin = dirEmployees.DirEmployeeLogin,
                            DirEmployeePswd = dirEmployees.DirEmployeePswd
                        }
                    ).ToList();

                for (int i = 0; i < query.Count(); i++)
                {
                    SendMsg +=
                    "Данные для входа:<br /><br />" +

                    "Web-сервис:<br />" +
                    "Ваш Логин: <b>" + query[0].DirEmployeeLogin + "@" + Login + "</b><br />" +
                    "Ваш Пароль: <b>" + query[0].DirEmployeePswd + "</b> (можно поменять в сервисе)<br /><br />" +

                    "<b>Прямая гиперссылка для входа в сервис:</b><br />" +
                    "<a href='https://sklad.intradecloud.com/account/login/?username=" + query[0].DirEmployeeLogin + "@" + Login + "&password=" + query[0].DirEmployeePswd + "&language=" + DirLanguageID + "&theme=1'>" +
                    "https://sklad.intradecloud.com/account/login/?username=" + query[0].DirEmployeeLogin + "@" + Login + "&password=" + query[0].DirEmployeePswd + "&language=" + DirLanguageID + "&theme=1</a><br /><br />" +
                    "<br />";
                }
            }
        }

        /*
        private void UpdateMsSQL(SqlConnection con2, int DirCustomersID)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO DirCustomersHistory (DirCustomersHistoryTypeID, DirCustomersID)values(1, @DirCustomersID)", con2))
            {
                SQLiteParameter parDirCustomersID = new SQLiteParameter("@DirCustomersID", System.Data.SqlDbType.Int); cmd.Parameters.Add(parDirCustomersID).Value = DirCustomersID;
                cmd.ExecuteNonQuery();
            }
        }
        */

        private void SendMail(string pEMail)
        {
            //Class.Functions.FunMailSend funMailSend = new Class.Functions.FunMailSend();
            Classes.Function.FunctionMSSQL.FunMailSend funMailSend = new Classes.Function.FunctionMSSQL.FunMailSend();
            funMailSend.SendTo_OtherEMail("Напоминание паролей в 'ВТорговомОблаке'. Складской учет и торговля онлайн.", TextMail(pEMail), pEMail);

            try { funMailSend.SendTo_ESklad24("Напоминание паролей.", "Для EMail: " + pEMail); }
            catch { }
        }
        private string TextMail(string pEMail)
        {
            string _Data = "<center> <h2>Напоминание паролей в 'ВТорговомОблаке'. Складской учет онлайн!</h2> </center><br /><br />" +
                SendMsg +
                "<br />" +


                /*"<hr>" +
                //"<img src='http://" + ProjectWWW + "/images/partner_work.png' alt='Партнёрская программа' align='left' width='5%' height='5%' />" +
                "У нас действует простая <b>Партнёрская программа</b>!<br /> " +
                "Вы получаете <b>35%</b> комиссии со всех оплат клиентов, которых Вы привели.<br /> " +
                "Процент Ваших отчислений будет постоянно расти, исходя из общего количества пришедших и оплативших услуги клиентов по Вашей реферальной гиперсылке.<br /> " +

                "Ваша партнерская ссылка для заработка на новых клиентах:<br /> " +
                "http://" + ProjectWWW + "/" + regConf.IdToString(pDirLanguageID) + "registration?ref=" + fun.RoundID() + DirCustomersID.ToString() + "<br /><br /> " +

                "Сcылка в панель 'Партнёрской программы':<br />" +
                "http://online." + ProjectWWW + "/PanelCustomers/" + "(Логин=" + pLogin + ", Пароль=" + pPswd + ")<br /><br /> " +

                "Желаем Вам больших и стабильных заработков вместе с нами!<br /><br /><hr> " +*/


                "Мы всегда готовы ответить на ваши вопросы:<br /> " +
                "support@intradecloud.com<br /> " +
                "МТС: +38-050-950-96-49<br /><br /> " +

                "С уважением команда сервиса ВТорговомОблаке<br /> " +
                "http://www.intradecloud.com/<br /> " +
                "https://www.facebook.com/intradecloud.com<br /> " +
                "https://vk.com/intradecloud<br /> ";


            return _Data;
        }
    }
}