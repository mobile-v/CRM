Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirWarehouses/viewDirWarehouses", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirWarehouses",

    layout: "border",
    region: "center",
    title: lanPointSales,
    width: 650, height: 350,
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
        
        
        //1.1. General-Panel
        var SmenaCloseTime_values = [
            ['16:00'],
            ['16:30'],
            ['17:00'],
            ['17:30'],
        ];
        var PanelGeneral = Ext.create('Ext.panel.Panel', {
            id: "PanelGeneral_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "north", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanGeneral,
            bodyPadding: 5,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "50", //width: 500, height: 200,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [
                //ID
                { xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, allowBlank: true, hidden: true }, //, readOnly: true
                //System record
                { xtype: "checkbox", boxLabel: "SysRecord", name: "SysRecord", id: "SysRecord" + this.UO_id, inputValue: true, hidden: true, readOnly: true },

                //Наименование
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanName, name: "DirWarehouseName", id: "DirWarehouseName" + this.UO_id, flex: 1, allowBlank: false }, //, readOnly: true
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanAddress, name: "DirWarehouseAddress", flex: 2, allowBlank: true }, //, readOnly: true
                        { xtype: 'textfield', fieldLabel: lanPhone, name: "Phone", flex: 1, allowBlank: true, margin: "0 0 0 5", }, //, readOnly: true
                    ]
                },

                { xtype: 'container', height: 5 },

                {xtype: "label", text: "Банк и Касса для каждой точки должны быть созданы в соответствующих справочниках!" },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanCashOffice, flex: 1, allowBlank: false, //, emptyText: "..."

                            store: this.storeDirCashOfficesGrid, // store getting items from server
                            valueField: 'DirCashOfficeID',
                            hiddenName: 'DirCashOfficeID',
                            displayField: 'DirCashOfficeName',
                            name: 'DirCashOfficeID', itemId: "DirCashOfficeID", id: "DirCashOfficeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirCashOfficeEdit", id: "btnDirCashOfficeEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirCashOfficeReload", id: "btnDirCashOfficeReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanBank, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirBanksGrid, // store getting items from server
                            valueField: 'DirBankID',
                            hiddenName: 'DirBankID',
                            displayField: 'DirBankName',
                            name: 'DirBankID', itemId: "DirBankID", id: "DirBankID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirBankEdit", id: "btnDirBankEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirBankReload", id: "btnDirBankReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },

                { xtype: 'container', height: 5 },




                {
                    title: "ККМ",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: "checkbox", boxLabel: "Использовать ККМ на точке?", name: "KKMSActive", itemId: "KKMSActive", flex: 1, id: "KKMSActive" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: "checkbox", boxLabel: "Использовать авто-закрытие смены на ККМ?", name: "SmenaClose", itemId: "SmenaClose", flex: 1, id: "SmenaClose" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                {
                                    xtype: 'textfield',
                                    allowBlank: true, flex: 1, fieldLabel: "Формат: 20:55", regex: /^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$/,
                                    name: 'SmenaCloseTime', itemId: "SmenaCloseTime", id: "SmenaCloseTime" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    disabled: true
                                },

                            ]
                        },

                    ]
                },


            ],
        });

        //1.2. Grid
        //var rowEditing = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGridDirWarehouse = Ext.create('Ext.grid.Panel', {
            id: "PanelGridDirWarehouse_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            //hidden: true,
            region: "center",
            loadMask: true,
            //autoScroll: true,
            //touchScroll: true,

            title: "Локации",
            itemId: "PanelGridDirWarehouse_grid",

            store: this.storeDirWarehousesGrid,

            columns: [
                { text: lanName, dataIndex: "DirWarehouseName", flex: 1 },
                { text: "Локация", dataIndex: "DirWarehouseLocName", flex: 1 }
            ],

        });

        var PanelUnion1 = Ext.create('Ext.panel.Panel', {
            id: "PanelUnion1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "north", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanGeneral,
            bodyPadding: 5,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "50", //width: 500, height: 200,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [
                
                PanelGeneral,
                PanelGridDirWarehouse

            ],
        });




        //4. Salary-Panel
        var SalaryDayMonthly_values = [
            [1, 'За день'],
            [2, 'За месяц'],
        ];
        var PanelSalary = Ext.create('Ext.panel.Panel', {
            id: "PanelSalary_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanSalary, //lanPayroll,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [
            
                { xtype: 'container', height: 5 },


                //Торговля
                {
                    title: "Торговля",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                {
                                    xtype: 'viewComboBox',
                                    //fieldLabel: "Тип", //, emptyText: "",
                                    allowBlank: false, width: 350,
                                    margin: "0 0 0 10",
                                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                                    store: new Ext.data.SimpleStore({
                                        fields: ['SalaryPercentTradeType', 'SalaryPercentTradeTypeName'],
                                        data: SalaryPercentTradeType_values
                                    }),

                                    valueField: 'SalaryPercentTradeType',
                                    hiddenName: 'SalaryPercentTradeType',
                                    displayField: 'SalaryPercentTradeTypeName',
                                    name: 'SalaryPercentTradeType', itemId: "SalaryPercentTradeType", id: "SalaryPercentTradeType" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                                { xtype: 'textfield', fieldLabel: "", labelWidth: 300, name: "SalaryPercentTrade", id: "SalaryPercentTrade" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10", flex: 1 },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },


                { xtype: 'container', height: 5 },


                //Сервисный центр
                {
                    title: "Сервисный центр",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [

                        //Выполненная работа
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Работа", //, emptyText: "",
                                    allowBlank: false, width: 350,
                                    margin: "0 0 0 10",
                                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                                    store: new Ext.data.SimpleStore({
                                        fields: ['SalaryPercentService1TabsType', 'SalaryPercentService1TabsTypeName'],
                                        data: SalaryPercentService1TabsType_values
                                    }),

                                    valueField: 'SalaryPercentService1TabsType',
                                    hiddenName: 'SalaryPercentService1TabsType',
                                    displayField: 'SalaryPercentService1TabsTypeName',
                                    name: 'SalaryPercentService1TabsType', itemId: "SalaryPercentService1TabsType", id: "SalaryPercentService1TabsType" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                                { xtype: 'textfield', fieldLabel: "", labelWidth: 300, name: "SalaryPercentService1Tabs", id: "SalaryPercentService1Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                        //Запчасть
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Запчасть", //, emptyText: "",
                                    allowBlank: false, width: 350,
                                    margin: "0 0 0 10",
                                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                                    store: new Ext.data.SimpleStore({
                                        fields: ['SalaryPercentService2TabsType', 'SalaryPercentService2TabsTypeName'],
                                        data: SalaryPercentService2TabsType_values
                                    }),

                                    valueField: 'SalaryPercentService2TabsType',
                                    hiddenName: 'SalaryPercentService2TabsType',
                                    displayField: 'SalaryPercentService2TabsTypeName',
                                    name: 'SalaryPercentService2TabsType', itemId: "SalaryPercentService2TabsType", id: "SalaryPercentService2TabsType" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                                { xtype: 'textfield', fieldLabel: "", labelWidth: 300, name: "SalaryPercentService2Tabs", id: "SalaryPercentService2Tabs" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },


                { xtype: 'container', height: 5 },


                //Б/У
                {
                    title: "Б/У",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [


                        //% с прибыли
                        {xtype: "label", text:"Точка продавшая аппарат:"},
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "% с прибыли", labelAlign: "top", labelWidth: 300, name: "SalaryPercentSecond", id: "SalaryPercentSecond" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                                { xtype: 'textfield', fieldLabel: "Фикс. с каждого проданной единиц", labelAlign: "top", labelWidth: 300, name: "SalaryPercent2Second", id: "SalaryPercent2Second" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                                { xtype: 'textfield', fieldLabel: "Фикс. за отремонтированную единицу", labelAlign: "top", labelWidth: 300, name: "SalaryPercent3Second", id: "SalaryPercent3Second" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                                { xtype: 'textfield', fieldLabel: "% от стоимости аппарата", labelAlign: "top", labelWidth: 300, name: "SalaryPercent7Second", id: "SalaryPercent7Second" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                            ]
                        },

                        { xtype: 'container', height: 15 },

                        //% с прибыли
                        { xtype: "label", text: "Точка купившая аппарат:" },
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "% с прибыли", labelAlign: "top", labelWidth: 300, name: "SalaryPercent4Second", id: "SalaryPercent4Second" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                                { xtype: 'textfield', fieldLabel: "Фикс. с каждого проданной единиц", labelAlign: "top", labelWidth: 300, name: "SalaryPercent5Second", id: "SalaryPercent5Second" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                                { xtype: 'textfield', fieldLabel: "Фикс. за отремонтированную единицу", labelAlign: "top", labelWidth: 300, name: "SalaryPercent6Second", id: "SalaryPercent6Second" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                                { xtype: 'textfield', fieldLabel: "% от стоимости аппарата", labelAlign: "top", labelWidth: 300, name: "SalaryPercent8Second", id: "SalaryPercent8Second" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                            ]
                        },

                    ]
                },


            ]
        });

        

        //5. 
        var PanelHelp = Ext.create('Ext.panel.Panel', {
            id: "PanelHelp_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Help", 
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,



            html:
                "Мастерская:<br />" +
                "1) % или фикс с работы (% с суммы всех работ или одна сумма за все работы в рамках одного ремонта)<br />" + 
                "2) % или фикс с каждой работы (% с суммы всех работ или фикс с каждой работы в рамках одного ремонта)<br />" + 
                "3) % или фикс с запчастей - поле использованные запчасти (% с стоимости запчасти или фиксированная сумма за одну использованную к ремонту запчасти)<br />" + 
                "<br />" +

                "Торговля:<br />" +
                "1) % или фикс с продаж<br />",



            monitorValid: true,
            defaultType: 'textfield'

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
                PanelUnion1, PanelSalary, PanelHelp
            ],

            listeners: {
                tabchange: function (tabPanel, newTab, oldTab, index) {
                    if (newTab.itemId == "PanelRightsAccess_") {
                        //Проверка, есть ли задваенные "radiogroup"
                        /*
                        var RightMyCompany1 = Ext.getCmp("RightMyCompany1" + tabPanel.UO_id).getValue(),
                            RightMyCompany2 = Ext.getCmp("RightMyCompany2" + tabPanel.UO_id).getValue(),
                            RightMyCompany3 = Ext.getCmp("RightMyCompany3" + tabPanel.UO_id).getValue();
                        */
                    }
                }
            },

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

            layout: 'border',
            defaults: { anchor: '100%' },

            region: "center",
            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,


            items: [
                tabPanel
            ],


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

                "-",

                /*{
                    id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHistory",
                    text: lanHistory, icon: '../Scripts/sklad/images/history.png',
                    disabled: true
                },*/

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                },


                /*
                { xtype: 'viewDateField', fieldLabel: "", width: 90, name: "HistoryDate", id: "HistoryDate" + this.UO_id, allowBlank: true, editable: false },

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnChange",
                    text: lanChange, icon: '../Scripts/sklad/images/change.png'
                },
                */
            ]

        });




        //body
        this.items = [

            Ext.create('widget.viewTreeDir', {

                conf: {
                    id: "tree_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

                store: this.storeGrid,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirWarehouse"
                },

                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    { text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Доступ', width: 50, dataIndex: 'Active', sortable: true },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                ],

                /*
                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirWarehouses_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirWarehouses_onTree_folderNewSub;
                        contextMenuTree.folderCopy = controllerDirWarehouses_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirWarehouses_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirWarehouses_onTree_folderSubNull;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }
                */
            }),


            // *** *** *** *** *** *** *** *** ***


            formPanel

        ],


        this.callParent(arguments);
    }

});
