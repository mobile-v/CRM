Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandMovTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocSecondHandMovTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 675, height: 140,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: "Спецификация",
    autoHeight: true,

    frame: true,
    border: false,
    resizable: false,
    //modal: true,
    buttonAlign: 'left',

    timeout: varTimeOutDefault,
    waitMsg: lanLoading,

    UO_maximize: false, //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    //Контроллер
    controller: 'viewcontrollerDocSecondHandMovTabsEdit',

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

                // === Не видимые поля === === ===
                { xtype: 'textfield', fieldLabel: "DirReturnTypeID", name: 'DirReturnTypeID', id: 'DirReturnTypeID' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirReturnTypeName", name: 'DirReturnTypeName', id: 'DirReturnTypeName' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirDescriptionID", name: 'DirDescriptionID', id: 'DirDescriptionID' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirDescriptionName", name: 'DirDescriptionName', id: 'DirDescriptionName' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                //DirServiceNomen
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [

                        //Остаток: максимум сколько можно списать
                        /*{
                            xtype: 'viewTriggerDirField',
                            allowBlank: true,
                            name: 'Remnant', itemId: "Remnant", id: "Remnant" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },*/

                        {
                            xtype: 'textfield',
                            fieldLabel: "Документ", labelAlign: "top", emptyText: "...", allowBlank: false, width: 100,
                            name: 'DocSecondHandPurchID', itemId: "DocSecondHandPurchID", id: "DocSecondHandPurchID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true, hidden: true
                        },

                        //Товар

                        {
                            xtype: 'textfield',
                            fieldLabel: "Артикул", labelAlign: "top", emptyText: "...", allowBlank: false, width: 100,
                            name: 'DirServiceNomenID', itemId: "DirServiceNomenID", id: "DirServiceNomenID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true, hidden: true
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: "Имя товара", labelAlign: "top", emptyText: "...", allowBlank: false, flex: 1, margin: "0 0 0 5",
                            name: 'DirServiceNomenName', itemId: "DirServiceNomenName", id: "DirServiceNomenName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },

                        //К-во
                        /*{
                            xtype: 'numberfield',
                            //regex: /^[+\-]?\d+(?:\.\d+)?$/,
                            value: 1, minValue: 1, maxValue: 999999,
                            allowBlank: false, width: 125, fieldLabel: "<b>" + lanCount + "</b>", labelAlign: "top", margin: "0 0 0 5",
                            name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            listeners: { change: 'onQuantityChange' },
                        },*/

                         //Партия
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: false,
                            name: 'Rem2PartyID', itemId: "Rem2PartyID", id: "Rem2PartyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },

                //Валюта + Цена в Валюте - !!! Не видимое !!!
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor', hidden: true,
                    title: lanPurchase,
                    autoHeight: true,
                    items: [
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            //hidden: true,
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
                                    readOnly: true,
                                    listeners: { select: 'onDirCurrencyIDSelect' },
                                },
                                //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCurrencyEdit", id: "btnCurrencyEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { click: 'onBtnCurrencyEditClick' }, },
                                //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCurrencyReload", id: "btnCurrencyReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { click: 'onBtnCurrencyReloadClick' }, },
                                //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCurrencyClear", id: "btnCurrencyClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { click: 'onBtnCurrencyClearClick' }, },


                                //Приходная цена в валюте
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, margin: "0 0 0 10", fieldLabel: lanPriceInCurr, margin: "0 0 0 10",
                                    name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true,
                                    listeners: { select: 'onPriceVATChange' },
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            //hidden: true,
                            items: [
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс", 
                                    name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true,
                                    listeners: { change: 'onDirCurrencyRateChange' },
                                },
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", margin: "0 0 0 10",
                                    name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true,
                                    listeners: { change: 'onDirCurrencyMultiplicityChange' },
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            //hidden: true,
                            items: [
                                //К-во
                                /*{
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>" + lanCount + "</b>",
                                    name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },*/

                                //Приходная цена
                                {
                                    xtype: 'textfield', 
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>" + lanPrice + "</b>", margin: "0 0 0 10",
                                    name: 'PriceCurrency', itemId: 'PriceCurrency', id: 'PriceCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true,
                                    listeners: { change: 'onPriceCurrencyChange' },
                                },
                            ]
                        },
                    ]
                },
                                
                //Цены - Расход - !!! НЕ ВИДИМАЯ !!!
                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' }, hidden: true,
                    items: [

                        {
                            name: 'ConsumableWholesale',
                            title: lanRetail,
                            autoHeight: true,

                            xtype: 'fieldset', flex: 1, layout: 'anchor',

                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [

                                        /*{
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupRetail', itemId: 'MarkupRetail', id: 'MarkupRetail' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245,
                                            readOnly: true,
                                            listeners: { change: 'onMarkupRetailChange' },
                                        },*/
                                        { //lanPriceVat
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true,
                                            readOnly: true,
                                            listeners: { change: 'onPriceRetailVATChange' },
                                        },
                                        //CurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceRetailCurrency', itemId: 'PriceRetailCurrency', id: 'PriceRetailCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245,
                                            readOnly: true,
                                            listeners: { change: 'onPriceRetailCurrencyChange' },
                                        }

                                    ]
                                },

                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },
                        

                        {
                            name: 'ConsumableWholesale',
                            title: lanWholesale,
                            autoHeight: true,

                            xtype: 'fieldset', flex: 1, layout: 'anchor',

                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [

                                        /*{
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupWholesale', itemId: 'MarkupWholesale', id: 'MarkupWholesale' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245,
                                            readOnly: true,
                                            listeners: { change: 'onMarkupWholesaleChange' },
                                        },*/
                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceWholesaleVAT', itemId: 'PriceWholesaleVAT', id: 'PriceWholesaleVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true,
                                            readOnly: true,
                                            listeners: { change: 'onPriceWholesaleVATChange' },
                                        },
                                        //CurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceWholesaleCurrency', itemId: 'PriceWholesaleCurrency', id: 'PriceWholesaleCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245,
                                            readOnly: true,
                                            listeners: { change: 'onPriceWholesaleCurrencyChange' },
                                        }

                                    ]
                                },

                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                        {
                            name: 'ConsumableIM',
                            title: "Интернет-магазин",
                            autoHeight: true,

                            xtype: 'fieldset', flex: 1, layout: 'anchor',

                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [

                                        /*{
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupIM', itemId: 'MarkupIM', id: 'MarkupIM' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245,
                                            readOnly: true,
                                            listeners: { change: 'onMarkupIMChange' },
                                        },*/
                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceIMVAT', itemId: 'PriceIMVAT', id: 'PriceIMVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true,
                                            readOnly: true,
                                            listeners: { change: 'onPriceIMVATChange' },
                                        },
                                        //CurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceIMCurrency', itemId: 'PriceIMCurrency', id: 'PriceIMCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245,
                                            readOnly: true,
                                            listeners: { change: 'onPriceIMCurrencyChange' },
                                        }

                                    ]
                                },

                            ]
                        },

                    ]
                }

            ]
        });



        this.items = [

            formPanel

        ];


        this.buttons = [
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                text: lanSave, tooltip: lanSave, icon: '../Scripts/sklad/images/save.png',
                listeners: { click: 'onBtnSaveClick' },
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                text: lanCancel, tooltip: lanCancel, icon: '../Scripts/sklad/images/cancel.png',
                listeners: { click: 'onBtnCancelClick' },
            },

            "->",

            {
                id: "btnDel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDel",
                text: lanDelete, tooltip: lanDelete, icon: '../Scripts/sklad/images/table_delete.png',
                listeners: { click: 'onBtnDelClick' },
            },

        ],


        this.callParent(arguments);
    }

});