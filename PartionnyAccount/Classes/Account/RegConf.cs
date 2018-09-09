using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using PartionnyAccount.Models;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;

namespace PartionnyAccount.Classes.Account
{
    public class RegConf
    {
        string Msg = "", pEMail = "", pLogin = "", pPswd = "", pDirLanguageID;
        int DirCustomersID = 0;
        public async Task<string[]> Confirmed(string sConfirmed)
        {
            //X - типа версия кода подтверрждения (X, Y, Z, ...)
            //8 - левых символов
            //Кодирование кода DirCustomersID
            //8 - левых символов

            if (sConfirmed[0].ToString() == "X")
            {
                string sDirCustomersID = sConfirmed.Remove(0, 9);
                sDirCustomersID = sDirCustomersID.Remove(sDirCustomersID.Length - 8, 8);

                //Получаем DirCustomersID
                string iDirCustomersID = "";
                for (int i = 0; i < sDirCustomersID.Length; i++)
                {
                    iDirCustomersID += ReturnIntegerConvert(sDirCustomersID[i].ToString()).ToString();
                }

                DirCustomersID = Convert.ToInt32(iDirCustomersID);

                try
                {
                    if (DirCustomersID > 0)
                    {
                        using (DbConnectionLogin con = new DbConnectionLogin("ConnStrMSSQL"))
                        {
                            //Проверяем Подтверждена ли запись, если "Да", то выводм сообщение, иначе Подтверждаем
                            if (!Check_Confirmed_True(con, DirCustomersID)) { throw new System.InvalidOperationException(Msg);}

                            //Создание БД.
                            Copy_Folder_DBFile(DirCustomersID);

                            //Подтверждаем и Активируем запись и
                            Confirmed_Active_True(con, DirCustomersID);

                            //Отправка по Е-Мейл
                            try { SendMail(); } catch { try { SendMail(); } catch { } }
                        }
                    }

                }
                catch (Exception ex)
                {
                    string sMsg = ex.Message;
                    if (sMsg.IndexOf("basic.db") > -1) { sMsg = "Unknown error ..."; }
                    throw new System.InvalidOperationException(sMsg);
                }
            }
            else
            {
                throw new System.InvalidOperationException("Извините, но 'код подтверждения' введён не верно не распознан!");
            }

            string[] ret = { pLogin, pPswd, pDirLanguageID };
            return ret;
        }

        private bool Check_Confirmed_True(DbConnectionLogin con, int DirCustomersID)
        {
            bool ret = false;
            Msg = "Приносим свои извинения, но учетная запись с такими регистрационными данными не найдена! Возможно 'код подтверждения' введен не верно!"; //По умолчанию

            var query =
                (
                    from dirCustomers in con.DirCustomers
                    where dirCustomers.DirCustomersID == DirCustomersID
                    select new
                    {
                        Confirmed = dirCustomers.Confirmed,
                        Email = dirCustomers.Email,
                        Pswd = dirCustomers.Pswd,
                        Login = dirCustomers.Login,
                        DirLanguageID = dirCustomers.DirLanguageID
                    }
                ).ToList();

            if (query.Count() > 0)
            {
                if (Convert.ToBoolean(query[0].Confirmed))
                {
                    Msg = "Данная учетная запись уже подтвержена! Вы можите войти в сервис (воспользуйтесь ссылкой ниже)!";
                    ret = false;
                }
                else
                {
                    pEMail = query[0].Email;
                    pPswd = query[0].Pswd;
                    pLogin = query[0].Login;
                    pDirLanguageID = query[0].DirLanguageID.ToString();
                    Msg = "Ваша учетная запись подтвержена! Теперь Вы можите пользоваться всеми возможностями сервиса!";
                    ret = true;
                }
            }

            return ret;
        }

