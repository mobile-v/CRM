Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSalaries/viewDocSalariesEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocSalariesEdit",

    layout: "border",
    region: "center",
    title: lanDocumentSales,
    width: 900, height: 575,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocAllEdit',

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {


        //Tab
        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

        var PanelDocumentDetails = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentDetails_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            
            defaults: { anchor: '100%' },
            width: "100%", height: 75 + varBodyPadding,

            items: [

                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", name: "DocSalaryID", id: "DocSalaryID" + this.UO_id, readOnly: true, width: 250, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true, hidden: true },
                        { xtype: 'viewDateField', fieldLabel: lanDateCounterparty, name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: false, editable: false, readOnly: true, hidden: true },

                        //{ xtype: 'textfield', fieldLabel: "Год", name: "DocYear", id: "DocYear" + this.UO_id, margin: "0 0 0 5", width: 250, allowBlank: false },
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Год", width: 250, allowBlank: true,
                            margin: "0 0 0 5",
                            store: new Ext.data.SimpleStore({
                                fields: ['DocYear', 'DocYearName'],
                                data: DocYear_values
                            }),
                            valueField: 'DocYear',
                            hiddenName: 'DocYear',
                            displayField: 'DocYearName',
                            name: 'DocYear', itemId: "DocYear", id: "DocYear" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                        //{ xtype: 'textfield', fieldLabel: "Месяц", name: "DocMonth", id: "DocMonth" + this.UO_id, margin: "0 0 0 5", width: 250, allowBlank: false },
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Месяц", width: 250, allowBlank: true,
                            margin: "0 0 0 5",
                            store: new Ext.data.SimpleStore({
                                fields: ['DocMonth', 'DocMonthName'],
                                data: ServiceTypeRepair_values
                            }),
                            valueField: 'DocMonth',
                            hiddenName: 'DocMonth',
                            displayField: 'DocMonthName',
                            name: 'DocMonth', itemId: "DocMonth", id: "DocMonth" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanBase, name: "Base", id: "Base" + this.UO_id, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanDisc, margin: "0 0 0 5", name: "Description", id: "Description" + this.UO_id, flex: 1, allowBlank: true }
                    ]
                },
            ]
        });



        //2. Грид
        var PanelGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            itemId: "grid",

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,

            store: this.storeGrid, //storeDocSalaryTabsGrid,

            columns: [
                //Сотрудник
                { text: "№", dataIndex: "DirEmployeeID", width: 50, style: "height: 25px;", hidden: true },
                { text: "Сотрудник", dataIndex: "DirEmployeeName", flex: 1 },
                //Валюта
                //{ text: "Валюта", dataIndex: "DirCurrencyID", flex: 1, hidden: true },
                { text: "Валюта", dataIndex: "DirCurrencyName", flex: 1, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 75, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 75, hidden: true },

                //Цвет
                { text: "X", dataIndex: "X1", width: 10, tdCls: 'x-change-cell' },

                //ЗП
                { text: "Зарплата", dataIndex: "Salary", width: 75 },
                //{ text: "Тип", dataIndex: "SalaryDayMonthly", width: 100 },
                { text: "Тип", dataIndex: "SalaryDayMonthlyName", width: 100 },
                { text: "Дней", dataIndex: "CountDay", width: 75 },
                { text: "Сумма", dataIndex: "SumSalary", width: 100 },

                //Цвет
                { text: "X", dataIndex: "X2", width: 10, tdCls: 'x-change-cell' },

                //Премия-1
                //{ text: "Премия (продавца)", dataIndex: "DirBonusID", flex: 1 },
                { text: "Премия (продавца)", dataIndex: "DirBonusName", flex: 1 },
                { text: "Сумма", dataIndex: "DirBonusIDSalary", flex: 1 },

                //Цвет
                { text: "X", dataIndex: "X3", width: 10, tdCls: 'x-change-cell' },

                //Премия-1
                //{ text: "Премия (мастера)", dataIndex: "DirBonusID2", flex: 1 },
                { text: "Премия (мастера)", dataIndex: "DirBonus2Name", flex: 1 },
                { text: "Сумма", dataIndex: "DirBonusID2Salary", flex: 1 },

                //Цвет
                { text: "X", dataIndex: "X4", width: 10, tdCls: 'x-change-cell' },

                //Сумма
                { text: "Общая", dataIndex: "Sums", flex: 1 },
            ],

            tbar: [
                
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanLoading, tooltip: lanLoading,
                    itemId: "btnGridAddPosition",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: lanEdit, tooltip: lanEdit, disabled: true,
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit"
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete"
                },

            ],


            //Формат даты
            viewConfig: {
                getRowClass: function (record, index) {

                    return 'price-del';

                }, //getRowClass

                stripeRows: true,

            } //viewConfig

        });


        //3. Футер
        var PanelFooter = Ext.create('Ext.panel.Panel', {
            id: "PanelFooter_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            //region: "south",
            bodyStyle: 'background:transparent;',

            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },

            items: [
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //Суммы
                        { xtype: 'textfield', fieldLabel: "Зарплата", labelAlign: 'top', name: "Sum1", id: "Sum1" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: "Премия (продажи)", labelAlign: 'top', name: "Sum2", id: "Sum2" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: "Премия (мастерская)", labelAlign: 'top', name: "Sum1", id: "Sum3" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: "Общая", labelAlign: 'top', name: "Sum4", id: "Sum4" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
                    ]
                },

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


            //bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
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
                PanelDocumentDetails,
                PanelGrid, PanelFooter
            ]
        });




        //body
        this.items = [

            formPanel

        ],


        this.buttons = [
            {
                id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", hidden: true, style: "width: 120px; height: 40px;",
                UO_Action: "held_cancel",
                text: lanHeldCancel, icon: '../Scripts/sklad/images/save_held.png'
            },
            {
                id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", hidden: true, style: "width: 120px; height: 40px;",
                UO_Action: "held",
                text: lanHelds, icon: '../Scripts/sklad/images/save_held.png'
            },
            {
                id: "btnRecord" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true, style: "width: 120px; height: 40px;",
                text: lanSave, icon: '../Scripts/sklad/images/save.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                        UO_Action: "save",
                        text: lanRecord, icon: '../Scripts/sklad/images/save.png',
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSaveClose",
                        UO_Action: "save_close",
                        text: lanRecordClose, icon: '../Scripts/sklad/images/save.png',
                    }
                ]
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", style: "width: 120px; height: 40px;",
                text: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
            },
            " ",
            {
                id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint", hidden: true, style: "width: 120px; height: 40px;",
                text: lanPrint, icon: '../Scripts/sklad/images/print.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintHtml",
                        UO_Action: "html",
                        text: "Html", icon: '../Scripts/sklad/images/html.png',
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintExcel",
                        UO_Action: "excel",
                        text: "MS Excel", icon: '../Scripts/sklad/images/excel.png',
                    }
                ]
            },
            "-",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp", style: "width: 120px; height: 40px;",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png',
            }

        ],


        this.callParent(arguments);
    }

});