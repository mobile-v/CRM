Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocOrderInts/viewDocOrderInts", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocOrderInts",

    layout: "border",
    region: "center",
    title: "Заказы",
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

    conf: {},

    initComponent: function () {


        //*** Шапка *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        // 0. Все
        var rowEditing_0 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid0 = Ext.create('widget.viewGridOrder', {
            conf: {
                id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid0_",
            store: this.storeGrid0,
            title: "Все",

            plugins: [rowEditing_0],
            rowEditing_0: rowEditing_0,

        });
        // 1. Мастерская
        var rowEditing_1 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid1 = Ext.create('widget.viewGridOrder', {
            conf: {
                id: "PanelGrid1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            //region: "center",
            itemId: "PanelGrid1_",
            store: this.storeGrid1,
            title: "Предзаказы (Лёгкий)",

            plugins: [rowEditing_1],
            rowEditing_1: rowEditing_1,
        });
        // 2. Предзаказы
        var rowEditing_2 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid2 = Ext.create('widget.viewGridOrder', {
            conf: {
                id: "PanelGrid2_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid5_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid2_",
            store: this.storeGrid2,
            title: "Впрок (Аналитика)",

            plugins: [rowEditing_2],
            rowEditing_2: rowEditing_2,
        });
        // 3. Впрок
        var rowEditing_3 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid3 = Ext.create('widget.viewGridOrder', {
            conf: {
                id: "PanelGrid3_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid2_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid3_",
            store: this.storeGrid3,
            title: "Мастерская",

            plugins: [rowEditing_3],
            rowEditing_3: rowEditing_3,
        });
        // 4. Впрок
        var rowEditing_4 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid4 = Ext.create('widget.viewGridOrder', {
            conf: {
                id: "PanelGrid4_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            //id: "PanelGrid2_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid4_",
            store: this.storeGrid4,
            title: "Б/У",

            plugins: [rowEditing_4],
            rowEditing_4: rowEditing_4,
        });
        // 9. Выдан (Архив)
        var rowEditing_9 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid9 = Ext.create('widget.viewGridOrder', {
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
            title: "Архив",

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
                PanelGrid0, PanelGrid1, PanelGrid2, PanelGrid3, PanelGrid4, PanelGrid9
            ],

            listeners: {
                tabchange: function (tabPanel, newTab, oldTab, index) {
                    var widgetXForm = Ext.getCmp("form_" + tabPanel.UO_id).setVisible(false);
                    if (newTab.itemId != "PanelGrid9_") newTab.store.load({ waitMsg: lanLoading });
                }
            },

        });




        //*** Форма "Мастерская" *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

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
            width: "100%", height: 50, //width: 500, height: 200,
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
                { xtype: 'textfield', fieldLabel: "Склад", name: "DirWarehouseName", readOnly: true, flex: 1, id: "DirWarehouseName" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Товар", name: "DirNomenXName6", readOnly: true, flex: 1, id: "DirNomenXName6" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: lanDate, name: "DocDate", id: "DocDate" + this.UO_id, width: 200, readOnly: true, allowBlank: false, editable: false, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirOrderIntStatusID", name: "DirOrderIntStatusID", id: "DirOrderIntStatusID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


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
                                                { xtype: 'displayfield', fieldLabel: '<b>ФИО</b>', name: "DirOrderIntContractorName", id: "DirOrderIntContractorName" + this.UO_id, readOnly: true, allowBlank: false, flex: 2, labelWidth: 45 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Телефон</b>', name: "DirOrderIntContractorPhone", id: "DirOrderIntContractorPhone" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 60 },
                                                { xtype: 'displayfield', fieldLabel: '<b>№</b>', name: "DocOrderIntID", id: "DocOrderIntID" + this.UO_id, readOnly: true, allowBlank: false, flex: 2 },

                                                { xtype: 'displayfield', fieldLabel: 'DirContractorID', name: "DirContractorID", id: "DirContractorID" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 85, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: 'DirPriceTypeID', name: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 85, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: 'DirNomenID', name: "DirNomenID", id: "DirNomenID" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 85, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: 'PriceVAT', name: "PriceVAT", id: "PriceVAT" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 85, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>Ор.стоимость</b>', name: "PriceCurrency", id: "PriceCurrency" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 85 },

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


        var PanelGridLog = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "gridLog",
            id: "gridLog0_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            region: "center",
            flex: 1,
            split: true,
            store: this.storeLogOrderIntsGrid, //storeDocAccountTabsGrid,

            columns: [
                /*
                //Услуга
                //{ text: "№", dataIndex: "LogOrderIntID", width: 50 },
                { text: "Тип", dataIndex: "DirOrderIntLogTypeName", width: 100 },
                { text: "Сотрудник", dataIndex: "DirEmployeeName", width: 100 },
                { text: "Статус", dataIndex: "DirOrderIntStatusName", width: 100 },
                { text: "Дата", dataIndex: "LogOrderIntDate", width: 75 },
                { text: "Текст", dataIndex: "Msg", flex: 1 },
                */

                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
                /*{
                    dataIndex: "Field2", flex: 1, 
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },*/
            ],

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
                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirOrderIntLogTypeName" || FN == "DirOrderIntStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                                //if (FN != "DirEmployeeName" || !(FN == "Msg" && record.data["Msg"] == "")) record.data["Field1"] += " - ";

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogOrderIntDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig

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

                        //Статусы
                        {
                            xtype: "button", tooltip: "Новый", icon: '../Scripts/sklad/images/Status/onagreeing24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus10" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus10",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "В ожидании", icon: '../Scripts/sklad/images/Status/work24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus20" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus20",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "В пути", icon: '../Scripts/sklad/images/Status/waiting24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus30" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus30",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "Готов к выдачи", icon: '../Scripts/sklad/images/Status/renovated24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus35" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus35",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "Исполнен + создать документ Поступление", icon: '../Scripts/sklad/images/Status/shop_16.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus40" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus40",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "Отказ", icon: '../Scripts/sklad/images/Status/renouncement24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus50" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus50",
                            enableToggle: true, pressed: false
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
            width: "100%", height: 250,
            bodyPadding: 5,
            autoHeight: true,

            items: [
                PanelCap1,
                //PanelGridUnion,
                //PanelStatusButton,
                PanelGridLog,

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

