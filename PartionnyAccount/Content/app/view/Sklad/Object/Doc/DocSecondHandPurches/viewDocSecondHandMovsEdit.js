Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandMovsEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocSecondHandMovsEdit",

    layout: "border",
    region: "center",
    title: "Перемещение",
    width: 900, height: 550,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocSecondHandMovsEdit',
    //listeners: { close: 'onViewDocSecondHandRetailsEditClose' },

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {

        //Создать 3-и панели:
        // Вверху      - Партии
        // По середине - Основное
        // Внизу       - Спецификация
        //И есть Сплитер


        //Панель
        var LoadFrom_values = [
            [0, "Все (кроме под-модуля 'Разбор')"],
            [1, "Куплен"],
            //[2, "Предпродажа (мастерская)"],
            //[3, "На согласовании"],
            //[4, "Согласовано"],
            [5, "Ожидает запчастей"],
            [7, "Готов для продажи"],
            [8, "На разбор"],
            [12, "В разборе"],
            [13, "В разборе - Готов"],
        ];
        var PanelParty = Ext.create('Ext.grid.Panel', {
            id: "gridParty_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, UO_View: "viewDocSecondHandInvsEdit",
            itemId: "gridParty",

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            //autoScroll: true,
            flex: 1,
            split: true,
            store: this.storeDocSecondHandPurchesGrid, //storeDocRetailTabsGrid,

            columns: [
                { text: "№ desk", dataIndex: "DocID", width: 50, hidden: true },
                { text: "№", dataIndex: "DocSecondHandPurchID", width: 50 },

                { text: lanState, dataIndex: "DirSecondHandStatusName", flex: 1, hidden: true },
                { text: "", dataIndex: "Status", width: 55, tdCls: 'x-change-cell2' },
                { text: "Аппарат", dataIndex: "DirServiceNomenName", flex: 2, tdCls: 'x-change-cell' },

                { text: "Серийный", dataIndex: "SerialNumber", width: 120 },
                { text: "Клиент", dataIndex: "DirServiceContractorName", flex: 1 },
                { text: lanDocDate, dataIndex: "DocDate", width: 75 },
                { text: lanWarehouse, dataIndex: "DirWarehouseName", width: 120 },

                { text: "Неисправность", dataIndex: "ProblemClientWords", flex: 1, hidden: true },
                { text: lanPhone, dataIndex: "DirServiceContractorPhone", flex: 1, hidden: true },
                { text: "Срочный", dataIndex: "UrgentRepairs", flex: 1, hidden: true },

                //{ text: "Готовность", dataIndex: "DateDone", width: 75, hidden: true },
                //{ text: "Готовность", dataIndex: "DateDone", width: 75, sortable: false, editor: { xtype: 'textfield' } },
                //{ header: "Готовность", align: 'center', width: 120, sortable: true, dataIndex: 'DateDone', renderer: Ext.util.Format.dateRenderer('Y-m-d'), editor: { xtype: 'datefield' } }, // // put this in your column model} 

                {
                    xtype: 'datecolumn',
                    dataIndex: 'DateDone',
                    header: 'Готовность',
                    sortable: true,
                    //id: 'depreciationStartPeriod',
                    width: 75,
                    format: 'Y-m-d', //format: 'Y-m-d H:i:s', // <------- this way
                    editor: {
                        xtype: 'datefield',
                        format: 'Y-m-d', //format: 'Y-m-d H:i:s',
                        submitFormat: 'c'  // <-------------- this way
                    }
                },


                //{ text: "Предоплата", dataIndex: "Prepayment", width: 50, hidden: true },
                //{ text: "Дата выдачи", dataIndex: "IssuanceDate", width: 100 },
                { text: "Сумма", dataIndex: "Sums", width: 100, summaryType: 'sum' },


                //{ text: lanOrg, dataIndex: "DirContractorNameOrg", flex: 1, hidden: true },
                //{ text: lanContractor, dataIndex: "DirContractorName", flex: 1 },


                //{ text: "Из СЦ", dataIndex: "FromService", width: 100, hidden: true },
                //{ text: "Из СЦ", dataIndex: "FromServiceString", width: 100, hidden: true },
                { text: "№ СЦ", dataIndex: "DocServicePurchID", width: 50 },

                { text: "Продажа", dataIndex: "DateRetail", width: 75, hidden: true },
                { text: "Возврат", dataIndex: "DateReturn", width: 75, hidden: true },
            ],


            tbar: [

                //LoadXFrom
            /*
                {
                    xtype: 'viewComboBox',
                    allowBlank: false, width: 300,
                    margin: "0 0 0 5",
                    store: new Ext.data.SimpleStore({
                        fields: ['LoadXFrom', 'LoadXFromName'],
                        data: LoadXFrom_values
                    }),
                    valueField: 'LoadXFrom',
                    hiddenName: 'LoadXFrom',
                    displayField: 'LoadXFromName',
                    name: 'LoadXFrom', itemId: "LoadXFrom", id: "LoadXFrom" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    listeners: {
                        //select: 'onLoadXFrom_select',
                        change: 'onLoadXFrom_changet',
                    }
                },
                { xtype: 'textfield', fieldLabel: "LoadFrom", name: 'LoadFrom', id: 'LoadFrom' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                */
                {
                    xtype: 'viewComboBox',
                    allowBlank: false, width: 300,
                    margin: "0 0 0 5",
                    store: new Ext.data.SimpleStore({
                        fields: ['LoadFrom', 'LoadFromName'],
                        data: LoadFrom_values
                    }),
                    valueField: 'LoadFrom',
                    hiddenName: 'LoadFrom',
                    displayField: 'LoadFromName',
                    name: 'LoadFrom', itemId: "LoadFrom", id: "LoadFrom" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    listeners: {
                        //select: 'onLoadFrom_select',
                        //change: 'onLoadFrom_changet',
                    }
                },

                //Загрузить все остатки
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/refresh16.png', tooltip: "Обновить", style: "width: 40px; height: 40px;",
                    id: "btnGridRefresh" + this.UO_id, itemId: "btnGridRefresh",
                    listeners: { click: 'onGrid_BtnGridRefresh' }
                },

            ],


            viewConfig: {
                getRowClass: function (record, index) {

                    // 1. === Исправляем формат даты: "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"  ===  ===  ===  ===  === 
                    for (var i = 0; i < record.store.model.fields.length; i++) {
                        //Если поле типа "Дата"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }
                    }



                    // 2.  === Стили ===  ===  ===  ===  === 
                    var StatusID = parseFloat(record.get('DirSecondHandStatusID'));

                    //2.1. Если аппарат просрочен: DateDone <= new Date()
                    if (new Date(record.get('DateDone')) <= new Date()) {
                        //return 'status-DateDone';
                        if (StatusID == 1) { return 'status-DateDone-1'; }
                        else if (StatusID == 2) { return 'status-DateDone-2'; }
                        else if (StatusID == 3) { return 'status-DateDone-3'; }
                        else if (StatusID == 4) { return 'status-DateDone-4'; } //{ return 'status-DateDone-4'; }
                        else if (StatusID == 5) { return 'status-DateDone-5'; }
                        else if (StatusID == 6) { return 'status-DateDone-6'; }
                        else if (StatusID == 7) { return 'status-7'; }
                        else if (StatusID == 8) { return 'status-8'; }
                        else if (StatusID == 9) { return 'status-9'; }
                        else if (StatusID == 10) { return 'status-10'; }
                        else if (StatusID == 11) { return 'status-11'; }
                        else if (StatusID == 12) { return 'status-12'; }
                        else if (StatusID == 13) { return 'status-13'; }
                        else if (StatusID == 14) { return 'status-14'; }
                        else if (StatusID == 15) { return 'status-15'; }
                    }

                    //2.2. Не просрочен
                    if (StatusID == 1) { return 'status-1'; }
                    else if (StatusID == 2) {
                        if (record.get('FromGuarantee')) { return 'status-FromGuarantee'; }
                        else { return 'status-2'; }
                    }
                    else if (StatusID == 3) { return 'status-3'; }
                    else if (StatusID == 4) { return 'status-4'; }
                    else if (StatusID == 5) { return 'status-5'; }
                    else if (StatusID == 6) { return 'status-6'; }
                    else if (StatusID == 7) { return 'status-7'; }
                    else if (StatusID == 8) { return 'status-8'; }
                    else if (StatusID == 9) { return 'status-9'; }
                    else if (StatusID == 10) { return 'status-10'; }
                    else if (StatusID == 11) { return 'status-11'; }
                    else if (StatusID == 12) { return 'status-12'; }
                    else if (StatusID == 13) { return 'status-13'; }
                    else if (StatusID == 14) { return 'status-14'; }
                    else if (StatusID == 15) { return 'status-15'; }

                }, //getRowClass

                stripeRows: true,
            }

        });
        



        //Tab
        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

        var PanelDocumentDetails = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentDetails_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            
            defaults: { anchor: '100%' },
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%", height: 95 + varBodyPadding,
            //autoScroll: true,
            //split: true,

            items: [

                // !!! Не видимые !!!
                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },  //, hidden: true
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirMovementStatusID", name: "DirMovementStatusID", id: "DirMovementStatusID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", name: "DocSecondHandMovID", id: "DocSecondHandMovID" + this.UO_id, readOnly: true, width: 150, allowBlank: true, labelAlign: "top" },
                        { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", width: 150, allowBlank: true, hidden: true, labelAlign: "top" },
                        { xtype: 'viewDateField', fieldLabel: lanDateCounterparty, name: "DocDate", id: "DocDate" + this.UO_id, width: 150, margin: "0 0 0 5", allowBlank: false, editable: false, labelAlign: "top" },


                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouseFrom, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirWarehousesGridFrom, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseIDFrom', itemId: "DirWarehouseIDFrom", id: "DirWarehouseIDFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                            labelAlign: "top"
                        },
                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirWarehouseEditFrom", id: "btnDirWarehouseEditFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirWarehouseReloadFrom", id: "btnDirWarehouseReloadFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouseTo, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirWarehousesGridTo, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseIDTo', itemId: "DirWarehouseIDTo", id: "DirWarehouseIDTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                            labelAlign: "top"
                        },
                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirWarehouseEditTo", id: "btnDirWarehouseEditTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirWarehouseReloadTo", id: "btnDirWarehouseReloadTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                        //"->",
                        {
                            id: "TriggerSearchGrid" + this.UO_id,
                            UO_id: this.UO_id,
                            UO_idMain: this.UO_idMain,
                            UO_idCall: this.UO_idCall,

                            xtype: 'viewTriggerSearch',
                            fieldLabel: "Поиск по документу", labelAlign: "top", margin: "0 0 0 5",
                            emptyText: "Поиск ...", allowBlank: true, flex: 1,
                            name: 'TriggerSearchGrid', id: "TriggerSearchGrid" + this.UO_id, itemId: "TriggerSearchGrid",
                            listeners: { ontriggerclick: "onTriggerSearchGridClick1", specialkey: "onTriggerSearchGridClick2", change: "onTriggerSearchGridClick3" }
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, hidden: true,
                    items: [
                        
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", flex: 1, allowBlank: false, //, emptyText: "..."
                            store: this.storeDirContractorsOrgGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },

                        { xtype: "checkbox", boxLabel: lanReserve, margin: "0 0 0 5", name: "Reserve", itemId: "Reserve", flex: 1, id: "Reserve" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true },
                    ]
                },


                { xtype: 'container', height: 5 },


                //Причина и Основание
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [

                        //Причина
                        {
                            xtype: 'viewComboBox',
                            flex: 1, allowBlank: true, fieldLabel: "Причина", labelWidth: 50,
                            
                            store: this.storeDirMovementDescriptionsGrid, // store getting items from server
                            valueField: 'DirMovementDescriptionName',
                            hiddenName: 'DirMovementDescriptionID',
                            displayField: 'DirMovementDescriptionName',
                            name: 'DescriptionMovement', itemId: "DescriptionMovement", id: "DescriptionMovement" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: true, typeAhead: true, minChars: 2
                        },

                        //Основание
                        { xtype: 'textfield', fieldLabel: lanBase, labelWidth: 60, name: "Base", id: "Base" + this.UO_id, flex: 1, allowBlank: true, margin: "0 0 0 5", },

                        //Курьер
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Курьер", labelWidth: 50, flex: 1, allowBlank: true, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirEmployeesGrid, // store getting items from server
                            valueField: 'DirEmployeeID',
                            hiddenName: 'DirEmployeeID',
                            displayField: 'DirEmployeeName',
                            name: 'DirEmployeeIDCourier', itemId: "DirEmployeeIDCourier", id: "DirEmployeeIDCourier" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: false, typeAhead: false, minChars: 200,
                            //labelAlign: "top"
                        },
                        {
                            xtype: 'button', text: "X", tooltip: "Очистить", itemId: "btnDirEmployeeIDCourier", id: "btnDirEmployeeIDCourier" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            listeners: { click: "onBtnDirEmployeeIDCourierClick" }
                        },
                    ]
                },

            ]
        });

        var PanelDocumentAdditionally = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentAdditionally_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanAdditionally,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%", height: 95 + varBodyPadding,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [
                //{ xtype: 'textfield', fieldLabel: lanBase, name: "Base", id: "Base" + this.UO_id, flex: 1, allowBlank: true },
                //{ xtype: 'textfield', fieldLabel: lanDisc, name: "Description", id: "Description" + this.UO_id, flex: 1, allowBlank: true }
            ]
        });
        

        //Tab-Panel
        var tabPanelDetails = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            items: [
                //PanelDocumentDetails, PanelDocumentAdditionally
            ]

        });

        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 
        


        //2. Грид
        var PanelGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            itemId: "grid",

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeGrid, //storeDocSecondHandMovTabsGrid,

            columns: [

                //Партия
                { text: "Партия", dataIndex: "Rem2PartyID", width: 50, hidden: true },

                //Документ БУ
                { text: "Документ", dataIndex: "DocSecondHandPurchID", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },

                //Товар
                { text: "Товар №", dataIndex: "DirServiceNomenID", width: 50, style: "height: 25px;", hidden: true },
                { text: lanNomenclature, dataIndex: "DirServiceNomenName", flex: 1 },
                //К-во
                //{ text: lanCount, dataIndex: "Quantity", width: 75 },
                { text: "Розница", dataIndex: "PriceRetailCurrency", width: 75 },
                { text: "Закупка", dataIndex: "PriceCurrency", width: 75 },
                //Цены
                //{ text: "Розница Наценка", dataIndex: "MarkupRetail", width: 100, hidden: true },
                //{ text: "Розница Цена", dataIndex: "PriceRetailCurrency", width: 100, hidden: true },
                //{ text: "Опт Наценка", dataIndex: "MarkupWholesale", width: 100, hidden: true },
                //{ text: "Опт Цена", dataIndex: "PriceWholesaleCurrency", width: 100, hidden: true },
                //{ text: "IM Наценка", dataIndex: "MarkupIM", width: 100, hidden: true },
                //{ text: "IM Цена", dataIndex: "PriceIMCurrency", width: 100, hidden: true },
                //Суммы
                //{ text: lanPriceVatFull, dataIndex: "PriceCurrency", width: 100 },
                //{ text: lanSum, dataIndex: "SUMPurchPriceVATCurrency", width: 100 },

                { text: "Тип возвр.", dataIndex: "DirReturnTypeName", width: 85, style: "height: 25px;", tdCls: 'x-change-cell', hidden: true },
                { text: "Причина", dataIndex: "DirDescriptionName", width: 85, style: "height: 25px;", tdCls: 'x-change-cell', hidden: true },
            ],

            tbar: [
                
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanAddPosition, tooltip: lanAddPosition,
                    itemId: "btnGridAddPosition",
                    listeners: { click: "onGrid_BtnGridAddPosition" }
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: lanEdit, tooltip: lanEdit, disabled: true,
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit",
                    listeners: { click: "onGrid_BtnGridEdit" }
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete",
                    listeners: { click: "onGrid_BtnGridDelete" }
                },

            ],


            listeners: {
                selectionchange: 'onGrid_selectionchange',
                itemclick: 'onGrid_itemclick',
                itemdblclick: 'onGrid_itemdblclick',
            },
        });


        //3. Футер
        var PanelFooter = Ext.create('Ext.panel.Panel', {
            id: "PanelFooter_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            //region: "south",
            bodyStyle: 'background:transparent;',

            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            //split: true,

            items: [
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //Суммы (, labelAlign: 'top')
                        { xtype: 'textfield', fieldLabel: "Сумма закупки", name: "SumPurch", id: "SumPurch" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: "Сумма розницы", name: "SumRetail", id: "SumRetail" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
                    ]
                },

            ]
        });



        //Form-Panel
        var formPanel = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)


            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "center", //!!! Важно для Ресайз-а !!!
            monitorValid: true,
            defaultType: 'textfield',

            //layout: 'border',
            //defaults: { anchor: '100%' },
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
                split: true,
            },
            split: true,

            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [
                //PanelSearch,
                PanelDocumentDetails, 
                PanelParty,
                PanelGrid, PanelFooter
            ]
        });




        //body
        this.items = [
            /*
            //Товар
            Ext.create('widget.viewTreeDirRetail', {

                conf: {
                    id: "tree_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

                store: this.storeDirServiceNomenTree,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirServiceNomen"
                },


                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    //{ text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: 'Наименование', flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'Остаток', dataIndex: 'Remains', width: 50, hidden: true, tdCls: 'x-change-cell' },
                    //{ text: 'DirServiceNomenPatchFull', dataIndex: 'DirServiceNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],

            }),
            */

            // *** *** *** *** *** *** *** *** ***


            //Шапка документа + табличная часть
            formPanel

        ],


        this.buttons = [
            {
                xtype: 'textfield',
                allowBlank: false, width: 200, fieldLabel: "Ваш пароль", //inputType: 'password',
                name: 'DirEmployeePswd', itemId: 'DirEmployeePswd', id: 'DirEmployeePswd' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                listeners: { click: "onGrid_BtnGridAddPosition" }
            },

            " ",

            {
                id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", hidden: true, style: "width: 120px; height: 40px;",
                UO_Action: "held_cancel",
                text: lanHeldCancel, icon: '../Scripts/sklad/images/save_held.png',
                listeners: { click: "onBtnHeldCancelClick" }
            },
            {
                id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", hidden: true, style: "width: 120px; height: 40px;",
                UO_Action: "held",
                text: lanHelds, icon: '../Scripts/sklad/images/save_held.png',
                listeners: { click: "onBtnHeldsClick" }
            },
            {
                id: "btnRecord" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true, style: "width: 120px; height: 40px;",
                text: lanSave, icon: '../Scripts/sklad/images/save.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                        UO_Action: "save",
                        text: lanRecord, icon: '../Scripts/sklad/images/save.png',
                        listeners: { click: "onBtnSaveClick" }
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSaveClose",
                        UO_Action: "save_close",
                        text: lanRecordClose, icon: '../Scripts/sklad/images/save.png',
                        listeners: { click: "onBtnSaveClick" }
                    }
                ]
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", style: "width: 120px; height: 40px;",
                text: lanCancel, icon: '../Scripts/sklad/images/cancel.png',
                listeners: { click: "onBtnCancelClick" }
            },
            " ",
            {
                id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint", hidden: true, style: "width: 120px; height: 40px;",
                text: lanPrint, icon: '../Scripts/sklad/images/print.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintHtml",
                        UO_Action: "html",
                        text: "Html", icon: '../Scripts/sklad/images/html.png',
                        listeners: { click: "onBtnPrintHtmlClick" }
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintExcel",
                        UO_Action: "excel",
                        text: "MS Excel", icon: '../Scripts/sklad/images/excel.png',
                        listeners: { click: "onBtnPrintHtmlClick" }
                    }
                ]
            },
            "-",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp", style: "width: 120px; height: 40px;",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png',
                listeners: { click: "onBtnHelpClick" }
            },
            "->",
            { xtype: "label", text: "Внимание: Если выбрать курьера, то документ попадёт в Логистику!", style: 'color: red; font-weight: bold;' }

        ],


        this.callParent(arguments);
    }

});
