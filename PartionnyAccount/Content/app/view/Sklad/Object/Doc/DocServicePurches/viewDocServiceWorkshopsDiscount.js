Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocServicePurches/viewDocServiceWorkshopsDiscount", {
    extend: "Ext.Window", //extend: "Ext.panel.Panel",
    //extend: InterfaceSystemObjName,
    alias: "widget.viewDocServiceWorkshopsDiscount",

    layout: "border",
    region: "center",
    title: "Скидка",
    width: 375, height: 225,
    autoScroll: false,
    closable: false,

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

                { xtype: 'container', height: 5 },

                {
                    xtype: 'textfield',
                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Сумма", labelWidth: 150, hidden: true,
                    name: 'SumTotal2a', itemId: "SumTotal2a", id: "SumTotal2a" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                },
                {
                    xtype: 'textfield',
                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Работа", labelWidth: 150, hidden: true,
                    name: 'SumDocServicePurch1Tabs', itemId: "SumDocServicePurch1Tabs", id: "SumDocServicePurch1Tabs" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                },
                {
                    xtype: 'textfield',
                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Запчасти", labelWidth: 150, hidden: true,
                    name: 'SumDocServicePurch2Tabs', itemId: "SumDocServicePurch2Tabs", id: "SumDocServicePurch2Tabs" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                },

                {
                    xtype: "label", text: "К оплате ...",
                    id: "labelSumTotal2a" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                },

                { xtype: 'container', height: 5 },


                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Скидки",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'numberfield', //regex: /^[+\-]?\d+(?:\.\d+)?$/,
                                    allowBlank: false, flex: 1, fieldLabel: "Скидка для работ", labelWidth: 150,
                                    name: 'DiscountX', itemId: "DiscountX", id: "DiscountX" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },
                                {
                                    xtype: "label", text: "К оплате ...", margin: "0 0 0 10",
                                    id: "labelDiscountX" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'numberfield', //regex: /^[+\-]?\d+(?:\.\d+)?$/,
                                    allowBlank: false, flex: 1, fieldLabel: "Скидка для зап.частей", labelWidth: 150,
                                    name: 'DiscountY', itemId: "DiscountY", id: "DiscountY" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },
                                {
                                    xtype: "label", text: "К оплате ...", margin: "0 0 0 10",
                                    id: "labelDiscountY" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },
                            ]
                        },

                    ]
                },



                //{ xtype: "label", text: "Выбирите тип оплаты!" },
                {
                    xtype: "label", text: "Выбирите тип оплаты!",
                    id: "label2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                },

            ],


            buttonAlign: 'left',
            buttons: [
                {
                    id: "btnDirPaymentTypeID1", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDirPaymentTypeID1", UO_Type: 1,
                    style: "width: 120px; height: 40px;", text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Наличная</b></font>", icon: '../Scripts/sklad/images/save.png'
                },
                "->",
                {
                    id: "btnDirPaymentTypeID2", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDirPaymentTypeID2", UO_Type: 2,
                    style: "width: 120px; height: 40px;", text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Безналичная</b></font>", icon: '../Scripts/sklad/images/save.png'
                }
            ]

        });



        //body
        this.items = [

            formPanelEdit

        ],


        this.callParent(arguments);
    }

});

