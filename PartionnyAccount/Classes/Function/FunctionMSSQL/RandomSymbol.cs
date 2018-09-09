using System;
using System.Text;

namespace PartionnyAccount.Classes.Function.FunctionMSSQL
{
    public class RandomSymbol
    {
        public string ReturnRandom(int Size)
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

        public string ReturnInteger(string Num)
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
    }
}