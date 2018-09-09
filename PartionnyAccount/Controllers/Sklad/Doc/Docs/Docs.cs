using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.IO;
using System.Collections;
using System.Data.OleDb;
using PartionnyAccount.Models;
using PartionnyAccount.Models.Sklad.Doc;

namespace PartionnyAccount.Controllers.Sklad.Doc.Docs
{
    public class Docs
    {
        DbConnectionSklad db;
        DbConnectionSklad dbRead;
        Models.Sklad.Doc.Doc doc;
        EntityState entityState; //EntityState.Added, Modified

        //Для Инвентаризации
        internal int DirWarehouseID = 0;
        internal int DirContractorIDOrg = 0; //Пока не используется

        //Метод для передачи данных в переменные
        internal Docs(
            DbConnectionSklad _db,
            DbConnectionSklad _dbRead,
            Models.Sklad.Doc.Doc _doc,
            EntityState _entityState //EntityState.Added, Modified
            )
        {
            db = _db;

            if (_dbRead != null) dbRead = _dbRead;
            else dbRead = _db;

            doc = _doc;
            entityState = _entityState;
        }


        //Проверка "на Удаление"
        private async Task<bool> Deleted()
        {
            //Только, если это НЕ новый документ
            if (doc.DocID > 0)
            {
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.Docs
                        where x.DocID == doc.DocID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].Del) return true;
            }

            return false;
        }

        //Проверка "на Инвентаризирован"
        private async Task<bool> Inv()
        {
            //Если дата последне инвентаризации >= дати документа, то выдать Эксепшен
            //И документ не текущая инвентаризация
            //37 - Акт выполненных работ
            //39 - Инвентаризация: Проверка на один и тот же Склад и Организацию, т.к. один склад - один акт. А вот Акты для разных Складов и Организация, могут быть разные.

            if (doc.DocID > 0 && doc.ListObjectID != 37)
            {
                if (doc.ListObjectID != 39)
                {
                    var query = await Task.Run(() =>
                        (
                            from x in dbRead.Docs
                            where x.ListObjectID == 39 && x.Held == true
                            select x
                        ).ToListAsync());


                    if (query.Count() > 0)
                    {
                        //Может быть несколько Инвенитаризация. 
                        //Проверяем все (весь список) и смотрим есть ли в нём текущая.
                        //Если есть то можно менять документ!

                        bool bActivInv = false;
                        for (int i = 0; i < query.Count(); i++)
                        {
                            if (query[i].DocID == doc.DocID) bActivInv = true;
                        }

                        if (!bActivInv && query[0].DocDate >= doc.DocDate && query[0].DocID != doc.DocID) throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg85_1);
                    }
                }
                else
                {
                    var query = await Task.Run(() =>
                        (
                            from x in dbRead.DocInventories
                            where x.doc.ListObjectID == 39
                            select x
                        ).ToListAsync());

                    if (query.Count() > 0)
                    {
                        //Проверка на один и тот же Склад и Организацию, т.к. один склад - один акт. 
                        //А вот Акты для разных Складов и Организация, могут быть разные.

                        bool bActivInv = false;
                        for (int i = 0; i < query.Count(); i++)
                        {
                            if (query[i].DirWarehouseID == DirWarehouseID && query[i].doc.DirContractorIDOrg == doc.DirContractorIDOrg) { bActivInv = true; break; }
                        }

                        if (bActivInv && query[0].DocDate >= doc.DocDate && query[0].DocID != doc.DocID) throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg85_2);
                    }
                }
            }

            return true;
        }

        internal async Task<Models.Sklad.Doc.Doc> Save()
        {
            //Проверка Удалён ли документ, если удалён, то не сохранять
            if (await Deleted()) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg85); }

            //Проверка Инвентаризирован ли документ, если Инвентаризирован, то не сохранять
            //if (Convert.ToBoolean(doc.Held) && await Inv()) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg85_1); }
            //if (Convert.ToBoolean(doc.Held)) { await Inv(); }
            await Inv();


            //Дата проведения
            if (!Convert.ToBoolean(doc.Held)) { doc.DocDateHeld = doc.DocDate; }
            else { doc.DocDateHeld = DateTime.Now; }

            if (entityState == EntityState.Added) { doc.DocDateCreate = DateTime.Now; }

            //Дата оплаты (проверить, если нет оплаты, то ставить дату Документа)
            if (doc.Payment == 0) { doc.DocDatePayment = doc.DocDate; }
            else
            {
                if (doc.DocDatePayment == null)
                {
                    Models.Sklad.Doc.Doc docX = await dbRead.Docs.FindAsync(doc.DocID);
                    doc.DocDatePayment = docX.DocDatePayment;
                }
            }



            //Сохраянем
            if (doc.DirPaymentTypeID == null) doc.DirPaymentTypeID = 1;
            db.Entry(doc).State = entityState;
            await db.SaveChangesAsync();

            return doc;
        }
    }
}