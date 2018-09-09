using System.Data.SQLite;

namespace PartionnyAccount.Classes.Function.FunctionSQLite
{
    public class CharindexToLike
    {
        public static class InitFunction
        {
            /// <summary>
            /// Initializes the collation UTF8CI 
            /// </summary>
            public static void initFunction()
            {
                SQLiteFunction.RegisterFunction(typeof(FunctionCharindexToLike2));
            }
        }

        [SQLiteFunction(Name = "CHARINDEX", Arguments = 2, FuncType = FunctionType.Scalar)]
        private class FunctionCharindexToLike : SQLiteFunction
        {
            /// <summary>
            /// Вызов скалярной функции CHARINDEX(sTextSearch, sFieldValue).
            /// </summary>
            /// <param name="args">Параметры функции</param>
            /// <returns>0 - не найдена подстрока, >=1 - найдена</returns>
            /*public override object Invoke(object[] args)
            {
                if (args.Length < 2 || args[0] == null || args[1] == null) return 0;

                int iRet = 0;
                try
                {
                    string sTextSearch = ((string)args[0]).ToLower();
                    string sFieldValue = ((string)args[1]).ToLower();

                    if (sFieldValue.IndexOf(sTextSearch) > -1) iRet = 1;
                    //else iRet = 0;
                }
                catch { }

                return iRet;
            }*/
            public override object Invoke(object[] args)
            {
                try
                {
                    if (args.Length < 2 || args[0] == null || args[1] == null) return 0;
                    //if (((string)args[1]).ToLower().IndexOf(((string)args[0]).ToLower()) > -1) return 1;
                    return ((string)args[1]).ToLower().IndexOf(((string)args[0]).ToLower()) + 1; // "+1" т.к. "-1" - это не найдено, а от 0 - найдено, а вернуть надо от 1 - найдено
                }
                catch { }
                return 0;
            }
        }

        [SQLiteFunction(Name = "CHARINDEX2", Arguments = 2, FuncType = FunctionType.Scalar)]
        private class FunctionCharindexToLike2 : SQLiteFunction
        {
            /// <summary>
            /// Вызов скалярной функции CHARINDEX(sTextSearch, sFieldValue).
            /// </summary>
            /// <param name="args">Параметры функции</param>
            /// <returns>0 - не найдена подстрока, >=1 - найдена</returns>
            /*public override object Invoke(object[] args)
            {
                if (args.Length < 2 || args[0] == null || args[1] == null) return 0;

                int iRet = 0;
                try
                {
                    string sTextSearch = ((string)args[0]).ToLower();
                    string sFieldValue = ((string)args[1]).ToLower();

                    if (sFieldValue.IndexOf(sTextSearch) > -1) iRet = 1;
                    //else iRet = 0;
                }
                catch { }

                return iRet;
            }*/
            public override object Invoke(object[] args)
            {
                try
                {
                    if (args.Length < 2 || args[0] == null || args[1] == null) return 0;
                    //if (((string)args[1]).ToLower().IndexOf(((string)args[0]).ToLower()) > -1) return 1;
                    return ((string)args[1]).ToLower().IndexOf(((string)args[0]).ToLower()) + 1; // "+1" т.к. "-1" - это не найдено, а от 0 - найдено, а вернуть надо от 1 - найдено
                }
                catch { }
                return 0;
            }
        }
    }
}