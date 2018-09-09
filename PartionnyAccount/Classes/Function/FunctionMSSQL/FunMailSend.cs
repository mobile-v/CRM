using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace PartionnyAccount.Classes.Function.FunctionMSSQL
{
    public class FunMailSend
    {
        #region Переменные
        string smtp2 = "smtp.mail.ru",
            login2 = "esklad24",
            password2 = "kraftfudz",
            FromMail2 = "esklad24@mail.ru",
            MyMail2 = "esklad24@mail.ru";
        int Port2 = 25;

        string smtp = "smtp.yandex.ru",
            login = "noreply@intradecloud.com",
            password = "250RubInMount",
            FromMail = "noreply@intradecloud.com",
            MyMail = "esklad24@mail.ru";
        int Port = 25; //465

        /*string smtp = "mail.infobox.ru",
            login = "noreply@intradecloud.com",
            password = "250RubInMount",
            FromMail = "noreply@intradecloud.com",
            MyMail = "esklad24@mail.ru";
        int Port = 25;*/

        /*string smtp = "mail.infobox.ru",
            login = "support@intradecloud.com",
            password = "250RubInMount",
            FromMail = "support@intradecloud.com",
            MyMail = "esklad24@mail.ru";
        int Port = 25;*/
        #endregion

        #region Отправка

        //Отправить Себе Любимому
        public void SendTo_ESklad24(string subject, string txt)
        {
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp, Port);
            Smtp.EnableSsl = true;
            Smtp.UseDefaultCredentials = false;
            Smtp.Credentials = new NetworkCredential(login, password);

            //Формирование письма
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(FromMail);
            Message.To.Add(new MailAddress(MyMail));
            Message.Subject = subject;
            Message.IsBodyHtml = true;
            Message.Body = "InTradeCloud.Com <br /> " + txt;

            //Smtp.Send(Message);//отправка
            //отправка
            try { Smtp.Send(Message); }
            catch
            {
                try { Smtp.Send(Message); }
                catch
                {
                    try { Smtp.Send(Message); }
                    catch
                    {
                        try { Smtp.Send(Message); }
                        catch
                        {
                            try { Smtp.Send(Message); }
                            catch
                            {
                                try { Smtp.Send(Message); }
                                catch
                                {
                                    try { Smtp.Send(Message); }
                                    catch
                                    {
                                        try { Smtp.Send(Message); }
                                        catch
                                        {
                                            try { Smtp.Send(Message); }
                                            catch
                                            {
                                                try { Smtp.Send(Message); }
                                                catch
                                                {
                                                    try { Smtp.Send(Message); }
                                                    catch
                                                    {
                                                        try { Smtp.Send(Message); }
                                                        catch
                                                        {
                                                            try { Smtp.Send(Message); }
                                                            catch
                                                            {
                                                                try { Smtp.Send(Message); }
                                                                catch
                                                                {
                                                                    try { Smtp.Send(Message); }
                                                                    catch
                                                                    {
                                                                        try { Smtp.Send(Message); }
                                                                        catch
                                                                        {
                                                                            try { Smtp.Send(Message); }
                                                                            catch
                                                                            {
                                                                                try { Smtp.Send(Message); }
                                                                                catch
                                                                                {
                                                                                    try { Smtp.Send(Message); }
                                                                                    catch
                                                                                    {
                                                                                        try { Smtp.Send(Message); }
                                                                                        catch { }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Отправка Клиенту
        public void SendTo_OtherEMail(string subject, string txt, string SendToEMail)
        {
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp, Port);
            Smtp.EnableSsl = true;
            Smtp.UseDefaultCredentials = false;
            Smtp.Credentials = new NetworkCredential(login, password);

            //Формирование письма
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(FromMail);
            Message.To.Add(new MailAddress(SendToEMail));
            Message.Subject = subject;
            Message.IsBodyHtml = true;
            Message.Body = txt;

            //Smtp.Send(Message);//отправка
            //отправка
            try { Smtp.Send(Message); }
            catch
            {
                try { Smtp.Send(Message); }
                catch
                {
                    try { Smtp.Send(Message); }
                    catch
                    {
                        try { Smtp.Send(Message); }
                        catch
                        {
                            try { Smtp.Send(Message); }
                            catch
                            {
                                try { Smtp.Send(Message); }
                                catch
                                {
                                    try { Smtp.Send(Message); }
                                    catch
                                    {
                                        try { Smtp.Send(Message); }
                                        catch
                                        {
                                            try { Smtp.Send(Message); }
                                            catch
                                            {
                                                try { Smtp.Send(Message); }
                                                catch
                                                {
                                                    try { Smtp.Send(Message); }
                                                    catch
                                                    {
                                                        try { Smtp.Send(Message); }
                                                        catch
                                                        {
                                                            try { Smtp.Send(Message); }
                                                            catch
                                                            {
                                                                try { Smtp.Send(Message); }
                                                                catch
                                                                {
                                                                    try { Smtp.Send(Message); }
                                                                    catch
                                                                    {
                                                                        try { Smtp.Send(Message); }
                                                                        catch
                                                                        {
                                                                            try { Smtp.Send(Message); }
                                                                            catch
                                                                            {
                                                                                try { Smtp.Send(Message); }
                                                                                catch
                                                                                {
                                                                                    try { Smtp.Send(Message); }
                                                                                    catch
                                                                                    {
                                                                                        try { Smtp.Send(Message); }
                                                                                        catch (Exception ex) { throw new System.InvalidOperationException(ex.Message); }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            //Отправка Клиенту если GMail.Com
            /*if (SendToEMail.IndexOf("gmail.com") != -1)
            {
                SendTo_OtherEMail_MailRu(subject, txt, SendToEMail);
            }*/
        }

        //Отправка Клиенту если GMail.Com
        public void SendTo_OtherEMail_MailRu(string subject, string txt, string SendToEMail)
        {
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp2, Port2);
            Smtp.EnableSsl = true;
            Smtp.UseDefaultCredentials = false;
            Smtp.Credentials = new NetworkCredential(login2, password2);

            //Формирование письма
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(FromMail2);
            Message.To.Add(new MailAddress(SendToEMail));
            Message.Subject = subject;
            Message.IsBodyHtml = true;
            Message.Body = txt;

            //Smtp.Send(Message);//отправка
            //отправка
            try { Smtp.Send(Message); }
            catch
            {
                try { Smtp.Send(Message); }
                catch
                {
                    try { Smtp.Send(Message); }
                    catch
                    {
                        try { Smtp.Send(Message); }
                        catch
                        {
                            try { Smtp.Send(Message); }
                            catch
                            {
                                try { Smtp.Send(Message); }
                                catch
                                {
                                    try { Smtp.Send(Message); }
                                    catch
                                    {
                                        try { Smtp.Send(Message); }
                                        catch
                                        {
                                            try { Smtp.Send(Message); }
                                            catch
                                            {
                                                try { Smtp.Send(Message); }
                                                catch
                                                {
                                                    try { Smtp.Send(Message); }
                                                    catch
                                                    {
                                                        try { Smtp.Send(Message); }
                                                        catch
                                                        {
                                                            try { Smtp.Send(Message); }
                                                            catch
                                                            {
                                                                try { Smtp.Send(Message); }
                                                                catch
                                                                {
                                                                    try { Smtp.Send(Message); }
                                                                    catch
                                                                    {
                                                                        try { Smtp.Send(Message); }
                                                                        catch
                                                                        {
                                                                            try { Smtp.Send(Message); }
                                                                            catch
                                                                            {
                                                                                try { Smtp.Send(Message); }
                                                                                catch
                                                                                {
                                                                                    try { Smtp.Send(Message); }
                                                                                    catch
                                                                                    {
                                                                                        try { Smtp.Send(Message); }
                                                                                        catch { }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SendTo_OtherEMail_File(string subject, string Text, string SendToEMail, string pachFile)
        {
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp, Port);
            Smtp.EnableSsl = true;
            Smtp.UseDefaultCredentials = false;
            Smtp.Credentials = new NetworkCredential(login, password);

            //Формирование письма
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(FromMail);
            Message.To.Add(new MailAddress(SendToEMail));
            Message.Subject = subject;
            Message.IsBodyHtml = true;
            Message.Body = Text;

            //Прикрепляем файл
            if (pachFile != "")
            {
                string file = pachFile;
                Attachment attach = new Attachment(file, MediaTypeNames.Application.Octet);

                // Добавляем информацию для файла
                ContentDisposition disposition = attach.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(file);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(file);

                Message.Attachments.Add(attach);
            }

            //Smtp.Send(Message);//отправка
            //отправка
            try { Smtp.Send(Message); }
            catch
            {
                try { Smtp.Send(Message); }
                catch
                {
                    try { Smtp.Send(Message); }
                    catch
                    {
                        try { Smtp.Send(Message); }
                        catch
                        {
                            try { Smtp.Send(Message); }
                            catch
                            {
                                try { Smtp.Send(Message); }
                                catch
                                {
                                    try { Smtp.Send(Message); }
                                    catch
                                    {
                                        try { Smtp.Send(Message); }
                                        catch
                                        {
                                            try { Smtp.Send(Message); }
                                            catch
                                            {
                                                try { Smtp.Send(Message); }
                                                catch
                                                {
                                                    try { Smtp.Send(Message); }
                                                    catch
                                                    {
                                                        try { Smtp.Send(Message); }
                                                        catch
                                                        {
                                                            try { Smtp.Send(Message); }
                                                            catch
                                                            {
                                                                try { Smtp.Send(Message); }
                                                                catch
                                                                {
                                                                    try { Smtp.Send(Message); }
                                                                    catch
                                                                    {
                                                                        try { Smtp.Send(Message); }
                                                                        catch
                                                                        {
                                                                            try { Smtp.Send(Message); }
                                                                            catch
                                                                            {
                                                                                try { Smtp.Send(Message); }
                                                                                catch
                                                                                {
                                                                                    try { Smtp.Send(Message); }
                                                                                    catch
                                                                                    {
                                                                                        try { Smtp.Send(Message); }
                                                                                        catch { }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}