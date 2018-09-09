Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocServicePurches/viewDocServiceWorkshopHistories", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocServiceWorkshopHistories",

    layout: "border",
    region: "center",
    title: "Сервис - Предыдущие ремнты",
    width: 950, height: 600,
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
        var PanelGrid0 = Ext.create('widget.viewGridService', {
            conf: {
                id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            region: "center",
            bodyStyle: 'background:transparent;',
            width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            //id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelGrid0_",
            store: this.storeGrid0,
            //title: "Все",
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
            width: "100%", height: 80, //width: 500, height: 200,
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
                { xtype: 'textfield', fieldLabel: "DirServiceStatusID", name: "DirServiceStatusID", id: "DirServiceStatusID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


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
                                        { xtype: 'textfield', fieldLabel: "Неисправность со слов клиента", name: "ProblemClientWords", id: "ProblemClientWords" + this.UO_id, readOnly: true, flex: 1, allowBlank: true },
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                            items: [
                                                { xtype: 'displayfield', fieldLabel: '<b>Аппарат</b>', name: "DirServiceNomenNameLittle", id: "DirServiceNomenNameLittle" + this.UO_id, readOnly: true, allowBlank: false, flex: 2, labelWidth: 60, hidden: true },
                                                { xtype: 'displayfield', fieldLabel: '<b>Аппарат</b>', name: "DirServiceNomenName", id: "DirServiceNomenName" + this.UO_id, readOnly: true, allowBlank: false, flex: 2, labelWidth: 60  },
                                                { xtype: 'displayfield', fieldLabel: '<b>ФИО</b>', name: "DirServiceContractorName", id: "DirServiceContractorName" + this.UO_id, readOnly: true, allowBlank: false, flex: 2, labelWidth: 45 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Телефон</b>', name: "DirServiceContractorPhone", id: "DirServiceContractorPhone" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 60 },
                                                { xtype: 'displayfield', fieldLabel: '<b>Ор.стоимость</b>', name: "PriceVAT", id: "PriceVAT" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 85 },
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
                                        { xtype: 'displayfield', fieldLabel: '<b>Квитанция</b>', name: "DocServicePurchID", id: "DocServicePurchID" + this.UO_id, readOnly: true, allowBlank: false, flex: 2 },
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
                                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            readOnly: true
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
        var PanelGrid1 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "grid1",
            id: "grid1_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            flex:1,
            hideHeaders: true,
            store: this.storeDocServicePurch1TabsGrid, //storeDocAccountTabsGrid,

            columns: [
                { text: "№", dataIndex: "DocServicePurch1TabID", width: 50, hidden: true, sortable: false },
                //Услуга
                { text: "№", dataIndex: "DirServiceJobNomenID", width: 50, sortable: false },
                { text: "Выполненная работа", dataIndex: "DirServiceJobNomenName", flex: 1, sortable: false, editor: { xtype: 'textfield' } },
                { text: lanPrice, dataIndex: "PriceCurrency", width: 75, sortable: false, editor: { xtype: 'textfield' } },

                { text: lanEmployee, dataIndex: "DirEmployeeName", width: 125, sortable: false },
                { text: "Комментарий", dataIndex: "DiagnosticRresults", width: 150, sortable: false },
            ],

            tbar: [
                { xtype: "label", text: "Выполненная работа " },
                /*"->",

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
                }*/
            ],

        });

        //1.2. Грид: Запчасти
        var PanelGrid2 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
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
                { text: "№", dataIndex: "DocServicePurch2TabID", width: 50, hidden: true, sortable: false },
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 50, hidden: true, sortable: false },
                //Услуга
                { text: "№", dataIndex: "DirNomenID", width: 50, sortable: false },
                { text: "Запчасть", dataIndex: "DirNomenName", flex: 1, sortable: false }, //flex: 1
                { text: lanPrice, dataIndex: "PriceCurrency", width: 100, sortable: false }, //, editor: { xtype: 'textfield' }

                { text: lanEmployee, dataIndex: "DirEmployeeName", width: 150, sortable: false }
            ],

            tbar: [
                { xtype: "label", text: "Запчасти " },
                /*"->",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "Запчасть", tooltip: lanAddPosition,
                    itemId: "btnGridAddPosition2",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion2" + this.UO_id, itemId: "btnGridDeletion2"
                }*/
            ],

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
                PanelGrid1,
                PanelGrid2
            ],

        });

        //3. Справа: Статусы-Кнопки
        var PanelGridLog0 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "gridLog0",
            id: "gridLog0_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            region: "east",
            flex: 1,
            split: true,
            store: this.storeLogServicesGrid, //storeDocAccountTabsGrid,

            columns: [
                /*
                //Услуга
                //{ text: "№", dataIndex: "LogServiceID", width: 50 },
                { text: "Тип", dataIndex: "DirServiceLogTypeName", width: 100 },
                { text: "Сотрудник", dataIndex: "DirEmployeeName", width: 100 },
                { text: "Статус", dataIndex: "DirServiceStatusName", width: 100 },
                { text: "Дата", dataIndex: "LogServiceDate", width: 75 },
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
                /*
                {
                    id: "btnSMS" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    tooltip: "Отправить SMS", icon: '../Scripts/sklad/images/sms16.png', //text: "SMS", 
                    itemId: "btnSMS",
                },
                {
                    id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    text: "Предыдущие ремонты", tooltip: "Предыдущие ремонты", icon: '../Scripts/sklad/images/tools.png', //text: "History", 
                    itemId: "btnHistory",
                },
                */
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
                            xtype: "button", tooltip: "В диагностике", icon: '../Scripts/sklad/images/Status/question24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus2",
                            enableToggle: true, pressed: false
                        },


                        {
                            xtype: "button", tooltip: "На согласовании", icon: '../Scripts/sklad/images/Status/onagreeing24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus3" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus3",
                            enableToggle: true, pressed: false
                        },
                        /*{
                            xtype: "button", tooltip: "Согласован", icon: '../Scripts/sklad/images/Status/agreeing24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus4" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus4",
                            enableToggle: true, pressed: false
                        },*/


                        {
                            xtype: "button", tooltip: "Ожидание запчастей", icon: '../Scripts/sklad/images/Status/waiting24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus5" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus5",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "В работе", icon: '../Scripts/sklad/images/Status/work24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus4" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus4",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "Отремонтирован", icon: '../Scripts/sklad/images/Status/renovated24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus7" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus7",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "В основном сервисе", icon: '../Scripts/sklad/images/Status/remote_sc24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus6" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus6",
                            enableToggle: true, pressed: false
                        },
                        {
                            xtype: "button", tooltip: "Отказной", icon: '../Scripts/sklad/images/Status/renouncement24.png', style: "width: 50px; height: 35px;", scale: 'large',
                            id: "btnStatus8" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus8",
                            enableToggle: true, pressed: false
                        },


                        //Только для вкладки "Выдать"
                        { xtype: 'textfield', fieldLabel: "Работа", labelAlign: 'top', name: "SumDocServicePurch1Tabs", id: "SumDocServicePurch1Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, margin: "0 0 0 5", hidden: true },
                        { xtype: 'textfield', fieldLabel: "Запчасти", labelAlign: 'top', name: "SumDocServicePurch2Tabs", id: "SumDocServicePurch2Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "Итого", labelAlign: 'top', name: "SumTotal", id: "SumTotal" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "Предоплата", margin: "0 0 0 15", labelAlign: 'top', name: "PrepaymentSum", id: "PrepaymentSum" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "<b style='color: red;'>К оплате</b>", labelAlign: 'top', name: "SumTotal2a", id: "SumTotal2a" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true, hidden: true, fieldStyle: 'color: red; font-weight: bold;' },
                        {
                            xtype: 'button', margin: "0 0 0 25",
                            id: "btnSave" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave", style: "width: 175px; height: 40px;",
                            text: "<b style='font-size: 22px; color: red;'>В ы д а т ь</b>", icon: '../Scripts/sklad/images/save_held.png',
                            hidden: true
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
                PanelGridLog0,

                //PanelFooter
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
                    PanelGrid0,
                    formPanel1
                ]
            }

        ],


        this.callParent(arguments);
    }

});