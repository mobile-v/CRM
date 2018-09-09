Ext.define("PartionnyAccount.view.Sklad/Object/Report/viewReportSalariesWarehouses", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewReportSalariesWarehouses",

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
                            fieldLabel: "Точка", flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 25",
                            store: this.storeDirWarehousesGrid, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnDirWarehousesClear", id: "btnDirWarehousesClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },


                //{ xtype: 'container', height: 5 },

                //{ xtype: "label", text: "Внимание: Суммы ЗП за месяц считается не зависимо от выбранной даты!", style: 'color: red; font-weight: bold;' }
            ],

            buttons: [
                /*{
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
                " ",*/

                {
                    id: "btnDocMovementsEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDocMovementsEdit",
                    text: "Сформировать перемещение", icon: '../Scripts/sklad/images/doc_of.png', hidden: true
                },
                " ",
                {
                    id: "btnReport" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnReport",
                    text: lanReport, icon: '../Scripts/sklad/images/reports.png', UO_Action: "report",
                },
                " ",
                {
                    id: "btnCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png', UO_Action: "cancel",
                },

                "->",

                /*
                { xtype: 'displayfield', fieldLabel: '<b>Магазин</b>', name: "Sum1_", id: "Sum1_" + this.UO_id, readOnly: true, allowBlank: false, width: 140, labelWidth: 85 },
                { xtype: 'displayfield', fieldLabel: '<b>СЦ работа</b>', name: "Sum21_", id: "Sum21_" + this.UO_id, readOnly: true, allowBlank: false, width: 140, labelWidth: 85 },
                { xtype: 'displayfield', fieldLabel: '<b>СЦ запчасти</b>', name: "Sum22_", id: "Sum22_" + this.UO_id, readOnly: true, allowBlank: false, width: 140, labelWidth: 85 },
                { xtype: 'displayfield', fieldLabel: '<b>БУ</b>', name: "Sum3_", id: "Sum3_" + this.UO_id, readOnly: true, allowBlank: false, width: 140, labelWidth: 85 },
                */
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

            store: this.storeReportSalariesWarehouses,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            
            columns: [
                //0. Дата (ПРИБЫЛЬ)
                { text: "Дата", dataIndex: "DocDate", width: 75, style: "height: 25px;" },
                //1,2
                {
                    text: "Торговля",
                    //style: "width: 300px",
                    columns: [
                        //все продажи  за наличку
                        { text: "Нал", dataIndex: "TradeCash", width: 75, summaryType: 'sum' },
                        //все продажи  за без наличку
                        { text: "Безнал", dataIndex: "TradeBank", width: 75, summaryType: 'sum' },
                    ],
                },
                //3,4
                {
                    text: "Сервисный цент",
                    //style: "width: 300px",
                    columns: [
                        //все продажи  за наличку
                        { text: "Нал", dataIndex: "ServiceCash", width: 75, summaryType: 'sum' },
                        //все продажи  за без наличку
                        { text: "Безнал", dataIndex: "ServiceBank", width: 75, summaryType: 'sum' },
                    ],
                },
                //5,6
                {
                    text: "Б/У",
                    //style: "width: 300px",
                    columns: [
                        //все продажи  за наличку
                        { text: "Нал", dataIndex: "SecondCash", width: 75, summaryType: 'sum' },
                        //все продажи  за без наличку
                        { text: "Безнал", dataIndex: "SecondBank", width: 75, summaryType: 'sum' },
                    ],
                },

                //7. Сумма: Нал + Безнал
                { text: "Касса", dataIndex: "TradeSumCashBank", width: 75, summaryType: 'sum' },


                //8. Цвет
                { text: "X", dataIndex: "X2", width: 10, tdCls: 'x-change-cell' },


                //9. Общий расход (суммирует все расходы)
                //{ text: "общий р-д.", dataIndex: "X1", flex: 1, style: "height: 25px;" },
                //ТОВ.СЕБЕСТ. - Сумма прихода
                { text: "Тов.Себест.", dataIndex: "PurchesSum", flex: 1, style: "height: 25px;", summaryType: 'sum' },

                //10, 11
                {
                    text: "Торг.Закупки",
                    //style: "width: 300px",
                    columns: [
                        //все продажи  за наличку
                        { text: "Нал", dataIndex: "DocPurchesCashSum", width: 75, summaryType: 'sum' },
                        //все продажи  за без наличку
                        { text: "Безнал", dataIndex: "DocPurchesBankSum", width: 75, summaryType: 'sum' },
                    ],
                },
                /*//ЗАКУП.НАЛ - сумма прихода проданного товара (чисто информативное поле, сейчас все точки сами себе делают закупку)
                { text: "ЗАКУП.НАЛ", dataIndex: "DocPurchesCashSum", flex: 1, style: "height: 25px;" },
                //ЗАКУП.БЕЗНАЛ - сумма прихода проданного товара (чисто информативное поле, сейчас все точки сами себе делают закупку)
                { text: "ЗАКУП.БЕЗНАЛ", dataIndex: "DocPurchesBankSum", flex: 1, style: "height: 25px;" },*/

                //12, 13
                {
                    text: "Б/У Закупки",
                    //style: "width: 300px",
                    columns: [
                        //все продажи  за наличку
                        { text: "Нал", dataIndex: "SecondCashPurch", width: 75, summaryType: 'sum' },
                        //все продажи  за без наличку
                        { text: "Безнал", dataIndex: "SecondBankPurch", width: 75, summaryType: 'sum' },
                    ],
                },



                {
                    text: "Выплаты", //Хоз.расходы
                    //style: "width: 300px",
                    columns: [
                        //ДР.РАСХОДЫ - курьер, хоз расчёты (DomesticExpenses == хоз расчёты)
                        { text: "Друг.", dataIndex: "DomesticExpenses", width: 75, summaryType: 'sum' },
                        //Выд.ЗП - выданная ЗП в середине месяца
                        { text: "Выд.ЗП", dataIndex: "DomesticExpensesSalary", width: 75, summaryType: 'sum' },
                        //ИНКАСС.
                        { text: "Инкас.", dataIndex: "Encashment", width: 75, summaryType: 'sum' },
                    ],
                },

                /*//Хоз.расходы
                //ДР.РАСХОДЫ - курьер, хоз расчёты (DomesticExpenses == хоз расчёты)
                { text: "Хоз.расчёты", dataIndex: "DomesticExpenses", flex: 1, style: "height: 25px;" },
                //Выд.ЗП - выданная ЗП в середине месяца
                { text: "Выд.ЗП", dataIndex: "DomesticExpensesSalary", flex: 1, style: "height: 25px;" },
                //ИНКАСС.
                { text: "ИНКАСС.", dataIndex: "Encashment", flex: 1, style: "height: 25px;" },*/



                //Инвентаризация: Сумма цена розницы на списание - Сумма цены розницы на поступление
                { text: "Инвентар.", dataIndex: "InventorySum1", width: 125, summaryType: 'sum' },


                //Цвет
                { text: "X", dataIndex: "X3", width: 10, tdCls: 'x-change-cell' },


                //Инвентаризация: Сумма цена розницы на списание - Сумма цены розницы на поступление
                { text: "ЗП Торговля", dataIndex: "sumSalaryPercentTrade", width: 115, summaryType: 'sum' },
                { text: "ЗП СЦ работы", dataIndex: "sumSalaryPercentService1Tabs", width: 115, summaryType: 'sum' }, { text: "СЦ К-во", dataIndex: "sumSalaryPercentService1TabsCount", width: 115, summaryType: 'sum' },
                { text: "ЗП СЦ запчасти", dataIndex: "sumSalaryPercentService2Tabs", width: 115, summaryType: 'sum' },

                { text: "ЗП БУ (Инвентаризация)", dataIndex: "sumSecondHandInventory", width: 115, summaryType: 'sum' },
                { text: "ЗП БУ (Фикс с каждого проданной единицы)", dataIndex: "sumSalaryPercent2Second", width: 115, summaryType: 'sum' },
                { text: "ЗП БУ (Фикс за отремонтированную единицу)", dataIndex: "sumSalaryPercent3Second", width: 115, summaryType: 'sum' },
                { text: "ЗП БУ", dataIndex: "sumSalaryPercentSecond", width: 115, summaryType: 'sum' },

                { text: "ЗП Итого", dataIndex: "SalatyProc", width: 115, summaryType: 'sum' },
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