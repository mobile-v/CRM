using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography; //.MD5;
using System.Text;
using System.Net;

namespace PartionnyAccount.Classes.SMS
{
    public class sms48_ru
    {

        //http://sms48.ru/send_sms.php?login=name@domain.ru&to=79103524545&from=Boss&msg=text&check2=sJhsn39smals992slGwd3cls2
        //https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx


        //public string Login = "esklad24@mail.ru", Password = "tel_31154", TelTo = "79257711344", TelFrom = "Siti-Master", Message = "Apparat otremontirovan. Zaberite ego - 1200 RUR";
        const string UrlFormat = "http://sms48.ru/send_sms.php?login={0}&to={1}&from={2}&msg={3}&check2={4}";


        internal string Send(Models.Sklad.Sys.SysSetting sysSetting, string TelTo, string Message)
        {
            /*
            string checksum = ComputeChecksum(Login, Password, TelTo);
            var requestUrl = String.Format(UrlFormat, Login, TelTo, TelFrom, Message, checksum);
            var request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            var response = (HttpWebResponse)request.GetResponse();

            var res = response.GetResponseStream();

            var res2 = res;
            */


            
            //Строка соединения
            //string URI = "http://sms48.ru/send_sms.php";
            //Параметры
            string checksum = ComputeChecksum(sysSetting.SmsLogin, sysSetting.SmsPassword, TelTo);
            var requestUrl = String.Format(UrlFormat, sysSetting.SmsLogin, TelTo, sysSetting.SmsTelFrom, Message, checksum);
            using (WebClient wc = new WebClient())
            {
                //wc.Encoding = System.Text.Encoding.UTF8;
                //wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return wc.UploadString(requestUrl, "");
            }
            
        }

        private string ComputeChecksum(string Login, string Password, string TelTo)
        {
            return CalculateMD5Hash(Login + CalculateMD5Hash(Password) + TelTo);
        }


        private string CalculateMD5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, input);

                return hash;
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

    }
}