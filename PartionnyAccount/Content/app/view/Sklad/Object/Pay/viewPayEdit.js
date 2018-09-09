Ext.define("PartionnyAccount.view.Sklad/Object/Pay/viewPayEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewPayEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 475, height: 200,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: "Оплата",
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

        //Расположение компонентов:
        //0. DocID, DocXID, DocCashBankID
        //0. DirEmployeeName - вычисляется на сервере
        //0. DirXSumTypeName - вычисляется на сервере

        //1. DirPaymentTypeID - Тип оплаты
        //1. DirXName - Касса или Банка
        //2. DocXSumDate - Дата операции в Кассе или Банке
        //3. DocXSumSum - Сумма
        //4. DirCurrencyName - Валюта (ставится по умолчанию)
        //   DirCurrencyRate и DirCurrencyMultiplicity

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

                //ID-шники: DocID, DocXID, DocCashBankID
                { xtype: 'textfield', fieldLabel: "№", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "№", name: "DocXID", id: "DocXID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "№", name: "DocCashBankID", id: "DocCashBankID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                //Тип оплаты
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //DirPaymentTypes
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Тип оплаты", flex: 2, allowBlank: false,
                            //margin: "0 0 0 5",
                            store: this.storeDirPaymentTypesGrid, // store getting items from server
                            valueField: 'DirPaymentTypeID',
                            hiddenName: 'DirPaymentTypeID',
                            displayField: 'DirPaymentTypeName',
                            name: 'DirPaymentTypeID', itemId: "DirPaymentTypeID", id: "DirPaymentTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                        },

                        //Номер документа
                        {
                            xtype: 'textfield', allowBlank: true, flex: 1, margin: "0 0 0 10", //, fieldLabel: lanName, emptyText: "..."
                            name: 'DirXName', itemId: "DirXName", id: "DirXName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

                //Дата и Сумма
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //Дата (2018-07-15 00:00:00)
                        //{ xtype: 'viewDateField', fieldLabel: lanPaymentDate, name: "DocXSumDate", id: "DocXSumDate" + this.UO_id, allowBlank: false, editable: false },
                        {
                            xtype: 'datefield', fieldLabel: lanPaymentDate, name: "DocXSumDate", id: "DocXSumDate" + this.UO_id, allowBlank: false, editable: false,
                            format: 'Y-m-d H:i:s',
                        },

                        //Приходная цена в валюте
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, margin: "0 0 0 10", fieldLabel: lanSum, margin: "0 0 0 10",
                            name: 'DocXSumSum', itemId: 'DocXSumSum', id: 'DocXSumSum' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },



                { xtype: 'container', height: 5 },

                //Валюта
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanCurrency, flex: 2, allowBlank: true, //readOnly: true,

                            store: this.storeDirCurrenciesGrid, // store getting items from server
                            valueField: 'DirCurrencyID',
                            hiddenName: 'DirCurrencyID',
                            displayField: 'DirCurrencyName',
                            name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                        },
                    ]
                },
                //Курс и Кратность
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс", readOnly: true,
                            name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true
                        },
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", readOnly: true, margin: "0 0 0 10",
                            name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true
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