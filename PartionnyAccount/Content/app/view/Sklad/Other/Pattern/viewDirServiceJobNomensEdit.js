Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewDirServiceJobNomensEdit", {
    extend: "Ext.panel.Panel",
    //extend: InterfaceSystemObjName,
    alias: "widget.viewDirServiceJobNomensEdit",

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

    conf: {},

    initComponent: function () {
        
        //1. General-Panel
        var GeneralPanel = Ext.create('Ext.panel.Panel', {
            id: "panel_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

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
            //autoHeight: true,

            items: [
                //ID
                { xtype: 'textfield', fieldLabel: "DirNomeID", name: "DirServiceJobNomenID", id: "DirServiceJobNomenID" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Sub", name: "Sub", id: "Sub" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirServiceJobNomenType", name: "DirServiceJobNomenType", id: "DirServiceJobNomenType" + this.UO_id, allowBlank: true, hidden: true },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' }, 
                    items: [

                        { xtype: 'textfield', fieldLabel: lanName, name: "DirServiceJobNomenName", id: "DirServiceJobNomenName" + this.UO_id, flex: 1, allowBlank: false },
                        { //lanPriceVat
                            xtype: 'textfield', fieldLabel: "Цена", labelWidth: 75,
                            margin: "0 0 0 15",
                            name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 300
                        },

                    ]
                },

                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: "Описание работы", name: "DirServiceJobNomenNameFull", id: "DirServiceJobNomenNameFull" + this.UO_id, flex: 1, allowBlank: true },
                { xtype: 'textfield', fieldLabel: lanArticle, name: "DirServiceJobNomenArticle", id: "DirServiceJobNomenArticle" + this.UO_id, flex: 1, allowBlank: true, hidden: true },
                //Валюта + Цена в Валюте
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, hidden: true,
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
                    ]
                },


                //Цены - Расход
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

                                        /*{ //lanPriceVat
                                            xtype: 'textfield', name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },*/
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
                                            xtype: 'textfield', name: 'PriceWholesaleVAT', itemId: 'PriceWholesaleVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceWholesaleVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },

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
                                            xtype: 'textfield', name: 'PriceIMVAT', itemId: 'PriceIMVAT', regex: /^[+\-]?\d+(?:\.\d+)?$/, id: 'PriceIMVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },

                                    ]
                                },

                            ]
                        },

                    ]
                },

            ],

        });



        //body
        this.items = [
            
            GeneralPanel

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

                "-",

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                },
        ],


        this.callParent(arguments);
    }

});