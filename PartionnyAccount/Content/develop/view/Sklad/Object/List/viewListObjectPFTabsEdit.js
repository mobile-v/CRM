Ext.define("PartionnyAccount.view.Sklad/Object/List/viewListObjectPFTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewListObjectPFTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 650, height: 200,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: lanTable,
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


    conf: {},

    initComponent: function () {

        var dataPosition = [
            [1, "слева"],
            [2, "в центре"],
            [3, "справа"]
        ];

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

                {
                    xtype: 'textfield',
                    fieldLabel: lanName, allowBlank: false, flex: 1,
                    name: 'ListObjectPFTabName', itemId: "ListObjectPFTabName", id: "ListObjectPFTabName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                },

                {
                    xtype: 'viewComboBox',
                    fieldLabel: lanListDocField, flex: 1, allowBlank: false,

                    store: this.storeListObjectFieldNamesGrid, // store getting items from server
                    valueField: 'ListObjectFieldNameID',
                    hiddenName: 'ListObjectFieldNameID',
                    displayField: 'ListObjectFieldNameRu',
                    name: 'ListObjectFieldNameID', itemId: "ListObjectFieldNameID", id: "ListObjectFieldNameID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //Поиск
                    editable: true, typeAhead: true, minChars: 2
                },
                {
                    xtype: 'textfield',
                    fieldLabel: lanListDocField, emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                    name: 'ListObjectFieldNameRu', itemId: "ListObjectFieldNameRu", id: "ListObjectFieldNameRu" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            flex: 1, 
                            fieldLabel: lanPosition, labelAlign:'top',
                            editable: false,
                            name: 'PositionID',
                            id: 'PositionID' + this.UO_id,
                            valueField: 'PositionID',
                            hiddenName: 'PositionID',
                            displayField: 'PositionName',
                            triggerAction: 'all',
                            minChars: 2,
                            forceSelection: true,
                            enableKeyEvents: true,
                            allowBlank: false,
                            typeAhead: false,
                            hideTrigger: false,
                            store: new Ext.data.SimpleStore({
                                fields: ['PositionID', 'PositionName'],
                                data: dataPosition
                            })
                        },

                        {
                            xtype: 'numberfield', margin: "0 0 0 5",
                            fieldLabel: "Табличная часть №", labelAlign: 'top', emptyText: "...", allowBlank: true, flex: 1, labelWidth: 120,
                            name: 'TabNum', itemId: "TabNum", id: "TabNum" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        {
                            xtype: 'numberfield', margin: "0 0 0 5",
                            fieldLabel: "Фикс размер ячейки", labelAlign: 'top', emptyText: "...", allowBlank: true, flex: 1, labelWidth: 120,
                            name: 'Width', itemId: "Width", id: "Width" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
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