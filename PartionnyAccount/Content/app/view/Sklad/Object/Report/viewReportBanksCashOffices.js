Ext.define("PartionnyAccount.view.Sklad/Object/Report/viewReportBanksCashOffices", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewReportBanksCashOffices",

    layout: "border",
    region: "center",
    title: "Отчет по Финансам",
    width: 450, height: 250,
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
            [1, 'Все операции'],
            [2, 'Продажи'],
            [7, 'Ремонты'],
            [8, 'Магаз.продажи + СЦ.Ремонты + БУ.продажи'],
            [3, 'Возвраты'],
            [4, 'Касса - Внесения'],
            [5, 'Касса - Выплаты'],
            [6, 'Z-отчет'],
            [9, 'Скидки'],
            [10, 'Расходы на покупку товара (накладные поступление)'],
            [11, 'Хоз.расходы - Выплаты'],
        ];
        var ReportGroup_values = [
            [1, 'Полный отчет'],
            [2, 'Группировать по дням'],
            [3, 'Группировать по дням и точке'],
            [4, 'Группировать по дням и сотруднику'],
            [5, 'Группировать по дням, точке и сотруднику'],
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

            width: "100%", height: 140,
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: false, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { boxLabel: 'Все', height: "35px", name: 'CasheBank', inputValue: 1, id: "CasheAndBank" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Наличные', height: "35px", name: 'CasheBank', inputValue: 1, id: "Cashe" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Безналичные', name: 'CasheBank', inputValue: 2, id: "Bank" + this.UO_id, UO_id: this.UO_id },
                            ]
                        },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'viewDateField', fieldLabel: "С", name: "DateS", id: "DateS" + this.UO_id, allowBlank: false },
                        { xtype: 'viewDateField', fieldLabel: "по", name: "DatePo", id: "DatePo" + this.UO_id, margin: "0 0 0 5", allowBlank: false },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Точка", flex: 1, allowBlank: true, //, emptyText: "..."
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

                //DirContractorIDOrg - НЕ Видимое поле
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
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
                            hidden: true,
                        },
                    ]
                },
                
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanEmployee, flex: 1, allowBlank: true, //, emptyText: "..."

                            store: this.storeDirEmployeesGrid, // store getting items from server
                            valueField: 'DirEmployeeID',
                            hiddenName: 'DirEmployeeID',
                            displayField: 'DirEmployeeName',
                            name: 'DirEmployeeID', itemId: "DirEmployeeID", id: "DirEmployeeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnDirEmployeesClear", id: "btnDirEmployeesClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

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

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Группировка", allowBlank: true, flex: 1,
                            margin: "0 0 0 5",
                            store: new Ext.data.SimpleStore({
                                fields: ['ReportGroup', 'ReportGroupName'],
                                data: ReportGroup_values
                            }),

                            valueField: 'ReportGroup',
                            hiddenName: 'ReportGroup',
                            displayField: 'ReportGroupName',
                            name: 'ReportGroup', itemId: "ReportGroup", id: "ReportGroup" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        {
                            xtype: 'numberfield', value: 0, minValue: 0, maxValue: 99999999999,
                            allowBlank: true, flex: 1, fieldLabel: "№ Док.", margin: "0 0 0 5",
                            name: 'DocXID', itemId: "DocXID", id: "DocXID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, //readOnly: true
                        },
                    ]
                },

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

                { xtype: "label", text: "К-во операций: " },
                { xtype: 'displayfield', name: "Quantity", id: "Quantity" + this.UO_id, readOnly: true, allowBlank: false, flex: 1, labelWidth: 100 },

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

            store: this.storeReportBanksCashOffices,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [
                { text: "DocID", dataIndex: "DocID", width: 50, hidden: true },
                { text: "№ док.", dataIndex: "DocXID", width: 60 },

                //{ text: "Дата", dataIndex: "DocCashOfficeSumDate", width: 120, hidden: true },
                //{ text: "Дата", dataIndex: "DocBankSumDate", width: 120, hidden: true },
                { text: "Дата", dataIndex: "DocCashOfficeBankSumDate", width: 120 },

                //{ text: "Тип операции", dataIndex: "DirCashOfficeSumTypeName", flex: 1, hidden: true },
                //{ text: "Тип операции", dataIndex: "DirBankSumTypeName", flex: 1, hidden: true },
                { text: "Тип операции", dataIndex: "DirCashOfficeBankSumTypeName", flex: 1 },

                //{ text: "Сумма", dataIndex: "DocCashOfficeSumSum", summaryType: 'sum', width: 100, hidden: true },
                //{ text: "Сумма", dataIndex: "DocBankSumSum", summaryType: 'sum', width: 100, hidden: true },
                { text: "Сумма", dataIndex: "DocCashOfficeBankSumSum", summaryType: 'sum', width: 100 },

                { text: lanFixedDiscount, dataIndex: "Discount", summaryType: 'sum', width: 100 },

                { text: "Оператор", dataIndex: "DirEmployeeName", width: 175 },
                { text: "Примечание", dataIndex: "Base", flex: 1 },

                //{ text: "Касса", dataIndex: "DirCashOfficeName", flex: 1, hidden: true },
                //{ text: "Банк", dataIndex: "DirBankName", flex: 1, hidden: true },
                { text: "Касса", dataIndex: "DirCashOfficeBankName", width: 110 },

                { text: "Точка", dataIndex: "DirWarehouseName", width: 110 },

                //{ text: "Валюта", dataIndex: "DirCurrencyName", width: 85, hidden: true },
                //{ text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 50, hidden: true },
                //{ text: "Курс", dataIndex: "DirCurrencyRate", width: 50, hidden: true },

                { text: "Чек№", dataIndex: "KKMSCheckNumber", width: 55 },

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