using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading.Tasks;
using PartionnyAccount.Models;
using PartionnyAccount.Models.Sklad.Dir;
using PartionnyAccount.Models.Sklad.Doc;

namespace PartionnyAccount.Classes.Function
{
    internal class Function
    {
        internal void NumberDecimalSeparator()
        {
            var format = new System.Globalization.NumberFormatInfo();
            if (format.NumberDecimalSeparator == ",") format.NumberDecimalSeparator = ".";
        }


        #region Format: Date, Dooble

        internal string ReturnDate(string pDate)
        {
            try { pDate = Convert.ToDateTime(pDate).ToString("yyyy-MM-dd") + " 00:00:00"; }
            catch { pDate = null; }

            return pDate;
        }
        internal string ReturnDate(DateTime pDate)
        {
            try { return pDate.ToString("yyyy-MM-dd"); }
            catch { return "01.01.1800"; }
        }

        internal DateTime ReturnCorrectDate(DateTime? pDate)
        {
            try
            {
                return Convert.ToDateTime(Convert.ToDateTime(pDate).ToString("yyyy-MM-dd"));
            }
            catch { }
            return Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
        }

        //Что бы коректно "вытягивало" данные из Истории (секунда + 1)
        internal DateTime ReturnHistorytDate(DateTime? pDate)
        {
            try
            {
                return Convert.ToDateTime(Convert.ToDateTime(pDate).ToString("yyyy-MM-dd 00:00:01"));
            }
            catch { }
            return Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:01"));
        }

        internal double Return_Dooble(string pDooble)
        {
            System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            try { return Convert.ToDouble(pDooble); }
            catch { return 0; }
        }

