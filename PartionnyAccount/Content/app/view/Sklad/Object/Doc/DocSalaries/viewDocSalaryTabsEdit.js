Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSalaries/viewDocSalaryTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocSalaryTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 625, height: 305,
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
                { xtype: 'textfield', fieldLabel: "DirCurrencyID", name: 'DirCurrencyID', id: 'DirCurrencyID' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirCurrencyName", name: 'DirCurrencyName', id: 'DirCurrencyName' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirCurrencyRate", name: 'DirCurrencyRate', id: 'DirCurrencyRate' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirCurrencyMultiplicity", name: 'DirCurrencyMultiplicity', id: 'DirCurrencyMultiplicity' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                { xtype: 'textfield', fieldLabel: "Sums", name: 'Sums', id: 'Sums' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                //DirNomen
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [
                        //Товар
                        {
                            xtype: 'textfield',
                            fieldLabel: "Сотрудник", emptyText: "...", allowBlank: false, flex: 1,
                            name: 'DirEmployeeName', itemId: "DirEmployeeName", id: "DirEmployeeName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: false,
                            name: 'DirEmployeeID', itemId: "DirEmployeeID", id: "DirEmployeeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },

                { xtype: 'container', height: 15 },

                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Зарплата",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Зарплата", 
                                    name: 'Salary', itemId: 'Salary', id: 'Salary' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                                { xtype: 'textfield', fieldLabel: "SalaryDayMonthly", name: 'SalaryDayMonthly', id: 'SalaryDayMonthly' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                { xtype: 'textfield', fieldLabel: "Тип", name: 'SalaryDayMonthlyName', id: 'SalaryDayMonthlyName' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Дней", margin: "0 0 0 10",
                                    name: 'CountDay', itemId: 'CountDay', id: 'CountDay' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Сумма", margin: "0 0 0 10",
                                    name: 'SumSalary', itemId: 'SumSalary', id: 'SumSalary' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Премия (продажа)",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "DirBonusID", name: 'DirBonusID', id: 'DirBonusID' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                                { xtype: 'textfield', fieldLabel: "Премия", name: 'DirBonusName', id: 'DirBonusName' + this.UO_id, readOnly: true, allowBlank: true },

                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Сумма", margin: "0 0 0 10",
                                    name: 'DirBonusIDSalary', itemId: 'DirBonusIDSalary', id: 'DirBonusIDSalary' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Премия (мастерская)",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "DirBonusID2", name: 'DirBonusID2', id: 'DirBonusID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                                { xtype: 'textfield', fieldLabel: "Премия", name: 'DirBonus2Name', id: 'DirBonus2Name' + this.UO_id, readOnly: true, allowBlank: true },

                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Сумма", margin: "0 0 0 10",
                                    name: 'DirBonusID2Salary', itemId: 'DirBonusID2Salary', id: 'DirBonusID2Salary' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                            ]
                        },

                        { xtype: 'container', height: 5 },

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