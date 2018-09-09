Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocServicePurches/viewDocServiceMovTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocServiceMovTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 675, height: 185,
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
    controller: 'viewcontrollerDocServiceMovTabsEdit',

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

                //Не видно
                { xtype: 'textfield', name: 'SerialNumber', itemId: "SerialNumber", readOnly: true, hidden: true },
                { xtype: 'textfield', name: 'DirServiceContractorName', itemId: "DirServiceContractorName", readOnly: true, hidden: true },
                { xtype: 'textfield', name: 'DirServiceContractorPhone', itemId: "DirServiceContractorPhone", readOnly: true, hidden: true },
                { xtype: 'textfield', name: 'PriceVAT', itemId: "PriceVAT", readOnly: true, hidden: true },
                { xtype: 'textfield', name: 'PrepaymentSum', itemId: "PrepaymentSum", readOnly: true, hidden: true },


                //DirServiceNomen
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [
                    
                        {
                            xtype: 'textfield',
                            fieldLabel: "Документ", labelAlign: "top", emptyText: "...", allowBlank: false, width: 100,
                            name: 'DocServicePurchID', itemId: "DocServicePurchID", id: "DocServicePurchID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
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
                        
                    ]
                },


                //DirServiceNomen
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [
                    
                        {
                            xtype: 'textfield',
                            fieldLabel: "DirServiceStatusID", labelAlign: "top", emptyText: "...", allowBlank: false, width: 100,
                            name: 'DirServiceStatusID', itemId: "DirServiceStatusID", id: "DirServiceStatusID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true, hidden: true
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: "Статус", labelAlign: "top", emptyText: "...", allowBlank: false, flex: 1, margin: "0 0 0 5",
                            name: 'DirServiceStatusName', itemId: "DirServiceStatusName", id: "DirServiceStatusName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },


                        {
                            xtype: 'textfield',
                            fieldLabel: "Артикул", labelAlign: "top", emptyText: "...", allowBlank: false, width: 100,
                            name: 'DirServiceStatusID_789', itemId: "DirServiceStatusID_789", id: "DirServiceStatusID_789" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true, hidden: true
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: "Имя товара", labelAlign: "top", emptyText: "...", allowBlank: false, flex: 1, margin: "0 0 0 5",
                            name: 'DirServiceStatusName_789', itemId: "DirServiceStatusName_789", id: "DirServiceStatusName_789" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true, hidden: true
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