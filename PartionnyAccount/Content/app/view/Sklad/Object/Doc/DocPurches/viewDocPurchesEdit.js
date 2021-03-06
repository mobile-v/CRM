﻿Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocPurches/viewDocPurchesEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocPurchesEdit",

    layout: "border",
    region: "center",
    title: "Поступление",
    width: 900, height: 550,
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

        //Создать 3-и панели:
        // Вверху      - Партии
        // По середине - Основное
        // Внизу       - Спецификация
        //И есть Сплитер


        //Панель
        /*var SearchType_values = [
            [1, 'В товаре (код, наименование)'],
            [2, 'В партиях (серии, штрих-код)'],
        ];*/
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
            width: "100%",
            autoScroll: true,

            //width: "100%", height: "100%", //width: 500, height: 200,
            //split: true,

            /*
            defaults: { anchor: '100%' },
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%",
            autoScroll: true,
            //split: true,
            */

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', name: "DirNomenPatchFull", id: "DirNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true }, //, fieldLabel: ""
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload",
                            id: "btnDirNomenReload" + this.UO_id, itemId: "btnDirNomenReload"
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
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        //"->",
                        {
                            id: "TriggerSearchTree" + this.UO_id,
                            UO_id: this.UO_id,
                            UO_idMain: this.UO_idMain,
                            UO_idCall: this.UO_idCall,

                            xtype: 'viewTriggerSearch',
                            emptyText: "Поиск ...", allowBlank: true, flex: 1,
                            name: 'TriggerSearchTree', id: "TriggerSearchTree" + this.UO_id, itemId: "TriggerSearchTree"
                        }
                    ]
                },

            ]
        });


        //Tab
        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

        var PanelDocumentDetails = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentDetails_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            //bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            width: "100%", height: 50 + varBodyPadding, //width: "100%", height: 115 + varBodyPadding,
            autoScroll: true,
            //split: true,

            items: [

                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },  //, hidden: true
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    hidden: true,
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", name: "DocPurchID", id: "DocPurchID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    hidden: true,
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
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'viewDateField', fieldLabel: lanDateCounterparty, name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", allowBlank: false, editable: false },

                        //DirContractor
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanContractor, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirContractorsGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorID', itemId: "DirContractorID", id: "DirContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirContractorEdit", id: "btnDirContractorEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirContractorReload", id: "btnDirContractorReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouse, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirWarehousesGrid, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirWarehouseEdit", id: "btnDirWarehouseEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirWarehouseReload", id: "btnDirWarehouseReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
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
            width: "100%", height: 115 + varBodyPadding,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            hidden: true,

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanWaybill, name: "NumberTT", id: "NumberTT" + this.UO_id, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanTaxBill, name: "NumberTax", id: "NumberTax" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true },

                    ]
                },


                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //DirVat
                        /*{
                            xtype: 'viewComboBox',
                            fieldLabel: lanVat, flex: 1, allowBlank: false, //, emptyText: "..."

                            store: this.storeDirVatsGrid, // store getting items from server
                            valueField: 'DirVatValue',
                            hiddenName: 'DirVatValue',
                            displayField: 'DirVatValue',
                            name: 'DirVatValue', itemId: "DirVatValue", id: "DirVatValue" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: false, typeAhead: false, minChars: 200,
                        },*/
                        { xtype: 'textfield', fieldLabel: lanBase, name: "DirVatValue", id: "DirVatValue" + this.UO_id, flex: 1, allowBlank: false, hidden: true },

                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanBase, name: "Base", id: "Base" + this.UO_id, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanDisc, margin: "0 0 0 5", name: "Description", id: "Description" + this.UO_id, flex: 1, allowBlank: true }
                    ]
                }

            ]
        });


        var PanelDocumentDiscount = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentDiscount_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanDiscount,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%", height: 115 + varBodyPadding,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,
            hidden: true,

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanDiscount, itemId: "Discount", name: "Discount", id: "Discount" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', itemId: "btnSpreadDiscount", tooltip: "Раскинуть скидку по спецификации", text: "Раскинуть скидку по спецификации", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanSum, itemId: "SummOther", name: "SummOther", id: "SummOther" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', itemId: "btnSpreadSummOther", tooltip: "Изменить сумму документа", text: "Изменить сумму документа", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
                    ]
                },

            ]
        });


        //Tab-Panel - не используется!!!
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
                //PanelDocumentDetails, PanelDocumentAdditionally, PanelDocumentDiscount
            ]

        });

        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 



        var PanelParty = Ext.create('widget.viewGridRemPurch', {

            conf: {
                id: "gridParty_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View, //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            itemId: "gridParty",
            title: "Партии товара",
            collapsible: true,
            autoScroll: true,
            flex: 1,
            //split: true,
            store: this.storeRemPartiesGrid,

            /*columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 50, hidden: true }, //, hidden: true
                //Товар
                { text: "Код", dataIndex: "DirNomenID", width: 50, style: "height: 25px;" },
                { text: "Дата док", dataIndex: "DocDate", width: 75 },
                { text: "Дата приём", dataIndex: "DocDatePurches", width: 75 },

                { text: "Поставщик", dataIndex: "DirContractorName", width: 75 },
                { text: "Склад", dataIndex: "DirWarehouseName", width: 75 },
                { text: "Документ", dataIndex: "ListDocNameRu", width: 75 },

                { text: "Налог", dataIndex: "DirVatValue", width: 50, hidden: true },
                { text: "Поступило", dataIndex: "Quantity", width: 85 },
                { text: "Остаток", dataIndex: "Remnant", width: 85 },

                { text: "Цена в вал.", dataIndex: "PriceVAT", width: 85, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 50, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 50, hidden: true },
                { text: "Цена поступ.", dataIndex: "PriceCurrency", width: 85 },

                { text: "Цена розница (в вал.)", dataIndex: "PriceRetailVAT", width: 85, hidden: true },
                { text: "Цена роз", dataIndex: "PriceRetailCurrency", width: 85, hidden: true },

                { text: "Цена опт (в вал.)", dataIndex: "PriceWholesaleVAT", width: 85, hidden: true },
                { text: "Цена опт", dataIndex: "PriceWholesaleCurrency", width: 85, hidden: true },

                { text: "Цена им (в вал.)", dataIndex: "PriceIMVAT", width: 85, hidden: true },
                { text: "Цена мм", dataIndex: "PriceIMCurrency", width: 85, hidden: true },

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", width: 125 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                { text: "Стиль", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Поставщик", dataIndex: "DirContractorName", width: 100, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: lanBarCode, dataIndex: "Barcode", width: 100, hidden: true },

                //Минимальный остаток
                { text: lanMinimumBalance, dataIndex: "DirNomenMinimumBalance", width: 100, hidden: true },
            ],*/

            //В Константах "нижнея панель" не нужна
            /*bbar: new Ext.PagingToolbar({
                store: this.storeRemPartiesGrid,                      // указано хранилище
                displayInfo: true,                          // вывести инфо обо общем числе записей
                displayMsg: lanShowing + "  {0} - {1} " + lanOf + " {2}"     // формат инфо
            }),*/

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
            split: true,

            store: this.storeGrid, //storeDocPurchTabsGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 50, hidden: true },
                //Товар
                { text: "№", dataIndex: "DirNomenID", width: 50, style: "height: 25px;" },
                { text: lanNomenclature, dataIndex: "DirNomenName", flex: 2 },
                //К-во
                { text: lanCount, dataIndex: "Quantity", width: 75 },
                //Цены
                { text: "Розница Наценка", dataIndex: "MarkupRetail", width: 100, hidden: true },
                { text: "Розница Цена", dataIndex: "PriceRetailCurrency", width: 100, hidden: true },
                { text: "Опт Наценка", dataIndex: "MarkupWholesale", width: 100, hidden: true },
                { text: "Опт Цена", dataIndex: "PriceWholesaleCurrency", width: 100, hidden: true },
                { text: "IM Наценка", dataIndex: "MarkupIM", width: 100, hidden: true },
                { text: "IM Цена", dataIndex: "PriceIMCurrency", width: 100, hidden: true },
                //Суммы
                { text: lanPriceVatFull, dataIndex: "PriceCurrency", width: 100 },
                { text: lanSum, dataIndex: "SUMPurchPriceVATCurrency", width: 100 },

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", flex: 1, hidden: true },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100 },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100 },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100 },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                //{ text: "Стиль", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Поставщик", dataIndex: "DirContractorName", width: 100 },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true },

                //Минимальный остаток
                { text: lanMinimumBalance, dataIndex: "DirNomenMinimumBalance", width: 100, hidden: true },
            ],

            tbar: [

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanAddPosition, tooltip: lanAddPosition,
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

                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/payment.png', text: lanPayments, tooltip: lanPayments, disabled: true,
                    id: "btnGridPayment" + this.UO_id, itemId: "btnGridPayment"
                },


                '-',
                { // *** Создать на основании Расход.накл *** *** *** *** *** *** *** *** *** *** ***
                    //itemId: 'OnBasisOfInvoice' + Doc_ID,
                    icon: '../Scripts/sklad/images/table_relation.png', text: lanOnBasisOfDoc + "...", tooltip: lanOnBasisOfDoc, disabled: true, id: "btnOnBasisOfDoc" + this.UO_id,
                    menu: [
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            icon: '../Scripts/sklad/images/table_relation.png', text: "создать 'Перемещение'", tooltip: "Создать 'Перемещение'",
                            id: "btnCreateDocMovements" + this.UO_id, itemId: "btnCreateDocMovements"
                        },
                    ]
                } // *** Создать на основании Расход.накл *** *** *** *** *** *** *** *** *** *** ***

            ],
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
            //split: true,

            items: [
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //Суммы
                        { xtype: 'textfield', fieldLabel: lanSumOfVAT, labelAlign: 'top', name: "SumOfVATCurrency", id: "SumOfVATCurrency" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanSumVAT, labelAlign: 'top', name: "SumVATCurrency", id: "SumVATCurrency" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },

                        //Оплачено
                        { xtype: 'textfield', fieldLabel: lanPaid, regex: /^[+\-]?\d+(?:\.\d+)?$/, margin: "0 0 0 5", flex: 1, allowBlank: false, name: "Payment", itemId: "Payment", id: "Payment" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true },

                        //Доплатить
                        { xtype: 'textfield', fieldLabel: lanHavePay, labelAlign: 'top', name: "HavePay", id: "HavePay" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
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
                PanelDocumentDetails, PanelDocumentAdditionally, PanelDocumentDiscount,
                PanelParty,
                PanelGrid, PanelFooter
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
                width: 215 + varBodyPadding,

                store: this.storeNomenTree,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirNomen"
                },


                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    //{ text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: 'Наименование', flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'Остаток', dataIndex: 'Remains', width: 50, hidden: true, tdCls: 'x-change-cell' },
                    //{ text: 'DirNomenPatchFull', dataIndex: 'DirNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],

                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDocPurchesEdit_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDocPurchesEdit_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDocPurchesEdit_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDocPurchesEdit_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDocPurchesEdit_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDocPurchesEdit_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDocPurchesEdit_onTree_addSub;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }

            }),


            // *** *** *** *** *** *** *** *** ***


            //Шапка документа + табличная часть
            formPanel

        ],


            this.buttons = [
                {
                    id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", hidden: true, style: "width: 130px; height: 40px;",
                    text: "<b style='font-size: 18px; color: red;'>" + lanHeldCancel + "</b>",  //text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanHeldCancel + "</b></font>", 
                    icon: '../Scripts/sklad/images/save_held.png',
                    UO_Action: "held_cancel",
                },
                {
                    id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", hidden: true, style: "width: 120px; height: 40px;",
                    text: "<b style='font-size: 18px; color: green;'>" + lanHelds + "</b>",  //text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanHelds + "</b></font>",
                    icon: '../Scripts/sklad/images/save_held.png',
                    UO_Action: "held",
                },
                {
                    id: "btnRecord" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true, style: "width: 140px; height: 40px;",
                    text: "<b style='font-size: 18px; color: green;'>" + lanSave + "</b>",  //text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanSave + "</b></font>",
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
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", style: "width: 120px; height: 40px;",
                    text: "<b style='font-size: 18px; color: red;'>" + lanCancel + "</b>",  //text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanCancel + "</b></font>",
                    icon: '../Scripts/sklad/images/cancel.png'
                },
                " ",
                {
                    id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint", hidden: true, style: "width: 120px; height: 40px;",
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanPrint + "</b></font>", icon: '../Scripts/sklad/images/print.png',
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
                            },
                            "-Bar-",

                            {
                                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_barcode",
                                text: "Только Штрих-Код", icon: '../Scripts/sklad/images/print.png',
                                UO_Action: "barcode",
                            },
                            {
                                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_barcode_price",
                                text: "Штрих-Код + Цена", icon: '../Scripts/sklad/images/print.png',
                                UO_Action: "barcode_price",
                            },
                            {
                                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_barcode_name",
                                text: "Штрих-Код + Наименование", icon: '../Scripts/sklad/images/print.png',
                                UO_Action: "barcode_name",
                            },

                            "-Qr-",

                            {
                                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_Qr",
                                text: "Только Штрих-Код", icon: '../Scripts/sklad/images/print.png',
                                UO_Action: "Qr",
                            },
                            {
                                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_Qr_price",
                                text: "Штрих-Код + Цена", icon: '../Scripts/sklad/images/print.png',
                                UO_Action: "Qr_price",
                            },
                            {
                                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_Qr_name",
                                text: "Штрих-Код + Наименование", icon: '../Scripts/sklad/images/print.png',
                                UO_Action: "Qr_name",
                            }
                        ]
                },
                "-",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp", style: "width: 120px; height: 40px;",
                    text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanHelp + "</b></font>", icon: '../Scripts/sklad/images/help16.png',
                }

        ],


        this.callParent(arguments);
    }

});