Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandWorkshops", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocSecondHandWorkshops",

    layout: "border",
    region: "center",
    title: "Б/У - Мастерская",
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
        var PanelGrid0 = Ext.create('widget.viewGridSecondHand', {
            conf: {
                id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid0_",
            store: this.storeGrid0,
            title: "Все", UO_title: "Все",

            plugins: [rowEditing_0],
            rowEditing_0: rowEditing_0,

        });
        // 1. Куплен (Принят в ремонт)
        var rowEditing_1 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid1 = Ext.create('widget.viewGridSecondHand', {
            conf: {
                id: "PanelGrid1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            //region: "center",
            itemId: "PanelGrid1_",
            store: this.storeGrid1,
            title: "Куплен", UO_title: "Куплен",

            plugins: [rowEditing_1],
            rowEditing_1: rowEditing_1,
        });
        // 2. Предпродажа (мастерская)
        var rowEditing_2 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid2 = Ext.create('widget.viewGridSecondHand', {
            conf: {
                id: "PanelGrid2_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid2_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid2_",
            store: this.storeGrid2,
            title: "Предпродажа", UO_title: "Предпродажа",

            plugins: [rowEditing_2],
            rowEditing_2: rowEditing_2,
        });
        // 5. Ожидает запчастей
        var rowEditing_5 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid5 = Ext.create('widget.viewGridSecondHand', {
            conf: {
                id: "PanelGrid5_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid5_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid5_",
            store: this.storeGrid5,
            title: "Ожидает запчасть", UO_title: "Ожидает запчасть",

            plugins: [rowEditing_5],
            rowEditing_5: rowEditing_5,
        });
        
        // 7. Готов для продажи
        var rowEditing_7 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid7 = Ext.create('widget.viewGridSecondHand', {
            conf: {
                id: "PanelGrid7_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid7_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid7_",
            store: this.storeGrid7,
            title: "Готов для продажи", UO_title: "Готов для продажи",

            plugins: [rowEditing_7],
            rowEditing_7: rowEditing_7,
        });
        // 8. На разбор
        var rowEditing_8 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid8 = Ext.create('widget.viewGridSecondHand', {
            conf: {
                id: "PanelGrid8_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid8_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid8_",
            store: this.storeGrid8,
            title: "На разбор", UO_title: "На разбор",

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],

            plugins: [rowEditing_8],
            rowEditing_8: rowEditing_8,
        });

        // 9. Выдан (Архив)
        var rowEditing_9 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid9 = Ext.create('widget.viewGridSecondHand', {
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
                    UO_DocX: "viewDocSecondHandWorkshops",
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
                }

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
                PanelGrid0, PanelGrid1, PanelGrid2, PanelGrid5, PanelGrid8, PanelGrid7, PanelGrid9
            ],

            listeners: {
                tabchange: function (tabPanel, newTab, oldTab, index) {

                    var widgetXForm = Ext.getCmp("form_" + tabPanel.UO_id).setVisible(false);
                    if (newTab.itemId != "PanelGrid9_") {

                        var newTabstore = newTab.store;
                        newTabstore.load({ waitMsg: lanLoading });
                        newTabstore.on('load', function () {
                            //newTabstore.load({ waitMsg: lanLoading });
                            //В наименовании вкладки ставим скобки и в них к-во аппаратов
                            newTab.setTitle(newTab.UO_title + " (" + newTabstore.data.length + ")");
                        });
                    }

                    //debugger;
                    //Скрыть/Показать колонки с датами продажи и возврата
                    var index1 = Ext.getCmp('PanelGrid9_' + newTab.UO_id).headerCt.items.findIndex('dataIndex', 'DateRetail');
                    var column1 = Ext.getCmp('PanelGrid9_' + newTab.UO_id).getColumns()[index1];
                    var index2 = Ext.getCmp('PanelGrid9_' + newTab.UO_id).headerCt.items.findIndex('dataIndex', 'DateReturn');
                    var column2 = Ext.getCmp('PanelGrid9_' + newTab.UO_id).getColumns()[index2];
                    if (newTab.itemId == "PanelGrid9_") {
                        if (!column1.isVisible()) {
                            column1.show();
                            column2.show();
                        }
                    }
                    else {
                        if (column1.isVisible()) {
                            column1.hide();
                            column2.hide();
                        }
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
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            //autoScroll: true,
            autoHeight: true,

            items: [

                //*** Не видно *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Организация", name: "DirContractorIDOrg", readOnly: true, flex: 1, id: "DirContractorIDOrg" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Склад", name: "DirWarehouseID", readOnly: true, flex: 1, id: "DirWarehouseID" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: lanDate, name: "DocDate", id: "DocDate" + this.UO_id, width: 200, readOnly: true, allowBlank: false, editable: false, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirSecondHandStatusID", name: "DirSecondHandStatusID", id: "DirSecondHandStatusID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                //К-во предыдущих ремонтов
                { xtype: 'textfield', fieldLabel: "QuantityOk", name: "QuantityOk", id: "QuantityOk" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "QuantityFail", name: "QuantityFail", id: "QuantityFail" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "QuantityCount", name: "QuantityCount", id: "QuantityCount" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirSecondHandContractorID", name: "DirSecondHandContractorID", id: "DirSecondHandContractorID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                //*** Видно *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        {
                            //title: "Аппарат",
                            autoHeight: true,
                            xtype: 'fieldset', flex: 1, layout: 'anchor',
                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                            items: [
                                                //{ xtype: 'textfield', fieldLabel: "Неисправность со слов клиента", labelWidth: 200, name: "ProblemClientWords", id: "ProblemClientWords" + this.UO_id, readOnly: true, flex: 1, allowBlank: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>Неисправность</b>', labelWidth: 120, name: "ProblemClientWords", id: "ProblemClientWords" + this.UO_id, readOnly: true, allowBlank: false, flex: 1 },

                                                { xtype: 'displayfield', fieldLabel: '<b>Сумма сдел.</b>', name: "PriceVAT", id: "PriceVAT" + this.UO_id, readOnly: true, allowBlank: false, width: 150, labelWidth: 85 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Сумма рем.</b>', name: "SumTotal2", id: "SumTotal2" + this.UO_id, readOnly: true, allowBlank: false, width: 150, labelWidth: 85 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Сумма апп.</b>', name: "PriceVATSums", id: "PriceVATSums" + this.UO_id, readOnly: true, allowBlank: false, width: 150, labelWidth: 85 },
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
                                                { xtype: 'displayfield', fieldLabel: '<b>' + lanPassword + '</b>', name: "ComponentPasText", id: "ComponentPasText" + this.UO_id, readOnly: true, allowBlank: false, width: 140, labelWidth: 50 },

                                                { xtype: 'displayfield', fieldLabel: '<b>Из СЦ</b>', name: "FromService", id: "FromService" + this.UO_id, readOnly: true, allowBlank: false, width: 150, labelWidth: 85, hidden: true },
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
                                        { xtype: 'displayfield', fieldLabel: '<b>Квитанция</b>', name: "DocSecondHandPurchID", id: "DocSecondHandPurchID" + this.UO_id, readOnly: true, allowBlank: false, flex: 2 },
                                        /*
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
                                        */
                                    ]
                                },
                                

                            ]
                        },

                    ]
                },

            ],

        });


        //1.1. Грид: Выполненная работа
        //var rowEditing1 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid21 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "grid1",
            id: "grid1_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,          //ObjectID
            UO_idMain: this.UO_idMain,  //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall,  //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,      //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            flex:1,
            hideHeaders: true,
            store: this.storeDocSecondHandPurch1TabsGrid, //storeDocAccountTabsGrid,

            columns: [
                { text: "№", dataIndex: "DocSecondHandPurch1TabID", width: 50, hidden: true, sortable: false, tdCls: 'x-change-cell-12' },
                //Услуга
                { text: "№", dataIndex: "DirSecondHandJobNomenID", width: 50, sortable: false, tdCls: 'x-change-cell-12' },
                { text: "Выполненная работа", dataIndex: "DirServiceJobNomenName", flex: 1, sortable: false, editor: { xtype: 'textfield' }, tdCls: 'x-change-cell-12' },
                { text: lanPrice, dataIndex: "PriceCurrency", width: 75, sortable: false, editor: { xtype: 'textfield' }, tdCls: 'x-change-cell-12' },

                { text: lanEmployee, dataIndex: "DirEmployeeName", width: 125, sortable: false, tdCls: 'x-change-cell-12' },
                {
                    text: "Комментарий", dataIndex: "DiagnosticRresults", width: 100, sortable: false,
                    renderer : function(value, metadata,record) {
                        return controllerDocSecondHandWorkshops_PanelGrid1_DiagnosticRresults(value, metadata, record);
                    },
                    tdCls: 'x-change-cell-12'
                },
                { text: lanDate, dataIndex: "TabDate", width: 80, sortable: false, tdCls: 'x-change-cell-12' },
                //{ text: "DirSecondHandStatusID", dataIndex: "DirSecondHandStatusID", width: 80, sortable: false, tdCls: 'x-change-cell-12' },
            ],

            tbar: [
                //{ xtype: "label", text: "Выполненная работа " },
                {
                    xtype: 'displayfield', fieldLabel: 'Выполненная работа на сумму ', name: "SumDocSecondHandPurch1Tabs2", id: "SumDocSecondHandPurch1Tabs2" + this.UO_id,
                    readOnly: true, allowBlank: false, labelWidth: 180, //flex: 2,
                },
                /*{
                    xtype: "displayfield",
                    name: "SumDocSecondHandPurch1Tabs2", id: "SumDocSecondHandPurch1Tabs2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    text: " на сумму 0 "
                },*/

                "->",

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

            //plugins: [rowEditing1], rowEditing1: rowEditing1,

            viewConfig: {
                getRowClass: function (record, index) {
                    
                    var StatusID = parseFloat(record.get('DirSecondHandStatusID'));

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
                                        controllerDocSecondHandWorkshops_onGrid1Edit(grid.UO_id, record, 4);
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
                                        controllerDocSecondHandWorkshops_onGrid1Edit(grid.UO_id, record, 3);
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
            store: this.storeDocSecondHandPurch2TabsGrid, //storeDocAccountTabsGrid,

            columns: [
                { text: "№", dataIndex: "DocSecondHandPurch2TabID", width: 50, hidden: true, sortable: false, tdCls: 'x-change-cell-12' },
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 50, hidden: true, sortable: false, tdCls: 'x-change-cell-12' },
                //Услуга
                { text: "№", dataIndex: "DirNomenID", width: 50, sortable: false, tdCls: 'x-change-cell-12' },
                { text: "Запчасть", dataIndex: "DirNomenName", flex: 1, sortable: false, tdCls: 'x-change-cell-12' }, //flex: 1
                { text: lanPrice, dataIndex: "PriceCurrency", width: 100, sortable: false, tdCls: 'x-change-cell-12' }, //, editor: { xtype: 'textfield' }

                { text: lanEmployee, dataIndex: "DirEmployeeName", width: 150, sortable: false, tdCls: 'x-change-cell-12' },
                { text: lanDate, dataIndex: "TabDate", width: 80, sortable: false, tdCls: 'x-change-cell-12' },
            ],

            tbar: [
                //{ xtype: "label", text: "Запчасти " },
                {
                    xtype: 'displayfield', fieldLabel: 'Запчасти на сумму', name: "SumDocSecondHandPurch2Tabs2", id: "SumDocSecondHandPurch2Tabs2" + this.UO_id,
                    readOnly: true, allowBlank: false, labelWidth: 180, //flex: 2,
                },
                "->",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "Склад", tooltip: "Запчасть со склада",
                    itemId: "btnGridAddPosition2",
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
                    
                    var IsZakaz = record.get('IsZakaz');

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
        var PanelGridLog0 = Ext.create('widget.viewGridSecondHandLog', { //widget.viewGridDoc
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
            store: this.storeLogSecondHandsGrid0, //storeDocAccountTabsGrid,
            title: "Все",

        });
        //Смена статуса
        var PanelGridLog1 = Ext.create('widget.viewGridSecondHandLog', { //widget.viewGridDoc
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
            store: this.storeLogSecondHandsGrid1, //storeDocAccountTabsGrid,
            title: "Статус",

        });
        //Комментарии и заметки
        var PanelGridLog3 = Ext.create('widget.viewGridSecondHandLog', { //widget.viewGridDoc
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
            store: this.storeLogSecondHandsGrid3, //storeDocAccountTabsGrid,
            title: "Заметки",

        });
        //Выполненная работа
        var PanelGridLog5 = Ext.create('widget.viewGridSecondHandLog', { //widget.viewGridDoc
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
            store: this.storeLogSecondHandsGrid5, //storeDocAccountTabsGrid,
            title: "Работа",

        });
        //Запчасть
        var PanelGridLog6 = Ext.create('widget.viewGridSecondHandLog', { //widget.viewGridDoc
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
            store: this.storeLogSecondHandsGrid6, //storeDocAccountTabsGrid,
            title: "Запчасть",

        });
        //Другое
        var PanelGridLog7 = Ext.create('widget.viewGridSecondHandLog', { //widget.viewGridDoc
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
            store: this.storeLogSecondHandsGrid7, //storeDocAccountTabsGrid,
            title: "Другое",

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
                PanelGridLog0, PanelGridLog1, PanelGridLog3, PanelGridLog5, PanelGridLog6, PanelGridLog7 //, PanelGridLog4, PanelGridLog8, PanelGridLog9
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
                /*{
                    id: "btnSMS" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    tooltip: "Отправить SMS", icon: '../Scripts/sklad/images/sms16.png',
                    itemId: "btnSMS",
                },*/
                /*{
                    id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    text: "Предыдущие ремонты", tooltip: "Предыдущие ремонты", icon: '../Scripts/sklad/images/tools.png',
                    itemId: "btnHistory",
                },*/

                "->",

                /*{
                    xtype: "datafield",
                    id: "labelAlerted" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    text: "Не оповещён"
                },*/
                { xtype: 'displayfield', name: "Alerted", id: "Alerted" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 85 },
            ],

        });





        //4. Футер: Кнопки-Статусы
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

                        //Статусы
                        
                        {
                            xtype: "button", tooltip: "Предпродажа", icon: '../Scripts/sklad/images/Status/question24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus2",
                            enableToggle: true, pressed: false,
                            
                        },
                        

                        /*
                        {
                            xtype: "button", tooltip: "Предпродажа", icon: '../Scripts/sklad/images/Status/onagreeing24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus3" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus3",
                            enableToggle: true, pressed: false,
                            style: { background: 'yellow' }
                        },
                        */
                        /*
                        {
                            xtype: "button", tooltip: "Согласован", icon: '../Scripts/sklad/images/Status/ok.png', style: "width: 50px; height: 35px;", scale: 'large', //, text: "<b style='font-size: 125%'>OK<b>"
                            id: "btnStatus4" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus4",
                            enableToggle: true, pressed: false,
                            style: { background: 'yellow' }
                        },
                        */

                        {
                            xtype: "button", tooltip: "Ожидание запчастей", icon: '../Scripts/sklad/images/Status/waiting24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus5" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus5",
                            enableToggle: true, pressed: false,
                            style: { background: '#8A2BE2' }
                        },
                        {
                            xtype: "button", tooltip: "Отремонтирован", icon: '../Scripts/sklad/images/Status/renovated24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus7" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus7",
                            enableToggle: true, pressed: false,
                            style: { background: '#90EE90' }
                        },
                        /*{
                            xtype: "button", tooltip: "В основном сервисе", icon: '../Scripts/sklad/images/Status/remote_sc24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus6" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus6",
                            enableToggle: true, pressed: false,
                            style: { background: '#808080' }
                        },*/
                        {
                            xtype: "button", tooltip: "На разбор", icon: '../Scripts/sklad/images/Status/renouncement24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus8" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus8",
                            enableToggle: true, pressed: false,
                            style: { background: 'red' }
                        },


                        //Только для вкладки "Выдать"
                        { xtype: 'textfield', fieldLabel: "Работа", labelAlign: 'top', name: "SumDocSecondHandPurch1Tabs", id: "SumDocSecondHandPurch1Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, margin: "0 0 0 5", hidden: true },
                        { xtype: 'textfield', fieldLabel: "Запчасти", labelAlign: 'top', name: "SumDocSecondHandPurch2Tabs", id: "SumDocSecondHandPurch2Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "Итого", labelAlign: 'top', name: "SumTotal", id: "SumTotal" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        //{ xtype: 'textfield', fieldLabel: "Предоплата", margin: "0 0 0 15", labelAlign: 'top', name: "PrepaymentSum", id: "PrepaymentSum" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "<b style='color: green;'>Сумма рем.</b>", labelAlign: 'top', name: "SumTotal2a", id: "SumTotal2a" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true, fieldStyle: 'color: red; font-weight: bold;' },
                        {
                            xtype: 'button', margin: "0 0 0 25",
                            id: "btnSave" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave", style: "width: 175px; height: 40px;",
                            text: "<b style='font-size: 22px; color: green;'>В продажу</b>", icon: '../Scripts/sklad/images/save_held.png',
                            hidden: true
                        },
                        {
                            xtype: 'button', margin: "0 0 0 25",
                            id: "btnRazbor" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnRazbor", style: "width: 175px; height: 40px;",
                            text: "<b style='font-size: 22px; color: red;'>На разбор</b>", icon: '../Scripts/sklad/images/save_held.png',
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
                                                    inputValue: 41
                                                },
                                                /*{
                                                    boxLabel: 'Акт',
                                                    name: 'ListObjectPFID',
                                                    inputValue: 33
                                                },
                                                {
                                                    boxLabel: 'Гарантия',
                                                    name: 'ListObjectPFID',
                                                    inputValue: 35
                                                },*/
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
