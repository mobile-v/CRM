Ext.define("PartionnyAccount.view.Sklad/Object/Sys/SysSettings/viewSysSettingsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewSysSettingsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 550, height: 400,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: lanSettings,

    frame: true,
    border: false,
    resizable: false,
    //modal: true,
    buttonAlign: 'left',

    timeout: varTimeOutDefault,
    waitMsg: lanLoading,

    UO_maximize: false, //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом

    autoHeight: true,

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {

        //1.
        var MethodAccounting_values = [
            [1, 'FIFO'],
            [2, 'LIFO']
        ];
        var PayType_values = [
            [0, 'Касса + Банк'],
            [1, 'Касса'],
            [1, 'Банк'],
        ];
        var DocsSortField_values = [
            [1, 'По порядковому номеру'],
            [2, 'По дате поставщика'],
            [3, 'По дате проведения'],
            [4, 'По дате оплаты'],
        ];
        var DiscountMarketType_values = [
            [1, 'Отнимать от суммы'],
            [2, 'Отнимать от цены']
        ];
        var SettingsValues = Ext.create('Ext.panel.Panel', {
            id: "SettingsValues_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanValues,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [
                //ID
                { xtype: 'textfield', fieldLabel: "SysSettingsID", name: "SysSettingsID", id: "SysSettingsID" + this.UO_id, hidden: true },

                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: lanDefault,
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Организация", flex: 1, width: "100%", allowBlank: true, //, emptyText: "..."

                                    store: this.storeDirContractorsOrgGrid, // store getting items from server
                                    valueField: 'DirContractorID',
                                    hiddenName: 'DirContractorID',
                                    displayField: 'DirContractorName',
                                    name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //disabled: true
                                    //editable: false, typeAhead: false, minChars: 200,
                                },

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: lanVat, flex: 1, width: "100%", allowBlank: false, //, emptyText: "..."
                                    margin: "0 0 0 10",
                                    store: this.storeDirVatsGrid, // store getting items from server
                                    valueField: 'DirVatValue',
                                    hiddenName: 'DirVatValue',
                                    displayField: 'DirVatValue',
                                    name: 'DirVatValue', itemId: "DirVatValue", id: "DirVatValue" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    //editable: false, typeAhead: false, minChars: 200,
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                //MethodAccounting
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: lanMethodAccounting, emptyText: lanMethodAccounting, allowBlank: false, flex: 1, width: "100%",

                                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                                    store: new Ext.data.SimpleStore({
                                        fields: ['MethodAccounting', 'MethodAccountingName'],
                                        data: MethodAccounting_values
                                    }),

                                    valueField: 'MethodAccounting',
                                    hiddenName: 'MethodAccounting',
                                    displayField: 'MethodAccountingName',
                                    name: 'MethodAccounting', itemId: "MethodAccounting", id: "MethodAccounting" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                                //DirCurrency
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: lanCurrency, flex: 1, width: "100%", allowBlank: true, //, emptyText: "..."
                                    margin: "0 0 0 10",
                                    store: this.storeDirCurrenciesGrid, // store getting items from server
                                    valueField: 'DirCurrencyID',
                                    hiddenName: 'DirCurrencyID',
                                    displayField: 'DirCurrencyName',
                                    name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //disabled: true
                                    //Поиск
                                    //editable: false, typeAhead: false, minChars: 200,
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                //DirWarehouse
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: lanWarehouse, flex: 1, width: "100%", allowBlank: true, //, emptyText: "..."

                                    store: this.storeDirWarehousesGrid, // store getting items from server
                                    valueField: 'DirWarehouseID',
                                    hiddenName: 'DirWarehouseID',
                                    displayField: 'DirWarehouseName',
                                    name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //disabled: true
                                    //Поиск
                                },

                                { xtype: 'numberfield', fieldLabel: lanMinimumBalance, name: "DirNomenMinimumBalance", margin: "0 0 0 10", regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, width: "100%", allowBlank: false, minValue: 0 },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                //DirPriceTypes
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Тип цены", flex: 1, width: "100%", allowBlank: true,
                                    store: this.storeDirPriceTypesGrid, // store getting items from server
                                    valueField: 'DirPriceTypeID',
                                    hiddenName: 'DirPriceTypeID',
                                    displayField: 'DirPriceTypeName',
                                    name: 'DirPriceTypeID', itemId: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    //editable: true, typeAhead: true, minChars: 2
                                },

                                //Тип оплаты
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Тип оплаты", emptyText: "Тип оплаты", allowBlank: false, flex: 1, width: "100%",
                                    margin: "0 0 0 10",
                                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                                    store: new Ext.data.SimpleStore({
                                        fields: ['PayType', 'PayTypeName'],
                                        data: PayType_values
                                    }),

                                    valueField: 'PayType',
                                    hiddenName: 'PayType',
                                    displayField: 'PayTypeName',
                                    name: 'PayType', itemId: "PayType", id: "PayType" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                //Тип оплаты
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Сортировка документов", emptyText: "Сортировка документов", allowBlank: false, flex: 1, width: "100%",
                                    //margin: "0 0 0 10",
                                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                                    store: new Ext.data.SimpleStore({
                                        fields: ['DocsSortField', 'DocsSortFieldName'],
                                        data: DocsSortField_values
                                    }),

                                    valueField: 'DocsSortField',
                                    hiddenName: 'DocsSortField',
                                    displayField: 'DocsSortFieldName',
                                    name: 'DocsSortField', itemId: "DocsSortField", id: "DocsSortField" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                     ]
                },


                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Скидки в % от суммы",
                    autoHeight: true,
                    items: [

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'numberfield', fieldLabel: "Магазине", name: "DiscountPercentMarket", regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, width: "100%", allowBlank: false, minValue: 0, maxValue: 100 },
                                { xtype: 'numberfield', fieldLabel: "СЦ", name: "DiscountPercentService", margin: "0 0 0 10", regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, width: "100%", allowBlank: false, maxValue: 100 },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'numberfield', fieldLabel: "БУ", name: "DiscountPercentSecondHand", regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, width: "100%", allowBlank: false, minValue: 0 },
                            ]
                        },


                    ]
                },


                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Скидка в магазине",
                    autoHeight: true,
                    items: [

                        { xtype: 'container', height: 5 },

                        //DiscountMarketType
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Тип", emptyText: "Тип", allowBlank: false, flex: 1, width: "100%",

                            //store: this.storeDirNomenTypesGrid, // store getting items from server
                            store: new Ext.data.SimpleStore({
                                fields: ['DiscountMarketType', 'DiscountMarketTypeName'],
                                data: DiscountMarketType_values
                            }),

                            valueField: 'DiscountMarketType',
                            hiddenName: 'DiscountMarketType',
                            displayField: 'DiscountMarketTypeName',
                            name: 'DiscountMarketType', itemId: "DiscountMarketType", id: "DiscountMarketType" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },


                    ]
                },


                // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Сервисный центр + Заказы",
                    autoHeight: true,
                    items: [

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                //ServiceTypeRepair
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Гарантия", flex: 1, width: "100%", allowBlank: false, //, emptyText: "..."

                                    store: new Ext.data.SimpleStore({
                                        fields: ['ServiceTypeRepair', 'ServiceTypeRepairName'],
                                        data: ServiceTypeRepair_values
                                    }),
                                    valueField: 'ServiceTypeRepair',
                                    hiddenName: 'ServiceTypeRepair',
                                    displayField: 'ServiceTypeRepairName',
                                    name: 'ServiceTypeRepair', itemId: "ServiceTypeRepair", id: "ServiceTypeRepair" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },
                                { xtype: 'numberfield', fieldLabel: "Готовность", name: "ReadinessDay", margin: "0 0 0 10", regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, width: "100%", allowBlank: false, minValue: 0 },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'textfield', fieldLabel: "Телефон.начало", name: "PhoneNumberBegin", flex: 1, width: "100%", allowBlank: false },
                                { xtype: 'numberfield', fieldLabel: "КПД: Готов или Отказной", name: "ServiceKPD", margin: "0 0 0 10", regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, width: "100%", allowBlank: false, minValue: 0 },
                            ]
                        },

                        { xtype: 'container', height: 10 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: "checkbox", boxLabel: "Блокировать повторный ремонт, если сошёл срок гарантии", name: "WarrantyPeriodPassed", id: "WarrantyPeriodPassed" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false },
                                { xtype: "checkbox", boxLabel: "СЦ: SMS при проведении перемещения", name: "SmsServiceMov", id: "SmsServiceMov" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false },
                            ]
                        },

                        { xtype: 'container', height: 10 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: "checkbox", boxLabel: "Заказы SMS: приём аппарата", name: "SmsOrderInt5", id: "SmsOrderInt5" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false },
                                { xtype: "checkbox", boxLabel: "Заказы SMS: готов к выдачи", name: "SmsOrderInt9", id: "SmsOrderInt9" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false },
                                { xtype: "checkbox", boxLabel: "Заказы SMS: исполнен", name: "SmsOrderInt10", id: "SmsOrderInt10" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false },
                            ]
                        },

                        

                    ]
                },



                // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                // !!! HIDDEN !!!
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Префиксы внутренних штрих-кодов (по умолчанию)", hidden: true,
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'numberfield', fieldLabel: lanNomen, name: "BarIntNomen", regex: /^[+]?\d+(?:\d+)?$/, flex: 1, allowBlank: false },
                                { xtype: 'numberfield', fieldLabel: lanContractor, margin: "0 0 0 10", name: "BarIntContractor", regex: /^[+]?\d+(?:\d+)?$/, flex: 1, allowBlank: false }
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'numberfield', fieldLabel: lanDocument, name: "BarIntDoc", regex: /^[+]?\d+(?:\d+)?$/, flex: 1, allowBlank: false },
                                { xtype: 'numberfield', fieldLabel: lanContractor, margin: "0 0 0 10", name: "BarIntEmployee", regex: /^[+]?\d+(?:\d+)?$/, flex: 1, allowBlank: false }
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 }

                    ]
                },


            ]
        });


        //2.
        var SettingsOperations = Ext.create('Ext.panel.Panel', {
            id: "SettingsOperations_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanOperations, //lanLegalDetails
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
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: lanDefault,
                    autoHeight: true,
                    items: [

                        { xtype: "checkbox", boxLabel: txtChangePriceNomen, name: "ChangePriceNomen", id: "ChangePriceNomen" + this.UO_id, inputValue: true, flex: 1 },
                        { xtype: "checkbox", boxLabel: txtMinusResidues, name: "MinusResidues", id: "MinusResidues" + this.UO_id, inputValue: true, flex: 1, readOnly: true },
                        { xtype: "checkbox", boxLabel: txtPurchBigerSale, name: "PurchBigerSale", id: "PurchBigerSale" + this.UO_id, inputValue: true, flex: 1 },
                        { xtype: "checkbox", boxLabel: lanDeletedRecordsShow + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ", name: "DeletedRecordsShow", id: "DeletedRecordsShow" + this.UO_id, inputValue: true, flex: 1 },
                        { xtype: "checkbox", boxLabel: lanReserveNomen, name: "Reserve", id: "Reserve" + this.UO_id, inputValue: true, flex: 1, readOnly: true },
                        { xtype: "checkbox", boxLabel: txtMsg028, name: "CashBookAdd", id: "CashBookAdd" + this.UO_id, inputValue: true, flex: 1 },
                        { xtype: "checkbox", boxLabel: "Touch эфект (открывать одним кликом)", name: "SelectOneClick", id: "SelectOneClick" + this.UO_id, inputValue: true, flex: 1 },
                        { xtype: "checkbox", boxLabel: "Открывать только одну вкладку", name: "TabIdenty", id: "TabIdenty" + this.UO_id, inputValue: true, flex: 1 },
                        { xtype: "checkbox", boxLabel: "Разрешить скидку в для комиссионных БУ аппаратов", name: "DocSecondHandSalesDiscount", id: "DocSecondHandSalesDiscount" + this.UO_id, inputValue: true, flex: 1 },

                    ]
                },

            ]

        });


        //3.
        var SettingsJurnal = Ext.create('Ext.panel.Panel', {
            id: "SettingsJurnal_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanJournals,
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
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: lanPeriodJurnals,
                    autoHeight: true,
                    items: [
                    
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'viewDateField', fieldLabel: lanWith, name: "JurDateS", id: "JurDateS" + this.UO_id, flex: 1, allowBlank: true, editable: false },
                                { xtype: 'viewDateField', fieldLabel: lanTo, name: "JurDatePo", id: "JurDatePo" + this.UO_id, flex: 1, allowBlank: true, editable: false },
                            ]
                        },

                        
                        { xtype: 'container', height: 5 }

                    ]
                },


                // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: txtMsg092,
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'numberfield', fieldLabel: lanDirectories, name: 'PageSizeDir', id: 'PageSizeDir' + this.UO_id, flex: 1, regex: /^[+\-]?\d+(?:\d+)?$/, allowBlank: false, maxValue: 25,
                                    listeners: {
                                        'change': function () {
                                            if (parseInt(this.getValue()) > 25) {
                                                Ext.Msg.alert(lanOrgName, txtMsg093 + " - 25");
                                                this.setValue(25);
                                            }
                                        }
                                    }
                                },
                                {
                                    xtype: 'numberfield', fieldLabel: lanJournals, margin: "0 0 0 10", name: 'PageSizeJurn', id: 'PageSizeJurn' + this.UO_id, flex: 1, regex: /^[+\-]?\d+(?:\d+)?$/, allowBlank: false, maxValue: 25,
                                    listeners: {
                                        'change': function () {
                                            if (parseInt(this.getValue()) > 25) {
                                                Ext.Msg.alert(lanOrgName, txtMsg094 + " - 25");
                                                this.setValue(25);
                                            }
                                        }
                                    }
                                }
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 }

                    ]
                },

            ]

        });


        //4.
        var SettingsPrice = Ext.create('Ext.panel.Panel', {
            id: "SettingsPrice_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanPrice,
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
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: lanFractionalPart + " (" + lanCharNumber + ")",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'numberfield', fieldLabel: lanInSum, name: 'FractionalPartInSum', id: 'FractionalPartInSum' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false, minValue: 0, maxValue: 12,
                                    listeners: {
                                        'change': function () {
                                            if (parseInt(this.getValue()) > 12) {
                                                Ext.Msg.alert(lanOrgName, lanMaximumNumber + " - 12");
                                                this.setValue(12);
                                            }
                                            else if (parseInt(this.getValue()) < 0) {
                                                Ext.Msg.alert(lanOrgName, lanMinimumNumber + " - 0");
                                                this.setValue(12);
                                            }
                                        }
                                    }
                                },
                                {
                                    xtype: 'numberfield', fieldLabel: lanInPrice, margin: "0 0 0 10", name: 'FractionalPartInPrice', id: 'FractionalPartInPrice' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false, maxValue: 12,
                                    listeners: {
                                        'change': function () {
                                            if (parseInt(this.getValue()) > 12) {
                                                Ext.Msg.alert(lanOrgName, lanMaximumNumber + " - 12");
                                                this.setValue(12);
                                            }
                                            else if (parseInt(this.getValue()) < 0) {
                                                Ext.Msg.alert(lanOrgName, lanMinimumNumber + " - 0");
                                                this.setValue(12);
                                            }
                                        }
                                    }
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'numberfield', fieldLabel: lanInOther, name: 'FractionalPartInOther', id: 'FractionalPartInOther' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false, maxValue: 12,
                                    listeners: {
                                        'change': function () {
                                            if (parseInt(this.getValue()) > 12) {
                                                Ext.Msg.alert(lanOrgName, lanMaximumNumber + " - 12");
                                                this.setValue(12);
                                            }
                                            else if (parseInt(this.getValue()) < 0) {
                                                Ext.Msg.alert(lanOrgName, lanMinimumNumber + " - 0");
                                                this.setValue(12);
                                            }
                                        }
                                    }
                                },
                            ]
                        },

                    ]
                },


                // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: lanSurcharge + "%",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'textfield', fieldLabel: lanRetail, name: 'MarkupRetail', id: 'MarkupRetail' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false },
                                { xtype: 'textfield', fieldLabel: "&nbsp;&nbsp;&nbsp;&nbsp;" + lanWholesale, name: 'MarkupWholesale', id: 'MarkupWholesale' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false }
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'textfield', fieldLabel: lanOnlineShop, name: 'MarkupIM', id: 'MarkupIM' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false },
                            ]
                        },

                    ]
                },

                // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: lanAdditionally + ": " + lanSurcharge + "%",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'textfield', fieldLabel: "№1", name: 'MarkupSales1', id: 'MarkupSales1' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false },
                                { xtype: 'textfield', fieldLabel: "&nbsp;&nbsp;&nbsp;&nbsp;№2", name: 'MarkupSales2', id: 'MarkupSales2' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false }
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'textfield', fieldLabel: "№3", name: 'MarkupSales3', id: 'MarkupSales3' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false },
                                { xtype: 'textfield', fieldLabel: "&nbsp;&nbsp;&nbsp;&nbsp;№4", name: 'MarkupSales4', id: 'MarkupSales4' + this.UO_id, flex: 1, regex: /^\s*(\+|-)?\d+\s*$/, allowBlank: false }
                            ]
                        },

                    ]
                },

            ]

        });


        //5. Этикетка
        var SettingsLabel_EncodeType_values = [
            [1, 'UPC-A'],
            [2, 'UPC-E'],
            [3, 'UPC 2 Digit Ext.'],
            [4, 'UPC 5 Digit Ext.'],
            [5, 'EAN-13'],
            [6, 'JAN-13'],
            [7, 'EAN-8'],
            [8, 'ITF-14'],
            [9, 'Codabar'],
            [10, 'PostNet'],
            [11, 'Bookland/ISBN'],
            [12, 'Code 11'],
            [13, 'Code 39'],
            [14, 'Code 39 Extended'],
            [15, 'Code 39 Mod 43'],
            [16, 'Code 93'],
            [17, 'LOGMARS'],
            [18, 'MSI'],
            [19, 'Interleaved 2 of 5'],
            [20, 'Standard 2 of 5'],
            [21, 'Code 128'],
            [22, 'Code 128-A'],
            [23, 'Code 128-B'],
            [24, 'Code 128-C'],
            [25, 'Telepen'],
            [26, 'FIM'],
            [27, 'Pharmacode']

        ];
        var SettingsLabel = Ext.create('Ext.panel.Panel', {
            id: "SettingsLabel_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Этикетка",
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
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'numberfield', fieldLabel: "Width", name: "LabelWidth", regex: /^[+]?\d+(?:\d+)?$/, flex: 1, allowBlank: false },
                        { xtype: 'numberfield', fieldLabel: "Height", margin: "0 0 0 10", name: "LabelHeight", regex: /^[+]?\d+(?:\d+)?$/, flex: 1, allowBlank: false }
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'viewComboBox',
                    fieldLabel: "Тип ШК", emptyText: "Тип ШК", allowBlank: false, flex: 1,

                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                    store: new Ext.data.SimpleStore({
                        fields: ['LabelEncodeType', 'LabelEncodeTypeName'],
                        data: SettingsLabel_EncodeType_values
                    }),

                    valueField: 'LabelEncodeType',
                    hiddenName: 'LabelEncodeType',
                    displayField: 'LabelEncodeTypeName',
                    name: 'LabelEncodeType', itemId: "LabelEncodeType", id: "LabelEncodeType" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    readOnly: true
                },

            ]

        });


        //6.
        var viewDirUnitMeasuresEdit_DateFormat_values = [
            [1, 'ГГГГ-ММ-ДД'],
            [2, 'ДД-ММ-ГГГГ']
        ];
        var SettingsAdditionally = Ext.create('Ext.panel.Panel', {
            id: "SettingsAdditionally_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanAdditionally,
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
                    xtype: 'viewComboBox',
                    fieldLabel: "Формат даты", emptyText: "Формат даты", margin: "0 0 0 10", allowBlank: false, flex: 1,

                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                    store: new Ext.data.SimpleStore({
                        fields: ['DateFormat', 'DateFormatName'],
                        data: viewDirUnitMeasuresEdit_DateFormat_values
                    }),

                    valueField: 'DateFormat',
                    hiddenName: 'DateFormat',
                    displayField: 'DateFormatName',
                    name: 'DateFormat', itemId: "DateFormat", id: "DateFormat" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                },

            ]

        });


        //7.
        var SettingsSms_values = [
            [1, "sms48.ru"],
            [3, "portal.infobip.com"],
        ];
        var SettingsSms = Ext.create('Ext.panel.Panel', {
            id: "SettingsSms_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "SMS",
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%",
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                { xtype: "checkbox", boxLabel: "Использовать SMS оповещение?", name: "SmsActive", itemId: "SmsActive", id: "SmsActive" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, inputValue: true, flex: 1 },

                {
                    xtype: 'viewComboBox',
                    fieldLabel: "Сервис", emptyText: "...", allowBlank: false, flex: 1, disabled: true,

                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                    store: new Ext.data.SimpleStore({
                        fields: ['SmsServiceID', 'SmsServiceName'],
                        data: SettingsSms_values
                    }),

                    valueField: 'SmsServiceID',
                    hiddenName: 'SmsServiceID',
                    displayField: 'SmsServiceName',
                    name: 'SmsServiceID', itemId: "SmsServiceID", id: "SmsServiceID" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                },

                { xtype: 'container', height: 5 },
                { xtype: 'textfield', fieldLabel: "Login", name: "SmsLogin", id: "SmsLogin" + this.UO_id, flex: 1, width: "100%", allowBlank: false, disabled: true },
                { xtype: 'container', height: 5 },
                { xtype: 'textfield', fieldLabel: "Password", name: "SmsPassword", id: "SmsPassword" + this.UO_id, flex: 1, width: "100%", allowBlank: false, disabled: true },
                { xtype: 'container', height: 5 },
                { xtype: 'textfield', fieldLabel: "From", name: "SmsTelFrom", id: "SmsTelFrom" + this.UO_id, flex: 1, width: "100%", allowBlank: false, disabled: true },
                { xtype: 'container', height: 5 },



                //Галочки
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Сервисный центр: авто-отправка СМС при ",
                    autoHeight: true,
                    items: [

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "checkbox", boxLabel: "примке аппарата", name: "DocServicePurchSmsAutoShow", id: "DocServicePurchSmsAutoShow" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false },
                                { xtype: "checkbox", boxLabel: "статусе Готов/Отказной", name: "SmsAutoShow", id: "SmsAutoShow" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false, disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "checkbox", boxLabel: "статусе Выдан", name: "SmsAutoShow9", id: "SmsAutoShow9" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false, disabled: true },
                                { xtype: "checkbox", boxLabel: "возврате на Диагностику из Архива", name: "SmsAutoShowServiceFromArchiv", id: "SmsAutoShowServiceFromArchiv" + this.UO_id, inputValue: true, flex: 1, width: "100%", allowBlank: false, disabled: true },

                            ]
                        },

                    ]
                },

            ]

        });


        //8.
        var SettingsKKMSTaxVariant_values = [
            [0, "0: Общая ОСН"],
            [1, "1: Упрощенная УСН (Доход)"],
            [2, "2: Упрощенная УСН (Доход минус Расход)"],
            [3, "3: Единый налог на вмененный доход ЕНВД"],
            [4, "4: Единый сельскохозяйственный налог ЕСН"],
            [5, "5: Патентная система налогообложения"],
        ];
        var SettingsKKMSTax_values = [
            [0, "0 (НДС 0%)"],
            [10, "10 (НДС 10%)"],
            [18, "18 (НДС 18%)"],
            [-1, "-1 (НДС не облагается)"],
            [118, "118 (НДС 18/118)"],
            [110, "110 (НДС 10/110)"],
        ];
        var SettingsKKMS = Ext.create('Ext.panel.Panel', {
            id: "SettingsKKMS_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "KKM",
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%",
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                { xtype: "checkbox", boxLabel: "Используется KKMServer в организации?", name: "KKMSActive", itemId: "KKMSActive", id: "KKMSActive" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, inputValue: true, flex: 1 },


                {
                    xtype: 'textfield', fieldLabel: "URL Server", name: "KKMSUrlServer", id: "KKMSUrlServer" + this.UO_id, flex: 1, width: "100%", allowBlank: false,
                    disabled: true
                },

                {
                    xtype: 'textfield', fieldLabel: "User", name: "KKMSUser", id: "KKMSUser" + this.UO_id, flex: 1, width: "100%", allowBlank: false,
                    disabled: true
                },


                {
                    xtype: 'textfield', fieldLabel: "Password", name: "KKMSPassword", id: "KKMSPassword" + this.UO_id, flex: 1, width: "100%", allowBlank: false,
                    disabled: true
                },


                {
                    xtype: 'textfield', fieldLabel: "Password", name: "KKMSCashierVATIN", id: "KKMSCashierVATIN" + this.UO_id, flex: 1, width: "100%", allowBlank: false,
                    disabled: true
                },

                {
                    xtype: "numberfield",
                    anchor: "100%",
                    name: "KKMSNumDevice", itemId: "KKMSNumDevice", id: "KKMSNumDevice" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    fieldLabel: "Номер устройства",
                    flex: 1,
                    value: 99, maxValue: 99, minValue: 0,
                    disabled: true
                },

                {
                    xtype: 'textfield', fieldLabel: "ИНН продавца тег ОФД 1203", name: "KKMSCashierVATIN", id: "KKMSCashierVATIN" + this.UO_id, flex: 1, width: "100%", allowBlank: false,
                    disabled: true
                },


                {
                    xtype: 'viewComboBox',
                    fieldLabel: "Система налогообложения", emptyText: "...", allowBlank: false, flex: 1, //disabled: true,

                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                    store: new Ext.data.SimpleStore({
                        fields: ['KKMSTaxVariant', 'KKMSTaxVariantName'],
                        data: SettingsKKMSTaxVariant_values
                    }),

                    valueField: 'KKMSTaxVariant',
                    hiddenName: 'KKMSTaxVariant',
                    displayField: 'KKMSTaxVariantName',
                    name: 'KKMSTaxVariant', itemId: "KKMSTaxVariant", id: "KKMSTaxVariant" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    disabled: true
                },


                {
                    xtype: 'viewComboBox',
                    fieldLabel: "НДС в процентах", emptyText: "...", allowBlank: false, flex: 1, //disabled: true,

                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                    store: new Ext.data.SimpleStore({
                        fields: ['KKMSTax', 'KKMSTaxName'],
                        data: SettingsKKMSTax_values
                    }),

                    valueField: 'KKMSTax',
                    hiddenName: 'KKMSTax',
                    displayField: 'KKMSTaxName',
                    name: 'KKMSTax', itemId: "KKMSTax", id: "KKMSTax" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    disabled: true
                },
                
            ]

        });



        //Tab-Panel
        var tabPanel = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,

            items: [
                SettingsValues, SettingsOperations, SettingsJurnal, SettingsPrice, SettingsLabel, SettingsAdditionally, SettingsSms, SettingsKKMS
            ]

        });

        //Form-Panel
        var formPanel = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)


            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "center", //!!! Важно для Ресайз-а !!!
            monitorValid: true,
            defaultType: 'textfield',

            region: "center",
            width: "100%", height: "100%",
            bodyPadding: 5,
            layout: 'fit',
            defaults: {
                anchor: '100%'
            },
            autoHeight: true,

            items: [
                tabPanel
            ]
        });

        this.items = [

            formPanel

        ];

        this.buttons = [
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                text: lanSave, icon: '../Scripts/sklad/images/save.png'
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                text: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
            },

            "-",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png',
            }

        ],

        this.callParent(arguments);
    }

});

