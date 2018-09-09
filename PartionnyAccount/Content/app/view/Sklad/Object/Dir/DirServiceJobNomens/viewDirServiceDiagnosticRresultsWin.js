Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirServiceJobNomens/viewDirServiceDiagnosticRresultsWin", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDirServiceDiagnosticRresultsWin",
    
    layout: "border",
    region: "center",
    title: "Результат диагностики",
    width: 725, height: 185,
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
               
                //Валюта
                /*
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, hidden: true,
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanCurrency, flex: 1, allowBlank: true,

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
                */

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        //К-во
                        {
                            xtype: 'numberfield',
                            value: 1, maxValue: 999999, minValue: 1,
                            allowBlank: false, fieldLabel: lanCount, labelWidth: 30, width: 85,
                            name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                        //Цена
                        {
                            xtype: 'textfield', fieldLabel: "Цена", labelWidth: 30, name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            margin: "0 0 0 5",
                            allowBlank: true, width: 100
                        },

                        //Результат диагностики
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Результат", labelWidth: 60, flex: 1, allowBlank: true, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: varStoreDirServiceDiagnosticRresultsGrid, // store getting items from server
                            valueField: "DirServiceDiagnosticRresultName",
                            hiddenName: "DirServiceDiagnosticRresultName",
                            displayField: "DirServiceDiagnosticRresultName",
                            name: "DirServiceDiagnosticRresultName", itemId: "DirServiceDiagnosticRresultName", id: "DirServiceDiagnosticRresultName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: true, typeAhead: false, minChars: 200,
                        },

                        {
                            xtype: 'button', tooltip: "Add", iconCls: "button-image-add", itemId: "btnDirServiceDiagnosticRresultNameAdd", id: "btnDirServiceDiagnosticRresultNameAdd" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            fieldLabel: "Цена", labelAlign: "top"
                        },

                    ],
                },


                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'textarea', name: "DiagnosticRresultTxt", flex: 1, id: "DiagnosticRresultTxt" + this.UO_id, allowBlank: false },

                    ]
                },

            ],


            //buttonAlign: 'left',
            buttons: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                    text: lanSave, icon: '../Scripts/sklad/images/save.png'
                },
                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
                },

                /*{
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                },*/
            ]

        });

        
        //body
        this.items = [

            formPanelEdit
        ],


        this.callParent(arguments);
    }

});