using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PartionnyAccount.Models;

namespace PartionnyAccount.Classes.Account
{
    //Only Comercial
    public class Payment
    {
        internal class CustomerPay
        {
            public DateTime DirCustomersDate;  //Дата создания пользователя
            public bool Pay;                   //Платный не платный
            public int DirPayServiceID;        //(-2, -1, 0, 1, 2, 3) Тарифный план (-2 - бесплатный, -1 - тестовый, 0 - Начинающий, 1,2,3 - ...)
            public string DirPayServiceName;   //Тарифный план - Название
            public int CountUser;              //"К-во пользователей"
            //public int CountTT;                //"К-во ТТ"
            public int CountNomen;             //"К-во Номенклатуры": "-1" - не ограниченно
            public int CountContr;               //"К-во Документов":   "-1" - не ограниченно
            //public int CountIM;                //"К-во Интернет-Магазинов":   "-1" - не ограниченно

            //Оплата
            //public int PayDateBegin;           //Дата начала оплаты
            //public int PayMonths;              //На к-во месяцев
            //public int PayDayBonus;            //Бонус
            public DateTime PayDateEnd = Convert.ToDateTime("2150-01-01");             //Дата конца оплаты
        }

        internal CustomerPay Return(int DirCustomersID)
        {
            CustomerPay customerPay = new CustomerPay();
            customerPay.PayDateEnd = DateTime.Now.AddMonths(1);

            using (Models.DbConnectionLogin db = new Models.DbConnectionLogin("ConnStrMSSQL"))
            {
                #region 1. Данные с таблицы DirCustomers (Дата и Платный) и  формирование данных

                //Выборка с таблицы "DirCustomers"
                var query1 = db.DirCustomers.Where(x => x.DirCustomersID == DirCustomersID).ToList();
                if (query1.Count() > 0)
                {
                    customerPay.DirCustomersDate = Convert.ToDateTime(query1[0].DirCustomersDate);
                    customerPay.Pay = Convert.ToBoolean(query1[0].Pay);
                }

                /*
                //Формирование данных
                if (customerPay.DirCustomersDate.AddMonths(1) >= DateTime.Now.Date) //customerPay.Pay && 
                {
                    customerPay.DirPayServiceID = -1;             //-1;
                    customerPay.DirPayServiceName = "BUSINESS";    //"Пробный";
                    customerPay.CountUser = 1;                    //К-во пользователей
                    //customerPay.CountTT = 1;                      //К-во ТТ
                    //customerPay.CountIM = 1;                      //К-во ИМ
                    customerPay.CountNomen = -1;                  //К-во Номенклатуры
                    customerPay.CountContr = -1;                    //К-во Документов
                }
                else
                {
                    customerPay.DirPayServiceID = 0;              //0;
                    customerPay.DirPayServiceName = "FREE"; //"Начинающий";
                    customerPay.CountUser = 1;                    //К-во пользователей
                    //customerPay.CountTT = 0;                      //К-во ТТ
                    //customerPay.CountIM = 0;                      //К-во ИМ
                    customerPay.CountNomen = 1000;                //К-во Номенклатуры
                    customerPay.CountContr = 100;                   //К-во Документов
                }
                */

                customerPay.DirPayServiceID = -1;             //-1;
                customerPay.DirPayServiceName = "BUSINESS";    //"Пробный";
                customerPay.CountUser = 10;                    //К-во пользователей
                                                              //customerPay.CountTT = 1;                      //К-во ТТ
                //customerPay.CountIM = 1;                      //К-во ИМ
                customerPay.CountNomen = -1;                  //К-во Номенклатуры
                customerPay.CountContr = -1;

                #endregion


                #region 2.Проплачен ли "Основная Услуга"

                /*
                var query2 =
                    (
                        from dirCustomers in db.DirCustomers
                        from dirPayCustomers in db.DirPayCustomers
                        from dirPayServices in db.DirPayServices
                        where
                            dirCustomers.DirCustomersID == DirCustomersID &&
                            dirCustomers.DirCustomersID == dirPayCustomers.DirCustomersID &&
                            dirPayCustomers.DirPayServiceID == dirPayServices.DirPayServiceID &&
                            DateTime.Now >= dirPayCustomers.PayDateBegin &&
                            DateTime.Now <= dirPayCustomers.PayDateEnd &&
                            dirPayCustomers.DirPayServiceID <= 3 &&
                            dirPayCustomers.Held == true
                        select new
                        {
                            DirPayServiceID = dirPayCustomers.DirPayServiceID,
                            DirPayServiceName = dirPayServices.DirPayServiceName,
                            PayDateEnd = dirPayCustomers.PayDateEnd
                        }
                    ).ToList();

                if (query2.Count() > 0)
                {
                    customerPay.DirPayServiceID = query2[0].DirPayServiceID;
                    customerPay.DirPayServiceName = query2[0].DirPayServiceName;
                    customerPay.PayDateEnd = query2[0].PayDateEnd;
                    //Если проплачено, то как минимум у Клиента 3-и сотрудника
                    customerPay.CountUser = 10;
                    //Если проплачено, то INFINITY Номенклатуры и Документов
                    //customerPay.CountTT = 1;
                    //customerPay.CountIM = 1;
                    customerPay.CountNomen = -1;
                    customerPay.CountContr = -1;
                }
                */

                #endregion
            }

            return customerPay;
        }

    }
}