Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocServicePurches/viewDocServiceMovsEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocServiceMovsEdit",

    layout: "border",
    region: "center",
    title: "Перемещение",
    width: 900, height: 550,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocServiceMovsEdit',
    //listeners: { close: 'onViewDocServiceRetailsEditClose' },

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {

        //Создать 3-и панели:
        // Вверху      - Партии
        // По середине - Основное
        // Внизу       - Спецификация
        //И есть Сплитер


        //Панель
        var PanelSearch = Ext.create('Ext.panel.Panel', {
            id: "PanelSearch_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',

            defaults: { anchor: '100%' },
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%",
            autoScroll: true,
            //split: true,

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'textfield', margin: "0 0 0 15", name: "DirNomenPatchFull", id: "DirNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true }, //, fieldLabel: ""
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload",
                            id: "btnDirServiceNomenReload" + this.UO_id, itemId: "btnDirServiceNomenReload",
                            listeners: { click: "onBtnDirServiceNomenReloadClick" }
                        },

                        {
                            xtype: 'viewComboBox',
                            allowBlank: true, flex: 1,

                            store: new Ext.data.SimpleStore({
                                fields: ['SearchType', 'SearchTypeName'],
                                data: SearchType_values
                            }),

                            valueField: 'SearchType',
                            hiddenName: 'SearchType',
                            displayField: 'SearchTypeName',
                            name: 'SearchType', itemId: "SearchType", id: "SearchType" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                        //"->",
                        {
                            id: "TriggerSearchTree" + this.UO_id,
                            UO_id: this.UO_id,
                            UO_idMain: this.UO_idMain,
                            UO_idCall: this.UO_idCall,

                            xtype: 'viewTriggerSearch',
                            emptyText: "Поиск ...", allowBlank: true, flex: 1,
                            name: 'TriggerSearchTree', id: "TriggerSearchTree" + this.UO_id, itemId: "TriggerSearchTree",
                            listeners: { ontriggerclick: "onTriggerSearchTreeClick1", specialkey: "onTriggerSearchTreeClick2", change: "onTriggerSearchTreeClick3" }
                        },
                    ]
                },

            ]
        });


        //2. Партии
        var PanelParty = Ext.create('widget.viewGridService', {

            conf: {
                id: "gridParty_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            itemId: "gridParty",
            title: "Партии товара",
            collapsible: true,
            autoScroll: true,
            flex: 1,

            listeners: { selectionchange: 'onGridParty_selectionchange' },

            store: this.storeDocServicePurchesGrid,

        });



        //Tab
        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

        var PanelDocumentDetails = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentDetails_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            
            defaults: { anchor: '100%' },
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%", height: 120 + varBodyPadding,
            //autoScroll: true,
            //split: true,

            items: [

                // !!! Не видимые !!!
                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },  //, hidden: true
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirMovementStatusID", name: "DirMovementStatusID", id: "DirMovementStatusID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", name: "DocServiceMovID", id: "DocServiceMovID" + this.UO_id, readOnly: true, width: 150, allowBlank: true, labelAlign: "top" },
                        { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", width: 150, allowBlank: true, hidden: true, labelAlign: "top" },
                        { xtype: 'viewDateField', fieldLabel: lanDateCounterparty, name: "DocDate", id: "DocDate" + this.UO_id, width: 150, margin: "0 0 0 5", allowBlank: false, editable: false, labelAlign: "top" },


                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouseFrom, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 15",
                            store: this.storeDirWarehousesGridFrom, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseIDFrom', itemId: "DirWarehouseIDFrom", id: "DirWarehouseIDFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                            labelAlign: "top"
                        },
                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirWarehouseEditFrom", id: "btnDirWarehouseEditFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirWarehouseReloadFrom", id: "btnDirWarehouseReloadFrom" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouseTo, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirWarehousesGridTo, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseIDTo', itemId: "DirWarehouseIDTo", id: "DirWarehouseIDTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                            labelAlign: "top"
                        },
                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirWarehouseEditTo", id: "btnDirWarehouseEditTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirWarehouseReloadTo", id: "btnDirWarehouseReloadTo" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, hidden: true,
                    items: [
                        
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", flex: 1, allowBlank: false, //, emptyText: "..."
                            store: this.storeDirContractorsOrgGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },

                        { xtype: "checkbox", boxLabel: lanReserve, margin: "0 0 0 5", name: "Reserve", itemId: "Reserve", flex: 1, id: "Reserve" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true },
                    ]
                },

                //{ xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [



                    ]
                },

                { xtype: 'container', height: 5 },


                //Причина и Основание
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [

                        //Причина
                        {
                            xtype: 'viewComboBox',
                            flex: 1, allowBlank: true, fieldLabel: "Причина",
                            
                            store: this.storeDirMovementDescriptionsGrid, // store getting items from server
                            valueField: 'DirMovementDescriptionName',
                            hiddenName: 'DirMovementDescriptionID',
                            displayField: 'DirMovementDescriptionName',
                            name: 'DescriptionMovement', itemId: "DescriptionMovement", id: "DescriptionMovement" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: true, typeAhead: true, minChars: 2
                        },

                        //Основание
                        { xtype: 'textfield', fieldLabel: lanBase, name: "Base", id: "Base" + this.UO_id, flex: 1, allowBlank: true, margin: "0 0 0 5", },

                        //Курьер
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Курьер", flex: 1, allowBlank: true, //, emptyText: "..."
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
                        {
                            xtype: 'button', text: "X", tooltip: "Очистить", itemId: "btnDirEmployeeIDCourier", id: "btnDirEmployeeIDCourier" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            listeners: { click: "onBtnDirEmployeeIDCourierClick" }
                        },
                    ]
                },

            ]
        });

        var PanelDocumentAdditionally = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentAdditionally_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanAdditionally,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%", height: 95 + varBodyPadding,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [
                //{ xtype: 'textfield', fieldLabel: lanBase, name: "Base", id: "Base" + this.UO_id, flex: 1, allowBlank: true },
                //{ xtype: 'textfield', fieldLabel: lanDisc, name: "Description", id: "Description" + this.UO_id, flex: 1, allowBlank: true }
            ]
        });
        

        //Tab-Panel
        var tabPanelDetails = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            items: [
                //PanelDocumentDetails, PanelDocumentAdditionally
            ]

        });

        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 
        


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
            split: true,

            store: this.storeGrid, //storeDocServiceMovTabsGrid,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [
            
                //Документ БУ
                { text: "Документ", dataIndex: "DocServicePurchID", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },

                //Товар
                { text: "Товар №", dataIndex: "DirServiceNomenID", width: 50, style: "height: 25px;", hidden: true },
                { text: lanNomenclature, dataIndex: "DirServiceNomenName", flex: 1 },
                //К-во
                { text: "Статус", dataIndex: "DirServiceStatusName", flex: 1 },
                { text: "Серийный", dataIndex: "SerialNumber", flex: 1 },
                { text: "Контрагент", dataIndex: "DirServiceContractorName", flex: 1 },
                { text: "Телефон", dataIndex: "DirServiceContractorPhone", flex: 1 },
                { text: "Ориентир.стоимость", dataIndex: "PriceVAT", width: 150 },
                { text: "Предоплата", dataIndex: "PrepaymentSum", width: 150, summaryType: 'sum'  },
            ],

            tbar: [
                
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanAddPosition, tooltip: lanAddPosition,
                    itemId: "btnGridAddPosition",
                    listeners: { click: "onGrid_BtnGridAddPosition" }
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: lanEdit, tooltip: lanEdit, disabled: true,
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit",
                    listeners: { click: "onGrid_BtnGridEdit" }
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete",
                    listeners: { click: "onGrid_BtnGridDelete" }
                },

            ],


            listeners: {
                selectionchange: 'onGrid_selectionchange',
                itemclick: 'onGrid_itemclick',
                itemdblclick: 'onGrid_itemdblclick',
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
                PanelSearch,
                PanelDocumentDetails, 
                PanelParty,
                PanelGrid, //PanelFooter
            ]
        });




        //body
        this.items = [
            /*
            //Товар
            Ext.create('widget.viewTreeDirRetail', {

                conf: {
                    id: "tree_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

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

            }),
            */

            // *** *** *** *** *** *** *** *** ***


            //Шапка документа + табличная часть
            formPanel

        ],


        this.buttons = [
            {
                xtype: 'textfield',
                allowBlank: false, width: 200, fieldLabel: "Ваш пароль", //inputType: 'password',
                name: 'DirEmployeePswd', itemId: 'DirEmployeePswd', id: 'DirEmployeePswd' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                listeners: { click: "onGrid_BtnGridAddPosition" }
            },

            " ",

            {
                id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", hidden: true, style: "width: 120px; height: 40px;",
                UO_Action: "held_cancel",
                text: lanHeldCancel, icon: '../Scripts/sklad/images/save_held.png',
                listeners: { click: "onBtnHeldCancelClick" }
            },
            {
                id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", hidden: true, style: "width: 120px; height: 40px;",
                UO_Action: "held",
                text: lanHelds, icon: '../Scripts/sklad/images/save_held.png',
                listeners: { click: "onBtnHeldsClick" }
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
                        listeners: { click: "onBtnSaveClick" }
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSaveClose",
                        UO_Action: "save_close",
                        text: lanRecordClose, icon: '../Scripts/sklad/images/save.png',
                        listeners: { click: "onBtnSaveClick" }
                    }
                ]
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", style: "width: 120px; height: 40px;",
                text: lanCancel, icon: '../Scripts/sklad/images/cancel.png',
                listeners: { click: "onBtnCancelClick" }
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
                        listeners: { click: "onBtnPrintHtmlClick" }
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintExcel",
                        UO_Action: "excel",
                        text: "MS Excel", icon: '../Scripts/sklad/images/excel.png',
                        listeners: { click: "onBtnPrintHtmlClick" }
                    }
                ]
            },
            "-",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp", style: "width: 120px; height: 40px;",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png',
                listeners: { click: "onBtnHelpClick" }
            },
            "->",
            { xtype: "label", text: "Внимание: Если выбрать курьера, то документ попадёт в Логистику!", style: 'color: red; font-weight: bold;' }

        ],


        this.callParent(arguments);
    }

});

