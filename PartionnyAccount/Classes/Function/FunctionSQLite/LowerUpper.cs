using System;
using System.Data.SQLite;

namespace PartionnyAccount.Classes.Function.FunctionSQLite
{
    public class LowerUpper
    {
        public static class InitFunction
        {
            /// <summary>
            /// Initializes the collation UTF8CI 
            /// </summary>
            public static void initLower() { SQLiteFunction.RegisterFunction(typeof(FunctionLower)); }
            public static void initUpper() { SQLiteFunction.RegisterFunction(typeof(FunctionUpper)); }

            //public static void initDate() { SQLiteFunction.RegisterFunction(typeof(FunctionLower)); }
        }

        /// <summary>
        /// Класс переопределяет функцию Lower() в SQLite, т.к. встроенная функция некорректно работает с символами > 128
        /// </summary>
        [SQLiteFunction(Name = "lower", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class FunctionLower : SQLiteFunction
        {

            /// <summary>
            /// Вызов скалярной функции Lower().
            /// </summary>
            /// <param name="args">Параметры функции</param>
            /// <returns>Строка в нижнем регистре</returns>
            public override object Invoke(object[] args)
            {
                if (args.Length == 0 || args[0] == null) return null;

                string s = "";
                try { s = ((string)args[0]).ToLower(); }
                catch { }
                return s; // ((string)args[0]).ToLower();
            }
        }

        /// <summary>
        /// Класс переопределяет функцию Upper() в SQLite, т.к. встроенная функция некорректно работает с символами > 128
        /// </summary>
        [SQLiteFunction(Name = "upper", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class FunctionUpper : SQLiteFunction
        {

            /// <summary>
            /// Вызов скалярной функции Upper().
            /// </summary>
            /// <param name="args">Параметры функции</param>
            /// <returns>Строка в верхнем регистре</returns>
            public override object Invoke(object[] args)
            {
                if (args.Length == 0 || args[0] == null) return null;
                return ((string)args[0]).ToUpper();
            }
        }


        /*
        /// <summary>
        /// Класс переопределяет функцию Date() в SQLite, т.к. встроенная функция некорректно работает с символами > 128
        /// </summary>
        [SQLiteFunction(Name = "date", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class FunctionDate : SQLiteFunction
        {

            /// <summary>
            /// Вызов скалярной функции Date().
            /// </summary>
            /// <param name="args">Параметры функции</param>
            /// <returns>Строка в нижнем регистре</returns>
            public override object Invoke(object[] args)
            {
                if (args.Length == 0 || args[0] == null) return null;

                string s = "";
                try { s = ((DateTime)args[0]).ToString("yyyy-MM-dd"); }
                catch { }
                return s; // ((string)args[0]).ToDate();
            }
        }
        */
    }
}