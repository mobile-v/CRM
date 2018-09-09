using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography; //.MD5;
using System.Text;
using System.Net;
using RestSharp;
using System.Web.Script.Serialization;
using Newtonsoft.Json;


namespace PartionnyAccount.Classes.SMS
{
    public class infobip_com
    {
        //C#
        //https://www.infobip.com/ru/platforma/rassylki/sms
        //https://dev.infobip.com/docs/send-single-sms
        //Help
        //https://dev.infobip.com/docs/send-sms-response
        //Auto
        //https://dev.infobip.com/docs/api-key-create

        internal string Send(Models.Sklad.Sys.SysSetting sysSetting, string TelTo, string Message)
        {
            try
            {
                //Base64 encoded string
                string SmsLogin = sysSetting.SmsLogin,
                       SmsPassword = sysSetting.SmsPassword;

                //KeyBase64 = "Ym9uaXRvOk0lYzlaMiRj";
                string KeyBase64 = Base64Encode(SmsLogin + ":" + SmsPassword);


                var client = new RestClient("https://api.infobip.com/sms/1/text/single");

                var request = new RestRequest(Method.POST);
                request.AddHeader("accept", "application/json");
                request.AddHeader("content-type", "application/json");

                //request.AddHeader("authorization", "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==");
                request.AddHeader("authorization", "Basic " + KeyBase64 + "");

                //request.AddParameter("application/json", "{\"from\":\"InfoSMS\", \"to\":\"41793026727\",\"text\":\"Test SMS.\"}", ParameterType.RequestBody);
                request.AddParameter("application/json",
                    "{" +
                    "\"from\":\"" + sysSetting.SmsTelFrom + "\"," +
                    " \"to\":\"" + TelTo + "\"," +
                    "\"text\":\"" + Message + "\"" +  //"\"text\":\""+ Message + "\"" + 
                    "}", ParameterType.RequestBody);


                IRestResponse response = client.Execute(request);


                //Чисто для примера
                string jsn =
                "{" +
                "  'messages':" +
                "  [" +
                "   {" +
                "     'to':'79222222222'," +
                "     'status':" +
                "        {" +
                "          'groupId':1," +
                "          'groupName':" +
                "          'PENDING'," +
                "          'id':7," +
                "          'name':'PENDING_ENROUTE'," +
                "          'description':'Message sent to next instance'" +
                "        }," +
                "     'smsCount':1," +
                "     'messageId':'4ad104e0-2222-2c2a-ab22-d22dd2c222ce'" +
                "   }" +
                "  ]" +
                "}";



                string sReturn = "Ответ:<br /> ";

                if (response.StatusCode.ToString().ToLower() == "unauthorized")
                {
                    sReturn = "SMS InfoBip.com: Логин или Пароль не верны! Сообщение сервера: " + response.StatusCode.ToString();
                }
                else
                {
                    string sContent = response.Content;
                    var Otvet = JsonConvert.DeserializeObject<Rootobject>(sContent);

                    sReturn = Otvet.messages[0].status.groupName + ": " + Otvet.messages[0].status.description;
                }


                //Получить ответ из "response" и отправить его!
                return sReturn; // response.StatusCode.ToString();
            }
            catch (Exception ex) { return ex.Message; }

        }


        #region Ответ с infobip.com

        public class Rootobject
        {
            public Message[] messages { get; set; }
        }

        public class Message
        {
            public string to { get; set; }
            public Status status { get; set; }
            public int smsCount { get; set; }
            public string messageId { get; set; }
        }

        public class Status
        {
            public int groupId { get; set; }
            public string groupName { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }

        #endregion


        // *** *** *** Base64 *** *** ***
        //Encode
        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        //Decode
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}