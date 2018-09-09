using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography; //.MD5;
using System.Text;
using System.Net;

namespace PartionnyAccount.Classes.SMS
{
    public class sms4b_ru
    {
        public string Login = "esklad24", Password = "tel_31154", Phone = "79257711344", Source = "Siti-Master", Text = "Apparat otremontirovan. Zaberite ego - 1200 RUR";
        const string UrlFormat = "https://sms4b.ru/ws/sms.asmx?op=SendSMS&Login={0}&Password={1}&Source={2}&Phone={3}&Text={4}";

        internal string Send()
        {
            /*
            //Строка соединения
            string URI = "https://sms4b.ru/ws/sms.asmx";
            //Параметры
            string myParameters = "op=SendSMS&Login={0}&Password={1}&Source={2}&Phone={3}&Text={4}";

            var myParametersX = String.Format(myParameters, Login, Password, Source, Phone, Text);
            using (WebClient wc = new WebClient())
            {
                //wc.Encoding = System.Text.Encoding.UTF8;
                //wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return wc.UploadString(URI, myParametersX);
            }
            */


            /*
            //Строка соединения
            string URI = "https://sms4b.ru/ws/sms.asmx?op=SendSMS";
            //Параметры
            string myParameters = "op=SendSMS&Login=esklad24&Password=tel_31154&Phone=79257711344&Source=Siti_Master&Text=Apparat";
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return wc.UploadString(URI, myParameters);
            }
            */


            
            string requestUrl = String.Format(UrlFormat, Login, Password, Source, Phone, Text);
            var request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            var response = (HttpWebResponse)request.GetResponse();

            var res = response.GetResponseStream();

            var res2 = res;

            return "";
        }
    }
}