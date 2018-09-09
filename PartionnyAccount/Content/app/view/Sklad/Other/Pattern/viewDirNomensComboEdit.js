Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewDirNomensComboEdit", {
    extend: "Ext.panel.Panel",
    //extend: InterfaceSystemObjName,
    alias: "widget.viewDirNomensComboEdit",

    layout: "border",
    region: "center",
    //title: lanGoods,
    //width: 750, height: 350,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDirNomens',

    conf: {},

    initComponent: function () {

        var GeneralPanel = Ext.create('Ext.panel.Panel', {
            id: "generalPanel_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',

            defaults: { anchor: '100%' },
            //width: "100%", height: 115 + varBodyPadding,
            autoScroll: true,
            //split: true,

            items: [

                { xtype: 'container', height: 5 },

                //ID
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    hidden: true,
                    items: [
                        { xtype: 'textfield', fieldLabel: "DocSecondHandPurchID", name: "DocSecondHandPurchID", id: "DocSecondHandPurchID" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: true, readOnly: true },

                        { xtype: 'textfield', fieldLabel: "Код (старый)", name: "DirNomenID_OLD", id: "DirNomenID_OLD" + this.UO_id, allowBlank: true, readOnly: true, margin: "0 0 0 10" },
                        { xtype: 'textfield', fieldLabel: "Sub", name: "Sub", id: "Sub" + this.UO_id, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: "Артикул", name: "DirNomenID", id: "DirNomenID" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: true, readOnly: true },
                    ]
                },
                /*
                //Article + NomenType
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "Артикул", name: "DirNomenID", id: "DirNomenID" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: true, hidden: true, readOnly: true },
                        { xtype: 'textfield', fieldLabel: "Артикул", name: "DirNomenID_INSERT", id: "DirNomenID_INSERT" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: true, readOnly: true },
                        //{ xtype: 'textfield', fieldLabel: lanArticle, name: "DirNomenArticle", id: "DirNomenArticle" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanNomenType, allowBlank: false, flex: 1, //, emptyText: "Тип"
                            margin: "0 0 0 10",
                            store: this.storeDirNomenTypesGrid, // store getting items from server
                            valueField: 'DirNomenTypeID',
                            hiddenName: 'DirNomenTypeID',
                            displayField: 'DirNomenTypeName',
                            name: 'DirNomenTypeID', itemId: "DirNomenTypeID", id: "DirNomenTypeID" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },
                */
                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: lanName, name: "DirNomenName", id: "DirNomenName" + this.UO_id, flex: 1, allowBlank: false, readOnly: true },
                { xtype: 'textfield', fieldLabel: lanNameFull, name: "DirNomenNameFull", id: "DirNomenNameFull" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },





                //Группы
                {
                    title: "Группа",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [
                        {
                            xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                            items: [


                                //Группы: 1 и 2
                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                    items: [
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                            items: [

                                                {
                                                    xtype: 'viewComboBox',
                                                    fieldLabel: "Марка", flex: 1, allowBlank: false, //Группа-1

                                                    store: this.storeDirNomensGrid1, // store getting items from server
                                                    valueField: 'DirNomenID',
                                                    hiddenName: 'DirNomenID',
                                                    displayField: 'DirNomenName',
                                                    name: "DirNomen1ID", itemId: "DirNomen1ID", id: "DirNomen1ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    //Поиск
                                                    editable: true, typeAhead: true, minChars: 2
                                                },
                                                //{ xtype: 'textfield', fieldLabel: "DirNomen1ID", name: "DirNomen1ID", id: "DirNomen1ID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                                { xtype: 'textfield', fieldLabel: "DirNomen1Name", name: "DirNomen1Name", id: "DirNomen1Name" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                                            ]
                                        },

                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                            items: [

                                                {
                                                    xtype: 'viewComboBox',
                                                    fieldLabel: "Модель", flex: 1, allowBlank: true, //Группа-2
                                                    margin: "0 0 0 10",
                                                    store: this.storeDirNomensGrid2, // store getting items from server
                                                    valueField: 'DirNomenID',
                                                    hiddenName: 'DirNomenID',
                                                    displayField: 'DirNomenName',
                                                    name: "DirNomen2ID", itemId: "DirNomen2ID", id: "DirNomen2ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    readOnly: true,
                                                    //Поиск
                                                    editable: true, typeAhead: true, minChars: 2
                                                },
                                                //{ xtype: 'textfield', fieldLabel: "DirNomen2ID", name: "DirNomen2ID", id: "DirNomen2ID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                                { xtype: 'textfield', fieldLabel: "DirNomen2Name", name: "DirNomen2Name", id: "DirNomen2Name" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                            ]
                                        },
                                    ]
                                },

                                { xtype: 'container', height: 5 },

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Товар", allowBlank: false, flex: 1, //, emptyText: "Тип"

                                    store: this.storeDirNomenCategoriesGrid, // store getting items from server
                                    valueField: 'DirNomenCategoryID',
                                    hiddenName: 'DirNomenCategoryID',
                                    displayField: 'DirNomenCategoryName',
                                    name: 'DirNomenCategoryID', itemId: "DirNomenCategoryID", id: "DirNomenCategoryID" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    editable: true, typeAhead: true, minChars: 2,
                                },
                                { xtype: 'textfield', fieldLabel: "DirNomenCategoryName", name: "DirNomenCategoryName", id: "DirNomenCategoryName" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },



                                { xtype: 'container', height: 5 },

                            ]
                        },

                    ]
                },





                //Валюта + Цена в Валюте
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: lanPurchase,
                    autoHeight: true,
                    items: [

                        //!!! НЕ видно !!!
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            hidden: true,
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
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, margin: "0 0 0 10", fieldLabel: lanPriceInCurr, margin: "0 0 0 10",
                                    name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },
                            ]
                        },


                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },


                        //!!! НЕ видно !!!
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            hidden: true,
                            items: [
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
                            ]
                        },


                        //Для растояния между Контейнерами
                        //{ xtype: 'container', height: 5 },


                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
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
                                },

                                //Приходная цена
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>Цена закупки</b>", margin: "0 0 0 10",
                                    name: 'PriceCurrency', itemId: 'PriceCurrency', id: 'PriceCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },

                                //Минимальный остаток
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: lanMinimumBalance, margin: "0 0 0 10",
                                    name: 'DirNomenMinimumBalance', itemId: 'DirNomenMinimumBalance', id: 'DirNomenMinimumBalance' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },
                            ]
                        },

                    ]
                },

                //Цены - Расход
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

        });
        

        //body
        this.items = [
            
            GeneralPanel,

        ],


        this.buttons = [

            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                text: lanSave, icon: '../Scripts/sklad/images/save.png'
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                text: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
            },

            /*
            "-",
            {
                id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHistory",
                text: lanHistory, icon: '../Scripts/sklad/images/history.png',
                disabled: true
            },

            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
            },
            */
    ],



        this.callParent(arguments);
    }

});