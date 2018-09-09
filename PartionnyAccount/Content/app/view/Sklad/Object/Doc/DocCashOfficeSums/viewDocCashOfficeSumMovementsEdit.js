Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocCashOfficeSums/viewDocCashOfficeSumMovementsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocCashOfficeSumMovementsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 700, height: 325,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: "Перемещение",
    autoHeight: true,

    frame: true,
    border: false,
    resizable: false,
    //modal: true,
    buttonAlign: 'left',

    timeout: varTimeOutDefault,
    waitMsg: lanLoading,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом
    UO_Modal: true,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocAllEdit',

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {


        var PanelDocumentDetails = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentDetails_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            
            defaults: { anchor: '100%' },
            width: "100%", height: "100%",

            items: [

                // !!! Не видимые !!!
                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },  //, hidden: true
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirMovementStatusID", name: "DirMovementStatusID", id: "DirMovementStatusID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                {
                    xtype: 'viewComboBox',
                    fieldLabel: "Организация", flex: 1, allowBlank: false, //, emptyText: "..."
                    store: this.storeDirContractorsOrgGrid, // store getting items from server
                    valueField: 'DirContractorID',
                    hiddenName: 'DirContractorID',
                    displayField: 'DirContractorName',
                    name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    hidden: true
                },

                { xtype: "checkbox", boxLabel: lanReserve, margin: "0 0 0 5", name: "Reserve", itemId: "Reserve", flex: 1, id: "Reserve" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true, hidden: true },



                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", labelWidth: 15, name: "DocCashOfficeSumMovementID", id: "DocCashOfficeSumMovementID" + this.UO_id, readOnly: true, width: 65, allowBlank: true, }, //labelAlign: "top" 
                        { xtype: 'textfield', fieldLabel: lanManual, labelWidth: 10, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", width: 55, allowBlank: true, hidden: true, }, //labelAlign: "top" 

                        { xtype: 'viewDateField', fieldLabel: lanDateCounterparty, labelWidth: 100, name: "DocDate", id: "DocDate" + this.UO_id, width: 200, margin: "0 0 0 5", allowBlank: false, editable: false, }, //labelAlign: "top" 


                        //Курьер
                        {
                            xtype: 'viewComboBox',
                            flex: 1, allowBlank: true, fieldLabel: "Курьер", labelWidth: 50, //labelAlign: "top",
                            margin: "0 0 0 5",
                            store: this.storeDirEmployeesGrid, // store getting items from server
                            valueField: 'DirEmployeeID',
                            hiddenName: 'DirEmployeeID',
                            displayField: 'DirEmployeeName',
                            name: 'DirEmployeeIDCourier', itemId: "DirEmployeeIDCourier", id: "DirEmployeeIDCourier" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: false, typeAhead: false, minChars: 200,
                            //labelAlign: "top"
                        },
                        { xtype: 'button', text: "X", tooltip: "Очистить", itemId: "btnDirEmployeeIDCourier", id: "btnDirEmployeeIDCourier" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },


                { xtype: 'container', height: 22 },


                //Точка  + Касса
                {
                    xtype: 'container', layout: { align: 'stretch', type: 'hbox' }, //width: "95%", 
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "С точки", labelAlign: "top", flex: 1, allowBlank: false, //, emptyText: "..."
                            //margin: "0 0 0 15",
                            store: this.storeDirWarehousesGridFrom, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseIDFrom', itemId: "DirWarehouseIDFrom", id: "DirWarehouseIDFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "С кассы", labelAlign: "top", flex: 1, allowBlank: false, //, emptyText: "..."
                            //margin: "0 0 0 15",
                            store: this.storeDirCashOfficesGridFrom, // store getting items from server
                            valueField: 'DirCashOfficeID',
                            hiddenName: 'DirCashOfficeID',
                            displayField: 'DirCashOfficeName',
                            name: 'DirCashOfficeIDFrom', itemId: "DirCashOfficeIDFrom", id: "DirCashOfficeIDFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200, readOnly: true,
                        },



                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "На точку", labelAlign: "top", flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirWarehousesGridTo, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseIDTo', itemId: "DirWarehouseIDTo", id: "DirWarehouseIDTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "В кассу", labelAlign: "top", flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirCashOfficesGridTo, // store getting items from server
                            valueField: 'DirCashOfficeID',
                            hiddenName: 'DirCashOfficeID',
                            displayField: 'DirCashOfficeName',
                            name: 'DirCashOfficeIDTo', itemId: "DirCashOfficeIDTo", id: "DirCashOfficeIDTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200, readOnly: true,
                        },

                    ]
                },
                //Суммы
                {
                    xtype: 'container', layout: { align: 'stretch', type: 'hbox' }, //width: "95%", 
                    items: [
                        //{ xtype: "label", text: "Общая сумма", },
                        {
                            xtype: 'textfield', //margin: "0 0 0 0",
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, //fieldLabel: "Курс",
                            name: 'DirCashOfficeSumFrom', itemId: "DirCashOfficeSumFrom", id: "DirCashOfficeSumFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true, 
                        },

                        //{ xtype: "label", text: "Общая сумма", margin: "0 0 0 5", },
                        {
                            xtype: 'textfield', margin: "0 0 0 5",
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, //fieldLabel: "Курс",
                            name: 'DirCashOfficeSumTo', itemId: "DirCashOfficeSumTo", id: "DirCashOfficeSumTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },

                    ]
                },

                { xtype: 'container', height: 22 },




                //Валюта + Цена в Валюте - !!! Не видимое !!!
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor', hidden: true,
                    title: lanPurchase,
                    autoHeight: true,
                    items: [
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            //hidden: true,
                            items: [
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс",
                                    name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },

                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс",
                                    name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", margin: "0 0 0 10",
                                    name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },
                            ]
                        },

                    ]
                },

                //Сумма
                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        {
                            xtype: 'textfield', fieldLabel: "Сумма", name: 'Sums', itemId: 'Sums', id: 'Sums' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: "50%",
                            fieldStyle: 'height: 32px; font-size:20px; color: green;' 
                        },
                        /*{
                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'SumsCurrency', itemId: 'SumsCurrency', id: 'SumsCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, hidden: true,
                        },*/

                    ]
                },



                { xtype: 'container', height: 32 },

                { xtype: "label", text: "Внимание: Если выбрать курьера, то документ попадёт в Логистику!", style: 'color: red; font-weight: bold;' },

            ]
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


            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "center", //!!! Важно для Ресайз-а !!!
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

            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [
                PanelDocumentDetails
            ]
        });



        //body
        this.items = [

            formPanel

        ],


        this.buttons = [
            {
                xtype: 'textfield',
                allowBlank: false, width: 200, fieldLabel: "Ваш пароль", labelWidth: 65, //inputType: 'password',
                name: 'DirEmployeePswd', itemId: 'DirEmployeePswd', id: 'DirEmployeePswd' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            " ",

            {
                id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", hidden: true, style: "width: 120px; height: 40px;",
                text: "<b style='font-size: 14px; color: red;'>" + lanHeldCancel + "</b>",  //text: lanHeldCancel, 
                icon: '../Scripts/sklad/images/save_held.png',
                UO_Action: "held_cancel",
            },
            {
                id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", hidden: true, style: "width: 100px; height: 40px;",
                text: "<b style='font-size: 14px; color: green;'>" + lanHelds + "</b>",  //text: lanHelds, 
                icon: '../Scripts/sklad/images/save_held.png',
                UO_Action: "held",
            },
            {
                id: "btnRecord" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true, style: "width: 120px; height: 40px;",
                text: "<b style='font-size: 14px; color: green;'>" + lanSave + "</b>",  //text: lanSave, 
                icon: '../Scripts/sklad/images/save.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                            text: lanRecord, icon: '../Scripts/sklad/images/save.png',
                            UO_Action: "save",
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSaveClose",
                        text: lanRecordClose, icon: '../Scripts/sklad/images/save.png',
                        UO_Action: "save_close",
                    }
                ]
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", style: "width: 100px; height: 40px;",
                text: "<b style='font-size: 14px; color: red;'>" + lanCancel + "</b>",  //text: lanCancel, 
                icon: '../Scripts/sklad/images/cancel.png'
            },
            " ",
            {
                id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint", hidden: true, style: "width: 100px; height: 40px;",
                text: lanPrint, icon: '../Scripts/sklad/images/print.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintHtml",
                            text: "Html", icon: '../Scripts/sklad/images/html.png',
                            UO_Action: "html",
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintExcel",
                        text: "MS Excel", icon: '../Scripts/sklad/images/excel.png',
                        UO_Action: "excel",
                    }
                ]
            },
            "-",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp", style: "width: 100px; height: 40px;",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png',
            },

        ],


        this.callParent(arguments);
    }

});

