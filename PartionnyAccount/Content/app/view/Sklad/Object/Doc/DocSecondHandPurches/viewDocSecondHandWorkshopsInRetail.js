Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandWorkshopsInRetail", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocSecondHandWorkshopsInRetail",

    layout: "border",
    region: "center",
    title: "В продажу",
    width: 500, height: 200,
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
                { xtype: 'textfield', fieldLabel: "DirVatID", name: "DirVatID", id: "DirVatID" + this.UO_id, allowBlank: true, hidden: true }, //, readOnly: true
                //System record
                { xtype: "checkbox", boxLabel: "SysRecord", name: "SysRecord", id: "SysRecord" + this.UO_id, inputValue: true, hidden: true, readOnly: true },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'displayfield',
                            //regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false,
                            flex: 1, fieldLabel: "Сумма сдел.", labelAlign: "top",
                            name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        {
                            xtype: 'displayfield',
                            //regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false,
                            flex: 1, fieldLabel: "Сумма рем.", margin: "0 0 0 10", labelAlign: "top",
                            name: 'SumTotal2', itemId: 'SumTotal2', id: 'SumTotal2' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        {
                            xtype: 'displayfield',
                            //regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false,
                            flex: 1, fieldLabel: "Сумма апп.", margin: "0 0 0 10", labelAlign: "top",
                            name: 'PriceVATSums', itemId: 'PriceVATSums', id: 'PriceVATSums' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                    ]
                },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Розница", labelAlign: "top",
                            name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Опт", margin: "0 0 0 10", labelAlign: "top",
                            name: 'PriceWholesaleVAT', itemId: 'PriceWholesaleVAT', id: 'PriceWholesaleVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "И-М", margin: "0 0 0 10", labelAlign: "top",
                            name: 'PriceIMVAT', itemId: 'PriceIMVAT', id: 'PriceIMVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                    ]
                },

            ],


            //buttonAlign: 'left',
            buttons: [
                /*
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, UO_CashBank: 1, itemId: "btnSave1",
                    text: "Наличная", icon: '../Scripts/sklad/images/save.png'
                },
                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, UO_CashBank: 2, itemId: "btnSave2",
                    text: "Безналичная", icon: '../Scripts/sklad/images/save.png'
                },
                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: "Отмена", icon: '../Scripts/sklad/images/help16.png'
                },
                */

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