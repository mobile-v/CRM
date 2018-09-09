Ext.define("PartionnyAccount.view.Sklad/Object/Report/viewDocServicePurchesReport", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocServicePurchesReport",

    layout: "border",
    region: "center",
    title: "Отчет по сервисному центру",
    width: 450, height: 300,
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
        
        //Form-Panel
        var ReportType_values = [
            [0, 'Все'],
            [1, 'Выданные (все)'],
            [2, 'Выданные (готовые)'],
            [3, 'Не отремонтированные все'], //Выданные (отказные)
            [4, 'Отремонтированные все'], //Сделанные
            [5, 'Принятые'],
        ];
        var formPanel = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)


            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "north", //!!! Важно для Ресайз-а !!!
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

            width: "100%", height: 110,
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [

                //Не видно *** *** *** *** ***
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, hidden: true,
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", flex: 2, allowBlank: false, //, emptyText: "..."

                            store: this.storeDirContractorsOrgGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                            
                        },

                        { xtype: "checkbox", boxLabel: "Гарантийный", name: "TypeRepair", itemId: "TypeRepair", inputValue: true, id: "TypeRepair" + this.UO_id },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Принял", flex: 1, allowBlank: true, //, emptyText: "..."

                            store: this.storeDirEmployeesGrid, // store getting items from server
                            valueField: 'DirEmployeeID',
                            hiddenName: 'DirEmployeeID',
                            displayField: 'DirEmployeeName',
                            name: 'DirEmployeeID', itemId: "DirEmployeeID", id: "DirEmployeeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnDirEmployeesClear", id: "btnDirEmployeesClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                        //DirServiceContractor
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Клиент", flex: 1, allowBlank: true, //, emptyText: "..."

                            store: this.storeDirServiceContractorsGrid, // store getting items from server
                            valueField: 'DirServiceContractorID',
                            hiddenName: 'DirServiceContractorID',
                            displayField: 'DirServiceContractorName',
                            name: 'DirServiceContractorID', itemId: "DirServiceContractorID", id: "DirServiceContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnDirServiceContractorsClear", id: "btnDirServiceContractorsClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },


                
                //Видно *** *** *** *** ***
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'viewDateField', fieldLabel: "С", name: "DateS", id: "DateS" + this.UO_id, allowBlank: false },
                        { xtype: 'viewDateField', fieldLabel: "по", name: "DatePo", id: "DatePo" + this.UO_id, margin: "0 0 0 25", allowBlank: false },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouse, flex: 1, allowBlank: true, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirWarehousesGrid, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanState, flex: 1, allowBlank: true,
                            store: this.storeDirServiceStatusesGrid, // store getting items from server
                            valueField: 'DirServiceStatusID',
                            hiddenName: 'DirServiceStatusID',
                            displayField: 'DirServiceStatusName',
                            name: 'DirServiceStatusID', itemId: "DirServiceStatusID", id: "DirServiceStatusID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                        },
                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnDirServiceStatusesClear", id: "btnDirServiceStatusesClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Принял", flex: 1, allowBlank: true, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirEmployeesMasterGrid, // store getting items from server
                            valueField: 'DirEmployeeID',
                            hiddenName: 'DirEmployeeID',
                            displayField: 'DirEmployeeName',
                            name: 'DirEmployeeIDMaster', itemId: "DirEmployeeIDMaster", id: "DirEmployeeIDMaster" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnDirEmployeesMasterClear", id: "btnDirEmployeesMasterClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Тип отчета", allowBlank: true, flex: 1,
                            margin: "0 0 0 5",
                            store: new Ext.data.SimpleStore({
                                fields: ['ReportType', 'ReportTypeName'],
                                data: ReportType_values
                            }),

                            valueField: 'ReportType',
                            hiddenName: 'ReportType',
                            displayField: 'ReportTypeName',
                            name: 'ReportType', itemId: "ReportType", id: "ReportType" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

            ],

            buttons: [
                {
                    text: lanPrint, icon: '../Scripts/sklad/images/print.png',
                    menu: [
                        {
                            id: "btnPrintRu" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintRu",
                            text: lanLanguageRu, UO_Language: 0,
                            icon: '../Scripts/sklad/images/Flag/ru.png'
                        },
                        {
                            id: "btnPrintUa" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintUa",
                            text: lanLanguageUa, UO_Language: 1,
                            icon: '../Scripts/sklad/images/Flag/ua.png'
                        }
                    ]
                },
                " ",
                {
                    id: "btnCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png', UO_Action: "cancel",
                },


                " ", " ", " ", " ",
                {
                    id: "labelCount_DocDate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "labelCount_DocDate",
                    xtype: "label", text: "Приёмка: 0",
                },
                " ", " ",
                {
                    id: "labelCount_IssuanceDate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "labelCount_IssuanceDate",
                    xtype: "label", text: "Готов/Отказ: 0",
                },
                " ", " ",
                {
                    id: "labelCount_DateStatusChange" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "labelCount_DateStatusChange",
                    xtype: "label", text: "Выдача: 0",
                },

                "->",
                {
                    id: "btnReport" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnReport",
                    text: lanReport, icon: '../Scripts/sklad/images/reports.png', UO_Action: "report",
                },
            ],

        });


        //2. Грид
        var PanelGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            itemId: "grid",

            conf: {},

            region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeDocServicePurchesReport,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [
                { text: "№", dataIndex: "DocServicePurchID", width: 70,  },
                { text: "Аппарат", dataIndex: "DirServiceNomenName", flex: 1,  },
                { text: "IMEI", dataIndex: "SerialNumber", width: 75 },
                { text: "ФИО", dataIndex: "DirServiceContractorName", flex: 1, },

                { text: "Приёмка", dataIndex: "DocDate", width: 75, id: "DocDate" + this.UO_id, },
                { text: "Готов/Отказ", dataIndex: "IssuanceDate", width: 75, id: "IssuanceDate" + this.UO_id, },
                { text: "Выдача", dataIndex: "DateStatusChange", width: 75, id: "DateStatusChange" + this.UO_id, },

                { text: "Мастер", dataIndex: "DirEmployeeNameMaster", flex: 1, },
                { text: "Предоплата", dataIndex: "PrepaymentSum", summaryType: 'sum', width: 75, },

                { text: "Скидка1", dataIndex: "DiscountX", summaryType: 'sum', width: 75, },
                { text: "Выполненная работа", dataIndex: "SumDocServicePurch1Tabs", summaryType: 'sum', width: 75, },

                { text: "Скидка2", dataIndex: "DiscountY", summaryType: 'sum', width: 75, },
                { text: "Запчасти", dataIndex: "SumDocServicePurch2Tabs", summaryType: 'sum', width: 75, },

                { text: "Итого", dataIndex: "SumTotal", summaryType: 'sum', width: 75, },
                { text: "К оплате", dataIndex: "SumTotal2", summaryType: 'sum', width: 75, }
            ],


            //Формат даты
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
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d"); // H:i:s
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }
                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig

        });


        //body
        this.items = [

            {
                xtype: "panel",
                layout: 'border',
                region: "center",
                items: [
                    formPanel,
                    PanelGrid
                ]
            },

        ],


        this.callParent(arguments);
    }

});