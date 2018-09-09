using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace PartionnyAccount.Classes.Function.FunctionSQLite
{
    public class SQLiteDate
    {
        internal string Return(string pDate)
        {
            try { pDate = Convert.ToDateTime(pDate).ToString("yyyy-MM-dd") + " 00:00:00"; }
            catch { pDate = null; }

            return pDate;
        }

        internal string ReturnCorrectDate(string pDate)
        {
            string _ex = "";
            try
            {
                try
                {
                    pDate = Convert.ToDateTime(pDate).ToString("yyyy-MM-dd");
                }
                catch { pDate = DateTime.Now.ToString("yyyy-MM-dd"); }
            }
            catch (Exception ex) { _ex = ex.Message; }
            return pDate;
        }

        internal double Return_Dooble(string pDooble)
        {
            //try {  System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = "."; } catch { return 0; }

            try { return Convert.ToDouble(pDooble); }
            catch { return 0; }
        }

        internal string Return_DoobleToString(string pDooble)
        {
            //try { System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = "."; } catch (Exception ex) { return "0"; }

            try { pDooble = String.Format("{0:0.00}", Convert.ToDouble(pDooble.Replace(",", ".")).ToString()); }
            catch { pDooble = String.Format("{0:0.00}", 0); }

            if (pDooble.IndexOf(".") == -1) pDooble += ".00";

            return pDooble;
        }

        internal string Return_ManyTypesOfDates(string pDate)
        {
            if (String.IsNullOrEmpty(pDate)) return pDate;

            //Если пришло с Точь-проета, то размер даты большой (много "левых" символов)
            if (pDate.Length > 10)
            {
                pDate = pDate.Remove(10);
            }

            string[] dates = new[] { pDate };
            string[] formats = new[] { "yyyy-MM-dd", "dd-MM-yyyy" };
            foreach (string dateIn in dates)
            {
                pDate = DateTime.ParseExact(dateIn, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd");
            }

            return pDate;
        }
    }
}