Ext.define("PartionnyAccount.view.Sklad/Object/Sms/viewSms", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewSms",

    layout: "border",
    region: "center",
    title: "Отправка SMS",
    width: 450, height: 500,
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


        var PanelGridSms = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "gridSms",
            id: "gridSms_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            region: "center",
            flex: 1,

            store: this.storeGrid, //storeDocAccountTabsGrid,

            columns: [
                { text: "№", dataIndex: "DirSmsTemplateID", width: 50, hidden: true },
                { text: lanName, dataIndex: "DirSmsTemplateName", flex: 1 },
                { text: lanMessage, dataIndex: "DirSmsTemplateMsg", flex: 1, hidden: true }
            ],
        });


        //Form-Panel
        var formPanel = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)


            //bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "south", //!!! Важно для Ресайз-а !!!
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

            width: "100%", height: 150,
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [
                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'textfield', name: "DocServicePurchID", id: "DocServicePurchID" + this.UO_id, flex: 1, allowBlank: true, hidden: true },

                        { xtype: 'textfield', name: "DocMovementID", id: "DocMovementID" + this.UO_id, flex: 1, allowBlank: true, hidden: true },
                        { xtype: 'textfield', name: "DocSecondHandMovementID", id: "DocSecondHandMovementID" + this.UO_id, flex: 1, allowBlank: true, hidden: true },
                        { xtype: 'textfield', name: "DirWarehouseNameFrom", id: "DirWarehouseNameFrom" + this.UO_id, flex: 1, allowBlank: true, hidden: true },

                        { xtype: 'textfield', name: "DirSmsTemplateID", id: "DirSmsTemplateID" + this.UO_id, flex: 1, allowBlank: false, hidden: true },
                        { xtype: 'textarea', name: "DirSmsTemplateMsg", id: "DirSmsTemplateMsg" + this.UO_id, flex: 1, allowBlank: true },

                    ]
                },
            ],

            buttons: [
                {
                    id: "btnSend" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSend", style: "width: 120px; height: 40px;",
                    text: "Отправить", icon: '../Scripts/sklad/images/send16.png'
                },

            ],

        });



        //body
        this.items = [

            {
                region: 'center',
                xtype: 'panel',
                layout: 'border', // тип лэйоута - трехколонник с подвалом и шапкой
                items: [
                   PanelGridSms,
                   formPanel
                ]
            }

        ],


        this.callParent(arguments);
    }

});