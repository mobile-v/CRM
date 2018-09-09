//Маршрутизатор на View-грид
//Параметры:
//  pObjectName - Наименование обекта: Справочник.Товары, Документ.[Приходные накладные]
//  Params      - массив дополнительных параметров:
//   - UO_idCall        - ID-и Вьюхи, которая вызвала
//   - UO_Center        - Разместить в центре экрана
//   - UO_Modal         - Все остальные элементы не активные
//   - UO_Function_Tree - При "Клике" на Tree вызвать функцию
//   - UO_Function_Grid - При "Клике" на Grid вызвать функцию
//   - TreeShow         - Показывать Tree, если есть
//   - GridShow         - Показывать Grid
//   - TreeServerParam1 - Передача параметра серверу (для Трии)
//   - GridServerParam1 - Передача параметра серверу (для Грида)
//   - ContainerWidget  - Контейнер, в котором будут распологатся виджет "pObjectName"

//Подбор товара
//   - UO_idTab         - id-шник Спецификации (Табличной части) документа, что бы туда вставить выбранный Товар (с левой панели)
//   - UO_FunRecalcSum  - Функция пересчета сумм, для каждого докумена своя

//Массив
//   - ArrList          - Массив "Params[12]" в которм содержатся параметры индивидуально для каждой Вьюхи
//                        ArrList = [Data1, Data2, ...]