        private void Copy_Folder_DBFile(int DirCustomersID)
        {
            //Class.Settings.Variables _var = new Class.Settings.Variables();
            Classes.Function.Variables.ConnectionString connectionString = new Function.Variables.ConnectionString();

            //1.Создание папок в Users\
            // user_XXX
            //  user_XXX\Base
            //  user_XXX\File
            if (!Directory.Exists(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID))
            {
                Directory.CreateDirectory(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID);
            }
            if (!Directory.Exists(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\Base"))
            {
                Directory.CreateDirectory(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\Base");
            }
            if (!Directory.Exists(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\File"))
            {
                Directory.CreateDirectory(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\File");
            }
            if (!Directory.Exists(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\File\Logo"))
            {
                Directory.CreateDirectory(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\File\Logo");
            }
            if (!Directory.Exists(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\File\Photo"))
            {
                Directory.CreateDirectory(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\File\Photo");
            }
            if (!Directory.Exists(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\File\Export"))
            {
                Directory.CreateDirectory(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\File\Export");
            }

            //2.Копирование БД из 'Users\Etalon\Base\basic.db' в 'Users\user_XXX\Base\basic.db'
            if (!File.Exists(connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\Base\basic.db"))
            {
                File.Copy(connectionString.SQLitePathEtalon(), connectionString.SQLitePathUser() + @"\user_" + DirCustomersID + @"\Base\basic.db");
            }

            /*
            using (SQLiteConnection con = new SQLiteConnection(connectionString.GetSQLiteBasicConnStr_DirCustomersID(DirCustomersID)))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("UPDATE DirEmployees SET DirEmployeePswd=@DirEmployeePswd", con))
                {
                    SQLiteParameter parDirEmployeePswd = new SQLiteParameter("@DirEmployeePswd", System.Data.DbType.String) { Value = pPswd }; cmd.Parameters.Add(parDirEmployeePswd);
                    con.Open(); cmd.ExecuteNonQuery(); con.Close(); con.Dispose();
                }
            }
            */

            //2.1.Меняем пароль Администратору
            using (DbConnectionSklad db = new DbConnectionSklad(connectionString.Return(DirCustomersID, null, true)))
            {
                Models.Sklad.Dir.DirEmployee dirEmployee = db.DirEmployees.Where(x => x.DirEmployeeID == 1).ToList()[0];
                dirEmployee.DirEmployeePswd = pPswd;

                db.Entry(dirEmployee).State = EntityState.Modified;
                db.SaveChanges();
            }

            //3.Для файлов Users/user_x (например изображения)
            if (!Directory.Exists(connectionString.FilePathUser() + @"\user_" + DirCustomersID))
            {
                Directory.CreateDirectory(connectionString.FilePathUser() + @"\user_" + DirCustomersID);
            }

        }


        private void Confirmed_Active_True(DbConnectionLogin con, int DirCustomersID)
        {
            //Получаем данные клиента.
            Models.Login.Dir.DirCustomer dirCustomer = con.DirCustomers.Where(x => x.DirCustomersID == DirCustomersID).ToList()[0];
            dirCustomer.Confirmed = true;
            dirCustomer.Active = true;
            //Меняем: Подтверждён и Активен
            con.Entry(dirCustomer).State = EntityState.Modified;
            con.SaveChanges();
        }

        private void SendMail()
        {
            //Class.Functions.FunMailSend funMailSend = new Class.Functions.FunMailSend();
            Classes.Function.FunctionMSSQL.FunMailSend funMailSend = new Classes.Function.FunctionMSSQL.FunMailSend();

            funMailSend.SendTo_OtherEMail("Подтверждение регистрации в Web-сервисе 'ВТорговомОблаке'. Складской учет онлайн.", TextMail(), pEMail);

            try { funMailSend.SendTo_ESklad24("Подтверждение регистрации.", "Login: " + pLogin + "<br />Клиент получил следующий текст:<hr />" + TextMail()); }
            catch { }
        }

        private string TextMail()
        {
            string _Data =
                "Здравствуйте, Мы благодарим Вас за подтверждение регистрации в сервисе ВТорговомОблаке." +
                "В течение 30 дней после регистрации вы можете бесплатно пользоваться всеми возможностями сервиса.<br /><br />" +

                "Для начала работы перейдите по ссылке:<br />" +
                "<a href='https://sklad.intradecloud.com/account/login/?username=admin@" + pLogin + "&password=" + pPswd + "&language=" + pDirLanguageID + "&theme=1'>https://sklad.intradecloud.com/account/login/?username=admin@" + pLogin + "&password=" + pPswd + "&language=" + pDirLanguageID + "&theme=1</a><br /><br />" +

                "Или войдите <a href='https://sklad.intradecloud.com/account/login/'>ВТорговомОблаке</a>, используя ваши учетные данные:<br />" +
                "Ваш Логин: <b>admin@" + pLogin + "</b><br />" +
                "Ваш Пароль: <b>" + pPswd + "</b> (можно поменять в сервисе ВТорговомОблаке: Справочники -> Сотрудники)<br /><br />" +

                "<a href='https://intradecloud.com/help'>Документация</a> " + " | " + " <a href='https://intradecloud.com/videos/'>Видео</a>" +
                "<br /><br />";


            /*
            //"<hr>" +
            //"<img src='http://www.intradecloud.com/images/partner_work.png' alt='Партнёрская программа' align='left' width='5%' height='5%' />" +
            _Data += "У нас действует простая <b><a href='http://www.intradecloud.com/" + IdToString(pDirLanguageID) + "partner'>Партнёрская программа</a></b>!<br /> " +
            "Вы получаете <b>35%</b> комиссии со всех оплат клиентов, которых Вы привели.<br /><br /> " +
            //"Процент Ваших отчислений будет постоянно расти, исходя из общего количества пришедших и оплативших услуги клиентов по Вашей реферальной гиперсылке.<br /> " +

            "Ваша партнерская ссылка для заработка на новых клиентах: " +
            "http://www.intradecloud.com/" + IdToString(pDirLanguageID) + "registration?ref=" + RoundID() + DirCustomersID.ToString() + "<br /> " +

            "Панель 'Партнёрской программы': " +
            "http://www.intradecloud.com/PanelCustomers/" + " (Логин: " + pLogin + ", Пароль: " + pPswd + ")<br /><br /> " +

            //"Желаем Вам больших и стабильных заработков вместе с нами!<br /><br /> " +
            "<hr>";
            */

            _Data += "Мы всегда готовы ответить на ваши вопросы:<br /> " +
            "support@intradecloud.com<br /> " +
            "МТС: +38-050-950-96-49<br /><br /> " +

            "С уважением команда сервиса ВТорговомОблаке.<br /> " +
            "http://www.intradecloud.com/<br /> " +
            "https://www.facebook.com/intradecloud.com<br /> " +
            "https://vk.com/intradecloud<br /> ";


            switch (pDirLanguageID)
            {
                case "2":
                    _Data =
                        "Вітаю, Ми дякуємо Вам за підтвердження реєстрації в сервісі ВТорговомОблаке." +
                        "Протягом 30 днів після реєстрації ви можете безкоштовно користуватися всіма можливостями сервісу.<br /><br />" +

                        "Для початку роботи перейдіть за посиланням:<br />" +
                        "<a href='https://sklad.intradecloud.com/account/login/?username=admin@" + pLogin + "&password=" + pPswd + "&language=" + pDirLanguageID + "&theme=1'>https://sklad.intradecloud.com/account/login/?username=admin@" + pLogin + "&password=" + pPswd + "&language=" + pDirLanguageID + "&theme=1</a><br /><br />" +

                        "Або ввійдіть <a href='https://sklad.intradecloud.com/account/login/'>ВТорговомОблаке</a>, використовуючи ваші облікові дані:<br />" +
                        "Ваш Логін: <b>admin@" + pLogin + "</b><br />" +
                        "Ваш Пароль: <b>" + pPswd + "</b> (можна поміняти в сервісі ВТорговомОблаке: Довідники -> Співробітники)<br /><br />" +

                        "<a href='https://intradecloud.com/help'>Документація</a> " + " | " + " <a href='https://intradecloud.com/videos'>Відео</a>" +
                        "<br /><br />";


                    //"<hr>" +
                    //"<img src='http://www.intradecloud.com/images/partner_work.png' alt='Партнёрская программа' align='left' width='5%' height='5%' />" +
                    _Data += "У нас діє проста <b><a href='http://www.intradecloud.com/" + IdToString(pDirLanguageID) + "partner'>Партнерська програма</a></b>!<br /> " +
                    "Ви отримуєте <b>35%</b> комісії з усіх оплат клієнтів, яких Ви привели.<br /><br /> " +
                    //"Відсоток Ваших відрахувань буде постійно зростати, виходячи із загальної кількості тих, що прийшли і сплатили послуги клієнтів по Вашій реферального гіперлінку.<br /> " +

                    "Ваша партнерськє гіперпосилання для заробітку на нових клієнтів: " +
                    "http://www.intradecloud.com/" + IdToString(pDirLanguageID) + "registration?ref=" + RoundID() + DirCustomersID.ToString() + "<br /> " +

                    "Панель 'Партнерської програми': " +
                    "http://www.intradecloud.com/PanelCustomers/" + " (Логін: " + pLogin + ", Пароль: " + pPswd + ")<br /><br /> " +

                    //"Бажаємо Вам великих і стабільних заробітків разом з нами!<br /><br /> " +
                    "<hr>";


                    _Data += "Ми завжди готові відповісти на ваші запитання:<br /> " +
                    "support@intradecloud.com<br /> " +
                    "МТС: +38-050-950-96-49<br /><br /> " +

                    "З повагою команда сервісу ВТорговомОблаке.<br /> " +
                    "http://www.intradecloud.com/<br /> " +
                    "https://www.facebook.com/intradecloud.com<br /> " +
                    "https://vk.com/intradecloud<br /> ";
                    break;

                case "3":
                    _Data =
                        "Hello, Thank you for the confirmation of registration." +
                        "Within 30 days after registration you can make free use of all the features of the service.<br /><br />" +

                        "To get started, go to:<br />" +
                        "<a href='https://sklad.intradecloud.com/account/login/?username=admin@" + pLogin + "&password=" + pPswd + "&language=" + pDirLanguageID + "&theme=1'>https://sklad.intradecloud.com/account/login/?username=admin@" + pLogin + "&password=" + pPswd + "&language=" + pDirLanguageID + "&theme=1</a><br /><br />" +

                        "Or sign <a href='https://sklad.intradecloud.com/account/login/'>ВТорговомОблаке</a>, using your credentials:<br />" +
                        "Your Login: <b>admin@" + pLogin + "</b><br />" +
                        "Your Password: <b>" + pPswd + "</b> (You can change: Directories -> Employees)<br /><br />" +

                        "<a href='https://intradecloud.com/help'>Documentation</a> " + " | " + " <a href='https://intradecloud.com/videos'>Video</a>" +
                        "<br /><br />";


                    //"<hr>" +
                    //"<img src='http://www.intradecloud.com/images/partner_work.png' alt='Партнёрская программа' align='left' width='5%' height='5%' />" +
                    _Data += "We have a simple <b><a href='http://www.intradecloud.com/" + IdToString(pDirLanguageID) + "partner'>Affiliate Program</a></b>!<br /> " +
                    "You receive <b>35%</b> commission on all payments the customers you brought.<br /><br /> " +
                    //"The percentage of your contributions will continue to grow based on the total number who came and paid services customers for your referral gipersylke.<br /> " +

                    "Your affiliate link to earn money for new customers: " +
                    "http://www.intradecloud.com/" + IdToString(pDirLanguageID) + "registration?ref=" + RoundID() + DirCustomersID.ToString() + "<br /> " +

                    "Panel 'Affiliate Program': " +
                    "http://www.intradecloud.com/PanelCustomers/" + " (Login: " + pLogin + ", Password: " + pPswd + ")<br /><br /> " +

                    //"We have a great and stable earnings with us!<br /><br /> " +
                    "<hr>";


                    _Data += "We are always ready to answer your questions:<br /> " +
                    "support@intradecloud.com<br /> " +
                    "МТС: +38-050-950-96-49<br /><br /> " +

                    "Sincerely service team.<br /> " +
                    "http://www.intradecloud.com/<br /> " +
                    "https://www.facebook.com/intradecloud.com<br /> " +
                    "https://vk.com/intradecloud<br /> ";
                    break;
            }


            return _Data;

        }


        #region Кодирование

        string ReturnRandom(int Size)
        {
            string ret = "";

            string[] mSymbol = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                 "A", "a", "B", "b", "C", "c", "D", "d", "E", "e", "F", "f", "G", "g", "H", "h", "I", "i", "J", "j", "K", "k", "L", "l", "M", "m",
                                 "N", "n", "O", "o", "P", "p", "Q", "q", "R", "r", "S", "s", "T", "t", "U", "u", "V", "v", "W", "w", "X", "x", "Y", "y", "Z", "z",
                                 "E", "U", "R"
                               };
            Random r = new Random();
            int temp = 0;
            for (int i = 0; i < Size; i++)
            {
                temp = r.Next(61);
                ret += mSymbol[temp];
            }

            return ret;
        }
        string ReturnInteger(string Num)
        {
            string ret = "";

            string[] mSymbol = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                 "A", "a", "B", "b", "C", "c", "D", "d", "E", "e", "F", "f", "G", "g", "H", "h", "I", "i", "J", "j", "K", "k", "L", "l", "M", "m",
                                 "N", "n", "O", "o", "P", "p", "Q", "q", "R", "r", "S", "s", "T", "t", "U", "u", "V", "v", "W", "w", "X", "x", "Y", "y", "Z", "z",
                                 "E", "U", "R"
                               };
            StringBuilder sb = new StringBuilder(Num);
            for (int i = 0; i < sb.Length; i++)
            {
                ret += mSymbol[10 + Convert.ToInt16(sb[i].ToString())];
            }

            return ret;
        }
        int ReturnIntegerConvert(string Num)
        {
            int ret = 0;

            string[] mSymbol = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                 "A", "a", "B", "b", "C", "c", "D", "d", "E", "e", "F", "f", "G", "g", "H", "h", "I", "i", "J", "j", "K", "k", "L", "l", "M", "m",
                                 "N", "n", "O", "o", "P", "p", "Q", "q", "R", "r", "S", "s", "T", "t", "U", "u", "V", "v", "W", "w", "X", "x", "Y", "y", "Z", "z",
                                 "E", "U", "R"
                               };
            for (int i = 0; i < mSymbol.Length; i++)
            {
                if (Num == mSymbol[i]) { ret = i - 10; break; }
            }

            return ret;
        }

        #endregion

        public string IdToString(string pDirLanguageID)
        {
            switch (pDirLanguageID)
            {
                case "1": return ""; //ru/
                case "2": return "ua/";
                case "3": return "by/";
                case "4": return "us/";
                case "5": return "az/";
                case "6": return "lt/";
                case "7": return "lv/";
                case "8": return "ee/";
                case "9": return "hu/";
                case "10": return "pl/";
                case "11": return "sk/";
                case "12": return "si/";
                case "13": return "cz/";
                case "14": return "de/";
                case "15": return "fr/";
                case "16": return "ro/";
                default: return "";
            }
        }

        public string RoundID()
        {
            Random random = new Random();
            return
                random.Next(1, 9).ToString() +
                random.Next(1, 9).ToString() +
                random.Next(1, 9).ToString() +
                random.Next(1, 9).ToString() +
                random.Next(1, 9).ToString();
        }

    }
}