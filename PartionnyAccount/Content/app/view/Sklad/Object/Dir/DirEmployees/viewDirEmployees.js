Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirEmployees/viewDirEmployees", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirEmployees",

    layout: "border",
    region: "center",
    title: lanEmployees,
    width: 725, height: 485,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    //Контроллер
    controller: 'viewcontrollerDirEmployees',

    conf: {},

    initComponent: function () {


        //1. General-Panel
        var PanelGeneral = Ext.create('Ext.panel.Panel', {
            id: "PanelGeneral_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanGeneral,
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
                { xtype: 'textfield', fieldLabel: "DirEmployeeID", name: "DirEmployeeID", id: "DirEmployeeID" + this.UO_id, hidden: true },
                //System record
                { xtype: "checkbox", boxLabel: "SysRecord", name: "SysRecord", id: "SysRecord" + this.UO_id, inputValue: true, hidden: true, readOnly: true },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanFIO, name: "DirEmployeeName", id: "DirEmployeeName" + this.UO_id, flex: 1, allowBlank: false },
                        { xtype: 'textfield', fieldLabel: lanEMail, margin: "0 0 0 10", name: "EMail", id: "EMail" + this.UO_id, flex: 1, allowBlank: true },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanAddress, name: "Address", id: "Address" + this.UO_id, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanPhone, margin: "0 0 0 10", name: "Phone", id: "Phone" + this.UO_id, width: 250, allowBlank: true }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'viewDateField', fieldLabel: lanDateHire, name: "DateHire", id: "DateHire" + this.UO_id, flex: 1, allowBlank: false, editable: false }, //"Y-m-d"
                        { xtype: 'viewDateField', fieldLabel: lanDateTermination, margin: "0 0 0 10", name: "DateTermination", id: "DateTermination" + this.UO_id, flex: 1, allowBlank: true } //"Y-m-d"
                    ]
                },


                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'textfield',
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            fieldLabel: lanImageURL, name: "ImageLink", itemId: "ImageLink", id: "ImageLink" + this.UO_id, flex: 1, allowBlank: true, labelWidth: 150,
                        },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'image',
                            id: "imageShow" + this.UO_id,
                            src: '../Scripts/sklad/images/ru_default_no_foto.jpg',
                            style: {
                                'display': 'block',
                                'margin': 'auto'
                            },
                            alt: "Photo",
                            width: 320, height: 240,
                            region: "center"
                        }
                    ]
                },
            ]
        });


        //2. Account-Panel
        var PanelAccount = Ext.create('Ext.panel.Panel', {
            id: "PanelAccount_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanAccess,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                { xtype: 'fieldset', width: "95%", layout: { align: 'stretch', type: 'column' }, title: lanAccess, items: [ { xtype: 'label', text: "Что бы дать доступ сотруднику в сервис - установите переключатель, придумайте логин и пароль, а так же на вкладке 'Права доступа' установите права." } ] },

                { xtype: "checkbox", boxLabel: lanAccessToProgram, name: "DirEmployeeActive", itemId: "DirEmployeeActive", flex: 1, id: "DirEmployeeActive" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanLogin, name: "DirEmployeeLogin", id: "DirEmployeeLogin" + this.UO_id, flex: 1, allowBlank: false, disabled: true },
                        { xtype: 'textfield', fieldLabel: lanPassword, margin: "0 0 0 10", name: "DirEmployeePswd", id: "DirEmployeePswd" + this.UO_id, inputType: 'password', flex: 1, allowBlank: false, disabled: true }
                    ]
                },

                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: lanHyperlink, emptyText: lanHyperlink + ": " + lanAccessToProgram, readOnly: true, name: "DirEmployeeLink", id: "DirEmployeeLink" + this.UO_id, width: 550, allowBlank: true },



                //Для растояния между Контейнерами
                { xtype: 'container', height: 10 },


                { xtype: 'fieldset', width: "95%", layout: { align: 'stretch', type: 'column' }, title: "Привязка", items: [{ xtype: 'label', text: "Выбор склада нужен для строгой привязки сотрудника к точке продаж, что бы сотрудник не мог совершать операции с остатками на других складах. При выборе переключателя 'Интерфейс розница' - сотруднику не будет доступен основной интерфейс сервиса, а только модуль Розница, так же нужно выбрать Организацию." }] },

                //{ xtype: "checkbox", boxLabel: "Доступ в интерфейс 'Розница'", name: "RetailOnly", itemId: "RetailOnly", flex: 1, id: "RetailOnly" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, disabled: true }, //, hidden: true

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", flex: 1, allowBlank: true, //, emptyText: "..."
                            //margin: "0 0 0 5",
                            store: this.storeDirContractorsOrgGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', itemId: "btnClearContractorOrg", id: "btnClearContractorOrg" + this.UO_id, tooltip: "Clear ContractorOrg", text: "X", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, disabled: true },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirContractorOrgEdit", id: "btnDirContractorOrgEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirContractorOrgReload", id: "btnDirContractorOrgReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },

            ]
        });



        //3. Warehouses-Panel

        //1. General-Panel
        var PanelWarehouses1 = Ext.create('Ext.panel.Panel', {
            id: "PanelWarehouses1_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

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

            //margin: "0 5 0 0",

            items: [
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'label', text: "Роль администратора точки даёт возможность выбирать 'Мастера' для ремонтируемого аппарата." },
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Точка", flex: 1, allowBlank: true, labelWidth: 50,

                            store: this.storeDirWarehousesGrid, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },

                        //{ xtype: "checkbox", boxLabel: "Администратор", name: "IsAdmin", itemId: "IsAdmin", width: 110, id: "IsAdmin" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, margin: "0 0 0 5" },

                        {
                            xtype: 'button', itemId: "btnAddWarehouse", id: "btnAddWarehouse" + this.UO_id,
                            //tooltip: "Clear Warehouse", text: "=>",
                            icon: '../Scripts/sklad/images/table_add.png', tooltip: lanNewM,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, //disabled: true
                            margin: "0 0 0 5"
                        },
                        {
                            id: "btnDelWarehouse" + this.UO_id,
                            UO_id: this.UO_id,
                            UO_idMain: this.UO_idMain,
                            UO_idCall: "PanelGridEmployeeWarehouses_" + this.UO_id,
                            xtype: "button",
                            icon: '../Scripts/sklad/images/table_delete.png', tooltip: lanDelete, disabled: true,
                            itemId: "PanelGridEmployeeWarehouses_btnDelete"
                        },


                        { xtype: "checkbox", boxLabel: "Отменить все?", name: "RightDocServicePurchesWarehouseAllCheck", itemId: "RightDocServicePurchesWarehouseAllCheck", flex: 1, id: "RightDocServicePurchesWarehouseAllCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 0 0 25" },
                    ]
                },

            ]
        });

        //2. Grid
        var rowEditing = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelWarehouses2 = Ext.create('Ext.grid.Panel', {
            itemId: "PanelGridEmployeeWarehouses_grid",
            id: "PanelGridEmployeeWarehouses_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            region: "center",
            loadMask: true,
            //autoScroll: true,
            touchScroll: true,
            title: lanGraduation,
            store: this.storeDirEmployeeWarehousesGrid,

            columns: [
                //{ text: "№", dataIndex: "DirWarehouseID", flex: 1 },
                { text: lanWarehouses, dataIndex: "DirWarehouseName", flex: 1 },
                
                { text: "Админ Точки?", dataIndex: "IsAdmin", width: 125, hidden: true },
                {
                    text: "Доступ", dataIndex: "IsAdminNameRu", width: 150, tdCls: 'x-change-cell102', //, tdCls: 'x-change-cell2',
                    editor: {
                        xtype: 'combo', 
                        UO_idNumber: this.UO_id, UO_id: this.UO_id, 
                        name: "IsAdminNameRu",
                        id: "IsAdminNameRu",

                        triggerAction: 'all', // query all records on trigger click
                        minChars: 2, // minimum characters to start the search
                        enableKeyEvents: true, // otherwise we will not receive key events 
                        pageSize: 9990000,
                        queryMode: 'local',
                        resizable: false, // make the drop down list resizable
                        minListWidth: 220, // we need wider list for paging toolbar
                        //allowBlank: false, // force user to fill something
                        typeAhead: false,
                        hideTrigger: false,
                        editable: false,
                        width: '95%',

                        valueField: 'IsAdminNameRu',
                        hiddenName: 'IsAdminNameRu',
                        displayField: 'IsAdminNameRu',

                        store: IsAdminNameRu_store,


                        tpl: [
                            '<tpl for=".">',
                            '<div class="x-boundlist-item">',
                            '<img style="float: left;" src="{img}"/> {IsAdminNameRu}',
                            '</div>',
                            '</tpl>'
                        ],
                       
                    }
                },
                
                { text: "Видит аппараты?", dataIndex: "WarehouseAll", width: 125, hidden: true },
                {
                    text: "СЦ: видит аппар.", dataIndex: "WarehouseAllNameRu", width: 105, tdCls: 'x-change-cell103', //, tdCls: 'x-change-cell2',
                    editor: {
                        xtype: 'combo',
                        UO_idNumber: this.UO_id, UO_id: this.UO_id,
                        name: "WarehouseAllNameRu",
                        id: "WarehouseAllNameRu",

                        triggerAction: 'all', // query all records on trigger click
                        minChars: 2, // minimum characters to start the search
                        enableKeyEvents: true, // otherwise we will not receive key events 
                        pageSize: 9990000,
                        queryMode: 'local',
                        resizable: false, // make the drop down list resizable
                        minListWidth: 220, // we need wider list for paging toolbar
                        //allowBlank: false, // force user to fill something
                        typeAhead: false,
                        hideTrigger: false,
                        editable: false,
                        width: '95%',

                        valueField: 'WarehouseAllNameRu',
                        hiddenName: 'WarehouseAllNameRu',
                        displayField: 'WarehouseAllNameRu',

                        store: WarehouseAllNameRu_store,


                        tpl: [
                            '<tpl for=".">',
                            '<div class="x-boundlist-item">',
                            '<img style="float: left;" align="left" src="{img}"/> {WarehouseAllNameRu}',
                            '</div>',
                            '</tpl>'
                        ],

                    }
                },


            ],
            

            viewConfig: {

                getRowClass: function (record, index) {
                    
                    //1.  === Стили-1 ===  ===  ===  ===  === 
                    /*
                    if (record.get('IsAdminNameRu') == 'Администратор') {
                        return 'prisutstvuetEmpl1';
                    }
                    else if (record.get('IsAdminNameRu') == 'Не Администратор') {
                        return 'spis-s-zpEmpl1';
                    }
                    */

                    if (record.get('IsAdminNameRu') == 'Администратор' && record.get('WarehouseAllNameRu') == 'Виден') {
                        return 'prisutstvuetEmpl1 prisutstvuetEmpl2';
                    }
                    else if (record.get('IsAdminNameRu') == 'Администратор' && record.get('WarehouseAllNameRu') == 'Не Виден') {
                        return 'prisutstvuetEmpl1 spis-s-zpEmpl2';
                    }
                    else if (record.get('IsAdminNameRu') == 'Не Администратор' && record.get('WarehouseAllNameRu') == 'Виден') {
                        return 'spis-s-zpEmpl1 prisutstvuetEmpl2';
                    }
                    else if (record.get('IsAdminNameRu') == 'Не Администратор' && record.get('WarehouseAllNameRu') == 'Не Виден') {
                        return 'spis-s-zpEmpl1 spis-s-zpEmpl2';
                    }

                },


                stripeRows: true,

            }, //viewConfig
            

            plugins: [rowEditing],
            rowEditing: rowEditing,

        });

        var PanelWarehouses = Ext.create('Ext.panel.Panel', {
            id: "PanelWarehouses_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Точки",
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'border',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                PanelWarehouses1, PanelWarehouses2

            ]
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

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanCurrency, flex: 1, allowBlank: true, //, emptyText: "..."

                            store: this.storeDirCurrenciesGrid, // store getting items from server
                            valueField: 'DirCurrencyID',
                            hiddenName: 'DirCurrencyID',
                            displayField: 'DirCurrencyName',
                            name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true, editable: false, typeAhead: false, minChars: 200, 
                            //hidden: true
                        }
                    ]
                },


                { xtype: 'container', height: 5 },


                {
                    title: "Зарплата",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [


                                //Может быть как ЗП за 1 день (Salary, SalaryDayMonthly), так и ЗП за месяц (SalaryFixedSalesMount)
                                { xtype: 'textfield', fieldLabel: lanSalary, name: "Salary", id: "Salary" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true },


                                {
                                    xtype: 'viewComboBox', 
                                    fieldLabel: lanSalaryDayMonthly, allowBlank: true, flex: 1, //, emptyText: lanSalaryDayMonthly
                                    margin: "0 0 0 10",
                                    //store: this.storeDirNomenTypesGrid, // store getting items from server
                                    store: new Ext.data.SimpleStore({
                                        fields: ['SalaryDayMonthly', 'SalaryDayMonthlyName'],
                                        data: SalaryDayMonthly_values
                                    }),

                                    valueField: 'SalaryDayMonthly',
                                    hiddenName: 'SalaryDayMonthly',
                                    displayField: 'SalaryDayMonthlyName',
                                    name: 'SalaryDayMonthly', itemId: "SalaryDayMonthly", id: "SalaryDayMonthly" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },


                { xtype: 'container', height: 5 },


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
                                    fieldLabel: lanBonus + " (продажа)", labelWidth: 125, flex: 1, allowBlank: true, //, emptyText: "..."

                                    store: this.storeDirBonusesGrid, // store getting items from server
                                    valueField: 'DirBonusID',
                                    hiddenName: 'DirBonusID',
                                    displayField: 'DirBonusName',
                                    name: 'DirBonusID', itemId: "DirBonusID", id: "DirBonusID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //disabled: true, editable: false, typeAhead: false, minChars: 200,
                                },
                                { xtype: 'button', itemId: "btnClearBonus", tooltip: "Clear Bonus", text: "X", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                                //Может быть как ЗП за 1 день (Salary, SalaryDayMonthly), так и ЗП за месяц (SalaryFixedSalesMount)
                                { xtype: 'textfield', fieldLabel: "Продажи: фиксированный оклад за месяц", labelWidth: 300, name: "SalaryFixedSalesMount", id: "SalaryFixedSalesMount" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 10" },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },


                { xtype: 'container', height: 5 },


                {
                    title: "Сервисный центр",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    hidden: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: lanBonus + " (ремонт)", labelWidth: 125, flex: 1, allowBlank: true, //, emptyText: "..."
                                    //margin: "0 0 0 10",
                                    store: this.storeDirBonusesGrid, // store getting items from server
                                    valueField: 'DirBonusID',
                                    hiddenName: 'DirBonusID',
                                    displayField: 'DirBonusName',
                                    name: 'DirBonus2ID', itemId: "DirBonus2ID", id: "DirBonus2ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //disabled: true, editable: false, typeAhead: false, minChars: 200,
                                },
                                { xtype: 'button', itemId: "btnClearBonus2", tooltip: "Clear Bonus", text: "X", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                                { xtype: 'textfield', fieldLabel: "Ремонт: фиксированная суммы с каждого ремонта", labelWidth: 300, name: "SalaryFixedServiceOne", id: "SalaryFixedServiceOne" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 10" },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },


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
                                    allowBlank: true, width: 350,
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

                                { xtype: 'textfield', fieldLabel: "", labelWidth: 300, name: "SalaryPercentService1Tabs", id: "SalaryPercentService1Tabs" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

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
                                    allowBlank: true, width: 350,
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

                                { xtype: 'textfield', fieldLabel: "", labelWidth: 300, name: "SalaryPercentService2Tabs", id: "SalaryPercentService2Tabs" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false, margin: "0 0 0 10" },

                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },


                { xtype: 'container', height: 5 },


                {
                    title: "Б/У",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [

                        //фикс за аппарат или процент с работ
                        {
                            title: "Мастерская",
                            autoHeight: true,
                            xtype: 'fieldset', flex: 1, layout: 'anchor',
                            items: [

                                //начисление зп может быть по факту ремонта или после продажи аппарата (нужно сделать возм выбрать для сотрудника)
                                { xtype: "checkbox", boxLabel: "Начислить после продажи", name: "SalarySecondHandWorkshopCheck", itemId: "SalarySecondHandWorkshopCheck", flex: 1, id: "SalarySecondHandWorkshopCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0" },

                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                    items: [

                                        //процент с работ
                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: lanBonus + " (маст)", labelWidth: 125, flex: 1, allowBlank: true, //, emptyText: "..."
                                            //margin: "0 0 0 10",
                                            store: this.storeDirBonusesGrid, // store getting items from server
                                            valueField: 'DirBonusID',
                                            hiddenName: 'DirBonusID',
                                            displayField: 'DirBonusName',
                                            name: 'DirBonus3ID', itemId: "DirBonus3ID", id: "DirBonus3ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //disabled: true, editable: false, typeAhead: false, minChars: 200,
                                        },
                                        { xtype: 'button', itemId: "btnClearBonus3", tooltip: "Clear Bonus", text: "X", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        //фикс за аппарат
                                        { xtype: 'textfield', fieldLabel: "Б/У: фиксированная суммы с каждого ремонта", labelWidth: 300, name: "SalaryFixedSecondHandWorkshopOne", id: "SalaryFixedSecondHandWorkshopOne" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 10" },

                                    ]
                                },

                                { xtype: 'container', height: 5 },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            title: "Продажа",
                            autoHeight: true,
                            xtype: 'fieldset', flex: 1, layout: 'anchor',
                            items: [

                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                    items: [

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: lanBonus + " (прод)", labelWidth: 125, flex: 1, allowBlank: true, //, emptyText: "..."
                                            //margin: "0 0 0 10",
                                            store: this.storeDirBonusesGrid, // store getting items from server
                                            valueField: 'DirBonusID',
                                            hiddenName: 'DirBonusID',
                                            displayField: 'DirBonusName',
                                            name: 'DirBonus4ID', itemId: "DirBonus4ID", id: "DirBonus4ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //disabled: true, editable: false, typeAhead: false, minChars: 200,
                                        },
                                        { xtype: 'button', itemId: "btnClearBonus4", tooltip: "Clear Bonus", text: "X", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                                        { xtype: 'textfield', fieldLabel: "Б/У: фиксированная суммы с каждой продажи", labelWidth: 300, name: "SalaryFixedSecondHandRetailOne", id: "SalaryFixedSecondHandRetailOne" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 10" },

                                    ]
                                },

                                { xtype: 'container', height: 5 },
                            ]
                        },

                        { xtype: 'container', height: 5 },


                    ]
                },


            ]
        });


        //5. Passport-Panel
        var PanelPassport = Ext.create('Ext.panel.Panel', {
            id: "PanelPassport_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanPassport,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanSeries, name: "PassportSeries", id: "PassportSeries" + this.UO_id, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanNumber, margin: "0 0 0 10", name: "PassportNumber", id: "PassportNumber" + this.UO_id, flex: 1, allowBlank: true }
                    ]
                },

                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: lanPasIssued, name: "PassportIssued", id: "PassportIssued" + this.UO_id, flex: 1, allowBlank: true },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanPinCode, name: "PassportPinCode", id: "PassportPinCode" + this.UO_id, flex: 1, allowBlank: true },
                        { xtype: 'viewDateField', fieldLabel: lanIssuedDate, margin: "0 0 0 10", name: "PassportIssuedDate", id: "PassportIssuedDate" + this.UO_id, flex: 1, allowBlank: true, editable: false }
                    ]
                },

            ]
        });


        //6. RightsAccess-Panel
        var PanelRightsAccess = Ext.create('Ext.panel.Panel', {
            id: "PanelRightsAccess_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            itemId: "PanelRightsAccess_",
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanRightsAccess,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                // *** Настройки *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: red;'>Настройки</b>", name: "RightSysSettings0", itemId: "RightSysSettings0", flex: 1, id: "RightSysSettings0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightSysSettings0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20", //, fieldLabel: lanMyCompany
                            items: [
                                { xtype: "checkbox", boxLabel: lanMyCompany, name: "RightMyCompanyCheck", itemId: "RightMyCompanyCheck", flex: 1, id: "RightMyCompanyCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightMyCompanyCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightMyCompany', inputValue: 1, id: "RightMyCompany1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightMyCompany', inputValue: 2, checked: false, id: "RightMyCompany2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightMyCompany', inputValue: 3, checked: false, id: "RightMyCompany3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20", //, fieldLabel: lanEmployees
                            items: [
                                { xtype: "checkbox", boxLabel: lanEmployees, name: "RightDirEmployeesCheck", itemId: "RightDirEmployeesCheck", flex: 1, id: "RightDirEmployeesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirEmployeesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirEmployees', inputValue: 1, id: "RightDirEmployees1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirEmployees', inputValue: 2, checked: false, id: "RightDirEmployees2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirEmployees', inputValue: 3, checked: false, id: "RightDirEmployees3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20", //, fieldLabel: lanSettings
                            items: [
                                { xtype: "checkbox", boxLabel: lanSettings, name: "RightSysSettingsCheck", itemId: "RightSysSettingsCheck", flex: 1, id: "RightSysSettingsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightSysSettingsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightSysSettings', inputValue: 1, id: "RightSysSettings1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightSysSettings', inputValue: 2, checked: false, id: "RightSysSettings2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightSysSettings', inputValue: 3, checked: false, id: "RightSysSettings3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20", //, fieldLabel: lanDispJurnalDetail
                            items: [
                                { xtype: "checkbox", boxLabel: lanDispJurnalDetail, name: "RightSysJourDispsCheck", itemId: "RightSysJourDispsCheck", flex: 1, id: "RightSysJourDispsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightSysJourDispsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightSysJourDisps', inputValue: 1, id: "RightSysJourDisps1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightSysJourDisps', inputValue: 2, checked: false, id: "RightSysJourDisps2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightSysJourDisps', inputValue: 3, checked: false, id: "RightSysJourDisps3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20", //, fieldLabel: lanExchangeData
                            items: [
                                { xtype: "checkbox", boxLabel: lanExchangeData, name: "RightDataExchangeCheck", itemId: "RightDataExchangeCheck", flex: 1, id: "RightDataExchangeCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDataExchangeCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDataExchange', inputValue: 1, id: "RightDataExchange1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDataExchange', inputValue: 2, checked: false, id: "RightDataExchange2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDataExchange', inputValue: 3, checked: false, id: "RightDataExchange3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20", //, fieldLabel: lanYourData
                            items: [
                                { xtype: "checkbox", boxLabel: lanYourData, name: "RightYourDataCheck", itemId: "RightYourDataCheck", flex: 1, id: "RightYourDataCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightYourDataCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightYourData', inputValue: 1, id: "RightYourData1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightYourData', inputValue: 2, checked: false, id: "RightYourData2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightYourData', inputValue: 3, checked: false, id: "RightYourData3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20", //, fieldLabel: lanDiscPay
                            items: [
                                { xtype: "checkbox", boxLabel: lanDiscPay, name: "RightDiscPayCheck", itemId: "RightDiscPayCheck", flex: 1, id: "RightDiscPayCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDiscPayCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDiscPay', inputValue: 1, id: "RightDiscPay1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDiscPay', inputValue: 2, checked: false, id: "RightDiscPay2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDiscPay', inputValue: 3, checked: false, id: "RightDiscPay3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },




                // *** Справочники *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: blue;'>Справочникики</b>", name: "RightDir0", itemId: "RightDir0", flex: 1, id: "RightDir0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightDir0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanGoods, name: "RightDirNomensCheck", itemId: "RightDirNomensCheck", flex: 1, id: "RightDirNomensCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirNomensCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirNomens', inputValue: 1, id: "RightDirNomens1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirNomens', inputValue: 2, checked: false, id: "RightDirNomens2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirNomens', inputValue: 3, checked: false, id: "RightDirNomens3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Категория товара", name: "RightDirNomenCategoriesCheck", itemId: "RightDirNomenCategoriesCheck", flex: 1, id: "RightDirNomenCategoriesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirNomenCategoriesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirNomenCategories', inputValue: 1, id: "RightDirNomenCategories1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirNomenCategories', inputValue: 2, checked: false, id: "RightDirNomenCategories2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirNomenCategories', inputValue: 3, checked: false, id: "RightDirNomenCategories3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanContractor, name: "RightDirContractorsCheck", itemId: "RightDirContractorsCheck", flex: 1, id: "RightDirContractorsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirContractorsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirContractors', inputValue: 1, id: "RightDirContractors1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirContractors', inputValue: 2, checked: false, id: "RightDirContractors2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirContractors', inputValue: 3, checked: false, id: "RightDirContractors3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanWarehouse, name: "RightDirWarehousesCheck", itemId: "RightDirWarehousesCheck", flex: 1, id: "RightDirWarehousesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirWarehousesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirWarehouses', inputValue: 1, id: "RightDirWarehouses1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirWarehouses', inputValue: 2, checked: false, id: "RightDirWarehouses2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirWarehouses', inputValue: 3, checked: false, id: "RightDirWarehouses3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanBank, name: "RightDirBanksCheck", itemId: "RightDirBanksCheck", flex: 1, id: "RightDirBanksCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirBanksCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirBanks', inputValue: 1, id: "RightDirBanks1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirBanks', inputValue: 2, checked: false, id: "RightDirBanks2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirBanks', inputValue: 3, checked: false, id: "RightDirBanks3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanCashOffice, name: "RightDirCashOfficesCheck", itemId: "RightDirCashOfficesCheck", flex: 1, id: "RightDirCashOfficesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCashOfficesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCashOffices', inputValue: 1, id: "RightDirCashOffices1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCashOffices', inputValue: 2, checked: false, id: "RightDirCashOffices2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCashOffices', inputValue: 3, checked: false, id: "RightDirCashOffices3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanCurrency, name: "RightDirCurrenciesCheck", itemId: "RightDirCurrenciesCheck", flex: 1, id: "RightDirCurrenciesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCurrenciesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCurrencies', inputValue: 1, id: "RightDirCurrencies1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCurrencies', inputValue: 2, checked: false, id: "RightDirCurrencies2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCurrencies', inputValue: 3, checked: false, id: "RightDirCurrencies3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanVats, name: "RightDirVatsCheck", itemId: "RightDirVatsCheck", flex: 1, id: "RightDirVatsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirVatsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirVats', inputValue: 1, id: "RightDirVats1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirVats', inputValue: 2, checked: false, id: "RightDirVats2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirVats', inputValue: 3, checked: false, id: "RightDirVats3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanDiscount, name: "RightDirDiscountsCheck", itemId: "RightDirDiscountsCheck", flex: 1, id: "RightDirDiscountsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirDiscountsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirDiscounts', inputValue: 1, id: "RightDirDiscounts1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirDiscounts', inputValue: 2, checked: false, id: "RightDirDiscounts2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirDiscounts', inputValue: 3, checked: false, id: "RightDirDiscounts3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanBonus, name: "RightDirBonusesCheck", itemId: "RightDirBonusesCheck", flex: 1, id: "RightDirBonusesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirBonusesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirBonuses', inputValue: 1, id: "RightDirBonuses1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirBonuses', inputValue: 2, checked: false, id: "RightDirBonuses2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirBonuses', inputValue: 3, checked: false, id: "RightDirBonuses3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: lanBonus, name: "RightDirOrdersStatesCheck", itemId: "RightDirOrdersStatesCheck", flex: 1, id: "RightDirOrdersStatesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirOrdersStatesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirOrdersStates', inputValue: 1, id: "RightDirOrdersStates1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirOrdersStates', inputValue: 2, checked: false, id: "RightDirOrdersStates2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirOrdersStates', inputValue: 3, checked: false, id: "RightDirOrdersStates3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },


                // *** Характеристики *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: 'fieldset', width: "95%", layout: { align: 'stretch', type: 'column' }, title: "<b style='color: blue;'>Характеристики</b>", items: [] },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Цвет", name: "RightDirCharColoursCheck", itemId: "RightDirCharColoursCheck", flex: 1, id: "RightDirCharColoursCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCharColoursCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCharColours', inputValue: 1, id: "RightDirCharColours1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCharColours', inputValue: 2, checked: false, id: "RightDirCharColours2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCharColours', inputValue: 3, checked: false, id: "RightDirCharColours3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Производитель", name: "RightDirCharMaterialsCheck", itemId: "RightDirCharMaterialsCheck", flex: 1, id: "RightDirCharMaterialsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCharMaterialsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCharMaterials', inputValue: 1, id: "RightDirCharMaterials1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCharMaterials', inputValue: 2, checked: false, id: "RightDirCharMaterials2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCharMaterials', inputValue: 3, checked: false, id: "RightDirCharMaterials3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Наименование", name: "RightDirCharNamesCheck", itemId: "RightDirCharNamesCheck", flex: 1, id: "RightDirCharNamesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCharNamesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCharNames', inputValue: 1, id: "RightDirCharNames1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCharNames', inputValue: 2, checked: false, id: "RightDirCharNames2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCharNames', inputValue: 3, checked: false, id: "RightDirCharNames3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Сезон", name: "RightDirCharSeasonsCheck", itemId: "RightDirCharSeasonsCheck", flex: 1, id: "RightDirCharSeasonsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCharSeasonsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCharSeasons', inputValue: 1, id: "RightDirCharSeasons1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCharSeasons', inputValue: 2, checked: false, id: "RightDirCharSeasons2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCharSeasons', inputValue: 3, checked: false, id: "RightDirCharSeasons3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Пол", name: "RightDirCharSexesCheck", itemId: "RightDirCharSexesCheck", flex: 1, id: "RightDirCharSexesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCharSexesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCharSexes', inputValue: 1, id: "RightDirCharSexes1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCharSexes', inputValue: 2, checked: false, id: "RightDirCharSexes2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCharSexes', inputValue: 3, checked: false, id: "RightDirCharSexes3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Размер", name: "RightDirCharSizesCheck", itemId: "RightDirCharSizesCheck", flex: 1, id: "RightDirCharSizesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCharSizesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCharSizes', inputValue: 1, id: "RightDirCharSizes1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCharSizes', inputValue: 2, checked: false, id: "RightDirCharSizes2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCharSizes', inputValue: 3, checked: false, id: "RightDirCharSizes3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Поставщик", name: "RightDirCharStylesCheck", itemId: "RightDirCharStylesCheck", flex: 1, id: "RightDirCharStylesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCharStylesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCharStyles', inputValue: 1, id: "RightDirCharStyles1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCharStyles', inputValue: 2, checked: false, id: "RightDirCharStyles2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCharStyles', inputValue: 3, checked: false, id: "RightDirCharStyles3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Текстура", name: "RightDirCharTexturesCheck", itemId: "RightDirCharTexturesCheck", flex: 1, id: "RightDirCharTexturesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirCharTexturesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirCharTextures', inputValue: 1, id: "RightDirCharTextures1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirCharTextures', inputValue: 2, checked: false, id: "RightDirCharTextures2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirCharTextures', inputValue: 3, checked: false, id: "RightDirCharTextures3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },




                // *** Документы *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: green;'>Документы</b>", name: "RightDoc0", itemId: "RightDoc0", flex: 1, id: "RightDoc0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightDoc0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Приёмка", name: "RightDocPurchesCheck", itemId: "RightDocPurchesCheck", flex: 1, id: "RightDocPurchesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocPurchesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocPurches', inputValue: 1, id: "RightDocPurches1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocPurches', inputValue: 2, checked: false, id: "RightDocPurches2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocPurches', inputValue: 3, checked: false, id: "RightDocPurches3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Возврат поставщику", name: "RightDocReturnVendorsCheck", itemId: "RightDocReturnVendorsCheck", flex: 1, id: "RightDocReturnVendorsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocReturnVendorsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocReturnVendors', inputValue: 1, id: "RightDocReturnVendors1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocReturnVendors', inputValue: 2, checked: false, id: "RightDocReturnVendors2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocReturnVendors', inputValue: 3, checked: false, id: "RightDocReturnVendors3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Перемещение", name: "RightDocMovementsCheck", itemId: "RightDocMovementsCheck", flex: 1, id: "RightDocMovementsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocMovementsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocMovements', inputValue: 1, id: "RightDocMovements1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocMovements', inputValue: 2, checked: false, id: "RightDocMovements2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocMovements', inputValue: 3, checked: false, id: "RightDocMovements3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Продажа", name: "RightDocSalesCheck", itemId: "RightDocSalesCheck", flex: 1, id: "RightDocSalesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSalesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSales', inputValue: 1, id: "RightDocSales1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSales', inputValue: 2, checked: false, id: "RightDocSales2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSales', inputValue: 3, checked: false, id: "RightDocSales3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Возврат от покупателя", name: "RightDocReturnsCustomersCheck", itemId: "RightDocReturnsCustomersCheck", flex: 1, id: "RightDocReturnsCustomersCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocReturnsCustomersCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocReturnsCustomers', inputValue: 1, id: "RightDocReturnsCustomers1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocReturnsCustomers', inputValue: 2, checked: false, id: "RightDocReturnsCustomers2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocReturnsCustomers', inputValue: 3, checked: false, id: "RightDocReturnsCustomers3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Акт выполненных работ", name: "RightDocActOnWorksCheck", itemId: "RightDocActOnWorksCheck", flex: 1, id: "RightDocActOnWorksCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocActOnWorksCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocActOnWorks', inputValue: 1, id: "RightDocActOnWorks1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocActOnWorks', inputValue: 2, checked: false, id: "RightDocActOnWorks2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocActOnWorks', inputValue: 3, checked: false, id: "RightDocActOnWorks3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Счет", name: "RightDocAccountsCheck", itemId: "RightDocAccountsCheck", flex: 1, id: "RightDocAccountsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocAccountsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocAccounts', inputValue: 1, id: "RightDocAccounts1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocAccounts', inputValue: 2, checked: false, id: "RightDocAccounts2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocAccounts', inputValue: 3, checked: false, id: "RightDocAccounts3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Списание", name: "RightDocActWriteOffsCheck", itemId: "RightDocActWriteOffsCheck", flex: 1, id: "RightDocActWriteOffsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocActWriteOffsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocActWriteOffs', inputValue: 1, id: "RightDocActWriteOffs1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocActWriteOffs', inputValue: 2, checked: false, id: "RightDocActWriteOffs2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocActWriteOffs', inputValue: 3, checked: false, id: "RightDocActWriteOffs3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Инвентаризация", name: "RightDocInventoriesCheck", itemId: "RightDocInventoriesCheck", flex: 1, id: "RightDocInventoriesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocInventoriesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocInventories', inputValue: 1, id: "RightDocInventories1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocInventories', inputValue: 2, checked: false, id: "RightDocInventories2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocInventories', inputValue: 3, checked: false, id: "RightDocInventories3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Переоценка", name: "RightDocNomenRevaluationsCheck", itemId: "RightDocNomenRevaluationsCheck", flex: 1, id: "RightDocNomenRevaluationsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocNomenRevaluationsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocNomenRevaluations', inputValue: 1, id: "RightDocNomenRevaluations1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocNomenRevaluations', inputValue: 2, checked: false, id: "RightDocNomenRevaluations2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocNomenRevaluations', inputValue: 3, checked: false, id: "RightDocNomenRevaluations3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Отчет по Торговле", name: "RightReportTotalTradeCheck", itemId: "RightReportTotalTradeCheck", flex: 1, id: "RightReportTotalTradeCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportTotalTradeCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightReportTotalTrade', inputValue: 1, id: "RightReportTotalTrade1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightReportTotalTrade', inputValue: 2, checked: false, id: "RightReportTotalTrade2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightReportTotalTrade', inputValue: 3, checked: false, id: "RightReportTotalTrade3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Отчет по Торговле: Цена+Прибыль", name: "RightReportTotalTradePriceCheck", itemId: "RightReportTotalTradePriceCheck", flex: 1, id: "RightReportTotalTradePriceCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportTotalTradePriceCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightReportTotalTradePrice', inputValue: 1, id: "RightReportTotalTradePrice1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightReportTotalTradePrice', inputValue: 2, checked: false, id: "RightReportTotalTradePrice2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightReportTotalTradePrice', inputValue: 3, checked: false, id: "RightReportTotalTradePrice3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "<span style='color: blue;'>Скидки в документах<span>", name: "RightDocDescriptionCheck", itemId: "RightDocDescriptionCheck", flex: 1, id: "RightDocDescriptionCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocDescriptionCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocDescription', inputValue: 1, id: "RightDocDescription1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocDescription', inputValue: 2, checked: false, id: "RightDocDescription2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocDescription', inputValue: 3, checked: false, id: "RightDocDescription3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },



                // *** Витрина *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: green;'>Витрина</b>", name: "RightVitrina0", itemId: "RightVitrina0", flex: 1, id: "RightVitrina0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightVitrina0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Розница Чек", name: "RightDocRetailsCheck", itemId: "RightDocRetailsCheck", flex: 1, id: "RightDocRetailsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocRetailsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocRetails', inputValue: 1, id: "RightDocRetails1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocRetails', inputValue: 2, checked: false, id: "RightDocRetails2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocRetails', inputValue: 3, checked: false, id: "RightDocRetails3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Розница Возврат", name: "RightDocRetailReturnsCheck", itemId: "RightDocRetailReturnsCheck", flex: 1, id: "RightDocRetailReturnsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocRetailReturnsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocRetailReturns', inputValue: 1, id: "RightDocRetailReturns1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocRetailReturns', inputValue: 2, checked: false, id: "RightDocRetailReturns2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocRetailReturns', inputValue: 3, checked: false, id: "RightDocRetailReturns3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                


                // *** Сервис *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #9400D3;'>Сервисный центр: Документы</b>", name: "RightDocService0", itemId: "RightDocService0", flex: 1, id: "RightDocService0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightDocService0Checked' } },

                /*{
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "<span style='color: blue;'>Видит только свои ремонты (только те аппараты, которые выбрал Администратор точки)<span>", name: "RightDocServiceWorkshopsOnlyUsersCheck", itemId: "RightDocServiceWorkshopsOnlyUsersCheck", flex: 1, id: "RightDocServiceWorkshopsOnlyUsersCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServiceWorkshopsOnlyUsersCheckChecked' } },

                                //{ boxLabel: 'Редактирование', height: "35px", name: 'RightDocServiceWorkshopsOnlyUsers', inputValue: 1, id: "RightDocServiceWorkshopsOnlyUsers1" + this.UO_id, UO_id: this.UO_id },
                                //{ boxLabel: 'Только чтение', name: 'RightDocServiceWorkshopsOnlyUsers', inputValue: 2, checked: false, id: "RightDocServiceWorkshopsOnlyUsers2" + this.UO_id, UO_id: this.UO_id },
                                //{ boxLabel: 'Нет доступа', name: 'RightDocServiceWorkshopsOnlyUsers', inputValue: 3, checked: false, id: "RightDocServiceWorkshopsOnlyUsers3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },*/
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "15 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Приёмка", name: "RightDocServicePurchesCheck", itemId: "RightDocServicePurchesCheck", flex: 1, id: "RightDocServicePurchesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurchesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServicePurches', inputValue: 1, id: "RightDocServicePurches1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServicePurches', inputValue: 2, checked: false, id: "RightDocServicePurches2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServicePurches', inputValue: 3, checked: false, id: "RightDocServicePurches3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Работы", name: "RightDocServicePurch1TabsCheck", itemId: "RightDocServicePurch1TabsCheck", flex: 1, id: "RightDocServicePurch1TabsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurch1TabsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServicePurch1Tabs', inputValue: 1, id: "RightDocServicePurch1Tabs1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServicePurch1Tabs', inputValue: 2, checked: false, id: "RightDocServicePurch1Tabs2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServicePurch1Tabs', inputValue: 3, checked: false, id: "RightDocServicePurch1Tabs3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Запчасти", name: "RightDocServicePurch2TabsCheck", itemId: "RightDocServicePurch2TabsCheck", flex: 1, id: "RightDocServicePurch2TabsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurch2TabsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServicePurch2Tabs', inputValue: 1, id: "RightDocServicePurch2Tabs1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServicePurch2Tabs', inputValue: 2, checked: false, id: "RightDocServicePurch2Tabs2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServicePurch2Tabs', inputValue: 3, checked: false, id: "RightDocServicePurch2Tabs3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Мастерская", name: "RightDocServiceWorkshopsCheck", itemId: "RightDocServiceWorkshopsCheck", flex: 1, id: "RightDocServiceWorkshopsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServiceWorkshopsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServiceWorkshops', inputValue: 1, id: "RightDocServiceWorkshops1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServiceWorkshops', inputValue: 2, checked: false, id: "RightDocServiceWorkshops2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServiceWorkshops', inputValue: 3, checked: false, id: "RightDocServiceWorkshops3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Возврат на склад", name: "RightDocServiceWorkshopsTab2ReturnCheck", itemId: "RightDocServiceWorkshopsTab2ReturnCheck", flex: 1, id: "RightDocServiceWorkshopsTab2ReturnCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServiceWorkshopsTab2ReturnCheckChecked' } },

                                
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Разрешить ручное добавление работ", name: "RightDocServiceWorkshopsTab1AddCheck", itemId: "RightDocServiceWorkshopsTab1AddCheck", flex: 1, id: "RightDocServiceWorkshopsTab1AddCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServiceWorkshopsTab1AddCheckChecked' } },


                            ]
                        },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Выдача своих", name: "RightDocServiceOutputsCheck", itemId: "RightDocServiceOutputsCheck", flex: 1, id: "RightDocServiceOutputsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServiceOutputsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServiceOutputs', inputValue: 1, id: "RightDocServiceOutputs1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServiceOutputs', inputValue: 2, checked: false, id: "RightDocServiceOutputs2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServiceOutputs', inputValue: 3, checked: false, id: "RightDocServiceOutputs3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Архив", name: "RightDocServiceArchivesCheck", itemId: "RightDocServiceArchivesCheck", flex: 1, id: "RightDocServiceArchivesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServiceArchivesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServiceArchives', inputValue: 1, id: "RightDocServiceArchives1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServiceArchives', inputValue: 2, checked: false, id: "RightDocServiceArchives2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServiceArchives', inputValue: 3, checked: false, id: "RightDocServiceArchives3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Выдача чужих", name: "RightDocServicePurchesExtraditionCheck", itemId: "RightDocServicePurchesExtraditionCheck", flex: 1, id: "RightDocServicePurchesExtraditionCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurchesExtraditionCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServicePurchesExtradition', inputValue: 1, id: "RightDocServicePurchesExtradition1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServicePurchesExtradition', inputValue: 2, checked: false, id: "RightDocServicePurchesExtradition2" + this.UO_id, UO_id: this.UO_id, disabled: true, },
                                { boxLabel: 'Нет доступа', name: 'RightDocServicePurchesExtradition', inputValue: 3, checked: false, id: "RightDocServicePurchesExtradition3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Разрешать скидку", name: "RightDocServicePurchesDiscountCheck", itemId: "RightDocServicePurchesDiscountCheck", flex: 1, id: "RightDocServicePurchesDiscountCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurchesDiscountCheckChecked' } },

                                //{ boxLabel: 'Редактирование', height: "35px", name: 'RightDocServicePurchesDiscount', inputValue: 1, id: "RightDocServicePurchesDiscount1" + this.UO_id, UO_id: this.UO_id },
                                //{ boxLabel: 'Только чтение', name: 'RightDocServicePurchesDiscount', inputValue: 2, checked: false, id: "RightDocServicePurchesDiscount2" + this.UO_id, UO_id: this.UO_id, disabled: true, },
                                //{ boxLabel: 'Нет доступа', name: 'RightDocServicePurchesDiscount', inputValue: 3, checked: false, id: "RightDocServicePurchesDiscount3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                        //{ xtype: "checkbox", boxLabel: "Разрешать скидку", name: "RightDocServicePurchesDiscountCheck", itemId: "RightDocServicePurchesDiscountCheck", flex: 1, id: "RightDocServicePurchesDiscountCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurchesDiscountCheckChecked' } },
                    ]
                },
                /*{
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Видит аппараты со всех точек", name: "RightDocServicePurchesWarehouseAllCheck", itemId: "RightDocServicePurchesWarehouseAllCheck", flex: 1, id: "RightDocServicePurchesWarehouseAllCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurchesWarehouseAllCheckChecked' } },

                                //{ boxLabel: 'Редактирование', height: "35px", name: 'RightDocServicePurchesDiscount', inputValue: 1, id: "RightDocServicePurchesDiscount1" + this.UO_id, UO_id: this.UO_id },
                                //{ boxLabel: 'Только чтение', name: 'RightDocServicePurchesDiscount', inputValue: 2, checked: false, id: "RightDocServicePurchesDiscount2" + this.UO_id, UO_id: this.UO_id, disabled: true, },
                                //{ boxLabel: 'Нет доступа', name: 'RightDocServicePurchesDiscount', inputValue: 3, checked: false, id: "RightDocServicePurchesDiscount3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                        //{ xtype: "checkbox", boxLabel: "Разрешать скидку", name: "RightDocServicePurchesWarehouseAllCheck", itemId: "RightDocServicePurchesWarehouseAllCheck", flex: 1, id: "RightDocServicePurchesWarehouseAllCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurchesWarehouseAllCheckChecked' } },
                    ]
                },*/

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Перемещение", name: "RightDocServiceMovementsCheck", itemId: "RightDocServiceMovementsCheck", flex: 1, id: "RightDocServiceMovementsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServiceMovementsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServiceMovements', inputValue: 1, id: "RightDocServiceMovements1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServiceMovements', inputValue: 2, checked: false, id: "RightDocServiceMovements2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServiceMovements', inputValue: 3, checked: false, id: "RightDocServiceMovements3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Разбор", name: "RightDocServiceInventoriesCheck", itemId: "RightDocServiceInventoriesCheck", flex: 1, id: "RightDocServiceInventoriesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServiceInventoriesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServiceInventories', inputValue: 1, id: "RightDocServiceInventories1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServiceInventories', inputValue: 2, checked: false, id: "RightDocServiceInventories2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServiceInventories', inputValue: 3, checked: false, id: "RightDocServiceInventories3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },


                // *** Сервис *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: 'fieldset', width: "95%", layout: { align: 'stretch', type: 'column' }, title: "<b style='color: #9400D3;'>Сервисный центр: Отчеты</b>", items: [] },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Отчет", name: "RightDocServicePurchesReportCheck", itemId: "RightDocServicePurchesReportCheck", flex: 1, id: "RightDocServicePurchesReportCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurchesReportCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocServicePurchesReport', inputValue: 1, id: "RightDocServicePurchesReport1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocServicePurchesReport', inputValue: 2, checked: false, id: "RightDocServicePurchesReport2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocServicePurchesReport', inputValue: 3, checked: false, id: "RightDocServicePurchesReport3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },


                // *** Сервис *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: 'fieldset', width: "95%", layout: { align: 'stretch', type: 'column' }, title: "<b style='color: #9400D3;'>Сервисный центр: Справочники</b>", items: [] },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Устройства", name: "RightDirServiceNomensCheck", itemId: "RightDirServiceNomensCheck", flex: 1, id: "RightDirServiceNomensCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirServiceNomensCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirServiceNomens', inputValue: 1, id: "RightDirServiceNomens1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirServiceNomens', inputValue: 2, checked: false, id: "RightDirServiceNomens2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirServiceNomens', inputValue: 3, checked: false, id: "RightDirServiceNomens3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Категории товара", name: "RightDirServiceNomenCategoriesCheck", itemId: "RightDirServiceNomenCategoriesCheck", flex: 1, id: "RightDirServiceNomenCategoriesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirServiceNomenCategoriesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirServiceNomenCategories', inputValue: 1, id: "RightDirServiceNomenCategories1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirServiceNomenCategories', inputValue: 2, checked: false, id: "RightDirServiceNomenCategories2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirServiceNomenCategories', inputValue: 3, checked: false, id: "RightDirServiceNomenCategories3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Клиент", name: "RightDirServiceContractorsCheck", itemId: "RightDirServiceContractorsCheck", flex: 1, id: "RightDirServiceContractorsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirServiceContractorsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirServiceContractors', inputValue: 1, id: "RightDirServiceContractors1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirServiceContractors', inputValue: 2, checked: false, id: "RightDirServiceContractors2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirServiceContractors', inputValue: 3, checked: false, id: "RightDirServiceContractors3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Выполненная работа", name: "RightDirServiceJobNomensCheck", itemId: "RightDirServiceJobNomensCheck", flex: 1, id: "RightDirServiceJobNomensCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirServiceJobNomensCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirServiceJobNomens', inputValue: 1, id: "RightDirServiceJobNomens1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirServiceJobNomens', inputValue: 2, checked: false, id: "RightDirServiceJobNomens2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirServiceJobNomens', inputValue: 3, checked: false, id: "RightDirServiceJobNomens3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Шаблоны СМС", name: "RightDirSmsTemplatesCheck", itemId: "RightDirSmsTemplatesCheck", flex: 1, id: "RightDirSmsTemplatesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirSmsTemplatesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirSmsTemplates', inputValue: 1, id: "RightDirSmsTemplates1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirSmsTemplates', inputValue: 2, checked: false, id: "RightDirSmsTemplates2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirSmsTemplates', inputValue: 3, checked: false, id: "RightDirSmsTemplates3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Результаты диагностики", name: "RightDirServiceDiagnosticRresultsCheck", itemId: "RightDirServiceDiagnosticRresultsCheck", flex: 1, id: "RightDirServiceDiagnosticRresultsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirServiceDiagnosticRresultsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirServiceDiagnosticRresults', inputValue: 1, id: "RightDirServiceDiagnosticRresults1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirServiceDiagnosticRresults', inputValue: 2, checked: false, id: "RightDirServiceDiagnosticRresults2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirServiceDiagnosticRresults', inputValue: 3, checked: false, id: "RightDirServiceDiagnosticRresults3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Типовые неисправности", name: "RightDirServiceNomenTypicalFaultsCheck", itemId: "RightDirServiceNomenTypicalFaultsCheck", flex: 1, id: "RightDirServiceNomenTypicalFaultsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirServiceNomenTypicalFaultsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirServiceNomenTypicalFaults', inputValue: 1, id: "RightDirServiceNomenTypicalFaults1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirServiceNomenTypicalFaults', inputValue: 2, checked: false, id: "RightDirServiceNomenTypicalFaults2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirServiceNomenTypicalFaults', inputValue: 3, checked: false, id: "RightDirServiceNomenTypicalFaults3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Разрешить менять дату готовности", name: "RightDocServicePurchesDateDoneCheck", itemId: "RightDocServicePurchesDateDoneCheck", flex: 1, id: "RightDocServicePurchesDateDoneCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocServicePurchesDateDoneCheckChecked' } },

                            ]
                        },
                    ]
                },


                
                // *** Б/У *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #9400D3;'>Б/У: Документы</b>", name: "RightDocSecondHands0", itemId: "RightDocSecondHands0", flex: 1, id: "RightDocSecondHands0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightDocSecondHands0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Приёмка", name: "RightDocSecondHandPurchesCheck", itemId: "RightDocSecondHandPurchesCheck", flex: 1, id: "RightDocSecondHandPurchesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandPurchesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandPurches', inputValue: 1, id: "RightDocSecondHandPurches1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandPurches', inputValue: 2, checked: false, id: "RightDocSecondHandPurches2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandPurches', inputValue: 3, checked: false, id: "RightDocSecondHandPurches3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Работы", name: "RightDocSecondHandPurch1TabsCheck", itemId: "RightDocSecondHandPurch1TabsCheck", flex: 1, id: "RightDocSecondHandPurch1TabsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandPurch1TabsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandPurch1Tabs', inputValue: 1, id: "RightDocSecondHandPurch1Tabs1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandPurch1Tabs', inputValue: 2, checked: false, id: "RightDocSecondHandPurch1Tabs2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandPurch1Tabs', inputValue: 3, checked: false, id: "RightDocSecondHandPurch1Tabs3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Запчасти", name: "RightDocSecondHandPurch2TabsCheck", itemId: "RightDocSecondHandPurch2TabsCheck", flex: 1, id: "RightDocSecondHandPurch2TabsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandPurch2TabsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandPurch2Tabs', inputValue: 1, id: "RightDocSecondHandPurch2Tabs1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandPurch2Tabs', inputValue: 2, checked: false, id: "RightDocSecondHandPurch2Tabs2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandPurch2Tabs', inputValue: 3, checked: false, id: "RightDocSecondHandPurch2Tabs3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Мастерская", name: "RightDocSecondHandWorkshopsCheck", itemId: "RightDocSecondHandWorkshopsCheck", flex: 1, id: "RightDocSecondHandWorkshopsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandWorkshopsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandWorkshops', inputValue: 1, id: "RightDocSecondHandWorkshops1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandWorkshops', inputValue: 2, checked: false, id: "RightDocSecondHandWorkshops2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandWorkshops', inputValue: 3, checked: false, id: "RightDocSecondHandWorkshops3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Розница", name: "RightDocSecondHandRetailsCheck", itemId: "RightDocSecondHandRetailsCheck", flex: 1, id: "RightDocSecondHandRetailsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandRetailsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandRetails', inputValue: 1, id: "RightDocSecondHandRetails1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandRetails', inputValue: 2, checked: false, id: "RightDocSecondHandRetails2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandRetails', inputValue: 3, checked: false, id: "RightDocSecondHandRetails3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Возврат", name: "RightDocSecondHandRetailReturnsCheck", itemId: "RightDocSecondHandRetailReturnsCheck", flex: 1, id: "RightDocSecondHandRetailReturnsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandRetailReturnsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandRetailReturns', inputValue: 1, id: "RightDocSecondHandRetailReturns1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandRetailReturns', inputValue: 2, checked: false, id: "RightDocSecondHandRetailReturns2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandRetailReturns', inputValue: 3, checked: false, id: "RightDocSecondHandRetailReturns3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Списание", name: "RightDocSecondHandRetailActWriteOffsCheck", itemId: "RightDocSecondHandRetailActWriteOffsCheck", flex: 1, id: "RightDocSecondHandRetailActWriteOffsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandRetailActWriteOffsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandRetailActWriteOffs', inputValue: 1, id: "RightDocSecondHandRetailActWriteOffs1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandRetailActWriteOffs', inputValue: 2, checked: false, id: "RightDocSecondHandRetailActWriteOffs2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandRetailActWriteOffs', inputValue: 3, checked: false, id: "RightDocSecondHandRetailActWriteOffs3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Перемещение", name: "RightDocSecondHandMovementsCheck", itemId: "RightDocSecondHandMovementsCheck", flex: 1, id: "RightDocSecondHandMovementsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandMovementsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandMovements', inputValue: 1, id: "RightDocSecondHandMovements1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandMovements', inputValue: 2, checked: false, id: "RightDocSecondHandMovements2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandMovements', inputValue: 3, checked: false, id: "RightDocSecondHandMovements3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Отчет", name: "RightDocSecondHandsReportCheck", itemId: "RightDocSecondHandsReportCheck", flex: 1, id: "RightDocSecondHandsReportCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandsReportCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandsReport', inputValue: 1, id: "RightDocSecondHandsReport1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandsReport', inputValue: 2, checked: false, id: "RightDocSecondHandsReport2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandsReport', inputValue: 3, checked: false, id: "RightDocSecondHandsReport3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Инвентаризация", name: "RightDocSecondHandInventoriesCheck", itemId: "RightDocSecondHandInventoriesCheck", flex: 1, id: "RightDocSecondHandInventoriesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandInventoriesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandInventories', inputValue: 1, id: "RightDocSecondHandInventories1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение + подпись', name: 'RightDocSecondHandInventories', inputValue: 2, checked: false, id: "RightDocSecondHandInventories2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandInventories', inputValue: 3, checked: false, id: "RightDocSecondHandInventories3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Разбор", name: "RightDocSecondHandRazborsCheck", itemId: "RightDocSecondHandRazborsCheck", flex: 1, id: "RightDocSecondHandRazborsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocSecondHandRazborsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocSecondHandRazbors', inputValue: 1, id: "RightDocSecondHandRazbors1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocSecondHandRazbors', inputValue: 2, checked: false, id: "RightDocSecondHandRazbors2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocSecondHandRazbors', inputValue: 3, checked: false, id: "RightDocSecondHandRazbors3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },



                // *** Заказы *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #ffbf00;'>Заказы</b>", name: "RightDocOrderInt0", itemId: "RightDocOrderInt0", flex: 1, id: "RightDocOrderInt0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightDocOrderInt0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Список", name: "RightDocOrderIntsNewCheck", itemId: "RightDocOrderIntsNewCheck", flex: 1, id: "RightDocOrderIntsNewCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocOrderIntsNewCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocOrderIntsNew', inputValue: 1, id: "RightDocOrderIntsNew1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocOrderIntsNew', inputValue: 2, checked: false, id: "RightDocOrderIntsNew2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocOrderIntsNew', inputValue: 3, checked: false, id: "RightDocOrderIntsNew3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Новый", name: "RightDocOrderIntsCheck", itemId: "RightDocOrderIntsCheck", flex: 1, id: "RightDocOrderIntsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocOrderIntsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocOrderInts', inputValue: 1, id: "RightDocOrderInts1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocOrderInts', inputValue: 2, checked: false, id: "RightDocOrderInts2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocOrderInts', inputValue: 3, checked: false, id: "RightDocOrderInts3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Отчет", name: "RightDocOrderIntsReportCheck", itemId: "RightDocOrderIntsReportCheck", flex: 1, id: "RightDocOrderIntsReportCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocOrderIntsReportCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocOrderIntsReport', inputValue: 1, id: "RightDocOrderIntsReport1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocOrderIntsReport', inputValue: 2, checked: false, id: "RightDocOrderIntsReport2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocOrderIntsReport', inputValue: 3, checked: false, id: "RightDocOrderIntsReport3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Архив", name: "RightDocOrderIntsArchiveCheck", itemId: "RightDocOrderIntsArchiveCheck", flex: 1, id: "RightDocOrderIntsArchiveCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocOrderIntsArchiveCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocOrderIntsArchive', inputValue: 1, id: "RightDocOrderIntsArchive1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocOrderIntsArchive', inputValue: 2, checked: false, id: "RightDocOrderIntsArchive2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocOrderIntsArchive', inputValue: 3, checked: false, id: "RightDocOrderIntsArchive3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Архив", name: "RightDirOrderIntContractorsCheck", itemId: "RightDirOrderIntContractorsCheck", flex: 1, id: "RightDirOrderIntContractorsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirOrderIntContractorsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirOrderIntContractors', inputValue: 1, id: "RightDirOrderIntContractors1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirOrderIntContractors', inputValue: 2, checked: false, id: "RightDirOrderIntContractors2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirOrderIntContractors', inputValue: 3, checked: false, id: "RightDirOrderIntContractors3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },




                // *** Деньги *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #ffbf00;'>Финансы: Банк и Касса</b>", name: "RightDocBankCash0", itemId: "RightDocBankCash0", flex: 1, id: "RightDocBankCash0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightDocBankCash0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Банк", name: "RightDocBankSumsCheck", itemId: "RightDocBankSumsCheck", flex: 1, id: "RightDocBankSumsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocBankSumsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocBankSums', inputValue: 1, id: "RightDocBankSums1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocBankSums', inputValue: 2, checked: false, id: "RightDocBankSums2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocBankSums', inputValue: 3, checked: false, id: "RightDocBankSums3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Перемещение денег в кассе", name: "RightDocCashOfficeSumMovementsCheck", itemId: "RightDocCashOfficeSumMovementsCheck", flex: 1, id: "RightDocCashOfficeSumMovementsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocCashOfficeSumMovementsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocCashOfficeSumMovements', inputValue: 1, id: "RightDocCashOfficeSumMovements1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocCashOfficeSumMovements', inputValue: 2, checked: false, id: "RightDocCashOfficeSumMovements2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocCashOfficeSumMovements', inputValue: 3, checked: false, id: "RightDocCashOfficeSumMovements3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Касса", name: "RightDocCashOfficeSumsCheck", itemId: "RightDocCashOfficeSumsCheck", flex: 1, id: "RightDocCashOfficeSumsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocCashOfficeSumsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocCashOfficeSums', inputValue: 1, id: "RightDocCashOfficeSums1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocCashOfficeSums', inputValue: 2, checked: false, id: "RightDocCashOfficeSums2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocCashOfficeSums', inputValue: 3, checked: false, id: "RightDocCashOfficeSums3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Отчет", name: "RightReportBanksCashOfficesCheck", itemId: "RightReportBanksCashOfficesCheck", flex: 1, id: "RightReportBanksCashOfficesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportBanksCashOfficesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightReportBanksCashOffices', inputValue: 1, id: "RightReportBanksCashOffices1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightReportBanksCashOffices', inputValue: 2, checked: false, id: "RightReportBanksCashOffices2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightReportBanksCashOffices', inputValue: 3, checked: false, id: "RightReportBanksCashOffices3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },



                // *** Зарплата *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #ffbf00;'>Зарплата</b>", name: "RightSalaries0", itemId: "RightSalaries0", flex: 1, id: "RightSalaries0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightSalaries0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Зарплата (по сотрудникам)", name: "RightReportSalariesCheck", itemId: "RightReportSalariesCheck", flex: 1, id: "RightReportSalariesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportSalariesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightReportSalaries', inputValue: 1, id: "RightReportSalaries1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightReportSalaries', inputValue: 2, checked: false, id: "RightReportSalaries2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightReportSalaries', inputValue: 3, checked: false, id: "RightReportSalaries3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Зарплата (по точкам)", name: "RightReportSalariesWarehousesCheck", itemId: "RightReportSalariesWarehousesCheck", flex: 1, id: "RightReportSalariesWarehousesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportSalariesWarehousesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightReportSalariesWarehouses', inputValue: 1, id: "RightReportSalariesWarehouses1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightReportSalariesWarehouses', inputValue: 2, checked: false, id: "RightReportSalariesWarehouses2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightReportSalariesWarehouses', inputValue: 3, checked: false, id: "RightReportSalariesWarehouses3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Справочник: Выплаты", name: "RightDirDomesticExpensesCheck", itemId: "RightDirDomesticExpensesCheck", flex: 1, id: "RightDirDomesticExpensesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirDomesticExpensesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirDomesticExpenses', inputValue: 1, id: "RightDirDomesticExpenses1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirDomesticExpenses', inputValue: 2, checked: false, id: "RightDirDomesticExpenses2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirDomesticExpenses', inputValue: 3, checked: false, id: "RightDirDomesticExpenses3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Выплаты ЗП", name: "RightDocDomesticExpenseSalariesCheck", itemId: "RightDocDomesticExpenseSalariesCheck", flex: 1, id: "RightDocDomesticExpenseSalariesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocDomesticExpenseSalariesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocDomesticExpenseSalaries', inputValue: 1, id: "RightDocDomesticExpenseSalaries1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocDomesticExpenseSalaries', inputValue: 2, checked: false, id: "RightDocDomesticExpenseSalaries2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocDomesticExpenseSalaries', inputValue: 3, checked: false, id: "RightDocDomesticExpenseSalaries3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Выплаты другое", name: "RightDocDomesticExpensesCheck", itemId: "RightDocDomesticExpensesCheck", flex: 1, id: "RightDocDomesticExpensesCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocDomesticExpensesCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocDomesticExpenses', inputValue: 1, id: "RightDocDomesticExpenses1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocDomesticExpenses', inputValue: 2, checked: false, id: "RightDocDomesticExpenses2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocDomesticExpenses', inputValue: 3, checked: false, id: "RightDocDomesticExpenses3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Видит ЗП всех сотрудников", name: "RightReportSalariesEmplCheck", itemId: "RightReportSalariesEmplCheck", flex: 1, id: "RightReportSalariesEmplCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportSalariesEmplCheckChecked' } },

                            ]
                        },
                    ]
                },




                // *** Логистика *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #ffbf00;'>Логистика</b>", name: "RightLogistics0", itemId: "RightLogistics0", flex: 1, id: "RightLogistics0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightLogistics0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Перемещение для Курьера", name: "RightDocMovementsLogisticsCheck", itemId: "RightDocMovementsLogisticsCheck", flex: 1, id: "RightDocMovementsLogisticsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocMovementsLogisticsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocMovementsLogistics', inputValue: 1, id: "RightDocMovementsLogistics1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocMovementsLogistics', inputValue: 2, checked: false, id: "RightDocMovementsLogistics2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocMovementsLogistics', inputValue: 3, checked: false, id: "RightDocMovementsLogistics3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },




                // *** Аналитика *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #ffbf00;'>Аналитика</b>", name: "RightAnalitics0", itemId: "RightAnalitics0", flex: 1, id: "RightAnalitics0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightAnalitics0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "по филиалу", name: "Right1AnaliticsCheck", itemId: "Right1AnaliticsCheck", flex: 1, id: "Right1AnaliticsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRight1AnaliticsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'Right1Analitics', inputValue: 1, id: "Right1Analitics1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'Right1Analitics', inputValue: 2, checked: false, id: "Right1Analitics2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'Right1Analitics', inputValue: 3, checked: false, id: "Right1Analitics3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "по сотруднику", name: "Right2AnaliticsCheck", itemId: "Right2AnaliticsCheck", flex: 1, id: "Right2AnaliticsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRight2AnaliticsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'Right2Analitics', inputValue: 1, id: "Right2Analitics1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'Right2Analitics', inputValue: 2, checked: false, id: "Right2Analitics2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'Right2Analitics', inputValue: 3, checked: false, id: "Right2Analitics3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },




                // *** Отчеты *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #008B8B;'>Отчеты</b>", name: "RightReport0", itemId: "RightReport0", flex: 1, id: "RightReport0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightReport0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Прайс-лист", name: "RightReportPriceListCheck", itemId: "RightReportPriceListCheck", flex: 1, id: "RightReportPriceListCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportPriceListCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightReportPriceList', inputValue: 1, id: "RightReportPriceList1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightReportPriceList', inputValue: 2, checked: false, id: "RightReportPriceList2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightReportPriceList', inputValue: 3, checked: false, id: "RightReportPriceList3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Остатки", name: "RightReportRemnantsCheck", itemId: "RightReportRemnantsCheck", flex: 1, id: "RightReportRemnantsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportRemnantsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightReportRemnants', inputValue: 1, id: "RightReportRemnants1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightReportRemnants', inputValue: 2, checked: false, id: "RightReportRemnants2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightReportRemnants', inputValue: 3, checked: false, id: "RightReportRemnants3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Прибыль", name: "RightReportProfitCheck", itemId: "RightReportProfitCheck", flex: 1, id: "RightReportProfitCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightReportProfitCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightReportProfit', inputValue: 1, id: "RightReportProfit1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightReportProfit', inputValue: 2, checked: false, id: "RightReportProfit2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightReportProfit', inputValue: 3, checked: false, id: "RightReportProfit3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Коды товара в Приходе", name: "RightDocPurchesPrintCodeCheck", itemId: "RightDocPurchesPrintCodeCheck", flex: 1, id: "RightDocPurchesPrintCodeCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDocPurchesPrintCodeCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDocPurchesPrintCode', inputValue: 1, id: "RightDocPurchesPrintCode1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDocPurchesPrintCode', inputValue: 2, checked: false, id: "RightDocPurchesPrintCode2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDocPurchesPrintCode', inputValue: 3, checked: false, id: "RightDocPurchesPrintCode3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },




                // *** ККМ *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #008B8B;'>ККМ</b>", name: "RightKKM0", itemId: "RightKKM0", flex: 1, id: "RightKKM0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightKKM0Checked' } },

                //X-отчет
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "X-отчет", name: "RightKKMXReportCheck", itemId: "RightKKMXReportCheck", flex: 1, id: "RightKKMXReportCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMXReportCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMXReport', inputValue: 1, id: "RightKKMXReport1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMXReport', inputValue: 2, checked: false, id: "RightKKMXReport2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMXReport', inputValue: 3, checked: false, id: "RightKKMXReport3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                //Открытие смены
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Открытие смены", name: "RightKKMOpenCheck", itemId: "RightKKMOpenCheck", flex: 1, id: "RightKKMOpenCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMOpenCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMOpen', inputValue: 1, id: "RightKKMOpen1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMOpen', inputValue: 2, checked: false, id: "RightKKMOpen2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMOpen', inputValue: 3, checked: false, id: "RightKKMOpen3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                //Инкассация денег из кассы
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Инкассация денег из кассы", name: "RightKKMEncashmentCheck", itemId: "RightKKMEncashmentCheck", flex: 1, id: "RightKKMEncashmentCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMEncashmentCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMEncashment', inputValue: 1, id: "RightKKMEncashment1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMEncashment', inputValue: 2, checked: false, id: "RightKKMEncashment2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMEncashment', inputValue: 3, checked: false, id: "RightKKMEncashment3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                //Внесение денег в кассу
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Внесение денег в кассу", name: "RightKKMAddingCheck", itemId: "RightKKMAddingCheck", flex: 1, id: "RightKKMAddingCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMAddingCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMAdding', inputValue: 1, id: "RightKKMAdding1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMAdding', inputValue: 2, checked: false, id: "RightKKMAdding2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMAdding', inputValue: 3, checked: false, id: "RightKKMAdding3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                //Закрытие смены
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Закрытие смены", name: "RightKKMCloseCheck", itemId: "RightKKMCloseCheck", flex: 1, id: "RightKKMCloseCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMCloseCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMClose', inputValue: 1, id: "RightKKMClose1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMClose', inputValue: 2, checked: false, id: "RightKKMClose2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMClose', inputValue: 3, checked: false, id: "RightKKMClose3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                //Печать состояния расчетов и связи с ОФД
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Печать состояния расчетов и связи с ОФД", name: "RightKKMPrintOFDCheck", itemId: "RightKKMPrintOFDCheck", flex: 1, id: "RightKKMPrintOFDCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMPrintOFDCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMPrintOFD', inputValue: 1, id: "RightKKMPrintOFD1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMPrintOFD', inputValue: 2, checked: false, id: "RightKKMPrintOFD2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMPrintOFD', inputValue: 3, checked: false, id: "RightKKMPrintOFD3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                //Получить данные последнего чека из ФН.
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Получить данные последнего чека из ФН", name: "RightKKMCheckLastFNCheck", itemId: "RightKKMCheckLastFNCheck", flex: 1, id: "RightKKMCheckLastFNCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMCheckLastFNCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMCheckLastFN', inputValue: 1, id: "RightKKMCheckLastFN1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMCheckLastFN', inputValue: 2, checked: false, id: "RightKKMCheckLastFN2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMCheckLastFN', inputValue: 3, checked: false, id: "RightKKMCheckLastFN3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                //Получить текущее состояние ККТ
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Получить текущее состояние ККТ", name: "RightKKMStateCheck", itemId: "RightKKMStateCheck", flex: 1, id: "RightKKMStateCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMStateCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMState', inputValue: 1, id: "RightKKMState1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMState', inputValue: 2, checked: false, id: "RightKKMState2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMState', inputValue: 3, checked: false, id: "RightKKMState3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                //Получение списка ККМ
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Получение списка ККМ", name: "RightKKMListCheck", itemId: "RightKKMListCheck", flex: 1, id: "RightKKMListCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightKKMListCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightKKMList', inputValue: 1, id: "RightKKMList1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightKKMList', inputValue: 2, checked: false, id: "RightKKMList2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightKKMList', inputValue: 3, checked: false, id: "RightKKMList3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },





                // *** Другое *** *** *** *** *** *** *** *** *** *** *** ***
                { xtype: "checkbox", boxLabel: "<b style='color: #8B7500;'>Другое</b>", name: "RightOther0", itemId: "RightOther0", flex: 1, id: "RightOther0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, listeners: { change: 'onRightOther0Checked' } },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "Дизайнер ПФ", name: "RightDevelopCheck", itemId: "RightDevelopCheck", flex: 1, id: "RightDevelopCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDevelopCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDevelop', inputValue: 1, id: "RightDevelop1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDevelop', inputValue: 2, checked: false, id: "RightDevelop2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDevelop', inputValue: 3, checked: false, id: "RightDevelop3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "API 1.0", name: "RightAPI10sCheck", itemId: "RightAPI10sCheck", flex: 1, id: "RightAPI10sCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightAPI10sCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightAPI10s', inputValue: 1, id: "RightAPI10s1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightAPI10s', inputValue: 2, checked: false, id: "RightAPI10s2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightAPI10s', inputValue: 3, checked: false, id: "RightAPI10s3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'radiogroup', allowBlank: true, flex: 1, cls: 'x-check-group-alt', margin: "0 0 0 20",
                            items: [
                                { xtype: "checkbox", boxLabel: "API 1.0", name: "RightDirWebShopUOsCheck", itemId: "RightDirWebShopUOsCheck", flex: 1, id: "RightDirWebShopUOsCheck" + this.UO_id, inputValue: true, UO_id: this.UO_id, margin: "0 5 0 0", readOnly: true, listeners: { change: 'onRightDirWebShopUOsCheckChecked' } },

                                { boxLabel: 'Редактирование', height: "35px", name: 'RightDirWebShopUOs', inputValue: 1, id: "RightDirWebShopUOs1" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Только чтение', name: 'RightDirWebShopUOs', inputValue: 2, checked: false, id: "RightDirWebShopUOs2" + this.UO_id, UO_id: this.UO_id },
                                { boxLabel: 'Нет доступа', name: 'RightDirWebShopUOs', inputValue: 3, checked: false, id: "RightDirWebShopUOs3" + this.UO_id, UO_id: this.UO_id }
                            ]
                        },
                    ]
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
                PanelGeneral, PanelAccount, PanelWarehouses, PanelSalary, PanelRightsAccess //, PanelPassport
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
        var formPanelEdit = Ext.create('Ext.form.Panel', {
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

            //region: "center",
            width: "100%", height: "100%",
            bodyPadding: 5,
            layout: 'fit',
            defaults: {
                anchor: '100%'
            },
            autoHeight: true,

            items: [
                tabPanel
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

                "-",

                {
                    id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHistory",
                    text: lanHistory, icon: '../Scripts/sklad/images/history.png',
                    disabled: true
                },

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
                    id: "DirEmployee"
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
                        contextMenuTree.folderNew = controllerDirEmployees_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirEmployees_onTree_folderNewSub;
                        contextMenuTree.folderCopy = controllerDirEmployees_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirEmployees_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirEmployees_onTree_folderSubNull;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }
                */

            }),


            // *** *** *** *** *** *** *** *** ***


            formPanelEdit

        ],


        this.callParent(arguments);
    }

});

