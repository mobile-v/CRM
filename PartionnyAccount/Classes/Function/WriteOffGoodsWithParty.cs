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
using PartionnyAccount.Models;
using PartionnyAccount.Models.Sklad.Doc;
using System.Data.SQLite;
using System.Web.Script.Serialization;

namespace PartionnyAccount.Classes.Function
{

    //Проверка: Было ли списание с партий

    public class WriteOffGoodsWithParty
    {
        internal async Task<bool> Exist(
            DbConnectionSklad db,
            int? iDocPurch_DocID
            )
        {
            //Есть ли списанный товар с данного прихода
            var queryRemParties = await Task.Run(() =>
                (
                    from x in db.RemParties
                    where x.DocID == iDocPurch_DocID && x.Quantity != x.Remnant
                    select x
                ).ToListAsync());

            //Есть!
            if (queryRemParties.Count() > 0)
            {
                //Смотрим, какие именно накладные списали товар.
                var queryRemPartyMinuses = await Task.Run(() =>
                    (
                        from remPartyMinuses in db.RemPartyMinuses

                        join remParties1 in db.RemParties on remPartyMinuses.RemPartyID equals remParties1.RemPartyID into remParties2
                        from remParties in remParties2.Where(x => x.DocID == iDocPurch_DocID) //.DefaultIfEmpty()

                        select new
                        {
                            DocID = remPartyMinuses.doc.DocID,
                            NumberReal = remPartyMinuses.doc.NumberReal,
                            ListObjectNameRu = remPartyMinuses.doc.listObject.ListObjectNameRu
                        }
                    ).Distinct().ToListAsync()); // - убрать повторяющиеся

                //Есть списания!
                if (queryRemPartyMinuses.Count() > 0)
                {
                    //Поиск всех DocID
                    string arrDocID = "";
                    for (int i = 0; i < queryRemPartyMinuses.Count(); i++)
                    {
                        arrDocID += "№ " +  queryRemPartyMinuses[i].NumberReal + " - " + queryRemPartyMinuses[i].ListObjectNameRu + " (общий № "+ queryRemPartyMinuses[i].DocID + ")";
                        if (i != queryRemPartyMinuses.Count() - 1) arrDocID += "<br />";
                    }
                    //Сообщение клиенту
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg102 + arrDocID + Classes.Language.Sklad.Language.msg102_1);
                }
            }

            return false;
        }

    }

    public class WriteOffGoodsWithParty2
    {
        internal async Task<bool> Exist(
            DbConnectionSklad db,
            int? iDocPurch_DocID
            )
        {
            //Есть ли списанный товар с данного прихода
            var queryRem2Parties = await Task.Run(() =>
                (
                    from x in db.Rem2Parties
                    where x.DocID == iDocPurch_DocID && x.Quantity != x.Remnant
                    select x
                ).ToListAsync());

            //Есть!
            if (queryRem2Parties.Count() > 0)
            {
                //Смотрим, какие именно накладные списали товар.
                var queryRem2PartyMinuses = await Task.Run(() =>
                    (
                        from rem2PartyMinuses in db.Rem2PartyMinuses

                        join rem2Parties1 in db.Rem2Parties on rem2PartyMinuses.Rem2PartyID equals rem2Parties1.Rem2PartyID into rem2Parties2
                        from rem2Parties in rem2Parties2.Where(x => x.DocID == iDocPurch_DocID) //.DefaultIfEmpty()

                        select new
                        {
                            DocID = rem2PartyMinuses.doc.DocID,
                            NumberReal = rem2PartyMinuses.doc.NumberReal,
                            ListObjectNameRu = rem2PartyMinuses.doc.listObject.ListObjectNameRu
                        }
                    ).Distinct().ToListAsync()); // - убрать повторяющиеся

                //Есть списания!
                if (queryRem2PartyMinuses.Count() > 0)
                {
                    //Поиск всех DocID
                    string arrDocID = "";
                    for (int i = 0; i < queryRem2PartyMinuses.Count(); i++)
                    {
                        arrDocID += "№ " + queryRem2PartyMinuses[i].NumberReal + " - " + queryRem2PartyMinuses[i].ListObjectNameRu + " (общий № " + queryRem2PartyMinuses[i].DocID + ")";
                        if (i != queryRem2PartyMinuses.Count() - 1) arrDocID += "<br />";
                    }
                    //Сообщение клиенту
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg102 + arrDocID + Classes.Language.Sklad.Language.msg102_1);
                }
            }

            return false;
        }

    }
}