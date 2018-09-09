//http://javascriptcompressor.com/
//Маршрутизатор на View-грид
//Параметры:
//  pObjectName - Наименование обекта: Справочник.Товары, Документ.[Приходные накладные]
//  Params      - массив дополнительных параметров:
//   - UO_idCall        - ID-и Вьюхи, которая вызвала
//   - UO_Center        - Разместить в центре экрана
//   - UO_Modal         - Все остальные элементы не активные
//   - New_Edit         - 1 - Новое, 2 - Редактировать, 3 - Копия, 0 - Подбор товара -> Количество (viewDirNomensQuantity)
//   - UO_GridSave      - true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
//   - UO_GridIndex     - Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
//   - UO_GridRecord    - Сожержит данные с Грида для загрузки данных в форму Б.С. и Договора

//Используется для подбора товара
//   UO_Param_id      - id - шник который передается в контрол документа, для записи в спецификацию
// (не используется)  - UO_Param_rec  - аналог "UO_GridRecord"      - rec - запись с информацией по выбраному Товару.
//   UO_Param_fn      - fn - функция, которую надо запустить в контроллере документа
//   UO_idTab         - id-шник Спецификации (Табличной части) документа, что бы туда вставить выбранный Товар (с левой панели)

//Используется для подбора товара
//   UO_GridServerParam1   - Передаём параметр. Например: Приходная накладная открывает форму ред. спецификации и передаёт "DirWarehouseID", для отображение остатков в справочнике Товаров
//   UO_FunRecalcSum       - Функция пересчета сумм, для каждого докумена своя

//Массив
//   - ArrList          - Создать "На основании ..."
//                        Массив "Params[12]" в которм содержатся параметры индивидуально для каждой Вьюхи: ArrList = [Data1, Data2, ...]

