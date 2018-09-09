Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocDomesticExpenses/viewDocDomesticExpenseTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocDomesticExpenseTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 550, height: 220,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: "Списание средств",
    autoHeight: true,
    frame: true,
    border: false,
    resizable: false,
    //modal: true,
    buttonAlign: 'left',
    timeout: varTimeOutDefault,
    waitMsg: lanLoading,
    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    UO_maximize: false, //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом

    //Контроллер
    controller: 'viewcontrollerDocDomesticExpenseTabsEdit',

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

            UO_GridServerParam1: this.UO_GridServerParam1, //Параметры для Грида, например передать склад, что бы показать поле остаток!


            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%",
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                // *** *** *** Not Visible *** *** *** *** *** *** *** ***

                //Родительская форма *** *** *** *** ***

                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                //{ xtype: 'viewDateField', fieldLabel: "Дата", name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", allowBlank: false, readOnly: true, hidden: true },

                //NumberReal == DocDomesticExpenseID
                //{ xtype: 'textfield', fieldLabel: "№", name: "NumberReal", id: "NumberReal" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "№", name: "DocDomesticExpenseID", id: "DocDomesticExpenseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirVatValue", name: "DirVatValue", id: "DirVatValue" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

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


                // *** *** *** Not Visible *** *** *** *** *** *** *** ***



                //Статья
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [

                        { xtype: 'viewDateField', fieldLabel: lanDate, labelAlign: "top", name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", allowBlank: false, readOnly: true },

                        //Статья
                        /*
                        {
                            xtype: 'textfield',
                            fieldLabel: "Код", labelAlign: "top", emptyText: "...", allowBlank: false, flex: 1, readOnly: true, margin: "0 0 0 10",
                            name: 'DirDomesticExpenseID', itemId: "DirDomesticExpenseID", id: "DirDomesticExpenseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: "Статья", labelAlign: "top", labelWidth: 75, emptyText: "...", allowBlank: false, flex: 2, readOnly: true, 
                            name: 'DirDomesticExpenseName', itemId: "DirDomesticExpenseName", id: "DirDomesticExpenseName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                        */
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Хоз.расходы", labelAlign: "top", labelWidth: 75, flex: 2, allowBlank: false,

                            store: this.storeDirDomesticExpensesGrid, // store getting items from server
                            valueField: 'DirDomesticExpenseID',
                            hiddenName: 'DirDomesticExpenseID',
                            displayField: 'DirDomesticExpenseName',
                            name: 'DirDomesticExpenseID', itemId: "DirDomesticExpenseID", id: "DirDomesticExpenseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                        },


                        //Приходная цена в валюте
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Сумма", labelAlign: "top", labelWidth: 75, margin: "0 0 0 10",
                            name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        //Приходная цена
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: true, flex: 1, fieldLabel: "Сумма", labelAlign: "top", labelWidth: 75, readOnly: true, margin: "0 0 0 10",
                            name: 'PriceCurrency', itemId: 'PriceCurrency', id: 'PriceCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },

                    ]
                },

                { xtype: 'container', height: 5 },

                //Валюта - not visilbe
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Информация",
                    autoHeight: true,
                    hidden: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: lanCurrency, labelWidth: 75, flex: 2, allowBlank: true,

                                    store: this.storeDirCurrenciesGrid, // store getting items from server
                                    valueField: 'DirCurrencyID',
                                    hiddenName: 'DirCurrencyID',
                                    displayField: 'DirCurrencyName',
                                    name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    //editable: true, typeAhead: true, minChars: 2
                                },
                                { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCurrencyEdit", id: "btnCurrencyEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCurrencyReload", id: "btnCurrencyReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCurrencyClear", id: "btnCurrencyClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                            ]
                        },


                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },


                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс",
                                    name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true
                                },
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", margin: "0 0 0 10",
                                    name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true
                                },
                            ]
                        },


                    ]
                },

                { xtype: 'container', height: 5 },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        /*
                        {
                            xtype: 'textfield',
                            allowBlank: false, width: 200, fieldLabel: "DirEmployeeID", labelAlign: "top", labelWidth: 75, readOnly: true, hidden: true,
                            name: 'DirEmployeeID', itemId: 'DirEmployeeID', id: 'DirEmployeeID' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: false, flex: 1, fieldLabel: lanEmployee, labelAlign: "top", labelWidth: 75, readOnly: true,
                            name: 'DirEmployeeName', itemId: 'DirEmployeeName', id: 'DirEmployeeName' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        */

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Сотрудник", labelAlign: "top", flex: 1, allowBlank: true, //disabled: true, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirEmployeesGrid, // store getting items from server
                            valueField: 'DirEmployeeID',
                            hiddenName: 'DirEmployeeID',
                            displayField: 'DirEmployeeName',
                            name: 'DirEmployeeIDSpisat', itemId: "DirEmployeeIDSpisat", id: "DirEmployeeIDSpisat" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },

                        {
                            xtype: 'textfield',
                            allowBlank: false, width: 200, fieldLabel: "Ваш пароль", labelAlign: "top", labelWidth: 75, margin: "0 0 0 10", //inputType: 'password',
                            name: 'DirEmployeePswd', itemId: 'DirEmployeePswd', id: 'DirEmployeePswd' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                    ]
                },

            ],

        });


        //В этот грид пишем выбранную позицию товара, при нажатии на кнопку "Расчет"
        //Что бы на сервере не переписывать код
        var PanelGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            //bodyStyle: 'background:transparent;',
            itemId: "grid",
            //listeners: { selectionchange: 'onGrid_selectionchange', edit: 'onGrid_edit' }, //, itemclick: 'onGrid_itemclick', itemdblclick: 'onGrid_itemdblclick'

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeGrid, //storeDocDomesticExpenseTabsGrid,

            columns: [
                //Товар
                { text: "№", dataIndex: "DirDomesticExpenseID", width: 50, style: "height: 25px;" },
                { text: "Статья", dataIndex: "DirDomesticExpenseName", flex: 1 }, //flex: 1
                //Суммы
                { text: lanPriceSale, dataIndex: "PriceCurrency", width: 100 },
            ]
        });


        this.items = [

            formPanel

        ];


        this.buttons = [
                {
                    xtype: "checkbox",
                    boxLabel: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanOnCredit + "</b></font>", labelAlign: 'top', inputValue: true, name: "OnCredit", id: "OnCredit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, style: "width: 120px; height: 40px;",
                    hidden: true
                },

                {
                    id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", style: "width: 240px; height: 40px;", hidden: true,
                    UO_Action: "held_cancel", hidden: true,
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanHeldCancel + "</b></font>", icon: '../Scripts/sklad/images/save_held.png',
                    listeners: { click: 'onBtnHeldCancelClick' }
                },


                "->",

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", style: "width: 120px; height: 40px;", hidden: true,
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanCancel + "</b></font>", icon: '../Scripts/sklad/images/cancel.png',
                    listeners: { click: 'onBtnCancelClick' }
                },


                //Расчет - не работает
                {
                    id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", style: "width: 120px; height: 40px;", hidden: true,
                    UO_Action: "held",
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanCalculates + "</b></font>", icon: '../Scripts/sklad/images/save_held.png',
                    listeners: { click: 'onBtnHeldsClick' }
                },
                /*
                //Наличная продажа
                {
                    id: "btnHelds1" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds1", style: "width: 130px; height: 40px;", hidden: true,
                    UO_Action: "held",
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + "Наличная" + "</b></font>", icon: '../Scripts/sklad/images/save_held.png',
                    listeners: { click: 'onBtnHelds1Click' }
                },
                //Безналичная продажа
                {
                    id: "btnHelds2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds2", style: "width: 150px; height: 40px;", hidden: true,
                    UO_Action: "held",
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + "Безналичная" + "</b></font>", icon: '../Scripts/sklad/images/save_held.png',
                    listeners: { click: 'onBtnHelds2Click' }
                },
                */


                " ",
                {
                    id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint", style: "width: 120px; height: 40px;", hidden: true,
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanPrint + "</b></font>", icon: '../Scripts/sklad/images/print.png',
                    menu:
                    [
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintHtml",
                            UO_Action: "html",
                            text: "Html", icon: '../Scripts/sklad/images/html.png',
                            listeners: { click: 'onBtnPrintHtmlClick' }
                        },
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintExcel",
                            UO_Action: "excel",
                            text: "MS Excel", icon: '../Scripts/sklad/images/excel.png',
                            listeners: { click: 'onBtnPrintHtmlClick' }
                        }
                    ]
                },
                //"-",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp", style: "width: 120px; height: 40px;", hidden: true,
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanHelp + "</b></font>", icon: '../Scripts/sklad/images/help16.png',
                    listeners: { click: 'onBtnHelpClick' }
                }
        ],


        this.callParent(arguments);
    }

});