function ObjectConfig(pObjectName, Params) {

    //Если окно открыто, то устанавливаем на него фокус
    if (funSearchWin(pObjectName, true)) return;

    //Параметры
    var UO_idCall = Params[0];                                                  // ID-и Вьюхи, которая вызвала
    var UO_Center = Params[1]; if (UO_Center == undefined) UO_Center = false;   // Разместить в центре экрана
    var UO_Modal = Params[2]; if (UO_Modal == undefined) UO_Modal = false;      // Все остальные элементы не активные
    var UO_Function_Tree = Params[3];                                           // При "Клике" (на Tree or Grid) вызвать функцию
    var UO_Function_Grid = Params[4];                                           // При "Клике" (на Tree or Grid) вызвать функцию
    var TreeShow = Params[5]; if (TreeShow == undefined) TreeShow = true;       // Показывать Tree, если есть
    var GridShow = Params[6]; if (GridShow == undefined) GridShow = true;       // Показывать Grid
    var TreeServerParam1 = Params[7];                                           // Передача параметра серверу (для Трии)
    var GridServerParam1 = Params[8];                                           // Передача параметра серверу (для Грида)
    var ContainerWidget = Params[9];                                            // Контейнер, в котором будут распологатся виджет "pObjectName"
    //Подбор товара
    var UO_idTab = Params[10];                                                  // id-шник Спецификации (Табличной части) документа, что бы туда вставить выбранный Товар (с левой панели)
    var UO_FunRecalcSum = Params[11];                                           // Функция пересчета сумм, для каждого докумена своя
    //Массив
    var ArrList = Params[12];                                                   //  Массив "Params[12]" в которм содержатся параметры индивидуально для каждой Вьюхи


    //Для id
    ObjectID++;

    //Виджет "widgetX" который в конце функции будет добавлен в функцию "ObjectShow(widgetX, ContainerWidget)"
    var widgetX;



    //try {


    //Блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
    //ObjectEditConfig_UO_idCall_true_false(UO_idCall, true);



    switch (pObjectName) {



        // === Main ===

        case "viewMain": {
            // === Формируем и показываем окно ===
            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName,
                UO_id: "",
                UO_idMain: pObjectName,
                UO_View: pObjectName,

                UO_idCall: UO_idCall, //"viewPanelMain"

                modal: UO_Modal
            });

            //Добавляем в свойство "Html" оплаты клиента
            var varDirPayServiceNameHtml = "Тарифный план: " + varDirPayServiceName + "<br />";
            if (varPaymentExpired) varDirPayServiceNameHtml = "<b style='color:red'>Тарифный план: " + varDirPayServiceName + "</b><br />";
            var varPayDateEndHtml = "Окончание: " + varPayDateEnd + "<br /><br />";
            if (varPaymentExpired) {
                varPayDateEndHtml = "<b style='color:red'>Окончание: " + varPayDateEnd + " (через: " + nDaysLeft + " дня)<br /></b><br />";
            }

            widgetX.setHtml(
                widgetX.html +
                "<center>" +

                "Логин: " + varDirEmployeeLogin + "<br />" +
                "Сотрудник: " + lanDirEmployeeName + "<br /><br />" +

                varDirPayServiceNameHtml + //"Тарифный план: " + varDirPayServiceName + "<br />" +
                "Сотрудников: " + varCountUser + "<br />" +
                //"Торговых точек: " + varCountTT + "<br />" +
                //"Интернет магазинов: " + varCountIM + "<br />" +
                varPayDateEndHtml + //"Окончание: " + varPayDateEnd + "<br /><br />" +
                "© «ВТорговомОблаке» 2017" +
                "</center>"
                );
            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //ObjectShow(widgetX);
            break;
        }



            //Настройки *** *** ***


        case "viewDirEmployees": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirEmployeesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirEmployees + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirEmployees + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirWarehouses"
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false);
            storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            storeDirWarehousesGrid.load({ waitMsg: lanLoading });

            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false);
            storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            //storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirCurrencies"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false);
            storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            //storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            var storeDirBonusesGrid = Ext.create("store.storeDirBonusesGrid"); storeDirBonusesGrid.setData([], false);
            storeDirBonusesGrid.proxy.url = HTTP_DirBonuses + "?type=Grid";
            //storeDirBonusesGrid.load({ waitMsg: lanLoading });

            //Store Grid "DirEmployeeWarehouses"
            var storeDirEmployeeWarehousesGrid = Ext.create("store.storeDirEmployeeWarehousesGrid"); storeDirEmployeeWarehousesGrid.setData([], false);


            //Panel
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirBonusesGrid: storeDirBonusesGrid,

                storeDirEmployeeWarehousesGrid: storeDirEmployeeWarehousesGrid
            });
            ObjectShow(widgetX);

            //Глючит вкладка "Права". Надо переключится на неё, что бы не глючила. Ну и потом обратно на первую вкладку.
            Ext.getCmp("tab_" + ObjectID).setActiveTab(4);
            Ext.getCmp("tab_" + ObjectID).setActiveTab(0);
            Ext.getCmp("SalaryPercentService1Tabs" + ObjectID).setValue(0);
            Ext.getCmp("SalaryPercentService2Tabs" + ObjectID).setValue(0);

            //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirWarehousesGrid.on('load', function () {
                if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
                    storeDirCurrenciesGrid.on('load', function () {
                        if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                        Ext.getCmp("DirCurrencyID" + ObjectID).setValue(1);

                        storeDirBonusesGrid.load({ waitMsg: lanLoading });
                        storeDirBonusesGrid.on('load', function () {
                            if (storeDirBonusesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirBonusesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            loadingMask.hide();

                            //Глючат права (перекидывает наверх при отметке прав)
                            var widgetXForm = Ext.getCmp("form_" + ObjectID);
                            widgetXForm.submit({ method: "POST", url: HTTP_DirEmployees + "?id=0", timeout: varTimeOutDefault, waitMsg: lanUploading, });

                            //Глючит вкладка "Права". Надо переключится на неё, что бы не глючила. Ну и потом обратно на первую вкладку.
                            //Ext.getCmp("tab_" + ObjectID).setActiveTab(4);
                            //Ext.getCmp("tab_" + ObjectID).setActiveTab(0);

                        });
                    });
                });
            });


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();

            return;
            //break;
        }

        case "viewDirEmployeeHistories": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirEmployeeHistoriesGrid"); storeGrid.setData([], false);
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (GridServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirEmployeeHistories + "?" + GridServerParam1;
            //Загружаем
            storeGrid.load({ waitMsg: lanLoading, GridShow: false });


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
            });

            //Убираем лишние кнопки
            Ext.getCmp("btnNew" + ObjectID).setVisible(false);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);
            Ext.getCmp("btnEdit" + ObjectID).setVisible(false);
            Ext.getCmp("btnHelp" + ObjectID).setVisible(false);

            //Грид не нужен - удалить Грид
            if (!GridShow) {
                widgetX.remove(Ext.getCmp("grid_" + ObjectID), false);
                var myPanel = Ext.getCmp("tree_" + ObjectID);
                if (myPanel != undefined) { myPanel.setRegion("center"); myPanel.collapsible = false; }
                widgetX.width = 220;
            }

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //ObjectShow(widgetX); //, GridShow
            break;
        }




            //Справочники *** *** ***


        case "viewDirNomens": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirNomensTree"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DirNomens + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirNomens + "?type=Tree" + "&" + TreeServerParam1;

            var storeDirNomenTypesGrid = Ext.create("store.storeDirNomenTypesGrid"); storeDirNomenTypesGrid.setData([], false); storeDirNomenTypesGrid.proxy.url = HTTP_DirNomenTypes + "?type=Grid"; storeDirNomenTypesGrid.load({ waitMsg: lanLoading });
            var storeDirNomenCategoriesGrid = Ext.create("store.storeDirNomenCategoriesGrid"); storeDirNomenCategoriesGrid.setData([], false); storeDirNomenCategoriesGrid.proxy.url = HTTP_DirNomenCategories + "?type=Grid";
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            //Информация - не обновляется
            //История цен
            var storeDirNomenHistoriesGrid = Ext.create("store.storeDirNomenHistoriesGrid"); storeDirNomenHistoriesGrid.setData([], false);
            storeDirNomenHistoriesGrid.proxy.url = HTTP_DirNomenHistories + "?type=Grid";
            //Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);
            storeRemPartiesGrid.proxy.url = HTTP_RemParties + "?type=Grid";
            //Списание партий
            var storeRemPartyMinusesGrid = Ext.create("store.storeRemPartyMinusesGrid"); storeRemPartyMinusesGrid.setData([], false);
            storeRemPartyMinusesGrid.proxy.url = HTTP_RemPartyMinuses + "?type=Grid";


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirNomenTypesGrid: storeDirNomenTypesGrid,
                storeDirNomenCategoriesGrid: storeDirNomenCategoriesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                storeDirNomenHistoriesGrid: storeDirNomenHistoriesGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,
                storeRemPartyMinusesGrid: storeRemPartyMinusesGrid,
            });
            ObjectShow(widgetX);

            //При наведении на "Ext.Img" сделать курсор в виде руки (pointer)
            Ext.getCmp("imageShow" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image1Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image2Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image3Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image4Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image5Show" + ObjectID).setStyle("cursor", "pointer");

            Ext.getCmp("SearchType" + ObjectID).setValue(1);

            //Прячим правую часть
            Ext.getCmp("tab_" + ObjectID).setVisible(GridShow);
            
            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirNomenTypesGrid.on('load', function () {

                storeDirNomenCategoriesGrid.load({ waitMsg: lanLoading });
                storeDirNomenCategoriesGrid.on('load', function () {

                    storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
                    storeDirCurrenciesGrid.on('load', function () {

                        loadingMask.hide();

                    });
                });
            });


            //Блокируем кнопки для Tree
            //Ext.getCmp("expandAll" + ObjectID).disable();
            //Ext.getCmp("collapseAll" + ObjectID).disable();
            //Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirContractors": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirContractorsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirContractors + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirContractors + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirContractor1Types"
            var storeDirContractor1TypesGrid = Ext.create("store.storeDirContractor1TypesGrid"); storeDirContractor1TypesGrid.setData([], false);
            storeDirContractor1TypesGrid.proxy.url = HTTP_DirContractor1Types + "?type=Grid";
            storeDirContractor1TypesGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirContractor2Types"
            var storeDirContractor2TypesGrid = Ext.create("store.storeDirContractor2TypesGrid"); storeDirContractor2TypesGrid.setData([], false);
            storeDirContractor2TypesGrid.proxy.url = HTTP_DirContractor2Types + "?type=Grid";
            //storeDirContractor2TypesGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirDiscounts"
            var storeDirDiscountsGrid = Ext.create("store.storeDirDiscountsGrid"); storeDirDiscountsGrid.setData([], false);
            storeDirDiscountsGrid.proxy.url = HTTP_DirDiscounts + "?type=Grid";
            //storeDirDiscountsGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirContractor1TypesGrid: storeDirContractor1TypesGrid,
                storeDirContractor2TypesGrid: storeDirContractor2TypesGrid,
                storeDirDiscountsGrid: storeDirDiscountsGrid,
            });
            ObjectShow(widgetX);

            //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX
            });
            //var loadingMask = new Ext.LoadMask(widgetX, { useMsg: false });
            loadingMask.show();
            storeDirContractor1TypesGrid.on('load', function () {
                storeDirContractor2TypesGrid.load({ waitMsg: lanLoading });
                storeDirContractor2TypesGrid.on('load', function () {
                    storeDirDiscountsGrid.load({ waitMsg: lanLoading });
                    storeDirDiscountsGrid.on('load', function () {
                        loadingMask.hide();
                    });
                });
            });

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();

            //Фиксированная скидка = 0
            Ext.getCmp("DirContractorDiscount" + ObjectID).setValue(0);

            return;
        }


        case "viewDirWarehouses": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirWarehousesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirWarehouses + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirWarehouses + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirCashOfficesGrid"
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBanksGrid"
            var storeDirBanksGrid = Ext.create("store.storeDirBanksGrid"); storeDirBanksGrid.setData([], false);
            storeDirBanksGrid.proxy.url = HTTP_DirBanks + "?type=Grid";
            //storeDirBanksGrid.load({ waitMsg: lanLoading });

            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false);

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirCashOfficesGrid: storeDirCashOfficesGrid,
                storeDirBanksGrid: storeDirBanksGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
            });
            ObjectShow(widgetX);


            Ext.getCmp("SalaryPercentTrade" + ObjectID).setValue(0);
            Ext.getCmp("SalaryPercentService1Tabs" + ObjectID).setValue(0);
            Ext.getCmp("SalaryPercentService2Tabs" + ObjectID).setValue(0);
            Ext.getCmp("SalaryPercentSecond" + ObjectID).setValue(0);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCashOfficesGrid.on('load', function () {
                if (storeDirCashOfficesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCashOfficesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirBanksGrid.load({ waitMsg: lanLoading });
                storeDirBanksGrid.on('load', function () {
                    if (storeDirBanksGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirBanksGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    loadingMask.hide();


                    //По-умолчанию
                    Ext.getCmp("SalaryPercentTradeType" + ObjectID).setValue(1);
                    Ext.getCmp("SalaryPercentTrade" + ObjectID).setValue(0);

                    Ext.getCmp("SalaryPercentService1TabsType" + ObjectID).setValue(1);
                    Ext.getCmp("SalaryPercentService1Tabs" + ObjectID).setValue(0);

                    Ext.getCmp("SalaryPercentService2TabsType" + ObjectID).setValue(1);
                    Ext.getCmp("SalaryPercentService2Tabs" + ObjectID).setValue(0);

                    Ext.getCmp("SalaryPercentSecond" + ObjectID).setValue(0);
                    Ext.getCmp("SalaryPercent2Second" + ObjectID).setValue(0);
                    Ext.getCmp("SalaryPercent3Second" + ObjectID).setValue(0);

                    Ext.getCmp("SalaryPercent4Second" + ObjectID).setValue(0);
                    Ext.getCmp("SalaryPercent5Second" + ObjectID).setValue(0);
                    Ext.getCmp("SalaryPercent6Second" + ObjectID).setValue(0);


                    //Сообщение!
                    Ext.Msg.alert(
                        lanOrgName,
                        "Внимание!<br/>" +
                        "Если создаёте новую Точку! То создайте новые записи для Кассы и Банка.<br/>" +
                        "И выберите их в Точке."
                    );

                });

            });


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirBanks": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirBanksTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirBanks + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirBanks + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCurrenciesGrid.on('load', function () {
                loadingMask.hide();
            });


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }

        case "viewDirBanksGrid": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Combo "storeGrid"
            var storeGrid = Ext.create("store.storeDirBanksGrid"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirBanks + "?type=Grid";
            if (GridServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirBanks + "?type=Grid" + "&" + GridServerParam1;
            storeGrid.load({ waitMsg: lanLoading });

            storeGrid.on('load', function () {
                var sss = storeGrid;
            });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid
            });
            ObjectShow(widgetX);

            Ext.getCmp("btnNew" + ObjectID).setVisible(false);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);
            Ext.getCmp("btnDelete" + ObjectID).setVisible(false);

            return;
        }


        case "viewDirCashOffices": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCashOfficesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCashOffices + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCashOffices + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCurrenciesGrid.on('load', function () {
                loadingMask.hide();
            });


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }

        case "viewDirCashOfficesGrid": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Combo "storeGrid"
            var storeGrid = Ext.create("store.storeDirCashOfficesGrid"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            if (GridServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid" + "&" + GridServerParam1;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid
            });
            ObjectShow(widgetX);

            Ext.getCmp("btnNew" + ObjectID).setVisible(false);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);
            Ext.getCmp("btnDelete" + ObjectID).setVisible(false);

            return;
        }


        case "viewDirCurrencies": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCurrenciesTree"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DirCurrencies + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCurrencies + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }

        case "viewDirCurrencyHistories": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCurrencyHistoriesGrid"); storeGrid.setData([], false);
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (GridServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCurrencyHistories + "?" + GridServerParam1;
            //Загружаем
            storeGrid.load({ waitMsg: lanLoading, GridShow: false });


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
            });

            //Убираем лишние кнопки
            Ext.getCmp("btnNew" + ObjectID).setVisible(false);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);
            Ext.getCmp("btnEdit" + ObjectID).setVisible(false);
            Ext.getCmp("btnHelp" + ObjectID).setVisible(false);

            //Грид не нужен - удалить Грид
            if (!GridShow) {
                widgetX.remove(Ext.getCmp("grid_" + ObjectID), false);
                var myPanel = Ext.getCmp("tree_" + ObjectID);
                if (myPanel != undefined) { myPanel.setRegion("center"); myPanel.collapsible = false; }
                widgetX.width = 220;
            }

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //ObjectShow(widgetX); //, GridShow
            break;
        }


        case "viewDirVats": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirVatsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirVats + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirVats + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);



            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirDiscounts": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirDiscountsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirDiscounts + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirDiscounts + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });


            //Store ComboGrid "DirBonuses"
            var storeDirDiscountTabsGrid = Ext.create("store.storeDirDiscountTabsGrid"); storeDirDiscountTabsGrid.setData([], false);
            //storeDirDiscountTabsGrid.proxy.url = HTTP_DirDiscountTabs + "/1/";
            //storeDirDiscountTabsGrid.load({ waitMsg: lanLoading });


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirDiscountTabsGrid: storeDirDiscountTabsGrid,
            });
            ObjectShow(widgetX);



            //Лоадер
            /*var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirDiscountTabsGrid.on('load', function () {
                loadingMask.hide();
            });*/



            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirBonuses": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirBonusesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirBonuses + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirBonuses + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });


            //Store ComboGrid "DirBonuses"
            var storeDirBonusTabsGrid = Ext.create("store.storeDirBonusTabsGrid"); storeDirBonusTabsGrid.setData([], false);
            //storeDirBonusTabsGrid.proxy.url = HTTP_DirBonusTabs + "/1/";
            //storeDirBonusTabsGrid.load({ waitMsg: lanLoading });


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirBonusTabsGrid: storeDirBonusTabsGrid,
            });
            ObjectShow(widgetX);


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }

        case "viewDirBonus2es": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirBonus2esTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirBonus2es + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirBonus2es + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });


            //Store ComboGrid "DirBonus2es"
            var storeDirBonus2TabsGrid = Ext.create("store.storeDirBonus2TabsGrid"); storeDirBonus2TabsGrid.setData([], false);
            //storeDirBonus2TabsGrid.proxy.url = HTTP_DirBonusTabs + "/1/";
            //storeDirBonus2TabsGrid.load({ waitMsg: lanLoading });


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirBonus2TabsGrid: storeDirBonus2TabsGrid,
            });
            ObjectShow(widgetX);


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirNomenCategories": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirNomenCategoriesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirNomenCategories + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirNomenCategories + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


            // === Характеристики === === ===

        case "viewDirCharColours": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCharColoursTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCharColours + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCharColours + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });*/

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirCharMaterials": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCharMaterialsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCharMaterials + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCharMaterials + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });*/

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirCharNames": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCharNamesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCharNames + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCharNames + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirCharSeasons": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCharSeasonsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCharSeasons + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCharSeasons + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirCharSexes": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCharSexesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCharSexes + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCharSexes + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirCharSizes": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCharSizesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCharSizes + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCharSizes + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirCharStyles": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCharStylesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCharStyles + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCharStyles + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirCharTextures": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirCharTexturesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirCharTextures + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirCharTextures + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


            // Сервисный центр

        case "viewDirServiceNomens": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirServiceNomensTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirServiceNomens + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirServiceNomens + "?type=Tree" + "&" + TreeServerParam1;


            var storeDirServiceNomenPricesGrid = Ext.create("store.storeDirServiceNomenPricesGrid"); storeDirServiceNomenPricesGrid.setData([], false); //storeDirServiceNomenPricesGrid.proxy.url = HTTP_DirServiceNomenPrices + "?type=Grid&DirServiceNomenID=" + IdcallModelData.id;

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirServiceNomenPricesGrid: storeDirServiceNomenPricesGrid
            });
            ObjectShow(widgetX);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);

            return;
        }


        case "viewDirServiceContractors": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirServiceContractorsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirServiceContractors + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirServiceContractors + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirContractor1Types"
            var storeDirContractor1TypesGrid = Ext.create("store.storeDirContractor1TypesGrid"); storeDirContractor1TypesGrid.setData([], false);
            storeDirContractor1TypesGrid.proxy.url = HTTP_DirContractor1Types + "?type=Grid";
            storeDirContractor1TypesGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirDiscounts"
            var storeDirDiscountsGrid = Ext.create("store.storeDirDiscountsGrid"); storeDirDiscountsGrid.setData([], false);
            storeDirDiscountsGrid.proxy.url = HTTP_DirDiscounts + "?type=Grid";
            //storeDirDiscountsGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirContractor1TypesGrid: storeDirContractor1TypesGrid,
                storeDirDiscountsGrid: storeDirDiscountsGrid
            });
            ObjectShow(widgetX);

            //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX
            });
            //var loadingMask = new Ext.LoadMask(widgetX, { useMsg: false });
            loadingMask.show();
            storeDirContractor1TypesGrid.on('load', function () {
                storeDirDiscountsGrid.load({ waitMsg: lanLoading });
                storeDirDiscountsGrid.on('load', function () {
                    loadingMask.hide();
                });
            });

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();

            return;
        }


        case "viewDirServiceNomenCategories": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirServiceNomenCategoriesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirServiceNomenCategories + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirServiceNomenCategories + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirServiceJobNomens": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirServiceJobNomensTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirServiceJobNomens + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirServiceJobNomens + "?type=Tree" + "&" + TreeServerParam1; //storeGrid.load({ waitMsg: lanLoading });

            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid
            });
            ObjectShow(widgetX);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                //var x = storeDirCurrenciesGrid;
                if (TreeServerParam1 == "DirServiceJobNomenType=1") {
                    Ext.getCmp("DirServiceJobNomenType" + ObjectID).setValue(1);
                    Ext.getCmp("DirServiceJobNomenType_" + ObjectID).setValue(1);
                }
                else {
                    Ext.getCmp("DirServiceJobNomenType" + ObjectID).setValue(2);
                    Ext.getCmp("DirServiceJobNomenType_" + ObjectID).setValue(2);
                }

                loadingMask.hide();

            });

            //Блокируем кнопки для Tree
            //Ext.getCmp("expandAll" + ObjectID).disable();
            //Ext.getCmp("collapseAll" + ObjectID).disable();
            //Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }


        case "viewDirServiceJobNomenPrices": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirServiceJobNomensTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirServiceJobNomens + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirServiceJobNomens + "?type=Tree" + "&" + TreeServerParam1; //storeGrid.load({ waitMsg: lanLoading });

            var storeDirServiceJobNomenHistoriesGrid = Ext.create("store.storeDirServiceJobNomenHistoriesGrid"); storeDirServiceJobNomenHistoriesGrid.setData([], false); storeDirServiceJobNomenHistoriesGrid.proxy.url = HTTP_DirServiceJobNomenHistories + "?type=Grid"; //storeDirServiceJobNomenHistoriesGrid.load({ waitMsg: lanLoading });


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                UO_FunRecalcSum: UO_FunRecalcSum,

                modal: UO_Modal,
                storeGrid: storeGrid,
                storeDirServiceJobNomenHistoriesGrid: storeDirServiceJobNomenHistoriesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
            });
            ObjectShow(widgetX);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(1);
            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(1);


            //var x = storeDirCurrenciesGrid;
            /*if (TreeServerParam1 == "DirServiceJobNomenType=1") {
                Ext.getCmp("DirServiceJobNomenType" + ObjectID).setValue(1);
                Ext.getCmp("DirServiceJobNomenType_" + ObjectID).setValue(1);
            }
            else {
                Ext.getCmp("DirServiceJobNomenType" + ObjectID).setValue(2);
                Ext.getCmp("DirServiceJobNomenType_" + ObjectID).setValue(2);
            }*/

            /*
            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirNomenTypesGrid.on('load', function () {
                storeDirServiceNomenCategoriesGrid.load({ waitMsg: lanLoading });

                storeDirServiceNomenCategoriesGrid.on('load', function () {
                    loadingMask.hide();
                });
            });
            */

            //Блокируем кнопки для Tree
            //Ext.getCmp("expandAll" + ObjectID).disable();
            //Ext.getCmp("collapseAll" + ObjectID).disable();
            //Ext.getCmp("FolderNewSub" + ObjectID).disable();


            if (TreeServerParam1 == "DirServiceJobNomenType=1") {
                Ext.getCmp("DirServiceJobNomenType" + ObjectID).setValue(1);
            }
            else {
                Ext.getCmp("DirServiceJobNomenType" + ObjectID).setValue(2);
            }


            return;
        }


        case "viewDirNomenRemParties": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirNomensTree"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DirNomens + "?type=Tree"; //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirNomens + "?type=Tree" + "&" + TreeServerParam1; //storeGrid.load({ waitMsg: lanLoading });
            //Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false); storeRemPartiesGrid.proxy.url = HTTP_RemParties + "?type=Grid";


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,

                storeRemPartiesGrid: storeRemPartiesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
            });
            ObjectShow(widgetX);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(1);

            Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(
                Ext.getCmp("DirContractorIDOrg" + Ext.getCmp(UO_idCall).UO_id).getValue()
                );
            Ext.getCmp("DirWarehouseID" + ObjectID).setValue(
                Ext.getCmp("DirWarehouseID" + Ext.getCmp(UO_idCall).UO_id).getValue()
                );
            Ext.getCmp("DocDate" + ObjectID).setValue(
                Ext.getCmp("DocDate" + Ext.getCmp(UO_idCall).UO_id).getValue()
            );

            //Важно!
            //Тип документа: Магазин (1, 2), Мастерская, БУ
            Ext.getCmp("DirOrderIntTypeID" + ObjectID).setValue(UO_idTab);


            //Блокируем кнопки для Tree
            //Ext.getCmp("expandAll" + ObjectID).disable();
            //Ext.getCmp("collapseAll" + ObjectID).disable();
            //Ext.getCmp("FolderNewSub" + ObjectID).disable();


            //Поиск в Дереве по наименованию модели
            //Надо искать 2-е вложености: Планшет / Lenovo / A5500 /
            //1. Планшет
            //2. Lenovo
            storeGrid.on('load', function () {
                if (storeGrid.UO_Loaded) return;
                storeGrid.UO_Loaded = true;

                //Только если есть на форме "ID0" (может быть это Разборка - там нет)
                if (Ext.getCmp("ID0" + Ext.getCmp(UO_idCall).UO_id)) {

                    var panel = Ext.getCmp("tree_" + ObjectID);
                    var rn = panel.getRootNode();

                    //ID0
                    var c0 = rn.findChild("text", Ext.getCmp("ID0" + Ext.getCmp(UO_idCall).UO_id).getValue().toUpperCase(), true);
                    if (c0) {
                        c0.expand();

                        //***
                        var c1 = rn.findChild("text", Ext.getCmp("ID1" + Ext.getCmp(UO_idCall).UO_id).getValue().toUpperCase(), true);
                        if (c1) {
                            c1.expand();

                            //***
                            var c2 = rn.findChild("text", Ext.getCmp("ID2" + Ext.getCmp(UO_idCall).UO_id).getValue().toUpperCase(), true);
                            if (c2) { c2.expand(); }
                        }

                        else {
                            var c2 = rn.findChild("text", Ext.getCmp("ID2" + Ext.getCmp(UO_idCall).UO_id).getValue().toUpperCase(), true);
                            if (c2) { c2.expand(); }
                        }
                    }
                    else {
                        var c1 = rn.findChild("text", Ext.getCmp("ID1" + Ext.getCmp(UO_idCall).UO_id).getValue().toUpperCase(), true);
                        if (c1) {
                            c1.expand();

                            //***
                            var c2 = rn.findChild("text", Ext.getCmp("ID2" + Ext.getCmp(UO_idCall).UO_id).getValue().toUpperCase(), true);
                            if (c2) { c2.expand(); }
                        }

                        else {
                            var c2 = rn.findChild("text", Ext.getCmp("ID2" + Ext.getCmp(UO_idCall).UO_id).getValue().toUpperCase(), true);
                            if (c2) { c2.expand(); }
                        }
                    }

                }
            });
            


            return;
        }


        case "viewDirServiceProblems": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirServiceProblemsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirServiceProblems + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirServiceProblems + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
            });
            ObjectShow(widgetX);

            //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, false);


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();

            return;
        }


        case "viewDirSmsTemplates": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirSmsTemplatesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirSmsTemplates + "?type=Tree&MenuID=" + UO_idTab;
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirSmsTemplates + "?type=Tree&MenuID=" + UO_idTab + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            var DirSmsTemplateType_values;
            if (UO_idTab == 1) DirSmsTemplateType_values = DirSmsTemplateType_values1;
            else if (UO_idTab == 2) DirSmsTemplateType_values = DirSmsTemplateType_values2;
            else if (UO_idTab == 3) DirSmsTemplateType_values = DirSmsTemplateType_values3;

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                UO_idTab: UO_idTab,

                modal: UO_Modal,
                storeGrid: storeGrid,
                DirSmsTemplateType_values: DirSmsTemplateType_values
            });
            ObjectShow(widgetX);

            //По умолчанию
            Ext.getCmp("MenuID" + ObjectID).setValue(UO_idTab);

            if (UO_idTab == 1) {
                Ext.getCmp("DirSmsTemplate7" + ObjectID).disable(true);
                Ext.getCmp("DirSmsTemplate8" + ObjectID).disable(true);
            } 
            else if (UO_idTab == 2) {
                //...
            }
            else {
                Ext.getCmp("DirSmsTemplate7" + ObjectID).disable(true);
                Ext.getCmp("DirSmsTemplate8" + ObjectID).disable(true);
            }

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }

        case "viewDirServiceDiagnosticRresults": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirServiceDiagnosticRresultsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirServiceDiagnosticRresults + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirServiceDiagnosticRresults + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
            });
            ObjectShow(widgetX);

            //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, false);


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();

            return;
        }

        case "viewDirServiceNomenTypicalFaults": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirServiceNomenTypicalFaultsTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirServiceNomenTypicalFaults + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirServiceNomenTypicalFaults + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);


            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({
                msg: 'Please wait...',
                target: widgetX //widgetX //Ext.getCmp("tree_" + aButton.UO_id)
            });
            loadingMask.show();
            storeDirCashOfficesGrid.on('load', function () {
                loadingMask.hide();
            });
            */


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();
            Ext.getCmp("FolderNew" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();
            Ext.getCmp("FolderCopy" + ObjectID).disable();
            Ext.getCmp("FolderDel" + ObjectID).disable();


            return;
        }

        case "viewDirOrdersStates": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirOrdersStatesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirOrdersStates + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirOrdersStates + "?type=Tree" + "&" + TreeServerParam1;
            //storeGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            /*
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false);
            storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";
            storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
            */

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);
            


            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();


            return;
        }




            //Торговля *** *** ***


        case "viewDocPurches": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;


            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid"; //&WarehouseAll=true

            //Store Grid
            var storeGrid = Ext.create("store.storeDocPurchesGrid"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DocPurches + "?DirWarehouseID=" + varDirWarehouseID;
            storeGrid.UO_Proxy_Url = HTTP_DocPurches; // + "?DirWarehouseID=" + varDirWarehouseID;
            storeGrid.pageSize = varPageSizeJurn;


            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);
            Ext.getCmp("DirWarehouseIDPanelGrid0_" + ObjectID).setVisible(true);

            
               
            storeDirWarehousesGrid.load({ waitMsg: lanLoading });
            storeDirWarehousesGrid.on('load', function () {
                if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                var rec = { DirWarehouseID: 0, Del: false, DirWarehouseName: "Все" }; storeDirWarehousesGrid.insert(0, rec);

                Ext.getCmp("DirWarehouseIDPanelGrid0_" + ObjectID).setValue(varDirWarehouseID);

                storeGrid.load({ waitMsg: lanLoading });
                storeGrid.on('load', function () {

                    //loadingMask.hide();

                });
            });


            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocSales": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocSalesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocSales + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocMovements": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            var param1;
            if (ArrList != undefined && ArrList.length > 0) param1 = ArrList[0];

            //Store Grid
            var storeGrid = Ext.create("store.storeDocMovementsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocMovements + "?DirWarehouseID=" + varDirWarehouseID + "&" + param1; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            //Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            //Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocReturnVendors": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocReturnVendorsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocReturnVendors + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocActWriteOffs": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocActWriteOffsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocActWriteOffs + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocReturnsCustomers": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocReturnsCustomersGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocReturnsCustomers + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocActOnWorks": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocActOnWorksGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocActOnWorks + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocAccounts": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocAccountsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocAccounts + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocInventories": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocInventoriesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocInventories + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocNomenRevaluations": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocNomenRevaluationsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocNomenRevaluations + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }



            //Торговля *** *** ***


        case "viewDocSalaries": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocSalariesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocSalaries + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


            //Логистика *** *** ***


        case "viewLogistics": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            //0
            var storeGrid0 = Ext.create("store.storeLogisticsGrid"); storeGrid0.setData([], false); storeGrid0.proxy.url = HTTP_Logistics + "?DirWarehouseID=" + varDirWarehouseID + "&DirMovementStatusID=77777";
            storeGrid0.load({ waitMsg: lanLoading });
            //2
            var storeGrid2 = Ext.create("store.storeLogisticsGrid"); storeGrid2.setData([], false); storeGrid2.proxy.url = HTTP_Logistics + "?DirWarehouseID=" + varDirWarehouseID + "&DirMovementStatusID=2";
            //storeGrid2.load({ waitMsg: lanLoading });
            //3
            var storeGrid3 = Ext.create("store.storeLogisticsGrid"); storeGrid3.setData([], false); storeGrid3.proxy.url = HTTP_Logistics + "?DirWarehouseID=" + varDirWarehouseID + "&DirMovementStatusID=3";
            //storeGrid3.load({ waitMsg: lanLoading });
            //4
            var storeGrid4 = Ext.create("store.storeLogisticsGrid"); storeGrid4.setData([], false); storeGrid4.proxy.url = HTTP_Logistics + "?DirWarehouseID=" + varDirWarehouseID + "&DirMovementStatusID=4";
            //storeGrid4.load({ waitMsg: lanLoading });


            //Log
            var storeLogLogisticsGrid0 = Ext.create("store.storeLogLogisticsGrid"); storeLogLogisticsGrid0.setData([], false);


            //Tab
            var storeLogisticTabsGrid = Ext.create("store.storeLogisticTabsGrid"); storeLogisticTabsGrid.setData([], false);



            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeGrid0: storeGrid0,
                storeGrid2: storeGrid2,
                storeGrid3: storeGrid3,
                storeGrid4: storeGrid4,

                storeLogLogisticsGrid0: storeLogLogisticsGrid0,
                storeLogisticTabsGrid: storeLogisticTabsGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            //Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            //Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }



            //Оплата (Pay) *** *** ***


        case "viewPay": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storePayGrid"); storeGrid.setData([], false); //storeGrid.load({ waitMsg: lanLoading });
            storeGrid.proxy.url = HTTP_Pays + "?DocID=" + Ext.getCmp("DocID" + Ext.getCmp(UO_idCall).UO_id).getValue();
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Убрать лишние кнопки
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);
            Ext.getCmp("btnHelp" + ObjectID).setVisible(false);
            Ext.getCmp("btnDelete" + ObjectID).setVisible(false);
            Ext.getCmp("TriggerSearchGrid" + ObjectID).setVisible(false);

            break;
        }



            // Сервисный центр

        case "viewDocServicePurches": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocServicePurchesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=1&DirWarehouseID=" + varDirWarehouseID;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }

        case "viewDocServicePurchesSelect": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid

            //DocServicePurches

            //Этого нет на форме!
            var storeGrid0 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid0.setData([], false);
            storeGrid0.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid1 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid1.setData([], false);
            storeGrid1.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=1&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid5 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid5.setData([], false);
            storeGrid5.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=5&DirServiceStatusIDPo=5&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid2 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid2.setData([], false);
            storeGrid2.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=2&DirServiceStatusIDPo=2&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid6 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid6.setData([], false);
            storeGrid6.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=6&DirServiceStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID;
            //Сотрудник
            var storeGridX = Ext.create("store.storeDocServicePurchesGrid"); storeGridX.setData([], false);
            storeGridX.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=0&DirServiceStatusIDPo=0&DirEmployeeID=" + varDirEmployeeID + "&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid3 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid3.setData([], false);
            storeGrid3.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=3&DirServiceStatusIDPo=3&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid4 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid4.setData([], false);
            storeGrid4.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=4&DirServiceStatusIDPo=4&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid7 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid7.setData([], false);
            storeGrid7.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=7&DirServiceStatusIDPo=8&DirWarehouseID=" + varDirWarehouseID;

            // !!!
            var storeGrid9 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid9.setData([], false);
            //storeGrid9.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=9&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID + "&DirServiceContractorID=" + TreeServerParam1 + "&DirServiceNomenID=" + GridServerParam1;
            storeGrid9.proxy.url = HTTP_DocServicePurches + "?DirWarehouseID=" + varDirWarehouseID + "&DirServiceContractorID=" + TreeServerParam1 + "&DirServiceNomenID=" + GridServerParam1;


            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeDocServicePurch1TabsGrid = Ext.create("store.storeDocServicePurch1TabsGrid"); storeDocServicePurch1TabsGrid.setData([], false);
            var storeDocServicePurch2TabsGrid = Ext.create("store.storeDocServicePurch2TabsGrid"); storeDocServicePurch2TabsGrid.setData([], false);

            //Log
            var storeLogServicesGrid0 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid0.setData([], false);
            var storeLogServicesGrid1 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid1.setData([], false);
            var storeLogServicesGrid3 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid3.setData([], false);
            var storeLogServicesGrid4 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid4.setData([], false);
            var storeLogServicesGrid5 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid5.setData([], false);
            var storeLogServicesGrid6 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid6.setData([], false);
            var storeLogServicesGrid8 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid8.setData([], false);
            var storeLogServicesGrid9 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid9.setData([], false);
            var storeLogServicesGrid7 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid7.setData([], false);
            //Panel
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeGrid0: storeGrid0,
                storeGrid1: storeGrid1,
                storeGrid5: storeGrid5,
                storeGrid2: storeGrid2,
                storeGrid6: storeGrid6,
                storeGridX: storeGridX,
                storeGrid3: storeGrid3,
                storeGrid4: storeGrid4,
                storeGrid7: storeGrid7,
                storeGrid9: storeGrid9,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeDocServicePurch1TabsGrid: storeDocServicePurch1TabsGrid,
                storeDocServicePurch2TabsGrid: storeDocServicePurch2TabsGrid,

                storeLogServicesGrid0: storeLogServicesGrid0,
                storeLogServicesGrid1: storeLogServicesGrid1,
                storeLogServicesGrid3: storeLogServicesGrid3,
                storeLogServicesGrid4: storeLogServicesGrid4,
                storeLogServicesGrid5: storeLogServicesGrid5,
                storeLogServicesGrid6: storeLogServicesGrid6,
                storeLogServicesGrid8: storeLogServicesGrid8,
                storeLogServicesGrid9: storeLogServicesGrid9,
                storeLogServicesGrid7: storeLogServicesGrid7,

            });

            
            //Лоадер
            var widgetXXX = Ext.getCmp("viewDocServicePurchesEdit" + UO_idCall);
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetXXX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeGrid9.load({ waitMsg: lanLoading });
            storeGrid9.on('load', function () {
                if (storeGrid9.UO_Loaded) return; //Уже загружали - выйти!
                storeGrid9.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                
                loadingMask.hide();

                
                if (storeGrid9.data.length > 0) {
                    widgetX.border = true;
                    widgetX.center();
                    widgetX.show();
                }

            });


            return;
        }


        case "viewDocServiceWorkshops": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid"; //&WarehouseAll=true

            //DocServicePurches
            var storeGrid0 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid0.setData([], false);
            storeGrid0.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID;
            storeGrid0.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            var storeGrid1 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid1.setData([], false);
            storeGrid1.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=1&DirWarehouseID=" + varDirWarehouseID;
            storeGrid1.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=1&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            var storeGrid5 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid5.setData([], false);
            storeGrid5.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=5&DirServiceStatusIDPo=5&DirWarehouseID=" + varDirWarehouseID;
            storeGrid5.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=5&DirServiceStatusIDPo=5&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            var storeGrid2 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid2.setData([], false);
            storeGrid2.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=2&DirServiceStatusIDPo=2&DirWarehouseID=" + varDirWarehouseID;
            storeGrid2.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=2&DirServiceStatusIDPo=2&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            var storeGrid6 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid6.setData([], false);
            storeGrid6.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=6&DirServiceStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID;
            storeGrid6.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=6&DirServiceStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            //Сотрудник
            var storeGridX = Ext.create("store.storeDocServicePurchesGrid"); storeGridX.setData([], false);
            storeGridX.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=0&DirServiceStatusIDPo=0&DirEmployeeID=" + varDirEmployeeID + "&DirWarehouseID=" + varDirWarehouseID;
            storeGridX.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=0&DirServiceStatusIDPo=0&DirEmployeeID=" + varDirEmployeeID + "&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            var storeGrid3 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid3.setData([], false);
            storeGrid3.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=3&DirServiceStatusIDPo=3&DirWarehouseID=" + varDirWarehouseID;
            storeGrid3.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=3&DirServiceStatusIDPo=3&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            var storeGrid4 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid4.setData([], false);
            storeGrid4.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=4&DirServiceStatusIDPo=4&DirWarehouseID=" + varDirWarehouseID;
            storeGrid4.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=4&DirServiceStatusIDPo=4&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            var storeGrid7 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid7.setData([], false);
            storeGrid7.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=7&DirServiceStatusIDPo=8&DirWarehouseID=" + varDirWarehouseID;
            storeGrid7.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=7&DirServiceStatusIDPo=8&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;

            var storeGrid9 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid9.setData([], false);
            storeGrid9.UO_Proxy_Url = HTTP_DocServicePurches + "?DirServiceStatusIDS=9&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID;
            storeGrid9.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=9&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID + "&DirWarehouseIDOnly=" + varDirWarehouseID;


            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeDocServicePurch1TabsGrid = Ext.create("store.storeDocServicePurch1TabsGrid"); storeDocServicePurch1TabsGrid.setData([], false);
            var storeDocServicePurch2TabsGrid = Ext.create("store.storeDocServicePurch2TabsGrid"); storeDocServicePurch2TabsGrid.setData([], false);

            //Log
            var storeLogServicesGrid0 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid0.setData([], false);
            var storeLogServicesGrid1 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid1.setData([], false);
            var storeLogServicesGrid3 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid3.setData([], false);
            var storeLogServicesGrid4 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid4.setData([], false);
            var storeLogServicesGrid5 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid5.setData([], false);
            var storeLogServicesGrid6 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid6.setData([], false);
            var storeLogServicesGrid8 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid8.setData([], false);
            var storeLogServicesGrid9 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid9.setData([], false);
            var storeLogServicesGrid7 = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid7.setData([], false);


            //Panel
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeDirWarehousesGrid: storeDirWarehousesGrid,

                storeGrid0: storeGrid0,
                storeGrid1: storeGrid1,
                storeGrid5: storeGrid5,
                storeGrid2: storeGrid2,
                storeGrid6: storeGrid6,
                storeGridX: storeGridX,
                storeGrid3: storeGrid3,
                storeGrid4: storeGrid4,
                storeGrid7: storeGrid7,
                storeGrid9: storeGrid9,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeDocServicePurch1TabsGrid: storeDocServicePurch1TabsGrid,
                storeDocServicePurch2TabsGrid: storeDocServicePurch2TabsGrid,

                storeLogServicesGrid0: storeLogServicesGrid0,
                storeLogServicesGrid1: storeLogServicesGrid1,
                storeLogServicesGrid3: storeLogServicesGrid3,
                storeLogServicesGrid4: storeLogServicesGrid4,
                storeLogServicesGrid5: storeLogServicesGrid5,
                storeLogServicesGrid6: storeLogServicesGrid6,
                storeLogServicesGrid8: storeLogServicesGrid8,
                storeLogServicesGrid9: storeLogServicesGrid9,
                storeLogServicesGrid7: storeLogServicesGrid7,
            });

            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirWarehousesGrid.load({ waitMsg: lanLoading });
            storeDirWarehousesGrid.on('load', function () {
                if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                var rec = { DirWarehouseID: 0, Del: false, DirWarehouseName: "Все" }; storeDirWarehousesGrid.insert(0, rec);

                Ext.getCmp("DirWarehouseIDPanelGrid0_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGrid1_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGrid5_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGrid2_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGrid6_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGridX_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGrid3_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGrid4_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGrid7_" + ObjectID).setValue(varDirWarehouseID);
                Ext.getCmp("DirWarehouseIDPanelGrid9_" + ObjectID).setValue(varDirWarehouseID);

                storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                storeDirEmployeesGrid.on('load', function () {

                    //Переключение на вкладку "В ремонте"
                    Ext.getCmp("tab_" + ObjectID).setActiveTab(Ext.getCmp("PanelGrid2_" + ObjectID));

                    storeGrid2.load({ waitMsg: lanLoading });
                    storeGrid2.on('load', function () {

                        loadingMask.hide();

                        if (!varRightDocServiceWorkshopsTab1AddCheck) {
                            Ext.getCmp("btnGridAddPosition11" + ObjectID).disable(true);
                        }

                    });
                });

            });


            return;
        }

        case "viewDocServiceWorkshopHistories": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid

            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeDocServicePurch1TabsGrid = Ext.create("store.storeDocServicePurch1TabsGrid"); storeDocServicePurch1TabsGrid.setData([], false);
            var storeDocServicePurch2TabsGrid = Ext.create("store.storeDocServicePurch2TabsGrid"); storeDocServicePurch2TabsGrid.setData([], false);
            var storeLogServicesGrid = Ext.create("store.storeLogServicesGrid"); storeLogServicesGrid.setData([], false);

            var storeGrid0 = Ext.create("store.storeDocServicePurchesGrid"); storeGrid0.setData([], false);
            storeGrid0.proxy.url = HTTP_DocServicePurches + "?" + GridServerParam1; //"?DirServiceStatusIDS=1&DirServiceStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID;

            //Panel
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeDocServicePurch1TabsGrid: storeDocServicePurch1TabsGrid,
                storeDocServicePurch2TabsGrid: storeDocServicePurch2TabsGrid,
                storeLogServicesGrid: storeLogServicesGrid,

                storeGrid0: storeGrid0,
            });

            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirEmployeesGrid.load({ waitMsg: lanLoading });
            storeDirEmployeesGrid.on('load', function () {

                storeGrid0.load({ waitMsg: lanLoading });
                storeGrid0.on('load', function () {

                    loadingMask.hide();

                });
            });


            return;
        }

        case "viewDocServiceOutputs": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocServicePurchesGrid"); storeGrid.setData([], false); //storeGrid.load({ waitMsg: lanLoading });
            storeGrid.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=7&DirServiceStatusIDPo=8&DirWarehouseID=" + varDirWarehouseID;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);
            //Прячем кнопки
            Ext.getCmp("btnNew" + ObjectID).setVisible(false);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);
            Ext.getCmp("btnDelete" + ObjectID).setVisible(false);

            break;
        }

        case "viewDocServiceArchives": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocServicePurchesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocServicePurches + "?DirServiceStatusIDS=9&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);
            //Прячем кнопки
            Ext.getCmp("btnNew" + ObjectID).setVisible(false);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);
            Ext.getCmp("btnDelete" + ObjectID).setVisible(false);

            break;
        }

        case "viewDocServiceMovs": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            var param1;
            if (ArrList != undefined && ArrList.length > 0) param1 = ArrList[0];

            //Store Grid
            var storeGrid = Ext.create("store.storeDocServiceMovsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocServiceMovs + "?DirWarehouseID=" + varDirWarehouseID + "&" + param1; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            //Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            //Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }

        case "viewDocServiceInvs": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocServiceInvsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocServiceInvs + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }



            // БУ

        case "viewDocSecondHandWorkshops": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid

            //DocSecondHandPurches
            var storeGrid0 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid0.setData([], false);
            storeGrid0.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID;

            var storeGrid1 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid1.setData([], false);
            storeGrid1.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=1&DirWarehouseID=" + varDirWarehouseID;

            var storeGrid2 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid2.setData([], false);
            storeGrid2.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=2&DirSecondHandStatusIDPo=4&DirWarehouseID=" + varDirWarehouseID;

            var storeGrid5 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid5.setData([], false);
            storeGrid5.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=5&DirSecondHandStatusIDPo=5&DirWarehouseID=" + varDirWarehouseID;

            var storeGrid7 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid7.setData([], false);
            storeGrid7.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=7&DirSecondHandStatusIDPo=7&DirWarehouseID=" + varDirWarehouseID;

            var storeGrid8 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid8.setData([], false);
            storeGrid8.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=8&DirSecondHandStatusIDPo=8&DirWarehouseID=" +varDirWarehouseID;

            var storeGrid9 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid9.setData([], false);
            storeGrid9.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=9&DirSecondHandStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID;


            /*
            var storeGrid6 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid6.setData([], false);
            storeGrid6.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=6&DirSecondHandStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID;

            //Сотрудник
            var storeGridX = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGridX.setData([], false);
            storeGridX.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=0&DirSecondHandStatusIDPo=0&DirEmployeeID=" + varDirEmployeeID + "&DirWarehouseID=" + varDirWarehouseID;

            var storeGrid3 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid3.setData([], false);
            storeGrid3.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=3&DirSecondHandStatusIDPo=3&DirWarehouseID=" + varDirWarehouseID;

            var storeGrid4 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid4.setData([], false);
            storeGrid4.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=4&DirSecondHandStatusIDPo=4&DirWarehouseID=" + varDirWarehouseID;
            */



            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeDocSecondHandPurch1TabsGrid = Ext.create("store.storeDocSecondHandPurch1TabsGrid"); storeDocSecondHandPurch1TabsGrid.setData([], false);
            var storeDocSecondHandPurch2TabsGrid = Ext.create("store.storeDocSecondHandPurch2TabsGrid"); storeDocSecondHandPurch2TabsGrid.setData([], false);

            //Log
            var storeLogSecondHandsGrid0 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid0.setData([], false);
            var storeLogSecondHandsGrid1 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid1.setData([], false);
            var storeLogSecondHandsGrid3 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid3.setData([], false);
            var storeLogSecondHandsGrid4 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid4.setData([], false);
            var storeLogSecondHandsGrid5 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid5.setData([], false);
            var storeLogSecondHandsGrid6 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid6.setData([], false);
            var storeLogSecondHandsGrid8 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid8.setData([], false);
            var storeLogSecondHandsGrid9 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid9.setData([], false);
            var storeLogSecondHandsGrid7 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid7.setData([], false);


            //Panel
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeGrid0: storeGrid0,
                storeGrid1: storeGrid1,
                storeGrid2: storeGrid2,
                storeGrid5: storeGrid5,
                //storeGrid6: storeGrid6,
                //storeGridX: storeGridX,
                //storeGrid3: storeGrid3,
                //storeGrid4: storeGrid4,
                storeGrid7: storeGrid7,
                storeGrid8: storeGrid8,
                storeGrid9: storeGrid9,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeDocSecondHandPurch1TabsGrid: storeDocSecondHandPurch1TabsGrid,
                storeDocSecondHandPurch2TabsGrid: storeDocSecondHandPurch2TabsGrid,

                storeLogSecondHandsGrid0: storeLogSecondHandsGrid0,
                storeLogSecondHandsGrid1: storeLogSecondHandsGrid1,
                storeLogSecondHandsGrid3: storeLogSecondHandsGrid3,
                storeLogSecondHandsGrid4: storeLogSecondHandsGrid4,
                storeLogSecondHandsGrid5: storeLogSecondHandsGrid5,
                storeLogSecondHandsGrid6: storeLogSecondHandsGrid6,
                storeLogSecondHandsGrid8: storeLogSecondHandsGrid8,
                storeLogSecondHandsGrid9: storeLogSecondHandsGrid9,
                storeLogSecondHandsGrid7: storeLogSecondHandsGrid7,
            });

            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirEmployeesGrid.load({ waitMsg: lanLoading });
            storeDirEmployeesGrid.on('load', function () {

                storeGrid0.load({ waitMsg: lanLoading });
                storeGrid0.on('load', function () {

                    loadingMask.hide();

                });
            });


            return;
        }

            // БУ.Разбор
        /*
        case "viewDocSecondHandRazbors": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Tree
            var storeTree = Ext.create("store.storeDirNomensTree"); storeTree.setData([], false); storeTree.proxy.url = HTTP_DirNomens + "?type=Tree";

            //DocSecondHandRazbors
            var storeGrid0 = Ext.create("store.storeDocSecondHandRazborsGrid"); storeGrid0.setData([], false);
            storeGrid0.proxy.url = HTTP_DocSecondHandRazbors + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=6&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid7 = Ext.create("store.storeDocSecondHandRazborsGrid"); storeGrid7.setData([], false);
            storeGrid7.proxy.url = HTTP_DocSecondHandRazbors + "?DirSecondHandStatusIDS=7&DirSecondHandStatusIDPo=7&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid9 = Ext.create("store.storeDocSecondHandRazborsGrid"); storeGrid9.setData([], false);
            storeGrid9.proxy.url = HTTP_DocSecondHandRazbors + "?DirSecondHandStatusIDS=9&DirSecondHandStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID;


            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeGrid = Ext.create("store.storeDocSecondHandRazborTabsGrid"); storeGrid.setData([], false);

            //Log
            var storeLogSecondHandsGrid0 = Ext.create("store.storeLogSecondHandRazborsGrid"); storeLogSecondHandsGrid0.setData([], false);
            

            //Panel
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeTree: storeTree,

                storeGrid0: storeGrid0,
                storeGrid7: storeGrid7,
                storeGrid9: storeGrid9,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeGrid: storeGrid,

                storeLogSecondHandsGrid0: storeLogSecondHandsGrid0,
            });

            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirEmployeesGrid.load({ waitMsg: lanLoading });
            storeDirEmployeesGrid.on('load', function () {

                storeGrid0.load({ waitMsg: lanLoading });
                storeGrid0.on('load', function () {

                    loadingMask.hide();

                });
            });


            //Спрятать список товара
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);


            return;
        }
        */

        case "viewDocSecondHandRazbors": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Tree
            var storeTree = Ext.create("store.storeDirNomensTree"); storeTree.setData([], false); storeTree.proxy.url = HTTP_DirNomens + "?type=Tree";
            
            //DocSecondHandRazbors
            var storeGrid0 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid0.setData([], false);
            storeGrid0.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=12&DirSecondHandStatusIDPo=12&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid7 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid7.setData([], false);
            storeGrid7.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=13&DirSecondHandStatusIDPo=13&DirWarehouseID=" + varDirWarehouseID;
            var storeGrid9 = Ext.create("store.storeDocSecondHandPurchesGrid"); storeGrid9.setData([], false);
            storeGrid9.proxy.url = HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=12&DirSecondHandStatusIDPo=14&DirWarehouseID=" + varDirWarehouseID;


            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeGrid = Ext.create("store.storeDocSecondHandRazbor2TabsGrid"); storeGrid.setData([], false);

            //Log
            var storeLogSecondHandsGrid0 = Ext.create("store.storeLogSecondHandsGrid"); storeLogSecondHandsGrid0.setData([], false);


            //Panel
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,
                modal: UO_Modal,

                storeTree: storeTree,

                storeGrid0: storeGrid0,
                storeGrid7: storeGrid7,
                storeGrid9: storeGrid9,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeGrid: storeGrid,

                storeLogSecondHandsGrid0: storeLogSecondHandsGrid0,
            });

            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirEmployeesGrid.load({ waitMsg: lanLoading });
            storeDirEmployeesGrid.on('load', function () {

                storeGrid0.load({ waitMsg: lanLoading });
                storeGrid0.on('load', function () {

                    loadingMask.hide();

                });
            });


            //Спрятать список товара
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);


            return;
        }


            // Финансы.Перемещение

        case "viewDocCashOfficeSumMovements": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            var param1;
            if (ArrList != undefined && ArrList.length > 0) param1 = ArrList[0];

            //Store Grid
            var storeGrid = Ext.create("store.storeDocCashOfficeSumMovementsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocCashOfficeSumMovements + "?DirWarehouseID=" + varDirWarehouseID + "&" + param1; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            //Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            //Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


            // Заказы

        case "viewDocOrderInts": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid

            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeDocServicePurch1TabsGrid = Ext.create("store.storeDocServicePurch1TabsGrid"); storeDocServicePurch1TabsGrid.setData([], false); //storeDocServicePurch1TabsGrid.proxy.url = HTTP_DocServicePurch1Tabs + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
            var storeDocServicePurch2TabsGrid = Ext.create("store.storeDocServicePurch2TabsGrid"); storeDocServicePurch2TabsGrid.setData([], false); //storeDocServicePurch2TabsGrid.proxy.url = HTTP_DocServicePurch2Tabs + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
            var storeLogOrderIntsGrid = Ext.create("store.storeLogOrderIntsGrid"); storeLogOrderIntsGrid.setData([], false); //storeLogOrderIntsGrid.proxy.url = HTTP_LogOrderInts + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;

            //DirOrderIntStatusID => DocOrderIntType
            //Все
            var storeGrid0 = Ext.create("store.storeDocOrderIntsGrid"); storeGrid0.setData([], false);
            storeGrid0.proxy.url = HTTP_DocOrderInts + "?DocOrderIntTypeS=1&DocOrderIntTypePo=4&DirWarehouseID=" + varDirWarehouseID;
            //Мастерская
            var storeGrid1 = Ext.create("store.storeDocOrderIntsGrid"); storeGrid1.setData([], false);
            storeGrid1.proxy.url = HTTP_DocOrderInts + "?DocOrderIntTypeS=1&DocOrderIntTypePo=1&DirWarehouseID=" + varDirWarehouseID;
            //Предзаказы
            var storeGrid2 = Ext.create("store.storeDocOrderIntsGrid"); storeGrid2.setData([], false);
            storeGrid2.proxy.url = HTTP_DocOrderInts + "?DocOrderIntTypeS=2&DocOrderIntTypePo=2&DirWarehouseID=" + varDirWarehouseID;
            //Впрок
            var storeGrid3 = Ext.create("store.storeDocOrderIntsGrid"); storeGrid3.setData([], false);
            storeGrid3.proxy.url = HTTP_DocOrderInts + "?DocOrderIntTypeS=3&DocOrderIntTypePo=3&DirWarehouseID=" + varDirWarehouseID;
            //Впрок
            var storeGrid4 = Ext.create("store.storeDocOrderIntsGrid"); storeGrid4.setData([], false);
            storeGrid4.proxy.url = HTTP_DocOrderInts + "?DocOrderIntTypeS=4&DocOrderIntTypePo=4&DirWarehouseID=" + varDirWarehouseID;
            //Архив
            var storeGrid9 = Ext.create("store.storeDocOrderIntsGrid"); storeGrid9.setData([], false);
            storeGrid9.proxy.url = HTTP_DocOrderInts + "?DocOrderIntTypeS=1&DocOrderIntTypePo=9&DirOrderIntStatusIDS=1&DirOrderIntStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID;


            //Panel
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeDocServicePurch1TabsGrid: storeDocServicePurch1TabsGrid,
                storeDocServicePurch2TabsGrid: storeDocServicePurch2TabsGrid,
                storeLogOrderIntsGrid: storeLogOrderIntsGrid,

                storeGrid0: storeGrid0,
                storeGrid1: storeGrid1,
                storeGrid2: storeGrid2,
                storeGrid3: storeGrid3,
                storeGrid4: storeGrid4,
                storeGrid9: storeGrid9,
            });

            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirEmployeesGrid.load({ waitMsg: lanLoading });
            storeDirEmployeesGrid.on('load', function () {

                storeGrid0.load({ waitMsg: lanLoading });
                storeGrid0.on('load', function () {

                    loadingMask.hide();

                });
            });


            return;
        }


            //БУ
            
        case "viewDocSecondHandMovements_OLD": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            var param1;
            if (ArrList != undefined && ArrList.length > 0) param1 = ArrList[0];

            //Store Grid
            var storeGrid = Ext.create("store.storeDocSecondHandMovementsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocSecondHandMovements + "?DirWarehouseID=" + varDirWarehouseID + "&" + param1; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            //Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            //Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }

        case "viewDocSecondHandMovs": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            var param1;
            if (ArrList != undefined && ArrList.length > 0) param1 = ArrList[0];

            //Store Grid
            var storeGrid = Ext.create("store.storeDocSecondHandMovsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocSecondHandMovs + "?DirWarehouseID=" + varDirWarehouseID + "&" + param1; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            //Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            //Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }

        case "viewDocSecondHandInventories_OLD": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocSecondHandInventoriesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocSecondHandInventories + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }

        case "viewDocSecondHandInvs": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocSecondHandInvsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocSecondHandInvs + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);
            Ext.getCmp("btnNewCopy" + ObjectID).setVisible(false);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


            // Заказы

        case "viewDirDomesticExpenses": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            // === Формируем и показываем окно ===

            //Store Grid
            var storeGrid = Ext.create("store.storeDirDomesticExpensesTree"); storeGrid.setData([], false);
            storeGrid.proxy.url = HTTP_DirDomesticExpenses + "?type=Tree";
            //Если есть параметр "TreeServerParam1", то изменить URL
            if (TreeServerParam1 != undefined) storeGrid.proxy.url = HTTP_DirDomesticExpenses + "?type=Tree" + "&" + TreeServerParam1;
            
            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,
                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                storeGrid: storeGrid,
                //storeDirCashOfficesGrid: storeDirCashOfficesGrid,
            });
            ObjectShow(widgetX);

            Ext.getCmp("Sign" + ObjectID).setValue(-1);

            //Блокируем кнопки для Tree
            Ext.getCmp("expandAll" + ObjectID).disable();
            Ext.getCmp("collapseAll" + ObjectID).disable();
            Ext.getCmp("FolderNewSub" + ObjectID).disable();

            return;
        }



            // Log

        case "viewLogServices": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeLogServicesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_LogServices + "?" + GridServerParam1;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            Ext.getCmp("DocServicePurchID" + ObjectID).setValue(Ext.getCmp("DocServicePurchID" + Ext.getCmp(UO_idCall).UO_id).getValue());
            //Комментарии и заметки
            Ext.getCmp("DirServiceLogTypeID" + ObjectID).setValue(3);

            break;
        }

        case "viewLogOrderInts": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeLogOrderIntsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_LogOrderInts + "?" + GridServerParam1;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            Ext.getCmp("DocOrderIntID" + ObjectID).setValue(Ext.getCmp("DocOrderIntID" + Ext.getCmp(UO_idCall).UO_id).getValue());
            //Комментарии и заметки
            Ext.getCmp("DirOrderIntLogTypeID" + ObjectID).setValue(2);

            break;
        }
            //viewLogSecondHands
        case "viewLogSecondHands": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeLogSecondHandsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_LogSecondHands + "?" +GridServerParam1;
            storeGrid.load({
                waitMsg: lanLoading
            });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            Ext.getCmp("DocSecondHandPurchID" +ObjectID).setValue(Ext.getCmp("DocSecondHandPurchID" +Ext.getCmp(UO_idCall).UO_id).getValue());
            //Комментарии и заметки
            Ext.getCmp("DirSecondHandLogTypeID" +ObjectID).setValue(3);

            break;
        }



            // Sms

        case "viewSms": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDirSmsTemplatesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DirSmsTemplates + "?type=Grid&" + GridServerParam1;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });


            if (Ext.getCmp("DocServicePurchID" + Ext.getCmp(UO_idCall).UO_id)) {
                Ext.getCmp("DocServicePurchID" + ObjectID).setValue(Ext.getCmp("DocServicePurchID" + Ext.getCmp(UO_idCall).UO_id).getValue());
            }
            else if (Ext.getCmp("DocMovementID" + Ext.getCmp(UO_idCall).UO_id)) {
                Ext.getCmp("DocMovementID" + ObjectID).setValue(Ext.getCmp("DocMovementID" + Ext.getCmp(UO_idCall).UO_id).getValue());
                Ext.getCmp("DirWarehouseNameFrom" + ObjectID).setValue(Ext.getCmp("DirWarehouseIDFrom" + Ext.getCmp(UO_idCall).UO_id).rawValue);
            }
            /*else if (Ext.getCmp("DocSecondHandMovementID" + Ext.getCmp(UO_idCall).UO_id)) {
                Ext.getCmp("DocSecondHandMovementID" + ObjectID).setValue(Ext.getCmp("DocSecondHandMovementID" + Ext.getCmp(UO_idCall).UO_id).getValue());
                Ext.getCmp("DirWarehouseNameFrom" + ObjectID).setValue(Ext.getCmp("DirWarehouseIDFrom" + Ext.getCmp(UO_idCall).UO_id).rawValue);
            }*/
            else if (Ext.getCmp("DocSecondHandMovID" + Ext.getCmp(UO_idCall).UO_id)) {
                Ext.getCmp("DocSecondHandMovID" + ObjectID).setValue(Ext.getCmp("DocSecondHandMovID" + Ext.getCmp(UO_idCall).UO_id).getValue());
                Ext.getCmp("DirWarehouseNameFrom" + ObjectID).setValue(Ext.getCmp("DirWarehouseIDFrom" + Ext.getCmp(UO_idCall).UO_id).rawValue);
            }
            else if (Ext.getCmp("DocOrderIntID" + Ext.getCmp(UO_idCall).UO_id)) {
                Ext.getCmp("DocMovementID" + ObjectID).setValue(Ext.getCmp("DocOrderIntID" + Ext.getCmp(UO_idCall).UO_id).getValue());
                //Ext.getCmp("DirWarehouseNameTo" + ObjectID).setValue(Ext.getCmp("DirWarehouseIDTo" + Ext.getCmp(UO_idCall).UO_id).rawValue);
            }

            break;
        }



            //Список ПФ для Документов *** *** ***


        case "viewListObjectPFs": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            if (UO_idTab == undefined) UO_idTab = 0;
            var storeGrid = Ext.create("store.storeListObjectPFsGrid"); storeGrid.setData([], false); //storeGrid.load({ waitMsg: lanLoading });
            storeGrid.proxy.url = HTTP_ListObjectPFs + "?type=Tree&ListObjectID=" + GridServerParam1 + "&ListObjectPFID=" + UO_idTab;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                GridServerParam1: GridServerParam1, //ListObjectID - Тип документа (отображить ПФ только этого типа документа)
                TreeServerParam1: TreeServerParam1, //DocID - ID-шник документа (при клике на ПФ передаётся на сервер)
                UO_Function_Tree: UO_Function_Tree, //Html или Excel

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            break;
        }




            //Retail *** *** ***


        case "viewDocRetails": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocRetailsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocRetails + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocRetailReturns": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocRetailReturnsGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocRetailReturns + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }


        case "viewDocDomesticExpenses": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectConfig(UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDocDomesticExpensesGrid"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DocDomesticExpenses + "?DirWarehouseID=" + varDirWarehouseID; storeGrid.pageSize = varPageSizeJurn;
            storeGrid.load({ waitMsg: lanLoading });

            //Panel
            widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_View: pObjectName,
                UO_idCall: UO_idCall, //"viewPanelMain",
                UO_Center: UO_Center,

                UO_Function_Tree: UO_Function_Tree,
                UO_Function_Grid: UO_Function_Grid,

                modal: UO_Modal,
                //storeGroup: storeGroup,
                storeGrid: storeGrid,
            });

            //Фильтр: ДатаС и ДатаПо
            Ext.getCmp("DateS" + ObjectID).setValue(varJurDateS);
            Ext.getCmp("DatePo" + ObjectID).setValue(varJurDatePo);

            //Прячем левый фильтр: "tree_"
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }



        default:
            Ext.Msg.alert("ObjectConfig", "Object '" + pObjectName + "' not found!");
            return;
    }



    //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
    //ObjectEditConfig_UO_idCall_true_false(UO_idCall, false);


    //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
    if (ContainerWidget == undefined) ObjectShow(widgetX);
        //Или в Панель, например в журналах "Подбор товара"
    else ObjectShow_NotCentral(widgetX, ContainerWidget);


    /*
    } catch (ex) {
        var exMsg = ex;
        if (exMsg.message != undefined) exMsg = ex.message;

        Ext.Msg.alert(lanOrgName, "Ошибка в скрипте! Вышлите, пожалуйста скриншот на: support@uchetoblako.ru<br />Подробности:" + exMsg);

        //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined) { Ext.getCmp(UO_idCall).enable(); }
    }
    */
}


//Блокировать или Разблокировать вызвавший элемент "UO_idCall"
function ObjectEditConfig_UO_idCall_true_false(UO_idCall, Disable) {
    if (Disable) {
        //Блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined) { Ext.getCmp(UO_idCall).disable(); }
    }
    else {
        //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined) { Ext.getCmp(UO_idCall).enable(); }
    }
}

