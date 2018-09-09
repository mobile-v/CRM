Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocCashOfficeSums/viewDocCashOfficeSumsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocCashOfficeSumsEdit",

    layout: "border",
    region: "center",
    title: lanCashOffice,
    width: 700, height: 400,
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

        //2. Грид
        var PanelGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            itemId: "grid",

            conf: {},

            region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeReportBanksCashOffices,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [
                { text: "DocID", dataIndex: "DocID", width: 50, hidden: true },
                { text: "№ док.", dataIndex: "DocXID", width: 60 },
                { text: "Дата", dataIndex: "DocCashOfficeBankSumDate", width: 120 },
                { text: "Тип операции", dataIndex: "DirCashOfficeBankSumTypeName", flex: 1 },
                { text: "Сумма", dataIndex: "DocCashOfficeBankSumSum", summaryType: 'sum', width: 75 },
                //{ text: lanFixedDiscount, dataIndex: "Discount", summaryType: 'sum', width: 100 },
                { text: "Оператор", dataIndex: "DirEmployeeName", width: 100 },
                { text: "Примечание", dataIndex: "Base", flex: 1 },
                //{ text: "Касса", dataIndex: "DirCashOfficeBankName", width: 110 },
                //{ text: "Точка", dataIndex: "DirWarehouseName", width: 110 },
                { text: "Чек№", dataIndex: "KKMSCheckNumber", width: 55 },
            ],


            //Формат даты
            viewConfig: {
                getRowClass: function (record, index) {

                    // 1. === Исправляем формат даты: "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"  ===  ===  ===  ===  === 
                    for (var i = 0; i < record.store.model.fields.length; i++) {
                        //Если поле типа "Дата"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:s");
                                }
                            }
                        }
                    }
                }, //getRowClass
                stripeRows: true,
            }, //viewConfig


            //В Константах "нижнея панель" не нужна
            bbar: new Ext.PagingToolbar({
                store: this.storeGrid,                      // указано хранилище
                displayInfo: true,                          // вывести инфо обо общем числе записей
                displayMsg: lanShowing + "  {0} - {1} " + lanOf + " {2}"     // формат инфо
            }),


        });


        var formPanel = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)


            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "north", //!!! Важно для Ресайз-а !!!
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

            width: "100%", height: 100,
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,


            items: [

                //Header
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, height: 55,
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", name: "DirCashOfficeID", id: "DirCashOfficeID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "№", name: "DirCashOfficeSumTypeID", id: "DirCashOfficeSumTypeID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                        {
                            xtype: 'textfield',
                            //labelCls: 'textbigger', //fieldCls: 'textbigger', cls: 'textbigger',
                            labelAlign: 'top', fieldLabel: "Наличные в кассе",
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1,
                            name: "DirCashOfficeSum", itemId: "DirCashOfficeSum", id: "DirCashOfficeSum" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },


                    ]
                },

                //Buttons
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: "button", height: 40, flex: 1,
                            id: "btnMake" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnMake",
                            UO_Action: "make",
                            text: "Внести", icon: '../Scripts/sklad/images/table_row_ins.png'
                        },

                        {
                            xtype: "button", height: 40, flex: 1,
                            id: "btnPay" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPay",
                            UO_Action: "pay",
                            text: "Виплатить", icon: '../Scripts/sklad/images/table_row_del.png'
                        },

                        {
                            xtype: "button", height: 40, flex: 1,
                            id: "btnZReport" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnZReport",
                            UO_Action: "zreport",
                            text: "Z-отчет", icon: '../Scripts/sklad/images/clear16.png'
                        }
                    ]
                },


                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },


                //Footer *** *** ***

                //Новая "Сумма"
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    id: "containerFooterX" + this.UO_id, hidden: true,
                    items: [
                        {
                            xtype: 'textfield',
                            //labelCls: 'textbigger', //fieldCls: 'textbigger', cls: 'textbigger',
                            labelAlign: 'top', fieldLabel: "Вид операции - ",
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false,
                            name: "DocCashOfficeSumSum", itemId: "DocCashOfficeSumSum", id: "DocCashOfficeSumSum" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },
                //Примечание
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    id: "containerFooterY" + this.UO_id, hidden: true,
                    items: [
                        {
                            xtype: 'textfield',
                            labelAlign: 'top', fieldLabel: "Примечание",
                            name: "Base", id: "Base" + this.UO_id, flex: 1, allowBlank: true
                        },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //Сотрудник
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    id: "containerFooterZ" + this.UO_id, hidden: true,
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanEmployee, //labelAlign: 'top', 
                            flex: 1, allowBlank: true, //, emptyText: "..."

                            store: this.storeDirEmployeesGrid, // store getting items from server
                            valueField: 'DirEmployeeID',
                            hiddenName: 'DirEmployeeID',
                            displayField: 'DirEmployeeName',
                            name: "DirEmployeeIDMoney", itemId: "DirEmployeeIDMoney", id: "DirEmployeeIDMoney" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnEmployeeClearDMoney", id: "btnEmployeeClearDMoney" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },



            ]

        });


        //Form-Panel
        var formPanel111 = Ext.create('Ext.form.Panel', {
            id: "form111_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
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


        });


        //body
        this.items = [

            {
                xtype: "panel",
                layout: 'border',
                region: "center",
                items: [
                    formPanel,
                    PanelGrid
                ]
            },

        ],


        this.buttons = [
            {
                id: "btnSave" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave", hidden: true,
                text: lanSave, icon: '../Scripts/sklad/images/save.png', UO_Action: "save",
            },
            " ",
            {
                id: "btnCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", hidden: true,
                text: lanCancel, icon: '../Scripts/sklad/images/cancel.png', UO_Action: "cancel",
            },

            "->",

            {
                id: "btnClose" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnClose",
                text: lanNotSave, icon: '../Scripts/sklad/images/cancel.png', UO_Action: "close",
            },
            {
                id: "btnHelp" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png', UO_Action: "help",
            },
        ],


        this.callParent(arguments);
    }

});