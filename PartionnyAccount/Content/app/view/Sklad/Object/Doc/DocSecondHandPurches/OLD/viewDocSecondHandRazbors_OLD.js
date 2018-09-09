Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandRazbors", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocSecondHandRazbors",

    layout: "border",
    region: "center",
    title: "Б/У - Разборка аппаратов",
    width: 750, height: 275,
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
        var PanelGrid0 = Ext.create('widget.viewGridSecondHandRazbor', {
            conf: {
                id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid0_",
            UO_itemId: "PanelGrid9_",
            store: this.storeGrid0,
            title: "Все", UO_title: "Все",

            plugins: [rowEditing_0],
            rowEditing_0: rowEditing_0,

        });
        // 7. Готов для продажи
        var rowEditing_7 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid7 = Ext.create('widget.viewGridSecondHandRazbor', {
            conf: {
                id: "PanelGrid7_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid7_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid7_",
            UO_itemId: "PanelGrid9_",
            store: this.storeGrid7,
            title: "Готов для продажи", UO_title: "Готов для продажи",

            plugins: [rowEditing_7],
            rowEditing_7: rowEditing_7,
        });
        // 9. Выдан (Архив)
        var rowEditing_9 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid9 = Ext.create('widget.viewGridSecondHandRazbor', {
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
                    UO_DocX: "viewDocSecondHandRazbors",
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

            UO_itemId: "PanelGrid9_",
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
                PanelGrid0, PanelGrid7, PanelGrid9
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
                            newTab.setTitle(newTab.UO_title  + " (" + newTabstore.data.length + ")");
                        });
                    }

                    //Спрятать список товара
                    Ext.getCmp("tree_" + tabPanel.UO_id).collapse(Ext.Component.DIRECTION_LEFT, true);
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
            width: "100%", height: 50,
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
                                                { xtype: 'displayfield', fieldLabel: '<b>№ док.</b>', labelWidth: 100, name: "DocSecondHandRazborID", id: "DocSecondHandRazborID" + this.UO_id, readOnly: true, allowBlank: false, width: 250 },

                                                { xtype: 'displayfield', fieldLabel: '<b>Сумма аппарата (покупка)</b>', labelWidth: 140, name: "PriceVAT", id: "PriceVAT" + this.UO_id, readOnly: true, allowBlank: false, width: 250 },

                                                { xtype: 'displayfield', fieldLabel: '<b>Аппарат</b>', labelWidth: 100, name: "DirServiceNomenNameLittle", id: "DirServiceNomenNameLittle" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>Аппарат</b>', labelWidth: 100, name: "DirServiceNomenName", id: "DirServiceNomenName" + this.UO_id, readOnly: true, allowBlank: false, flex: 1 },

                                                { xtype: 'displayfield', fieldLabel: '<b>ID0</b>', name: "ID0", id: "ID0" + this.UO_id, readOnly: true, allowBlank: false, width: 0, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>ID1</b>', name: "ID1", id: "ID1" + this.UO_id, readOnly: true, allowBlank: false, width: 0, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>ID2</b>', name: "ID2", id: "ID2" + this.UO_id, readOnly: true, allowBlank: false, width: 0, hidden: true },
                                            ]
                                        },
                                    ]
                                },

                            ]
                        },

                    ]
                },

            ],

        });

        //1.2. Грид: Запчасти
        //var rowEditing2 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid2 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "grid", //itemId: "grid2",
            id: "grid_" + this.UO_id, //id: "grid2_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            flex: 1,
            hideHeaders: true,
            store: this.storeGrid, //storeDocSecondHandPurch2TabsGrid, //storeDocAccountTabsGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 50, hidden: true },
                //Товар
                { text: "№", dataIndex: "DirNomenID", width: 50, style: "height: 25px;" },
                { text: lanNomenclature, dataIndex: "DirNomenName", flex: 2 },
                //К-во
                { text: lanCount, dataIndex: "Quantity", width: 50 },
                //Цены
                { text: "Розница Наценка", dataIndex: "MarkupRetail", width: 100, hidden: true },
                { text: "Розница Цена", dataIndex: "PriceRetailCurrency", width: 100 },
                { text: "Опт Наценка", dataIndex: "MarkupWholesale", width: 100, hidden: true },
                { text: "Опт Цена", dataIndex: "PriceWholesaleCurrency", width: 100, hidden: true },
                { text: "IM Наценка", dataIndex: "MarkupIM", width: 100, hidden: true },
                { text: "IM Цена", dataIndex: "PriceIMCurrency", width: 100, hidden: true },
                //Суммы
                { text: lanPriceVatFull, dataIndex: "PriceCurrency", width: 100 },
                { text: lanSum, dataIndex: "SUMPurchPriceVATCurrency", width: 100, hidden: true },

                //Характеристики
                /*
                { text: "Характеристики", dataIndex: "DirChar", flex: 1, hidden: true },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                //{ text: "Стиль", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Поставщик", dataIndex: "DirContractorName", width: 100, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true },
                */

                //Минимальный остаток
                { text: lanMinimumBalance, dataIndex: "DirNomenMinimumBalance", width: 100, hidden: true },
            ],

            tbar: [
                {
                    xtype: 'displayfield',
                    fieldLabel: 'Запчасти на сумму',
                    name: 'SumOfVATCurrency', id: "SumOfVATCurrency" + this.UO_id,
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
                    icon: '../Scripts/sklad/images/table_delete.png', text: "Удалить", tooltip: "Удалить", disabled: true,
                    id: "btnGridDeletion2" + this.UO_id, itemId: "btnGridDeletion2"
                }
            ],

            //plugins: [rowEditing2],
            //rowEditing2: rowEditing2,

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
                PanelGrid2
            ],

        });





        //3. Справа: Статусы-Кнопки
        //Все
        var PanelGridLog0 = Ext.create('widget.viewGridSecondHandRazborLog', { //widget.viewGridDoc
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
                PanelGridLog0
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
               
                "->",
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
                            xtype: "button", tooltip: "Пред-Разборка", icon: '../Scripts/sklad/images/Status/question24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus2",
                            enableToggle: true, pressed: false,
                            
                        },
                        {
                            xtype: "button", tooltip: "Разобран", icon: '../Scripts/sklad/images/Status/renovated24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus7" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus7",
                            enableToggle: true, pressed: false,
                            style: { background: '#90EE90' }
                        },
                        
                        {
                            xtype: "button", tooltip: "Списать", icon: '../Scripts/sklad/images/Status/renouncement24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus8" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus8",
                            enableToggle: true, pressed: false,
                            style: { background: 'red' }
                        },


                        //Только для вкладки "Выдать"
                        { xtype: 'textfield', fieldLabel: "Работа", labelAlign: 'top', name: "SumDocSecondHandPurch1Tabs", id: "SumDocSecondHandPurch1Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, margin: "0 0 0 5", hidden: true },
                        { xtype: 'textfield', fieldLabel: "Запчасти", labelAlign: 'top', name: "SumDocSecondHandPurch2Tabs", id: "SumDocSecondHandPurch2Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "Итого", labelAlign: 'top', name: "SumTotal", id: "SumTotal" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        //{ xtype: 'textfield', fieldLabel: "Предоплата", margin: "0 0 0 15", labelAlign: 'top', name: "PrepaymentSum", id: "PrepaymentSum" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "<b style='color: red;'>Сумма рем.</b>", labelAlign: 'top', name: "SumTotal2a", id: "SumTotal2a" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true, fieldStyle: 'color: red; font-weight: bold;' },
                        {
                            xtype: 'button', margin: "0 0 0 25",
                            id: "btnSave" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave", style: "width: 175px; height: 40px;",
                            text: "<b style='font-size: 22px; color: red;'>В продажу</b>", icon: '../Scripts/sklad/images/save_held.png',
                            hidden: true
                        },


                        /*
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
                        */

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
            width: "100%", height: 300,
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

            Ext.create('widget.viewTreeDir', {

                conf: {
                    id: "tree_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                    UO_OnStop: this.UO_OnStop, //Что бы не было событий при перегрузке Дерева, то глючит (в контролере "controllerDirNomens" в методе "onTree_beforedrop" врубается ждущее событие "storeNomenTree.on(...)" и происходит перемещение объектов)
                },

                store: this.storeTree, //storeGrid,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirNomen"
                },
                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    //{ text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Доступ', width: 50, dataIndex: 'Active', sortable: true },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'DirNomenPatchFull', dataIndex: 'DirNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],

                listeners: {

                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirNomens_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirNomens_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDirNomens_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDirNomens_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirNomens_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirNomens_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDirNomens_onTree_addSub;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    },

                    expand: function (theParentNode) {
                        theParentNode.eachChild(
                            function (node) {
                                if (nodeIWantSelected == node) {
                                    this.getView().focusRow(node);
                                }
                            }
                        )
                    }

                }

            }),


            // *** *** *** *** *** *** *** *** ***




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

