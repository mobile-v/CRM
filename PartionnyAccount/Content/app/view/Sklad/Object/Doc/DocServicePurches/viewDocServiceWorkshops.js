Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocServicePurches/viewDocServiceWorkshops", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocServiceWorkshops",

    layout: "border",
    region: "center",
    title: "Сервис - Мастерская",
    width: 750, height: 350,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    controller: 'viewcontrollerDocAllEdit',

    conf: {},

    initComponent: function () {


        //*** Шапка *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        // 0. Все
        var rowEditing_0 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid0 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid0_', itemId: "DirWarehouseIDPanelGrid0_", id: "DirWarehouseIDPanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],

            //id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid0_",
            store: this.storeGrid0,
            title: "Все", UO_title: "Все",
            UO_Num: 1,

            plugins: [rowEditing_0],
            rowEditing_0: rowEditing_0,

        });
        // 1. Принят в ремонт
        var rowEditing_1 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid1 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid1_', itemId: "DirWarehouseIDPanelGrid1_", id: "DirWarehouseIDPanelGrid1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],

            itemId: "PanelGrid1_",
            store: this.storeGrid1,
            title: "Принят в ремонт", UO_title: "Принят в ремонт",

            plugins: [rowEditing_1],
            rowEditing_1: rowEditing_1,
        });
        // 5. Ожидает запчасть
        var rowEditing_5 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid5 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid5_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid5_', itemId: "DirWarehouseIDPanelGrid5_", id: "DirWarehouseIDPanelGrid5_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],

            itemId: "PanelGrid5_",
            store: this.storeGrid5,
            title: "Ожидает запчасть", UO_title: "Ожидает запчасть",

            plugins: [rowEditing_5],
            rowEditing_5: rowEditing_5,
        });
        // 2. В ремонте
        var rowEditing_2 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid2 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid2_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid2_', itemId: "DirWarehouseIDPanelGrid2_", id: "DirWarehouseIDPanelGrid2_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],
            
            itemId: "PanelGrid2_",
            store: this.storeGrid2,
            title: "В ремонте", UO_title: "В ремонте",

            plugins: [rowEditing_2],
            rowEditing_2: rowEditing_2,
        });
        // 6. В удалённым СЦ
        var rowEditing_6 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid6 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid6_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid6_', itemId: "DirWarehouseIDPanelGrid6_", id: "DirWarehouseIDPanelGrid6_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],

            itemId: "PanelGrid6_",
            store: this.storeGrid6,
            title: "В удалённым СЦ", UO_title: "В удалённым СЦ",

            plugins: [rowEditing_6],
            rowEditing_6: rowEditing_6,
        });
        // X. Администратор
        var rowEditing_X = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGridX = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGridX_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGridX_', itemId: "DirWarehouseIDPanelGridX_", id: "DirWarehouseIDPanelGridX_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],

            itemId: "PanelGridX_",
            store: this.storeGridX,
            title: "Мои", UO_title: "Мои",

            plugins: [rowEditing_X],
            rowEditing_X: rowEditing_X,
        });
        // 3. Согласование
        var rowEditing_3 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid3 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid3_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid3_', itemId: "DirWarehouseIDPanelGrid3_", id: "DirWarehouseIDPanelGrid3_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],
            
            itemId: "PanelGrid3_",
            store: this.storeGrid3,
            title: "Согласование", UO_title: "Согласование",

            plugins: [rowEditing_3],
            rowEditing_3: rowEditing_3,
        });
        // 4. Согласовано
        var rowEditing_4 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid4 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid4_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid4_', itemId: "DirWarehouseIDPanelGrid4_", id: "DirWarehouseIDPanelGrid4_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],
            
            itemId: "PanelGrid4_",
            store: this.storeGrid4,
            title: "Согласовано", UO_title: "Согласовано",

            plugins: [rowEditing_4],
            rowEditing_4: rowEditing_4,
        });
        // 7. Выдача
        var rowEditing_7 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid7 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid7_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid7_', itemId: "DirWarehouseIDPanelGrid7_", id: "DirWarehouseIDPanelGrid7_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },
            ],

            itemId: "PanelGrid7_",
            store: this.storeGrid7,
            title: "Выдача", UO_title: "Выдача",

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],

            plugins: [rowEditing_7],
            rowEditing_7: rowEditing_7,
        });
        // 9. Выдан (Архив)
        var rowEditing_9 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid9 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid9_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [

                {
                    id: "TriggerSearchGrid" + this.UO_id,
                    UO_id: this.UO_id,
                    UO_idMain: this.UO_idMain,
                    UO_idCall: this.UO_idCall,

                    xtype: 'viewTriggerSearchGrid',
                    //fieldLabel: lanGroup,
                    emptyText: "Поиск ...",
                    name: 'TriggerSearchGrid',
                    id: "TriggerSearchGrid" + this.UO_id, itemId: "TriggerSearchGrid",
                    UO_DocX: "viewDocServiceWorkshops",
                    allowBlank: true,
                },

                { xtype: 'label', forId: "DateS" + this.UO_id, text: lanWith, margin: "0 0 0 15" },
                {
                    UO_id: this.UO_id,
                    UO_idMain: this.UO_idMain,
                    UO_idCall: this.UO_idCall,
                    margin: "0 0 0 5",
                    xtype: "viewDateFieldFix", name: "DateS", id: "DateS" + this.UO_id, itemId: "DateS", width: 100, editable: false,
                },

                { xtype: 'label', forId: "DatePo" + this.UO_id, text: lanTo, margin: "0 0 0 5" },
                {
                    UO_id: this.UO_id,
                    UO_idMain: this.UO_idMain,
                    UO_idCall: this.UO_idCall,
                    margin: "0 0 0 5",
                    xtype: "viewDateFieldFix", name: "DatePo", id: "DatePo" + this.UO_id, itemId: "DatePo", width: 100, editable: false,
                },

                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanWarehouse, width: 300, allowBlank: true, //, emptyText: "..."
                    margin: "0 0 0 5",
                    store: this.storeDirWarehousesGrid, // store getting items from server
                    valueField: 'DirWarehouseID',
                    hiddenName: 'DirWarehouseID',
                    displayField: 'DirWarehouseName',
                    name: 'DirWarehouseIDPanelGrid9_', itemId: "DirWarehouseIDPanelGrid9_", id: "DirWarehouseIDPanelGrid9_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: false, typeAhead: false, minChars: 200,
                },

            ],

            //id: "PanelGrid7_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid9_",
            store: this.storeGrid9,
            title: "Архив", UO_title: "Архив",

            plugins: [rowEditing_9],
            rowEditing_9: rowEditing_9,
        });


        //Tab-Panel
        var tabPanel = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            items: [
                PanelGrid0, PanelGrid1, PanelGrid5, PanelGrid2, PanelGrid6, PanelGridX, PanelGrid3, PanelGrid4, PanelGrid7, PanelGrid9
            ],

            listeners: {
                tabchange: function (tabPanel, newTab, oldTab, index) {
                    var widgetXForm = Ext.getCmp("form_" + tabPanel.UO_id).setVisible(false);
                    if (newTab.itemId != "PanelGrid9_") {
                        
                        var newTabstore = newTab.store;
                        var url = newTabstore.proxy.url; //Сохраняем урл, что бы вернуть потом
                        newTabstore.proxy.url = newTabstore.UO_Proxy_Url + "&DirWarehouseIDOnly=" + Ext.getCmp("DirWarehouseID" + newTab.itemId + tabPanel.UO_id).getValue();

                        newTabstore.load({ waitMsg: lanLoading });
                        newTabstore.on('load', function () {
                            //возвращаем предыдущий
                            newTabstore.proxy.url = url;

                            //В наименовании вкладки ставим скобки и в них к-во аппаратов
                            newTab.setTitle(newTab.UO_title  + " (" + newTabstore.data.length + ")");

                        });
                    }
                }
            },

        });




        //*** Форма "Мастерская" *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        //0. Шапка
        var PanelCap1 = Ext.create('Ext.panel.Panel', {
            id: "panel1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)

            region: "north", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: 80, 
            bodyPadding: 0,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,
            autoHeight: true,

            items: [

                //*** Не видно *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Организация", name: "DirContractorIDOrg", readOnly: true, flex: 1, id: "DirContractorIDOrg" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Склад", name: "DirWarehouseID", readOnly: true, flex: 1, id: "DirWarehouseID" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Phone", name: "Phone", readOnly: true, flex: 1, id: "Phone" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseAddress", name: "DirWarehouseAddress", readOnly: true, flex: 1, id: "DirWarehouseAddress" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: lanDate, name: "DocDate", id: "DocDate" + this.UO_id, width: 200, readOnly: true, allowBlank: false, editable: false, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirServiceStatusID", name: "DirServiceStatusID", id: "DirServiceStatusID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                //К-во предыдущих ремонтов
                { xtype: 'textfield', fieldLabel: "QuantityOk", name: "QuantityOk", id: "QuantityOk" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "QuantityFail", name: "QuantityFail", id: "QuantityFail" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "QuantityCount", name: "QuantityCount", id: "QuantityCount" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirServiceContractorID", name: "DirServiceContractorID", id: "DirServiceContractorID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                //Номер Чека ККМ
                { xtype: 'textfield', fieldLabel: "KKMSCheckNumber", name: 'KKMSCheckNumber', itemId: "KKMSCheckNumber", id: "KKMSCheckNumber" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "KKMSIdCommand", name: 'KKMSIdCommand', itemId: "KKMSIdCommand", id: "KKMSIdCommand" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true },

                //DirPaymentTypes
                {
                    xtype: 'viewComboBox',
                    labelAlign: 'top',
                    fieldLabel: "Тип оплаты", allowBlank: false, flex: 1,
                    //margin: "0 0 0 5",
                    store: this.storeDirPaymentTypesGrid, // store getting items from server
                    valueField: 'DirPaymentTypeID',
                    hiddenName: 'DirPaymentTypeID',
                    displayField: 'DirPaymentTypeName',
                    name: 'DirPaymentTypeID', itemId: "DirPaymentTypeID", id: "DirPaymentTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    hidden: true
                },



                //*** Видно *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                //hbox -  размещение элементов по вертикали (снизу добавлять)
                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        //Это рамочка
                        {
                            //title: "Аппарат",
                            autoHeight: true,
                            xtype: 'fieldset', flex: 1, layout: 'anchor',
                            items: [

                                //vbox - это 
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                            items: [
                                                //{ xtype: 'textfield', fieldLabel: "Неисправность со слов клиента", labelWidth: 200, name: "ProblemClientWords", id: "ProblemClientWords" + this.UO_id, readOnly: true, flex: 1, allowBlank: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>Неисправность</b>', labelWidth: 120, name: "ProblemClientWords", id: "ProblemClientWords" + this.UO_id, readOnly: true, allowBlank: false, flex: 1 },

                                                { xtype: 'displayfield', fieldLabel: '<b>' + lanPassword + '</b>', name: "ComponentPasText", id: "ComponentPasText" + this.UO_id, readOnly: true, allowBlank: false, width: 140, labelWidth: 50 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Сумма</b>', name: "SumTotal2", id: "SumTotal2" + this.UO_id, readOnly: true, allowBlank: false, width: 115, labelWidth: 50 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Предоплата</b>', name: "PrepaymentSum", id: "PrepaymentSum2" + this.UO_id, readOnly: true, allowBlank: false, width: 140, labelWidth: 85 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Предоплата</b>', name: "PrepaymentSum_1", id: "PrepaymentSum_1" + this.UO_id, readOnly: true, allowBlank: false, width: 140, labelWidth: 85, hidden: true },
                                            ]
                                        },

                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                            items: [
                                                { xtype: 'displayfield', fieldLabel: '<b>Аппарат</b>', name: "DirServiceNomenNameLittle", id: "DirServiceNomenNameLittle" + this.UO_id, readOnly: true, allowBlank: false, flex: 2, labelWidth: 55, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>Аппарат</b>', name: "DirServiceNomenName", id: "DirServiceNomenName" + this.UO_id, readOnly: true, allowBlank: false, flex: 2, labelWidth: 55 },

                                                { xtype: 'displayfield', fieldLabel: '<b>ID0</b>', name: "ID0", id: "ID0" + this.UO_id, readOnly: true, allowBlank: false, width: 0, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>ID1</b>', name: "ID1", id: "ID1" + this.UO_id, readOnly: true, allowBlank: false, width: 0, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>ID2</b>', name: "ID2", id: "ID2" + this.UO_id, readOnly: true, allowBlank: false, width: 0, hidden: true },

                                                { xtype: 'displayfield', fieldLabel: '<b>ФИО</b>', name: "DirServiceContractorName", id: "DirServiceContractorName" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 35 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Телефон</b>', name: "DirServiceContractorPhone", id: "DirServiceContractorPhone" + this.UO_id, readOnly: true, allowBlank: false, width: 145, labelWidth: 60 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Ор.стоимость</b>', name: "PriceVAT", id: "PriceVAT" + this.UO_id, readOnly: true, allowBlank: false, width: 150, labelWidth: 85 },

                                                { xtype: 'displayfield', fieldLabel: '<b>В БУ</b>', name: "InSecondHand", id: "InSecondHand" + this.UO_id, readOnly: true, allowBlank: false, width: 150, labelWidth: 85, hidden: true },
                                            ]
                                        },
                                    ]
                                },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            //title: "Комплектация",
                            autoHeight: true,
                            xtype: 'fieldset', width: 250, layout: 'anchor',
                            items: [
                                {
                                    xtype: 'container', flex: 2, layout: { align: 'stretch', type: 'vbox' },
                                    items: [

                                        //hbox - размещение элементов по горизонтали (слева добавлять)
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                            items: [
                                                { xtype: 'displayfield', fieldLabel: '<b>Квитанция</b>', name: "DocServicePurchID", id: "DocServicePurchID" + this.UO_id, readOnly: true, allowBlank: false, flex: 2 },
                                                { xtype: 'button', tooltip: "Мастер", iconCls: "button-image-user", itemId: "btnMasterEdit", id: "btnMasterEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                            ]
                                        },

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Гарантия", flex: 1, allowBlank: true,
                                            
                                            store: new Ext.data.SimpleStore({
                                                fields: ['ServiceTypeRepair', 'ServiceTypeRepairName'],
                                                data: ServiceTypeRepair_values
                                            }),
                                            valueField: 'ServiceTypeRepair',
                                            hiddenName: 'ServiceTypeRepair',
                                            displayField: 'ServiceTypeRepairName',
                                            name: 'ServiceTypeRepair', itemId: "ServiceTypeRepair", id: "ServiceTypeRepair" + this.UO_id,
                                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }
                                    ]
                                },

                            ]
                        },

                    ]
                },

            ],

        });


        //1.1. Грид: Выполненная работа
        var rowEditing1 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid21 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "grid1",
            id: "grid1_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,          //ObjectID
            UO_idMain: this.UO_idMain,  //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall,  //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,      //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            flex:1,
            hideHeaders: true,
            store: this.storeDocServicePurch1TabsGrid, //storeDocAccountTabsGrid,

            columns: [
                { text: "№", dataIndex: "DocServicePurch1TabID", width: 45, hidden: true, sortable: false, tdCls: 'x-change-cell-12' },
                //Услуга
                { text: "№", dataIndex: "DirServiceJobNomenID", width: 45, sortable: false, tdCls: 'x-change-cell-12' },
                { text: "Выполненная работа", dataIndex: "DirServiceJobNomenName", flex: 1, sortable: false, editor: { xtype: 'textfield' }, tdCls: 'x-change-cell-12' },
                { text: lanPrice, dataIndex: "PriceCurrency", width: 75, sortable: false, editor: { xtype: 'textfield' }, tdCls: 'x-change-cell-12' },

                { text: lanEmployee, dataIndex: "DirEmployeeName", width: 125, sortable: false, tdCls: 'x-change-cell-12' },
                {
                    text: "Комментарий", dataIndex: "DiagnosticRresults", width: 100, sortable: false,
                    renderer : function(value, metadata,record) {
                        return controllerDocServiceWorkshops_PanelGrid1_DiagnosticRresults(value, metadata, record);
                    },
                    tdCls: 'x-change-cell-12'
                },
                { text: lanDate, dataIndex: "TabDate", width: 80, sortable: false, tdCls: 'x-change-cell-12' },
                //{ text: "DirServiceStatusID", dataIndex: "DirServiceStatusID", width: 80, sortable: false, tdCls: 'x-change-cell-12' },
                { text: "Оплата", dataIndex: "PayDate", width: 80, sortable: false, tdCls: 'x-change-cell-12' },

                { text: "№", dataIndex: "RemontN", width: 20, sortable: false, tdCls: 'x-change-cell-12' },
            ],

            tbar: [
                //{ xtype: "label", text: "Выполненная работа " },
                {
                    xtype: 'displayfield', fieldLabel: 'Выполненная работа на сумму ', name: "SumDocServicePurch1Tabs2", id: "SumDocServicePurch1Tabs2" + this.UO_id,
                    readOnly: true, allowBlank: false, labelWidth: 180, //flex: 2,
                },
                /*{
                    xtype: "displayfield",
                    name: "SumDocServicePurch1Tabs2", id: "SumDocServicePurch1Tabs2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    text: " на сумму 0 "
                },*/

                "->",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/add.png', tooltip: lanAddPosition,
                    id: "btnGridAddPosition11" + this.UO_id, itemId: "btnGridAddPosition11",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "Прайс-Лист", tooltip: lanAddPosition,
                    itemId: "btnGridAddPosition12",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion1" + this.UO_id, itemId: "btnGridDeletion1"
                }
            ],

            plugins: [rowEditing1],
            rowEditing1: rowEditing1,


            viewConfig: {
                getRowClass: function (record, index) {
                    
                    var StatusID = parseFloat(record.get('DirServiceStatusID'));

                    if (StatusID == 3) { return 'status-3'; }
                    else if (StatusID == 4) { return 'status-4'; }

                }, //getRowClass

                stripeRows: true,
            },


            stripeRows: true,
            listeners: {
                itemcontextmenu: function (view, rec, node, index, e) {
                    e.stopEvent();

                    var contextMenu = Ext.create('Ext.menu.Menu', {
                        UO_id: this.UO_id,
                        UO_idMain: this,
                        UO_idCall: this,
                        width: 200,
                        items: [
                            {
                                text: 'Согласовано',
                                UO_id: this.UO_id,
                                UO_idMain: this,
                                UO_idCall: this,
                                handler: function (par1, par2) {
                                    var grid = Ext.getCmp("grid1_" + par1.UO_id);
                                    var record = grid ? grid.getSelection()[0] : null;
                                    if (record) {
                                        controllerDocServiceWorkshops_onGrid1Edit(grid.UO_id, record, 4);
                                    }
                                    else {
                                        Ext.Msg.alert(lanOrgName, "Выберите запись!");
                                    }
                                }
                            },
                            {
                                text: 'Не согласовано',
                                UO_id: this.UO_id,
                                UO_idMain: this,
                                UO_idCall: this,
                                handler: function (par1, par2) {
                                    var grid = Ext.getCmp("grid1_" + par1.UO_id);
                                    var record = grid ? grid.getSelection()[0] : null;
                                    if (record) {
                                        controllerDocServiceWorkshops_onGrid1Edit(grid.UO_id, record, 3);
                                    }
                                    else {
                                        Ext.Msg.alert(lanOrgName, "Выберите запись!");
                                    }
                                }
                            },

                        ]
                    });
                    contextMenu.UO_idMain = this;
                    contextMenu.UO_idCall = this;
                    contextMenu.showAt(e.getXY());

                    return false;
                }
            }

        });

        //1.2. Грид: Запчасти
        //var rowEditing2 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid22 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "grid2",
            id: "grid2_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            flex: 1,
            hideHeaders: true,
            store: this.storeDocServicePurch2TabsGrid, //storeDocAccountTabsGrid,

            columns: [
                { text: "№", dataIndex: "DocServicePurch2TabID", width: 45, hidden: true, sortable: false, tdCls: 'x-change-cell-12' },
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 45, hidden: true, sortable: false, tdCls: 'x-change-cell-12' },
                //Услуга
                { text: "№", dataIndex: "DirNomenID", width: 45, sortable: false, tdCls: 'x-change-cell-12' },
                { text: "Запчасть", dataIndex: "DirNomenName", flex: 1, sortable: false, tdCls: 'x-change-cell-12' }, //flex: 1
                { text: lanPrice, dataIndex: "PriceCurrency", width: 95, sortable: false, tdCls: 'x-change-cell-12' }, //, editor: { xtype: 'textfield' }

                { text: lanEmployee, dataIndex: "DirEmployeeName", width: 140, sortable: false, tdCls: 'x-change-cell-12' },
                { text: lanDate, dataIndex: "TabDate", width: 80, sortable: false, tdCls: 'x-change-cell-12' },
                { text: "Оплата", dataIndex: "PayDate", width: 80, sortable: false, tdCls: 'x-change-cell-12' },

                { text: "№", dataIndex: "RemontN", width: 20, sortable: false, tdCls: 'x-change-cell-12' },
            ],

            tbar: [
                //{ xtype: "label", text: "Запчасти " },
                {
                    xtype: 'displayfield', fieldLabel: 'Запчасти на сумму', name: "SumDocServicePurch2Tabs2", id: "SumDocServicePurch2Tabs2" + this.UO_id,
                    readOnly: true, allowBlank: false, labelWidth: 180, //flex: 2,
                },
                "->",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "Склад", tooltip: "Запчасть со склада",
                    id: "btnGridAddPosition2" + this.UO_id, itemId: "btnGridAddPosition2",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: "Возврат на склад", tooltip: "Вернуть выбранную позицию товара на склад", disabled: true,
                    id: "btnGridDeletion2" + this.UO_id, itemId: "btnGridDeletion2"
                }
            ],

            //plugins: [rowEditing2],
            //rowEditing2: rowEditing2,


            viewConfig: {
                getRowClass: function (record, index) {
  
                    var IsZakaz =record.get('IsZakaz');

                    if (IsZakaz == true) { return 'status-3'; }
                    //else if (IsZakaz == 4) { return 'status-4'; }

                }, //getRowClass

                stripeRows: true,
            },

        });

        //1. Содержит оба грида
        var PanelGridUnion = Ext.create('Ext.panel.Panel', {
            id: "PanelGridUnion_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            bodyStyle: 'background:transparent;',
            region: "center",
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
            },
            flex: 1,

            items: [
                PanelGrid21,
                PanelGrid22
            ],

        });





        //3. Справа: Статусы-Кнопки
        //Все
        var PanelGridLog0 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog0",
            conf: {
                id: "gridLog0_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            //region: "center",
            //flex: 1,
            //split: true,
            store: this.storeLogServicesGrid0, //storeDocAccountTabsGrid,
            title: "Все",

        });
        //Смена статуса
        var PanelGridLog1 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog1",
            conf: {
                id: "gridLog1_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            //region: "center",
            //flex: 1,
            //split: true,
            store: this.storeLogServicesGrid1, //storeDocAccountTabsGrid,
            title: "Статус",

            /*
            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }



                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogServiceDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                        if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) { record.data["Field1"] += " *** </b>"; }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig
            */

        });
        //Комментарии и заметки
        var PanelGridLog3 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog3",
            conf: {
                id: "gridLog3_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            region: "center",
            flex: 1,
            //split: true,
            store: this.storeLogServicesGrid3, //storeDocAccountTabsGrid,
            title: "Заметки",

            /*
            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }



                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogServiceDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                        if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) { record.data["Field1"] += " *** </b>"; }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig
            */

        });
        //Отправка СМС
        var PanelGridLog4 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog4",
            conf: {
                id: "gridLog4_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            //region: "center",
            //flex: 1,
            //split: true,
            store: this.storeLogServicesGrid4, //storeDocAccountTabsGrid,
            title: "SMS",

            /*
            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }



                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogServiceDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                        if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) { record.data["Field1"] += " *** </b>"; }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig
            */

        });
        //Выполненная работа
        var PanelGridLog5 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog5",
            conf: {
                id: "gridLog5_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            //region: "center",
            //flex: 1,
            //split: true,
            store: this.storeLogServicesGrid5, //storeDocAccountTabsGrid,
            title: "Работа",

            /*
            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }



                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogServiceDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                        if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) { record.data["Field1"] += " *** </b>"; }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig
            */

        });
        //Запчасть
        var PanelGridLog6 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog6",
            conf: {
                id: "gridLog6_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            //region: "center",
            //flex: 1,
            //split: true,
            store: this.storeLogServicesGrid6, //storeDocAccountTabsGrid,
            title: "Запчасть",

            /*
            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }



                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogServiceDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                        if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) { record.data["Field1"] += " *** </b>"; }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig
            */

        });
        //Смена гарантии
        var PanelGridLog8 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog8",
            conf: {
                id: "gridLog8_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            //region: "center",
            //flex: 1,
            //split: true,
            store: this.storeLogServicesGrid8, //storeDocAccountTabsGrid,
            title: "Гарантия",

            /*
            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }



                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogServiceDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                        if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) { record.data["Field1"] += " *** </b>"; }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig
            */

        });
        //Возврат по гарантии
        var PanelGridLog9 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog9",
            conf: {
                id: "gridLog9_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            //region: "center",
            //flex: 1,
            //split: true,
            store: this.storeLogServicesGrid9, //storeDocAccountTabsGrid,
            title: "Возврат",

            /*
            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }



                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogServiceDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                        if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) { record.data["Field1"] += " *** </b>"; }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig
            */

        });
        //Другое
        var PanelGridLog7 = Ext.create('widget.viewGridServiceLog', { //widget.viewGridDoc
            itemId: "gridLog7",
            conf: {
                id: "gridLog7_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            //region: "center",
            //flex: 1,
            //split: true,
            store: this.storeLogServicesGrid7, //storeDocAccountTabsGrid,
            title: "Другое",

            /*

            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }



                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogServiceDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                        if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) { record.data["Field1"] += " *** </b>"; }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig
            */
        });

        //Tab-Panel
        var tabLogPanel = Ext.create('Ext.tab.Panel', {
            id: "tabLog_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "east",
            bodyStyle: 'background:transparent;',
            flex: 1,  //width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            items: [
                PanelGridLog0, PanelGridLog1, PanelGridLog3, PanelGridLog4, PanelGridLog5, PanelGridLog6, PanelGridLog8, PanelGridLog9, PanelGridLog7
            ],

            listeners: {
                tabchange: function (tabPanel, newTab, oldTab, index) {
                    //var widgetXForm = Ext.getCmp("form_" + tabPanel.UO_id).setVisible(false);
                    newTab.store.load({ waitMsg: lanLoading });
                }
            },


            tbar: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    tooltip: "Добавить запись в Лог", icon: '../Scripts/sklad/images/add.png',
                    itemId: "btnPanelGridLogAdd",
                },
                {
                    id: "btnSMS" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    tooltip: "Отправить SMS", icon: '../Scripts/sklad/images/sms2.png',
                    itemId: "btnSMS",
                },
                {
                    id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    text: "Предыдущие ремонты", tooltip: "Предыдущие ремонты", icon: '../Scripts/sklad/images/tools.png',
                    itemId: "btnHistory",
                },

                "->",

                /*{
                    xtype: "datafield",
                    id: "labelAlerted" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    text: "Не оповещён"
                },*/
                { xtype: 'displayfield', name: "Alerted", id: "Alerted" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 85 },
            ],

        });





        //4. Футер
        var PanelFooter = Ext.create('Ext.panel.Panel', {
            id: "PanelFooter_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            //region: "south",
            bodyStyle: 'background:transparent;',
            
            region: "south",
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            //split: true,

            items: [
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        //БУ
                        {
                            xtype: "button", tooltip: "В Б/У", icon: '../Scripts/sklad/images/secondhand16.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnSecondHand" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSecondHand",
                            //enableToggle: true, pressed: false,

                        },


                        //Статусы
                        {
                            xtype: "button", tooltip: "В диагностику", icon: '../Scripts/sklad/images/Status/question24.png', scale: 'large',
                            id: "btnStatus2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus2",
                            enableToggle: true, pressed: false,
                            style: "width: 50px; height: 35px;", 
 
                        },


                        {
                            xtype: "button", tooltip: "На согласовании", icon: '../Scripts/sklad/images/Status/onagreeing24.png', scale: 'large',
                            id: "btnStatus3" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus3",
                            enableToggle: true, pressed: false,
                            style: "width: 50px; height: 35px;", style: { background: 'yellow' }
                        },
                        /*{
                            xtype: "button", tooltip: "Согласован", icon: '../Scripts/sklad/images/Status/agreeing24.png', scale: 'large',
                            id: "btnStatus4" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus4",
                            enableToggle: true, pressed: false,
                            style: "width: 50px; height: 35px;", style: { background: 'yellow' }
                        },*/
                        {
                            xtype: "button", tooltip: "Согласован", icon: '../Scripts/sklad/images/Status/ok.png', scale: 'large', //, text: "<b style='font-size: 125%'>OK<b>"
                            id: "btnStatus4" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus4",
                            enableToggle: true, pressed: false,
                            style: "width: 50px; height: 35px;", style: { background: 'yellow' }
                        },

                        {
                            xtype: "button", tooltip: "Ожидание запчастей", icon: '../Scripts/sklad/images/Status/waiting24.png', scale: 'large',
                            id: "btnStatus5" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus5",
                            enableToggle: true, pressed: false,
                            style: "width: 50px; height: 35px;", style: { background: '#8A2BE2' }
                        },
                        {
                            xtype: "button", tooltip: "Отремонтирован", icon: '../Scripts/sklad/images/Status/renovated24.png', scale: 'large',
                            id: "btnStatus7" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus7",
                            enableToggle: true, pressed: false,
                            style: "width: 50px; height: 35px;", style: { background: '#90EE90' }
                        },
                        /*{
                            xtype: "button", tooltip: "В основном сервисе", icon: '../Scripts/sklad/images/Status/remote_sc24.png', scale: 'large',
                            hidden: true,
                            id: "btnStatus6" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus6",
                            enableToggle: true, pressed: false,
                            style: "width: 50px; height: 35px;", style: { background: '#808080' },
                        },*/
                        {
                            xtype: "button", tooltip: "Отказной", icon: '../Scripts/sklad/images/Status/renouncement24.png', scale: 'large',
                            id: "btnStatus8" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus8",
                            enableToggle: true, pressed: false,
                            style: "width: 50px; height: 35px;", style: { background: 'red' }
                        },


                        //Только для вкладки "Выдать"
                        { xtype: 'textfield', fieldLabel: "Работа", labelAlign: 'top', name: "SumDocServicePurch1Tabs", id: "SumDocServicePurch1Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, margin: "0 0 0 5", hidden: true },
                        { xtype: 'textfield', fieldLabel: "Запчасти", labelAlign: 'top', name: "SumDocServicePurch2Tabs", id: "SumDocServicePurch2Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "Итого", labelAlign: 'top', name: "SumTotal", id: "SumTotal" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "Предоплата", margin: "0 0 0 15", labelAlign: 'top', name: "PrepaymentSum", id: "PrepaymentSum" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "<b style='color: green;'>К оплате</b>", labelAlign: 'top', name: "SumTotal2a", id: "SumTotal2a" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, fieldStyle: 'color: green; font-weight: bold;' },

                        { xtype: 'displayfield', fieldLabel: 'Скидка(Р)', width: 75, margin: "0 0 0 15", labelAlign: 'top', name: "DiscountX", id: "DiscountX" + this.UO_id, readOnly: true, allowBlank: false, hidden: true },
                        { xtype: 'displayfield', fieldLabel: 'Скидка(З)', width: 75, margin: "0 0 0 15", labelAlign: 'top', name: "DiscountY", id: "DiscountY" + this.UO_id, readOnly: true, allowBlank: false, hidden: true },

                        {
                            xtype: 'button', margin: "0 0 0 25",
                            id: "btnSave" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave", style: "width: 175px; height: 40px;",
                            text: "<b style='font-size: 22px; color: green;'>В ы д а т ь</b>", icon: '../Scripts/sklad/images/save_held.png',
                            hidden: true
                        },



                        {
                            xtype: 'fieldset',
                            id: "fsListObjectPFID" + this.UO_id,
                            title: "Восстановить документы",
                            layout: 'column',
                            items: [

                                {
                                    xtype: 'container',
                                    layout: { align: 'stretch', type: 'hbox' },
                                    items: [

                                        {
                                            xtype: 'radiogroup',
                                            id: "rgListObjectPFID" + this.UO_id,
                                            columns: 1,
                                            vertical: true,
                                            items: [
                                                {
                                                    checked: true,
                                                    boxLabel: 'Квитанция',
                                                    name: 'ListObjectPFID',
                                                    inputValue: 31
                                                },
                                                {
                                                    boxLabel: 'Акт',
                                                    name: 'ListObjectPFID',
                                                    inputValue: 33
                                                },
                                                {
                                                    boxLabel: 'Гарантия',
                                                    name: 'ListObjectPFID',
                                                    inputValue: 35
                                                },
                                            ]
                                        },


                                        {
                                            xtype: "button", text: "Печать", tooltip: "Печать", icon: '../Scripts/sklad/images/print.png', style: "width: 125px; height: 50px;", scale: 'large',
                                            id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint"
                                        }

                                    ]
                                },

                            ]
                        },


                    ]
                },

            ]
        });


        //Form-Panel
        var formPanel1 = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,
            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)

            //bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "south", //!!! Важно для Ресайз-а !!!
            monitorValid: true,
            defaultType: 'textfield',
            layout: 'border',
            split: true,
            hidden: true,
            width: "100%", height: 400,
            bodyPadding: 5,
            autoHeight: true,

            items: [
                PanelCap1,
                PanelGridUnion,
                tabLogPanel,

                PanelFooter
            ]

        });





        //body
        this.items = [

            //tabPanel,

            {
                region: 'center',
                xtype: 'panel',
                layout: 'border', // тип лэйоута - трехколонник с подвалом и шапкой
                items: [
                    tabPanel,
                    formPanel1
                ]
            }

        ],


        this.callParent(arguments);
    }

});