function ObjectEditConfig(pObjectName, Params) {

    //Если окно открыто, то устанавливаем на него фокус
    if (funSearchWin(pObjectName, true)) return;

    //Параметры
    var UO_idCall = Params[0];                                                    // ID-к Вьюхи, которая вызвала
    var UO_Center = Params[1]; if (UO_Center == undefined) UO_Center = false;     // Разместить в центре экрана
    var UO_Modal = Params[2]; if (UO_Modal == undefined) UO_Modal = false;        // Все остальные элементы не активные
    var New_Edit = Params[3];                                                     // 1 - Новое, 2 - Редактировать, 3 - Копия
    var UO_GridSave = Params[4];                                                  // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
    var UO_GridIndex = Params[5];                                                 // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
    var UO_GridRecord = Params[6];                                                // Сожержит данные с Грида для загрузки данных в форму Б.С. и Договора
    //Используется для подбора товара
    var UO_Param_id = Params[7];                                                  // id - шник который передается в контрол документа, для записи в спецификацию
    //var UO_Param_rec = Params[8];  - аналог "UO_GridRecord"                     // rec - запись с информацией по выбраному Товару.
    var UO_Param_fn = Params[8];                                                  // fn - функция, которую надо запустить в контроллере документа
    var UO_idTab = Params[9];                                                     // id-шник Спецификации (Табличной части) документа, что бы туда вставить выбранный Товар (с левой панели)
    //Добавить позицию и Подбор товара
    var UO_GridServerParam1 = Params[10];                                         // Передаём параметр. Например: Приходная накладная открывает форму ред. спецификации и передаёт "DirWarehouseID"
    var UO_FunRecalcSum = Params[11];                                             // Функция пересчета сумм, для каждого докумена своя
    var ArrList = Params[12];                                                     // Массив "Params[12]" в которм содержатся параметры индивидуально для каждой Вьюхи
    var GridTree = Params[13];                                                    // Признак: true - редактирование данных с Грида, false - редактирование данных с Дерева


    //Для id
    ObjectID++;


    //try {


    //Блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, true);



    switch (pObjectName) {

        //Настройки *** *** ***

        case "viewSysSettingsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1"; storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            
            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirContractorsOrgGrid.on('load', function () {
                if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
                storeDirCurrenciesGrid.on('load', function () {
                    if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirVatsGrid.load({ waitMsg: lanLoading });
                        storeDirVatsGrid.on('load', function () {
                            if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            loadingMask.hide();


                            //Загрузка данных в Форму "widgetXPanel"
                            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                            if (!UO_GridSave) {

                                //Если форма уже загружена выходим!
                                if (widgetXForm.UO_Loaded) return;

                                widgetXForm.load({
                                    method: "GET",
                                    timeout: varTimeOutDefault,
                                    waitMsg: lanLoading,
                                    url: HTTP_SysSettings + "/1/",
                                    success: function (form, action) {
                                        widgetXForm.UO_Loaded = true;

                                        //Разблокировка вызвавшего окна
                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();
                                    },
                                    failure: function (form, action) {
                                        widgetX.close(); funPanelSubmitFailure(form, action);

                                        //Разблокировка вызвавшего окна
                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();
                                    }
                                });
                            }
                            else {
                                var form = widgetXForm.getForm();
                                form.loadRecord(UO_GridRecord);

                                //Разблокировка вызвавшего окна
                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                //Фокус на открывшийся Виджет
                                widgetX.focus();
                            }

                        });
                    });
                });
            });


            break;
        }


        case "viewImportsDocPurchesExcel": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=2&DirContractor2TypeID2=4";
            //var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                //storeDirWarehousesGrid: storeDirWarehousesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            //Ext.getCmp("sheetName" + ObjectID).setReadOnly("Лист1");


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
            storeDirContractorsOrgGrid.on('load', function () {
                if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsGrid.load({ waitMsg: lanLoading });
                storeDirContractorsGrid.on('load', function () {
                    if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    /*
                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                        */

                    loadingMask.hide();

                    //Организация
                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    //});
                });
            });

            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }


            //Справочники *** *** ***

            /* Товар */

        case "viewDirNomensWinEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName; // + "_" + IdcallModelData.id;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }


            //Store ComboGrid "DirNomens"
            var storeDirNomenTypesGrid = Ext.create("store.storeDirNomenTypesGrid"); storeDirNomenTypesGrid.setData([], false);
            storeDirNomenTypesGrid.proxy.url = HTTP_DirNomenTypes + "?type=Grid";
            storeDirNomenTypesGrid.load({ waitMsg: lanLoading });

            //Store ComboGrid "DirBonuses"
            var storeDirNomenCategoriesGrid = Ext.create("store.storeDirNomenCategoriesGrid"); storeDirNomenCategoriesGrid.setData([], false);
            storeDirNomenCategoriesGrid.proxy.url = HTTP_DirNomenCategories + "?type=Grid";
            //storeDirNomenCategoriesGrid.load({ waitMsg: lanLoading });


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirNomenTypesGrid: storeDirNomenTypesGrid,
                storeDirNomenCategoriesGrid: storeDirNomenCategoriesGrid,
            });

            //Если создаём подкатегорию, то надо указать "Sub"
            if (UO_GridIndex != undefined) { Ext.getCmp("Sub" + ObjectID).setValue(UO_GridIndex); }

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //При наведении на "Ext.Img" сделать курсор в виде руки (pointer)
            Ext.getCmp("imageShow" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image1Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image2Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image3Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image4Show" + ObjectID).setStyle("cursor", "pointer");
            //Ext.getCmp("image5Show" + ObjectID).setStyle("cursor", "pointer");



            //Событие на загрузку в Grid
            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirNomenTypesGrid.on('load', function () {
                if (storeDirNomenTypesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirNomenTypesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirNomenCategoriesGrid.load({ waitMsg: lanLoading });
                storeDirNomenCategoriesGrid.on('load', function () {
                    if (storeDirNomenCategoriesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirNomenCategoriesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    loadingMask.hide();

                    Ext.getCmp("DirNomenTypeID" + ObjectID).setValue(1);

                    if (New_Edit == 1) {

                        //Артикул (он же код товара) делаем редактируемым
                        Ext.getCmp("DirNomenID_INSERT" + ObjectID).setReadOnly(false);
                        //ККМ.Налог
                        Ext.getCmp("KKMSTax" + ObjectID).setValue(0);

                        //Фокус на открывшийся Виджет
                        widgetX.focus();

                        //Разблокировка вызвавшего окна
                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    }
                    else if (New_Edit == 2 || New_Edit == 3) {

                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                        //Если форма уже загружена выходим!
                        if (widgetXForm.UO_Loaded) return;

                        widgetXForm.load({
                            method: "GET",
                            timeout: varTimeOutDefault,
                            waitMsg: lanLoading,
                            url: HTTP_DirNomens + IdcallModelData.id + "/", //rec.get('id') + "/",
                            success: function (form, action) {
                                widgetXForm.UO_Loaded = true;

                                //Image
                                //0.
                                Ext.getCmp("SysGenID" + ObjectID).setValue(action.result.data.SysGenID);
                                Ext.getCmp("SysGenIDPatch" + ObjectID).setValue(action.result.data.SysGenIDPatch);
                                if (action.result.data.SysGenIDPatch != "") { Ext.getCmp("imageShow" + ObjectID).setSrc(action.result.data.SysGenIDPatch); }
                                else { Ext.getCmp("imageShow" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                //1.
                                Ext.getCmp("SysGen1ID" + ObjectID).setValue(action.result.data.SysGen1ID);
                                Ext.getCmp("SysGen1IDPatch" + ObjectID).setValue(action.result.data.SysGen1IDPatch);
                                if (action.result.data.SysGen1IDPatch != "") { Ext.getCmp("image1Show" + ObjectID).setSrc(action.result.data.SysGen1IDPatch); }
                                else { Ext.getCmp("image1Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                //2.
                                Ext.getCmp("SysGen2ID" + ObjectID).setValue(action.result.data.SysGen2ID);
                                Ext.getCmp("SysGen2IDPatch" + ObjectID).setValue(action.result.data.SysGen2IDPatch);
                                if (action.result.data.SysGen2IDPatch != "") { Ext.getCmp("image2Show" + ObjectID).setSrc(action.result.data.SysGen2IDPatch); }
                                else { Ext.getCmp("image2Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                //3.
                                Ext.getCmp("SysGen3ID" + ObjectID).setValue(action.result.data.SysGen3ID);
                                Ext.getCmp("SysGen3IDPatch" + ObjectID).setValue(action.result.data.SysGen3IDPatch);
                                if (action.result.data.SysGen3IDPatch != "") { Ext.getCmp("image3Show" + ObjectID).setSrc(action.result.data.SysGen3IDPatch); }
                                else { Ext.getCmp("image3Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                //4.
                                Ext.getCmp("SysGen4ID" + ObjectID).setValue(action.result.data.SysGen4ID);
                                Ext.getCmp("SysGen4IDPatch" + ObjectID).setValue(action.result.data.SysGen4IDPatch);
                                if (action.result.data.SysGen4IDPatch != "") { Ext.getCmp("image4Show" + ObjectID).setSrc(action.result.data.SysGen4IDPatch); }
                                else { Ext.getCmp("image4Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                //5.
                                Ext.getCmp("SysGen5ID" + ObjectID).setValue(action.result.data.SysGen5ID);
                                Ext.getCmp("SysGen5IDPatch" + ObjectID).setValue(action.result.data.SysGen5IDPatch);
                                if (action.result.data.SysGen5IDPatch != "") { Ext.getCmp("image5Show" + ObjectID).setSrc(action.result.data.SysGen5IDPatch); }
                                else { Ext.getCmp("image5Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }

                                if (New_Edit == 3) { Ext.getCmp("DirNomenID" + ObjectID).setValue(0); }

                                //Разблокировка вызвавшего окна
                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                            },
                            failure: function (form, action) {
                                funPanelSubmitFailure(form, action);

                                //Разблокировка вызвавшего окна
                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                            }
                        });
                    }

                });
            });


            break;
        }

        case "viewDirNomensWinComboEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            var storeDirNomensGrid1 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid1.setData([], false); storeDirNomensGrid1.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=0";

            var storeDirNomensGrid2 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid2.setData([], false);
            //var storeDirNomensGrid3 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid3.setData([], false);
            ///var storeDirNomensGrid4 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid4.setData([], false);
            //var storeDirNomensGrid5 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid5.setData([], false);
            //var storeDirNomensGrid6 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid6.setData([], false);

            var storeDirNomenCategoriesGrid = Ext.create("store.storeDirNomenCategoriesGrid"); storeDirNomenCategoriesGrid.setData([], false); storeDirNomenCategoriesGrid.proxy.url = HTTP_DirNomenCategories + "?type=Grid";
            storeDirNomenCategoriesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,

                storeDirNomensGrid1: storeDirNomensGrid1,
                storeDirNomensGrid2: storeDirNomensGrid2,
                //storeDirNomensGrid3: storeDirNomensGrid3,
                //storeDirNomensGrid4: storeDirNomensGrid4,
                //storeDirNomensGrid5: storeDirNomensGrid5,
                //storeDirNomensGrid6: storeDirNomensGrid6,

                storeDirNomenCategoriesGrid: storeDirNomenCategoriesGrid,

                storeDirCharColoursGrid: varStoreDirCharColoursGrid,
                storeDirCharMaterialsGrid: varStoreDirCharMaterialsGrid,
                storeDirCharNamesGrid: varStoreDirCharNamesGrid,
                storeDirCharSeasonsGrid: varStoreDirCharSeasonsGrid,
                storeDirCharSexesGrid: varStoreDirCharSexesGrid,
                storeDirCharSizesGrid: varStoreDirCharSizesGrid,
                storeDirCharStylesGrid: varStoreDirCharStylesGrid,
                storeDirCharTexturesGrid: varStoreDirCharTexturesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);
                        
            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title + " № Новая");


            //По умолчанию
            Ext.getCmp("DocSecondHandPurchID" + ObjectID).setValue(Ext.getCmp("DocSecondHandPurchID" + Ext.getCmp(UO_idCall).UO_id).getValue());
            Ext.getCmp("DirNomenMinimumBalance" + ObjectID).setValue(varDirNomenMinimumBalance);

            varPriceChange_ReadOnly = true;
            Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
            Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
            Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);

            Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
            Ext.getCmp("PriceCurrency" + ObjectID).setValue(0);
            varPriceChange_ReadOnly = false;
            Ext.getCmp("MarkupRetail" + ObjectID).setValue(varMarkupRetail);
            Ext.getCmp("MarkupWholesale" + ObjectID).setValue(varMarkupWholesale);
            Ext.getCmp("MarkupIM" + ObjectID).setValue(varMarkupIM);


            //Событие на загрузку в Grid
            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirNomenCategoriesGrid.on('load', function () {
                if (storeDirNomenCategoriesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirNomenCategoriesGrid.UO_Loaded = true;

                storeDirNomensGrid1.load({ waitMsg: lanLoading });
                storeDirNomensGrid1.on('load', function () {
                    if (storeDirNomensGrid1.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirNomensGrid1.UO_Loaded = true;


                    //Только если есть на форме "ID0" (может быть это Разборка - там нет)
                    var
                        ID01 = Ext.getCmp("ID01" + Ext.getCmp(UO_idCall).UO_id), ID0 = Ext.getCmp("ID0" + Ext.getCmp(UO_idCall).UO_id),
                        ID11 = Ext.getCmp("ID11" + Ext.getCmp(UO_idCall).UO_id), ID1 = Ext.getCmp("ID1" + Ext.getCmp(UO_idCall).UO_id),
                        ID21 = Ext.getCmp("ID21" + Ext.getCmp(UO_idCall).UO_id), ID2 = Ext.getCmp("ID2" + Ext.getCmp(UO_idCall).UO_id);
                    if (ID01) {
                        
                        //Присваиваем ID-шник первому КомбоБоксу
                        Ext.getCmp("DirNomen1ID" + ObjectID).setValue(ID01.getValue());
                        //Проверка: если не нашли аппарат - выдать сообщение и обнулить КБ "DirNomen1ID"
                        if (!fun_isInteger(Ext.getCmp("DirNomen1ID" + ObjectID).value)) {
                            loadingMask.hide();
                            Ext.getCmp("DirNomen1ID" + ObjectID).setValue("");
                            Ext.Msg.alert(lanOrgName, "Марка аппарата не найдена! Придётся выбрать её вручную!");
                            return;
                        }


                        //Загружаем данные во второй КомбоБокс
                        storeDirNomensGrid2.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=" + ID01.getValue();
                        storeDirNomensGrid2.load({ waitMsg: lanLoading });
                        storeDirNomensGrid2.on('load', function () {
                            Ext.getCmp("DirNomen2ID" + ObjectID).setReadOnly(false); Ext.getCmp("DirNomen2ID" + ObjectID).setValue(ID11.getValue());
                            Ext.getCmp("DirNomenCategoryID" + ObjectID).setReadOnly(false); Ext.getCmp("DirNomenCategoryID" + ObjectID).setValue("");


                            //Проверка: если не нашли аппарат - выдать сообщение и обнулить КБ "DirNomen2ID"
                            if (!fun_isInteger(Ext.getCmp("DirNomen2ID" + ObjectID).value)) {
                                loadingMask.hide();
                                Ext.getCmp("DirNomen2ID" + ObjectID).setValue("");
                                Ext.Msg.alert(lanOrgName, "Модель аппарата не найдена! Придётся выбрать её вручную!");
                                return;
                            }


                        });

                    }




                    loadingMask.hide();

                });

            });


            break;
        }

        case "viewDirNomensImg": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                UO_Param_id: UO_Param_id, //Какое изображение: "", "1", "2", ... "5"

            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            break;
        }

            /* Товар */

        case "viewDirServiceNomensWinEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }


            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName; // + "_" + IdcallModelData.id;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;


            var storeDirServiceNomenPricesGrid = Ext.create("store.storeDirServiceNomenPricesGrid"); storeDirServiceNomenPricesGrid.setData([], false); storeDirServiceNomenPricesGrid.proxy.url = HTTP_DirServiceNomenPrices + "?type=Grid&DirServiceNomenID=" + IdcallModelData.id;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirServiceNomenPricesGrid: storeDirServiceNomenPricesGrid
            });

            //Если создаём подкатегорию, то надо указать "Sub"
            if (UO_GridIndex != undefined) { Ext.getCmp("Sub" + ObjectID).setValue(UO_GridIndex); }
           
            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);



            //Событие на загрузку в Grid
            //Лоадер
            if (New_Edit == 1) {

                //...

                //Фокус на открывшийся Виджет
                widgetX.focus();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

            }
            else if (New_Edit == 2 || New_Edit == 3) {

                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                //Если форма уже загружена выходим!
                if (widgetXForm.UO_Loaded) return;

                widgetXForm.load({
                    method: "GET",
                    timeout: varTimeOutDefault,
                    waitMsg: lanLoading,
                    url: HTTP_DirServiceNomens + IdcallModelData.id + "/", //rec.get('id') + "/",
                    success: function (form, action) {
                        widgetXForm.UO_Loaded = true;

                        if (New_Edit == 3) { Ext.getCmp("DirServiceNomenID" + ObjectID).setValue(0); }

                        var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();
                        storeDirServiceNomenPricesGrid.load({ waitMsg: lanLoading });
                        storeDirServiceNomenPricesGrid.on('load', function () {
                            loadingMask.hide();
                        });


                        //Разблокировка вызвавшего окна
                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                    },
                    failure: function (form, action) {
                        funPanelSubmitFailure(form, action);

                        //Разблокировка вызвавшего окна
                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                    }
                });
            }


            break;
        }

            /* Товар - Выбор */

        case "viewDirNomensSelect": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridIndex: UO_GridIndex,   //Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid
                UO_GridRecord: UO_GridRecord, //Ext.getCmp(view.grid.UO_idCall).UO_id
                UO_Param_id: UO_Param_id,     //view.grid.UO_id
                UO_Param_fn: UO_Param_fn,     //record,
                UO_idTab: UO_idTab            //view.grid.UO_idMain
            });

            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);
            

            //Данные из "record"
            Ext.getCmp("DirNomenName" + ObjectID).setValue(UO_Param_fn.data.DirNomenName);
            //Получаем тип цены
            var DirPriceTypeID = parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue());
            switch (DirPriceTypeID) {
                case 1:
                    {
                        Ext.getCmp("PriceCurrency" + ObjectID).setValue(UO_Param_fn.data.PriceRetailCurrency);
                    }
                    break;
                case 2:
                    {
                        Ext.getCmp("PriceCurrency" + ObjectID).setValue(UO_Param_fn.data.PriceWholesaleCurrency);
                    }
                    break;
                case 3:
                    {
                        Ext.getCmp("PriceCurrency" + ObjectID).setValue(UO_Param_fn.data.PriceIMCurrency);
                    }
                    break;
            }



            break;
        }

            /* Выполненная работа - Сервис */

        case "viewDirServiceJobNomensWinEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName; // + "_" + IdcallModelData.id;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
            });

            //Если создаём подкатегорию, то надо указать "Sub"
            if (UO_GridIndex != undefined) { Ext.getCmp("Sub" + ObjectID).setValue(UO_GridIndex); }

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);



            //Событие на загрузку в Grid
            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);

                if (New_Edit == 1) {

                    //...
                    //var x = storeDirCurrenciesGrid;
                    if (UO_GridServerParam1 == 1) {
                        Ext.getCmp("DirServiceJobNomenType" + ObjectID).setValue(1);
                        //Ext.getCmp("DirServiceJobNomenType_" + ObjectID).setValue(1);
                    }
                    else {
                        Ext.getCmp("DirServiceJobNomenType" + ObjectID).setValue(2);
                        //Ext.getCmp("DirServiceJobNomenType_" + ObjectID).setValue(2);
                    }


                    //Фокус на открывшийся Виджет
                    widgetX.focus();

                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                }
                else if (New_Edit == 2 || New_Edit == 3) {

                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                    //Если форма уже загружена выходим!
                    if (widgetXForm.UO_Loaded) return;

                    widgetXForm.load({
                        method: "GET",
                        timeout: varTimeOutDefault,
                        waitMsg: lanLoading,
                        url: HTTP_DirServiceJobNomens + IdcallModelData.id + "/", //rec.get('id') + "/",
                        success: function (form, action) {
                            widgetXForm.UO_Loaded = true;

                            if (New_Edit == 3) { Ext.getCmp("DirServiceNomenID" + ObjectID).setValue(0); }

                            //Разблокировка вызвавшего окна
                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                        },
                        failure: function (form, action) {
                            funPanelSubmitFailure(form, action);

                            //Разблокировка вызвавшего окна
                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                        }
                    });
                }

            });


            break;
        }

            /* Результат диагностики */

        case "viewDirServiceDiagnosticRresultsWin": {
            
            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            
            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                UO_Param_id: UO_Param_id, UO_Param_fn: UO_Param_fn, UO_idTab: UO_idTab, UO_GridServerParam1: UO_GridServerParam1,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
            });

            //Если создаём подкатегорию, то надо указать "Sub"
            //if (UO_GridIndex != undefined) { Ext.getCmp("Sub" + ObjectID).setValue(UO_GridIndex); }

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);



            //Событие на загрузку в Grid
            //Лоадер
            /*
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();
                Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);

            });
            */


            break;
        }

            /* Товар - Заказ */

        case "viewDocOrderIntsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            var storeDirServiceNomensGrid = Ext.create("store.storeDirServiceNomensGrid"); storeDirServiceNomensGrid.setData([], false); storeDirServiceNomensGrid.proxy.url = HTTP_DirServiceNomens + "?type=Grid&GroupID=0";
            var storeDirNomensGrid1 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid1.setData([], false); storeDirNomensGrid1.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=0";
            var storeDirNomensGrid2 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid2.setData([], false);
            var storeDirNomensGrid3 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid3.setData([], false);
            ///var storeDirNomensGrid4 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid4.setData([], false);
            //var storeDirNomensGrid5 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid5.setData([], false);
            //var storeDirNomensGrid6 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid6.setData([], false);

            var storeDirNomenCategoriesGrid = Ext.create("store.storeDirNomenCategoriesGrid"); storeDirNomenCategoriesGrid.setData([], false); storeDirNomenCategoriesGrid.proxy.url = HTTP_DirNomenCategories + "?type=Grid";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid";

            
            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget.viewDocOrderIntsEdit", {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,

                storeDirServiceNomensGrid: storeDirServiceNomensGrid,
                storeDirNomensGrid1: storeDirNomensGrid1,
                storeDirNomensGrid2: storeDirNomensGrid2,
                storeDirNomensGrid3: storeDirNomensGrid3,
                //storeDirNomensGrid4: storeDirNomensGrid4,
                //storeDirNomensGrid5: storeDirNomensGrid5,
                //storeDirNomensGrid6: storeDirNomensGrid6,

                storeDirNomenCategoriesGrid: storeDirNomenCategoriesGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,

                storeDirCharColoursGrid: varStoreDirCharColoursGrid,
                storeDirCharMaterialsGrid: varStoreDirCharMaterialsGrid,
                storeDirCharNamesGrid: varStoreDirCharNamesGrid,
                storeDirCharSeasonsGrid: varStoreDirCharSeasonsGrid,
                storeDirCharSexesGrid: varStoreDirCharSexesGrid,
                storeDirCharSizesGrid: varStoreDirCharSizesGrid,
                storeDirCharStylesGrid: varStoreDirCharStylesGrid,
                storeDirCharTexturesGrid: varStoreDirCharTexturesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Передеча параметров *** *** *** *** *** *** *** ***
            //ID вызвавшей вьюхи
            var UO_id = Ext.getCmp(UO_idCall).UO_id;
            //Тип
            Ext.getCmp("DirOrderIntTypeID" + ObjectID).setValue(UO_GridIndex);
            //Если Мастерская, то: убрать предоплату и Тел + ФИО подтягивать с СС
            if (UO_GridIndex > 2) {
                Ext.getCmp("PrepaymentSum" + ObjectID).setValue(0);
                Ext.getCmp("PrepaymentSum" + ObjectID).setVisible(false);
                Ext.getCmp("DirOrderIntContractorName" + ObjectID).allowBlank = true;
                Ext.getCmp("DirOrderIntContractorPhone" + ObjectID).setVisible(false);
                Ext.getCmp("DirOrderIntContractorName" + ObjectID).setVisible(false);
            }
            //ID-шники с формы, в зависимости от типа
            if (UO_GridIndex > 2) {
                //ID-шник вьюхи "viewDocServiceWorkshops" или "viewDocSecondHandWorkshops"

                var UO_id2;
                if (Ext.getCmp("viewDocServiceWorkshops" + Ext.getCmp(Ext.getCmp(UO_idCall).UO_idCall).UO_id)) {
                    UO_id2 = Ext.getCmp("viewDocServiceWorkshops" + Ext.getCmp(Ext.getCmp(UO_idCall).UO_idCall).UO_id).UO_id;

                    Ext.getCmp("NumberReal" + ObjectID).setValue(Ext.getCmp("DocServicePurchID" + UO_id2).getValue());
                }
                else if (Ext.getCmp("viewDocSecondHandWorkshops" + Ext.getCmp(Ext.getCmp(UO_idCall).UO_idCall).UO_id)) {
                    UO_id2 = Ext.getCmp("viewDocSecondHandWorkshops" + Ext.getCmp(Ext.getCmp(UO_idCall).UO_idCall).UO_id).UO_id;
                }

                //Дата
                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                //Получаем: DocID, DirWarehouseID
                Ext.getCmp("DocID2_" + ObjectID).setValue(Ext.getCmp("DocID" + UO_id2).getValue());
                Ext.getCmp("DocIDBase" + ObjectID).setValue(Ext.getCmp("DocID" + UO_id2).getValue());
                Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(Ext.getCmp("DirContractorIDOrg" + UO_id2).getValue());
                Ext.getCmp("DirWarehouseID" + ObjectID).setValue(Ext.getCmp("DirWarehouseID" + UO_id2).getValue());
            }
            else {
                //Дата
                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());

                Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);
                Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
            }

            
            var dat = new Date(); dat.setDate(dat.getDate() + varReadinessDay);
            Ext.getCmp("DateDone" + ObjectID).setValue(dat);
            Ext.getCmp("DirOrderIntContractorPhone" + ObjectID).setValue(varPhoneNumberBegin);

            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(1);
            Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
            Ext.getCmp("PriceCurrency" + ObjectID).setValue(0);
            Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
            Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
            Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);


            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title + " № Новая");
            

            //Событие на загрузку в Grid
            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirServiceNomensGrid.load({ waitMsg: lanLoading });
            storeDirServiceNomensGrid.on('load', function () {
                if (storeDirServiceNomensGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomensGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                
                storeDirContractorsGrid.load({ waitMsg: lanLoading });
                storeDirContractorsGrid.on('load', function () {
                    if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirNomensGrid1.load({ waitMsg: lanLoading });
                    storeDirNomensGrid1.on('load', function () {
                        if (storeDirNomensGrid1.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirNomensGrid1.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        loadingMask.hide();



                        //Если выбран *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                        //UO_idCall == Ext.getCmp("tree_" + id)
                        if (Ext.getCmp(UO_idCall).getSelectionModel().hasSelection()) {



                            //rec
                            var rec = Ext.getCmp(UO_idCall).getSelectionModel().getSelection();
                            if (rec.length == 0) return;
                            else rec = rec[0];


                            //Ставим на Комбы признак (в конце метода снимим), что бы автоматически не обновляло их с сервера
                            var cb1 = Ext.getCmp("DirNomen1ID" + ObjectID);
                            var cb2 = Ext.getCmp("DirNomen2ID" + ObjectID);
                            var cb3 = Ext.getCmp("DirNomenID" + ObjectID);

                            controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, false, cb1, cb2, cb3); //, cb3, cb4, cb5, cb6


                            //КомбоБоксы (Парсим "rec.get('DirNomenIDFull')" в массив)
                            if (rec.get('DirNomenIDFull') == undefined) return;
                            var arr = rec.get('DirNomenIDFull').split(',');


                            if (arr.length >= 1) {
                                cb1.setValue(arr[0]);

                                var storeDirNomensGrid2 = cb2.store; //Ext.getCmp("viewDocOrderIntsPattern" + ObjectID).storeDirNomensGrid2;
                                storeDirNomensGrid2.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=" + arr[0];
                                storeDirNomensGrid2.arr = arr;
                                storeDirNomensGrid2.rec = rec;
                                storeDirNomensGrid2.load({ waitMsg: lanLoading });
                                storeDirNomensGrid2.on('load', function () {
                                    if (storeDirNomensGrid2.UO_Loaded) return; //Уже загружали - выйти!
                                    storeDirNomensGrid2.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                    cb2.setReadOnly(false); cb2.setValue("");

                                    if (storeDirNomensGrid2.arr.length >= 2) {
                                        cb2.setValue(storeDirNomensGrid2.arr[1]);



                                        var storeDirNomensGrid3 = cb3.store; //Ext.getCmp("viewDocOrderIntsPattern" + ObjectID).storeDirNomensGrid3;
                                        storeDirNomensGrid3.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=" + arr[1];
                                        storeDirNomensGrid3.arr = arr;
                                        storeDirNomensGrid3.rec = rec;
                                        storeDirNomensGrid3.load({ waitMsg: lanLoading });
                                        storeDirNomensGrid3.on('load', function () {
                                            if (storeDirNomensGrid3.UO_Loaded) return; //Уже загружали - выйти!
                                            storeDirNomensGrid3.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                            cb3.setReadOnly(false); cb3.setValue("");


                                            // !!! ТОВАР !!!
                                            if (storeDirNomensGrid3.arr.length >= 3) {
                                                cb3.setValue(storeDirNomensGrid3.arr[2]); //storeDirNomensGrid2.rec.get('DirNomenID')
                                            }


                                        }); //storeDirNomensGrid3


                                        //Активизируем эвент Комбов

                                        controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, true, cb1, cb2, cb3);


                                    } //if (arr.length > 2) {
                                    else { controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, true, cb1, cb2, cb3); } //, cb3, cb4, cb5, cb6

                                }); //storeDirNomensGrid2

                            } //if (arr.length > 1) {
                            else { controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, true, cb1, cb2, cb3); } //, cb3, cb4, cb5, cb6


                            //Наименование
                            //Ext.getCmp("DirNomenName" + ObjectID).setValue(rec.get('DirNomenPatchFull'));
                            //Ext.getCmp("DirNomenID" + ObjectID).setValue(rec.get('id'));
                            fun_controllerDocOrderIntsEdit_RequestPrice(rec.get('id'), ObjectID);

                            //Ставим на Комбы признак (в конце метода снимим), что бы автоматически не обновляло их с сервера
                            Ext.getCmp("DirNomen1ID" + ObjectID).UO_NoAutoLoad = true;

                        }

                        //Если выбран *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                    });

                }); //storeDirContractorsGrid

            }); //storeDirServiceNomensGrid


            break;
        }

        /* Товар - Заказ */

        case "viewDocOrderIntsNomensEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Grid
            var storeGrid = Ext.create("store.storeDirNomensTree"); storeGrid.setData([], false); storeGrid.proxy.url = HTTP_DirNomens + "?type=Tree";

            var storeDirServiceNomensGrid = Ext.create("store.storeDirServiceNomensGrid"); storeDirServiceNomensGrid.setData([], false); storeDirServiceNomensGrid.proxy.url = HTTP_DirServiceNomens + "?type=Grid&GroupID=0";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid";
            var storeDirNomensGrid1 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid1.setData([], false); storeDirNomensGrid1.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=0";
            var storeDirNomensGrid2 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid2.setData([], false);
            var storeDirNomensGrid3 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid3.setData([], false);
            //var storeDirNomensGrid4 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid4.setData([], false);
            //var storeDirNomensGrid5 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid5.setData([], false);
            //var storeDirNomensGrid6 = Ext.create("store.storeDirNomensGrid"); storeDirNomensGrid6.setData([], false);

            var storeDirNomenCategoriesGrid = Ext.create("store.storeDirNomenCategoriesGrid"); storeDirNomenCategoriesGrid.setData([], false); storeDirNomenCategoriesGrid.proxy.url = HTTP_DirNomenCategories + "?type=Grid";


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget.viewDocOrderIntsNomensEdit", {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,

                storeGrid: storeGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,

                storeDirServiceNomensGrid: storeDirServiceNomensGrid,
                storeDirNomensGrid1: storeDirNomensGrid1,
                storeDirNomensGrid2: storeDirNomensGrid2,
                storeDirNomensGrid3: storeDirNomensGrid3,
                //storeDirNomensGrid4: storeDirNomensGrid4,
                //storeDirNomensGrid5: storeDirNomensGrid5,
                //storeDirNomensGrid6: storeDirNomensGrid6,

                storeDirNomenCategoriesGrid: storeDirNomenCategoriesGrid,


                storeDirCharColoursGrid: varStoreDirCharColoursGrid,
                storeDirCharMaterialsGrid: varStoreDirCharMaterialsGrid,
                storeDirCharNamesGrid: varStoreDirCharNamesGrid,
                storeDirCharSeasonsGrid: varStoreDirCharSeasonsGrid,
                storeDirCharSexesGrid: varStoreDirCharSexesGrid,
                storeDirCharSizesGrid: varStoreDirCharSizesGrid,
                storeDirCharStylesGrid: varStoreDirCharStylesGrid,
                storeDirCharTexturesGrid: varStoreDirCharTexturesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Передеча параметров *** *** *** *** *** *** *** ***
            //ID вызвавшей вьюхи
            var UO_id = Ext.getCmp(UO_idCall).UO_id;
            //Тип
            Ext.getCmp("DirOrderIntTypeID" + ObjectID).setValue(UO_GridIndex);
            //ID-шники с формы, в зависимости от типа
            if (UO_GridIndex > 2) {

                if (Ext.getCmp(UO_idCall).UO_idCall == undefined) {
                    alert("Ошибка! параметр 'Ext.getCmp(UO_idCall).UO_idCal' is undefined");
                    return;
                }

                //ID-шник вьюхи "viewDocServiceWorkshops"
                var UO_id2 = Ext.getCmp("viewDocServiceWorkshops" + Ext.getCmp(Ext.getCmp(UO_idCall).UO_idCall).UO_id).UO_id;

                //Дата
                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                //Получаем: DocID, DocServicePurchID, DirWarehouseID
                Ext.getCmp("DocID2" + ObjectID).setValue(Ext.getCmp("DocID" + UO_id2).getValue());
                Ext.getCmp("NumberReal" + ObjectID).setValue(Ext.getCmp("DocServicePurchID" + UO_id2).getValue());
                Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(Ext.getCmp("DirContractorIDOrg" + UO_id2).getValue());
                Ext.getCmp("DirWarehouseID" + ObjectID).setValue(Ext.getCmp("DirWarehouseID" + UO_id2).getValue());
            }
            else {
                //Дата
                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());

                Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);
                Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
            }


            var dat = new Date(); dat.setDate(dat.getDate() + varReadinessDay);
            Ext.getCmp("DateDone" + ObjectID).setValue(dat);
            Ext.getCmp("DirOrderIntContractorPhone" + ObjectID).setValue(varPhoneNumberBegin);

            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(1);
            Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
            Ext.getCmp("PriceCurrency" + ObjectID).setValue(0);
            Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
            Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
            Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);


            //Событие на загрузку в Grid
            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();
            
            storeDirServiceNomensGrid.load({ waitMsg: lanLoading });
            storeDirServiceNomensGrid.on('load', function () {
                if (storeDirServiceNomensGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomensGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                
                storeDirContractorsGrid.load({ waitMsg: lanLoading });
                storeDirContractorsGrid.on('load', function () {
                    if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirNomensGrid1.load({ waitMsg: lanLoading });
                    storeDirNomensGrid1.on('load', function () {
                        if (storeDirNomensGrid1.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirNomensGrid1.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        loadingMask.hide();

                    }); //storeDirNomensGrid1

                }); //storeDirContractorsGrid

            }); //storeDirServiceNomensGrid


            break;
        }


        /* Б/У - На продажу */

        case "viewDocOrderIntsPurches": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;


            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,

                UO_GridServerParam1: UO_GridServerParam1,
                UO_Param_fn: UO_Param_fn,   //controllerDocSecondHandWorkshops_ChangeStatus_Request
                UO_idTab: UO_idTab,         //aButton
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);



            storeDirContractorsGrid.load({ waitMsg: lanLoading });
            storeDirContractorsGrid.on('load', function () {
                if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                
                Ext.getCmp("DirContractorID" + ObjectID).setValue(Ext.getCmp("DirContractorID" + UO_Param_id).getValue());


                //!!! Цены !!!
                Ext.getCmp("DirNomenID" + ObjectID).setValue(Ext.getCmp("DirNomenID" + UO_Param_id).getValue());
                if (Ext.getCmp("DirNomenID" + UO_Param_id).getValue()) {

                    Ext.Ajax.request({
                        timeout: varTimeOutDefault, waitMsg: lanUpload, method: 'GET',
                        url: HTTP_DirNomenHistories + Ext.getCmp("DirNomenID" + UO_Param_id).getValue() + "/?Action=1",
                        success: function (result) {

                            
                            //varPriceChange_ReadOnly = true;


                            var sData = Ext.decode(result.responseText);
                            if (sData.success == false) {
                                //Если нет данных на Сервере, то проставляем по умолчанию:
                                Ext.getCmp("PriceCurrency" + ObjectID).setValue(0);
                                Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
                                Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                                Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                                Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                                //Если наценки отрицательные, то ставим их из Настроек
                                funMarkupSet(ObjectID, sData.data + "<br />");
                                //return;
                            }
                            else {
                                                        
                                var form = Ext.getCmp("form_" + ObjectID);
                                if (form != undefined) {
                                    //Создаём модель
                                    var UO_GridRecord = Ext.create("PartionnyAccount.model.Sklad/Object/Rem/RemParties/modelRemPartiesGrid")
                                    //Пишем в модель данные
                                    UO_GridRecord.data = sData.data.Result;
                                    form.loadRecord(UO_GridRecord);
                                    funMarkupSet(ObjectID); //Если наценки отрицательные, то ставим их из Настроек
                                }
                                else {
                                    Ext.getCmp("PriceCurrency" + ObjectID).setValue(sData.data.Result.PriceRetailCurrency);
                                }

                                //return;
                            }

                            
                            //varPriceChange_ReadOnly = false;


                        },
                        failure: function (form, action) { funPanelSubmitFailure(form, action); }
                    });
                }
                else {

                    varPriceChange_ReadOnly = true;

                    Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                    Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                    Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);

                    Ext.getCmp("PriceVAT" + ObjectID).setValue(Ext.getCmp("PriceVAT" + UO_Param_id).getValue());
                    if (parseFloat(Ext.getCmp("PriceVAT" + UO_Param_id).getValue()) == 0 && parseFloat(Ext.getCmp("PriceCurrency" + UO_Param_id).getValue()) > 0) {
                        Ext.getCmp("PriceVAT" + ObjectID).setValue(Ext.getCmp("PriceCurrency" + UO_Param_id).getValue());
                    }
                    Ext.getCmp("PriceCurrency" + ObjectID).setValue(Ext.getCmp("PriceCurrency" + UO_Param_id).getValue());

                    Ext.getCmp("PriceRetailVAT" + ObjectID).setValue(0); Ext.getCmp("PriceRetailCurrency" + ObjectID).setValue(0);
                    Ext.getCmp("PriceWholesaleVAT" + ObjectID).setValue(0); Ext.getCmp("PriceWholesaleCurrency" + ObjectID).setValue(0);
                    Ext.getCmp("PriceIMVAT" + ObjectID).setValue(0); Ext.getCmp("PriceIMCurrency" + ObjectID).setValue(0);

                    varPriceChange_ReadOnly = false;

                    Ext.getCmp("MarkupRetail" + ObjectID).setValue(varMarkupRetail);
                    Ext.getCmp("MarkupWholesale" + ObjectID).setValue(varMarkupWholesale);
                    Ext.getCmp("MarkupIM" + ObjectID).setValue(varMarkupIM);

                }


            }); //storeDirContractorsGrid


            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }


            //Торговля *** *** ***


            /* Приход */

        case "viewDocPurchesEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }


            //Если открыли документ из Партий
            if (UO_GridSave) {
                IdcallModelData.DocPurchID = IdcallModelData.NumberReal;
            }


            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocPurchID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Переключаемся на уже открытую вкладку
            /*var tabPanelMain = Ext.getCmp("viewContainerCentral");
            var i = tabPanelMain.items.items.length;
            for (var i = 0; i < tabPanelMain.items.items.length; i++) {
                if (tabPanelMain.items.items[i].UO_Identy == UO_Identy) {

                    //Делаем активной найденную вкладку
                    tabPanelMain.setActiveTab(tabPanelMain.items.items[i]);

                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    return;
                }
            }*/



            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //2. Combo
            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=2&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocPurchTabsGrid = Ext.create("store.storeDocPurchTabsGrid"); storeDocPurchTabsGrid.setData([], false);
            storeDocPurchTabsGrid.proxy.url = HTTP_DocPurchTabs + "?DocPurchID=" + IdcallModelData.DocPurchID;
            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_Center: UO_Center,
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocPurchTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel"
            Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                        storeDirWarehousesGrid.on('load', function () {
                            if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirVatsGrid.load({ waitMsg: lanLoading });
                            storeDirVatsGrid.on('load', function () {
                                if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                loadingMask.hide();

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Скидка
                                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                    //Сумма с Налогом
                                    Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                    //Сума Налога
                                    Ext.getCmp("SumVATCurrency" + ObjectID).setValue(0);
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();


                                    //Справочники
                                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                                    //Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                                    Ext.getCmp("Payment" + ObjectID).setValue(0);


                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                                    else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                    //Остаток по Складу: Присваиваем Товару - Склад
                                    if (varDirWarehouseIDEmpl == 0) { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
                                    else { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    storeDocPurchTabsGrid.load({ waitMsg: lanLoading });
                                    storeDocPurchTabsGrid.on('load', function () {
                                        if (storeDocPurchTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                        storeDocPurchTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                        //Если форма уже загружена выходим!
                                        if (widgetXForm.UO_Loaded) return;
                                        
                                        widgetXForm.load({
                                            method: "GET",
                                            timeout: varTimeOutDefault,
                                            waitMsg: lanLoading,
                                            url: HTTP_DocPurches + IdcallModelData.DocPurchID + "/?DocID=" + IdcallModelData.DocID,
                                            success: function (form, action) {

                                                widgetXForm.UO_Loaded = true;
                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Если Копия
                                                if (New_Edit == 3) {
                                                    Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocPurchID" + ObjectID).setValue(null);
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                else {
                                                    //Наименование окна (сверху)
                                                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocPurchID" + ObjectID).getValue());

                                                    //Проведён или нет
                                                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                        Ext.Msg.alert(lanOrgName, txtMsg020);
                                                        Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                    }
                                                    else {
                                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                                    }
                                                    Ext.getCmp("btnPrint" + ObjectID).show();
                                                    Ext.getCmp("btnGridPayment" + ObjectID).enable();
                                                }

                                                //Остаток по Складу: Присваиваем Товару - Склад
                                                storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();

                                                Ext.getCmp("btnOnBasisOfDoc" + ObjectID).enable(true);

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                            },
                                            failure: function (form, action) {
                                                //loadingMask.hide();
                                                widgetX.close();
                                                funPanelSubmitFailure(form, action);

                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            }
                                        });

                                    });

                                }

                            });
                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Приходная накладная: Редактирование Грида */

        case "viewDocPurchTabsEdit": {
            
            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,

                storeDirCharColoursGrid: varStoreDirCharColoursGrid,
                storeDirCharMaterialsGrid: varStoreDirCharMaterialsGrid,
                storeDirCharNamesGrid: varStoreDirCharNamesGrid,
                storeDirCharSeasonsGrid: varStoreDirCharSeasonsGrid,
                storeDirCharSexesGrid: varStoreDirCharSexesGrid,
                storeDirCharSizesGrid: varStoreDirCharSizesGrid,
                storeDirCharStylesGrid: varStoreDirCharStylesGrid,
                storeDirCharTexturesGrid: varStoreDirCharTexturesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //DocPurchID
            if (Ext.getCmp("DocPurchID" + Ext.getCmp(UO_idCall).UO_id)) {
                Ext.getCmp("DocPurchID" + ObjectID).setValue(Ext.getCmp("DocPurchID" + Ext.getCmp(UO_idCall).UO_id).getValue());
            }
            else {
                Ext.getCmp("DocInventoryID" + ObjectID).setValue(Ext.getCmp("DocInventoryID" + Ext.getCmp(UO_idCall).UO_id).getValue());
            }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsGrid.load({ waitMsg: lanLoading });
                storeDirContractorsGrid.on('load', function () {
                    if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    loadingMask.hide();
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    if (New_Edit == 1) { // - Не используется !!!
                        /*
                        //Если новая запись, то установить "по умолчанию"
                        Ext.getCmp("btnDel" + ObjectID).setVisible(false);
                        //Если наценки отрицательные, то ставим их из Настроек
                        funMarkupSet(ObjectID);
                        */
                    }
                    else if (New_Edit == 2 || New_Edit == 3) {
                        //Если редактировать, то: Загрузка данных в Форму "widgetXPanel"
                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                        if (UO_GridSave) {
                            varPriceChange_ReadOnly = true; //Запретить редактировать цены
                            //Форма
                            var form = widgetXForm.getForm();

                            if (GridTree) {
                                //Редактирование (загрузить из грида)
                                form.loadRecord(UO_GridRecord);
                            }
                            else {
                                //Новый товар
                                //Может возникнуть ситуация, когда не выбран товар
                                if (UO_GridRecord != undefined) {
                                    Ext.getCmp("DirNomenID" + ObjectID).setValue(UO_GridRecord.data.id);
                                    Ext.getCmp("DirNomenName" + ObjectID).setValue(UO_GridRecord.data.text);


                                    //Пробегаемся по всем партиям и ищим с последней датой
                                    //Если не находим, то ставим всё "по нулям"
                                    //1. Грид Party *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                                    var id = Ext.getCmp(UO_idCall).UO_id;

                                    //Выбранная партия
                                    var IdcallModelData = Ext.getCmp("gridParty_" + id).getSelectionModel().getSelection();

                                    //1. Если не выбрана партия товара
                                    if (IdcallModelData.length == 0) {

                                        var PanelParty = Ext.getCmp("gridParty_" + id).store.data.items;

                                        //2. Выбираем данные из партии
                                        if (PanelParty.length > 0) {
                                            //2.1. Если есть Партии, то выбираем самую последнюю
                                            UO_GridRecord = PanelParty[0];
                                            for (var i = PanelParty.length - 1; i > 0; i--) { //for (var i = 1; i < PanelParty.length; i++) {
                                                if (PanelParty[i].data.DocDate > UO_GridRecord.data.DocDate) UO_GridRecord = PanelParty[i];
                                            }
                                        }
                                        else {
                                            //2.2. Если нет Партии, то делаем запрос на Сервер за Партией, которые уже проданы,
                                            //     если на Сервере тоже нет данных выдаём сообщение
                                            fun_viewDocPurchTabsEdit_RequestPrice(form, UO_GridRecord, ObjectID);

                                        }
                                    }

                                        //2. Если выбрана партия товара, то её и берём на основу!
                                    else {
                                        UO_GridRecord = IdcallModelData[0]
                                    }

                                    form.loadRecord(UO_GridRecord);
                                    Ext.getCmp("Quantity" + ObjectID).setValue(1);
                                    //Мин.остаток
                                    Ext.getCmp("DirNomenMinimumBalance" + ObjectID).setValue(varDirNomenMinimumBalance);



                                    //Поставщик
                                    /*
                                    var locDirContractorID = Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id).getRawValue();
                                    var comboBox = Ext.getCmp("DirCharStyleID" + ObjectID);
                                    var store = comboBox.store;
                                    var locResult = store.findExact("DirCharStyleName", locDirContractorID);
                                    Ext.getCmp("DirCharStyleID" + ObjectID).setValue(store.getAt(locResult));
                                    */
                                    if (Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id)) {
                                        var locDirContractorID = Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id).getRawValue();
                                        var comboBox = Ext.getCmp("DirContractorID" + ObjectID);
                                        var store = comboBox.store;
                                        var locResult = store.findExact("DirContractorName", locDirContractorID);
                                        Ext.getCmp("DirContractorID" + ObjectID).setValue(store.getAt(locResult));
                                    }


                                }
                            }
                            form.UO_Loaded = true;
                            varPriceChange_ReadOnly = false; //Разрешить редактировать цены
                        }
                    }

                }); //storeDirContractorsGrid
            }); //storeDirCurrenciesGrid

            break;
        }


            /* Расход */

        case "viewDocSalesEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocSaleID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=3&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocSaleTabsGrid = Ext.create("store.storeDocSaleTabsGrid"); storeDocSaleTabsGrid.setData([], false); storeDocSaleTabsGrid.proxy.url = HTTP_DocSaleTabs + "?DocSaleID=" + IdcallModelData.DocSaleID;
            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocSaleTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                        storeDirWarehousesGrid.on('load', function () {
                            if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirVatsGrid.load({ waitMsg: lanLoading });
                            storeDirVatsGrid.on('load', function () {
                                if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                loadingMask.hide();

                                //Тип цен
                                Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Скидка
                                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                    //Сумма с Налогом
                                    Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                    //Сума Налога
                                    Ext.getCmp("SumVATCurrency" + ObjectID).setValue(0);
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();

                                    //Справочники
                                    //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                                    //Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                                    Ext.getCmp("Payment" + ObjectID).setValue(0);
                                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменоц проведения прихода)

                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                                    else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                    //Для "остаток по складу": Присваиваем Товару - Склад
                                    if (varDirWarehouseIDEmpl == 0) { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
                                    else { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                                    //ArrList - значит
                                    if (!ArrList) {

                                        storeDocSaleTabsGrid.load({ waitMsg: lanLoading });
                                        storeDocSaleTabsGrid.on('load', function () {
                                            if (storeDocSaleTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                            storeDocSaleTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                                            //Если форма уже загружена выходим!
                                            if (widgetXForm.UO_Loaded) return;

                                            widgetXForm.load({
                                                method: "GET",
                                                timeout: varTimeOutDefault,
                                                waitMsg: lanLoading,
                                                url: HTTP_DocSales + IdcallModelData.DocSaleID + "/?DocID=" + IdcallModelData.DocID,
                                                success: function (form, action) {

                                                    widgetXForm.UO_Loaded = true;
                                                    //Фокус на открывшийся Виджет
                                                    widgetX.focus();

                                                    //Если Копия
                                                    if (New_Edit == 3) {
                                                        Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSaleID" + ObjectID).setValue(null);
                                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                                    }
                                                    else {
                                                        //Наименование окна (сверху)
                                                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSaleID" + ObjectID).getValue());

                                                        //Проведён или нет
                                                        if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                            Ext.Msg.alert(lanOrgName, txtMsg020);
                                                            Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                        }
                                                        else {
                                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                                        }
                                                        //Кнопку "Печать" - делаем активной"
                                                        Ext.getCmp("btnPrint" + ObjectID).show();
                                                        //Кнопку "Платежи" - делаем активной"
                                                        Ext.getCmp("btnGridPayment" + ObjectID).enable();
                                                    }

                                                    //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                                                    Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                    //Остаток по Складу: Присваиваем Товару - Склад
                                                    storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                                    //Разблокировка вызвавшего окна
                                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                },
                                                failure: function (form, action) {
                                                    //loadingMask.hide();
                                                    widgetX.close();
                                                    funPanelSubmitFailure(form, action);

                                                    //Фокус на открывшийся Виджет
                                                    widgetX.focus();

                                                    //Разблокировка вызвавшего окна
                                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                }
                                            });

                                        });

                                    } //if(!ArrList)
                                    //Создать "На основании ..."
                                    else {
                                        //Переменные
                                        var formRec = ArrList[0];
                                        var gridRec = ArrList[1];
                                        //Форма
                                        var form = widgetXForm.getForm();
                                        form.loadRecord(formRec);
                                        //Грид
                                        //storeDocSaleTabsGrid.load({ waitMsg: lanLoading });
                                        for (var i = 0; i < gridRec.data.length; i++) storeDocSaleTabsGrid.add(gridRec.data.items[i].data);

                                        // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                                        widgetXForm.UO_Loaded = true;
                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();

                                        //Если Копия
                                        if (New_Edit == 3) {
                                            Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSaleID" + ObjectID).setValue(null);
                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                        }
                                        else {
                                            //Наименование окна (сверху)
                                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSaleID" + ObjectID).getValue());
                                            //Проведён или нет
                                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                                Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                            }
                                            else {
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            Ext.getCmp("btnPrint" + ObjectID).show();
                                        }

                                        //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                        Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                        //Остаток по Складу: Присваиваем Товару - Склад
                                        storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                        //Разблокировка вызвавшего окна
                                        //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                    }

                                }

                            });
                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Расходная накладная: Редактирование Грида */

        case "viewDocSaleTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false);
            storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                        UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }


            /* Перемещение */

        case "viewDocMovementsEdit": {
            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = [0];
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection();
                if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                    return;
                }
            }
            else {
                IdcallModelData.DocMovementID = 0;
            }


            //Если Логистика
            if (IdcallModelData.DocMovementID == undefined && IdcallModelData.LogisticID > 0) {
                IdcallModelData.DocMovementID = IdcallModelData.LogisticID;
            }

            
            //Если открыли документ из Партий
            if (UO_GridSave) {
                IdcallModelData.DocMovementID = IdcallModelData.NumberReal;
            }


            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocMovementID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;


            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //Если есть параметр "TreeServerParam1", то изменить URL
            //if (GridServerParam1 != undefined) storeNomenTree.proxy.url = HTTP_DirNomensTree + "?" + GridServerParam1;

            //2. Combo
            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            //Store ComboGrid "Contractors"
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=2&DirContractor2TypeID2=4";
            //Store ComboGrid "Warehouses"
            var storeDirWarehousesGridFrom = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridFrom.setData([], false); storeDirWarehousesGridFrom.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            //Store ComboGrid "Warehouses" (для документа "DocMovements" показать все склады)
            var storeDirWarehousesGridTo = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridTo.setData([], false); storeDirWarehousesGridTo.proxy.url = HTTP_DirWarehouses + "?type=Grid&ListObjectID=33";
            //2.2. DirMovementDescriptions
            var storeDirMovementDescriptionsGrid = Ext.create("store.storeDirMovementDescriptionsGrid"); storeDirMovementDescriptionsGrid.setData([], false);
            //2.2. DirEmployeesGrid
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            //3. Табличная часть
            var storeDocMovementTabsGrid = Ext.create("store.storeDocMovementTabsGrid"); storeDocMovementTabsGrid.setData([], false);
            storeDocMovementTabsGrid.proxy.url = HTTP_DocMovementTabs + "?DocMovementID=" + IdcallModelData.DocMovementID;


            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);
            //storeRemPartiesGrid.load();


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocMovementTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirWarehousesGridFrom: storeDirWarehousesGridFrom,
                storeDirWarehousesGridTo: storeDirWarehousesGridTo,

                storeDirEmployeesGrid: storeDirEmployeesGrid,

                storeDirMovementDescriptionsGrid: storeDirMovementDescriptionsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel"
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGridFrom.load({ waitMsg: lanLoading });
                    storeDirWarehousesGridFrom.on('load', function () {
                        if (storeDirWarehousesGridFrom.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGridFrom.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGridTo.load({ waitMsg: lanLoading });
                        storeDirWarehousesGridTo.on('load', function () {
                            if (storeDirWarehousesGridTo.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGridTo.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirMovementDescriptionsGrid.proxy.url = HTTP_DirMovementDescriptions + "?type=Grid";
                            storeDirMovementDescriptionsGrid.load({ waitMsg: lanLoading });
                            storeDirMovementDescriptionsGrid.on('load', function () {

                                storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                                storeDirEmployeesGrid.on('load', function () {
                                    if (storeDirEmployeesGrid.UO_Loaded) return;
                                    storeDirEmployeesGrid.UO_Loaded = true;

                                    loadingMask.hide();
                                    
                                    if (New_Edit == 1) {

                                        //Если новая запись, то установить "по умолчанию"

                                        //Дата
                                        Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                        //Скидка
                                        //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                        //Сумма с Налогом
                                        Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                        //Наименование окна (сверху)
                                        widgetX.setTitle(widgetX.title + " № Новая");

                                        //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                        Ext.getCmp("btnRecord" + ObjectID).show();


                                        //Справочники
                                        //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID);
                                        //Ext.getCmp("DirVatValue" + ObjectID).setValue(0);


                                        //Склад и Организация привязанные к сотруднику
                                        //Склад
                                        if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                        else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                        //Организация
                                        if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                        else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                        //Остаток по Складу: Присваиваем Товару - Склад
                                        //if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                        //else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                        storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();

                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();

                                        //Разблокировка вызвавшего окна
                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                    }
                                    else if (New_Edit == 2 || New_Edit == 3) {

                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                                        //ArrList - значит
                                        if (!ArrList) {

                                            storeDocMovementTabsGrid.load({ waitMsg: lanLoading });
                                            storeDocMovementTabsGrid.on('load', function () {
                                                if (storeDocMovementTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                                storeDocMovementTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                                                //Если форма уже загружена выходим!
                                                if (widgetXForm.UO_Loaded) return;


                                                widgetXForm.load({
                                                    method: "GET",
                                                    timeout: varTimeOutDefault,
                                                    waitMsg: lanLoading,
                                                    url: HTTP_DocMovements + IdcallModelData.DocMovementID + "/?DocID=" + IdcallModelData.DocID,
                                                    success: function (form, action) {

                                                        widgetXForm.UO_Loaded = true;
                                                        //Фокус на открывшийся Виджет
                                                        widgetX.focus();

                                                        //Если Копия
                                                        if (New_Edit == 3) {
                                                            Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocMovementID" + ObjectID).setValue();
                                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                                        }
                                                        else {
                                                            //Наименование окна (сверху)
                                                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocMovementID" + ObjectID).getValue());

                                                            //Проведён или нет
                                                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                                                Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                            }
                                                            else {
                                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                                            }
                                                            Ext.getCmp("btnPrint" + ObjectID).show();
                                                        }


                                                        //!!! ОСТОРОЖНО !!! Нельзя менять параметры после загрузки!!!
                                                        /*
                                                        //Склад и Организация привязанные к сотруднику
                                                        //Склад
                                                        if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                                        else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                                        //Организация
                                                        if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                                        else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }
                                                        */

                                                        //Остаток по Складу: Присваиваем Товару - Склад
                                                        //if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                                        //else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                                        storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                                        //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                        Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                        //Разблокировка вызвавшего окна
                                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                    },
                                                    failure: function (form, action) {
                                                        //loadingMask.hide();
                                                        widgetX.close();
                                                        funPanelSubmitFailure(form, action);

                                                        //Фокус на открывшийся Виджет
                                                        widgetX.focus();

                                                        //Разблокировка вызвавшего окна
                                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                    }
                                                });


                                            });


                                        } //if(!ArrList)
                                            //Создать "На основании ..."
                                        else {

                                            //Переменные
                                            var formRec = ArrList[0];
                                            var gridRec = ArrList[1];
                                            //var locDirWarehouseID = ArrList[2];
                                            //Форма
                                            var form = widgetXForm.getForm();
                                            formRec.data.DirWarehouseIDFrom = formRec.data.DirWarehouseID;
                                            form.loadRecord(formRec);
                                            //Грид
                                            //storeDocMovementTabsGrid.load({ waitMsg: lanLoading });
                                            for (var i = 0; i < gridRec.data.length; i++) storeDocMovementTabsGrid.add(gridRec.data.items[i].data);
                                            /*{
                                                gridRec.data.items[i].data.Quantity = gridRec.data.items[i].data.Remnant;
                                                storeDocMovementTabsGrid.add(gridRec.data.items[i].data);
                                            }*/

                                            // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                                            
                                            widgetXForm.UO_Loaded = true;
                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();
                                            
                                            //Если Копия
                                            if (New_Edit == 3) {
                                                Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocMovementID" + ObjectID).setValue(null);
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            else {
                                                //Наименование окна (сверху)
                                                widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocMovementID" + ObjectID).getValue());
                                                //Проведён или нет
                                                if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                    Ext.Msg.alert(lanOrgName, txtMsg020);
                                                    Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                }
                                                else {
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                Ext.getCmp("btnPrint" + ObjectID).show();
                                            }


                                            //Дата
                                            Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                            //Причина: Брак
                                            Ext.getCmp("DescriptionMovement" + ObjectID).setValue("Брак");
                                            //Дата
                                            Ext.getCmp("Base" + ObjectID).setValue("На основании отчета по торговле, тип 'Брак'");
                                            //Склад и Организация привязанные к сотруднику
                                            //Склад
                                            //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(locDirWarehouseID);
                                            //Организация
                                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                            //Остаток по Складу: Присваиваем Товару - Склад
                                            //if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                            //else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                            storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                            //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                            Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                            //Разблокировка вызвавшего окна
                                            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                            //Прячим партии
                                            Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_TOP, true);
                                        }


                                    }

                                });
                            });
                        });
                    });
                });
            });


            //Убираем кнопки
            //Ext.getCmp("expandAll" + ObjectID).setVisible(false);
            //Ext.getCmp("collapseAll" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);

            break;
        }

            /* Перемещение: Редактирование Грида */

        case "viewDocMovementTabsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,

                storeDirCharColoursGrid: varStoreDirCharColoursGrid,
                storeDirCharMaterialsGrid: varStoreDirCharMaterialsGrid,
                storeDirCharNamesGrid: varStoreDirCharNamesGrid,
                storeDirCharSeasonsGrid: varStoreDirCharSeasonsGrid,
                storeDirCharSexesGrid: varStoreDirCharSexesGrid,
                storeDirCharSizesGrid: varStoreDirCharSizesGrid,
                storeDirCharStylesGrid: varStoreDirCharStylesGrid,
                storeDirCharTexturesGrid: varStoreDirCharTexturesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    //Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                        //UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            break;
        }


            /* Возврат */

        case "viewDocReturnVendorsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocReturnVendorID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=2&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocReturnVendorTabsGrid = Ext.create("store.storeDocReturnVendorTabsGrid"); storeDocReturnVendorTabsGrid.setData([], false); storeDocReturnVendorTabsGrid.proxy.url = HTTP_DocReturnVendorTabs + "?DocReturnVendorID=" + IdcallModelData.DocReturnVendorID;
            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocReturnVendorTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                        storeDirWarehousesGrid.on('load', function () {
                            if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirVatsGrid.load({ waitMsg: lanLoading });
                            storeDirVatsGrid.on('load', function () {
                                if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                loadingMask.hide();

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Скидка
                                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                    //Сумма с Налогом
                                    Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                    //Сума Налога
                                    Ext.getCmp("SumVATCurrency" + ObjectID).setValue(0);
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();

                                    //Справочники
                                    //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                                    //Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                                    Ext.getCmp("Payment" + ObjectID).setValue(0);
                                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменоц проведения прихода)


                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                                    else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                    //Для "остаток по складу": Присваиваем Товару - Склад
                                    if (varDirWarehouseIDEmpl == 0) { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
                                    else { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    storeDocReturnVendorTabsGrid.load({ waitMsg: lanLoading });
                                    storeDocReturnVendorTabsGrid.on('load', function () {
                                        if (storeDocReturnVendorTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                        storeDocReturnVendorTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                        //Если форма уже загружена выходим!
                                        if (widgetXForm.UO_Loaded) return;

                                        widgetXForm.load({
                                            method: "GET",
                                            timeout: varTimeOutDefault,
                                            waitMsg: lanLoading,
                                            url: HTTP_DocReturnVendors + IdcallModelData.DocReturnVendorID + "/?DocID=" + IdcallModelData.DocID,
                                            success: function (form, action) {

                                                widgetXForm.UO_Loaded = true;
                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Если Копия
                                                if (New_Edit == 3) {
                                                    Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocReturnVendorID" + ObjectID).setValue(null);
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                else {
                                                    //Наименование окна (сверху)
                                                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocReturnVendorID" + ObjectID).getValue());

                                                    //Проведён или нет
                                                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                        Ext.Msg.alert(lanOrgName, txtMsg020);
                                                        Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                    }
                                                    else {
                                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                                    }
                                                    //Кнопку "Принтер - делаем активной"
                                                    Ext.getCmp("btnPrint" + ObjectID).show();
                                                    //Кнопку "Платежи - делаем активной"
                                                    Ext.getCmp("btnGridPayment" + ObjectID).enable();
                                                }

                                                //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                //Остаток по Складу: Присваиваем Товару - Склад
                                                storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            },
                                            failure: function (form, action) {
                                                //loadingMask.hide();
                                                widgetX.close();
                                                funPanelSubmitFailure(form, action);

                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            }
                                        });

                                    });

                                }

                            });
                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Возврат: Редактирование Грида */

        case "viewDocReturnVendorTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false);
            storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        UO_GridRecord.data.PriceCurrency = UO_GridRecord.data.PriceCurrency;
                        UO_GridRecord.data.PriceVAT = UO_GridRecord.data.PriceVAT;

                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }


            /* Списание */

        case "viewDocActWriteOffsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocActWriteOffID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);

            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=3&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocActWriteOffTabsGrid = Ext.create("store.storeDocActWriteOffTabsGrid"); storeDocActWriteOffTabsGrid.setData([], false); storeDocActWriteOffTabsGrid.proxy.url = HTTP_DocActWriteOffTabs + "?DocActWriteOffID=" + IdcallModelData.DocActWriteOffID;
            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);
            //storeRemPartiesGrid.load();


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocActWriteOffTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                        storeDirWarehousesGrid.on('load', function () {
                            if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirVatsGrid.load({ waitMsg: lanLoading });
                            storeDirVatsGrid.on('load', function () {
                                if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                loadingMask.hide();

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Сумма с Налогом
                                    Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();


                                    //Справочники
                                    //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменоц проведения прихода)


                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
                                    else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
                                    else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

                                    //Для "остаток по складу": Присваиваем Товару - Склад
                                    if (varDirWarehouseIDEmpl == 0) { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
                                    else { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    storeDocActWriteOffTabsGrid.load({ waitMsg: lanLoading });
                                    storeDocActWriteOffTabsGrid.on('load', function () {
                                        if (storeDocActWriteOffTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                        storeDocActWriteOffTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                        //Если форма уже загружена выходим!
                                        if (widgetXForm.UO_Loaded) return;

                                        widgetXForm.load({
                                            method: "GET",
                                            timeout: varTimeOutDefault,
                                            waitMsg: lanLoading,
                                            url: HTTP_DocActWriteOffs + IdcallModelData.DocActWriteOffID + "/?DocID=" + IdcallModelData.DocID,
                                            success: function (form, action) {

                                                widgetXForm.UO_Loaded = true;
                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Если Копия
                                                if (New_Edit == 3) {
                                                    Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocActWriteOffID" + ObjectID).setValue(null);
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                else {
                                                    //Наименование окна (сверху)
                                                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocActWriteOffID" + ObjectID).getValue());

                                                    //Проведён или нет
                                                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                        Ext.Msg.alert(lanOrgName, txtMsg020);
                                                        Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                    }
                                                    else {
                                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                                    }
                                                    Ext.getCmp("btnPrint" + ObjectID).show();
                                                }

                                                //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                //Остаток по Складу: Присваиваем Товару - Склад
                                                storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            },
                                            failure: function (form, action) {
                                                //loadingMask.hide();
                                                widgetX.close();
                                                funPanelSubmitFailure(form, action);

                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            }
                                        });

                                    });

                                }

                            });
                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Списание: Редактирование Грида */

        case "viewDocActWriteOffTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false);
            storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        UO_GridRecord.data.PriceCurrency = UO_GridRecord.data.PriceCurrency;
                        UO_GridRecord.data.PriceVAT = UO_GridRecord.data.PriceVAT;

                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }


            /* Возврат от покупателя */

        case "viewDocReturnsCustomersEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocReturnsCustomerID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            //var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //2. Combo
            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false);
            storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=3&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocReturnsCustomerTabsGrid = Ext.create("store.storeDocReturnsCustomerTabsGrid"); storeDocReturnsCustomerTabsGrid.setData([], false); storeDocReturnsCustomerTabsGrid.proxy.url = HTTP_DocReturnsCustomerTabs + "?DocReturnsCustomerID=" + IdcallModelData.DocReturnsCustomerID;
            //4. Партии
            var storeRemPartyMinusesGrid = Ext.create("store.storeRemPartyMinusesGrid"); storeRemPartyMinusesGrid.setData([], false);


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                //storeNomenTree: storeNomenTree,
                storeGrid: storeDocReturnsCustomerTabsGrid,
                storeRemPartyMinusesGrid: storeRemPartyMinusesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel"
            //Ext.getCmp("gridPartyMinus_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            //Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid

            storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
            storeDirContractorsOrgGrid.on('load', function () {
                if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsGrid.load({ waitMsg: lanLoading });
                storeDirContractorsGrid.on('load', function () {
                    if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                    storeDirWarehousesGrid.on('load', function () {
                        if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirVatsGrid.load({ waitMsg: lanLoading });
                        storeDirVatsGrid.on('load', function () {
                            if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            loadingMask.hide();

                            if (New_Edit == 1) {

                                //Если новая запись, то установить "по умолчанию"

                                //Дата
                                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                //Скидка
                                //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                //Сумма с Налогом
                                Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                //Сума Налога
                                Ext.getCmp("SumVATCurrency" + ObjectID).setValue(0);
                                //Наименование окна (сверху)
                                widgetX.setTitle(widgetX.title + " № Новая");

                                //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                Ext.getCmp("btnHelds" + ObjectID).show();
                                Ext.getCmp("btnRecord" + ObjectID).show();


                                //Справочники
                                //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                                Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                                //Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                                Ext.getCmp("Payment" + ObjectID).setValue(0);


                                //Склад и Организация привязанные к сотруднику
                                //Склад
                                if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
                                else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
                                //Организация
                                if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
                                else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

                                //Остаток по Складу: Присваиваем Товару - Склад
                                //if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();}
                                //else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}

                                //Фокус на открывшийся Виджет
                                widgetX.focus();

                                //Разблокировка вызвавшего окна
                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                Ext.Msg.alert(lanOrgName, "Документ создаётся на основании Продажи. Выберите документ Продажа.");

                            }
                            else if (New_Edit == 2 || New_Edit == 3) {

                                storeDocReturnsCustomerTabsGrid.load({ waitMsg: lanLoading });
                                storeDocReturnsCustomerTabsGrid.on('load', function () {
                                    if (storeDocReturnsCustomerTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                    storeDocReturnsCustomerTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                    //Если форма уже загружена выходим!
                                    if (widgetXForm.UO_Loaded) return;

                                    widgetXForm.load({
                                        method: "GET",
                                        timeout: varTimeOutDefault,
                                        waitMsg: lanLoading,
                                        url: HTTP_DocReturnsCustomers + IdcallModelData.DocReturnsCustomerID + "/?DocID=" + IdcallModelData.DocID,
                                        success: function (form, action) {

                                            widgetXForm.UO_Loaded = true;
                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Если Копия
                                            if (New_Edit == 3) {
                                                Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocReturnsCustomerID" + ObjectID).setValue(null);
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            else {
                                                //Наименование окна (сверху)
                                                widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocReturnsCustomerID" + ObjectID).getValue());

                                                //Проведён или нет
                                                if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                    Ext.Msg.alert(lanOrgName, txtMsg020);
                                                    Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                }
                                                else {
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                //Кнопку "Печать - делаем активной"
                                                Ext.getCmp("btnPrint" + ObjectID).show();
                                                //Кнопку "Платежи - делаем активной"
                                                Ext.getCmp("btnGridPayment" + ObjectID).enable();
                                            }

                                            //Остаток по Складу: Присваиваем Товару - Склад
                                            // storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                            //Разблокировка вызвавшего окна
                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            //Загружаем данные в "gridPartyMinus_" по "DocSaleID"
                                            var storeGrid = Ext.getCmp("gridPartyMinus_" + ObjectID).getStore();
                                            storeGrid.proxy.url = HTTP_RemPartyMinuses + "?DocSaleID=" + Ext.getCmp("DocSaleID" + ObjectID).getValue();
                                            storeGrid.load();
                                        },
                                        failure: function (form, action) {
                                            //loadingMask.hide();
                                            widgetX.close();
                                            funPanelSubmitFailure(form, action);

                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Разблокировка вызвавшего окна
                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                        }
                                    });

                                });

                            }

                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Возврат от покупателя: Редактирование Грида */

        case "viewDocReturnsCustomerTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false);
            storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridPartyMinus_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);
                    Ext.getCmp("Remnant" + ObjectID).setValue(IdcallModelData.Quantity); //что бы нельзя было ввести больше того что продали!

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }


            /* Акт выполненных работ */

        case "viewDocActOnWorksEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocActOnWorkID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //Если есть параметр "TreeServerParam1", то изменить URL
            //if (GridServerParam1 != undefined) storeNomenTree.proxy.url = HTTP_DirNomensTree + "?" + GridServerParam1;


            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=3&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocActOnWorkTabsGrid = Ext.create("store.storeDocActOnWorkTabsGrid"); storeDocActOnWorkTabsGrid.setData([], false); storeDocActOnWorkTabsGrid.proxy.url = HTTP_DocActOnWorkTabs + "?DocActOnWorkID=" + IdcallModelData.DocActOnWorkID;
            //4. Партии
            //var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);
            //3. Табличная часть
            var storeDocPurchTabsGrid = Ext.create("store.storeDocPurchTabsGrid"); storeDocPurchTabsGrid.setData([], false);
            storeDocPurchTabsGrid.proxy.url = HTTP_DocPurchTabs + "?DocPurchID=" + IdcallModelData.DocPurchID;
            //storeDocPurchTabsGrid.load();


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocActOnWorkTabsGrid,
                //storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                        storeDirWarehousesGrid.on('load', function () {
                            if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirVatsGrid.load({ waitMsg: lanLoading });
                            storeDirVatsGrid.on('load', function () {
                                if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                loadingMask.hide();

                                //Тип цен
                                Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Скидка
                                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                    //Сумма с Налогом
                                    Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                    //Сума Налога
                                    Ext.getCmp("SumVATCurrency" + ObjectID).setValue(0);
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    //Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();


                                    //Справочники
                                    //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                                    //Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                                    Ext.getCmp("Payment" + ObjectID).setValue(0);


                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
                                    else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
                                    else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

                                    //Для "остаток по складу": Присваиваем Товару - Склад
                                    if (varDirWarehouseIDEmpl == 0) { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
                                    else { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    storeDocActOnWorkTabsGrid.load({ waitMsg: lanLoading });
                                    storeDocActOnWorkTabsGrid.on('load', function () {
                                        if (storeDocActOnWorkTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                        storeDocActOnWorkTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                        //Если форма уже загружена выходим!
                                        if (widgetXForm.UO_Loaded) return;

                                        widgetXForm.load({
                                            method: "GET",
                                            timeout: varTimeOutDefault,
                                            waitMsg: lanLoading,
                                            url: HTTP_DocActOnWorks + IdcallModelData.DocActOnWorkID + "/?DocID=" + IdcallModelData.DocID,
                                            success: function (form, action) {

                                                widgetXForm.UO_Loaded = true;
                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Если Копия
                                                if (New_Edit == 3) {
                                                    Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocActOnWorkID" + ObjectID).setValue(null);
                                                }
                                                else {
                                                    //Наименование окна (сверху)
                                                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocActOnWorkID" + ObjectID).getValue());
                                                    //Кнопку "Печать - делаем активной"
                                                    Ext.getCmp("btnPrint" + ObjectID).show();
                                                    //Кнопку "Платежи - делаем активной"
                                                    Ext.getCmp("btnGridPayment" + ObjectID).enable();
                                                }

                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                                //Остаток по Складу: Присваиваем Товару - Склад
                                                storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            },
                                            failure: function (form, action) {
                                                //loadingMask.hide();
                                                widgetX.close();
                                                funPanelSubmitFailure(form, action);

                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            }
                                        });

                                    });

                                }

                            });
                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Акт выполненных работ: Редактирование Грида */

        case "viewDocActOnWorkTabsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,

                storeDirCurrenciesGrid: storeDirCurrenciesGrid
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                if (New_Edit == 1) {
                    //Если новая запись, то установить "по умолчанию"
                    Ext.getCmp("btnDel" + ObjectID).setVisible(false);
                    //Если наценки отрицательные, то ставим их из Настроек
                    funMarkupSet(ObjectID);
                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    //Если редактировать, то: Загрузка данных в Форму "widgetXPanel"
                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                    if (UO_GridSave) {
                        varPriceChange_ReadOnly = true; //Запретить редактировать цены
                        //Форма
                        var form = widgetXForm.getForm();

                        if (GridTree) {
                            //Редактирование (загрузить из грида)
                            form.loadRecord(UO_GridRecord);
                        }
                        else {
                            //Новый товар
                            //Может возникнуть ситуация, когда не выбран товар
                            if (UO_GridRecord != undefined) {
                                Ext.getCmp("DirNomenID" + ObjectID).setValue(UO_GridRecord.data.id);
                                Ext.getCmp("DirNomenName" + ObjectID).setValue(UO_GridRecord.data.text);
                                Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());

                                //Пробегаемся по всем партиям и ищим с последней датой
                                //Если не находим, то ставим всё "по нулям"
                                //1. Грид Party *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                                var id = Ext.getCmp(UO_idCall).UO_id;
                                //var PanelParty = Ext.getCmp("gridParty_" + id).store.data.items;

                                //2. Запрос на сервер за ценой из истории
                                Ext.Ajax.request({
                                    timeout: varTimeOutDefault, waitMsg: lanUpload, method: 'GET',
                                    url: HTTP_DirNomenHistories + UO_GridRecord.data.id + "/?Action=1",
                                    success: function (result) {
                                        var sData = Ext.decode(result.responseText);
                                        if (sData.success == false) {
                                            //Еси нет данных на Сервере, то проставляем по умолчанию:
                                            Ext.getCmp("PriceCurrency" + ObjectID).setValue(0);
                                            Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
                                            Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                                            Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                                            Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                                            //Если наценки отрицательные, то ставим их из Настроек
                                            //funMarkupSet(ObjectID, sData.data + "<br />");
                                            return;
                                        }
                                        else {
                                            //Создаём модель
                                            var UO_GridRecord = Ext.create("PartionnyAccount.model.Sklad/Object/Rem/RemParties/modelRemPartiesGrid");
                                            UO_GridRecord.data = sData.data.Result;
                                            UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));

                                            //Пишем в модель данные
                                            form.loadRecord(UO_GridRecord);
                                            //funMarkupSet(ObjectID); //Если наценки отрицательные, то ставим их из Настроек
                                        }
                                    },
                                    failure: function (form, action) { funPanelSubmitFailure(form, action); }
                                });

                                form.loadRecord(UO_GridRecord);
                                Ext.getCmp("Quantity" + ObjectID).setValue(1);
                            }
                        }
                        form.UO_Loaded = true;
                        varPriceChange_ReadOnly = false; //Разрешить редактировать цены
                    }
                }
            }); //storeDirCurrenciesGrid

            break;
        }


            /* Счет */

        case "viewDocAccountsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocAccountID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //Если есть параметр "TreeServerParam1", то изменить URL
            //if (GridServerParam1 != undefined) storeNomenTree.proxy.url = HTTP_DirNomensTree + "?" + GridServerParam1;

            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=3&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocAccountTabsGrid = Ext.create("store.storeDocAccountTabsGrid"); storeDocAccountTabsGrid.setData([], false); storeDocAccountTabsGrid.proxy.url = HTTP_DocAccountTabs + "?DocAccountID=" + IdcallModelData.DocAccountID;
            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocAccountTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                        storeDirWarehousesGrid.on('load', function () {
                            if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirVatsGrid.load({ waitMsg: lanLoading });
                            storeDirVatsGrid.on('load', function () {
                                if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                loadingMask.hide();


                                //Тип цен
                                Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Скидка
                                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                    //Сумма с Налогом
                                    Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                    //Сума Налога
                                    Ext.getCmp("SumVATCurrency" + ObjectID).setValue(0);
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    //Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();


                                    //Справочники
                                    //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                                    //Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                                    Ext.getCmp("Payment" + ObjectID).setValue(0);
                                    //Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)


                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
                                    else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
                                    else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

                                    //Для "остаток по складу": Присваиваем Товару - Склад
                                    if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();}
                                    else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}

                                    Ext.getCmp("Reserve" + ObjectID).setValue(true);

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    storeDocAccountTabsGrid.load({ waitMsg: lanLoading });
                                    storeDocAccountTabsGrid.on('load', function () {
                                        if (storeDocAccountTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                        storeDocAccountTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                        //Если форма уже загружена выходим!
                                        if (widgetXForm.UO_Loaded) return;

                                        widgetXForm.load({
                                            method: "GET",
                                            timeout: varTimeOutDefault,
                                            waitMsg: lanLoading,
                                            url: HTTP_DocAccounts + IdcallModelData.DocAccountID + "/?DocID=" + IdcallModelData.DocID,
                                            success: function (form, action) {

                                                widgetXForm.UO_Loaded = true;
                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Если Копия
                                                if (New_Edit == 3) {
                                                    Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocAccountID" + ObjectID).setValue(null);
                                                }
                                                else {
                                                    //Наименование окна (сверху)
                                                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocAccountID" + ObjectID).getValue());

                                                    Ext.getCmp("btnPrint" + ObjectID).show();
                                                    //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                    //Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                    //Кнопку "Платежи - делаем активной"
                                                    //Ext.getCmp("btnGridPayment" + ObjectID).enable();
                                                }

                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                                //Остаток по Складу: Присваиваем Товару - Склад
                                                storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();

                                                Ext.getCmp("btnOnBasisOfDoc" + ObjectID).enable(true);

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            },
                                            failure: function (form, action) {
                                                //loadingMask.hide();
                                                widgetX.close();
                                                funPanelSubmitFailure(form, action);

                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            }
                                        });

                                    });

                                }

                            });
                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Счет: Редактирование Грида */

        case "viewDocAccountTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false);
            storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                        UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }


            /* Инвентаризация */

        case "viewDocInventoriesEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocInventoryID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);

            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=3&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocInventoryTabsGrid = Ext.create("store.storeDocInventoryTabsGrid"); storeDocInventoryTabsGrid.setData([], false); storeDocInventoryTabsGrid.proxy.url = HTTP_DocInventoryTabs + "?DocInventoryID=" + IdcallModelData.DocInventoryID;
            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);
            //storeRemPartiesGrid.load();


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocInventoryTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                        storeDirWarehousesGrid.on('load', function () {
                            if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirVatsGrid.load({ waitMsg: lanLoading });
                            storeDirVatsGrid.on('load', function () {
                                if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                loadingMask.hide();

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Сумма с Налогом
                                    Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();


                                    //Справочники
                                    //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменоц проведения прихода)


                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0){ Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
                                    else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
                                    else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

                                    //Для "остаток по складу": Присваиваем Товару - Склад
                                    if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();}
                                    else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    storeDocInventoryTabsGrid.load({ waitMsg: lanLoading });
                                    storeDocInventoryTabsGrid.on('load', function () {
                                        if (storeDocInventoryTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                        storeDocInventoryTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                        //Если форма уже загружена выходим!
                                        if (widgetXForm.UO_Loaded) return;

                                        widgetXForm.load({
                                            method: "GET",
                                            timeout: varTimeOutDefault,
                                            waitMsg: lanLoading,
                                            url: HTTP_DocInventories + IdcallModelData.DocInventoryID + "/?DocID=" + IdcallModelData.DocID,
                                            success: function (form, action) {

                                                widgetXForm.UO_Loaded = true;
                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Если Копия
                                                if (New_Edit == 3) {
                                                    Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocInventoryID" + ObjectID).setValue(null);
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                else {
                                                    //Наименование окна (сверху)
                                                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocInventoryID" + ObjectID).getValue());

                                                    //Проведён или нет
                                                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                        Ext.Msg.alert(lanOrgName, txtMsg020);
                                                        Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                    }
                                                    else {
                                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                                    }
                                                    Ext.getCmp("btnPrint" + ObjectID).show();
                                                }

                                                //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                //Остаток по Складу: Присваиваем Товару - Склад
                                                storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            },
                                            failure: function (form, action) {
                                                //loadingMask.hide();
                                                widgetX.close();
                                                funPanelSubmitFailure(form, action);

                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            }
                                        });

                                    });

                                }

                            });
                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Инвентаризация: Редактирование Грида */

        case "viewDocInventoryTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false);
            storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                
                if (New_Edit == 1) {
                    
                    //Не используется
                    /*
                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        UO_GridRecord.data.PriceCurrency = UO_GridRecord.data.PriceCurrency;
                        UO_GridRecord.data.PriceVAT = UO_GridRecord.data.PriceVAT;

                        UO_GridRecord.data.Quantity_WriteOff = 0;
                        UO_GridRecord.data.Quantity_Purch = 1;
                        form.loadRecord(UO_GridRecord);
                    }
                    */

                }
                else if (New_Edit == 2 || New_Edit == 3) {

                    if (GridTree) {
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        //Может возникнуть ситуация, когда не выбран товар
                        if (UO_GridRecord != undefined) {
                            Ext.getCmp("DirNomenID" + ObjectID).setValue(UO_GridRecord.data.id);
                            Ext.getCmp("DirNomenName" + ObjectID).setValue(UO_GridRecord.data.text);


                            //Пробегаемся по всем партиям и ищим с последней датой
                            //Если не находим, то ставим всё "по нулям"
                            //1. Грид Party *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                            var id = Ext.getCmp(UO_idCall).UO_id;

                            //Выбранная партия
                            var IdcallModelData = Ext.getCmp("gridParty_" + id).getSelectionModel().getSelection();

                            //1. Если не выбрана партия товара
                            if (IdcallModelData.length == 0) {

                                var PanelParty = Ext.getCmp("gridParty_" + id).store.data.items;

                                //2. Выбираем данные из партии
                                if (PanelParty.length > 0) {
                                    //2.1. Если есть Партии, то выбираем самую последнюю
                                    UO_GridRecord = PanelParty[0];
                                    for (var i = PanelParty.length - 1; i > 0; i--) { //for (var i = 1; i < PanelParty.length; i++) {
                                        if (PanelParty[i].data.DocDate > UO_GridRecord.data.DocDate) UO_GridRecord = PanelParty[i];
                                    }
                                }
                                else {
                                    //2.2. Если нет Партии, то делаем запрос на Сервер за Партией, которые уже проданы,
                                    //     если на Сервере тоже нет данных выдаём сообщение
                                    fun_viewDocPurchTabsEdit_RequestPrice(form, UO_GridRecord, ObjectID);

                                }
                            }

                                //2. Если выбрана партия товара, то её и берём на основу!
                            else {
                                UO_GridRecord = IdcallModelData[0]
                            }

                            form.loadRecord(UO_GridRecord);
                            Ext.getCmp("Quantity_WriteOff" + ObjectID).setValue(0);
                            Ext.getCmp("Quantity_Purch" + ObjectID).setValue(1);
                            Ext.getCmp("SUMPurchPriceVATCurrency" + ObjectID).setValue(0);
                            //Мин.остаток
                            Ext.getCmp("DirNomenMinimumBalance" + ObjectID).setValue(varDirNomenMinimumBalance);



                            //Поставщик
                            /*
                            var locDirContractorID = Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id).getRawValue();
                            var comboBox = Ext.getCmp("DirCharStyleID" + ObjectID);
                            var store = comboBox.store;
                            var locResult = store.findExact("DirCharStyleName", locDirContractorID);
                            Ext.getCmp("DirCharStyleID" + ObjectID).setValue(store.getAt(locResult));
                            */
                            if (Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id)) {
                                var locDirContractorID = Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id).getRawValue();
                                var comboBox = Ext.getCmp("DirContractorID" + ObjectID);
                                var store = comboBox.store;
                                var locResult = store.findExact("DirContractorName", locDirContractorID);
                                Ext.getCmp("DirContractorID" + ObjectID).setValue(store.getAt(locResult));
                            }


                        }
                    }


                    //Если отсутствет поле "RemPartyID", то списание всегда == "0"
                    if (isNaN(parseInt(Ext.getCmp("RemPartyID" + ObjectID).getValue())) || parseInt(Ext.getCmp("RemPartyID" + ObjectID).getValue()) == 0) {
                        Ext.getCmp("Quantity_WriteOff" + ObjectID).setValue(0);
                        Ext.getCmp("Quantity_WriteOff" + ObjectID).disable();
                    }

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }


            //Сервис *** *** ***

            /* Сервис - Приёмка */

        case "viewDocServicePurchesEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData; // = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            if (New_Edit > 1) {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                    return;
                }
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName; // + "_" + IdcallModelData.DocServicePurchID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //2. Combo
            //Store Combo "ContractorsOrg"Warehouses
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            //var storeDirServiceContractorsGrid = Ext.create("store.storeDirServiceContractorsGrid"); storeDirServiceContractorsGrid.setData([], false); storeDirServiceContractorsGrid.proxy.url = HTTP_DirServiceContractors + "?type=Grid";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeDirServiceComplectsGrid = Ext.create("store.storeDirServiceComplectsGrid"); storeDirServiceComplectsGrid.setData([], false); storeDirServiceComplectsGrid.proxy.url = HTTP_DirServiceComplects + "?type=Grid";
            var storeDirServiceProblemsGrid = Ext.create("store.storeDirServiceProblemsGrid"); storeDirServiceProblemsGrid.setData([], false); storeDirServiceProblemsGrid.proxy.url = HTTP_DirServiceProblems + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirServiceNomenTree: storeDirServiceNomenTree,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                //storeDirServiceContractorsGrid: storeDirServiceContractorsGrid,
                varStoreDirServiceContractorsGrid: varStoreDirServiceContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeDirServiceComplectsGrid: storeDirServiceComplectsGrid,
                storeDirServiceProblemsGrid: storeDirServiceProblemsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Ext.getCmp("SearchType" + ObjectID).setValue(1);
            Ext.getCmp("DirServiceNomenPatchFull" + ObjectID).setText("<b>...</b>", false);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();
            
            //Событие на загрузку в Grid
            //storeDirServiceNomenTree.load({ waitMsg: lanLoading });
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return;
                storeDirServiceNomenTree.UO_Loaded = true;

                storeDirServiceComplectsGrid.load({ waitMsg: lanLoading });
                storeDirServiceComplectsGrid.on('load', function () {
                    if (storeDirServiceComplectsGrid.UO_Loaded) return;
                    storeDirServiceComplectsGrid.UO_Loaded = true;

                    storeDirServiceProblemsGrid.load({ waitMsg: lanLoading });
                    storeDirServiceProblemsGrid.on('load', function () {
                        if (storeDirServiceProblemsGrid.UO_Loaded) return;
                        storeDirServiceProblemsGrid.UO_Loaded = true;

                        loadingMask.hide();
                        
                        if (New_Edit == 1) {
                            
                            //Если новая запись, то установить "по умолчанию"

                            //Даты
                            Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                            var dat = new Date(); dat.setDate(dat.getDate() + varReadinessDay);
                            Ext.getCmp("DateDone" + ObjectID).setValue(dat);
                            //Наименование окна (сверху)
                            widgetX.setTitle(widgetX.title + " № Новая");


                            Ext.getCmp("DirServiceContractorPhone" + ObjectID).setValue(varPhoneNumberBegin);
                            Ext.getCmp("ComponentDevice" + ObjectID).setValue(true);
                            Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                            Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                            Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                            //Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
                            //Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                            //Ext.getCmp("PrepaymentSum" + ObjectID).setValue(0);
                            Ext.getCmp("DirEmployeeIDMaster" + ObjectID).setValue(varDirEmployeeID);
                            //Справочники
                            //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                            //Склад и Организация привязанные к сотруднику
                            //Склад
                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                            //Организация
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }
                            //К-во по нулям
                            Ext.getCmp("QuantityOk" + ObjectID).setValue(0);
                            Ext.getCmp("QuantityFail" + ObjectID).setValue(0);
                            Ext.getCmp("QuantityCount" + ObjectID).setValue(0);
                            //Предоплата
                            Ext.getCmp("PrepaymentSum" + ObjectID).setValue(0);

                            //Фокус на открывшийся Виджет
                            widgetX.focus();
                            //Разблокировка вызвавшего окна
                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                        }
                        else if (New_Edit == 2 || New_Edit == 3) {

                            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                            //Если форма уже загружена выходим!
                            if (widgetXForm.UO_Loaded) return;

                            widgetXForm.load({
                                method: "GET",
                                timeout: varTimeOutDefault,
                                waitMsg: lanLoading,
                                url: HTTP_DocServicePurches + IdcallModelData.DocServicePurchID + "/?DocID=" + IdcallModelData.DocID,
                                success: function (form, action) {

                                    widgetXForm.UO_Loaded = true;
                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Если Копия
                                    if (New_Edit == 3) {
                                        Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocServicePurchID" + ObjectID).setValue(null);
                                    }
                                    else {
                                        //Наименование окна (сверху)
                                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocServicePurchID" + ObjectID).getValue());
                                    }

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                },
                                failure: function (form, action) {
                                    //loadingMask.hide();
                                    widgetX.close();
                                    funPanelSubmitFailure(form, action);

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                }
                            });

                        }

                    });
                });
            });

            /*
            storeDirServiceContractorsGrid.load({ waitMsg: lanLoading });
            storeDirServiceContractorsGrid.on('load', function () {
                if (storeDirServiceContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
            });
            */


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Сервис - мастерская */

        case "viewDocServiceWorkshopsEdit": {
            
            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName; // + "_" + IdcallModelData.DocServicePurchID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            var storeDocServicePurch1TabsGrid = Ext.create("store.storeDocServicePurch1TabsGrid"); storeDocServicePurch1TabsGrid.setData([], false); storeDocServicePurch1TabsGrid.proxy.url = HTTP_DocServicePurch1Tabs + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
            var storeDocServicePurch2TabsGrid = Ext.create("store.storeDocServicePurch2TabsGrid"); storeDocServicePurch2TabsGrid.setData([], false); storeDocServicePurch2TabsGrid.proxy.url = HTTP_DocServicePurch2Tabs + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirServiceStatusesGrid: varStoreDirServiceStatusesGrid,
                storeDocServicePurch1TabsGrid: storeDocServicePurch1TabsGrid,
                storeDocServicePurch2TabsGrid: storeDocServicePurch2TabsGrid,
                storeDirEmployeesGrid: storeDirEmployeesGrid
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();


            storeDirEmployeesGrid.load({ waitMsg: lanLoading });
            storeDirEmployeesGrid.on('load', function () {
                if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"

                loadingMask.hide();

                if (New_Edit == 1) {

                    //Если новая запись, то установить "по умолчанию"

                    //Дата
                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " № Новая");

                    Ext.getCmp("ComponentDevice" + ObjectID).setValue(true);
                    Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                    Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                    Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                    Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
                    //Справочники
                    Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                    //Склад и Организация привязанные к сотруднику
                    //Склад
                    if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
                    else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
                    //Организация
                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                    //Фокус на открывшийся Виджет
                    widgetX.focus();

                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                }
                else if (New_Edit == 2 || New_Edit == 3) {

                    storeDocServicePurch1TabsGrid.load({ waitMsg: lanLoading });
                    storeDocServicePurch1TabsGrid.on('load', function () {
                        if (storeDocServicePurch1TabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDocServicePurch1TabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"

                        storeDocServicePurch2TabsGrid.load({ waitMsg: lanLoading });
                        storeDocServicePurch2TabsGrid.on('load', function () {
                            if (storeDocServicePurch2TabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDocServicePurch2TabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"

                            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                            //Если форма уже загружена выходим!
                            if (widgetXForm.UO_Loaded) return;

                            widgetXForm.load({
                                method: "GET",
                                timeout: varTimeOutDefault,
                                waitMsg: lanLoading,
                                url: HTTP_DocServicePurches + IdcallModelData.DocServicePurchID + "/?DocID=" + IdcallModelData.DocID,
                                success: function (form, action) {

                                    widgetXForm.UO_Loaded = true;
                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Если Копия
                                    if (New_Edit == 3) {
                                        Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocServicePurchID" + ObjectID).setValue(null);
                                    }
                                    else {
                                        //Наименование окна (сверху)
                                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocServicePurchID" + ObjectID).getValue());
                                    }

                                    controllerDocServiceWorkshopsEdit_RecalculationSums(ObjectID);
                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                },
                                failure: function (form, action) {
                                    //loadingMask.hide();
                                    widgetX.close();
                                    funPanelSubmitFailure(form, action);

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                }
                            });


                        });
                    });

                }

            });

            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Сервис - Выдача */

        case "viewDocServiceOutputsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                return;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocServicePurchID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);



            if (New_Edit == 1) {

                //Если новая запись, то установить "по умолчанию"

                //Дата
                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                //Наименование окна (сверху)
                widgetX.setTitle(widgetX.title + " № Новая");

                Ext.getCmp("ComponentDevice" + ObjectID).setValue(true);
                Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
                //Справочники
                Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                //Склад и Организация привязанные к сотруднику
                //Склад
                if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
                else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
                //Организация
                if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
                else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

                //Фокус на открывшийся Виджет
                widgetX.focus();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

            }
            else if (New_Edit == 2 || New_Edit == 3) {

                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Если форма уже загружена выходим!
                if (widgetXForm.UO_Loaded) return;

                widgetXForm.load({
                    method: "GET",
                    timeout: varTimeOutDefault,
                    waitMsg: lanLoading,
                    url: HTTP_DocServicePurches + IdcallModelData.DocServicePurchID + "/?DocID=" + IdcallModelData.DocID,
                    success: function (form, action) {

                        widgetXForm.UO_Loaded = true;
                        //Фокус на открывшийся Виджет
                        widgetX.focus();

                        //Если Копия
                        if (New_Edit == 3) {
                            Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocServicePurchID" + ObjectID).setValue(null);
                        }
                        else {
                            //Наименование окна (сверху)
                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocServicePurchID" + ObjectID).getValue());
                        }

                        //Разблокировка вызвавшего окна
                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                    },
                    failure: function (form, action) {
                        //loadingMask.hide();
                        widgetX.close();
                        funPanelSubmitFailure(form, action);

                        //Фокус на открывшийся Виджет
                        widgetX.focus();

                        //Разблокировка вызвавшего окна
                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                    }
                });

            }


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Сервис - Смена мастера Администратором точки */

        case "viewDocServiceMasterSelect": {

            //Открыть это окно, только при условии, что это Админ точки!
            if (!varIsAdmin) {
                Ext.Msg.alert(lanOrgName, "Вы не являетесь Админом этой точки! Выбор сотрудника запрещён!"); return;
            }

            /*if (!varIsAdmin && varRightDocServicePurchesExtraditionCheck) {
                Ext.Msg.alert(lanOrgName, "Вы не являетесь Админом этой точки и у Вас отключена возможность выбирать сотрудника!"); return;
            }*/

            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid&DirWarehouseID=" + varDirWarehouseID + "&DeletedRecordsShow=false"; //&ForEmployee=1

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
            });

            //ObjectShow(widgetX);

            widgetX.border = true;
            widgetX.center();
            widgetX.show();

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeDirEmployeesGrid.load({ waitMsg: lanLoading });
            storeDirEmployeesGrid.on('load', function () {
                if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

        /* Сервис - Скидка при выдачи */

        case "viewDocServiceWorkshopsDiscount": {

            //Открыть это окно, только при условии, что это Админ точки!
            /*if (!varIsAdmin) {
                Ext.Msg.alert(lanOrgName, "Вы не являетесь Админом этой точки! Выбор сотрудника запрещён!"); return;
            }*/

            /*if (!varIsAdmin && varRightDocServicePurchesExtraditionCheck) {
                Ext.Msg.alert(lanOrgName, "Вы не являетесь Админом этой точки и у Вас отключена возможность выдавать аппараты!"); return;
            }*/

            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid&DirWarehouseID=" + varDirWarehouseID + "&DeletedRecordsShow=false"; //&ForEmployee=1

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                UO_Param_id: UO_Param_id, //aButton
                UO_Param_fn: UO_Param_fn, //controllerDocServiceWorkshops_ChangeStatus_Request_Union(aButton)

                storeDirEmployeesGrid: storeDirEmployeesGrid,
            });

            //ObjectShow(widgetX);

            widgetX.border = true;
            widgetX.center();
            widgetX.show();

            
            //Проставляем значения
            var
                SumTotal2a = Ext.getCmp("SumTotal2a" + UO_Param_id.UO_id).getValue(),
                SumDocServicePurch1Tabs = Ext.getCmp("SumDocServicePurch1Tabs" + UO_Param_id.UO_id).getValue(),
                SumDocServicePurch2Tabs = Ext.getCmp("SumDocServicePurch2Tabs" + UO_Param_id.UO_id).getValue();

            Ext.getCmp("SumTotal2a" + ObjectID).setValue(SumTotal2a);
            Ext.getCmp("SumDocServicePurch1Tabs" + ObjectID).setValue(SumDocServicePurch1Tabs);
            Ext.getCmp("SumDocServicePurch2Tabs" + ObjectID).setValue(SumDocServicePurch2Tabs);
            
            Ext.getCmp("labelSumTotal2a" + ObjectID).setText(
                "К оплате " + Ext.getCmp("SumTotal2a" + UO_Param_id.UO_id).getValue() + " руб" // + "   (" + SumDocServicePurch1Tabs + " + " + SumDocServicePurch2Tabs + ")"
            );
            Ext.getCmp("labelSumTotal2a" + ObjectID).setStyle({ "color": "green", "font-weight": "600" });

            Ext.getCmp("DiscountX" + ObjectID).setValue(0); Ext.getCmp("DiscountX" + ObjectID).maxLength = parseFloat(SumDocServicePurch1Tabs);
            Ext.getCmp("labelDiscountX" + ObjectID).setText(" " + SumDocServicePurch1Tabs );

            Ext.getCmp("DiscountY" + ObjectID).setValue(0); Ext.getCmp("DiscountY" + ObjectID).maxLength = parseFloat(SumDocServicePurch1Tabs);
            Ext.getCmp("labelDiscountY" + ObjectID).setText( " " + SumDocServicePurch2Tabs );


            //Если по умолчанию уже выбран тип платежа
            if (parseInt(Ext.getCmp("DirPaymentTypeID" + UO_Param_id.UO_id).getValue()) == 1) {
                Ext.getCmp("btnDirPaymentTypeID2" + ObjectID).disabled(true);
            }
            else if (parseInt(Ext.getCmp("DirPaymentTypeID" + UO_Param_id.UO_id).getValue()) == 2) {
                Ext.getCmp("btnDirPaymentTypeID1" + ObjectID).disabled(true);
            }

            Ext.getCmp("label2" + ObjectID).setStyle({ "color": "green", "font-weight": "600" });


            break;
        }

        case "viewDocServiceMovsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = [0];
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection();
                if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                    return;
                }
            }
            else {
                IdcallModelData.DocServiceMovID = 0;
            }

            //Если Логистика
            if (IdcallModelData.DocServiceMovID == undefined && IdcallModelData.LogisticID > 0) {
                IdcallModelData.DocServiceMovID = IdcallModelData.LogisticID
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocServiceMovID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //1. Store Grid
            //var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //Если есть параметр "TreeServerParam1", то изменить URL
            //if (GridServerParam1 != undefined) storeDirServiceNomenTree.proxy.url = HTTP_DirNomensTree + "?" + GridServerParam1;

            //2. Combo
            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            //Store ComboGrid "Contractors"
            //var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=2&DirContractor2TypeID2=4";
            //Store ComboGrid "Warehouses"
            var storeDirWarehousesGridFrom = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridFrom.setData([], false); storeDirWarehousesGridFrom.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            //Store ComboGrid "Warehouses" (для документа "DocServiceMovs" показать все склады)
            var storeDirWarehousesGridTo = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridTo.setData([], false); storeDirWarehousesGridTo.proxy.url = HTTP_DirWarehouses + "?type=Grid&ListObjectID=33";
            //2.2. DirMovementDescriptions
            var storeDirMovementDescriptionsGrid = Ext.create("store.storeDirMovementDescriptionsGrid"); storeDirMovementDescriptionsGrid.setData([], false);
            //2.2. DirEmployeesGrid
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            //3. Табличная часть
            var storeDocServiceMovTabsGrid = Ext.create("store.storeDocServiceMovTabsGrid"); storeDocServiceMovTabsGrid.setData([], false);
            storeDocServiceMovTabsGrid.proxy.url = HTTP_DocServiceMovTabs + "?DocServiceMovID=" + IdcallModelData.DocServiceMovID;
            //4. Партии
            //var storeRem2PartiesGrid = Ext.create("store.storeRem2PartiesGrid"); storeRem2PartiesGrid.setData([], false);
            var storeDocServicePurchesGrid = Ext.create("store.storeDocServicePurchesGrid"); storeDocServicePurchesGrid.setData([], false);


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                //storeDirServiceNomenTree: storeDirServiceNomenTree,
                storeGrid: storeDocServiceMovTabsGrid,

                storeDocServicePurchesGrid: storeDocServicePurchesGrid,
                //storeRem2PartiesGrid: storeRem2PartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirWarehousesGridFrom: storeDirWarehousesGridFrom,
                storeDirWarehousesGridTo: storeDirWarehousesGridTo,

                storeDirEmployeesGrid: storeDirEmployeesGrid,

                storeDirMovementDescriptionsGrid: storeDirMovementDescriptionsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel"
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            /*
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
                */

            storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
            storeDirContractorsOrgGrid.on('load', function () {
                if (storeDirContractorsOrgGrid.UO_Loaded) return;
                storeDirContractorsOrgGrid.UO_Loaded = true;

                storeDirWarehousesGridFrom.load({ waitMsg: lanLoading });
                storeDirWarehousesGridFrom.on('load', function () {
                    if (storeDirWarehousesGridFrom.UO_Loaded) return;
                    storeDirWarehousesGridFrom.UO_Loaded = true;

                    storeDirWarehousesGridTo.load({ waitMsg: lanLoading });
                    storeDirWarehousesGridTo.on('load', function () {
                        if (storeDirWarehousesGridTo.UO_Loaded) return;
                        storeDirWarehousesGridTo.UO_Loaded = true;

                        storeDirMovementDescriptionsGrid.proxy.url = HTTP_DirMovementDescriptions + "?type=Grid";
                        storeDirMovementDescriptionsGrid.load({ waitMsg: lanLoading });
                        storeDirMovementDescriptionsGrid.on('load', function () {

                            storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                            storeDirEmployeesGrid.on('load', function () {
                                if (storeDirEmployeesGrid.UO_Loaded) return;
                                storeDirEmployeesGrid.UO_Loaded = true;


                                loadingMask.hide();

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Скидка
                                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                    //Суммы
                                    //Ext.getCmp("SumPurch" + ObjectID).setValue(0);
                                    //Ext.getCmp("SumRetail" + ObjectID).setValue(0);
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();


                                    //Справочники
                                    //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID);
                                    //Ext.getCmp("DirVatValue" + ObjectID).setValue(0);


                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                    else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                    //Остаток по Складу: Присваиваем Товару - Склад
                                    //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                    //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                    //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();



                                    //alert(Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue());
                                    loadingMask.show();
                                    /*
                                    storeRem2PartiesGrid.proxy.url =
                                        HTTP_Rem2Parties +
                                        "?DirServiceNomenID=0" +
                                        "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                        //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                                        //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                                        "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();
                                    storeRem2PartiesGrid.load({ waitMsg: lanLoading });
                                    storeRem2PartiesGrid.on('load', function () {
                                        if (storeRem2PartiesGrid.UO_Loaded) return;
                                        storeRem2PartiesGrid.UO_Loaded = true;
                                        */

                                    storeDocServicePurchesGrid.proxy.url =
                                        HTTP_DocServicePurches +
                                        //"?DirServiceNomenID=0" +
                                        "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                        "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue() +
                                        "&DirServiceStatusIDS=1&DirServiceStatusIDPo=8" +
                                        //"&DirServiceStatusID_789=7";
                                        "&DirWarehouseIDOnly=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();

                                    storeDocServicePurchesGrid.load({ waitMsg: lanLoading });
                                    storeDocServicePurchesGrid.on('load', function () {
                                        if (storeDocServicePurchesGrid.UO_Loaded) return;
                                        storeDocServicePurchesGrid.UO_Loaded = true;

                                        loadingMask.hide();

                                    });



                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                                    //ArrList - значит
                                    if (!ArrList) {

                                        storeDocServiceMovTabsGrid.load({ waitMsg: lanLoading });
                                        storeDocServiceMovTabsGrid.on('load', function () {
                                            if (storeDocServiceMovTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                            storeDocServiceMovTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"


                                            //Если форма уже загружена выходим!
                                            if (widgetXForm.UO_Loaded) return;

                                            widgetXForm.load({
                                                method: "GET",
                                                timeout: varTimeOutDefault,
                                                waitMsg: lanLoading,
                                                url: HTTP_DocServiceMovs + IdcallModelData.DocServiceMovID + "/?DocID=" + IdcallModelData.DocID,
                                                success: function (form, action) {


                                                    //alert(Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue());
                                                    loadingMask.show();
                                                    /*
                                                    storeRem2PartiesGrid.proxy.url =
                                                        HTTP_Rem2Parties +
                                                        "?DirServiceNomenID=0" +
                                                        "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                                        //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                                                        //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                                                        "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();
                                                    storeRem2PartiesGrid.load({ waitMsg: lanLoading });
                                                    storeRem2PartiesGrid.on('load', function () {
                                                        if (storeRem2PartiesGrid.UO_Loaded) return;
                                                        storeRem2PartiesGrid.UO_Loaded = true;
                                                        */

                                                    storeDocServicePurchesGrid.proxy.url =
                                                        HTTP_DocServicePurches +
                                                        //"?DirServiceNomenID=0" +
                                                        "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                                        "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue() +
                                                        "&DirServiceStatusIDS=1&DirServiceStatusIDPo=8" +
                                                        //"&DirServiceStatusID_789=7";
                                                        "&DirWarehouseIDOnly=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();

                                                    storeDocServicePurchesGrid.load({ waitMsg: lanLoading });
                                                    storeDocServicePurchesGrid.on('load', function () {
                                                        if (storeDocServicePurchesGrid.UO_Loaded) return;
                                                        storeDocServicePurchesGrid.UO_Loaded = true;

                                                        loadingMask.hide();

                                                    });


                                                    widgetXForm.UO_Loaded = true;
                                                    //Фокус на открывшийся Виджет
                                                    widgetX.focus();

                                                    //Если Копия
                                                    if (New_Edit == 3) {
                                                        Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocServiceMovID" + ObjectID).setValue();
                                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                                    }
                                                    else {
                                                        //Наименование окна (сверху)
                                                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocServiceMovID" + ObjectID).getValue());

                                                        //Проведён или нет
                                                        if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                            Ext.Msg.alert(lanOrgName, txtMsg020);
                                                            Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                        }
                                                        else {
                                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                                        }
                                                        Ext.getCmp("btnPrint" + ObjectID).show();
                                                    }


                                                    //!!! ОСТОРОЖНО !!! Нельзя менять параметры после загрузки!!!
                                                    /*
                                                    //Склад и Организация привязанные к сотруднику
                                                    //Склад
                                                    if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                                    else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                                    //Организация
                                                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }
                                                    */

                                                    //Остаток по Складу: Присваиваем Товару - Склад
                                                    //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                                    //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                                    //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                                    //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                    Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                    //Разблокировка вызвавшего окна
                                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                },
                                                failure: function (form, action) {
                                                    //loadingMask.hide();
                                                    widgetX.close();
                                                    funPanelSubmitFailure(form, action);

                                                    //Фокус на открывшийся Виджет
                                                    widgetX.focus();

                                                    //Разблокировка вызвавшего окна
                                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                }
                                            });


                                        });


                                    } //if(!ArrList)
                                    //Создать "На основании ..."
                                    else {

                                        //Переменные
                                        var formRec = ArrList[0];
                                        var gridRec = ArrList[1];
                                        //var locDirWarehouseID = ArrList[2];
                                        //Форма
                                        var form = widgetXForm.getForm();
                                        formRec.data.DirWarehouseIDFrom = formRec.data.DirWarehouseID;
                                        form.loadRecord(formRec);
                                        //Грид
                                        //storeDocServiceMovTabsGrid.load({ waitMsg: lanLoading });
                                        for (var i = 0; i < gridRec.data.length; i++) storeDocServiceMovTabsGrid.add(gridRec.data.items[i].data);
                                        /*{
                                            gridRec.data.items[i].data.Quantity = gridRec.data.items[i].data.Remnant;
                                            storeDocServiceMovTabsGrid.add(gridRec.data.items[i].data);
                                        }*/

                                        // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                                        widgetXForm.UO_Loaded = true;
                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();

                                        //Если Копия
                                        if (New_Edit == 3) {
                                            Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocServiceMovID" + ObjectID).setValue(null);
                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                        }
                                        else {
                                            //Наименование окна (сверху)
                                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocServiceMovID" + ObjectID).getValue());
                                            //Проведён или нет
                                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                                Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                            }
                                            else {
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            Ext.getCmp("btnPrint" + ObjectID).show();
                                        }


                                        //Дата
                                        Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                        //Причина: Брак
                                        Ext.getCmp("DescriptionMovement" + ObjectID).setValue("Брак");
                                        //Дата
                                        Ext.getCmp("Base" + ObjectID).setValue("На основании отчета по торговле, тип 'Брак'");
                                        //Склад и Организация привязанные к сотруднику
                                        //Склад
                                        //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(locDirWarehouseID);
                                        //Организация
                                        if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                        else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                        //Остаток по Складу: Присваиваем Товару - Склад
                                        //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                        //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                        //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                        //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                        Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                        //Разблокировка вызвавшего окна
                                        //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                        //Прячим партии
                                        Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_TOP, true);
                                    }


                                }


                            });
                        });
                    });
                });
            });

            //});


            //Убираем кнопки
            //Ext.getCmp("expandAll" + ObjectID).setVisible(false);
            //Ext.getCmp("collapseAll" + ObjectID).setVisible(false);
            /*
            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);
            */

            break;
        }

        case "viewDocServiceMovTabsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,

                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    //Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirServiceNomenName" + ObjectID).setValue(IdcallModelData.DirServiceNomenName);
                    Ext.getCmp("DirServiceNomenID" + ObjectID).setValue(IdcallModelData.DirServiceNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                        //UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            break;
        }


        case "viewDocServiceInvsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData;
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
                }
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            //var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //2.1. DirReturnTypes
            var storeDirReturnTypesGrid = Ext.create("store.storeDirReturnTypesGrid"); storeDirReturnTypesGrid.setData([], false);
            //2.2. DirDescriptions
            var storeDirDescriptionsGrid = Ext.create("store.storeDirDescriptionsGrid"); storeDirDescriptionsGrid.setData([], false);
            //3. Табличная часть
            var storeDocServiceInvTabsGrid = Ext.create("store.storeDocServiceInvTabsGrid"); storeDocServiceInvTabsGrid.setData([], false);
            storeDocServiceInvTabsGrid.UO_viewConfig = false; //Надо для подсчёта суммы при редактировании Грида (см. viewConfig в Гриде)
            storeDocServiceInvTabsGrid.UO_id = ObjectID;
            //4. Партии
            //var storeRem2PartiesGrid = Ext.create("store.storeRem2PartiesGrid"); storeRem2PartiesGrid.setData([], false);
            var storeDocServicePurchesGrid = Ext.create("store.storeDocServicePurchesGrid"); storeDocServicePurchesGrid.setData([], false);
            //5. Склад
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            //5. Сотрудник
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";

            if (varStoreDirPaymentTypesGrid == undefined) {
                varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
            }


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                //storeDirServiceNomenTree: storeDirServiceNomenTree,
                storeGrid: storeDocServiceInvTabsGrid, //storeDocServiceInvTabsGrid,
                //storeRem2PartiesGrid: storeRem2PartiesGrid,
                storeDocServicePurchesGrid: storeDocServicePurchesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,

                storeDirReturnTypesGrid: varStoreDirReturnTypesGrid, //storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: storeDirDescriptionsGrid,

                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirEmployeesGrid: storeDirEmployeesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Убираем в Гриде не нужные поля
            /*var columns = Ext.getCmp('gridParty_' + ObjectID).columns;
            for (i = 0; i < columns.length; i++) {
                if (columns[i].dataIndex == "DirContractorName" || columns[i].dataIndex == "DirCharMaterialName") {
                    columns[i].setVisible(false);
                    columns[i].hide();
                }
            }*/

            //Прячем дерево Аппаратов
            //Ext.getCmp("tree_" + ObjectID).collapse();

            //Сумма с Налогом
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title);
            //Фокус на открывшийся Виджет
            widgetX.focus();
            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            /*
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
                */

            storeDirWarehousesGrid.load({ waitMsg: lanLoading });
            storeDirWarehousesGrid.on('load', function () {
                if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                storeDirEmployeesGrid.on('load', function () {
                    if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirDescriptionsGrid.proxy.url = HTTP_DirDescriptions + "?type=Grid";
                    storeDirDescriptionsGrid.load({ waitMsg: lanLoading });
                    storeDirDescriptionsGrid.on('load', function () {

                        //storeDirReturnTypesGrid.proxy.url = HTTP_DirReturnTypes + "?type=Grid";
                        //storeDirReturnTypesGrid.load({ waitMsg: lanLoading });
                        //storeDirReturnTypesGrid.on('load', function () {


                        if (Ext.getCmp("TriggerSearchTree" + ObjectID)) Ext.getCmp("TriggerSearchTree" + ObjectID).focus();


                        //Склад и Организация привязанные к сотруднику
                        //Если у Сотрудника выбран Склад и Организация - блокируем их!
                        //Склад

                        if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
                        if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                        else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                        //Для "остаток по складу": Присваиваем Товару - Склад
                        //if (varDirWarehouseIDEmpl == 0) { storeDirServiceNomenTree.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
                        //else { storeDirServiceNomenTree.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

                        //Организация
                        if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
                        if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                        else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                        //Тип цен
                        Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

                        //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
                        //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
                        //Ext.getCmp("SearchType" + ObjectID).setValue(1);
                        //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                        //Дата
                        //Ext.getCmp("DocDateS" + ObjectID).setValue(new Date());
                        //Ext.getCmp("DocDatePo" + ObjectID).setValue(new Date());


                        loadingMask.hide();


                        //Если "Edit" (не новый документ)
                        //if (IdcallModelData.DocServiceInvID > 0) {
                        if (New_Edit == 1) {

                            //Если новая запись, то установить "по умолчанию"
                            Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                            Ext.getCmp("SumOfVATCurrency1" + ObjectID).setValue(0);
                            Ext.getCmp("SumOfVATCurrency2" + ObjectID).setValue(0);

                            //Сумма с Налогом
                            widgetX.setTitle(widgetX.title + " № Новая");

                            Ext.getCmp("SpisatS" + ObjectID).setValue(1);

                            //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                            Ext.getCmp("btnHelds" + ObjectID).show();
                            Ext.getCmp("btnRecord" + ObjectID).show();

                            storeDocServiceInvTabsGrid.UO_viewConfig = true;

                            //Фокус на открывшийся Виджет
                            widgetX.focus();

                        }
                        else if (New_Edit == 2 || New_Edit == 3) {

                            storeDocServiceInvTabsGrid.proxy.url =
                                HTTP_DocServiceInvTabs +
                                //"?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                                //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                                //"&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                "?DocServiceInvID=" + IdcallModelData.DocServiceInvID; //Ext.getCmp("DocServiceInvID" + ObjectID).getValue();

                            storeDocServiceInvTabsGrid.load({ waitMsg: lanLoading });
                            storeDocServiceInvTabsGrid.on('load', function () {
                                if (storeDocServiceInvTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDocServiceInvTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
                                storeDocServiceInvTabsGrid.UO_viewConfig = true;

                                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                //Если форма уже загружена выходим!
                                if (widgetXForm.UO_Loaded) return;

                                widgetXForm.load({
                                    method: "GET",
                                    timeout: varTimeOutDefault,
                                    waitMsg: lanLoading,
                                    url: HTTP_DocServiceInvs + IdcallModelData.DocServiceInvID + "/?DocID=" + IdcallModelData.DocID,
                                    success: function (form, action) {

                                        widgetXForm.UO_Loaded = true;
                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();

                                        //Проведён или нет
                                        if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                            Ext.Msg.alert(lanOrgName, txtMsg020);
                                            Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                        }
                                        else {
                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                        }
                                        Ext.getCmp("btnPrint" + ObjectID).show();

                                    },
                                    failure: function (form, action) {
                                        //loadingMask.hide();
                                        widgetX.close();
                                        funPanelSubmitFailure(form, action);

                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();

                                        //Разблокировка вызвавшего окна
                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                    }
                                });


                            });

                        }


                    });
                });
            });


            //});


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            /*
            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);
            */
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);


            break;
        }





        /* Переоценка */

        case "viewDocNomenRevaluationsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=3&DirContractor2TypeID2=4";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirVatsGrid = Ext.create("store.storeDirVatsGrid"); storeDirVatsGrid.setData([], false); storeDirVatsGrid.proxy.url = HTTP_DirVats + "?type=Grid";
            //3. Табличная часть
            var storeDocNomenRevaluationTabsGrid = Ext.create("store.storeDocNomenRevaluationTabsGrid"); storeDocNomenRevaluationTabsGrid.setData([], false); storeDocNomenRevaluationTabsGrid.proxy.url = HTTP_DocNomenRevaluationTabs + "?DocNomenRevaluationID=" + IdcallModelData.DocNomenRevaluationID;
            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocNomenRevaluationTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirVatsGrid: storeDirVatsGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                        storeDirWarehousesGrid.on('load', function () {
                            if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirVatsGrid.load({ waitMsg: lanLoading });
                            storeDirVatsGrid.on('load', function () {
                                if (storeDirVatsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirVatsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                loadingMask.hide();

                                var rec = { DirWarehouseID: 0, DirWarehouseName: "Все" }; storeDirWarehousesGrid.insert(0, rec);

                                if (New_Edit == 1) {

                                    //Если новая запись, то установить "по умолчанию"

                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " № Новая");

                                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                    Ext.getCmp("btnRecord" + ObjectID).show();

                                    //Справочники
                                    //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                                    //Ext.getCmp("DirVatValue" + ObjectID).setValue(0);

                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                                    else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                    //Для "остаток по складу": Присваиваем Товару - Склад
                                    if (varDirWarehouseIDEmpl == 0) { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
                                    else { storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                }
                                else if (New_Edit == 2 || New_Edit == 3) {

                                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                                    //ArrList - значит
                                    if (!ArrList) {
                                       
                                        storeDocNomenRevaluationTabsGrid.load({ waitMsg: lanLoading });
                                        storeDocNomenRevaluationTabsGrid.on('load', function () {
                                            if (storeDocNomenRevaluationTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                            storeDocNomenRevaluationTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                                            //Если форма уже загружена выходим!
                                            if (widgetXForm.UO_Loaded) return;

                                            widgetXForm.load({
                                                method: "GET",
                                                timeout: varTimeOutDefault,
                                                waitMsg: lanLoading,
                                                url: HTTP_DocNomenRevaluations + IdcallModelData.DocNomenRevaluationID + "/?DocID=" + IdcallModelData.DocID,
                                                success: function (form, action) {

                                                    widgetXForm.UO_Loaded = true;
                                                    //Фокус на открывшийся Виджет
                                                    widgetX.focus();

                                                    //Если Копия
                                                    if (New_Edit == 3) {
                                                        Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocNomenRevaluationID" + ObjectID).setValue(null);
                                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                                    }
                                                    else {
                                                        //Наименование окна (сверху)
                                                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocNomenRevaluationID" + ObjectID).getValue());

                                                        //Проведён или нет
                                                        if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                            Ext.Msg.alert(lanOrgName, txtMsg020);
                                                            Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                        }
                                                        else {
                                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                                        }
                                                        //Кнопку "Печать" - делаем активной"
                                                        //Ext.getCmp("btnPrint" + ObjectID).show();
                                                        //Кнопку "Платежи" - делаем активной"
                                                        //Ext.getCmp("btnGridPayment" + ObjectID).enable();
                                                    }

                                                    //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                    //Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                    //Остаток по Складу: Присваиваем Товару - Склад
                                                    storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                                    //Разблокировка вызвавшего окна
                                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                },
                                                failure: function (form, action) {
                                                    //loadingMask.hide();
                                                    widgetX.close();
                                                    funPanelSubmitFailure(form, action);

                                                    //Фокус на открывшийся Виджет
                                                    widgetX.focus();

                                                    //Разблокировка вызвавшего окна
                                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                }
                                            });

                                        });

                                    } //if(!ArrList)
                                        //Создать "На основании ..."
                                    else {
                                        //Переменные
                                        var formRec = ArrList[0];
                                        var gridRec = ArrList[1];
                                        //Форма
                                        var form = widgetXForm.getForm();
                                        form.loadRecord(formRec);
                                        //Грид
                                        storeDocNomenRevaluationTabsGrid.load({ waitMsg: lanLoading });
                                        for (var i = 0; i < gridRec.data.length; i++) storeDocNomenRevaluationTabsGrid.add(gridRec.data.items[i].data);

                                        // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                                        widgetXForm.UO_Loaded = true;
                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();

                                        //Если Копия
                                        if (New_Edit == 3) {
                                            Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocNomenRevaluationID" + ObjectID).setValue(null);
                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                        }
                                        else {
                                            //Наименование окна (сверху)
                                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocNomenRevaluationID" + ObjectID).getValue());
                                            //Проведён или нет
                                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                                Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                            }
                                            else {
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            Ext.getCmp("btnPrint" + ObjectID).show();
                                        }

                                        //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                        Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                        //Остаток по Складу: Присваиваем Товару - Склад
                                        storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                        //Разблокировка вызвавшего окна
                                        //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                    }

                                }

                            });
                        });
                    });
                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }


            /* Переоценка: Редактирование Грида */

        case "viewDocNomenRevaluationTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            varPriceChange_ReadOnly = true; //Запретить редактировать цены
            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            varPriceChange_ReadOnly = true; //Запретить редактировать цены
            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
            //Форма
            var form = widgetXForm.getForm();

            if (New_Edit == 1) {

                //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                if (GridTree) {
                    //Редактирование (загрузить из грида)
                    form.loadRecord(UO_GridRecord);
                }
                else {
                    //Новый товар
                    //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                    //UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                    //UO_GridRecord.data.Quantity = 1;

                    UO_GridRecord.data.PriceRetailVAT_OLD = UO_GridRecord.data.PriceRetailVAT;
                    UO_GridRecord.data.PriceRetailCurrency_OLD = UO_GridRecord.data.PriceRetailCurrency;

                    UO_GridRecord.data.PriceWholesaleVAT_OLD = UO_GridRecord.data.PriceWholesaleVAT;
                    UO_GridRecord.data.PriceWholesaleCurrency_OLD = UO_GridRecord.data.PriceWholesaleCurrency;

                    UO_GridRecord.data.PriceIMVAT_OLD = UO_GridRecord.data.PriceIMVAT;
                    UO_GridRecord.data.PriceIMCurrency_OLD = UO_GridRecord.data.PriceIMCurrency;

                    form.loadRecord(UO_GridRecord);
                }

            }
            else if (New_Edit == 2 || New_Edit == 3) {
                form.loadRecord(UO_GridRecord);
            }

            varPriceChange_ReadOnly = false; //Разрешить редактировать цены


            break;
        }




            /* Зарплата */

        case "viewDocSalariesEdit": {
            
            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
            //Если запись помечена на удаление, то сообщить об этом и выйти
            if (IdcallModelData.Del == true) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocSalaryID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //2. Combo
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            //3. Табличная часть
            var storeDocSalaryTabsGrid = Ext.create("store.storeDocSalaryTabsGrid"); storeDocSalaryTabsGrid.setData([], false); storeDocSalaryTabsGrid.proxy.url = HTTP_DocSalaryTabs + "?DocSalaryID=" + IdcallModelData.DocSalaryID;


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeGrid: storeDocSalaryTabsGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
            storeDirContractorsOrgGrid.on('load', function () {
                if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                if (New_Edit == 1) {

                    //Если новая запись, то установить "по умолчанию"

                    //Дата
                    var year = (new Date()).getFullYear();
                    var month = (new Date()).getMonth() + 1;
                    //Ext.getCmp("DocDate" + ObjectID).setValue(new Date(year + "-" + month + "-01"));
                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                    //Скидка
                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Сумма с Налогом
                    Ext.getCmp("Sum1" + ObjectID).setValue(0);
                    Ext.getCmp("Sum2" + ObjectID).setValue(0);
                    Ext.getCmp("Sum3" + ObjectID).setValue(0);
                    Ext.getCmp("Sum4" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " № Новая");

                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    Ext.getCmp("btnRecord" + ObjectID).show();

                    //Организация
                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }



                    //Запрос на сервер за ЗП + Премии за заданный месяц по всем Сотрудникам
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        waitMsg: lanUpload,
                        url: HTTP_DocSalaries + "777/777/",
                        method: 'GET',
                        success: function (result) {
                            
                            var sData = Ext.decode(result.responseText);
                            if (sData.success == false) {
                                Ext.MessageBox.show({ title: lanFailure, msg: sData, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                            }
                            else {
                                //По умолчению 2017 год, Январь
                                var DocYear = 2017, DocMonth = 1;

                                //Последний документ
                                if (sData.data != null) {
                                    DocYear = sData.data.DocYear;
                                    DocMonth = sData.data.DocMonth + 1;
                                    if (DocMonth == 13) {
                                        DocYear = DocYear + 1;
                                        DocMonth = 1;
                                    }
                                }

                                //Заполняем поля формы
                                Ext.getCmp("DocYear" + ObjectID).setValue(DocYear);
                                Ext.getCmp("DocMonth" + ObjectID).setValue(DocMonth);

                                //Запрос за ЗП
                                storeDocSalaryTabsGrid.setData([], false);
                                storeDocSalaryTabsGrid.proxy.url = HTTP_DocSalaryTabs + "777/?DocYear=" + DocYear + "&DocMonth=" + DocMonth;
                                storeDocSalaryTabsGrid.load({ waitMsg: lanLoading });
                                storeDocSalaryTabsGrid.on('load', function () {
                                    if (storeDocSalaryTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                    storeDocSalaryTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                    controllerDocSalariesEdit_RecalculationSums(ObjectID);

                                });
                            }

                        },
                        failure: function (result) {
                            if (varCountErrorSettingsRequest < varCountErrorRequest + 5) {
                                varCountErrorSettingsRequest++;
                                Variables_SettingsRequest();
                            }
                            else {
                                Ext.MessageBox.show({
                                    title: lanOrgName,
                                    msg: txtMsg017,
                                    icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                                    fn: function (buttons) {
                                        if (buttons == "yes") { location.reload(); }
                                    }
                                });
                            }
                        }
                    });



                    //Фокус на открывшийся Виджет
                    widgetX.focus();

                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                }
                else if (New_Edit == 2 || New_Edit == 3) {

                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                    //ArrList - значит
                    if (!ArrList) {

                        storeDocSalaryTabsGrid.load({ waitMsg: lanLoading });
                        storeDocSalaryTabsGrid.on('load', function () {
                            if (storeDocSalaryTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                            storeDocSalaryTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                            //Если форма уже загружена выходим!
                            if (widgetXForm.UO_Loaded) return;

                            widgetXForm.load({
                                method: "GET",
                                timeout: varTimeOutDefault,
                                waitMsg: lanLoading,
                                url: HTTP_DocSalaries + IdcallModelData.DocSalaryID + "/?DocID=" + IdcallModelData.DocID,
                                success: function (form, action) {

                                    widgetXForm.UO_Loaded = true;
                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Если Копия
                                    if (New_Edit == 3) {
                                        Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSalaryID" + ObjectID).setValue(null);
                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                    }
                                    else {
                                        //Наименование окна (сверху)
                                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSalaryID" + ObjectID).getValue());

                                        //Проведён или нет
                                        if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                            Ext.Msg.alert(lanOrgName, txtMsg020);
                                            Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                        }
                                        else {
                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                        }
                                        //Кнопку "Печать" - делаем активной"
                                        Ext.getCmp("btnPrint" + ObjectID).show();
                                    }

                                    //Пересчет сумм
                                    controllerDocSalariesEdit_RecalculationSums(ObjectID);

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                },
                                failure: function (form, action) {
                                    //loadingMask.hide();
                                    widgetX.close();
                                    funPanelSubmitFailure(form, action);

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                }
                            });

                        });

                    } //if(!ArrList)
                        //Создать "На основании ..."
                    else {
                        //Переменные
                        var formRec = ArrList[0];
                        var gridRec = ArrList[1];
                        //Форма
                        var form = widgetXForm.getForm();
                        form.loadRecord(formRec);
                        //Грид
                        //storeDocSalaryTabsGrid.load({ waitMsg: lanLoading });
                        for (var i = 0; i < gridRec.data.length; i++) storeDocSalaryTabsGrid.add(gridRec.data.items[i].data);

                        // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                        widgetXForm.UO_Loaded = true;
                        //Фокус на открывшийся Виджет
                        widgetX.focus();

                        //Если Копия
                        if (New_Edit == 3) {
                            Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSalaryID" + ObjectID).setValue(null);
                            Ext.getCmp("btnHelds" + ObjectID).show();
                            Ext.getCmp("btnRecord" + ObjectID).show();
                        }
                        else {
                            //Наименование окна (сверху)
                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSalaryID" + ObjectID).getValue());
                            //Проведён или нет
                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                            }
                            else {
                                Ext.getCmp("btnHelds" + ObjectID).show();
                                Ext.getCmp("btnRecord" + ObjectID).show();
                            }
                            Ext.getCmp("btnPrint" + ObjectID).show();
                        }

                        //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                        Ext.getCmp("Reserve" + ObjectID).setValue(true);
                        //Остаток по Складу: Присваиваем Товару - Склад
                        storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                        //Разблокировка вызвавшего окна
                        //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                    }

                }

            });

            break;
        }

            /* Зарплата: Редактирование Грида */

        case "viewDocSalaryTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false);
            storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid";
            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                        UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }




        //Б/У *** *** ***

            /* Б/У - Приёмка */

        case "viewDocSecondHandPurchesEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData; // = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            if (New_Edit > 1) {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                    return;
                }
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName; // + "_" + IdcallModelData.DocSecondHandID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //2. Combo
            //Store Combo "ContractorsOrg"Warehouses
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            //var storeDirServiceContractorsGrid = Ext.create("store.storeDirServiceContractorsGrid"); storeDirServiceContractorsGrid.setData([], false); storeDirServiceContractorsGrid.proxy.url = HTTP_DirServiceContractors + "?type=Grid";
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            var storeDirServiceComplectsGrid = Ext.create("store.storeDirServiceComplectsGrid"); storeDirServiceComplectsGrid.setData([], false); storeDirServiceComplectsGrid.proxy.url = HTTP_DirServiceComplects + "?type=Grid";
            var storeDirServiceProblemsGrid = Ext.create("store.storeDirServiceProblemsGrid"); storeDirServiceProblemsGrid.setData([], false); storeDirServiceProblemsGrid.proxy.url = HTTP_DirServiceProblems + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirServiceNomenTree: storeDirServiceNomenTree,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                //storeDirServiceContractorsGrid: storeDirServiceContractorsGrid,
                varStoreDirServiceContractorsGrid: varStoreDirServiceContractorsGrid,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeDirServiceComplectsGrid: storeDirServiceComplectsGrid,
                storeDirServiceProblemsGrid: storeDirServiceProblemsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Ext.getCmp("SearchType" + ObjectID).setValue(1);
            Ext.getCmp("DirServiceNomenPatchFull" + ObjectID).setText("<b>...</b>", false);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return;
                storeDirServiceNomenTree.UO_Loaded = true;

                storeDirServiceComplectsGrid.load({ waitMsg: lanLoading });
                storeDirServiceComplectsGrid.on('load', function () {
                    if (storeDirServiceComplectsGrid.UO_Loaded) return;
                    storeDirServiceComplectsGrid.UO_Loaded = true;

                    storeDirServiceProblemsGrid.load({ waitMsg: lanLoading });
                    storeDirServiceProblemsGrid.on('load', function () {
                        if (storeDirServiceProblemsGrid.UO_Loaded) return;
                        storeDirServiceProblemsGrid.UO_Loaded = true;

                        loadingMask.hide();

                        if (New_Edit == 1) {

                            //Если новая запись, то установить "по умолчанию"

                            //Даты
                            Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                            var dat = new Date(); dat.setDate(dat.getDate() + varReadinessDay);
                            Ext.getCmp("DateDone" + ObjectID).setValue(dat);
                            //Наименование окна (сверху)
                            widgetX.setTitle(widgetX.title + " № Новая");


                            Ext.getCmp("DirServiceContractorPhone" + ObjectID).setValue(varPhoneNumberBegin);
                            //Ext.getCmp("ComponentDevice" + ObjectID).setValue(true);
                            Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                            Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                            Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                            //Ext.getCmp("PriceVAT" + ObjectID).setValue(0);
                            //Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                            //Ext.getCmp("PrepaymentSum" + ObjectID).setValue(0);
                            Ext.getCmp("DirEmployeeIDMaster" + ObjectID).setValue(varDirEmployeeID);
                            //Справочники
                            //Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);
                            //Склад и Организация привязанные к сотруднику
                            //Склад
                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                            //Организация
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }
                            //К-во по нулям
                            //Ext.getCmp("QuantityOk" + ObjectID).setValue(0);
                            //Ext.getCmp("QuantityFail" + ObjectID).setValue(0);
                            //Ext.getCmp("QuantityCount" + ObjectID).setValue(0);
                            //Предоплата
                            //Ext.getCmp("PrepaymentSum" + ObjectID).setValue(0);

                            //Фокус на открывшийся Виджет
                            widgetX.focus();
                            //Разблокировка вызвавшего окна
                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                        }
                        else if (New_Edit == 2 || New_Edit == 3) {

                            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                            //Если форма уже загружена выходим!
                            if (widgetXForm.UO_Loaded) return;

                            widgetXForm.load({
                                method: "GET",
                                timeout: varTimeOutDefault,
                                waitMsg: lanLoading,
                                url: HTTP_DocSecondHands + IdcallModelData.DocSecondHandID + "/?DocID=" + IdcallModelData.DocID,
                                success: function (form, action) {

                                    widgetXForm.UO_Loaded = true;
                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Если Копия
                                    if (New_Edit == 3) {
                                        Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSecondHandID" + ObjectID).setValue(null);
                                    }
                                    else {
                                        //Наименование окна (сверху)
                                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandID" + ObjectID).getValue());
                                    }

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                },
                                failure: function (form, action) {
                                    //loadingMask.hide();
                                    widgetX.close();
                                    funPanelSubmitFailure(form, action);

                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Разблокировка вызвавшего окна
                                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                }
                            });

                        }

                    });
                });
            });




            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Б/У - На продажу */

        case "viewDocSecondHandWorkshopsInRetail": {
            
            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

                UO_GridServerParam1: UO_GridServerParam1,
                UO_Param_fn: UO_Param_fn,   //controllerDocSecondHandWorkshops_ChangeStatus_Request
                UO_idTab: UO_idTab,         //aButton
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
            //Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
            //form.loadRecord(UO_GridRecord);
            Ext.getCmp("PriceVAT" + ObjectID).setValue(Ext.getCmp("PriceVAT" + UO_Param_id).getValue());
            Ext.getCmp("SumTotal2" + ObjectID).setValue(Ext.getCmp("SumTotal2" + UO_Param_id).getValue());
            Ext.getCmp("PriceVATSums" + ObjectID).setValue(Ext.getCmp("PriceVATSums" + UO_Param_id).getValue());

            

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }

            /* Розница */

        case "viewDocSecondHandRetailsEdit_OLD": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData;
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
                }
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;
            
            //1. Store Grid
            var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //2.1. DirReturnTypes
            var storeDirReturnTypesGrid = Ext.create("store.storeDirReturnTypesGrid"); storeDirReturnTypesGrid.setData([], false);
            //2.2. DirDescriptions
            var storeDirDescriptionsGrid = Ext.create("store.storeDirDescriptionsGrid"); storeDirDescriptionsGrid.setData([], false);
            //3. Табличная часть
            var storeDocSecondHandRetailTabsGrid = Ext.create("store.storeDocSecondHandRetailTabsGrid"); storeDocSecondHandRetailTabsGrid.setData([], false); //storeDocSecondHandRetailTabsGrid.proxy.url = HTTP_DocSecondHandRetailTabs + "?DocSecondHandRetailID=" + IdcallModelData.DocSecondHandRetailID;
            //4. Партии
            var storeRem2PartiesGrid = Ext.create("store.storeRem2PartiesGrid"); storeRem2PartiesGrid.setData([], false);

            if (varStoreDirPaymentTypesGrid == undefined) {
                varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
            }


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirServiceNomenTree: storeDirServiceNomenTree,
                storeGrid: storeDocSecondHandRetailTabsGrid,
                storeRem2PartiesGrid: storeRem2PartiesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,

                storeDirReturnTypesGrid: varStoreDirReturnTypesGrid, //storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: storeDirDescriptionsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Убираем в Гриде не нужные поля
            /*var columns = Ext.getCmp('gridParty_' + ObjectID).columns;
            for (i = 0; i < columns.length; i++) {
                if (columns[i].dataIndex == "DirContractorName" || columns[i].dataIndex == "DirCharMaterialName") {
                    columns[i].setVisible(false);
                    columns[i].hide();
                }
            }*/

            //Прячем дерево Аппаратов
            Ext.getCmp("tree_" + ObjectID).collapse();

            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
            //Для "остаток по складу": Присваиваем Товару - Склад
            if (varDirWarehouseIDEmpl == 0) { storeDirServiceNomenTree.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
            else { storeDirServiceNomenTree.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Тип цен
            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Дата
            Ext.getCmp("DocDateS" + ObjectID).setValue(new Date());
            Ext.getCmp("DocDatePo" + ObjectID).setValue(new Date());
            //Сумма с Налогом
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title);
            //Фокус на открывшийся Виджет
            widgetX.focus();
            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"

                storeDocSecondHandRetailTabsGrid.proxy.url =
                    HTTP_DocSecondHandRetailTabs +
                    "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                    "&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                    "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                    "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();

                storeDocSecondHandRetailTabsGrid.load({ waitMsg: lanLoading });
                storeDocSecondHandRetailTabsGrid.on('load', function () {
                    if (storeDocSecondHandRetailTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDocSecondHandRetailTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"

                    storeRem2PartiesGrid.proxy.url =
                        HTTP_Rem2Parties +
                        "?DirServiceNomenID=0" +
                        "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                        //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                        //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                        "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                    storeRem2PartiesGrid.load({ waitMsg: lanLoading });
                    storeRem2PartiesGrid.on('load', function () {
                        if (storeRem2PartiesGrid.UO_Loaded) return;
                        storeRem2PartiesGrid.UO_Loaded = true;


                        storeDirDescriptionsGrid.proxy.url = HTTP_DirDescriptions + "?type=Grid";
                        storeDirDescriptionsGrid.load({ waitMsg: lanLoading });
                        storeDirDescriptionsGrid.on('load', function () {

                            //storeDirReturnTypesGrid.proxy.url = HTTP_DirReturnTypes + "?type=Grid";
                            //storeDirReturnTypesGrid.load({ waitMsg: lanLoading });
                            //storeDirReturnTypesGrid.on('load', function () {

                            loadingMask.hide();

                            if (Ext.getCmp("TriggerSearchTree" + ObjectID)) Ext.getCmp("TriggerSearchTree" + ObjectID).focus();

                            //});
                        });


                    });

                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);


            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);


            break;
        }

        case "viewDocSecondHandSalesEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData;
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
                }
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //2.1. DirReturnTypes
            var storeDirReturnTypesGrid = Ext.create("store.storeDirReturnTypesGrid"); storeDirReturnTypesGrid.setData([], false);
            //2.2. DirDescriptions
            var storeDirDescriptionsGrid = Ext.create("store.storeDirDescriptionsGrid"); storeDirDescriptionsGrid.setData([], false);
            //3. Табличная часть
            var storeDocSecondHandSalesGrid = Ext.create("store.storeDocSecondHandSalesGrid"); storeDocSecondHandSalesGrid.setData([], false); //storeDocSecondHandSalesGrid.proxy.url = HTTP_DocSecondHandRetailTabs + "?DocSecondHandRetailID=" + IdcallModelData.DocSecondHandRetailID;
            //4. Партии
            var storeDocSecondHandPurchesGrid = Ext.create("store.storeDocSecondHandPurchesGrid"); storeDocSecondHandPurchesGrid.setData([], false);

            if (varStoreDirPaymentTypesGrid == undefined) {
                varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
            }


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirServiceNomenTree: storeDirServiceNomenTree,
                storeGrid: storeDocSecondHandSalesGrid,
                storeDocSecondHandPurchesGrid: storeDocSecondHandPurchesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,

                storeDirReturnTypesGrid: varStoreDirReturnTypesGrid, //storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: storeDirDescriptionsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Убираем в Гриде не нужные поля
            /*var columns = Ext.getCmp('gridParty_' + ObjectID).columns;
            for (i = 0; i < columns.length; i++) {
                if (columns[i].dataIndex == "DirContractorName" || columns[i].dataIndex == "DirCharMaterialName") {
                    columns[i].setVisible(false);
                    columns[i].hide();
                }
            }*/

            //Прячем дерево Аппаратов
            Ext.getCmp("tree_" + ObjectID).collapse();

            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
            //Для "остаток по складу": Присваиваем Товару - Склад
            if (varDirWarehouseIDEmpl == 0) { storeDirServiceNomenTree.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
            else { storeDirServiceNomenTree.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Тип цен
            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            //Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Дата
            Ext.getCmp("DocDateS" + ObjectID).setValue(new Date());
            Ext.getCmp("DocDatePo" + ObjectID).setValue(new Date());
            //Сумма с Налогом
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title);
            //Фокус на открывшийся Виджет
            widgetX.focus();
            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"

                storeDocSecondHandSalesGrid.proxy.url =
                    HTTP_DocSecondHandSales +
                    "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                    "&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                    "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                    "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();

                storeDocSecondHandSalesGrid.load({ waitMsg: lanLoading });
                storeDocSecondHandSalesGrid.on('load', function () {
                    if (storeDocSecondHandSalesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDocSecondHandSalesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"

                    storeDocSecondHandPurchesGrid.proxy.url =
                        HTTP_DocSecondHandPurches +
                        //"?DirServiceNomenID=0" +
                        "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                        "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue() + 
                        "&DirSecondHandStatusIDS=9&DirSecondHandStatusIDPo=9" + 
                        "&DirSecondHandStatusID_789=7";
                    
                    storeDocSecondHandPurchesGrid.load({ waitMsg: lanLoading });
                    storeDocSecondHandPurchesGrid.on('load', function () {
                        if (storeDocSecondHandPurchesGrid.UO_Loaded) return;
                        storeDocSecondHandPurchesGrid.UO_Loaded = true;


                        storeDirDescriptionsGrid.proxy.url = HTTP_DirDescriptions + "?type=Grid";
                        storeDirDescriptionsGrid.load({ waitMsg: lanLoading });
                        storeDirDescriptionsGrid.on('load', function () {

                            //storeDirReturnTypesGrid.proxy.url = HTTP_DirReturnTypes + "?type=Grid";
                            //storeDirReturnTypesGrid.load({ waitMsg: lanLoading });
                            //storeDirReturnTypesGrid.on('load', function () {

                            loadingMask.hide();

                            if (Ext.getCmp("TriggerSearchTree" + ObjectID)) Ext.getCmp("TriggerSearchTree" + ObjectID).focus();

                            //});
                        });


                    });

                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);


            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);


            break;
        }

        case "viewDocSecondHandRetailTabsEdit_OLD": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName; // + "_" + IdcallModelData.DocPurchID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid"; storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            //3. Табличная часть
            var storeDocSecondHandRetailTabsGrid = Ext.create("store.storeDocSecondHandRetailTabsGrid"); storeDocSecondHandRetailTabsGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeGrid: storeDocSecondHandRetailTabsGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Всегда зарезервирован (есть проблема с отменой проведения прихода)
            Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                if (New_Edit == 1) {

                    //Поля *** *** *** *** ***

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirServiceNomenName" + ObjectID).setValue(IdcallModelData.DirServiceNomenName);
                    Ext.getCmp("DirServiceNomenID" + ObjectID).setValue(IdcallModelData.DirServiceNomenID);

                    //Новый товар
                    //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                    UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                    UO_GridRecord.data.Quantity = 1;
                    form.loadRecord(UO_GridRecord);

                    //Кнопки *** *** *** *** ***

                    //UO_GridRecord.data.Discount = 0;
                    Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    //Ext.getCmp("btnHelds1" + ObjectID).show();
                    //Ext.getCmp("btnHelds2" + ObjectID).show();
                    //Ext.getCmp("btnRecord" + ObjectID).show();

                    //Значения формы
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);
                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                    Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));//Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(Ext.getCmp("DocDate" + Ext.getCmp(UO_idCall).UO_id).getValue(), "Y-m-d"));
                    Ext.getCmp("DocSecondHandRetailID" + ObjectID).setValue(0);

                    //Фокус на кнопку "Расчет"
                    Ext.getCmp("btnHelds" + ObjectID).focus();

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);

                    //Ext.getCmp("DocSecondHandRetailID" + ObjectID).setValue(Ext.getCmp("NumberReal" + ObjectID).getValue());
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandRetailID" + ObjectID).getValue());

                    //Проведён или нет
                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                        Ext.Msg.alert(lanOrgName, txtMsg020);
                        Ext.getCmp("btnHeldCancel" + ObjectID).show();
                    }
                    else {
                        Ext.getCmp("btnHelds" + ObjectID).show();
                        //Ext.getCmp("btnHelds1" + ObjectID).show();
                        //Ext.getCmp("btnHelds2" + ObjectID).show();
                    }
                    //Кнопку "Печать" - делаем активной"
                    //Ext.getCmp("btnPrint" + ObjectID).show();

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }

        case "viewDocSecondHandSaleTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName; // + "_" + IdcallModelData.DocPurchID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid"; storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            //3. Табличная часть
            //var storeDocSecondHandRetailTabsGrid = Ext.create("store.storeDocSecondHandRetailTabsGrid"); storeDocSecondHandRetailTabsGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                //storeGrid: storeDocSecondHandRetailTabsGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Всегда зарезервирован (есть проблема с отменой проведения прихода)
            Ext.getCmp("Reserve" + ObjectID).setValue(true);

            
            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                if (New_Edit == 1) {

                    //Поля *** *** *** *** ***

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirServiceNomenName" + ObjectID).setValue(IdcallModelData.DirServiceNomenName);
                    Ext.getCmp("DirServiceNomenID" + ObjectID).setValue(IdcallModelData.DirServiceNomenID);

                    //Новый товар
                    //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                    UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                    UO_GridRecord.data.Quantity = 1;
                    form.loadRecord(UO_GridRecord);
                    Ext.getCmp("DocID" + ObjectID).setValue("");
                    Ext.getCmp("DocID2" + ObjectID).setValue("");
                    Ext.getCmp("Held" + ObjectID).setValue("");

                    //Кнопки *** *** *** *** ***

                    //UO_GridRecord.data.Discount = 0;
                    Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    //Ext.getCmp("btnHelds1" + ObjectID).show();
                    //Ext.getCmp("btnHelds2" + ObjectID).show();
                    //Ext.getCmp("btnRecord" + ObjectID).show();

                    //Значения формы
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);
                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                    Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));//Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(Ext.getCmp("DocDate" + Ext.getCmp(UO_idCall).UO_id).getValue(), "Y-m-d"));
                    Ext.getCmp("DocSecondHandSaleID" + ObjectID).setValue(0);

                    //Фокус на кнопку "Расчет"
                    Ext.getCmp("btnHelds" + ObjectID).focus();

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);

                    //Ext.getCmp("DocSecondHandSaleID" + ObjectID).setValue(Ext.getCmp("NumberReal" + ObjectID).getValue());
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandSaleID" + ObjectID).getValue());

                    //Проведён или нет
                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                        Ext.Msg.alert(lanOrgName, txtMsg020);
                        Ext.getCmp("btnHeldCancel" + ObjectID).show();
                    }
                    else {
                        Ext.getCmp("btnHelds" + ObjectID).show();
                        //Ext.getCmp("btnHelds1" + ObjectID).show();
                        //Ext.getCmp("btnHelds2" + ObjectID).show();
                    }
                    //Кнопку "Печать" - делаем активной"
                    //Ext.getCmp("btnPrint" + ObjectID).show();

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }

            //Возврат
        case "viewDocSecondHandRetailReturnTabsEdit_OLD": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid"; storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            //3. Табличная часть
            var storeDocSecondHandRetailTabsGrid = Ext.create("store.storeDocSecondHandRetailTabsGrid"); storeDocSecondHandRetailTabsGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeGrid: storeDocSecondHandRetailTabsGrid,
                storeDirReturnTypesGrid: Ext.getCmp("viewDocSecondHandRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: Ext.getCmp("viewDocSecondHandRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirDescriptionsGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Всегда зарезервирован (есть проблема с отменой проведения прихода)
            Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                if (New_Edit == 1) {

                    //Поля *** *** *** *** ***

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("grid_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    //Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    //Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    //1 шт
                    //UO_GridRecord.data.Quantity = 1;
                    //UO_GridRecord.data.DocID = 0;
                    form.loadRecord(UO_GridRecord);
                    //Ext.getCmp("Quantity" + ObjectID).setValue(1);
                    Ext.getCmp("DocID" + ObjectID).setValue(0);

                    //Кнопки *** *** *** *** ***

                    //UO_GridRecord.data.Discount = 0;
                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    //Ext.getCmp("btnHelds1" + ObjectID).show();
                    //Ext.getCmp("btnHelds2" + ObjectID).show();
                    //Ext.getCmp("btnRecord" + ObjectID).show();

                    //Значения формы
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);
                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                    Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));
                    Ext.getCmp("DocSecondHandRetailReturnID" + ObjectID).setValue(0);

                    //Фокус на кнопку "Расчет"
                    Ext.getCmp("btnHelds" + ObjectID).focus();

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);

                    //Ext.getCmp("DocSecondHandRetailID" + ObjectID).setValue(Ext.getCmp("NumberReal" + ObjectID).getValue());
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandRetailID" + ObjectID).getValue());

                    //Проведён или нет
                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                        Ext.Msg.alert(lanOrgName, txtMsg020);
                        Ext.getCmp("btnHeldCancel" + ObjectID).show();
                    }
                    else {
                        Ext.getCmp("btnHelds" + ObjectID).show();
                        //Ext.getCmp("btnHelds1" + ObjectID).show();
                        //Ext.getCmp("btnHelds2" + ObjectID).show();
                    }
                    //Кнопку "Печать" - делаем активной"
                    //Ext.getCmp("btnPrint" + ObjectID).show();

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }

        case "viewDocSecondHandReturnTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid"; storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            //3. Табличная часть
            //var storeDocSecondHandRetailTabsGrid = Ext.create("store.storeDocSecondHandRetailTabsGrid"); storeDocSecondHandRetailTabsGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                //storeGrid: storeDocSecondHandRetailTabsGrid,
                storeDirReturnTypesGrid: Ext.getCmp("viewDocSecondHandSalesEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: Ext.getCmp("viewDocSecondHandSalesEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirDescriptionsGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Всегда зарезервирован (есть проблема с отменой проведения прихода)
            Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                if (New_Edit == 1) {

                    //Поля *** *** *** *** ***

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("grid_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    //Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    //Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    //1 шт
                    //UO_GridRecord.data.Quantity = 1;
                    //UO_GridRecord.data.DocID = 0;
                    form.loadRecord(UO_GridRecord);
                    //Ext.getCmp("Quantity" + ObjectID).setValue(1);
                    Ext.getCmp("DocID" + ObjectID).setValue(0);

                    //Кнопки *** *** *** *** ***

                    //UO_GridRecord.data.Discount = 0;
                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    //Ext.getCmp("btnHelds1" + ObjectID).show();
                    //Ext.getCmp("btnHelds2" + ObjectID).show();
                    //Ext.getCmp("btnRecord" + ObjectID).show();

                    //Значения формы
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);
                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                    Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));
                    Ext.getCmp("DocSecondHandReturnID" + ObjectID).setValue(0);

                    //Фокус на кнопку "Расчет"
                    Ext.getCmp("btnHelds" + ObjectID).focus();

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);

                    //Ext.getCmp("DocSecondHandRetailID" + ObjectID).setValue(Ext.getCmp("NumberReal" + ObjectID).getValue());
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandRetailID" + ObjectID).getValue());

                    //Проведён или нет
                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                        Ext.Msg.alert(lanOrgName, txtMsg020);
                        Ext.getCmp("btnHeldCancel" + ObjectID).show();
                    }
                    else {
                        Ext.getCmp("btnHelds" + ObjectID).show();
                        //Ext.getCmp("btnHelds1" + ObjectID).show();
                        //Ext.getCmp("btnHelds2" + ObjectID).show();
                    }
                    //Кнопку "Печать" - делаем активной"
                    //Ext.getCmp("btnPrint" + ObjectID).show();

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }

            //Списание
        case "viewDocSecondHandRetailActWriteOffsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid"; storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            //3. Табличная часть
            var storeDocSecondHandRetailTabsGrid = Ext.create("store.storeDocSecondHandRetailTabsGrid"); storeDocSecondHandRetailTabsGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeGrid: storeDocSecondHandRetailTabsGrid,
                storeDirReturnTypesGrid: Ext.getCmp("viewDocSecondHandRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: Ext.getCmp("viewDocSecondHandRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirDescriptionsGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Всегда зарезервирован (есть проблема с отменой проведения прихода)
            Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                if (New_Edit == 1) {

                    //Поля *** *** *** *** ***

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirServiceNomenName" + ObjectID).setValue(IdcallModelData.DirServiceNomenName);
                    Ext.getCmp("DirServiceNomenID" + ObjectID).setValue(IdcallModelData.DirServiceNomenID);

                    //Новый товар
                    //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                    UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                    UO_GridRecord.data.Quantity = 1;
                    UO_GridRecord.data.Description = UO_GridRecord.data.DirDescriptionName;
                    form.loadRecord(UO_GridRecord);

                    //Кнопки *** *** *** *** ***

                    //UO_GridRecord.data.Discount = 0;
                    Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    //Ext.getCmp("btnHelds1" + ObjectID).show();
                    //Ext.getCmp("btnHelds2" + ObjectID).show();
                    //Ext.getCmp("btnRecord" + ObjectID).show();

                    //Значения формы
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);
                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                    Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));//Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(Ext.getCmp("DocDate" + Ext.getCmp(UO_idCall).UO_id).getValue(), "Y-m-d"));
                    Ext.getCmp("DocSecondHandRetailActWriteOffID" + ObjectID).setValue(0);

                    //Фокус на кнопку "Расчет"
                    Ext.getCmp("btnHelds" + ObjectID).focus();

                }
                else if (New_Edit == 2 || New_Edit == 3) {

                    // !!! !!! !!! НЕ ИСПОЛЬЗУЕТСЯ !!! !!! !!!

                    form.loadRecord(UO_GridRecord);

                    //Ext.getCmp("DocSecondHandRetailActWriteOffID" + ObjectID).setValue(Ext.getCmp("NumberReal" + ObjectID).getValue());
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandRetailActWriteOffID" + ObjectID).getValue());

                    //Проведён или нет
                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                        Ext.Msg.alert(lanOrgName, txtMsg020);
                        Ext.getCmp("btnHeldCancel" + ObjectID).show();
                    }
                    else {
                        Ext.getCmp("btnHelds" + ObjectID).show();
                        //Ext.getCmp("btnHelds1" + ObjectID).show();
                        //Ext.getCmp("btnHelds2" + ObjectID).show();
                    }
                    //Кнопку "Печать" - делаем активной"
                    //Ext.getCmp("btnPrint" + ObjectID).show();

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }

            /* Перемещение */

        case "viewDocSecondHandMovementsEdit_OLD": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = [0];
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection();
                if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                    return;
                }
            }
            else {
                IdcallModelData.DocSecondHandMovementID = 0;
            }

            //Если Логистика
            if (IdcallModelData.DocSecondHandMovementID == undefined && IdcallModelData.LogisticID > 0) {
                IdcallModelData.DocSecondHandMovementID = IdcallModelData.LogisticID
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocSecondHandMovementID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //1. Store Grid
            var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //Если есть параметр "TreeServerParam1", то изменить URL
            //if (GridServerParam1 != undefined) storeDirServiceNomenTree.proxy.url = HTTP_DirNomensTree + "?" + GridServerParam1;

            //2. Combo
            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            //Store ComboGrid "Contractors"
            //var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=2&DirContractor2TypeID2=4";
            //Store ComboGrid "Warehouses"
            var storeDirWarehousesGridFrom = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridFrom.setData([], false); storeDirWarehousesGridFrom.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            //Store ComboGrid "Warehouses" (для документа "DocSecondHandMovements" показать все склады)
            var storeDirWarehousesGridTo = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridTo.setData([], false); storeDirWarehousesGridTo.proxy.url = HTTP_DirWarehouses + "?type=Grid&ListObjectID=33";
            //2.2. DirMovementDescriptions
            var storeDirMovementDescriptionsGrid = Ext.create("store.storeDirMovementDescriptionsGrid"); storeDirMovementDescriptionsGrid.setData([], false);
            //2.2. DirEmployeesGrid
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            //3. Табличная часть
            var storeDocSecondHandMovementTabsGrid = Ext.create("store.storeDocSecondHandMovementTabsGrid"); storeDocSecondHandMovementTabsGrid.setData([], false);
            storeDocSecondHandMovementTabsGrid.proxy.url = HTTP_DocSecondHandMovementTabs + "?DocSecondHandMovementID=" + IdcallModelData.DocSecondHandMovementID;
            //4. Партии
            var storeRem2PartiesGrid = Ext.create("store.storeRem2PartiesGrid"); storeRem2PartiesGrid.setData([], false);


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirServiceNomenTree: storeDirServiceNomenTree,
                storeGrid: storeDocSecondHandMovementTabsGrid,
                storeRem2PartiesGrid: storeRem2PartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirWarehousesGridFrom: storeDirWarehousesGridFrom,
                storeDirWarehousesGridTo: storeDirWarehousesGridTo,

                storeDirEmployeesGrid: storeDirEmployeesGrid,

                storeDirMovementDescriptionsGrid: storeDirMovementDescriptionsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel"
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();
            
            //Событие на загрузку в Grid
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; 
                    storeDirContractorsOrgGrid.UO_Loaded = true;

                    storeDirWarehousesGridFrom.load({ waitMsg: lanLoading });
                    storeDirWarehousesGridFrom.on('load', function () {
                        if (storeDirWarehousesGridFrom.UO_Loaded) return;
                        storeDirWarehousesGridFrom.UO_Loaded = true;

                        storeDirWarehousesGridTo.load({ waitMsg: lanLoading });
                        storeDirWarehousesGridTo.on('load', function () {
                            if (storeDirWarehousesGridTo.UO_Loaded) return;
                            storeDirWarehousesGridTo.UO_Loaded = true;

                            storeDirMovementDescriptionsGrid.proxy.url = HTTP_DirMovementDescriptions + "?type=Grid";
                            storeDirMovementDescriptionsGrid.load({ waitMsg: lanLoading });
                            storeDirMovementDescriptionsGrid.on('load', function () {

                                storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                                storeDirEmployeesGrid.on('load', function () {
                                    if (storeDirEmployeesGrid.UO_Loaded) return;
                                    storeDirEmployeesGrid.UO_Loaded = true;


                                        loadingMask.hide();

                                        if (New_Edit == 1) {

                                            //Если новая запись, то установить "по умолчанию"

                                            //Дата
                                            Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                            //Скидка
                                            //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                            //Сумма с Налогом
                                            Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                            //Наименование окна (сверху)
                                            widgetX.setTitle(widgetX.title + " № Новая");

                                            //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                            Ext.getCmp("btnRecord" + ObjectID).show();


                                            //Справочники
                                            //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID);
                                            //Ext.getCmp("DirVatValue" + ObjectID).setValue(0);


                                            //Склад и Организация привязанные к сотруднику
                                            //Склад
                                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                            else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                            //Организация
                                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                            //Остаток по Складу: Присваиваем Товару - Склад
                                            //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                            //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                            //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();



                                            //alert(Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue());
                                            loadingMask.show();
                                            storeRem2PartiesGrid.proxy.url =
                                                HTTP_Rem2Parties +
                                                "?DirServiceNomenID=0" +
                                                "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                                //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                                                //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                                                "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();
                                            storeRem2PartiesGrid.load({ waitMsg: lanLoading });
                                            storeRem2PartiesGrid.on('load', function () {
                                                if (storeRem2PartiesGrid.UO_Loaded) return;
                                                storeRem2PartiesGrid.UO_Loaded = true;

                                                loadingMask.hide();

                                            });



                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Разблокировка вызвавшего окна
                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                        }
                                        else if (New_Edit == 2 || New_Edit == 3) {

                                            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                                            //ArrList - значит
                                            if (!ArrList) {

                                                storeDocSecondHandMovementTabsGrid.load({ waitMsg: lanLoading });
                                                storeDocSecondHandMovementTabsGrid.on('load', function () {
                                                    if (storeDocSecondHandMovementTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                                    storeDocSecondHandMovementTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"


                                                    //Если форма уже загружена выходим!
                                                    if (widgetXForm.UO_Loaded) return;

                                                    widgetXForm.load({
                                                        method: "GET",
                                                        timeout: varTimeOutDefault,
                                                        waitMsg: lanLoading,
                                                        url: HTTP_DocSecondHandMovements + IdcallModelData.DocSecondHandMovementID + "/?DocID=" + IdcallModelData.DocID,
                                                        success: function (form, action) {


                                                            //alert(Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue());
                                                            loadingMask.show();
                                                            storeRem2PartiesGrid.proxy.url =
                                                                HTTP_Rem2Parties +
                                                                "?DirServiceNomenID=0" +
                                                                "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                                                //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                                                                //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                                                                "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();
                                                            storeRem2PartiesGrid.load({ waitMsg: lanLoading });
                                                            storeRem2PartiesGrid.on('load', function () {
                                                                if (storeRem2PartiesGrid.UO_Loaded) return;
                                                                storeRem2PartiesGrid.UO_Loaded = true;

                                                                loadingMask.hide();

                                                            });


                                                            widgetXForm.UO_Loaded = true;
                                                            //Фокус на открывшийся Виджет
                                                            widgetX.focus();

                                                            //Если Копия
                                                            if (New_Edit == 3) {
                                                                Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSecondHandMovementID" + ObjectID).setValue();
                                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                                            }
                                                            else {
                                                                //Наименование окна (сверху)
                                                                widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandMovementID" + ObjectID).getValue());

                                                                //Проведён или нет
                                                                if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                                    Ext.Msg.alert(lanOrgName, txtMsg020);
                                                                    Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                                }
                                                                else {
                                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                                }
                                                                Ext.getCmp("btnPrint" + ObjectID).show();
                                                            }


                                                            //!!! ОСТОРОЖНО !!! Нельзя менять параметры после загрузки!!!
                                                            /*
                                                            //Склад и Организация привязанные к сотруднику
                                                            //Склад
                                                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                                            else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                                            //Организация
                                                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }
                                                            */

                                                            //Остаток по Складу: Присваиваем Товару - Склад
                                                            //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                                            //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                                            //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                                            //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                            Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                            //Разблокировка вызвавшего окна
                                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                        },
                                                        failure: function (form, action) {
                                                            //loadingMask.hide();
                                                            widgetX.close();
                                                            funPanelSubmitFailure(form, action);

                                                            //Фокус на открывшийся Виджет
                                                            widgetX.focus();

                                                            //Разблокировка вызвавшего окна
                                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                        }
                                                    });


                                                });


                                            } //if(!ArrList)
                                                //Создать "На основании ..."
                                            else {

                                                //Переменные
                                                var formRec = ArrList[0];
                                                var gridRec = ArrList[1];
                                                //var locDirWarehouseID = ArrList[2];
                                                //Форма
                                                var form = widgetXForm.getForm();
                                                formRec.data.DirWarehouseIDFrom = formRec.data.DirWarehouseID;
                                                form.loadRecord(formRec);
                                                //Грид
                                                //storeDocSecondHandMovementTabsGrid.load({ waitMsg: lanLoading });
                                                for (var i = 0; i < gridRec.data.length; i++) storeDocSecondHandMovementTabsGrid.add(gridRec.data.items[i].data);
                                                /*{
                                                    gridRec.data.items[i].data.Quantity = gridRec.data.items[i].data.Remnant;
                                                    storeDocSecondHandMovementTabsGrid.add(gridRec.data.items[i].data);
                                                }*/

                                                // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                                                widgetXForm.UO_Loaded = true;
                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Если Копия
                                                if (New_Edit == 3) {
                                                    Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSecondHandMovementID" + ObjectID).setValue(null);
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                else {
                                                    //Наименование окна (сверху)
                                                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandMovementID" + ObjectID).getValue());
                                                    //Проведён или нет
                                                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                        Ext.Msg.alert(lanOrgName, txtMsg020);
                                                        Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                    }
                                                    else {
                                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                                    }
                                                    Ext.getCmp("btnPrint" + ObjectID).show();
                                                }


                                                //Дата
                                                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                                //Причина: Брак
                                                Ext.getCmp("DescriptionMovement" + ObjectID).setValue("Брак");
                                                //Дата
                                                Ext.getCmp("Base" + ObjectID).setValue("На основании отчета по торговле, тип 'Брак'");
                                                //Склад и Организация привязанные к сотруднику
                                                //Склад
                                                //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(locDirWarehouseID);
                                                //Организация
                                                if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                                else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                                //Остаток по Складу: Присваиваем Товару - Склад
                                                //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                                //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                                //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                                //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                //Разблокировка вызвавшего окна
                                                //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                                //Прячим партии
                                                Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_TOP, true);
                                            }


                                        }


                                });
                            });
                        });
                    });
                });
            });


            //Убираем кнопки
            //Ext.getCmp("expandAll" + ObjectID).setVisible(false);
            //Ext.getCmp("collapseAll" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);

            break;
        }

        case "viewDocSecondHandMovsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = [0];
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection();
                if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                    return;
                }
            }
            else {
                IdcallModelData.DocSecondHandMovID = 0;
            }

            //Если Логистика
            if (IdcallModelData.DocSecondHandMovID == undefined && IdcallModelData.LogisticID > 0) {
                IdcallModelData.DocSecondHandMovID = IdcallModelData.LogisticID
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocSecondHandMovID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //1. Store Grid
            //var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //Если есть параметр "TreeServerParam1", то изменить URL
            //if (GridServerParam1 != undefined) storeDirServiceNomenTree.proxy.url = HTTP_DirNomensTree + "?" + GridServerParam1;

            //2. Combo
            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            //Store ComboGrid "Contractors"
            //var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=2&DirContractor2TypeID2=4";
            //Store ComboGrid "Warehouses"
            var storeDirWarehousesGridFrom = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridFrom.setData([], false); storeDirWarehousesGridFrom.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            //Store ComboGrid "Warehouses" (для документа "DocSecondHandMovs" показать все склады)
            var storeDirWarehousesGridTo = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridTo.setData([], false); storeDirWarehousesGridTo.proxy.url = HTTP_DirWarehouses + "?type=Grid&ListObjectID=33";
            //2.2. DirMovementDescriptions
            var storeDirMovementDescriptionsGrid = Ext.create("store.storeDirMovementDescriptionsGrid"); storeDirMovementDescriptionsGrid.setData([], false);
            //2.2. DirEmployeesGrid
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            //3. Табличная часть
            var storeDocSecondHandMovTabsGrid = Ext.create("store.storeDocSecondHandMovTabsGrid"); storeDocSecondHandMovTabsGrid.setData([], false);
            storeDocSecondHandMovTabsGrid.proxy.url = HTTP_DocSecondHandMovTabs + "?DocSecondHandMovID=" + IdcallModelData.DocSecondHandMovID;
            //4. Партии
            //var storeRem2PartiesGrid = Ext.create("store.storeRem2PartiesGrid"); storeRem2PartiesGrid.setData([], false);
            var storeDocSecondHandPurchesGrid = Ext.create("store.storeDocSecondHandPurchesGrid"); storeDocSecondHandPurchesGrid.setData([], false);


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                //storeDirServiceNomenTree: storeDirServiceNomenTree,
                storeGrid: storeDocSecondHandMovTabsGrid,

                storeDocSecondHandPurchesGrid: storeDocSecondHandPurchesGrid,
                //storeRem2PartiesGrid: storeRem2PartiesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirWarehousesGridFrom: storeDirWarehousesGridFrom,
                storeDirWarehousesGridTo: storeDirWarehousesGridTo,

                storeDirEmployeesGrid: storeDirEmployeesGrid,

                storeDirMovementDescriptionsGrid: storeDirMovementDescriptionsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel"
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            //Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            Ext.getCmp("Reserve" + ObjectID).setValue(true);
            Ext.getCmp("LoadFrom" + ObjectID).setValue(0); //*Ext.getCmp("LoadXFrom" + ObjectID).setValue(0);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            /*
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
                */

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return;
                    storeDirContractorsOrgGrid.UO_Loaded = true;

                    storeDirWarehousesGridFrom.load({ waitMsg: lanLoading });
                    storeDirWarehousesGridFrom.on('load', function () {
                        if (storeDirWarehousesGridFrom.UO_Loaded) return;
                        storeDirWarehousesGridFrom.UO_Loaded = true;

                        storeDirWarehousesGridTo.load({ waitMsg: lanLoading });
                        storeDirWarehousesGridTo.on('load', function () {
                            if (storeDirWarehousesGridTo.UO_Loaded) return;
                            storeDirWarehousesGridTo.UO_Loaded = true;

                            storeDirMovementDescriptionsGrid.proxy.url = HTTP_DirMovementDescriptions + "?type=Grid";
                            storeDirMovementDescriptionsGrid.load({ waitMsg: lanLoading });
                            storeDirMovementDescriptionsGrid.on('load', function () {

                                storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                                storeDirEmployeesGrid.on('load', function () {
                                    if (storeDirEmployeesGrid.UO_Loaded) return;
                                    storeDirEmployeesGrid.UO_Loaded = true;


                                    loadingMask.hide();

                                    if (New_Edit == 1) {

                                        //Если новая запись, то установить "по умолчанию"

                                        //Дата
                                        Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                        //Скидка
                                        //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                        //Суммы
                                        Ext.getCmp("SumPurch" + ObjectID).setValue(0);
                                        Ext.getCmp("SumRetail" + ObjectID).setValue(0);
                                        //Наименование окна (сверху)
                                        widgetX.setTitle(widgetX.title + " № Новая");

                                        //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                        Ext.getCmp("btnRecord" + ObjectID).show();


                                        //Справочники
                                        //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID);
                                        //Ext.getCmp("DirVatValue" + ObjectID).setValue(0);


                                        //Склад и Организация привязанные к сотруднику
                                        //Склад
                                        if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                        else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                        //Организация
                                        if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                        else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                        //Остаток по Складу: Присваиваем Товару - Склад
                                        //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                        //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                        //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();



                                        //alert(Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue());
                                        loadingMask.show();
                                        /*
                                        storeRem2PartiesGrid.proxy.url =
                                            HTTP_Rem2Parties +
                                            "?DirServiceNomenID=0" +
                                            "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                            //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                                            //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                                            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();
                                        storeRem2PartiesGrid.load({ waitMsg: lanLoading });
                                        storeRem2PartiesGrid.on('load', function () {
                                            if (storeRem2PartiesGrid.UO_Loaded) return;
                                            storeRem2PartiesGrid.UO_Loaded = true;
                                            */

                                        storeDocSecondHandPurchesGrid.proxy.url =
                                            HTTP_DocSecondHandPurches +
                                            //"?DirServiceNomenID=0" +
                                            "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue() +
                                            "&DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=8";
                                            //+ "&DirSecondHandStatusID_789=7";

                                        storeDocSecondHandPurchesGrid.load({ waitMsg: lanLoading });
                                        storeDocSecondHandPurchesGrid.on('load', function () {
                                            if (storeDocSecondHandPurchesGrid.UO_Loaded) return;
                                            storeDocSecondHandPurchesGrid.UO_Loaded = true;

                                            loadingMask.hide();

                                        });



                                        //Фокус на открывшийся Виджет
                                        widgetX.focus();

                                        //Разблокировка вызвавшего окна
                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                    }
                                    else if (New_Edit == 2 || New_Edit == 3) {

                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                                        //ArrList - значит
                                        if (!ArrList) {

                                            storeDocSecondHandMovTabsGrid.load({ waitMsg: lanLoading });
                                            storeDocSecondHandMovTabsGrid.on('load', function () {
                                                if (storeDocSecondHandMovTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                                storeDocSecondHandMovTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"


                                                //Если форма уже загружена выходим!
                                                if (widgetXForm.UO_Loaded) return;

                                                widgetXForm.load({
                                                    method: "GET",
                                                    timeout: varTimeOutDefault,
                                                    waitMsg: lanLoading,
                                                    url: HTTP_DocSecondHandMovs + IdcallModelData.DocSecondHandMovID + "/?DocID=" + IdcallModelData.DocID,
                                                    success: function (form, action) {


                                                        //alert(Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue());
                                                        loadingMask.show();
                                                        /*
                                                        storeRem2PartiesGrid.proxy.url =
                                                            HTTP_Rem2Parties +
                                                            "?DirServiceNomenID=0" +
                                                            "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                                            //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                                                            //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                                                            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();
                                                        storeRem2PartiesGrid.load({ waitMsg: lanLoading });
                                                        storeRem2PartiesGrid.on('load', function () {
                                                            if (storeRem2PartiesGrid.UO_Loaded) return;
                                                            storeRem2PartiesGrid.UO_Loaded = true;
                                                            */
                                                        
                                                        storeDocSecondHandPurchesGrid.proxy.url =
                                                            HTTP_DocSecondHandPurches +
                                                            //"?DirServiceNomenID=0" +
                                                            "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                                            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue() +
                                                            "&DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=9";
                                                            //+ "&DirSecondHandStatusID_789=7";

                                                        storeDocSecondHandPurchesGrid.load({ waitMsg: lanLoading });
                                                        storeDocSecondHandPurchesGrid.on('load', function () {
                                                            if (storeDocSecondHandPurchesGrid.UO_Loaded) return;
                                                            storeDocSecondHandPurchesGrid.UO_Loaded = true;

                                                            loadingMask.hide();

                                                        });


                                                        widgetXForm.UO_Loaded = true;
                                                        //Фокус на открывшийся Виджет
                                                        widgetX.focus();

                                                        //Если Копия
                                                        if (New_Edit == 3) {
                                                            Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSecondHandMovID" + ObjectID).setValue();
                                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                                        }
                                                        else {
                                                            //Наименование окна (сверху)
                                                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandMovID" + ObjectID).getValue());

                                                            //Проведён или нет
                                                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                                                Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                            }
                                                            else {
                                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                                            }
                                                            Ext.getCmp("btnPrint" + ObjectID).show();
                                                        }


                                                        //!!! ОСТОРОЖНО !!! Нельзя менять параметры после загрузки!!!
                                                        /*
                                                        //Склад и Организация привязанные к сотруднику
                                                        //Склад
                                                        if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                                        else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                                        //Организация
                                                        if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                                        else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }
                                                        */

                                                        //Остаток по Складу: Присваиваем Товару - Склад
                                                        //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                                        //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                                        //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                                        //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                                        Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                                        //Разблокировка вызвавшего окна
                                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                    },
                                                    failure: function (form, action) {
                                                        //loadingMask.hide();
                                                        widgetX.close();
                                                        funPanelSubmitFailure(form, action);

                                                        //Фокус на открывшийся Виджет
                                                        widgetX.focus();

                                                        //Разблокировка вызвавшего окна
                                                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                                    }
                                                });


                                            });


                                        } //if(!ArrList)
                                        //Создать "На основании ..."
                                        else {

                                            //Переменные
                                            var formRec = ArrList[0];
                                            var gridRec = ArrList[1];
                                            //var locDirWarehouseID = ArrList[2];
                                            //Форма
                                            var form = widgetXForm.getForm();
                                            formRec.data.DirWarehouseIDFrom = formRec.data.DirWarehouseID;
                                            form.loadRecord(formRec);
                                            //Грид
                                            //storeDocSecondHandMovTabsGrid.load({ waitMsg: lanLoading });
                                            for (var i = 0; i < gridRec.data.length; i++) storeDocSecondHandMovTabsGrid.add(gridRec.data.items[i].data);
                                            /*{
                                                gridRec.data.items[i].data.Quantity = gridRec.data.items[i].data.Remnant;
                                                storeDocSecondHandMovTabsGrid.add(gridRec.data.items[i].data);
                                            }*/

                                            // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                                            widgetXForm.UO_Loaded = true;
                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Если Копия
                                            if (New_Edit == 3) {
                                                Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocSecondHandMovID" + ObjectID).setValue(null);
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            else {
                                                //Наименование окна (сверху)
                                                widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandMovID" + ObjectID).getValue());
                                                //Проведён или нет
                                                if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                    Ext.Msg.alert(lanOrgName, txtMsg020);
                                                    Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                }
                                                else {
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                Ext.getCmp("btnPrint" + ObjectID).show();
                                            }


                                            //Дата
                                            Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                            //Причина: Брак
                                            Ext.getCmp("DescriptionMovement" + ObjectID).setValue("Брак");
                                            //Дата
                                            Ext.getCmp("Base" + ObjectID).setValue("На основании отчета по торговле, тип 'Брак'");
                                            //Склад и Организация привязанные к сотруднику
                                            //Склад
                                            //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(locDirWarehouseID);
                                            //Организация
                                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                            //Остаток по Складу: Присваиваем Товару - Склад
                                            //if (varDirWarehouseIDEmpl == 0) {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                            //else {storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                            //storeDirServiceNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                            //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                            Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                            //Разблокировка вызвавшего окна
                                            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                            //Прячим партии
                                            Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_TOP, true);
                                        }


                                    }


                                });
                            });
                        });
                    });
                });

            //});


            //Убираем кнопки
            //Ext.getCmp("expandAll" + ObjectID).setVisible(false);
            //Ext.getCmp("collapseAll" + ObjectID).setVisible(false);
            /*
            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);
            */

            break;
        }

            /* Перемещение: Редактирование Грида */

        case "viewDocSecondHandMovementTabsEdit_OLD": {
      
            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,

                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                
                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    //Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirServiceNomenName" + ObjectID).setValue(IdcallModelData.DirServiceNomenName);
                    Ext.getCmp("DirServiceNomenID" + ObjectID).setValue(IdcallModelData.DirServiceNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                        //UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            break;
        }

        case "viewDocSecondHandMovTabsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,

                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    //Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirServiceNomenName" + ObjectID).setValue(IdcallModelData.DirServiceNomenName);
                    Ext.getCmp("DirServiceNomenID" + ObjectID).setValue(IdcallModelData.DirServiceNomenID);

                    if (GridTree) {
                        //Редактирование (загрузить из грида)
                        form.loadRecord(UO_GridRecord);
                    }
                    else {
                        //Новый товар
                        //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                        //UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                        UO_GridRecord.data.Quantity = 1;
                        form.loadRecord(UO_GridRecord);
                    }

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);
                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            break;
        }

        /* Инв */

        case "viewDocSecondHandInventoriesEdit_OLD": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData;
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
                }
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            //var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //2.1. DirReturnTypes
            var storeDirReturnTypesGrid = Ext.create("store.storeDirReturnTypesGrid"); storeDirReturnTypesGrid.setData([], false);
            //2.2. DirDescriptions
            var storeDirDescriptionsGrid = Ext.create("store.storeDirDescriptionsGrid"); storeDirDescriptionsGrid.setData([], false);
            //3. Табличная часть
            var storeDocSecondHandInventoryTabsGrid = Ext.create("store.storeDocSecondHandInventoryTabsGrid"); storeDocSecondHandInventoryTabsGrid.setData([], false); //storeDocSecondHandInventoryTabsGrid.proxy.url = HTTP_DocSecondHandRetailTabs + "?DocSecondHandRetailID=" + IdcallModelData.DocSecondHandRetailID;
            storeDocSecondHandInventoryTabsGrid.UO_viewConfig = false; //Надо для подсчёта суммы при редактировании Грида (см. viewConfig в Гриде)
            storeDocSecondHandInventoryTabsGrid.UO_id = ObjectID; 
            //4. Партии
            var storeRem2PartiesGrid = Ext.create("store.storeRem2PartiesGrid"); storeRem2PartiesGrid.setData([], false);
            //5. Склад
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            //5. Сотрудник
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";

            if (varStoreDirPaymentTypesGrid == undefined) {
                varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
            }


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                //storeDirServiceNomenTree: storeDirServiceNomenTree,
                storeGrid: storeDocSecondHandInventoryTabsGrid,
                storeRem2PartiesGrid: storeRem2PartiesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,

                storeDirReturnTypesGrid: varStoreDirReturnTypesGrid, //storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: storeDirDescriptionsGrid,

                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirEmployeesGrid: storeDirEmployeesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Убираем в Гриде не нужные поля
            /*var columns = Ext.getCmp('gridParty_' + ObjectID).columns;
            for (i = 0; i < columns.length; i++) {
                if (columns[i].dataIndex == "DirContractorName" || columns[i].dataIndex == "DirCharMaterialName") {
                    columns[i].setVisible(false);
                    columns[i].hide();
                }
            }*/

            //Прячем дерево Аппаратов
            //Ext.getCmp("tree_" + ObjectID).collapse();

            //Сумма с Налогом
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title);
            //Фокус на открывшийся Виджет
            widgetX.focus();
            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            /*
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
                */

                storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                storeDirWarehousesGrid.on('load', function () {
                    if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                    storeDirEmployeesGrid.on('load', function () {
                        if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        storeDirDescriptionsGrid.proxy.url = HTTP_DirDescriptions + "?type=Grid";
                        storeDirDescriptionsGrid.load({ waitMsg: lanLoading });
                        storeDirDescriptionsGrid.on('load', function () {

                            //storeDirReturnTypesGrid.proxy.url = HTTP_DirReturnTypes + "?type=Grid";
                            //storeDirReturnTypesGrid.load({ waitMsg: lanLoading });
                            //storeDirReturnTypesGrid.on('load', function () {


                            if (Ext.getCmp("TriggerSearchTree" + ObjectID)) Ext.getCmp("TriggerSearchTree" + ObjectID).focus();


                            //Склад и Организация привязанные к сотруднику
                            //Если у Сотрудника выбран Склад и Организация - блокируем их!
                            //Склад
                            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                            //Для "остаток по складу": Присваиваем Товару - Склад
                            //if (varDirWarehouseIDEmpl == 0) { storeDirServiceNomenTree.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue(); }
                            //else { storeDirServiceNomenTree.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl; }

                            //Организация
                            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                            //Тип цен
                            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

                            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
                            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
                            //Ext.getCmp("SearchType" + ObjectID).setValue(1);
                            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                            //Дата
                            //Ext.getCmp("DocDateS" + ObjectID).setValue(new Date());
                            //Ext.getCmp("DocDatePo" + ObjectID).setValue(new Date());


                            loadingMask.hide();

                            
                            //Если "Edit" (не новый документ)
                            //if (IdcallModelData.DocSecondHandInventoryID > 0) {
                            if (New_Edit == 1) {

                                //Если новая запись, то установить "по умолчанию"
                                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                Ext.getCmp("SumOfVATCurrency1" + ObjectID).setValue(0);
                                Ext.getCmp("SumOfVATCurrency2" + ObjectID).setValue(0);

                                //Сумма с Налогом
                                widgetX.setTitle(widgetX.title + " № Новая");

                                Ext.getCmp("SpisatS" + ObjectID).setValue(1);

                                //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                Ext.getCmp("btnHelds" + ObjectID).show();
                                Ext.getCmp("btnRecord" + ObjectID).show();

                                storeDocSecondHandInventoryTabsGrid.UO_viewConfig = true;

                                //Фокус на открывшийся Виджет
                                widgetX.focus();

                            }
                            else if (New_Edit == 2 || New_Edit == 3) {

                                storeDocSecondHandInventoryTabsGrid.proxy.url =
                                    HTTP_DocSecondHandInventoryTabs +
                                    //"?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                                    //"&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                                    //"&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                                    //"&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                                    "?DocSecondHandInventoryID=" + IdcallModelData.DocSecondHandInventoryID; //Ext.getCmp("DocSecondHandInventoryID" + ObjectID).getValue();

                                storeDocSecondHandInventoryTabsGrid.load({ waitMsg: lanLoading });
                                storeDocSecondHandInventoryTabsGrid.on('load', function () {
                                    if (storeDocSecondHandInventoryTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                    storeDocSecondHandInventoryTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
                                    storeDocSecondHandInventoryTabsGrid.UO_viewConfig = true;

                                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                    //Если форма уже загружена выходим!
                                    if (widgetXForm.UO_Loaded) return;

                                    widgetXForm.load({
                                        method: "GET",
                                        timeout: varTimeOutDefault,
                                        waitMsg: lanLoading,
                                        url: HTTP_DocSecondHandInventories + IdcallModelData.DocSecondHandInventoryID + "/?DocID=" + IdcallModelData.DocID,
                                        success: function (form, action) {

                                            widgetXForm.UO_Loaded = true;
                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Проведён или нет
                                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                                Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                            }
                                            else {
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            Ext.getCmp("btnPrint" + ObjectID).show();

                                        },
                                        failure: function (form, action) {
                                            //loadingMask.hide();
                                            widgetX.close();
                                            funPanelSubmitFailure(form, action);

                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Разблокировка вызвавшего окна
                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                        }
                                    });


                                });

                            }


                        });
                    });
                });


            //});


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            /*
            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);
            */
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);


            break;
        }

        case "viewDocSecondHandInvsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData;
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
                }
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            //var storeDirServiceNomenTree = Ext.create("store.storeDirServiceNomensTree"); storeDirServiceNomenTree.setData([], false);
            //2.1. DirReturnTypes
            var storeDirReturnTypesGrid = Ext.create("store.storeDirReturnTypesGrid"); storeDirReturnTypesGrid.setData([], false);
            //2.2. DirDescriptions
            var storeDirDescriptionsGrid = Ext.create("store.storeDirDescriptionsGrid"); storeDirDescriptionsGrid.setData([], false);
            //3. Табличная часть
            var storeDocSecondHandInvTabsGrid = Ext.create("store.storeDocSecondHandInvTabsGrid"); storeDocSecondHandInvTabsGrid.setData([], false);
            storeDocSecondHandInvTabsGrid.UO_viewConfig = false; //Надо для подсчёта суммы при редактировании Грида (см. viewConfig в Гриде)
            storeDocSecondHandInvTabsGrid.UO_id = ObjectID;
            //4. Партии
            //var storeRem2PartiesGrid = Ext.create("store.storeRem2PartiesGrid"); storeRem2PartiesGrid.setData([], false);
            var storeDocSecondHandPurchesGrid = Ext.create("store.storeDocSecondHandPurchesGrid"); storeDocSecondHandPurchesGrid.setData([], false);
            //5. Склад
            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            //5. Сотрудник
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";
            //6. Подписи
            var storeDirEmployees2Grid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployees2Grid.setData([], false); storeDirEmployees2Grid.proxy.url = HTTP_DirEmployees + "?type=Grid&DirWarehouseID=" + varDirWarehouseIDEmpl;

            if (varStoreDirPaymentTypesGrid == undefined) {
                varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
            }


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                //storeDirServiceNomenTree: storeDirServiceNomenTree,
                storeGrid: storeDocSecondHandInvTabsGrid, //storeDocSecondHandInvTabsGrid,
                //storeRem2PartiesGrid: storeRem2PartiesGrid,
                storeDocSecondHandPurchesGrid: storeDocSecondHandPurchesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,

                storeDirReturnTypesGrid: varStoreDirReturnTypesGrid, //storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: storeDirDescriptionsGrid,

                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeDirEmployees2Grid: storeDirEmployees2Grid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Убираем в Гриде не нужные поля
            /*var columns = Ext.getCmp('gridParty_' + ObjectID).columns;
            for (i = 0; i < columns.length; i++) {
                if (columns[i].dataIndex == "DirContractorName" || columns[i].dataIndex == "DirCharMaterialName") {
                    columns[i].setVisible(false);
                    columns[i].hide();
                }
            }*/

            //Прячем дерево Аппаратов
            //Ext.getCmp("tree_" + ObjectID).collapse();

            //Сумма с Налогом
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Наименование окна (сверху)
            //widgetX.setTitle(widgetX.title);
            //Фокус на открывшийся Виджет
            widgetX.focus();
            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            /*
            storeDirServiceNomenTree.on('load', function () {
                if (storeDirServiceNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeDirServiceNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
                */

            storeDirWarehousesGrid.load({ waitMsg: lanLoading });
            storeDirWarehousesGrid.on('load', function () {
                if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                storeDirEmployeesGrid.on('load', function () {
                    if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                    
                    storeDirEmployees2Grid.load({ waitMsg: lanLoading });
                    storeDirEmployees2Grid.on('load', function () {
                        if (storeDirEmployees2Grid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirEmployees2Grid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () 

                        storeDirDescriptionsGrid.proxy.url = HTTP_DirDescriptions + "?type=Grid";
                        storeDirDescriptionsGrid.load({ waitMsg: lanLoading });
                        storeDirDescriptionsGrid.on('load', function () {
                            
                            if (Ext.getCmp("TriggerSearchTree" + ObjectID)) Ext.getCmp("TriggerSearchTree" + ObjectID).focus();
                            
                            //Склад и Организация привязанные к сотруднику
                            //Если у Сотрудника выбран Склад и Организация - блокируем их!
                            //Склад

                            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
                            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

                            //Организация
                            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                            //Тип цен
                            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);


                            loadingMask.hide();


                            if (New_Edit == 1) {
                                // *** *** *** Новый *** *** ***

                                //Если новая запись, то установить "по умолчанию"
                                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                Ext.getCmp("SumOfVATCurrency1" + ObjectID).setValue(0);
                                Ext.getCmp("SumOfVATCurrency2" + ObjectID).setValue(0);

                                //Сумма с Налогом
                                widgetX.setTitle(widgetX.title + " № Новая");

                                Ext.getCmp("SpisatS" + ObjectID).setValue(1);

                                //Товаровед
                                Ext.getCmp("DirEmployee1ID" + ObjectID).setValue(varDirEmployeeID);

                                //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                Ext.getCmp("btnHelds" + ObjectID).show();
                                Ext.getCmp("btnRecord" + ObjectID).show();

                                storeDocSecondHandInvTabsGrid.UO_viewConfig = true;

                                //Фокус на открывшийся Виджет
                                widgetX.focus();

                            }
                            else if (New_Edit == 2 || New_Edit == 3) {

                                // *** *** *** Редактируем или копия *** *** ***

                                storeDocSecondHandInvTabsGrid.proxy.url =
                                    HTTP_DocSecondHandInvTabs + "?DocSecondHandInvID=" + IdcallModelData.DocSecondHandInvID;

                                storeDocSecondHandInvTabsGrid.load({ waitMsg: lanLoading });
                                storeDocSecondHandInvTabsGrid.on('load', function () {
                                    if (storeDocSecondHandInvTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                                    storeDocSecondHandInvTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeDirServiceNomenTree.on('load', function () {"
                                    storeDocSecondHandInvTabsGrid.UO_viewConfig = true;

                                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                                    //Если форма уже загружена выходим!
                                    if (widgetXForm.UO_Loaded) return;

                                    widgetXForm.load({
                                        method: "GET",
                                        timeout: varTimeOutDefault,
                                        waitMsg: lanLoading,
                                        url: HTTP_DocSecondHandInvs + IdcallModelData.DocSecondHandInvID + "/?DocID=" + IdcallModelData.DocID,
                                        success: function (form, action) {

                                            widgetXForm.UO_Loaded = true;
                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Проведён или нет
                                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                                Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                            }
                                            else {
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            Ext.getCmp("btnPrint" + ObjectID).show();

                                            
                                            //Подписи
                                            if (Ext.getCmp("DirEmployee1ID" + ObjectID).getValue() == varDirEmployeeID) {
                                                //1. Не подписал Товаровед
                                                if (!Ext.getCmp("DirEmployee1Podpis" + ObjectID).getValue()) {
                                                    Ext.getCmp("btn1Podpis" + ObjectID).setDisabled(false);
                                                }
                                                else
                                                    //2. Если подписал Товаровед, но не подписал Админ
                                                    if (Ext.getCmp("DirEmployee1Podpis" + ObjectID).getValue() && !Ext.getCmp("DirEmployee2Podpis" + ObjectID).getValue()) {
                                                        Ext.getCmp("btn11Podpis" + ObjectID).setDisabled(false);

                                                        Ext.getCmp("DirEmployee2ID" + ObjectID).setReadOnly(false);
                                                    }
                                                    else
                                                        //2. Если подписал Админ
                                                        if (Ext.getCmp("DirEmployee2Podpis" + ObjectID).getValue()) {
                                                            Ext.getCmp("DirEmployee2ID" + ObjectID).setReadOnly(true);

                                                            var sMsg = "Документ уже подписан Админом точки!";
                                                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                                sMsg = txtMsg020;
                                                            }
                                                            Ext.Msg.alert(lanOrgName, sMsg);
                                                        }
                                            }
                                            else if (Ext.getCmp("DirEmployee2ID" + ObjectID).getValue() == varDirEmployeeID) {
                                                
                                                Ext.getCmp("DirEmployee2ID" + ObjectID).setReadOnly(true);

                                                //1. Подписал Товаровед
                                                if (Ext.getCmp("DirEmployee1Podpis" + ObjectID).getValue() && !Ext.getCmp("DirEmployee2Podpis" + ObjectID).getValue()) {
                                                    Ext.getCmp("btn2Podpis" + ObjectID).setDisabled(false);
                                                }
                                                else
                                                    //2. Если подписал Товаровед и подписал Админ
                                                    if (Ext.getCmp("DirEmployee1Podpis" + ObjectID).getValue() && Ext.getCmp("DirEmployee2Podpis" + ObjectID).getValue()) {
                                                        Ext.getCmp("btn2Podpis" + ObjectID).setDisabled(true);
                                                        Ext.getCmp("btn21Podpis" + ObjectID).setDisabled(false);
                                                    }
                                                    else {
                                                        Ext.Msg.alert(lanOrgName, "Документ ещё не подписан Товароведом!");
                                                    }
                                            }


                                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocSecondHandInvID" + ObjectID).getValue());

                                        },
                                        failure: function (form, action) {
                                            //loadingMask.hide();
                                            widgetX.close();
                                            funPanelSubmitFailure(form, action);

                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Разблокировка вызвавшего окна
                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                        }
                                    });


                                });

                            }


                        });

                    });
                });
            });


            //});


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            /*
            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);
            */
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);


            break;
        }

        /* БУ.Разбор: Добавление запчасти */

        case "viewDocSecondHandRazborNomens": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //DocSecondHandPurchID (DocSecondHandRazborID)
            Ext.getCmp("DocSecondHandPurchID" + ObjectID).setValue(Ext.getCmp("DocSecondHandPurchID" + Ext.getCmp(UO_idCall).UO_id).getValue());

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsGrid.load({ waitMsg: lanLoading });
                storeDirContractorsGrid.on('load', function () {
                    if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    loadingMask.hide();
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    if (New_Edit == 1) { // - Не используется !!!
                        /*
                        //Если новая запись, то установить "по умолчанию"
                        Ext.getCmp("btnDel" + ObjectID).setVisible(false);
                        //Если наценки отрицательные, то ставим их из Настроек
                        funMarkupSet(ObjectID);
                        */
                    }
                    else if (New_Edit == 2 || New_Edit == 3) {
                        //Если редактировать, то: Загрузка данных в Форму "widgetXPanel"
                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                        if (UO_GridSave) {
                            varPriceChange_ReadOnly = true; //Запретить редактировать цены
                            //Форма
                            var form = widgetXForm.getForm();

                            if (GridTree) {
                                //Редактирование (загрузить из грида)
                                form.loadRecord(UO_GridRecord);
                            }
                            else {
                                //Новый товар
                                //Может возникнуть ситуация, когда не выбран товар
                                if (UO_GridRecord != undefined) {
                                    Ext.getCmp("DirNomenID" + ObjectID).setValue(UO_GridRecord.data.id);
                                    Ext.getCmp("DirNomenName" + ObjectID).setValue(UO_GridRecord.data.text);


                                    //Пробегаемся по всем партиям и ищим с последней датой
                                    //Если не находим, то ставим всё "по нулям"
                                    //1. Грид Party *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                                    var id = Ext.getCmp(UO_idCall).UO_id;


                                    //Выбранная партия
                                    /*
                                    var IdcallModelData = Ext.getCmp("gridParty_" + id).getSelectionModel().getSelection();

                                    //1. Если не выбрана партия товара
                                    if (IdcallModelData.length == 0) {

                                        var PanelParty = Ext.getCmp("gridParty_" + id).store.data.items;

                                        //2. Выбираем данные из партии
                                        if (PanelParty.length > 0) {
                                            //2.1. Если есть Партии, то выбираем самую последнюю
                                            UO_GridRecord = PanelParty[0];
                                            for (var i = PanelParty.length - 1; i > 0; i--) { //for (var i = 1; i < PanelParty.length; i++) {
                                                if (PanelParty[i].data.DocDate > UO_GridRecord.data.DocDate) UO_GridRecord = PanelParty[i];
                                            }
                                        }
                                        else {
                                            //2.2. Если нет Партии, то делаем запрос на Сервер за Партией, которые уже проданы,
                                            //     если на Сервере тоже нет данных выдаём сообщение
                                            fun_viewDocPurchTabsEdit_RequestPrice(form, UO_GridRecord, ObjectID);

                                        }
                                    }

                                    //2. Если выбрана партия товара, то её и берём на основу!
                                    else {
                                        UO_GridRecord = IdcallModelData[0]
                                    }
                                    */


                                    fun_viewDocPurchTabsEdit_RequestPrice(form, UO_GridRecord, ObjectID);


                                    form.loadRecord(UO_GridRecord);
                                    Ext.getCmp("Quantity" + ObjectID).setValue(1);
                                    //Мин.остаток
                                    Ext.getCmp("DirNomenMinimumBalance" + ObjectID).setValue(varDirNomenMinimumBalance);



                                    //Поставщик
                                    /*
                                    var locDirContractorID = Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id).getRawValue();
                                    var comboBox = Ext.getCmp("DirCharStyleID" + ObjectID);
                                    var store = comboBox.store;
                                    var locResult = store.findExact("DirCharStyleName", locDirContractorID);
                                    Ext.getCmp("DirCharStyleID" + ObjectID).setValue(store.getAt(locResult));
                                    */
                                    /*if (Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id)) {
                                        var locDirContractorID = Ext.getCmp("DirContractorID" + Ext.getCmp(UO_idCall).UO_id).getRawValue();
                                        var comboBox = Ext.getCmp("DirContractorID" + ObjectID);
                                        var store = comboBox.store;
                                        var locResult = store.findExact("DirContractorName", locDirContractorID);
                                        Ext.getCmp("DirContractorID" + ObjectID).setValue(store.getAt(locResult));
                                    }*/


                                }
                            }
                            form.UO_Loaded = true;
                            varPriceChange_ReadOnly = false; //Разрешить редактировать цены
                        }
                    }

                }); //storeDirContractorsGrid
            }); //storeDirCurrenciesGrid

            break;
        }




            //Оплата

        case "viewPayEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                UO_GridServerParam1: UO_GridServerParam1,

                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                loadingMask.hide();

                if (New_Edit == 1) {

                    //Если новая запись, то установить "по умолчанию"

                    //Дата
                    Ext.getCmp("DocID" + ObjectID).setValue(Ext.getCmp("DocID" + Ext.getCmp(Ext.getCmp(UO_idCall).UO_idCall).UO_id).getValue());
                    //Ext.getCmp("DocXID" + ObjectID).setValue(Ext.getCmp("DocID" + Ext.getCmp(Ext.getCmp(UO_idCall).UO_idCall).UO_id).getValue());
                    //Дата
                    Ext.getCmp("DocXSumDate" + ObjectID).setValue(new Date());
                    //Тип оплаты
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    //Валюта
                    Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                    Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                    Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);
                    //Доплаить == сумма
                    var HavePay = Ext.getCmp("HavePay" + Ext.getCmp(Ext.getCmp(UO_idCall).UO_idCall).UO_id).getValue();
                    Ext.getCmp("DocXSumSum" + ObjectID).setValue(HavePay);

                    //Фокус на открывшийся Виджет
                    widgetX.focus();

                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                }
                else if (New_Edit == 2 || New_Edit == 3) {

                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                    //Если форма уже загружена выходим!
                    if (widgetXForm.UO_Loaded) return;

                    widgetXForm.load({
                        method: "GET",
                        timeout: varTimeOutDefault,
                        waitMsg: lanLoading,
                        url: HTTP_Pays + IdcallModelData.DocCashBankID + "/?DirPaymentTypeID=" + IdcallModelData.DirPaymentTypeID,
                        success: function (form, action) {

                            widgetXForm.UO_Loaded = true;
                            //Фокус на открывшийся Виджет
                            widgetX.focus();
                            //Наименование окна (сверху)
                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocCashBankID" + ObjectID).getValue());
                            //Разблокировка вызвавшего окна
                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                        },
                        failure: function (form, action) {
                            //loadingMask.hide();
                            widgetX.close();
                            funPanelSubmitFailure(form, action);

                            //Фокус на открывшийся Виджет
                            widgetX.focus();

                            //Разблокировка вызвавшего окна
                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                        }
                    });

                }


            }); //storeDirCurrenciesGrid

            break;
        }



            //Деньги: Касса + Банк *** *** ***

            /* Касса */

        case "viewDocCashOfficeSumsEdit": {
            
            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DirCashOfficeID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;


            //Store Combo "storeDirEmployeesGrid"
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false);
            storeDirEmployeesGrid.proxy.url = storeDirEmployeesGrid.proxy.url + "?type=Grid";
            storeDirEmployeesGrid.load({ waitMsg: lanLoading });

            var storeReportBanksCashOffices = Ext.create("store.storeReportBanksCashOffices");
            storeReportBanksCashOffices.setData([], false);
            storeReportBanksCashOffices.proxy.url =
                HTTP_ReportBanksCashOffices +
                "?pLanguage=1&" +
                //"DateS=" + Ext.Date.format(Ext.getCmp("DateS" + ObjectID).getValue(), 'Y-m-d') + "&" +
                //"DatePo=" + Ext.Date.format(Ext.getCmp("DatePo" + ObjectID).getValue(), 'Y-m-d') + "&" +
                //"DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() + "&" +
                "DirCashOfficeID=" + IdcallModelData.DirCashOfficeID + "&" +
                //"DirEmployeeID=" + Ext.getCmp("DirEmployeeID" + ObjectID).getValue() + "&" +
                //"CasheAndBank=" + Ext.getCmp("CasheAndBank" + ObjectID).getValue() + "&" +
                "Cashe=true&" +
                //"Bank=" + Ext.getCmp("Bank" + ObjectID).getValue() + "&" +
                "ReportType=1&";
                //"DocXID=" + Ext.getCmp("DocXID" + ObjectID).getValue();
            storeReportBanksCashOffices.pageSize = varPageSizeJurn;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
                storeReportBanksCashOffices: storeReportBanksCashOffices,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeDirEmployeesGrid.on('load', function () {
                if (storeDirEmployeesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirEmployeesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                storeReportBanksCashOffices.load({ waitMsg: lanLoading });
                storeReportBanksCashOffices.on('load', function () {
                    if (storeReportBanksCashOffices.UO_Loaded) return;
                    storeReportBanksCashOffices.UO_Loaded = true;


                    loadingMask.hide();

                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                    //Если форма уже загружена выходим!
                    //if (widgetXForm.UO_Loaded) return;

                    widgetXForm.load({
                        method: "GET",
                        timeout: varTimeOutDefault,
                        waitMsg: lanLoading,
                        url: HTTP_DirCashOffices + IdcallModelData.DirCashOfficeID + "/", // + "/?DocID=" + IdcallModelData.DocID,
                        success: function (form, action) {
                            widgetXForm.UO_Loaded = true;
                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DirCashOfficeID" + ObjectID).getValue());
                            widgetX.focus();

                            //Разблокировка вызвавшего окна
                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                        },
                        failure: function (form, action) {
                            //loadingMask.hide();
                            widgetX.close();
                            funPanelSubmitFailure(form, action);

                            //Фокус на открывшийся Виджет
                            widgetX.focus();

                            //Разблокировка вызвавшего окна
                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                        }
                    });

                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }

            /* Банк */

        case "viewDocBankSumsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DirBankID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;


            var storeReportBanksCashOffices = Ext.create("store.storeReportBanksCashOffices");
            storeReportBanksCashOffices.setData([], false);
            storeReportBanksCashOffices.proxy.url =
                HTTP_ReportBanksCashOffices +
                "?pLanguage=1&" +
                //"DateS=" + Ext.Date.format(Ext.getCmp("DateS" + ObjectID).getValue(), 'Y-m-d') + "&" +
                //"DatePo=" + Ext.Date.format(Ext.getCmp("DatePo" + ObjectID).getValue(), 'Y-m-d') + "&" +
                //"DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() + "&" +
                "DirBankID=" + IdcallModelData.DirBankID + "&" +
                //"DirEmployeeID=" + Ext.getCmp("DirEmployeeID" + ObjectID).getValue() + "&" +
                //"CasheAndBank=" + Ext.getCmp("CasheAndBank" + ObjectID).getValue() + "&" +
                //"Cashe=true&" +
                "Bank=true&" +
                "ReportType=1&";
                //"DocXID=" + Ext.getCmp("DocXID" + ObjectID).getValue();
            storeReportBanksCashOffices.pageSize = varPageSizeJurn;


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeReportBanksCashOffices: storeReportBanksCashOffices,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

            storeReportBanksCashOffices.load({ waitMsg: lanLoading });
            storeReportBanksCashOffices.on('load', function () {
                if (storeReportBanksCashOffices.UO_Loaded) return;
                storeReportBanksCashOffices.UO_Loaded = true;

                widgetXForm.load({
                    method: "GET",
                    timeout: varTimeOutDefault,
                    waitMsg: lanLoading,
                    url: HTTP_DirBanks + IdcallModelData.DirBankID + "/", // + "/?DocID=" + IdcallModelData.DocID,
                    success: function (form, action) {
                        widgetXForm.UO_Loaded = true;
                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DirBankID" + ObjectID).getValue());
                        widgetX.focus();

                        //Разблокировка вызвавшего окна
                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                    },
                    failure: function (form, action) {
                        //loadingMask.hide();
                        widgetX.close();
                        funPanelSubmitFailure(form, action);

                        //Фокус на открывшийся Виджет
                        widgetX.focus();

                        //Разблокировка вызвавшего окна
                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                    }
                });

            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }


        /* Перемещение */

        case "viewDocCashOfficeSumMovementsEdit": {
            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = [0];
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection();
                if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                    return;
                }
            }
            else {
                IdcallModelData.DocCashOfficeSumMovementID = 0;
            }


            //Если Логистика
            if (IdcallModelData.DocCashOfficeSumMovementID == undefined && IdcallModelData.LogisticID > 0) {
                IdcallModelData.DocCashOfficeSumMovementID = IdcallModelData.LogisticID
            }


            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.DocCashOfficeSumMovementID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;


            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //2. Combo
            //Store Combo "ContractorsOrg"
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";



            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirCashOfficesGrid = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGrid.setData([], false); storeDirCashOfficesGrid.proxy.url = HTTP_DirCashOffices + "?type=Grid";

            //Store ComboGrid "Warehouses"
            //var storeDirWarehousesGridFrom = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridFrom.setData([], false); storeDirWarehousesGridFrom.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            //var storeDirCashOfficesGridFrom = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGridFrom.setData([], false); storeDirCashOfficesGridFrom.proxy.url = HTTP_DirCashOffices + "?type=Grid";

            //Store ComboGrid "Warehouses" (для документа "DocCashOfficeSumMovements" показать все склады)
            //var storeDirWarehousesGridTo = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGridTo.setData([], false); storeDirWarehousesGridTo.proxy.url = HTTP_DirWarehouses + "?type=Grid&ListObjectID=79"; //33
            //var storeDirCashOfficesGridTo = Ext.create("store.storeDirCashOfficesGrid"); storeDirCashOfficesGridTo.setData([], false); storeDirCashOfficesGridTo.proxy.url = HTTP_DirCashOffices + "?type=Grid";

            //2.2. DirEmployeesGrid
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,


                storeDirWarehousesGridFrom: storeDirWarehousesGrid,
                storeDirCashOfficesGridFrom: storeDirCashOfficesGrid,

                storeDirWarehousesGridTo: storeDirWarehousesGrid,
                storeDirCashOfficesGridTo: storeDirCashOfficesGrid,

                //storeDirWarehousesGridFrom: storeDirWarehousesGridFrom,
                //storeDirCashOfficesGridFrom: storeDirCashOfficesGridFrom,

                //storeDirWarehousesGridTo: storeDirWarehousesGridTo,
                //storeDirCashOfficesGridTo: storeDirCashOfficesGridTo,

                storeDirEmployeesGrid: storeDirEmployeesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            //Прячем правое меню сообщений: "MessageRightPanel"
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            //Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            //Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
            storeDirContractorsOrgGrid.on('load', function () {
                if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                storeDirWarehousesGrid.on('load', function () {
                    if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirCashOfficesGrid.load({ waitMsg: lanLoading });
                    storeDirCashOfficesGrid.on('load', function () {
                        if (storeDirCashOfficesGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirCashOfficesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                        
                        storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                        storeDirEmployeesGrid.on('load', function () {
                            if (storeDirEmployeesGrid.UO_Loaded) return;
                            storeDirEmployeesGrid.UO_Loaded = true;

                            loadingMask.hide();

                            if (New_Edit == 1) {

                                //Если новая запись, то установить "по умолчанию"

                                //Дата
                                Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                //Скидка
                                //Ext.getCmp("Discount" + ObjectID).setValue(0);
                                //Сумма с Налогом
                                //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
                                //Наименование окна (сверху)
                                widgetX.setTitle(widgetX.title + " № Новая");

                                //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                Ext.getCmp("btnHelds" + ObjectID).show();
                                Ext.getCmp("btnRecord" + ObjectID).show();


                                //Справочники
                                //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID);
                                //Ext.getCmp("DirVatValue" + ObjectID).setValue(0);


                                //Склад и Организация привязанные к сотруднику

                                //Организация
                                if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                //Склад
                                if (varDirWarehouseIDEmpl == 0) {
                                    Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID);
                                }
                                else {
                                    Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl);
                                }

                                //Касса
                                var locDirWarehouseIDFrom = Ext.getCmp("DirWarehouseIDFrom" + ObjectID).valueCollection.items[0].data;
                                Ext.getCmp("DirCashOfficeIDFrom" + ObjectID).setValue(locDirWarehouseIDFrom.DirCashOfficeID);
                                //Валюты
                                Ext.getCmp("DirCurrencyID" + ObjectID).setValue(locDirWarehouseIDFrom.DirCurrencyID);
                                Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(locDirWarehouseIDFrom.DirCurrencyRate);
                                Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(locDirWarehouseIDFrom.DirCurrencyMultiplicity);
                                //Сумма
                                Ext.getCmp("DirCashOfficeSumFrom" + ObjectID).setValue(locDirWarehouseIDFrom.DirCashOfficeSum);


                                //Остаток по Складу: Присваиваем Товару - Склад
                                //if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                //else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                //storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();

                                //Фокус на открывшийся Виджет
                                widgetX.focus();

                                //Разблокировка вызвавшего окна
                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                            }
                            else if (New_Edit == 2 || New_Edit == 3) {

                                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);

                                //ArrList - значит
                                if (!ArrList) {

                                    //Если форма уже загружена выходим!
                                    if (widgetXForm.UO_Loaded) return;


                                    widgetXForm.load({
                                        method: "GET",
                                        timeout: varTimeOutDefault,
                                        waitMsg: lanLoading,
                                        url: HTTP_DocCashOfficeSumMovements + IdcallModelData.DocCashOfficeSumMovementID + "/?DocID=" + IdcallModelData.DocID,
                                        success: function (form, action) {

                                            widgetXForm.UO_Loaded = true;
                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Если Копия
                                            if (New_Edit == 3) {
                                                Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocCashOfficeSumMovementID" + ObjectID).setValue();
                                                Ext.getCmp("btnHelds" + ObjectID).show();
                                                Ext.getCmp("btnRecord" + ObjectID).show();
                                            }
                                            else {
                                                //Наименование окна (сверху)
                                                widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocCashOfficeSumMovementID" + ObjectID).getValue());

                                                //Проведён или нет
                                                if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                                    Ext.Msg.alert(lanOrgName, txtMsg020);
                                                    Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                                }
                                                else {
                                                    Ext.getCmp("btnHelds" + ObjectID).show();
                                                    Ext.getCmp("btnRecord" + ObjectID).show();
                                                }
                                                Ext.getCmp("btnPrint" + ObjectID).show();
                                            }


                                            //!!! ОСТОРОЖНО !!! Нельзя менять параметры после загрузки!!!
                                            /*
                                            //Склад и Организация привязанные к сотруднику
                                            //Склад
                                            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseID); }
                                            else { Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(varDirWarehouseIDEmpl); }
                                            //Организация
                                            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }
                                            */

                                            //Остаток по Складу: Присваиваем Товару - Склад
                                            //if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                            //else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                            //storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                            //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                            //Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                            //Разблокировка вызвавшего окна
                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                        },
                                        failure: function (form, action) {
                                            //loadingMask.hide();
                                            widgetX.close();
                                            funPanelSubmitFailure(form, action);

                                            //Фокус на открывшийся Виджет
                                            widgetX.focus();

                                            //Разблокировка вызвавшего окна
                                            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                        }
                                    });


                                } //if(!ArrList)
                                //Создать "На основании ..."
                                else {

                                    //Переменные
                                    var formRec = ArrList[0];
                                    var gridRec = ArrList[1];
                                    //var locDirWarehouseID = ArrList[2];
                                    //Форма
                                    var form = widgetXForm.getForm();
                                    formRec.data.DirWarehouseIDFrom = formRec.data.DirWarehouseID;
                                    form.loadRecord(formRec);
                                    //Грид
                                    //storeDocCashOfficeSumMovementTabsGrid.load({ waitMsg: lanLoading });
                                    for (var i = 0; i < gridRec.data.length; i++) storeDocCashOfficeSumMovementTabsGrid.add(gridRec.data.items[i].data);
                                    /*{
                                        gridRec.data.items[i].data.Quantity = gridRec.data.items[i].data.Remnant;
                                        storeDocCashOfficeSumMovementTabsGrid.add(gridRec.data.items[i].data);
                                    }*/

                                    // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                                    widgetXForm.UO_Loaded = true;
                                    //Фокус на открывшийся Виджет
                                    widgetX.focus();

                                    //Если Копия
                                    if (New_Edit == 3) {
                                        Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("DocCashOfficeSumMovementID" + ObjectID).setValue(null);
                                        Ext.getCmp("btnHelds" + ObjectID).show();
                                        Ext.getCmp("btnRecord" + ObjectID).show();
                                    }
                                    else {
                                        //Наименование окна (сверху)
                                        widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocCashOfficeSumMovementID" + ObjectID).getValue());
                                        //Проведён или нет
                                        if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                            Ext.Msg.alert(lanOrgName, txtMsg020);
                                            Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                                        }
                                        else {
                                            Ext.getCmp("btnHelds" + ObjectID).show();
                                            Ext.getCmp("btnRecord" + ObjectID).show();
                                        }
                                        Ext.getCmp("btnPrint" + ObjectID).show();
                                    }


                                    //Дата
                                    Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
                                    //Причина: Брак
                                    Ext.getCmp("DescriptionMovement" + ObjectID).setValue("Брак");
                                    //Дата
                                    Ext.getCmp("Base" + ObjectID).setValue("На основании отчета по торговле, тип 'Брак'");
                                    //Склад и Организация привязанные к сотруднику
                                    //Склад
                                    //Ext.getCmp("DirWarehouseIDFrom" + ObjectID).setValue(locDirWarehouseID);
                                    //Организация
                                    if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                    else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

                                    //Остаток по Складу: Присваиваем Товару - Склад
                                    //if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();}
                                    //else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}
                                    storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + ObjectID).getValue();


                                    //Всегда зарезервирован (есть проблема с отменоц проведения прихода)
                                    Ext.getCmp("Reserve" + ObjectID).setValue(true);
                                    //Разблокировка вызвавшего окна
                                    //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                                    //Прячим партии
                                    Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_TOP, true);
                                }


                            }

                        });

                    });
                });
            });


            //Убираем кнопки
            //Ext.getCmp("expandAll" + ObjectID).setVisible(false);
            //Ext.getCmp("collapseAll" + ObjectID).setVisible(false);
            /*Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);*/

            break;
        }



            //Retail *** *** ***


            /* Розница */

        case "viewDocRetailsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData;
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
                }
            }

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //1. Store Grid
            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false);
            //2.1. DirReturnTypes
            var storeDirReturnTypesGrid = Ext.create("store.storeDirReturnTypesGrid"); storeDirReturnTypesGrid.setData([], false);
            //2.2. DirDescriptions
            var storeDirDescriptionsGrid = Ext.create("store.storeDirDescriptionsGrid"); storeDirDescriptionsGrid.setData([], false);
            //3. Табличная часть
            var storeDocRetailTabsGrid = Ext.create("store.storeDocRetailTabsGrid"); storeDocRetailTabsGrid.setData([], false); //storeDocRetailTabsGrid.proxy.url = HTTP_DocRetailTabs + "?DocRetailID=" + IdcallModelData.DocRetailID;
            //4. Партии
            var storeRemPartiesGrid = Ext.create("store.storeRemPartiesGrid"); storeRemPartiesGrid.setData([], false);

            if (varStoreDirPaymentTypesGrid == undefined) {
                varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
            }



            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeGrid: storeDocRetailTabsGrid,
                storeRemPartiesGrid: storeRemPartiesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,

                storeDirReturnTypesGrid: varStoreDirReturnTypesGrid, //storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: storeDirDescriptionsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Убираем в Гриде не нужные поля
            /*var columns = Ext.getCmp('gridParty_' + ObjectID).columns;
            for (i = 0; i < columns.length; i++) {
                if (columns[i].dataIndex == "DirContractorName" || columns[i].dataIndex == "DirCharMaterialName") {
                    columns[i].setVisible(false);
                    columns[i].hide();
                }
            }*/

            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true);}
            if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
            else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
            //Для "остаток по складу": Присваиваем Товару - Склад
            if (varDirWarehouseIDEmpl == 0) {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();}
            else {storeNomenTree.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + varDirWarehouseIDEmpl;}

            //Организация
            if (varDirContractorIDOrgEmpl > 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true);}
            if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
            else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

            //Тип цен
            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

            //Прячем правое меню сообщений: "MessageRightPanel" - в Рознице надо показать!!!
            //Ext.getCmp("gridParty_" + ObjectID).collapse(Ext.Component.DIRECTION_NORTH, true);
            Ext.getCmp("SearchType" + ObjectID).setValue(1);
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Дата
            Ext.getCmp("DocDateS" + ObjectID).setValue(new Date());
            Ext.getCmp("DocDatePo" + ObjectID).setValue(new Date());
            //Сумма с Налогом
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title);
            //Фокус на открывшийся Виджет
            widgetX.focus();
            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                storeDocRetailTabsGrid.proxy.url =
                    HTTP_DocRetailTabs +
                    "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + ObjectID).getValue() +
                    "&DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + ObjectID).getValue(), "Y-m-d") +
                    "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + ObjectID).getValue(), "Y-m-d") +
                    "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();

                storeDocRetailTabsGrid.load({ waitMsg: lanLoading });
                storeDocRetailTabsGrid.on('load', function () {
                    if (storeDocRetailTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDocRetailTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                    storeDirReturnTypesGrid.proxy.url = HTTP_DirReturnTypes + "?type=Grid";
                    storeDirReturnTypesGrid.load({ waitMsg: lanLoading });
                    storeDirReturnTypesGrid.on('load', function () {

                        storeDirDescriptionsGrid.proxy.url = HTTP_DirDescriptions + "?type=Grid";
                        storeDirDescriptionsGrid.load({ waitMsg: lanLoading });
                        storeDirDescriptionsGrid.on('load', function () {
                            
                            loadingMask.hide();
                            
                            if (Ext.getCmp("TriggerSearchTree" + ObjectID)) Ext.getCmp("TriggerSearchTree" + ObjectID).focus();

                        });
                    });

                });
            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);


            Ext.getCmp("FolderNew" + ObjectID).setVisible(false);
            Ext.getCmp("FolderNewSub" + ObjectID).setVisible(false);
            Ext.getCmp("FolderCopy" + ObjectID).setVisible(false);
            Ext.getCmp("FolderDel" + ObjectID).setVisible(false);
            //Ext.getCmp("tree_" + ObjectID).collapse(Ext.Component.DIRECTION_LEFT, true);

            break;
        }

        case "viewDocRetailTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid"; storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            //3. Табличная часть
            var storeDocRetailTabsGrid = Ext.create("store.storeDocRetailTabsGrid"); storeDocRetailTabsGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,

                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeGrid: storeDocRetailTabsGrid,
                storeDirDescriptionsGrid: Ext.getCmp("viewDocRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirDescriptionsGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true);}
            if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
            else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}

            //Организация
            if (varDirContractorIDOrgEmpl > 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true);}
            if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
            else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

            //Всегда зарезервирован (есть проблема с отменой проведения прихода)
            Ext.getCmp("Reserve" + ObjectID).setValue(true);

            //title
            widgetX.setTitle(widgetX.title + " ("+ varPayTypeName + ")");

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();


                if (New_Edit == 1) {

                    //Поля *** *** *** *** ***

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    //Новый товар
                    //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                    UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                    UO_GridRecord.data.Quantity = 1;
                    form.loadRecord(UO_GridRecord);

                    //Полное наименование!
                    Ext.getCmp("DirNomenName" + ObjectID).setValue(Ext.getCmp("DirNomenPatchFull" + UO_Param_id).getValue()); //Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);

                    //Кнопки *** *** *** *** ***

                    //UO_GridRecord.data.Discount = 0;
                    Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    //Ext.getCmp("btnHelds1" + ObjectID).show();
                    //Ext.getCmp("btnHelds2" + ObjectID).show();
                    //Ext.getCmp("btnRecord" + ObjectID).show();

                    //Значения формы
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);
                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                    Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));//Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(Ext.getCmp("DocDate" + Ext.getCmp(UO_idCall).UO_id).getValue(), "Y-m-d"));
                    Ext.getCmp("DocRetailID" + ObjectID).setValue(0);

                    //Фокус на кнопку "Расчет"
                    Ext.getCmp("btnHelds" + ObjectID).focus();

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);

                    //Ext.getCmp("DocRetailID" + ObjectID).setValue(Ext.getCmp("NumberReal" + ObjectID).getValue());
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocRetailID" + ObjectID).getValue());

                    //Проведён или нет
                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                        Ext.Msg.alert(lanOrgName, txtMsg020);
                        Ext.getCmp("btnHeldCancel" + ObjectID).show();
                    }
                    else {
                        Ext.getCmp("btnHelds" + ObjectID).show();
                        //Ext.getCmp("btnHelds1" + ObjectID).show();
                        //Ext.getCmp("btnHelds2" + ObjectID).show();
                    }
                    //Кнопку "Печать" - делаем активной"
                    //Ext.getCmp("btnPrint" + ObjectID).show();

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }

        case "viewDocRetailActWriteOffsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid"; storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            //3. Табличная часть
            var storeDocRetailTabsGrid = Ext.create("store.storeDocRetailTabsGrid"); storeDocRetailTabsGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeGrid: storeDocRetailTabsGrid,
                storeDirReturnTypesGrid: Ext.getCmp("viewDocRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: Ext.getCmp("viewDocRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirDescriptionsGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Всегда зарезервирован (есть проблема с отменой проведения прихода)
            Ext.getCmp("Reserve" + ObjectID).setValue(true);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                if (New_Edit == 1) {

                    //Поля *** *** *** *** ***

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("gridParty_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    //Новый товар
                    //Запуск функция "fun_DirPriceTypeID_ChangePrice"
                    UO_GridRecord = fun_DirPriceTypeID_ChangePrice(UO_GridRecord, parseInt(Ext.getCmp("DirPriceTypeID" + Ext.getCmp(UO_idCall).UO_id).getValue()));
                    UO_GridRecord.data.Quantity = 1;
                    UO_GridRecord.data.Description = UO_GridRecord.data.DirDescriptionName;
                    form.loadRecord(UO_GridRecord);

                    //Кнопки *** *** *** *** ***

                    //UO_GridRecord.data.Discount = 0;
                    Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    //Ext.getCmp("btnHelds1" + ObjectID).show();
                    //Ext.getCmp("btnHelds2" + ObjectID).show();
                    //Ext.getCmp("btnRecord" + ObjectID).show();

                    //Значения формы
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);
                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                    Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));//Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(Ext.getCmp("DocDate" + Ext.getCmp(UO_idCall).UO_id).getValue(), "Y-m-d"));
                    Ext.getCmp("DocRetailActWriteOffID" + ObjectID).setValue(0);

                    //Фокус на кнопку "Расчет"
                    Ext.getCmp("btnHelds" + ObjectID).focus();

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    
                    // !!! !!! !!! НЕ ИСПОЛЬЗУЕТСЯ !!! !!! !!!

                    form.loadRecord(UO_GridRecord);

                    //Ext.getCmp("DocRetailActWriteOffID" + ObjectID).setValue(Ext.getCmp("NumberReal" + ObjectID).getValue());
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocRetailActWriteOffID" + ObjectID).getValue());

                    //Проведён или нет
                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                        Ext.Msg.alert(lanOrgName, txtMsg020);
                        Ext.getCmp("btnHeldCancel" + ObjectID).show();
                    }
                    else {
                        Ext.getCmp("btnHelds" + ObjectID).show();
                        //Ext.getCmp("btnHelds1" + ObjectID).show();
                        //Ext.getCmp("btnHelds2" + ObjectID).show();
                    }
                    //Кнопку "Печать" - делаем активной"
                    //Ext.getCmp("btnPrint" + ObjectID).show();

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }


            //Пока не удаляй ...
        case "viewDocRetailReturnsEdit_OLD": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData;
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
                }
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //2. Табличная часть
            var storeDocRetailReturnTabsGrid = Ext.create("store.storeDocRetailReturnTabsGrid"); storeDocRetailReturnTabsGrid.setData([], false);
            //3. Партии
            var storeRemPartyMinusesGrid = Ext.create("store.storeRemPartyMinusesGrid"); storeRemPartyMinusesGrid.setData([], false);

            if (varStoreDirPaymentTypesGrid == undefined) {
                varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
            }


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                //storeNomenTree: storeNomenTree,
                storeGrid: storeDocRetailReturnTabsGrid,
                storeRemPartyMinusesGrid: storeRemPartyMinusesGrid,

                //storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                //storeDirContractorsGrid: storeDirContractorsGrid,
                //storeDirWarehousesGrid: storeDirWarehousesGrid,
                //storeDirVatsGrid: storeDirVatsGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            if (varDirWarehouseIDEmpl > 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true);}
            if (varDirContractorIDOrgEmpl > 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true);}

            Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
            Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Тип цен
            Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);

            //Склад и Организация привязанные к сотруднику
            //Склад
            if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
            else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}
            //Организация
            if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
            else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

            //Дата
            Ext.getCmp("DocDate" + ObjectID).setValue(new Date());
            //Сумма с Налогом
            Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title + " № Новая");

            //Фокус на открывшийся Виджет
            widgetX.focus();


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeDocRetailReturnTabsGrid.proxy.url = HTTP_DocRetailReturnTabs + "?DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + ObjectID).getValue(), "Y-m-d") + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
            storeDocRetailReturnTabsGrid.load({ waitMsg: lanLoading });
            storeDocRetailReturnTabsGrid.on('load', function () {
                if (storeDocRetailReturnTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDocRetailReturnTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeRemPartyMinusesGrid.proxy.url = HTTP_RemPartyMinuses + "?DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + ObjectID).getValue(), "Y-m-d") + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + ObjectID).getValue();
                storeRemPartyMinusesGrid.load({ waitMsg: lanLoading });
                storeRemPartyMinusesGrid.on('load', function () {
                    if (storeRemPartyMinusesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeRemPartyMinusesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    loadingMask.hide();

                });
            });


            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

            Ext.Msg.alert(lanOrgName, "Документ создаётся на основании Продажи. Выберите документ Продажа.");

            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }


        case "viewDocRetailReturnTabsEdit": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeDirCurrenciesGrid"
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = storeDirCurrenciesGrid.proxy.url + "?type=Grid"; storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            //3. Табличная часть
            var storeDocRetailTabsGrid = Ext.create("store.storeDocRetailTabsGrid"); storeDocRetailTabsGrid.setData([], false);

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeGrid: storeDocRetailTabsGrid,
                storeDirReturnTypesGrid: Ext.getCmp("viewDocRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirReturnTypesGrid,
                storeDirDescriptionsGrid: Ext.getCmp("viewDocRetailsEdit" + Ext.getCmp(UO_idCall).UO_id).storeDirDescriptionsGrid,

                UO_GridServerParam1: UO_GridServerParam1
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true);}
            if (varDirWarehouseIDEmpl == 0) {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID);}
            else {Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl);}

            //Организация
            if (varDirContractorIDOrgEmpl > 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true);}
            if (varDirContractorIDOrgEmpl == 0) {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg);}
            else {Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl);}

            //Всегда зарезервирован (есть проблема с отменой проведения прихода)
            Ext.getCmp("Reserve" + ObjectID).setValue(true);

            //title
            widgetX.setTitle(widgetX.title + " (" + varPayTypeName + ")");

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                varPriceChange_ReadOnly = true; //Запретить редактировать цены
                var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                //Форма
                var form = widgetXForm.getForm();

                if (New_Edit == 1) {

                    //Поля *** *** *** *** ***

                    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
                    var IdcallModelData = Ext.getCmp("grid_" + UO_Param_id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(Ext.getCmp("DirPriceTypeID" + UO_Param_id).getValue());
                    //Ext.getCmp("DirNomenName" + ObjectID).setValue(IdcallModelData.DirNomenName);
                    //Ext.getCmp("DirNomenID" + ObjectID).setValue(IdcallModelData.DirNomenID);

                    //1 шт
                    //UO_GridRecord.data.Quantity = 1;
                    //UO_GridRecord.data.DocID = 0;
                    form.loadRecord(UO_GridRecord);
                    //Ext.getCmp("Quantity" + ObjectID).setValue(1);
                    Ext.getCmp("DocID" + ObjectID).setValue(0);

                    //Кнопки *** *** *** *** ***

                    //UO_GridRecord.data.Discount = 0;
                    //Ext.getCmp("Discount" + ObjectID).setValue(0);
                    //Ext.getCmp("btnHeldCancel" + ObjectID).show(); //.setVisible(false);
                    Ext.getCmp("btnHelds" + ObjectID).show();
                    //Ext.getCmp("btnHelds1" + ObjectID).show();
                    //Ext.getCmp("btnHelds2" + ObjectID).show();
                    //Ext.getCmp("btnRecord" + ObjectID).show();

                    //Значения формы
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                    Ext.getCmp("DirPriceTypeID" + ObjectID).setValue(varDirPriceTypeID);
                    Ext.getCmp("Reserve" + ObjectID).setValue(true); //Всегда зарезервирован (есть проблема с отменой проведения прихода)
                    Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));
                    Ext.getCmp("DocRetailReturnID" + ObjectID).setValue(0);

                    //Фокус на кнопку "Расчет"
                    Ext.getCmp("btnHelds" + ObjectID).focus();

                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    form.loadRecord(UO_GridRecord);

                    //Ext.getCmp("DocRetailID" + ObjectID).setValue(Ext.getCmp("NumberReal" + ObjectID).getValue());
                    Ext.getCmp("DirPaymentTypeID" + ObjectID).setValue(1);
                    Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                    //Наименование окна (сверху)
                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocRetailID" + ObjectID).getValue());

                    //Проведён или нет
                    if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                        Ext.Msg.alert(lanOrgName, txtMsg020);
                        Ext.getCmp("btnHeldCancel" + ObjectID).show();
                    }
                    else {
                        Ext.getCmp("btnHelds" + ObjectID).show();
                        //Ext.getCmp("btnHelds1" + ObjectID).show();
                        //Ext.getCmp("btnHelds2" + ObjectID).show();
                    }
                    //Кнопку "Печать" - делаем активной"
                    //Ext.getCmp("btnPrint" + ObjectID).show();

                }

                form.UO_Loaded = true;
                varPriceChange_ReadOnly = false; //Разрешить редактировать цены

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }




            //Разработчик ПФ *** *** ***

            /* Редактирование ПФ */

        case "viewListObjectPFsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName + "_" + IdcallModelData.ListObjectPFID;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeListLanguagesGrid"
            var storeListLanguagesGrid = Ext.create("store.storeListLanguagesGrid"); storeListLanguagesGrid.setData([], false);
            storeListLanguagesGrid.proxy.url = storeListLanguagesGrid.proxy.url + "?type=Grid";
            storeListLanguagesGrid.load({ waitMsg: lanLoading });

            //Табличная часть
            var storeListObjectPFTabsGrid = Ext.create("store.storeListObjectPFTabsGrid"); storeListObjectPFTabsGrid.setData([], false);
            storeListObjectPFTabsGrid.proxy.url = HTTP_ListObjectPFTabs + "?ListObjectPFID=" + IdcallModelData.ListObjectPFID;
            //storeListObjectPFTabsGrid.load();


            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,
                UO_Param_id: UO_Param_id, //это нужно, т.к. загрузка html происходит автоматически!

                storeListLanguagesGrid: storeListLanguagesGrid,
                storeListObjectPFTabsGrid: storeListObjectPFTabsGrid,
            });


            //Что за документ: Приход, Касса, Банка, Расход, ...
            Ext.getCmp("ListObjectID" + ObjectID).setValue(UO_Param_id);

            //Не нужно менять строку
            //мы передаём ID-шник "ListObjectID" через параметр "UO_Param_id"
            //var PanelHeaderEast = Ext.getCmp("PanelHeaderEast_" + ObjectID);
            //PanelHeaderEast.loader.url = HTTP_ListObjectFieldNames + "?ListObjectField=ListObjectFieldHeaderShow&ListObjectID=" + UO_Param_id; //IdcallModelData.ListObjectPFID;


            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeListLanguagesGrid.on('load', function () {
                if (storeListLanguagesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeListLanguagesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);


                if (New_Edit == 1) {
                    //Если новая запись, то установить "по умолчанию"

                    Ext.getCmp("ListObjectPFHtmlHeaderUse" + ObjectID).setValue(true);
                    Ext.getCmp("ListObjectPFHtmlFooterUse" + ObjectID).setValue(true);
                }
                else if (New_Edit == 2 || New_Edit == 3) {

                    loadingMask.show();
                    storeListObjectPFTabsGrid.load({ waitMsg: lanLoading });
                    storeListObjectPFTabsGrid.on('load', function () {
                        if (storeListObjectPFTabsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeListObjectPFTabsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        loadingMask.hide();

                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                        //Если форма уже загружена выходим!
                        if (widgetXForm.UO_Loaded) return;

                        widgetXForm.load({
                            method: "GET",
                            timeout: varTimeOutDefault,
                            waitMsg: lanLoading,
                            url: HTTP_ListObjectPFs + IdcallModelData.ListObjectPFID,
                            success: function (form, action) {

                                widgetXForm.UO_Loaded = true;
                                //Фокус на открывшийся Виджет
                                widgetX.focus();

                                //Если Копия
                                if (New_Edit == 3) {
                                    Ext.getCmp("DocID" + ObjectID).setValue(null); Ext.getCmp("ListObjectPFID" + ObjectID).setValue(null);
                                }
                                else {
                                    //Наименование окна (сверху)
                                    widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("ListObjectPFID" + ObjectID).getValue());
                                }

                                //Разблокировка вызвавшего окна
                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                            },
                            failure: function (form, action) {
                                //loadingMask.hide();
                                widgetX.close();
                                funPanelSubmitFailure(form, action);

                                //Фокус на открывшийся Виджет
                                widgetX.focus();

                                //Разблокировка вызвавшего окна
                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                            }
                        });

                    });

                }

            }); //storeListLanguagesGrid



            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }

            /* Редактирование табличной части ПФ */

        case "viewListObjectPFTabsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Store Combo "storeListObjectFieldNamesGrid"
            var storeListObjectFieldNamesGrid = Ext.create("store.storeListObjectFieldNamesGrid"); storeListObjectFieldNamesGrid.setData([], false);
            storeListObjectFieldNamesGrid.proxy.url = storeListObjectFieldNamesGrid.proxy.url + "?type=Grid&" + UO_GridServerParam1;
            storeListObjectFieldNamesGrid.load({ waitMsg: lanLoading });

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                UO_GridServerParam1: UO_GridServerParam1,

                storeListObjectFieldNamesGrid: storeListObjectFieldNamesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            storeListObjectFieldNamesGrid.on('load', function () {
                if (storeListObjectFieldNamesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeListObjectFieldNamesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();


                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);


                if (New_Edit == 1) {
                    //Если новая запись, то установить "по умолчанию"

                    Ext.getCmp("btnDel" + ObjectID).setVisible(false);
                }
                else if (New_Edit == 2 || New_Edit == 3) {
                    //Если редактировать, то: Загрузка данных в Форму "widgetXPanel"

                    var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);


                    if (UO_GridSave) {

                        //Форма
                        var form = widgetXForm.getForm();
                        //Если уже загружены данные - выйти
                        //if (form.UO_Loaded == true) return;

                        form.loadRecord(UO_GridRecord);

                        form.UO_Loaded = true;
                    }

                }

            }); //storeDirCurrenciesGrid

            //Разблокировка вызвавшего окна и Фокус на открывшийся Виджет
            //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
            //Фокус на открывшийся Виджет
            //widgetX.focus();

            break;
        }






            //Финансы *** *** ***


            /* Хоз.расходы */

        case "viewDocDomesticExpensesEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData;
            if (UO_idCall != "viewContainerHeader") {
                IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
                //Если запись помечена на удаление, то сообщить об этом и выйти
                if (IdcallModelData.Del == true) {
                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

                    Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK }); return;
                }
            }

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //Если создано "на Основании", то убрать вызвавший грид (спецификация Счета), т.к. после сохранения формы Продажа, спецификация Счета обновится!
            if (ArrList) {
                //Разблокировка вызвавшего окна
                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                //Что бы не обновляло вызвавший грид
                UO_idCall = undefined;
            }

            //1. Store Grid
            var storeDomesticExpenseTree = Ext.create("store.storeDirDomesticExpensesTree"); storeDomesticExpenseTree.setData([], false);


            //Сука!!!
            // !!! При смене даты (внизу) запускается Лоад() этого стора !!!
            //3. Табличная часть
            var storeDocDomesticExpenseTabsGrid = Ext.create("store.storeDocDomesticExpenseTabsGrid"); storeDocDomesticExpenseTabsGrid.setData([], false);
            //storeDocDomesticExpenseTabsGrid.proxy.url = HTTP_DocDomesticExpenseTabs + "?UO_GridIndex=" + UO_GridIndex;
            /*storeDocDomesticExpenseTabsGrid.load();
            storeDocDomesticExpenseTabsGrid.on('load', function () {
                var zzzz = storeDocDomesticExpenseTabsGrid;
            });
            */

            if (varStoreDirPaymentTypesGrid == undefined) {
                varStoreDirPaymentTypesGrid = Ext.create("store.storeDirPaymentTypesGrid"); varStoreDirPaymentTypesGrid.setData([], false); varStoreDirPaymentTypesGrid.proxy.url = HTTP_DirPaymentTypes + "?type=Grid"; varStoreDirPaymentTypesGrid.load({ waitMsg: lanLoading })
            }
            

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                UO_GridIndex: UO_GridIndex, //Тип: 1 - ЗП, 2 - Другое

                storeDomesticExpenseTree: storeDomesticExpenseTree,
                storeGrid: storeDocDomesticExpenseTabsGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);

            //Убираем в Гриде не нужные поля
            /*var columns = Ext.getCmp('gridParty_' + ObjectID).columns;
            for (i = 0; i < columns.length; i++) {
                if (columns[i].dataIndex == "DirContractorName" || columns[i].dataIndex == "DirCharMaterialName") {
                    columns[i].setVisible(false);
                    columns[i].hide();
                }
            }*/

            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }

            //Дата
            Ext.getCmp("DocDateS" + ObjectID).setValue(new Date());
            Ext.getCmp("DocDatePo" + ObjectID).setValue(new Date());
            //Сумма с Налогом
            //Ext.getCmp("SumOfVATCurrency" + ObjectID).setValue(0);
            //Наименование окна (сверху)
            widgetX.setTitle(widgetX.title);
            //Фокус на открывшийся Виджет
            widgetX.focus();
            //Разблокировка вызвавшего окна
            ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);

            break;
        }

        case "viewDocDomesticExpenseTabsEdit": {

            //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
            var IdcallModelData = Ext.getCmp(UO_idCall).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            //DirCurrencies
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";
            //DirDomesticExpenses
            var storeDirDomesticExpensesGrid = Ext.create("store.storeDirDomesticExpensesGrid"); storeDirDomesticExpensesGrid.setData([], false);
            storeDirDomesticExpensesGrid.proxy.url = HTTP_DirDomesticExpenses + "?type=Grid&UO_idTab=" + UO_idTab;
            //3. Табличная часть
            var storeDocDomesticExpenseTabsGrid = Ext.create("store.storeDocDomesticExpenseTabsGrid"); storeDocDomesticExpenseTabsGrid.setData([], false);
            //4. Сотрудник
            var storeDirEmployeesGrid = Ext.create("store.storeDirEmployeesGrid"); storeDirEmployeesGrid.setData([], false); storeDirEmployeesGrid.proxy.url = HTTP_DirEmployees + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID, //"win_" + pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                UO_idTab: UO_idTab, 

                UO_GridServerParam1: UO_GridServerParam1,

                storeDirPaymentTypesGrid: varStoreDirPaymentTypesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,
                storeDirDomesticExpensesGrid: storeDirDomesticExpensesGrid,
                storeGrid: storeDocDomesticExpenseTabsGrid,
                storeDirEmployeesGrid: storeDirEmployeesGrid,
            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            //ObjectShow(widgetX, pObjectName, ObjectID, UO_Modal);
            ObjectShow(widgetX);


            //Склад и Организация привязанные к сотруднику
            //Если у Сотрудника выбран Склад и Организация - блокируем их!
            //Склад
            if (varDirWarehouseIDEmpl > 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setReadOnly(true); }
            if (varDirWarehouseIDEmpl == 0) { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseID); }
            else { Ext.getCmp("DirWarehouseID" + ObjectID).setValue(varDirWarehouseIDEmpl); }

            //Организация
            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }
            if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
            else { Ext.getCmp("DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }


            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX }); loadingMask.show();

            storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
            storeDirCurrenciesGrid.on('load', function () {
                if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirDomesticExpensesGrid.load({ waitMsg: lanLoading });
                storeDirDomesticExpensesGrid.on('load', function () {
                    if (storeDirDomesticExpensesGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirDomesticExpensesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"
                    if (UO_idTab == 2) {
                        Ext.getCmp("DirDomesticExpenseID" + ObjectID).setValue(3);
                    }

                    storeDirEmployeesGrid.load({ waitMsg: lanLoading });
                    storeDirEmployeesGrid.on('load', function () {
                        if (storeDirEmployeesGrid.UO_Loaded) return;
                        storeDirEmployeesGrid.UO_Loaded = true;

                        loadingMask.hide();
                        //Разблокировка вызвавшего окна
                        ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);



                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                        //Форма
                        var form = widgetXForm.getForm();


                        if (New_Edit == 1) {

                            //Поля *** *** *** *** ***

                            //Ext.getCmp("DirDomesticExpenseID" + ObjectID).setValue(UO_GridRecord.data.id);
                            //Ext.getCmp("DirDomesticExpenseName" + ObjectID).setValue(UO_GridRecord.data.text);

                            Ext.getCmp("DirVatValue" + ObjectID).setValue(varDirVatValue);
                            Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(new Date(), "Y-m-d"));//Ext.getCmp("DocDate" + ObjectID).setValue(Ext.Date.format(Ext.getCmp("DocDate" + Ext.getCmp(UO_idCall).UO_id).getValue(), "Y-m-d"));
                            Ext.getCmp("DocDomesticExpenseID" + ObjectID).setValue(0);

                            Ext.getCmp("DirCurrencyID" + ObjectID).setValue(varDirCurrencyID);
                            Ext.getCmp("DirCurrencyRate" + ObjectID).setValue(varDirCurrencyRate);
                            Ext.getCmp("DirCurrencyMultiplicity" + ObjectID).setValue(varDirCurrencyMultiplicity);

                            Ext.getCmp("DirEmployeeIDSpisat" + ObjectID).setValue(varDirEmployeeID);
                            //Ext.getCmp("DirEmployeeName" + ObjectID).setValue(lanDirEmployeeName);

                            //Кнопки *** *** *** *** ***
                            Ext.getCmp("btnHelds" + ObjectID).show();

                            //Фокус на кнопку "Расчет"
                            Ext.getCmp("btnHelds" + ObjectID).focus();

                        }
                        else if (New_Edit == 2 || New_Edit == 3) {
                            form.loadRecord(UO_GridRecord);

                            Ext.getCmp("DirVatValue" + ObjectID).setValue(0);
                            //Наименование окна (сверху)
                            widgetX.setTitle(widgetX.title + " №" + Ext.getCmp("DocDomesticExpenseID" + ObjectID).getValue());

                            //Проведён или нет
                            if (funParseBool(Ext.getCmp("Held" + ObjectID).getValue())) {
                                Ext.Msg.alert(lanOrgName, txtMsg020);
                                Ext.getCmp("btnHeldCancel" + ObjectID).show();
                            }
                            else {
                                Ext.getCmp("btnHelds" + ObjectID).show();
                                //Ext.getCmp("btnHelds1" + ObjectID).show();
                                //Ext.getCmp("btnHelds2" + ObjectID).show();
                            }
                            //Кнопку "Печать" - делаем активной"
                            //Ext.getCmp("btnPrint" + ObjectID).show();

                        }


                        form.UO_Loaded = true;
                        varPriceChange_ReadOnly = false; //Разрешить редактировать цены


                    }); //storeDirEmployeesGrid

                }); //storeDirDomesticExpensesGrid

            }); //storeDirCurrenciesGrid

            break;
        }




            //Sms

        /*case "viewSms": {

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

            });

            //Добавление в центральный контейнер "viewContainerCentral" (\Content\sklad\js\other\Function\Function.js)
            //Если панель, но удаляется текущий виджет
            ObjectShow(widgetX);


            Ext.getCmp("DocServicePurchID" + ObjectID).setValue(Ext.getCmp("DocServicePurchID" + Ext.getCmp(UO_idCall).UO_id).getValue());

            break;
        }*/



            //ККМ

        //Start (выбор склада при входе в Сервис)
        case "viewKKMAdding": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid"; //&ForEmployee=1

            var locTitle = "Внесение денег в кассу";
            if (New_Edit == 2) locTitle = "Инкассация денег из кассы";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                title: locTitle,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                UO_Type: New_Edit,

                storeDirWarehousesGrid: storeDirWarehousesGrid,
            });

            //ObjectShow(widgetX);

            widgetX.border = true;
            widgetX.center();
            widgetX.show();


            Ext.getCmp("DocCashOfficeSumSum" + ObjectID).setValue(0.00);


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }



            //Start (выбор склада при входе в Сервис)
        case "viewDirWarehouseSelect": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid"; //&ForEmployee=1

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirWarehousesGrid: storeDirWarehousesGrid,
            });

            //ObjectShow(widgetX);

            widgetX.border = true;
            widgetX.center();
            widgetX.show();

            //Заполняем поле: Сотрудник, если он есть
            if (lanDirEmployeeName != "...") {
                Ext.getCmp("labelEmployeeCashiername").setText(lanDirEmployeeName);
                Ext.getCmp("labelEmployeeCashiername").show();
            }

            //Лоадер
            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
            loadingMask.show();

            //Событие на загрузку в Grid
            storeDirWarehousesGrid.load({ waitMsg: lanLoading });
            storeDirWarehousesGrid.on('load', function () {
                if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                loadingMask.hide();

                Ext.getCmp("KKMSNumDeviceStart").setValue(varKKMSNumDevice);
                if (!varKKMSActive) {
                    if (Ext.getCmp("KKMSNumDeviceStart")) {
                    }
                }

            });


            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }



        //Печать чека на ККМ
        case "viewKKMLoader": {

            //Переключаемся на уже открытую вкладку
            //var UO_Identy = pObjectName;
            //if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;
            
            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName, // + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,
                
            });

            //ObjectShow(widgetX);

            widgetX.border = true;
            widgetX.center();
            widgetX.show();
            
            //Убираем вкладку "Скидка"
            //Ext.getCmp("PanelDocumentDiscount_" + ObjectID).setVisible(false);

            break;
        }


            //API

            //API 1.0
        case "viewAPI10": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeDirWarehousesGrid: storeDirWarehousesGrid,
            });

            //ObjectShow(widgetX);

            widgetX.border = true;
            widgetX.center();
            widgetX.show();



            //Загрузка формы
            var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
            widgetXForm.load({
                method: "GET",
                timeout: varTimeOutDefault,
                waitMsg: lanLoading,
                url: HTTP_Api10 + "1",
                success: function (form, action) {

                    widgetXForm.UO_Loaded = true;
                    //Фокус на открывшийся Виджет
                    widgetX.focus();

                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                },
                failure: function (form, action) {
                    
                    if (action != undefined && action.result != undefined && action.result.data != undefined) {
                        Ext.MessageBox.show({ title: lanFailure, msg: action.result.data, icon: Ext.MessageBox.INFO, buttons: Ext.Msg.OK });
                        Ext.getCmp("ExportDirNomens" + ObjectID).setValue(true);
                        Ext.getCmp("ExportDirNomens" + ObjectID).setValue(false);
                    }
                    else {
                        widgetX.close();
                        funPanelSubmitFailure(form, action);
                    }

                    //Фокус на открывшийся Виджет
                    //widgetX.focus();

                    //Разблокировка вызвавшего окна
                    ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                }
            });



            break;
        }



            //Интернет-Магазины

            //Витрина
        case "viewVitrinaInTrade": {

            //Переключаемся на уже открытую вкладку
            var UO_Identy = pObjectName;
            if (fun_TabIdenty_ObjectEditConfig(UO_idCall, New_Edit, UO_Identy)) return;

            var storeNomenTree = Ext.create("store.storeDirNomensTree"); storeNomenTree.setData([], false); storeNomenTree.proxy.url = HTTP_DirNomens + "?type=Tree";
            var storeDirContractorsOrgGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsOrgGrid.setData([], false); storeDirContractorsOrgGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=1";
            var storeDirContractorsGrid = Ext.create("store.storeDirContractorsGrid"); storeDirContractorsGrid.setData([], false); storeDirContractorsGrid.proxy.url = HTTP_DirContractors + "?type=Grid&DirContractor2TypeID1=2&DirContractor2TypeID2=4";

            var storeDirOrdersStatesGrid_Orders_Doc = Ext.create("store.storeDirOrdersStatesGrid"); storeDirOrdersStatesGrid_Orders_Doc.setData([], false); storeDirOrdersStatesGrid_Orders_Doc.proxy.url = HTTP_DirOrdersStates + "?type=Grid&CustomerPurch=1";
            var storeDirOrdersStatesGrid_Orders_Nomen = Ext.create("store.storeDirOrdersStatesGrid"); storeDirOrdersStatesGrid_Orders_Nomen.setData([], false); storeDirOrdersStatesGrid_Orders_Nomen.proxy.url = HTTP_DirOrdersStates + "?type=Grid&CustomerPurch=2";

            var storeDirWarehousesGrid = Ext.create("store.storeDirWarehousesGrid"); storeDirWarehousesGrid.setData([], false); storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid";
            var storeDirCurrenciesGrid = Ext.create("store.storeDirCurrenciesGrid"); storeDirCurrenciesGrid.setData([], false); storeDirCurrenciesGrid.proxy.url = HTTP_DirCurrencies + "?type=Grid";

            // === Формируем и показываем окно ===
            var widgetX = Ext.create("widget." + pObjectName, {
                id: pObjectName + ObjectID,
                UO_Identy: UO_Identy, //Идентификатор, не дающий открыть 2-ды одно и то же окно
                UO_id: ObjectID,
                UO_idMain: pObjectName + ObjectID,
                UO_idCall: UO_idCall,
                UO_GridSave: UO_GridSave, UO_GridIndex: UO_GridIndex, UO_GridRecord: UO_GridRecord,
                modal: UO_Modal,
                UO_Center: UO_Center,

                storeNomenTree: storeNomenTree,
                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirPriceTypesGrid: varStoreDirPriceTypesGrid,

                storeDirContractorsOrgGrid: storeDirContractorsOrgGrid,
                storeDirContractorsGrid: storeDirContractorsGrid,

                storeDirOrdersStatesGrid_Orders_Doc: storeDirOrdersStatesGrid_Orders_Doc,
                storeDirOrdersStatesGrid_Orders_Nomen: storeDirOrdersStatesGrid_Orders_Nomen,

                storeDirWarehousesGrid: storeDirWarehousesGrid,
                storeDirCurrenciesGrid: storeDirCurrenciesGrid,

            });

            //ObjectShow(widgetX);

            widgetX.border = true;
            widgetX.center();
            widgetX.show();

            //Slider
            Ext.getCmp("image1Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image2Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image3Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image4Show" + ObjectID).setStyle("cursor", "pointer");

            //Recommended
            /*
            //4
            Ext.getCmp("image5Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image6Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image7Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image8Show" + ObjectID).setStyle("cursor", "pointer");
            //8
            Ext.getCmp("image9Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image10Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image11Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image12Show" + ObjectID).setStyle("cursor", "pointer");
            //12
            Ext.getCmp("image13Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image14Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image15Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image16Show" + ObjectID).setStyle("cursor", "pointer");
            //16
            Ext.getCmp("image17Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image18Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image19Show" + ObjectID).setStyle("cursor", "pointer");
            Ext.getCmp("image20Show" + ObjectID).setStyle("cursor", "pointer");
            */

            if (varDirContractorIDOrgEmpl > 0) { Ext.getCmp("DirContractorIDOrg" + ObjectID).setReadOnly(true); }


            storeNomenTree.on('load', function () {
                if (storeNomenTree.UO_Loaded) return; //Уже загружали - выйти!
                storeNomenTree.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                storeDirContractorsOrgGrid.load({ waitMsg: lanLoading });
                storeDirContractorsOrgGrid.on('load', function () {
                    if (storeDirContractorsOrgGrid.UO_Loaded) return; //Уже загружали - выйти!
                    storeDirContractorsOrgGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                    storeDirContractorsGrid.load({ waitMsg: lanLoading });
                    storeDirContractorsGrid.on('load', function () {
                        if (storeDirContractorsGrid.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirContractorsGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                        storeDirOrdersStatesGrid_Orders_Doc.load({ waitMsg: lanLoading });
                        storeDirOrdersStatesGrid_Orders_Doc.on('load', function () {
                            if (storeDirOrdersStatesGrid_Orders_Doc.UO_Loaded) return; //Уже загружали - выйти!
                            storeDirOrdersStatesGrid_Orders_Doc.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                            storeDirOrdersStatesGrid_Orders_Nomen.load({ waitMsg: lanLoading });
                            storeDirOrdersStatesGrid_Orders_Nomen.on('load', function () {
                                if (storeDirOrdersStatesGrid_Orders_Nomen.UO_Loaded) return; //Уже загружали - выйти!
                                storeDirOrdersStatesGrid_Orders_Nomen.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                storeDirWarehousesGrid.load({ waitMsg: lanLoading });
                                storeDirWarehousesGrid.on('load', function () {
                                    if (storeDirWarehousesGrid.UO_Loaded) return; //Уже загружали - выйти!
                                    storeDirWarehousesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                                    storeDirCurrenciesGrid.load({ waitMsg: lanLoading });
                                    storeDirCurrenciesGrid.on('load', function () {
                                        if (storeDirCurrenciesGrid.UO_Loaded) return; //Уже загружали - выйти!
                                        storeDirCurrenciesGrid.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"


                                        //Организация
                                        if (varDirContractorIDOrgEmpl == 0) { Ext.getCmp("Orders_DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrg); }
                                        else { Ext.getCmp("Orders_DirContractorIDOrg" + ObjectID).setValue(varDirContractorIDOrgEmpl); }


                                        //Загрузка формы
                                        var widgetXForm = Ext.getCmp("form_" + widgetX.UO_id);
                                        widgetXForm.load({
                                            method: "GET",
                                            timeout: varTimeOutDefault,
                                            waitMsg: lanLoading,
                                            url: HTTP_DirWebShopUO + "1",
                                            success: function (form, action) {
                                                
                                                //Заполняем наименование КомбТри
                                                //1. Слайдер
                                                if (action.result.data.x.Slider_DirNomen1) {
                                                    Ext.getCmp("Slider_DirNomen1ID" + ObjectID).setRawValue(action.result.data.x.Slider_DirNomen1.DirNomenID + "." + action.result.data.x.Slider_DirNomen1.DirNomenName);

                                                    //Image
                                                    Ext.getCmp("SysGen1ID" + ObjectID).setValue(action.result.data.x.SysGen1ID);
                                                    Ext.getCmp("SysGen1IDPatch" + ObjectID).setValue(action.result.data.x.SysGen1IDPatch);
                                                    if (action.result.data.SysGen1IDPatch != "") { Ext.getCmp("image1Show" + ObjectID).setSrc(action.result.data.SysGen1IDPatch); }
                                                    else { Ext.getCmp("image1Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                                }
                                                if (action.result.data.x.Slider_DirNomen2) {
                                                    Ext.getCmp("Slider_DirNomen2ID" + ObjectID).setRawValue(action.result.data.x.Slider_DirNomen2.DirNomenID + "." + action.result.data.x.Slider_DirNomen2.DirNomenName);

                                                    //Image
                                                    Ext.getCmp("SysGen2ID" + ObjectID).setValue(action.result.data.x.SysGen2ID);
                                                    Ext.getCmp("SysGen2IDPatch" + ObjectID).setValue(action.result.data.x.SysGen2IDPatch);
                                                    if (action.result.data.SysGen2IDPatch != "") { Ext.getCmp("image2Show" + ObjectID).setSrc(action.result.data.SysGen2IDPatch); }
                                                    else { Ext.getCmp("image2Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                                }
                                                if (action.result.data.x.Slider_DirNomen3) {
                                                    Ext.getCmp("Slider_DirNomen3ID" + ObjectID).setRawValue(action.result.data.x.Slider_DirNomen3.DirNomenID + "." + action.result.data.x.Slider_DirNomen3.DirNomenName);

                                                    //Image
                                                    Ext.getCmp("SysGen3ID" + ObjectID).setValue(action.result.data.x.SysGen3ID);
                                                    Ext.getCmp("SysGen3IDPatch" + ObjectID).setValue(action.result.data.x.SysGen3IDPatch);
                                                    if (action.result.data.SysGen3IDPatch != "") { Ext.getCmp("image3Show" + ObjectID).setSrc(action.result.data.SysGen3IDPatch); }
                                                    else { Ext.getCmp("image3Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                                }
                                                if (action.result.data.x.Slider_DirNomen4) {
                                                    Ext.getCmp("Slider_DirNomen4ID" + ObjectID).setRawValue(action.result.data.x.Slider_DirNomen4.DirNomenID + "." + action.result.data.x.Slider_DirNomen4.DirNomenName);

                                                    //Image
                                                    Ext.getCmp("SysGen4ID" + ObjectID).setValue(action.result.data.x.SysGen4ID);
                                                    Ext.getCmp("SysGen4IDPatch" + ObjectID).setValue(action.result.data.x.SysGen4IDPatch);
                                                    if (action.result.data.SysGen4IDPatch != "") { Ext.getCmp("image4Show" + ObjectID).setSrc(action.result.data.SysGen4IDPatch); }
                                                    else { Ext.getCmp("image4Show" + ObjectID).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                                                }
                                                //2. Рекомендованные
                                                //4
                                                if (action.result.data.x.Recommended_DirNomen1) { Ext.getCmp("Recommended_DirNomen1ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen1.DirNomenID + "." + action.result.data.x.Recommended_DirNomen1.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen2) { Ext.getCmp("Recommended_DirNomen2ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen2.DirNomenID + "." + action.result.data.x.Recommended_DirNomen2.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen3) { Ext.getCmp("Recommended_DirNomen3ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen3.DirNomenID + "." + action.result.data.x.Recommended_DirNomen3.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen4) { Ext.getCmp("Recommended_DirNomen4ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen4.DirNomenID + "." + action.result.data.x.Recommended_DirNomen4.DirNomenName); }
                                                //8
                                                if (action.result.data.x.Recommended_DirNomen5) { Ext.getCmp("Recommended_DirNomen5ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen5.DirNomenID + "." + action.result.data.x.Recommended_DirNomen5.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen6) { Ext.getCmp("Recommended_DirNomen6ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen6.DirNomenID + "." + action.result.data.x.Recommended_DirNomen6.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen7) { Ext.getCmp("Recommended_DirNomen7ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen7.DirNomenID + "." + action.result.data.x.Recommended_DirNomen7.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen8) { Ext.getCmp("Recommended_DirNomen8ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen8.DirNomenID + "." + action.result.data.x.Recommended_DirNomen8.DirNomenName); }
                                                //12
                                                if (action.result.data.x.Recommended_DirNomen9) { Ext.getCmp("Recommended_DirNomen9ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen9.DirNomenID + "." + action.result.data.x.Recommended_DirNomen9.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen10) { Ext.getCmp("Recommended_DirNomen10ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen10.DirNomenID + "." + action.result.data.x.Recommended_DirNomen10.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen11) { Ext.getCmp("Recommended_DirNomen11ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen11.DirNomenID + "." + action.result.data.x.Recommended_DirNomen11.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen12) { Ext.getCmp("Recommended_DirNomen12ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen12.DirNomenID + "." + action.result.data.x.Recommended_DirNomen12.DirNomenName); }
                                                //16
                                                if (action.result.data.x.Recommended_DirNomen13) { Ext.getCmp("Recommended_DirNomen13ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen13.DirNomenID + "." + action.result.data.x.Recommended_DirNomen13.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen14) { Ext.getCmp("Recommended_DirNomen14ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen14.DirNomenID + "." + action.result.data.x.Recommended_DirNomen14.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen15) { Ext.getCmp("Recommended_DirNomen15ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen15.DirNomenID + "." + action.result.data.x.Recommended_DirNomen15.DirNomenName); }
                                                if (action.result.data.x.Recommended_DirNomen16) { Ext.getCmp("Recommended_DirNomen16ID" + ObjectID).setRawValue(action.result.data.x.Recommended_DirNomen16.DirNomenID + "." + action.result.data.x.Recommended_DirNomen16.DirNomenName); }


                                                Ext.getCmp("tab_1_" + ObjectID).setActiveTab(1);
                                                Ext.getCmp("tab_1_" + ObjectID).setActiveTab(0);


                                                widgetXForm.UO_Loaded = true;
                                                //Фокус на открывшийся Виджет
                                                widgetX.focus();

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            },
                                            failure: function (form, action) {

                                                /*
                                                if (action != undefined && action.result != undefined && action.result.data != undefined) {
                                                    Ext.MessageBox.show({ title: lanFailure, msg: action.result.data, icon: Ext.MessageBox.INFO, buttons: Ext.Msg.OK });
                                                    //Ext.getCmp("ExportDirNomens" + ObjectID).setValue(true);
                                                    //Ext.getCmp("ExportDirNomens" + ObjectID).setValue(false);
                                                }
                                                else {
                                                    widgetX.close();
                                                    funPanelSubmitFailure(form, action);
                                                }

                                                //Фокус на открывшийся Виджет
                                                //widgetX.focus();
                                                */

                                                Ext.getCmp("DirNomenGroup_Top" + ObjectID).setValue(2);
                                                Ext.getCmp("Orders_Doc_DirOrdersStateID" + ObjectID).setValue(1);
                                                Ext.getCmp("Nomen_DirPriceTypeID" + ObjectID).setValue(3);
                                                Ext.getCmp("DomainName" + ObjectID).setValue(action.result.data);
                                                Ext.Msg.alert(lanOrgName, "Ещё нет записей!");

                                                //Разблокировка вызвавшего окна
                                                ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
                                            }

                                        });

                                    });
                                });
                            });
                        });
                    });
                });
            });


            break;
        }




        default: {
            Ext.Msg.alert("ObjectEditConfig", "Object '" + pObjectName + "' not found!");
            //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
            if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined && New_Edit > 1) { Ext.getCmp(UO_idCall).enable(); }
            break;
        }

    }



    /*} catch (ex) {
        var exMsg = ex;
        if (exMsg.message != undefined) exMsg = ex.message;
        Ext.Msg.alert(lanOrgName, "Ошибка в скрипте! Вышлите, пожалуйста скриншот на: support@uchetoblako.ru<br />Подробности:" + exMsg);
        //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined && New_Edit > 1) { Ext.getCmp(UO_idCall).enable(); }
        //Разблокировка вызвавшего окна
        //ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, false);
    }*/


}



// === Function ===

//Блокировать или Разблокировать вызвавший элемент "UO_idCall"
function ObjectEditConfig_UO_idCall_true_false(UO_idCall, New_Edit, Disable) {
    //Не блокировать вызванные из меню!
    if (UO_idCall == "viewContainerHeader") return;
    if (Disable) {
        //Блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined && New_Edit > 1) { Ext.getCmp(UO_idCall).disable(); }
    }
    else {
        //Раз-блокируем грид, который вызвал редактирования, что бы 2-ды не открыли на редактирование
        if (UO_idCall != undefined && Ext.getCmp(UO_idCall) != undefined && New_Edit > 1) { Ext.getCmp(UO_idCall).enable(); }
    }
}


