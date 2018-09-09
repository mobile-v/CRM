using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace PartionnyAccount.Classes.Account
{
    internal class EncodeDecode
    {
        #region Union

        internal string UnionEncode(string Data)
        {
            //return FreeEncode(Data);

            return ComercialEncode(Data);
        }

        internal string UnionDecode(string Data)
        {
            //return FreeDecode(Data);

            return ComercialDecode(Data);
        }

        #endregion


        #region Free

        #region === Encode ===

        private string FreeEncode(string Data)
        {
            /*string ret = FreeStringAdd();

            //Кодируем
            byte[] EncodeValues = Encoding.Default.GetBytes(Data);
            for (int i = 0; i < EncodeValues.Length; i++)
            {
                int h = EncodeValues[i] + 7;
                ret += h + " ";
            }

            return "" + ret;*/

            string ret = FreeStringAdd();

            //Кодируем
            //byte[] EncodeValues = Encoding.Default.GetBytes(Data);
            byte[] EncodeValues = Encoding.UTF8.GetBytes(Data);
            for (int i = 0; i < EncodeValues.Length; i++)
            {
                int h = EncodeValues[i] + 7;
                ret += h + " ";
            }

            return "" + ret;
        }

        private string FreeStringAdd()
        {
            //1: 21 + к-во левых символов: 214 - 4 левых символов
            //2: 214 358 125 548 211

            string ret = "";

            Random rand1 = new Random(); int n = rand1.Next(8); ret += 21 + n.ToString() + " ";
            Random rand2 = new Random();
            for (int i = 0; i < n; i++)
            {
                ret += rand1.Next(101, 301) + " ";
            }

            return ret;
        }

        #endregion


        #region === Decode ===

        private string FreeDecode(string Data)
        {
            /*string ret = "";

            int n = FreeStringRemove(Data);
            //Расскодируем
            string[] words = Data.Split(' ');
            byte[] DecodeValues = new byte[words.Length - 1 - n];
            for (int i = n; i < words.Length - 1; i++)
            {
                DecodeValues[i - n] = Convert.ToByte(Convert.ToInt32(words[i]) - 7);
            }
            //Преобразовывем расскодирование в Сктринг
            ret = Encoding.Default.GetString(DecodeValues); ;

            return ret;*/

            string ret = "";

            int n = FreeStringRemove(Data);
            //Расскодируем
            string[] words = Data.Split(' ');
            byte[] DecodeValues = new byte[words.Length - 1 - n];
            for (int i = n; i < words.Length - 1; i++)
            {
                DecodeValues[i - n] = Convert.ToByte(Convert.ToInt32(words[i]) - 7);
            }
            //Преобразовывем расскодирование в Сктринг
            //ret = Encoding.Default.GetString(DecodeValues);
            ret = Encoding.UTF8.GetString(DecodeValues);

            return ret;
        }

        private int FreeStringRemove(string Data)
        {
            string[] words = Data.Split(' ');
            return Convert.ToInt32(words[0].Remove(0, 2)) + 1;
        }

        #endregion

        #endregion


        #region Comercial

        #region === Encode ===

        private string ComercialEncode(string Data)
        {
            /*string ret = ComercialStringAdd();

            //Кодируем
            byte[] EncodeValues = Encoding.Default.GetBytes(Data);
            for (int i = 0; i < EncodeValues.Length; i++)
            {
                int h = EncodeValues[i] + 7;
                ret += h + " ";
            }

            return "" + ret;*/

            string ret = FreeStringAdd();

            //Кодируем
            //byte[] EncodeValues = Encoding.Default.GetBytes(Data);
            byte[] EncodeValues = Encoding.UTF8.GetBytes(Data);
            for (int i = 0; i < EncodeValues.Length; i++)
            {
                int h = EncodeValues[i] + 7;
                ret += h + " ";
            }

            return "" + ret;
        }

        private string ComercialStringAdd()
        {
            //1: 21 + к-во левых символов: 214 - 4 левых символов
            //2: 214 358 125 548 211

            string ret = "";

            Random rand1 = new Random(); int n = rand1.Next(8); ret += 21 + n.ToString() + " ";
            Random rand2 = new Random();
            for (int i = 0; i < n; i++)
            {
                ret += rand1.Next(101, 301) + " ";
            }

            return ret;
        }

        #endregion


        #region === Decode ===

        private string ComercialDecode(string Data)
        {
            /*string ret = "";

            int n = ComercialStringRemove(Data);
            //Расскодируем
            string[] words = Data.Split(' ');
            byte[] DecodeValues = new byte[words.Length - 1 - n];
            for (int i = n; i < words.Length - 1; i++)
            {
                DecodeValues[i - n] = Convert.ToByte(Convert.ToInt32(words[i]) - 7);
            }
            //Преобразовывем расскодирование в Сктринг
            ret = Encoding.Default.GetString(DecodeValues);

            return ret;*/

            string ret = "";

            int n = FreeStringRemove(Data);
            //Расскодируем
            string[] words = Data.Split(' ');
            byte[] DecodeValues = new byte[words.Length - 1 - n];
            for (int i = n; i < words.Length - 1; i++)
            {
                DecodeValues[i - n] = Convert.ToByte(Convert.ToInt32(words[i]) - 7);
            }
            //Преобразовывем расскодирование в Сктринг
            //ret = Encoding.Default.GetString(DecodeValues);
            ret = Encoding.UTF8.GetString(DecodeValues);

            return ret;
        }

        private int ComercialStringRemove(string Data)
        {
            string[] words = Data.Split(' ');
            return Convert.ToInt32(words[0].Remove(0, 2)) + 1;
        }

        #endregion

        #endregion

    }
}