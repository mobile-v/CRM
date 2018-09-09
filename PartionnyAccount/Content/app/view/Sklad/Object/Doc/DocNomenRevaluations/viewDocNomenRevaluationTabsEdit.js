Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocNomenRevaluations/viewDocNomenRevaluationTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocNomenRevaluationTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 750, height: 230,
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
            //autoScroll: true,

            items: [

                //DirNomen
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [
                        //Остаток: максимум сколько можно списать
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: true,
                            name: 'Remnant', itemId: "Remnant", id: "Remnant" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        //Товар
                        {
                            xtype: 'textfield',
                            fieldLabel: "Имя товара", emptyText: "...", allowBlank: false, flex: 1,
                            name: 'DirNomenName', itemId: "DirNomenName", id: "DirNomenName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: false,
                            name: 'DirNomenID', itemId: "DirNomenID", id: "DirNomenID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                        //Партия
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: false,
                            name: 'RemPartyID', itemId: "RemPartyID", id: "RemPartyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    hidden: true,
                    items: [
                        //Валюты
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
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс",
                            name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", margin: "0 0 0 10",
                            name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        //Приходная цена в валюте
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, margin: "0 0 0 10", fieldLabel: lanPriceInCurr, margin: "0 0 0 10",
                            name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },



                { xtype: 'container', height: 15 },

                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' },
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

                                        {
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupRetail', itemId: 'MarkupRetail', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'MarkupRetail' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },

                                        //lanPriceVat
                                        { 
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true
                                        },
                                        //CurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceRetailCurrency', itemId: 'PriceRetailCurrency', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },

                                        //lanPriceVat_OLD
                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr + " (старая)", name: 'PriceRetailVAT_OLD', itemId: 'PriceRetailVAT_OLD', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailVAT_OLD' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true, readOnly: true
                                        },
                                        //CurrentExchange_OLD
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice + " (старая)", name: 'PriceRetailCurrency_OLD', itemId: 'PriceRetailCurrency_OLD', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailCurrency_OLD' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, readOnly: true
                                        }

                                    ]
                                },

                            ]
                        },

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

                                        {
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupWholesale', itemId: 'MarkupWholesale', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'MarkupWholesale' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },

                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceWholesaleVAT', itemId: 'PriceWholesaleVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceWholesaleVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true
                                        },
                                        //lanPriceCurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceWholesaleCurrency', itemId: 'PriceWholesaleCurrency', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceWholesaleCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },

                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr + " (старая)", name: 'PriceWholesaleVAT_OLD', itemId: 'PriceWholesaleVAT_OLD', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceWholesaleVAT_OLD' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true, readOnly: true
                                        },
                                        //lanPriceCurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice + " (старая)", name: 'PriceWholesaleCurrency_OLD', itemId: 'PriceWholesaleCurrency_OLD', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceWholesaleCurrency_OLD' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, readOnly: true
                                        }

                                    ]
                                },

                            ]
                        },

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

                                        {
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupIM', itemId: 'MarkupIM', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'MarkupIM' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },

                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceIMVAT', itemId: 'PriceIMVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceIMVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true
                                        },
                                        //lanPriceCurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceIMCurrency', itemId: 'PriceIMCurrency', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceIMCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },

                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr + " (старая)", name: 'PriceIMVAT_OLD', itemId: 'PriceIMVAT_OLD', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceIMVAT_OLD' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true, readOnly: true
                                        },
                                        //lanPriceCurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice + " (старая)", name: 'PriceIMCurrency_OLD', itemId: 'PriceIMCurrency_OLD', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceIMCurrency_OLD' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, readOnly: true
                                        },

                                    ]
                                },

                            ]
                        },

                    ]
                },

            ]
        });

        this.items = [

            formPanel

        ];


        this.buttons = [
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                text: lanSave, tooltip: lanSave, icon: '../Scripts/sklad/images/save.png'
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                text: lanCancel, tooltip: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
            },

            "->",

            {
                id: "btnDel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDel",
                text: lanDelete, tooltip: lanDelete, icon: '../Scripts/sklad/images/table_delete.png'
            },

        ],


        this.callParent(arguments);
    }

});