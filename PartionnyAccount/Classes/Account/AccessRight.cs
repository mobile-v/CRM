using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PartionnyAccount.Models;
using System.Data.SQLite;

namespace PartionnyAccount.Classes.Account
{
    internal class AccessRight
    {

        internal int Access(string ConStr, int DirEmployeeID, string RightField)
        {
            int ret = 3;

            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT " + RightField + " AS RightField FROM DirEmployees WHERE DirEmployeeID=@DirEmployeeID", con))
                {
                    SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID }; cmd.Parameters.Add(parDirEmployeeID);
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (String.IsNullOrEmpty((dr["RightField"].ToString()))) ret = 3;
                            else ret = Convert.ToInt32(dr["RightField"].ToString());
                        }
                    }
                }
            }

            return ret;
        }

        internal bool AccessCheck(string ConStr, int DirEmployeeID, string RightField)
        {
            bool ret = false;

            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT " + RightField + " AS RightField FROM DirEmployees WHERE DirEmployeeID=@DirEmployeeID", con))
                {
                    SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID }; cmd.Parameters.Add(parDirEmployeeID);
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (String.IsNullOrEmpty(dr["RightField"].ToString()))
                            {
                                ret = false;
                            }
                            else
                            {
                                ret = Convert.ToBoolean(dr["RightField"].ToString());
                            }
                        }
                    }
                }
            }

            return ret;
        }

        internal bool AccessIsAdmin(string ConStr, int DirEmployeeID, int? DirWarehouseID)
        {
            if (DirEmployeeID == 1 || DirWarehouseID == null || DirWarehouseID == 0) return true;


            bool ret = false;

            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT IsAdmin AS IsAdmin FROM DirEmployeeWarehouses WHERE DirEmployeeID=@DirEmployeeID and DirWarehouseID=@DirWarehouseID", con))
                {
                    SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID }; cmd.Parameters.Add(parDirEmployeeID);
                    SQLiteParameter parDirWarehouseID = new SQLiteParameter("@DirWarehouseID", System.Data.DbType.Int32) { Value = DirWarehouseID }; cmd.Parameters.Add(parDirWarehouseID);
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            ret = Convert.ToBoolean(dr["IsAdmin"].ToString());
                        }
                    }
                }
            }

            return ret;
        }

        internal int Access111(DbConnectionSklad db, int DirEmployeesID, string ListObjectNameSys)
        {
            /*
            var query =
                (
                    from sysDirRightAccess in db.SysDirRightAccess
                    from dirEmployeess in db.DirEmployees
                    from listObjects in db.ListObjects

                    where
                        dirEmployeess.DirEmployeesID == DirEmployeesID &&
                        sysDirRightAccess.SysDirRightsID == dirEmployeess.SysDirRightsID &&
                        sysDirRightAccess.ListObjectID == listObjects.ListObjectID &&
                        listObjects.ListObjectNameSys == ListObjectNameSys

                    select new
                    {
                        Status = sysDirRightAccess.Status
                    }
                ).ToList();

            if (query.Count() > 0) return query[0].Status;
            else return 1;
            */

            return 1;
        }

    }
}