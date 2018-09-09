Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandReturnTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocSecondHandReturnTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 625, height: 285,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: "Возврат",
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
    controller: 'viewcontrollerDocSecondHandReturnTabsEdit',

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
                { xtype: 'viewDateField', fieldLabel: "Дата", name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", allowBlank: false, readOnly: true, hidden: true },

                //{ xtype: 'textfield', fieldLabel: "№", name: "Rem2PartyMinusID", id: "Rem2PartyMinusID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "№", name: "DocSecondHandReturnID", id: "DocSecondHandReturnID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "№", name: "DocSecondHandSaleID", id: "DocSecondHandSaleID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: "checkbox", boxLabel: lanReserve, margin: "0 0 0 5", name: "Reserve", itemId: "Reserve", flex: 1, id: "Reserve" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirVatValue", name: "DirVatValue", id: "DirVatValue" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                //Номер Чека ККМ
                { xtype: 'textfield', fieldLabel: "KKMSCheckNumber", name: 'KKMSCheckNumber', itemId: "KKMSCheckNumber", id: "KKMSCheckNumber" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "KKMSIdCommand", name: 'KKMSIdCommand', itemId: "KKMSIdCommand", id: "KKMSIdCommand" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true },


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


                //Эта форма *** *** *** *** ***

                //Тип цены - передаётся из вьюхи "viewDocSalesEdit"
                { xtype: 'viewTriggerDirField', name: 'DirPriceTypeID', itemId: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true },
                //Остаток: максимум сколько можно списать
                //{ xtype: 'viewTriggerDirField', name: 'Remnant', itemId: "Remnant", id: "Remnant" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true },
                //Партия
                //{ xtype: 'viewTriggerDirField', name: 'Rem2PartyID', itemId: "Rem2PartyID", id: "Rem2PartyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: false, },

                // *** *** *** Not Visible *** *** *** *** *** *** *** ***



                //ID-шники - some (некоторых) not visible
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [

                        //Документ
                        {
                            xtype: 'textfield',
                            fieldLabel: "Документ", emptyText: "...", allowBlank: false, flex: 1, readOnly: true,
                            name: 'DocSecondHandPurchID', itemId: "DocSecondHandPurchID", id: "DocSecondHandPurchID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },

                        //Товар
                        {
                            xtype: 'textfield',
                            fieldLabel: "Артикул", emptyText: "...", allowBlank: false, flex: 1, readOnly: true,
                            name: 'DirServiceNomenID', itemId: "DirServiceNomenID", id: "DirServiceNomenID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: "Имя товара", emptyText: "...", allowBlank: false, flex: 2, readOnly: true, margin: "0 0 0 10",
                            name: 'DirServiceNomenName', itemId: "DirServiceNomenName", id: "DirServiceNomenName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },

                { xtype: 'container', height: 15 },

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
                                    fieldLabel: lanCurrency, flex: 2, allowBlank: true,

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


                                //Приходная цена в валюте
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: lanPriceInCurr, margin: "0 0 0 10",
                                    name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },
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


                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                    ]
                },

                { xtype: 'container', height: 5 },

                //Quantity + Price
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [
                        //К-во
                        /*{
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>" + lanCount + "</b>",
                            name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },*/
                        {
                            xtype: 'numberfield',
                            value: 1, maxValue: 999999, minValue: 1,
                            allowBlank: false, flex: 1, fieldLabel: "<b>" + lanCount + "</b>",
                            name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },

                        //Приходная цена
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: lanPrice, readOnly: true, margin: "0 0 0 10",
                            name: 'PriceCurrency', itemId: 'PriceCurrency', id: 'PriceCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                        //Скидка
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>Скидка</b>", readOnly: false, margin: "0 0 0 10",
                            name: 'Discount', itemId: 'Discount', id: 'Discount' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                            listeners: { change: 'onChangeDiscount' },
                        },
                    ]
                },


                { xtype: 'container', height: 5 },


                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Возврат",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                //{ xtype: 'textfield', fieldLabel: lanDisc, name: "Description", id: "Description" + this.UO_id, flex: 1, allowBlank: true }
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Тип возврата", flex: 1, allowBlank: false,
                                    store: this.storeDirReturnTypesGrid, // store getting items from server
                                    valueField: 'DirReturnTypeID',
                                    hiddenName: 'DirReturnTypeID',
                                    displayField: 'DirReturnTypeName',
                                    name: 'DirReturnTypeID', itemId: "DirReturnTypeID", id: "DirReturnTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    //editable: true, typeAhead: true, minChars: 2
                                },

                                { xtype: 'container', height: 5 },

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Причина", flex: 1, allowBlank: true,
                                    margin: "0 0 0 10",
                                    store: this.storeDirDescriptionsGrid, // store getting items from server
                                    valueField: 'DirDescriptionName',
                                    //hiddenName: 'DirDescriptionName',
                                    displayField: 'DirDescriptionName',
                                    name: 'Description', itemId: "Description", id: "Description" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    editable: true, typeAhead: true, minChars: 2
                                },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },


                { xtype: 'container', height: 5 },

                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "ККМ",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                //Phone
                                {
                                    xtype: 'textfield',
                                    allowBlank: true, flex: 1, fieldLabel: "Phone",
                                    name: 'KKMSPhone', itemId: 'KKMSPhone', id: 'KKMSPhone' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },

                                //EMail
                                {
                                    xtype: 'textfield',
                                    allowBlank: true, flex: 1, fieldLabel: "EMail", vtype: "email", margin: "0 0 0 10",
                                    name: 'KKMSEMail', itemId: 'KKMSEMail', id: 'KKMSEMail' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

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

            store: this.storeGrid, //storeDocSecondHandSaleTabsGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "Rem2PartyID", width: 75, hidden: true },
                { text: "Партия", dataIndex: "Rem2PartyMinusID", width: 75, hidden: true },
                //Товар
                { text: "№", dataIndex: "DirServiceNomenID", width: 50, style: "height: 25px;" },
                { text: lanNomenclature, dataIndex: "DirServiceNomenName", flex: 1 }, //flex: 1
                //К-во
                { text: lanCount, dataIndex: "Quantity", width: 75 },
                //Суммы
                { text: "Тип цены", dataIndex: "DirPriceTypeName", width: 75, hidden: true },
                { text: lanPriceSale, dataIndex: "PriceCurrency", width: 100 },
                { text: lanSum, dataIndex: "SUMSalePriceVATCurrency", width: 100 },

                //Характеристики
                /*
                { text: "Характеристики", dataIndex: "DirChar", flex: 1 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 70, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 70, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 70, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 70, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 70, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 70, hidden: true },
                { text: "Поставщик", dataIndex: "DirCharStyleName", width: 70, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 70, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 70, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 70, hidden: true },
                */
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