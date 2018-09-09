using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartionnyAccount.Classes.Language.Login
{
    public static class Language
    {
        //Текст отправляемый клиенту, который зарегистрировался
        public static string txtRegistrationMail(PartionnyAccount.Models.Login.Dir.DirCustomer dirCustomers, int iLang)
        {
            //Формируем код подтверждения используя ID-шник, только, что созданой записи:
            //DirCustomersID - ридумать алгоритм работы!
            Classes.Function.FunctionMSSQL.RandomSymbol randomSymbol1 = new Classes.Function.FunctionMSSQL.RandomSymbol(); Classes.Function.FunctionMSSQL.RandomSymbol randomSymbol2 = new Classes.Function.FunctionMSSQL.RandomSymbol(); Classes.Function.FunctionMSSQL.RandomSymbol randomSymbol3 = new Classes.Function.FunctionMSSQL.RandomSymbol();
            string Link = "X" + randomSymbol1.ReturnRandom(8) + randomSymbol2.ReturnInteger(dirCustomers.DirCustomersID.ToString()) + randomSymbol3.ReturnRandom(8);
            //X - типа версия кода подтверрждения (X, Y, Z, ...)
            //8 - левых символов
            //Кодирование кода DirCustomersID
            //8 - левых символов

            string _Data = "";

            switch (iLang)
            {

                case 1:
                    _Data = "Здравствуйте, Мы благодарим Вас за регистрацию в ВТорговомОблаке. " +
                        "Для завершения регистрации Вам надо её подтвердить:<br /><br />" +
                        "<a href='https://sklad.intradecloud.com/account/regconf/?Confirmed=" + Link + "'>https://sklad.intradecloud.com/account/regconf/?Confirmed=" + Link + "</a><br /><br />" +
                        "Или на странице: <a href='https://sklad.intradecloud.com/account/regconf/'>https://sklad.intradecloud.com/account/regconf/</a><br />" +
                        "ввести код подтверждения: " + Link + "<br /><br /><br />" +

                        "Мы всегда готовы ответить на ваши вопросы:<br /> " +
                        "support@intradecloud.com<br /> " +
                        "МТС: +38-050-950-96-49<br /><br /> " +

                        "С уважением команда сервиса ВТорговомОблаке.<br /> " +
                        "http://www.intradecloud.com/<br /> " +
                        "https://www.facebook.com/intradecloud.com<br /> " +
                        "https://vk.com/intradecloud<br /> ";
                    break;

                case 2:
                    _Data = "Вітаю, Ми дякуємо Вам за реєстрацію в ВТорговомОблаке. " +
                        "Для завершення реєстрації Вам треба її підтвердити:<br /><br />" +
                        "<a href='https://sklad.intradecloud.com/account/regconf/?Confirmed=" + Link + "'>https://sklad.intradecloud.com/account/regconf/?Confirmed=" + Link + "</a><br /><br />" +
                        "Або на сторінці: <a href='https://sklad.intradecloud.com/account/regconf/'>https://sklad.intradecloud.com/account/regconf/</a><br />" +
                        "ввести код підтвердження: " + Link + "<br /><br /><br />" +

                        "Ми завжди готові відповісти на ваші запитання:<br /> " +
                        "support@intradecloud.com<br /> " +
                        "МТС: +38-050-950-96-49<br /><br /> " +

                        "З повагою команда сервісу ВТорговомОблаке.<br /> " +
                        "http://www.intradecloud.com/<br /> " +
                        "https://www.facebook.com/intradecloud.com<br /> " +
                        "https://vk.com/intradecloud<br /> ";
                    break;

                case 3:
                    _Data = "Hello, Thank you for registering InTradeCloud. " +
                        "To complete registration, you need to confirm it:<br /><br />" +
                        "<a href='https://sklad.intradecloud.com/account/regconf/?Confirmed=" + Link + "'>https://sklad.intradecloud.com/account/regconf/?Confirmed=" + Link + "</a><br /><br />" +
                        "Or on the page: <a href='https://sklad.intradecloud.com/account/regconf/'>https://sklad.intradecloud.com/account/regconf/</a><br />" +
                        "enter the verification code: " + Link + "<br /><br /><br />" +

                        "We are always ready to answer your questions:<br /> " +
                        "support@intradecloud.com<br /> " +
                        "МТС: +38-050-950-96-49<br /><br /> " +

                        "Sincerely service team InTradeCloud.<br /> " +
                        "http://www.intradecloud.com/<br /> " +
                        "https://www.facebook.com/intradecloud.com<br /> " +
                        "https://vk.com/intradecloud<br /> ";
                    break;

                default:
                    _Data = "Здравствуйте, Мы благодарим Вас за регистрацию в ВТорговомОблаке. " +
                        "Для завершения регистрации Вам надо её подтвердить:<br /><br />" +
                        "<a href='https://sklad.intradecloud.com/account/regconf/?Confirmed=" + Link + "'>https://sklad.intradecloud.com/account/regconf/?Confirmed=" + Link + "</a><br /><br />" +
                        "Или на странице: <a href='https://sklad.intradecloud.com/account/regconf/'>https://sklad.intradecloud.com/account/regconf/</a><br />" +
                        "ввести код подтверждения: " + Link + "<br /><br /><br />" +

                        "Мы всегда готовы ответить на ваши вопросы:<br /> " +
                        "support@intradecloud.com<br /> " +
                        "МТС: +38-050-950-96-49<br /><br /> " +

                        "С уважением команда сервиса ВТорговомОблаке.<br /> " +
                        "http://www.intradecloud.com/<br /> " +
                        "https://www.facebook.com/intradecloud.com<br /> " +
                        "https://vk.com/intradecloud<br /> ";
                    break;
            }

            return _Data;
        }

        public static string error(int iLang)
        {
            switch (iLang)
            {
                case 1: return "Ошибка";
                case 2: return "Помилка";
                case 3: return "Error";
                default: return "Ошибка";
            }
        }


        #region msgXXX

        public static string msg1(int iLang)
        {
            switch (iLang)
            {
                case 1: return "Извините, но такой Логин уже занят! Подберите пожалуйста другой Логин!";
                case 2: return "Вибачте, але такий Логін вже зайнятий! Підберіть будь ласка інший Логін!";
                case 3: return "Sorry, but this login is already taken! Pick another please login!";
                default: return "Извините, но такой Логин уже занят! Подберите пожалуйста другой Логин!";
            }
        }

        public static string msg2(int iLang)
        {
            switch (iLang)
            {
                case 1: return "Регистрация в сервисе ВТорговомОблаке";
                case 2: return "Реєстрація в сервісі ВТорговомОблаке";
                case 3: return "Registration in InTradeCloud";
                default: return "Регистрация в сервисе ВТорговомОблаке";
            }
        }

        public static string msg3(int iLang)
        {
            switch (iLang)
            {
                case 1: return "Клиент получил следующий текст:";
                case 2: return "Клієнт отримав наступний текст:";
                case 3: return "The client received the following text:";
                default: return "Клиент получил следующий текст:";
            }
        }

        public static string msg4(int iLang)
        {
            switch (iLang)
            {
                case 1: return "Вы зарегистрировались, но пока, что Mail Server на профилактике. Ссылка с подтверждением регистрации придёт попозже!<br />Системное сообщение:<br />";
                case 2: return "Ви зареєструвалися, але поки, що Mail Server на профілактиці. Посилання з підтвердженням реєстрації прийде пізніше! <br /> Системне повідомлення: <br />";
                case 3: return "You registered, but until that Mail Server on prevention. Link to the registration confirmation will come later! <br /> The system message: <br />";
                default: return "Вы зарегистрировались, но пока, что Mail Server на профилактике. Ссылка с подтверждением регистрации придёт попозже!<br />Системное сообщение:<br />";
            }
        }

        public static string msg5(int iLang)
        {
            switch (iLang)
            {
                case 1: return "Успешная регистрация в ВТорговомОблаке";
                case 2: return "Успішна реєстрація в ВТорговомОблаке";
                case 3: return "Successful registration InTradeCloud";
                default: return "Успешная регистрация в ВТорговомОблаке";
            }
        }

        public static string msg6(int iLang)
        {
            switch (iLang)
            {
                case 1: return "Если не можете решить проблему, сообщите пожалуйста об этом на email адрес";
                case 2: return "Якщо не можете вирішити проблему, повідомте будь ласка про це на email адреса";
                case 3: return "If you can not solve the problem, please tell us about it in your email address";
                default: return "Если не можете решить проблему, сообщите пожалуйста об этом на email адрес";
            }
        }

        public static string msg7(int iLang)
        {
            switch (iLang)
            {
                case 1: return "Осталось подтвердить регистрацию. Зайдите в email, который Вы указали при регистрацию и кликните по гиперссылке";
                case 2: return "Залишилося підтвердити реєстрацію. Зайдіть в email, який Ви вказали при реєстрації і клацніть по гіперпосиланню";
                case 3: return "Confirm registration - go to email, which you registered and click on the hyperlink";
                default: return "Осталось подтвердить регистрацию. Зайдите в email, который Вы указали при регистрацию и кликните по гиперссылке";
            }
        }

        #endregion
    }
}