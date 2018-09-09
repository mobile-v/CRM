Ext.define("PartionnyAccount.view.Sklad/Object/Report/viewReportTotalTrade", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewReportTotalTrade",

    layout: "border",
    region: "center",
    title: "Отчет по Торговле",
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
            [1, 'Проданный товар (Опт + Розница)'],
            [2, 'Приходы'],
            [3, 'Товар в наличии'],
            [4, 'Возвраты (Опт + Розница)'],
            [5, 'Заканчивающийся товар'],
            [6, 'Списанный товар'],
            [7, 'Товар отсутствует'],
            [8, 'Брак'],
        ];
        var DirPriceType_values = [
            [1, 'Показывать приходную цену и прибыль'],
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

            width: "100%", height: 95,
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
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouse, flex: 1, allowBlank: true, //, emptyText: "..."
                            store: this.storeDirWarehousesGrid, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnDirWarehousesClear", id: "btnDirWarehousesClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Цены", allowBlank: true, flex: 1,
                            margin: "0 0 0 25",
                            store: new Ext.data.SimpleStore({
                                fields: ['DirPriceTypeID', 'DirPriceTypeName'],
                                data: DirPriceType_values
                            }),

                            valueField: 'DirPriceTypeID',
                            hiddenName: 'DirPriceTypeID',
                            displayField: 'DirPriceTypeName',
                            name: 'DirPriceTypeID', itemId: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Тип отчета", allowBlank: true, flex: 1,
                            margin: "0 0 0 25",
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

            store: this.storeReportTotalTrade,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [
                { text: "DocID", dataIndex: "DocID", width: 50, hidden: true },
                { text: "ListObjectNameRu", dataIndex: "ListObjectNameRu", width: 50, hidden: true },
                { text: "RemPartyID", dataIndex: "RemPartyID", width: 50, hidden: true },

                //Код товара
                { text: "Код", dataIndex: "DirNomenID", width: 50, hidden: true },
                //Товар Наименование
                { text: "Товар", dataIndex: "DirNomenName", flex: 1, hidden: true },
                //Цена закупки
                { text: "Закупка", dataIndex: "PriceCurrency", width: 85, hidden: true },
                //Сумма закупки
                //{ text: "Сумма закупки", dataIndex: "Purch_Sums", width: 85, hidden: true },
                //Цена продажи: Розница
                { text: "Розница", dataIndex: "PriceRetailVAT", width: 85, hidden: true },
                //Цена продажи: Розница
                { text: "Розница", dataIndex: "PriceRetailCurrency", summaryType: 'sum', width: 85, hidden: true },
                //Цена продажи: Опт
                { text: "Опт", dataIndex: "PriceWholesaleVAT", width: 85, hidden: true },
                { text: "Опт", dataIndex: "PriceWholesaleCurrency", width: 85, hidden: true },
                //Цена продажи: И-М
                { text: "И-М", dataIndex: "PriceIMVAT", width: 85, hidden: true },
                //Сумма Пришло
                { text: "Сумма Закупки", dataIndex: "SumQuantity", summaryType: 'sum', width: 85, hidden: true },
                //Цена закупки
                { text: "Цена закупки", dataIndex: "Purch_PriceCurrency", width: 85, hidden: true },
                //Цена продажи
                { text: "Цена продажи", dataIndex: "Sale_PriceCurrency", width: 85, hidden: true },
                //К-во
                { text: "К-во", dataIndex: "Sale_Quantity", summaryType: 'sum', width: 85, hidden: true },
                //Сумма
                { text: "Сумма", dataIndex: "Sums", summaryType: 'sum', width: 85, hidden: true },
                //Прибыль
                { text: "Прибыль", dataIndex: "SumProfit", summaryType: 'sum', width: 85, hidden: true },
                //Скидка
                { text: "Скидка", dataIndex: "Sale_Discount", width: 85, hidden: true },
                //К-во
                { text: "К-во", dataIndex: "Quantity", summaryType: 'sum', width: 85, hidden: true },
                //Остаток
                { text: "Остаток", dataIndex: "Remnant", summaryType: 'sum', width: 85, hidden: true },
                //Сумма Остатка
                { text: "Сумма Остатка", dataIndex: "SumRemnant", summaryType: 'sum', width: 85, hidden: true },
                //Сумма Остатка
                { text: "Минимальный остаток", dataIndex: "DirNomenMinimumBalance", width: 85, hidden: true },
                //Продавец
                { text: "Продавец", dataIndex: "DirEmployeeName", width: 85, hidden: true },
                //Дата
                { text: "Дата", dataIndex: "DocDate", width: 125, hidden: true },
                //Точка
                { text: "Точка", dataIndex: "DirWarehouseName", width: 85, hidden: true },
                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", width: 85, hidden: true },

                //Тип + Причина
                { text: "Тип", dataIndex: "DirReturnTypeName", width: 85, hidden: true },
                { text: "Причина", dataIndex: "DirDescriptionName", width: 85, hidden: true },

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