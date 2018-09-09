Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocOrderInts/viewDocOrderIntsPurches", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocOrderIntsPurches",

    layout: "border",
    region: "center",
    title: "В продажу",
    width: 585, height: 185,
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
        
        //1. General-Panel
        var formPanelEdit = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanGeneral,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,
            autoHeight: true,

            items: [

                //ID
                { xtype: 'textfield', fieldLabel: "DirNomenID", name: "DirNomenID", id: "DirNomenID" + this.UO_id, allowBlank: true, hidden: true }, //, readOnly: true

                //Валюта + Цена в Валюте
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    hidden: true,
                    items: [
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс",
                            name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс",
                            name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", margin: "0 0 0 10",
                            name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },


                    ]
                },


                //Валюта + Цена в Валюте
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: lanPurchase,
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                //К-во
                                {
                                    xtype: 'numberfield',
                                    value: 1, maxValue: 999999, minValue: 1,
                                    allowBlank: false, width: 125, fieldLabel: "<b>" + lanCount + "</b>", labelWidth: 50,
                                    name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },

                                //Приходная цена
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 185, fieldLabel: "<b>Цена закупки</b>", labelWidth: 100,
                                    margin: "0 0 0 10",
                                    name: 'PriceCurrency', itemId: 'PriceCurrency', id: 'PriceCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },

                                //Приходная цена в валюте
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, margin: "0 0 0 10", fieldLabel: lanPriceInCurr, labelWidth: 75,
                                    margin: "0 0 0 10",
                                    name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    hidden: true
                                },

                                //Минимальный остаток
                                /*{
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: lanMinimumBalance, margin: "0 0 0 10",
                                    name: 'DirNomenMinimumBalance', itemId: 'DirNomenMinimumBalance', id: 'DirNomenMinimumBalance' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },*/


                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Поставщик", labelWidth: 75, flex: 1, allowBlank: false,
                                    margin: "0 0 0 10",
                                    store: this.storeDirContractorsGrid, // store getting items from server
                                    valueField: 'DirContractorID',
                                    hiddenName: 'DirContractorID',
                                    displayField: 'DirContractorName',
                                    name: 'DirContractorID', itemId: "DirContractorID", id: "DirContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    editable: true, typeAhead: true, minChars: 2
                                },

                            ]
                        },

                    ]
                },




                //Валюта + Цена в Валюте
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Продажа",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                {
                                    xtype: 'textfield', fieldLabel: lanSurcharge + " %", labelWidth: 75,
                                    name: 'MarkupRetail', itemId: 'MarkupRetail', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'MarkupRetail' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1
                                },
                                { //lanPriceVat
                                    xtype: 'textfield', fieldLabel: lanPriceInCurr, labelWidth: 75, margin: "0 0 0 10",
                                    name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, hidden: true
                                },
                                //CurrentExchange
                                {
                                    xtype: 'textfield', fieldLabel: lanPrice, labelWidth: 75, margin: "0 0 0 10",
                                    name: 'PriceRetailCurrency', itemId: 'PriceRetailCurrency', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1
                                }

                            ]
                        },

                    ]
                },




                //Цены - Расход
                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' },
                    hidden: true,
                    items: [

                        /*{
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
                                        { //lanPriceVat
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true
                                        },
                                        //CurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceRetailCurrency', itemId: 'PriceRetailCurrency', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        }

                                    ]
                                },

                            ]
                        },*/

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
                                        }

                                    ]
                                },

                            ]
                        },

                    ]
                }



            ],


            //buttonAlign: 'left',
            buttons: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, UO_CashBank: 1, itemId: "btnSave",
                    text: "В продажу", icon: '../Scripts/sklad/images/save.png'
                },
                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: "Отмена", icon: '../Scripts/sklad/images/help16.png'
                },
            ]

        });



        //body
        this.items = [

            formPanelEdit

        ],


        this.callParent(arguments);
    }

});