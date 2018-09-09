Ext.define("PartionnyAccount.view.Sklad/Object/Report/viewReportSalaries", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewReportSalaries",

    layout: "border",
    region: "center",
    title: "Отчет по Зарплате",
    width: 650, height: 240,
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
            [1, 'по дате выполнения работы'],
            [2, 'по дате оплаты за работу'],
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

            width: "100%", height: 75,
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [


                //Не видимые!!!
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
                    hidden: true
                },
               
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'viewDateField', fieldLabel: "С", name: "DateS", id: "DateS" + this.UO_id, allowBlank: false },
                        { xtype: 'viewDateField', fieldLabel: "по", name: "DatePo", id: "DatePo" + this.UO_id, margin: "0 0 0 25", allowBlank: false },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanEmployee, flex: 1, allowBlank: true, //, emptyText: "..."
                            margin: "0 0 0 25",
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
                            fieldLabel: "Мастерская, работы", labelWidth: 150, allowBlank: true, flex: 1,
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


                //{ xtype: 'container', height: 5 },

                //{ xtype: "label", text: "Внимание: Суммы ЗП за месяц считается не зависимо от выбранной даты!", style: 'color: red; font-weight: bold;' }
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


                "->",

                {
                    id: "btnDocMovementsEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDocMovementsEdit",
                    text: "Сформировать перемещение", icon: '../Scripts/sklad/images/doc_of.png', hidden: true
                },
                " ",
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

            store: this.storeReportSalaries,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],

            columns: [


                //Если выбран конкретный сотрудник, то в разрезе по дням
                
                //Сотрудник
                { text: "№", dataIndex: "DirEmployeeID", width: 50, style: "height: 25px;", hidden: true },
                { text: "Сотрудник", dataIndex: "DirEmployeeName", flex: 1 },

                //Дата (ПРИБЫЛЬ)
                { text: "Дата", dataIndex: "DocDate", id: "DocDate" + this.UO_id, width: 75, style: "height: 25px;" },



                //Валюта
                //{ text: "Валюта", dataIndex: "DirCurrencyID", flex: 1, hidden: true },
                { text: "Валюта", dataIndex: "DirCurrencyName", flex: 1, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 75, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 75, hidden: true },


                //Цвет
                { text: "X", dataIndex: "X1", width: 10, tdCls: 'x-change-cell' },


                //ЗП
                {
                    text: "Зарплата",
                    //style: "width: 300px",
                    columns: [
                        { text: "Зарплата", dataIndex: "Salary", width: 65, summaryType: 'sum' },
                        { text: "Тип", dataIndex: "SalaryDayMonthlyName", width: 75 },
                        { text: "Дней", dataIndex: "CountDay", width: 65, summaryType: 'sum' },
                        { text: "Сумма", dataIndex: "SumSalary", width: 75, summaryType: 'sum' },
                        { text: "Сумма фикс.", dataIndex: "SalaryFixedSalesMount", width: 85, summaryType: 'sum' },
                    ],
                },


                //Цвет
                { text: "X", dataIndex: "X2", width: 10, tdCls: 'x-change-cell' },


                //Т: Премия-1
                {
                    text: "Торг.премия",
                    //style: "width: 300px",
                    columns: [
                        { text: "Премия (продавца)", dataIndex: "DirBonusName", flex: 1 },
                        { text: "Сумма", dataIndex: "DirBonusIDSalary", flex: 1, summaryType: 'sum' },
                    ],
                },


                //Цвет
                { text: "X", dataIndex: "X3", width: 10, tdCls: 'x-change-cell' },


                //СЦ: Премия-2
                {
                    text: "СЦ.премия",
                    //style: "width: 300px",
                    columns: [
                        /*
                        { text: "Премия (СЦ)", dataIndex: "DirBonus2Name", flex: 1 },
                        { text: "Сумма", dataIndex: "DirBonus2IDSalary", flex: 1, summaryType: 'sum' },
                        { text: "За ремонт", dataIndex: "SumSalaryFixedServiceOne", flex: 1, summaryType: 'sum' },
                        */
                        { text: "ЗП СЦ работы", dataIndex: "sumSalaryPercentService1Tabs", width: 115, summaryType: 'sum' }, { text: "СЦ К-во", dataIndex: "sumSalaryPercentService1TabsCount", width: 115, summaryType: 'sum' },
                        { text: "ЗП СЦ запчасти", dataIndex: "sumSalaryPercentService2Tabs", width: 115, summaryType: 'sum' },
                    ],
                },


                //Цвет
                { text: "X", dataIndex: "X3", width: 10, tdCls: 'x-change-cell' },


                //Б/У
                {
                    text: "Б/У",
                    //style: "width: 300px",
                    columns: [

                        {
                            text: "Маст.премия",
                            //style: "width: 300px",
                            columns: [
                                { text: "Премия (маст)", dataIndex: "DirBonus3Name", flex: 1 },
                                { text: "Сумма", dataIndex: "DirBonus3IDSalary", flex: 1, summaryType: 'sum' },
                                { text: "За ремонт", dataIndex: "SumSalaryFixedSecondHandWorkshopOne", flex: 1, summaryType: 'sum' },
                            ],
                        },

                        { text: "X", dataIndex: "X4", width: 10, tdCls: 'x-change-cell-posr' },


                        {
                            text: "Торг.премия",
                            //style: "width: 300px",
                            columns: [
                                { text: "Премия (продавца)", dataIndex: "DirBonus4Name", flex: 1 },
                                { text: "Сумма", dataIndex: "DirBonus4IDSalary", flex: 1, summaryType: 'sum' },
                                { text: "За прод", dataIndex: "SumSalaryFixedSecondHandRetailOne", flex: 1, summaryType: 'sum' },
                            ],
                        },
                    ],
                },


                //Цвет
                { text: "X", dataIndex: "X3", width: 10, tdCls: 'x-change-cell' },


                //Хоз.расходы (выплаченые ЗП сотруднику)
                {
                    text: "Хоз.расходы",
                    //style: "width: 300px",
                    columns: [
                        //Выд.ЗП - выданная ЗП в середине месяца
                        { text: "Выд.ЗП", dataIndex: "DomesticExpensesSalary", width: 75, summaryType: 'sum' },
                    ],
                },


                { text: "ЗП БУ (Инвентаризация)", dataIndex: "sumSecondHandInventory", width: 115, summaryType: 'sum' },

                //Цвет
                { text: "X", dataIndex: "X4", width: 10, tdCls: 'x-change-cell' },

                //Сумма
                { text: "Общая", dataIndex: "Sums", width: 100, summaryType: 'sum' },
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
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
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