        internal string Return_DoobleToString(string pDooble)
        {
            System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";

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

        #endregion


        internal bool ExceptionFkExist(Exception ex)
        {
            string sMsg = ex.Message.ToLower();
            if (ex.InnerException != null) sMsg = ex.InnerException.Message.ToLower();
            if (ex.InnerException.InnerException != null) sMsg = ex.InnerException.InnerException.Message.ToLower();

            if (sMsg.IndexOf("constraint failed") > -1 && sMsg.IndexOf("foreign key") > -1) return true;

            return false;
        }

        internal int iLanguage(HttpCookie authCookie)
        {
            try { return Convert.ToInt32(authCookie["CookieL"]) - 1; }
            catch { return 0; }
        }

        //Если пользователь воспользовался поиском.
        //FieldName-поле по каторому ищем, parSearch-текст поиска. ("WHERE lower(SysDirConstantsName) LIKE @SysDirConstantsName";)
        //Для SQLite
        internal SQLiteParameter[] parCollection = null;
        internal string Search_WhereLike(string pSQL, string parSearch, string[] FieldName, string[] ParamName, string[] FieldType, bool CloseTag) //SQLiteCommand cmd, 
        {
            string SQL = "";
            if (parSearch != null && parSearch != "")
            {

                /*if (FieldName.Length > 0)
                {
                    string _pSQL = pSQL.ToLower();
                    if (_pSQL.Contains(" where ")) { SQL = "and("; }
                    else { SQL = " WHERE ("; }
                }*/
                SQL = "and(";

                //SQLiteParameter[] parCollection = new SQLiteParameter[FieldName.Length];
                parCollection = new SQLiteParameter[FieldName.Length];
                bool i_bool = false; //хотя бы одна запись занеслать в SQL
                for (int i = 0; i < FieldName.Length; i++)
                {
                    if (FieldType[i].ToLower() == "int")
                    {
                        try
                        {
                            Regex rxNums = new Regex(@"^\d+$");
                            if (rxNums.IsMatch(parSearch))
                            {
                                int i_parSearch = Convert.ToInt32(parSearch);

                                if (i_bool) SQL += "or";
                                SQL += "(" + FieldName[i] + "=@" + ParamName[i] + "1)";
                                parCollection[i] = new SQLiteParameter("@" + ParamName[i] + "1", System.Data.DbType.Int32) { Value = parSearch.ToLower() };
                                i_bool = true;
                            }
                        }
                        catch { }
                    }
                    else if (FieldType[i].ToLower() == "int64")
                    {
                        try
                        {
                            Int64 i_parSearch = Convert.ToInt64(parSearch);

                            if (i_bool) SQL += "or";
                            SQL += "(" + FieldName[i] + "=@" + ParamName[i] + "1)";
                            parCollection[i] = new SQLiteParameter("@" + ParamName[i] + "1", System.Data.DbType.Int64) { Value = parSearch.ToLower() };
                            i_bool = true;
                        }
                        catch { }
                    }
                    else if (FieldType[i].ToLower() == "date")
                    {
                        try
                        {
                            DateTime dateTime = new DateTime();
                            bool bResultDateTime = DateTime.TryParse(parSearch.ToLower(), out dateTime);
                            if (bResultDateTime)
                            {
                                if (i_bool) SQL += "or";
                                SQL += "(" + FieldName[i] + "=@" + ParamName[i] + "1)";
                                parCollection[i] = new SQLiteParameter("@" + ParamName[i] + "1", System.Data.DbType.Date) { Value = dateTime };
                                i_bool = true;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        if (i_bool) SQL += "or";
                        if (FieldName[i].IndexOf("Lower") != -1 || FieldName[i].IndexOf("Article") != -1) SQL += "(" + FieldName[i] + " LIKE @" + ParamName[i] + "1)"; //Не использовать тормозную функцию "lower(...)"
                        else SQL += "(lower(" + FieldName[i] + ") LIKE @" + ParamName[i] + "1)";
                        parCollection[i] = new SQLiteParameter("@" + ParamName[i] + "1", System.Data.DbType.String) { Value = "%" + parSearch.ToLower() + "%" };
                        i_bool = true;
                    }

                    if (parCollection[i] != null)
                    {
                        //cmd.Parameters.Add(parCollection[i]);
                    }
                }
                //pSQL = "PRAGMA case_sensitive_like=ON; " + pSQL + SQL; // +")";
                pSQL = pSQL + SQL; // +")";
                if (CloseTag) pSQL += ")";
            }
            return pSQL;
        }


        #region Получить Все группы

        //Получить все ПодГруппы Группы
        //pID = "DirNomen", значит имя группы будет pID + "Group"
        internal class AllSubGroups
        {
            public int ID;
            public bool Read = false;
        }
        internal ArrayList SelectAllSubGroups(SQLiteConnection con, string TableNames, string TableName, int GroupID)
        {
            ArrayList alRet = new ArrayList();

            //Вносим в массив первый элемент - Группу "GroupID"
            AllSubGroups _allSubGroups = new AllSubGroups();
            _allSubGroups.ID = GroupID;
            _allSubGroups.Read = false;
            alRet.Add(_allSubGroups);

            int iCount = 0; //На всязкий случае, если всё таки зациклит
            while (true)
            {
                if (SelectSubGroups(con, TableNames, TableName, alRet) == 0) break;

                //На всязкий случае, если всё таки зациклит
                iCount++;
                if (iCount > 200) break;
            }

            return alRet;
        }
        private int SelectSubGroups(SQLiteConnection con, string TableNames, string TableName, ArrayList alRet)
        {
            int iCount = 0;

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT " + TableName + "ID FROM " + TableNames + " WHERE Sub=@GroupID", con))
            {
                int forCount = alRet.Count;
                ArrayList alMas = new ArrayList(); //Для "Read = true"
                for (int i = 0; i < forCount; i++)
                {
                    AllSubGroups _allSubGroups = (AllSubGroups)alRet[i];

                    //Если этот "GroupID" ещё не обработали
                    if (!_allSubGroups.Read)
                    {
                        cmd.Parameters.Clear();
                        SQLiteParameter parGroupID = new SQLiteParameter("@GroupID", System.Data.DbType.Int32) { Value = _allSubGroups.ID }; cmd.Parameters.Add(parGroupID);
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                AllSubGroups _allSubGroups2 = new AllSubGroups();
                                _allSubGroups2.ID = Convert.ToInt32(dr[TableName + "ID"].ToString());
                                _allSubGroups2.Read = false;
                                alRet.Add(_allSubGroups2);

                                iCount++;
                            }
                        }
                        alMas.Add(i);
                    }

                    /*alRet.RemoveAt(i);
                    AllSubGroups _allSubGroups3 = new AllSubGroups();
                    _allSubGroups3.ID = _allSubGroups3.ID;
                    _allSubGroups3.Read = true;
                    alRet.Add(_allSubGroups3);*/
                }

                //Ставим "Read = true"
                for (int i = 0; i < alMas.Count; i++)
                {
                    int iMas = Convert.ToInt32(alMas[i].ToString());
                    AllSubGroups _allSubGroups = (AllSubGroups)alRet[iMas];
                    _allSubGroups.Read = true;
                    alRet[iMas] = _allSubGroups;
                }
            }

            return iCount;
        }

        #endregion

        //Проверить, есть ли русские буквы в строке c# или не латиница
        internal bool IsStringLatin(string content)
        {
            bool result = false;

            char[] letters = content.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                int charValue = System.Convert.ToInt32(letters[i]);

                if (charValue > 128)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }


        #region СМС Парсинг: вставляем метаданные в текст СМС

        internal async Task<string> mSms_DocServicePurches(
            DbConnectionSklad db,
            DirSmsTemplate dirSmsTemplate,
            DocServicePurch docServicePurch,
            Models.Sklad.Sys.SysSetting sysSetting
            )
        {
            string ret = dirSmsTemplate.DirSmsTemplateMsg;

            //Парсим текст

            //Ваш (тип устройства/марка/модель) Mob.tel/Nokia/535 принят в ремонт, номер ремонта 2005
            //Мастерская Сити-Мастер
            //тел (тут вставляем номер тел точки чтоб чел мог набрать)


            //1. DirServiceNomenName: надо получить полное наименование
            Controllers.Sklad.Doc.DocServicePurches.DocServicePurchesController docServicePurchesController = new Controllers.Sklad.Doc.DocServicePurches.DocServicePurchesController();
            string[] DirServiceNomenPatchFullarr = await Task.Run(() => docServicePurchesController.mPatchFull(db, docServicePurch.DirServiceNomenID));
            string DirServiceNomenPatchFull = DirServiceNomenPatchFullarr[0];

            ret = ret.Replace("[[[ТоварНаименование]]]", DirServiceNomenPatchFull);


            //2. DocServicePurchID
            ret = ret.Replace("[[[ДокументНомер]]]", docServicePurch.DocServicePurchID.ToString());


            //3. Организация: получаем из настроек
            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(sysSetting.DirContractorIDOrg);
            ret = ret.Replace("[[[Организация]]]", dirContractor.DirContractorName);


            //4. Телефон: получаем из "docServicePurch.DirWarehouseID"
            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docServicePurch.DirWarehouseID);
            ret = ret.Replace("[[[ТочкаНаименование]]]", dirWarehouse.DirWarehouseName);
            ret = ret.Replace("[[[ТочкаАдрес]]]", dirWarehouse.DirWarehouseAddress);
            ret = ret.Replace("[[[ТочкаТелефон]]]", dirWarehouse.Phone);


            //5. [[[Сумма]]]
            ret = ret.Replace("[[[Сумма]]]", docServicePurch.Sums.ToString()); //SumsTotal



            return ret;
        }

        #endregion



    }
}