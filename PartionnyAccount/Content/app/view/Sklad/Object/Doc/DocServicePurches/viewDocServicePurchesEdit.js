﻿Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocServicePurches/viewDocServicePurchesEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocServicePurchesEdit",

    layout: "border",
    region: "center",
    title: "Сервис - Приёмка", 
    width: 900, height: 550,
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
        
        var locPriceVAT_values = [
            ['Сообщить'],
        ];
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


                //!!! Не видимое !!! *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        //{ xtype: 'fieldset', width: "95%", layout: { align: 'stretch', type: 'column' }, title: "Основное", items: [] },
                        { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },  //, hidden: true
                        { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true, hidden: true },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Мастер", flex: 1, allowBlank: false, store: this.storeDirEmployeesGrid, valueField: 'DirEmployeeID', hiddenName: 'DirEmployeeID', displayField: 'DirEmployeeName', name: 'DirEmployeeIDMaster', itemId: "DirEmployeeIDMaster", id: "DirEmployeeIDMaster" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true,
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirEmployeeEdit", id: "btnDirEmployeeEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirEmployeeReload", id: "btnDirEmployeeReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },

                        /*{
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", flex: 1, allowBlank: false, store: this.storeDirContractorsOrgGrid, valueField: 'DirContractorID', hiddenName: 'DirContractorID', displayField: 'DirContractorName', name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true,
                        },*/
                        { xtype: 'textfield', fieldLabel: "Организация", name: "DirContractorIDOrg", readOnly: true, flex: 1, id: "DirContractorIDOrg" + this.UO_id, allowBlank: true, hidden: true },

                        /*{
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouse, flex: 1, allowBlank: false, store: this.storeDirWarehousesGrid, valueField: 'DirWarehouseID', hiddenName: 'DirWarehouseID', displayField: 'DirWarehouseName', name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true,
                        },*/
                        { xtype: 'textfield', fieldLabel: "Точка", name: "DirWarehouseID", readOnly: true, flex: 1, id: "DirWarehouseID" + this.UO_id, allowBlank: true, hidden: true },

                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirWarehouseEdit", id: "btnDirWarehouseEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },
                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirWarehouseReload", id: "btnDirWarehouseReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },

                        { xtype: 'textfield', fieldLabel: "Аппарат", name: "DirServiceNomenName", readOnly: true, flex: 1, id: "DirServiceNomenName" + this.UO_id, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "DirServiceNomenID", margin: "0 0 0 15", name: "DirServiceNomenID", flex: 1, id: "DirServiceNomenID" + this.UO_id, allowBlank: false, hidden: true },

                        { xtype: "checkbox", boxLabel: "Гарантийный", margin: "0 0 0 5", name: "TypeRepair", itemId: "TypeRepair", inputValue: true, id: "TypeRepair" + this.UO_id, hidden: true },

                        { xtype: "checkbox", boxLabel: "Аккумулятор", name: "ComponentBattery", itemId: "ComponentBattery", inputValue: true, id: "ComponentBattery" + this.UO_id, hidden: true },
                        { xtype: 'textfield', fieldLabel: "с/н", margin: "0 0 0 15", name: "ComponentBatterySerial", flex: 1, id: "ComponentBatterySerial" + this.UO_id, allowBlank: true, hidden: true },
                        { xtype: "checkbox", boxLabel: "Задняя крышка", name: "ComponentBackCover", itemId: "ComponentBackCover", inputValue: true, id: "ComponentBackCover" + this.UO_id, hidden: true },
                        { xtype: "checkbox", boxLabel: "Аппарат", name: "ComponentDevice", itemId: "ComponentDevice", inputValue: true, id: "ComponentDevice" + this.UO_id, hidden: true },

                        { xtype: 'textfield', fieldLabel: "Примечание", labelAlign: "top", name: "Note", flex: 1, id: "Note" + this.UO_id, allowBlank: true, hidden: true },

                        { xtype: "checkbox", boxLabel: "Постоянный", name: "DirServiceContractorRegular", itemId: "DirServiceContractorRegular", inputValue: true, id: "DirServiceContractorRegular" + this.UO_id, hidden: true },
                        
                        /*
                        {
                            xtype: 'viewComboBox', fieldLabel: "Клиент", flex: 1, allowBlank: true,
                            store: this.storeDirServiceContractorsGrid, 
                            valueField: 'DirServiceContractorID', hiddenName: 'DirServiceContractorID', displayField: 'DirServiceContractorName', name: 'DirServiceContractorID', itemId: "DirServiceContractorID", id: "DirServiceContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirServiceContractorEdit", id: "btnDirServiceContractorEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirServiceContractorReload", id: "btnDirServiceContractorReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },
                        */
                        { xtype: 'textfield', fieldLabel: "Клиент", name: "DirServiceContractorID", flex: 1, id: "DirServiceContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true },

                        { xtype: 'textfield', fieldLabel: "Email", margin: "0 0 0 15", name: "DirServiceContractorEmail", flex: 1, id: "DirServiceContractorEmail" + this.UO_id, allowBlank: true, hidden: true },

                        {
                            xtype: 'viewComboBox', fieldLabel: "Тип оплаты", flex: 1, allowBlank: false,
                            store: this.storeDirPaymentTypesGrid,
                            valueField: 'DirPaymentTypeID', hiddenName: 'DirPaymentTypeID', displayField: 'DirPaymentTypeName', name: 'DirPaymentTypeID', itemId: "DirPaymentTypeID", id: "DirPaymentTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },

                        {
                            xtype: 'viewComboBox', fieldLabel: lanCurrency, flex: 1, allowBlank: false,
                            store: this.storeDirCurrenciesGrid, 
                            valueField: 'DirCurrencyID', hiddenName: 'DirCurrencyID', displayField: 'DirCurrencyName', name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },
                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCurrencyEdit", id: "btnCurrencyEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },
                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCurrencyReload", id: "btnCurrencyReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },
                        {
                            xtype: 'textfield', regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс", hidden: true,
                            name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },
                        {
                            xtype: 'textfield', regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", margin: "0 0 0 10", hidden: true,
                            name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },

                        { xtype: "checkbox", margin: "0 0 0 25", boxLabel: "Предоплата", name: "Prepayment", itemId: "Prepayment", inputValue: true, id: "Prepayment" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },

                        { xtype: 'textfield', fieldLabel: "Адрес", margin: "0 0 0 5", name: "DirServiceContractorAddress", itemId: "DirServiceContractorAddress", id: "DirServiceContractorAddress" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, flex: 2, hidden: true },

                        //Номер Чека ККМ
                        { xtype: 'textfield', fieldLabel: "KKMSCheckNumber", name: 'KKMSCheckNumber', itemId: "KKMSCheckNumber", id: "KKMSCheckNumber" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "KKMSIdCommand", name: 'KKMSIdCommand', itemId: "KKMSIdCommand", id: "KKMSIdCommand" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: true, hidden: true },

                    ]
                },


                //Шапка *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'label', name: "DirServiceNomenPatchFull", id: "DirServiceNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true, UO_Text: "" },
                        { xtype: 'viewDateFieldFix', fieldLabel: lanDate, name: "DocDate", id: "DocDate" + this.UO_id, width: 200, readOnly: true, allowBlank: false, editable: false },
                        { xtype: 'textfield', margin: "0 0 0 5", name: "DocServicePurchID", id: "DocServicePurchID" + this.UO_id, width: 75, readOnly: true, allowBlank: true }, //, fieldLabel: "№"
                    ]
                },


                //Аппарат *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        {
                            title: "Клиент",
                            autoHeight: true,
                            xtype: 'fieldset', width: 350, layout: 'anchor',
                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [
                                        //Телефон + ФИО
                                        {
                                            xtype: 'viewComboBox',
                                            //labelAlign: 'top',
                                            fieldLabel: "Тел", labelWidth: 32,
                                            allowBlank: false, flex: 1,
                                            //margin: "0 0 0 5",
                                            store: this.varStoreDirServiceContractorsGrid, // store getting items from server
                                            valueField: 'DirServiceContractorPhone',
                                            hiddenName: 'DirServiceContractorPhone',
                                            displayField: 'DirServiceContractorPhone',
                                            name: 'DirServiceContractorPhone', itemId: "DirServiceContractorPhone", id: "DirServiceContractorPhone" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },

                                        { xtype: 'textfield', fieldLabel: "ФИО", labelWidth: 32, name: "DirServiceContractorName", id: "DirServiceContractorName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, allowBlank: false, flex: 1 },

                                        { xtype: "checkbox", boxLabel: "Срочный ремонт", name: "UrgentRepairs", itemId: "UrgentRepairs", inputValue: true, id: "UrgentRepairs" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                    ]
                                },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            title: "Комплектация",
                            autoHeight: true,
                            xtype: 'fieldset', flex: 2, layout: 'anchor',
                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [

                                        //IMEI + пароль
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                            items: [
                                                { xtype: "checkbox", boxLabel: "Неизвестен", name: "SerialNumberNo", itemId: "SerialNumberNo", width: 100, id: "SerialNumberNo" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                                { xtype: 'textfield', fieldLabel: "IMEI", labelWidth: 30, name: "SerialNumber", flex: 1, id: "SerialNumber" + this.UO_id, allowBlank: false, margin: "0 0 0 5" },

                                                { xtype: "checkbox", boxLabel: "Неизвестен", name: "ComponentPasTextNo", itemId: "ComponentPasTextNo", width: 100, id: "ComponentPasTextNo" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, margin: "0 0 0 5" },
                                                { xtype: 'textfield', fieldLabel: lanPassword, name: "ComponentPasText", id: "ComponentPasText" + this.UO_id, allowBlank: false, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, margin: "0 0 0 5" },
                                            ]
                                        },

                                        { xtype: 'container', height: 5 },

                                        { xtype: 'textfield', fieldLabel: "Другое", labelWidth: 130, name: "ComponentOtherText", flex: 1, id: "ComponentOtherText" + this.UO_id, allowBlank: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }, //, fieldLabel: "Другое"


                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                            items: [
                                                {
                                                    xtype: 'viewComboBox',
                                                    flex: 1, allowBlank: true,
                                                    store: this.storeDirServiceComplectsGrid,
                                                    valueField: 'DirServiceComplectID',
                                                    hiddenName: 'DirServiceComplectID',
                                                    displayField: 'DirServiceComplectName',
                                                    name: 'DirServiceComplectID', itemId: "DirServiceComplectID", id: "DirServiceComplectID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    //Поиск
                                                    //editable: true, typeAhead: true, minChars: 2
                                                },
                                            ]
                                        },

                                    ]
                                },

                            ]
                        },

                    ]
                },

                              
                //Оплата *** *** *** *** *** *** *** *** *** *** *** *** *** *** *
                {
                    xtype: 'fieldset', width: "95%", title: "Статистика + Оплата", items: [

                    //К-во аппаратов
                    {
                        xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                        items: [
                            { xtype: 'displayfield', fieldLabel: "Готовые", name: "QuantityOk", id: "QuantityOk" + this.UO_id, allowBlank: false, flex: 1 },
                            { xtype: 'displayfield', fieldLabel: "Отказные", name: "QuantityFail", id: "QuantityFail" + this.UO_id, allowBlank: false, flex: 1, margin: "0 0 0 5" },
                            { xtype: 'displayfield', fieldLabel: "Общее", name: "QuantityCount", id: "QuantityCount" + this.UO_id, allowBlank: false, flex: 1, margin: "0 0 0 5" },
                        ]
                    },

                    { xtype: 'container', height: 5 },

                    {
                        xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                        items: [

                            //Цена
                            /*{
                                xtype: 'textfield',
                                allowBlank: false, flex: 1, fieldLabel: "Ориентир.стоимость", labelStyle: 'width:130px', //regex: /^[+\-]?\d+(?:\.\d+)?$/, 
                                name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                            },*/
                            {
                                xtype: 'viewComboBox',
                                fieldLabel: "Ориентир.стоимость", labelStyle: 'width:130px',
                                allowBlank: false, flex: 1,

                                store: new Ext.data.SimpleStore({
                                    fields: ['PriceVAT', 'PriceVAT'],
                                    data: locPriceVAT_values
                                }),

                                valueField: 'PriceVAT',
                                hiddenName: 'PriceVAT',
                                displayField: 'PriceVAT',
                                name: 'PriceVAT', itemId: "PriceVAT", id: "PriceVAT" + this.UO_id,
                                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                editable: true
                            },

                            //Предоплата
                            {
                                xtype: 'textfield',
                                fieldLabel: "Предоплата", labelStyle: 'width:100px', regex: /^[+\-]?\d+(?:\.\d+)?$/,
                                margin: "0 0 0 25", name: "PrepaymentSum", flex: 1, id: "PrepaymentSum" + this.UO_id, allowBlank: false
                            },

                            { xtype: 'viewDateField', margin: "0 0 0 25", fieldLabel: "Дата готовности", name: "DateDone", id: "DateDone" + this.UO_id, allowBlank: false, editable: false },
                        ]
                    },

                    { xtype: 'container', height: 5 },

                ]},


                //Неисправность *** *** *** *** *** *** *** *** *** *** *** *** ***
                {
                    xtype: 'fieldset', width: "95%", title: "Неисправность со слов клиента", items: [

                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [

                              { xtype: 'textarea', name: "ProblemClientWords", flex: 1, id: "ProblemClientWords" + this.UO_id, allowBlank: false },

                          ]
                      },

                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [
                              //{ xtype: 'button', tooltip: "Add", iconCls: "button-image-up", readOnly: true, itemId: "btnDirServiceProblemAdd", id: "btnDirServiceProblemAdd" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              {
                                  xtype: 'viewComboBox',
                                  //fieldLabel: "Неисправность", labelWidth: 150,
                                  flex: 1, allowBlank: true, //readOnly: true,
                                  store: this.storeDirServiceProblemsGrid, // store getting items from server
                                  valueField: 'DirServiceProblemID',
                                  hiddenName: 'DirServicevID',
                                  displayField: 'DirServiceProblemName',
                                  name: 'DirServiceProblemID', itemId: "DirServiceProblemID", id: "DirServiceProblemID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                  //Поиск
                                  //editable: true, typeAhead: true, minChars: 2
                              },
                              { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirServiceProblemEdit", id: "btnDirServiceProblemEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirServiceProblemReload", id: "btnDirServiceProblemReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                          ]
                      },

                    ]
                },




                //Типовые неисправности *** *** *** *** *** *** *** *** *** *** ***
                {
                    xtype: 'fieldset', width: "95%", title: "Типовые неисправности", items: [

                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [

                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault1ID", itemId: "DirServiceNomenTypicalFault1ID", flex: 1, id: "DirServiceNomenTypicalFault1ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault2ID", itemId: "DirServiceNomenTypicalFault2ID", flex: 1, id: "DirServiceNomenTypicalFault2ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                          ]
                      },
                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [

                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault3ID", itemId: "DirServiceNomenTypicalFault3ID", flex: 1, id: "DirServiceNomenTypicalFault3ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault4ID", itemId: "DirServiceNomenTypicalFault4ID", flex: 1, id: "DirServiceNomenTypicalFault4ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                          ]
                      },
                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [

                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault5ID", itemId: "DirServiceNomenTypicalFault5ID", flex: 1, id: "DirServiceNomenTypicalFault5ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault6ID", itemId: "DirServiceNomenTypicalFault6ID", flex: 1, id: "DirServiceNomenTypicalFault6ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                          ]
                      },
                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [

                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault7ID", itemId: "DirServiceNomenTypicalFault7ID", flex: 1, id: "DirServiceNomenTypicalFault7ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault8ID", itemId: "DirServiceNomenTypicalFault8ID", flex: 1, id: "DirServiceNomenTypicalFault8ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                          ]
                      },
                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [

                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault9ID", itemId: "DirServiceNomenTypicalFault9ID", flex: 1, id: "DirServiceNomenTypicalFault9ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault10ID", itemId: "DirServiceNomenTypicalFault10ID", flex: 1, id: "DirServiceNomenTypicalFault10ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                          ]
                      },
                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [

                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault11ID", itemId: "DirServiceNomenTypicalFault11ID", flex: 1, id: "DirServiceNomenTypicalFault11ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault12ID", itemId: "DirServiceNomenTypicalFault12ID", flex: 1, id: "DirServiceNomenTypicalFault12ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                          ]
                      },
                      {
                          xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                          items: [

                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault13ID", itemId: "DirServiceNomenTypicalFault13ID", flex: 1, id: "DirServiceNomenTypicalFault13ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                              { xtype: "checkbox", boxLabel: "нет", disabled: true, name: "DirServiceNomenTypicalFault14ID", itemId: "DirServiceNomenTypicalFault14ID", flex: 1, id: "DirServiceNomenTypicalFault14ID" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                          ]
                      },


                    ]
                },


            ],


            //buttonAlign: 'left',
            buttons: [
                {
                    id: "btnSave" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                    UO_Action: "save",
                    text: "<b>Принять аппарат</b>", icon: '../Scripts/sklad/images/save.png',
                    style: "width: 140px; height: 40px;",
                    /*menu:
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
                    ]*/
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
                        },

                        "-",

                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_barcode",
                            UO_Action: "barcode",
                            text: "Штрих-Код", icon: '../Scripts/sklad/images/print.png',
                        },
                        /*
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_barcode_price",
                            UO_Action: "barcode_price",
                            text: "Штрих-Код + Цена", icon: '../Scripts/sklad/images/print.png',
                        },
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_barcode_name",
                            UO_Action: "barcode_name",
                            text: "Штрих-Код + Наименование", icon: '../Scripts/sklad/images/print.png',
                        }
                        */
                    ]
                },
            ]

        });


        //body
        this.items = [

            //Товар
            Ext.create('widget.viewTreeDir', {

                conf: {
                    id: "tree_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

                region: 'west',
                width: 215,

                store: this.storeDirServiceNomenTree,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirServiceNomen"
                },


                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    //{ text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: 'Наименование', flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'Остаток', dataIndex: 'Remains', width: 50, hidden: true, tdCls: 'x-change-cell' },
                    //{ text: 'DirServiceNomenPatchFull', dataIndex: 'DirServiceNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],

                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDocServicePurchesEdit_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDocServicePurchesEdit_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDocServicePurchesEdit_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDocServicePurchesEdit_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDocServicePurchesEdit_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDocServicePurchesEdit_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDocServicePurchesEdit_onTree_addSub;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }

            }),


            // *** *** *** *** *** *** *** *** ***


            formPanelEdit

        ],


        this.callParent(arguments);
    }

});