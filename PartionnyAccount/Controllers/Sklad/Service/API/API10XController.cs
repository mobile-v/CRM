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
using PartionnyAccount.Models.Sklad.Service.API;
using System.Data.SQLite;
using System.Web.Script.Serialization;
using System.Data.OleDb;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace PartionnyAccount.Controllers.Sklad.Service.API
{
    public class API10XController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();
        Classes.Account.Login.Field field = new Classes.Account.Login.Field();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 73;

        #endregion


        #region SELECT

        class Params
        {
            public string sKey = "";
            public string pID = "";

            //DirNomens
            public string sub = "";
            public string id = ""; //Товар
            //DirChars
            public string chartype = "";
            //DirContractors
            public string type2id = "";
            //DirContractors
            public string org = "";
            public string point = "";
        }

        // GET: api/APIs/5
        //[ResponseType(typeof(API))]
        public async Task<IHttpActionResult> GetAPI10X(HttpRequestMessage request)
        {
            try
            {
                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.sKey = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "Key", true) == 0).Value;
                _params.pID = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pID", true) == 0).Value;

                //DirNomens
                _params.sub = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sub", true) == 0).Value;
                _params.id = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "id", true) == 0).Value;

                //DirChars
                _params.chartype = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "chartype", true) == 0).Value;

                //DirContractors
                _params.type2id = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "type2id", true) == 0).Value;

                //ExportRemRemnants
                _params.org = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "org", true) == 0).Value;
                _params.point = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "point", true) == 0).Value;

                #endregion


                #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

                if (String.IsNullOrEmpty(_params.pID) || _params.pID.Length < 14) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg127); }

                //1. Проверка "Key"
                if (String.IsNullOrEmpty(_params.sKey) || _params.sKey.Length < 39) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg127_2); }

                Controllers.Sklad.Service.API.API10Controller api = new Controllers.Sklad.Service.API.API10Controller();
                if (!(await api.KeyGen_Verify(_params.sKey))) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg127_2); }
                int iDirCustomersID = api.iDirCustomersID;


                //Изменяем строку соединения
                db = new DbConnectionSklad(connectionString.Return(iDirCustomersID, null, true));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                //Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                //Получам API (что можно и нужно выгружать)
                Models.Sklad.Service.API.API10 aPI10 = await db.API10s.FindAsync(1);

                #endregion


                #region switch + отправка JSON

                switch (_params.pID)
                {
                    //Export


                    //Торговля

                    case "ExportDirNomens":

                        #region ExportDirNomens

                        if (Convert.ToBoolean(aPI10.ExportDirNomens))
                        {
                            #region query

                            var query =
                            (
                             from x in db.DirNomens
                             where
                                x.Del == false &&

                                //ImportToIM == true и все её подветки и подветки подветок не отображать!
                                x.ImportToIM == true &&

                                !(
                                    from o1 in db.DirNomens
                                    where o1.ImportToIM == false
                                    select o1.DirNomenID
                                ).Contains(x.Sub) &&

                                !(
                                    from o1 in db.DirNomens
                                    where 
                                        //o1.ImportToIM == true &&

                                        (
                                            from o2 in db.DirNomens
                                            where o2.ImportToIM == false
                                            select o2.DirNomenID
                                        ).Contains(o1.Sub)

                                    select o1.DirNomenID
                                ).Contains(x.Sub)


                             select new
                             {
                                 id = x.DirNomenID,
                                 sub = x.Sub,
                                 text = x.DirNomenName,
                                 //Del = x.Del,

                                 catid = x.DirNomenCategoryID,
                                 catname = x.dirNomenCategory.DirNomenCategoryName,

                                 nfull = aPI10.ExportDirNomen_DirNomenNameFull == true ? x.DirNomenNameFull : "",
                                 desc = aPI10.ExportDirNomen_Description == true ? x.Description : "",
                                 dfull = aPI10.ExportDirNomen_DescriptionFull == true ? x.DescriptionFull : "",

                                 img =
                                       aPI10.ExportDirNomen_ImageLink == true ?
                                       (x.SysGenID == null ? "" : @"/Users/user_" + field.DirCustomersID + "/" + x.SysGenID + ".jpg") :
                                       "",

                                 dupd = x.DateTimeUpdate

                                 //Содержит подчинённые - false, иначе - true (по логике наоборот)
                                 /*leaf =
                                     (
                                      from y in db.DirNomens
                                      where y.Sub == x.DirNomenID
                                      select y
                                     ).Count() == 0 ? true : false,*/
                             }
                            );

                            #endregion

                            #region Условия (параметры) *** *** ***

                            //1.Sub
                            if (!String.IsNullOrEmpty(_params.sub))
                            {
                                try
                                {
                                    //1.1.
                                    if (_params.sub.ToLower() == "null") { query = query.Where(x => x.sub == null); }
                                    else
                                    {
                                        //1.2.
                                        int? iSub = Convert.ToInt32(_params.sub);
                                        query = query.Where(x => x.sub == iSub);
                                    }
                                }
                                catch (Exception ex) { }
                            }

                            //2.id
                            if (!String.IsNullOrEmpty(_params.id))
                            {
                                try
                                {
                                    int? iID = Convert.ToInt32(_params.id);
                                    query = query.Where(x => x.id == iID);
                                }
                                catch (Exception ex) { }
                            }

                            //2.
                            query = query.OrderBy(x => x.sub);

                            #endregion

                            #region Отправка JSON

                            dynamic collectionWrapper = new
                            {
                                ExportDirNomens = query
                            };
                            return Ok(await Task.Run(() => collectionWrapper));

                            #endregion
                        }
                        else
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127_1));
                        }

                        #endregion

                        break;

                    case "ExportDirChars":

                        #region ExportDirChars

                        if (Convert.ToBoolean(aPI10.ExportDirChars))
                        {
                            if (_params.chartype == "DirCharColours")
                            {
                                #region query

                                var query =
                                    (
                                     from x in db.DirCharColours
                                     select new
                                     {
                                         id = x.DirCharColourID,
                                         text = x.DirCharColourName
                                     }
                                    );

                                #endregion

                                #region Отправка JSON

                                dynamic collectionWrapper = new
                                {
                                    DirCharColours = query
                                };
                                return Ok(await Task.Run(() => collectionWrapper));

                                #endregion
                            }
                            else if (_params.chartype == "DirCharMaterials")
                            {
                                #region query

                                var query =
                                    (
                                     from x in db.DirCharMaterials
                                     select new
                                     {
                                         id = x.DirCharMaterialID,
                                         text = x.DirCharMaterialName
                                     }
                                    );

                                #endregion

                                #region Отправка JSON

                                dynamic collectionWrapper = new
                                {
                                    DirCharMaterials = query
                                };
                                return Ok(await Task.Run(() => collectionWrapper));

                                #endregion
                            }
                            else if (_params.chartype == "DirCharNames")
                            {
                                #region query

                                var query =
                                    (
                                     from x in db.DirCharNames
                                     select new
                                     {
                                         id = x.DirCharNameID,
                                         text = x.DirCharNameName
                                     }
                                    );

                                #endregion

                                #region Отправка JSON

                                dynamic collectionWrapper = new
                                {
                                    DirCharNames = query
                                };
                                return Ok(await Task.Run(() => collectionWrapper));

                                #endregion
                            }
                            else if (_params.chartype == "DirCharSeasons")
                            {
                                #region query

                                var query =
                                    (
                                     from x in db.DirCharSeasons
                                     select new
                                     {
                                         id = x.DirCharSeasonID,
                                         text = x.DirCharSeasonName
                                     }
                                    );

                                #endregion

                                #region Отправка JSON

                                dynamic collectionWrapper = new
                                {
                                    DirCharSeasons = query
                                };
                                return Ok(await Task.Run(() => collectionWrapper));

                                #endregion
                            }
                            else if (_params.chartype == "DirCharSexes")
                            {
                                #region query

                                var query =
                                    (
                                     from x in db.DirCharSexes
                                     select new
                                     {
                                         id = x.DirCharSexID,
                                         text = x.DirCharSexName
                                     }
                                    );

                                #endregion

                                #region Отправка JSON

                                dynamic collectionWrapper = new
                                {
                                    DirCharSexes = query
                                };
                                return Ok(await Task.Run(() => collectionWrapper));

                                #endregion
                            }
                            else if (_params.chartype == "DirCharSizes")
                            {
                                #region query

                                var query =
                                    (
                                     from x in db.DirCharSizes
                                     select new
                                     {
                                         id = x.DirCharSizeID,
                                         text = x.DirCharSizeName
                                     }
                                    );

                                #endregion

                                #region Отправка JSON

                                dynamic collectionWrapper = new
                                {
                                    DirCharSizes = query
                                };
                                return Ok(await Task.Run(() => collectionWrapper));

                                #endregion
                            }
                            else if (_params.chartype == "DirCharStyles")
                            {
                                #region query

                                var query =
                                    (
                                     from x in db.DirCharStyles
                                     select new
                                     {
                                         id = x.DirCharStyleID,
                                         text = x.DirCharStyleName
                                     }
                                    );

                                #endregion

                                #region Отправка JSON

                                dynamic collectionWrapper = new
                                {
                                    DirCharStyles = query
                                };
                                return Ok(await Task.Run(() => collectionWrapper));

                                #endregion
                            }
                            else if (_params.chartype == "DirCharTextures")
                            {
                                #region query

                                var query =
                                    (
                                     from x in db.DirCharTextures
                                     select new
                                     {
                                         id = x.DirCharTextureID,
                                         text = x.DirCharTextureName
                                     }
                                    );

                                #endregion

                                #region Отправка JSON

                                dynamic collectionWrapper = new
                                {
                                    DirCharTextures = query
                                };
                                return Ok(await Task.Run(() => collectionWrapper));

                                #endregion
                            }
                            else
                            {
                                return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127_3));
                            }

                        }
                        else
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127_1));
                        }

                        #endregion

                        break;

                    case "ExportDirContractors":

                        #region ExportDirContractors

                        if (Convert.ToBoolean(aPI10.ExportDirContractors))
                        {
                            #region query

                            var query =
                                (
                                 from x in db.DirContractors
                                 where x.Del == false
                                 select new
                                 {
                                     id = x.DirContractorID,
                                     text = x.DirContractorName,
                                     //leaf = true,

                                     //Тип: Моя фирма, Поставщик, Покупатель, И поставщик и покупатель
                                     type2id = x.DirContractor2TypeID,
                                     type2name = x.dirContractor2Type.DirContractor2TypeName,

                                     /*
                                     adr = x.DirContractorAddress,
                                     phone = x.DirContractorPhone,
                                     email = x.DirContractorEmail,
                                     www = x.DirContractorWWW,
                                     */
                                 }
                                );

                            #endregion

                            #region Условия (параметры) *** *** ***

                            //1.org
                            if (!String.IsNullOrEmpty(_params.type2id))
                            {
                                try
                                {
                                    int iType2id = Convert.ToInt32(_params.type2id);
                                    if (iType2id > 0)
                                    {
                                        query = query.Where(x => x.type2id == iType2id);
                                    }
                                }
                                catch (Exception ex) { }
                            }

                            //2.id
                            if (!String.IsNullOrEmpty(_params.id))
                            {
                                try
                                {
                                    int? iID = Convert.ToInt32(_params.id);
                                    query = query.Where(x => x.id == iID);
                                }
                                catch (Exception ex) { }
                            }

                            //2.
                            //query = query.OrderBy(x => x.sub);

                            #endregion

                            #region Отправка JSON

                            dynamic collectionWrapper = new
                            {
                                ExportDirContractors = query
                            };
                            return Ok(await Task.Run(() => collectionWrapper));

                            #endregion
                        }
                        else
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127_1));
                        }

                        #endregion

                        break;

                    case "ExportRemRemnants":

                        #region ExportRemRemnants

                        if (Convert.ToBoolean(aPI10.ExportRemRemnants))
                        {
                            #region query

                            var query =
                                (
                                 from x in db.RemRemnants
                                 where x.Quantity > 0
                                 select new
                                 {
                                     org = x.DirContractorIDOrg,
                                     id = x.DirNomenID,
                                     point = x.DirWarehouseID,
                                     Quantity = x.Quantity
                                 }
                                );

                            #endregion

                            #region Условия (параметры) *** *** ***

                            //1.org
                            if (!String.IsNullOrEmpty(_params.org))
                            {
                                try
                                {
                                    int iOrg = Convert.ToInt32(_params.org);
                                    if (iOrg > 0)
                                    {
                                        query = query.Where(x => x.org == iOrg);
                                    }
                                }
                                catch (Exception ex) { }
                            }

                            //2.id
                            if (!String.IsNullOrEmpty(_params.id))
                            {
                                try
                                {
                                    int? iID = Convert.ToInt32(_params.id);
                                    query = query.Where(x => x.id == iID);
                                }
                                catch (Exception ex) { }
                            }

                            //3.point
                            if (!String.IsNullOrEmpty(_params.point))
                            {
                                try
                                {
                                    int? iPoint = Convert.ToInt32(_params.point);
                                    query = query.Where(x => x.point == iPoint);
                                }
                                catch (Exception ex) { }
                            }

                            //4.
                            //query = query.OrderBy(x => x.sub);

                            #endregion

                            #region Отправка JSON

                            dynamic collectionWrapper = new
                            {
                                ExportRemRemnants = query
                            };
                            return Ok(await Task.Run(() => collectionWrapper));

                            #endregion
                        }
                        else
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127_1));
                        }

                        #endregion

                        break;

                    case "ExportRemParties":

                        #region ExportRemParties

                        if (Convert.ToBoolean(aPI10.ExportRemParties))
                        {
                            #region query

                            var query =
                                (
                                 from x in db.RemParties
                                 where x.Remnant > 0
                                 select new
                                 {
                                     org = x.DirContractorIDOrg,
                                     id = x.DirNomenID,
                                     point = x.DirWarehouseID,
                                     contr = x.DirContractorID, //Поставщик
                                     rem = x.Remnant, //Остаток


                                     //Характеристики

                                     ColourID = x.DirCharColourID,
                                     //Colourname = x.dirCharColour.DirCharColourName,

                                     MaterialID = x.DirCharMaterialID,
                                     //MaterialName = x.dirCharMaterial.DirCharMaterialName,

                                     NameID = x.DirCharNameID,
                                     //NameName = x.dirCharName.DirCharNameName,

                                     SeasonID = x.DirCharSeasonID,
                                     //SeasonName = x.dirCharSeason.DirCharSeasonName,

                                     SexID = x.DirCharSexID,
                                     //SexName = x.dirCharSex.DirCharSexName,

                                     SizeID = x.DirCharSizeID,
                                     //SizeName = x.dirCharSize.DirCharSizeName,

                                     StyleID = x.DirCharStyleID,
                                     //StyleName = x.dirCharStyle.DirCharStyleName,

                                     TextureID = x.DirCharTextureID,
                                     //TextureName = x.dirCharTexture.DirCharTextureName,


                                     //Цены

                                     CurrID = x.DirCurrencyID,
                                     //CurrName = x.dirCurrency.DirCurrencyName,

                                     pRetailV = x.PriceRetailVAT,
                                     pRetailC = x.PriceRetailCurrency,

                                     pWholesaleV = x.PriceWholesaleVAT,
                                     pWholesaleC = x.PriceWholesaleCurrency,

                                     pIMV = x.PriceIMVAT,
                                     pIMC = x.PriceIMCurrency,



                                     MinBal = x.DirNomenMinimumBalance,

                                 }
                                );

                            #endregion

                            #region Условия (параметры) *** *** ***

                            //1.org
                            if (!String.IsNullOrEmpty(_params.org))
                            {
                                try
                                {
                                    int iOrg = Convert.ToInt32(_params.org);
                                    if (iOrg > 0)
                                    {
                                        query = query.Where(x => x.org == iOrg);
                                    }
                                }
                                catch (Exception ex) { }
                            }

                            //2.id
                            if (!String.IsNullOrEmpty(_params.id))
                            {
                                try
                                {
                                    int? iID = Convert.ToInt32(_params.id);
                                    query = query.Where(x => x.id == iID);
                                }
                                catch (Exception ex) { }
                            }

                            //3.point
                            if (!String.IsNullOrEmpty(_params.point))
                            {
                                try
                                {
                                    int? iPoint = Convert.ToInt32(_params.point);
                                    query = query.Where(x => x.point == iPoint);
                                }
                                catch (Exception ex) { }
                            }

                            //4.
                            //query = query.OrderBy(x => x.sub);

                            #endregion

                            #region Отправка JSON

                            dynamic collectionWrapper = new
                            {
                                ExportRemParties = query
                            };
                            return Ok(await Task.Run(() => collectionWrapper));

                            #endregion
                        }
                        else
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127_1));
                        }

                        #endregion

                        break;




                    //СЦ


                    case "ExportDirServiceNomens":

                        #region ExportDirServiceNomens

                        if (Convert.ToBoolean(aPI10.ExportDirServiceNomens))
                        {
                            #region query

                            var query =
                            (
                             from x in db.DirServiceNomens
                             where
                                x.Del == false &&

                                //ImportToIM == true и все её подветки и подветки подветок не отображать!
                                x.ImportToIM == true &&

                                !(
                                    from o1 in db.DirServiceNomens
                                    where o1.ImportToIM == false
                                    select o1.DirServiceNomenID
                                ).Contains(x.Sub) &&

                                !(
                                    from o1 in db.DirServiceNomens
                                    where
                                        //o1.ImportToIM == true &&

                                        (
                                            from o2 in db.DirServiceNomens
                                            where o2.ImportToIM == false
                                            select o2.DirServiceNomenID
                                        ).Contains(o1.Sub)

                                    select o1.DirServiceNomenID
                                ).Contains(x.Sub)


                             select new
                             {
                                 id = x.DirServiceNomenID,
                                 sub = x.Sub,
                                 text = x.DirServiceNomenName,
                                 //Del = x.Del,

                                 //catid = x.DirServiceNomenCategoryID,
                                 //catname = x.dirNomenCategory.DirServiceNomenCategoryName,

                                 //nfull = aPI10.ExportDirServiceNomen_DirServiceNomenNameFull == true ? x.DirServiceNomenNameFull : "",
                                 //desc = aPI10.ExportDirServiceNomen_Description == true ? x.Description : "",
                                 //dfull = aPI10.ExportDirServiceNomen_DescriptionFull == true ? x.DescriptionFull : "",

                                 /*
                                 img =
                                       aPI10.ExportDirServiceNomen_ImageLink == true ?
                                       (x.SysGenID == null ? "" : @"/Users/user_" + field.DirCustomersID + "/" + x.SysGenID + ".jpg") :
                                       "",
                                 */

                                 //Содержит подчинённые - false, иначе - true (по логике наоборот)
                                 /*leaf =
                                     (
                                      from y in db.DirServiceNomens
                                      where y.Sub == x.DirServiceNomenID
                                      select y
                                     ).Count() == 0 ? true : false,*/

                                 dupd = x.DateTimeUpdate,

                                 f1c = x.Faults1Check,
                                 f1p = x.Faults1Price,

                                 f2c = x.Faults2Check,
                                 f2p = x.Faults2Price,

                                 f3c = x.Faults3Check,
                                 f3p = x.Faults3Price,

                                 f4c = x.Faults4Check,
                                 f4p = x.Faults4Price,

                                 f5c = x.Faults5Check,
                                 f5p = x.Faults5Price,

                                 f6c = x.Faults6Check,
                                 f6p = x.Faults6Price,

                                 f7c = x.Faults7Check,
                                 f7p = x.Faults7Price,

                                 f8c = x.Faults8Check,
                                 f8p = x.Faults8Price,

                                 f9c = x.Faults9Check,
                                 f9p = x.Faults9Price,

                                 f10c = x.Faults10Check,
                                 f10p = x.Faults10Price,

                                 f11c = x.Faults11Check,
                                 f11p = x.Faults11Price,

                                 f12c = x.Faults12Check,
                                 f12p = x.Faults12Price,

                                 f13c = x.Faults13Check,
                                 f13p = x.Faults13Price,

                                 f14c = x.Faults14Check,
                                 f14p = x.Faults14Price,

                             }
                            );

                            #endregion

                            #region Условия (параметры) *** *** ***

                            //1.Sub
                            if (!String.IsNullOrEmpty(_params.sub))
                            {
                                try
                                {
                                    //1.1.
                                    if (_params.sub.ToLower() == "null") { query = query.Where(x => x.sub == null); }
                                    else
                                    {
                                        //1.2.
                                        int? iSub = Convert.ToInt32(_params.sub);
                                        query = query.Where(x => x.sub == iSub);
                                    }
                                }
                                catch (Exception ex) { }
                            }

                            //2.id
                            if (!String.IsNullOrEmpty(_params.id))
                            {
                                try
                                {
                                    int? iID = Convert.ToInt32(_params.id);
                                    query = query.Where(x => x.id == iID);
                                }
                                catch (Exception ex) { }
                            }

                            //2.
                            query = query.OrderBy(x => x.sub);

                            #endregion

                            #region Отправка JSON

                            dynamic collectionWrapper = new
                            {
                                ExportDirServiceNomens = query
                            };
                            return Ok(await Task.Run(() => collectionWrapper));

                            #endregion
                        }
                        else
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127_1));
                        }

                        #endregion

                        break;



                        //БУ

                    case "ExportRem2Parties":

                        #region ExportRem2Parties

                        if (Convert.ToBoolean(aPI10.ExportRem2Parties))
                        {
                            #region query

                            var query =
                                (
                                 from x in db.Rem2Parties
                                 where x.Remnant > 0
                                 select new
                                 {
                                     org = x.DirContractorIDOrg,
                                     id = x.DirServiceNomenID,
                                     point = x.DirWarehouseID,
                                     //contr = x.DirServiceContractorID, //Поставщик
                                     rem = x.Remnant, //Остаток
                                     

                                     //Цены

                                     CurrID = x.DirCurrencyID,
                                     //CurrName = x.dirCurrency.DirCurrencyName,

                                     pRetailV = x.PriceRetailVAT,
                                     pRetailC = x.PriceRetailCurrency,

                                     pWholesaleV = x.PriceWholesaleVAT,
                                     pWholesaleC = x.PriceWholesaleCurrency,

                                     pIMV = x.PriceIMVAT,
                                     pIMC = x.PriceIMCurrency,



                                     MinBal = x.DirNomenMinimumBalance,

                                 }
                                );

                            #endregion

                            #region Условия (параметры) *** *** ***

                            //1.org
                            if (!String.IsNullOrEmpty(_params.org))
                            {
                                try
                                {
                                    int iOrg = Convert.ToInt32(_params.org);
                                    if (iOrg > 0)
                                    {
                                        query = query.Where(x => x.org == iOrg);
                                    }
                                }
                                catch (Exception ex) { }
                            }

                            //2.id
                            if (!String.IsNullOrEmpty(_params.id))
                            {
                                try
                                {
                                    int? iID = Convert.ToInt32(_params.id);
                                    query = query.Where(x => x.id == iID);
                                }
                                catch (Exception ex) { }
                            }

                            //3.point
                            if (!String.IsNullOrEmpty(_params.point))
                            {
                                try
                                {
                                    int? iPoint = Convert.ToInt32(_params.point);
                                    query = query.Where(x => x.point == iPoint);
                                }
                                catch (Exception ex) { }
                            }

                            //4.
                            //query = query.OrderBy(x => x.sub);

                            #endregion

                            #region Отправка JSON

                            dynamic collectionWrapper = new
                            {
                                ExportRem2Parties = query
                            };
                            return Ok(await Task.Run(() => collectionWrapper));

                            #endregion
                        }
                        else
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127_1));
                        }

                        #endregion

                        break;





                    //Import

                    case "ImportDirNomens":
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127));
                        }
                        break;

                    case "ImportDirContractors":
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127));
                        }
                        break;

                    case "ImportDocOrderInts":
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127));
                        }
                        break;


                    //Default

                    default:
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127));
                        }
                        break;
                }

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // POST: api/API10s
        [ResponseType(typeof(API10))]
        public async Task<IHttpActionResult> PostAPI10X(DocOrderInt docOrderInt)
        {
            try
            {
                #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

                if (String.IsNullOrEmpty(docOrderInt.pID) || docOrderInt.pID.Length < 14) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg127); }

                //1. Проверка "Key"
                if (String.IsNullOrEmpty(docOrderInt.Key) || docOrderInt.Key.Length < 39) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg127_2); }

                Controllers.Sklad.Service.API.API10Controller api = new Controllers.Sklad.Service.API.API10Controller();
                if (!(await api.KeyGen_Verify(docOrderInt.Key))) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg127_2); }
                int iDirCustomersID = api.iDirCustomersID;


                //Изменяем строку соединения
                db = new DbConnectionSklad(connectionString.Return(iDirCustomersID, null, true));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                //Получам API (что можно и нужно выгружать)
                Models.Sklad.Service.API.API10 aPI10 = await db.API10s.FindAsync(1);

                #endregion


                #region Параметры
                
                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocOrderInt[] docOrderIntCollection = null;
                if (!String.IsNullOrEmpty(docOrderInt.recordsDocOrderInt))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docOrderIntCollection = serializer.Deserialize<Models.Sklad.Doc.DocOrderInt[]>(docOrderInt.recordsDocOrderInt);
                }

                //Проставляем все необходимые значения
                for (int i = 0; i < docOrderIntCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocOrderInt _docOrderInt = (Models.Sklad.Doc.DocOrderInt)docOrderIntCollection[i];
                    _docOrderInt.DirOrderIntTypeID = 4; //Из И-М
                    _docOrderInt.DirOrderIntStatusID = 1; //Новый


                    //Находим полное "DirNomenName" по "DirNomenID"

                    Models.Sklad.Dir.DirNomen dirNomen = await db.DirNomens.FindAsync(_docOrderInt.DirNomenID);

                    Controllers.Sklad.Dir.DirNomens.DirNomensController dirNomensController = new Dir.DirNomens.DirNomensController();
                    string[] ret = await Task.Run(() => dirNomensController.DirNomenSubFind2(db, _docOrderInt.DirNomenID));
                    _docOrderInt.DirNomenName = ret[0] + " / "+ dirNomen.DirNomenName;


                    //Группы: DirNomen1ID, DirNomen2ID

                    string[] separators = { "," };
                    string[] words = ret[2].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        _docOrderInt.DirNomen1ID = Convert.ToInt32(words[0]);
                        _docOrderInt.DirNomen2ID = Convert.ToInt32(words[1]);
                    }
                    catch (Exception ex) { }


                    //Категория: DirNomenCategoryID
                    _docOrderInt.DirNomenCategoryID = Convert.ToInt32(dirNomen.DirNomenCategoryID);


                    //Цены
                    _docOrderInt.PriceVAT = _docOrderInt.PriceCurrency;
                    _docOrderInt.DirCurrencyID = sysSetting.DirCurrencyID;
                }

                #endregion


                #region switch + отправка JSON

                switch (docOrderInt.pID)
                {

                    //Import

                    case "ImportDirNomens":
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127));
                        }
                        break;

                    case "ImportDirContractors":
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127));
                        }
                        break;

                    case "ImportDocOrderInts":
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127));
                        }
                        break;


                    //Default

                    default:
                        {
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg127));
                        }
                        break;
                }

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion

    }
}
