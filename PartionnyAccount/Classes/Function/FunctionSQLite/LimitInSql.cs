using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartionnyAccount.Classes.Function.FunctionSQLite
{
    public class LimitInSql
    {
        public static string Return(string SQL, int limit, int page)
        {
            string firstCount = (limit * (page - 1)).ToString();
            SQL += " LIMIT " + firstCount + ", " + limit;

            return SQL;
        }

        public static string ReturnNotSQL(int limit, int page)
        {
            string firstCount = (limit * (page - 1)).ToString();
            return " LIMIT " + firstCount + ", " + limit;
        }
    }
}