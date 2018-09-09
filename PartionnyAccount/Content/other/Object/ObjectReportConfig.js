function ObjectReportConfig(pObjectName, Params) {

    //Если окно открыто, то устанавливаем на него фокус
    if (funSearchWin(pObjectName, true)) return;

    //Параметры
    var UO_idCall = Params[0];                                                    // ID-к Вьюхи, которая вызвала
    var UO_Center = Params[1]; if (UO_Center == undefined) UO_Center = false;     // Разместить в центре экрана
    var UO_Modal = Params[2]; if (UO_Modal == undefined) UO_Modal = false;        // Все остальные элементы не активные


    //Для id
    ObjectID++;


    //try {


        //Блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        ObjectEditConfig_UO_idCall_true_false(true);



        switch (pObjectName) {

                //Сервис *** *** ***

            case "viewDocServicePurchesReport": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
                var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
                var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
                //var storeDirServiceContractorsGrid = Ext.create("store.storeDirServiceContractorsGrid"); storeDirServiceContractorsGrid.setData([], false); storeDirServiceContractorsGrid.proxy.url = HTTP_DirServiceContractors + "?type=Grid";
                var storeDocServicePurchesReport = Ext.create("store.storeDocServicePurchesReport"); storeDocServicePurchesReport.setData([], false); storeDocServicePurchesReport.proxy.url = HTTP_DocServicePurchesReport + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                    storeDirWarehousesGrid: storeDirWarehousesGrid,
                    storeDirEmployeesGrid: storeDirEmployeesGrid,
                    storeDirEmployeesMasterGrid: storeDirEmployeesGrid,
                    storeDirServiceStatusesGrid: varStoreDirServiceStatusesGrid,
                    //storeDirServiceContractorsGrid: storeDirServiceContractorsGrid,

                    storeDocServicePurchesReport: storeDocServicePurchesReport
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);


                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                        var rec = { DirWarehouseID: 0, Del: false, DirWarehouseName: "Все" }; storeDirWarehousesGrid.insert(0, rec);

                        storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                        storeDirEmployeesGrid.on('load', function () {
                            if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                            var rec = { DirEmployeeID: 0, DirEmployeeName: "Все" }; storeDirEmployeesGrid.insert(0, rec);

                                loadingMask.hide();

                                Ext.getCmp("DateS" + ObjectID).setValue(new Date());
                                Ext.getCmp("DatePo" + ObjectID).setValue(new Date());
                                Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);

                                //Склад и Организация привязанные к сотруднику
                                //Склад
                                if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                //Организация
                                if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }


                            
                        });
                    });
                });


                break;
            }

                //Прайс-Лист *** *** ***

            case "viewReportPriceList": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirNomensTree = Ext.create("store.storeDirNomensTree"); storeDirNomensTree.setData([], false);

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                    storeDirNomensTree: storeDirNomensTree,
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);

                Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(1);
                Ext.getCmp("PriceGreater0" + ObjectID).setValue(1);

                break;
            }

                //Прибыль *** *** ***

            case "viewReportProfit": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
                var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
                var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                    storeDirWarehousesGrid: storeDirWarehousesGrid,
                    storeDirEmployeesGrid: storeDirEmployeesGrid
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);


                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                        storeDirEmployeesGrid.on('load', function () {
                            if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            loadingMask.hide();

                            Ext.getCmp("DateS" + ObjectID).setValue(new Date());
                            Ext.getCmp("DatePo" + ObjectID).setValue(new Date());
                            Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);

                            //Склад и Организация привязанные к сотруднику
                            //Склад
                            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                            //Организация
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }


                        });
                    });
                });


                break;
            }

                //Остаток *** *** ***

            case "viewReportRemnants": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
                var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
                var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                    storeDirWarehousesGrid: storeDirWarehousesGrid,
                    storeDirEmployeesGrid: storeDirEmployeesGrid
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);

                Ext.getCmp("OperationalBalances" + ObjectID).setValue(true);
                Ext.getCmp("DateS" + ObjectID).disable();
                Ext.getCmp("DatePo" + ObjectID).disable();
                Ext.getCmp("DirEmployeeID" + ObjectID).disable();

                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                        storeDirEmployeesGrid.on('load', function () {
                            if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            loadingMask.hide();

                            Ext.getCmp("DateS" + ObjectID).setValue(new Date());
                            Ext.getCmp("DatePo" + ObjectID).setValue(new Date());
                            Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);

                            //Склад и Организация привязанные к сотруднику
                            //Склад
                            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                            //Организация
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }


                        });
                    });
                });


                break;
            }

                //Отчет по Торговле *** *** ***

            case "viewReportTotalTrade": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
                var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
                var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
                //Стор для Таблицы, в которой будет отображён результат отчета
                var storeReportTotalTrade = Ext.create("store.storeReportTotalTrade"); storeReportTotalTrade.setData([], false); storeReportTotalTrade.proxy.url = HTTP_DirEmployees + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                    storeDirWarehousesGrid: storeDirWarehousesGrid,
                    storeDirEmployeesGrid: storeDirEmployeesGrid,
                    storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,

                    storeReportTotalTrade: storeReportTotalTrade,
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);


                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                        var rec = { DirWarehouseID: 0, Del: false, DirWarehouseName: "Все"  }; storeDirWarehousesGrid.insert(0, rec);


                        storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                        storeDirEmployeesGrid.on('load', function () {
                            if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                            var rec = { DirEmployeeID: 0, DirEmployeeName: "Все" }; storeDirEmployeesGrid.insert(0, rec);

                            loadingMask.hide();

                            Ext.getCmp("DateS" + ObjectID).setValue(new Date());
                            Ext.getCmp("DatePo" + ObjectID).setValue(new Date());
                            Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);

                            //Склад и Организация привязанные к сотруднику
                            //Склад, если Администратор, то есть доступ выбирать другой склад
                            if (varDirEmployeeID != 1) {
                                if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                                else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); Ext.getCmp("DirWarehouseID" + ObjectID).setDisabled(true); /*Ext.getCmp("btnDirWarehousesClear" + ObjectID).setDisabled(true);*/ }
                            }
                            //Организация
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); Ext.getCmp("DirContractorIDOrg" + ObjectID).setDisabled(true); }
                            //Тип цены
                            if (varDirWarehouseIDEmpl != 0 && varDirContractorIDOrgEmpl != 0) { Ext.getCmp("DirPriceTypeID" + ObjectID).setDisabled(true); }
                            //Тип отчета
                            Ext.getCmp("ReportType" + ObjectID).setValue(1);


                        });
                    });
                });


                break;
            }

                //Отчет "Заказ товара на перемещение" *** *** ***

            case "viewReportMovementNomen": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
                var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
                var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                    storeDirWarehousesGrid: storeDirWarehousesGrid,
                    storeDirEmployeesGrid: storeDirEmployeesGrid,
                    storeDirPriceTypesGrid: varStoreDirPriceTypesGrid
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);


                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                        storeDirEmployeesGrid.on('load', function () {
                            if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            loadingMask.hide();

                            Ext.getCmp("DirWarehouseIDTo" + ObjectID).setValue(varDirWarehouseID);

                            //Склад и Организация привязанные к сотруднику
                            //Склад
                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDTo" + ObjectID).setValue(varDirWarehouseID); }
                            else { Ext.getCmp("DirWarehouseIDTo" + ObjectID).setValue(varDirWarehouseIDEmpl); Ext.getCmp("DirWarehouseIDTo" + ObjectID).setDisabled(true); }
                            //Организация
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); Ext.getCmp("DirContractorIDOrg" + ObjectID).setDisabled(true); }


                        });
                    });
                });


                break;
            }

                //Отчет по Логистики *** *** ***

            case "viewReportLogistics": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
                var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
                var storeDirMovementStatusesGrid = Ext.create("store.storeDirMovementStatusesGrid"); storeDirMovementStatusesGrid.setData([], false); storeDirMovementStatusesGrid.proxy.url = HTTP_DirMovementStatuses + "?type=Grid";
                //Стор для Таблицы, в которой будет отображён результат отчета
                var storeReportLogistics = Ext.create("store.storeReportLogistics"); storeReportLogistics.setData([], false); storeReportLogistics.proxy.url = HTTP_DirEmployees + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                    storeDirEmployeesGrid: storeDirEmployeesGrid,
                    storeDirMovementStatusesGrid: storeDirMovementStatusesGrid,

                    storeReportLogistics: storeReportLogistics,
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);


                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                    storeDirEmployeesGrid.on('load', function () {
                        if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                        var rec = { DirEmployeeID: 0, DirEmployeeName: "Все" }; storeDirEmployeesGrid.insert(0, rec);

                        storeDirMovementStatusesGrid.load({ waitMsg: lanLoading });
                        storeDirMovementStatusesGrid.on('load', function () {
                            if (storeDirMovementStatusesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirMovementStatusesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                            var rec = { DirEmployeeID: 0, DirEmployeeName: "Все" }; storeDirMovementStatusesGrid.insert(0, rec);

                            loadingMask.hide();

                            Ext.getCmp("DateS" + ObjectID).setValue(new Date());
                            Ext.getCmp("DatePo" + ObjectID).setValue(new Date());

                            //Склад и Организация привязанные к сотруднику
                            //Организация
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); Ext.getCmp("DirContractorIDOrg" + ObjectID).setDisabled(true); }

                        });
                    });
                });


                break;
            }
                
                //Денег в кассе

            case "viewReportBanksCashOffices": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
                var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
                //Стор для Таблицы, в которой будет отображён результат отчета
                var storeReportBanksCashOffices = Ext.create("store.storeReportBanksCashOffices"); storeReportBanksCashOffices.setData([], false); storeReportBanksCashOffices.proxy.url = HTTP_ReportBanksCashOffices + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirWarehousesGrid: storeDirWarehousesGrid,
                    storeDirEmployeesGrid: storeDirEmployeesGrid,
                    storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,

                    storeReportBanksCashOffices: storeReportBanksCashOffices,
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);

                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                storeDirWarehousesGrid.on('load', function () {
                    if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                    var rec = { DirWarehouseID: 0, Del: false, DirWarehouseName: "Все" }; storeDirWarehousesGrid.insert(0, rec);

                    storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                    storeDirEmployeesGrid.on('load', function () {
                        if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        var rec = { DirEmployeeID: 0, DirEmployeeName: "Все" }; storeDirEmployeesGrid.insert(0, rec);

                        loadingMask.hide();

                        Ext.getCmp("CasheAndBank" + ObjectID).setValue(true);
                        Ext.getCmp("DateS" + ObjectID).setValue(new Date());
                        Ext.getCmp("DatePo" + ObjectID).setValue(new Date());
                        Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);

                        //Склад и Организация привязанные к сотруднику
                        //Склад, если Администратор, то есть доступ выбирать другой склад
                        if (varDirEmployeeID != 1) {
                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); Ext.getCmp("DirWarehouseID" + ObjectID).setDisabled(true); } //Ext.getCmp("btnDirWarehousesClear" + ObjectID).setDisabled(true); 
                        }

                        //Сотрудник
                        Ext.getCmp("DirEmployeeID" + ObjectID).setValue(0);
                        //Тип отчета
                        Ext.getCmp("ReportType" + ObjectID).setValue(1);
                        //Группировка
                        Ext.getCmp("ReportGroup" + ObjectID).setValue(1);


                    });

                });


                break;
            }


                //Зарплата: по сотрудникам

            case "viewReportSalaries": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
                var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
                //Стор для Таблицы, в которой будет отображён результат отчета
                var storeReportSalaries = Ext.create("store.storeReportSalaries"); storeReportSalaries.setData([], false); storeReportSalaries.proxy.url = HTTP_DirEmployees + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                    storeDirEmployeesGrid: storeDirEmployeesGrid,
                    storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,

                    storeReportSalaries: storeReportSalaries,
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);
                


                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                        storeDirEmployeesGrid.on('load', function () {
                            if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                            var rec = { DirEmployeeID: 0, DirEmployeeName: "Все" }; storeDirEmployeesGrid.insert(0, rec);

                            loadingMask.hide();

                            Ext.getCmp("DateS" + ObjectID).setValue(new Date());
                            Ext.getCmp("DatePo" + ObjectID).setValue(new Date());
                            Ext.getCmp("ReportType" + ObjectID).setValue(1);

                            //Организация
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); Ext.getCmp("DirContractorIDOrg" + ObjectID).setDisabled(true); }

                        });
                });


                break;
            }

            //Зарплата: по сотрудникам

            case "viewReportSalariesWarehouses": {

                //Переключаемся на уже открытую вкладку
                var UO_Identy = pObjectName;
                if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

                var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
                var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
                //Стор для Таблицы, в которой будет отображён результат отчета
                var storeReportSalariesWarehouses = Ext.create("store.storeReportSalariesWarehouses"); storeReportSalariesWarehouses.setData([], false); storeReportSalariesWarehouses.proxy.url = HTTP_DirEmployees + "?type=Grid";

                // === Формируем и показываем окно ===
                var widgetX = Ext.create("widget." + pObjectName, {
                    id: pObjectName + ObjectID,
                    UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                    UO_id: ObjectID,
                    UO_idMain: pObjectName + ObjectID,
                    UO_idCall: UO_idCall,
                    //UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                    modal: UO_Modal,
                    UO_Center: UO_Center,

                    storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                    storeDirWarehousesGrid: storeDirWarehousesGrid,
                    storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,

                    storeReportSalariesWarehouses: storeReportSalariesWarehouses,
                });

                //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
                //Если панель, но удаляется текущий виджет
                ObjectShow(widgetX);


                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
                loadingMask.show();

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                        var rec = { DirEmployeeID: 0, DirEmployeeName: "Все" }; storeDirWarehousesGrid.insert(0, rec);

                        loadingMask.hide();

                        Ext.getCmp("DateS" + ObjectID).setValue(new Date());
                        Ext.getCmp("DatePo" + ObjectID).setValue(new Date());

                        //Организация
                        if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                        else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); Ext.getCmp("DirContractorIDOrg" + ObjectID).setDisabled(true); }

                    });
                });


                //Прячем лишние колонки, если не Админ (требование Сергея)
                if (varDirEmployeeID != 1) {
                    var col = Ext.getCmp("grid_" + ObjectID).columns;
                    col[0].setVisible(false);
                    col[1].setVisible(false);
                    col[2].setVisible(false);
                    col[3].setVisible(false);
                    col[4].setVisible(false);
                    col[5].setVisible(false);
                    col[6].setVisible(false);
                    col[7].setVisible(false);
                    col[8].setVisible(false);
                    col[9].setVisible(false);
                    col[10].setVisible(false);
                    col[11].setVisible(false);
                    col[12].setVisible(false);
                    col[13].setVisible(false);
                }


                break;
            }




            default: {
                Ext.Msg.alert("ObjectEditConfig", "Object '" + pObjectName + "' not found!");

                //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
                //if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined && New_Edit > 1) { Ext.getCmp(UO_idCall).enable();}

                break;
            }

        }



        // === Function ===

        //Блокировать или Разблокировать вызвавший элемент "UO_idCall"
        function ObjectEditConfig_UO_idCall_true_false(Disable) {
            //Не блокировать вызванные из меню!
            if (UO_idCall == "viewContainerHeader") return;

            /*
            if (Disable) {
                //Блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
                if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined && New_Edit > 1) { Ext.getCmp(UO_idCall).disable();}
            }
            else {
                //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
                if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined && New_Edit > 1) { Ext.getCmp(UO_idCall).enable();}
            }
            */
        }

    /*
    } catch (ex) {
        var exMsg = ex;
        if (exMsg.message != undefined) exMsg = ex.message;

        Ext.Msg.alert(lanOrgName, "Ошибка в скрипте! Вышлите, пожалуйста скриншот на: support@uchetoblako.ru<br />Подробности:" + exMsg);

        //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        //if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined && New_Edit > 1) { Ext.getCmp(UO_idCall).enable();}

        //Разблокировка вызвавшего окна
        //ObjectEditConfig_UO_idCall_true_false(false);
    }
    */

}