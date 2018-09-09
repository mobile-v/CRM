Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocRetails/viewDocRetailsEdit", {
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocRetailsEdit",

    layout: "border",
    region: "center",
    title: "Чек -> ",
    width: 900, height: 575,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocRetailsEdit',

    listeners: { close: 'onViewDocRetailsEditClose' },

    bodyStyle: 'background:white;', //bodyStyle: 'opacity:0.5;',
    bodyPadding: 5,

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
            //bodyStyle: 'background:transparent;',
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

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', name: "DirNomenPatchFull", id: "DirNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true }, //, fieldLabel: ""
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload",
                            id: "btnDirNomenReload" + this.UO_id, itemId: "btnDirNomenReload",
                            listeners: { click: "onBtnDirNomenReloadClick" }
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
                        }
                    ]
                },

            ]
        });


        var PanelParty = Ext.create('widget.viewGridRem', {
            
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
            //split: true,
            store: this.storeRemPartiesGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 85 }, //, hidden: true
                //Товар
                { text: "Код", dataIndex: "DirNomenID", width: 85 },
                { text: "Найм.", dataIndex: "DirNomenName", flex: 1 },
                { text: "Дата док", dataIndex: "DocDate", width: 75 },
                { text: "Дата приём", dataIndex: "DocDatePurches", width: 75 },

                { text: "Поставщик", dataIndex: "DirContractorName", flex: 1, hidden: true },
                { text: "Склад", dataIndex: "DirWarehouseName", flex: 1, hidden: true },
                { text: "Документ", dataIndex: "ListDocNameRu", flex: 1, hidden: true },

                { text: "Налог", dataIndex: "DirVatValue", width: 85, hidden: true },
                { text: "Поступило", dataIndex: "Quantity", width: 85, hidden: true },
                { text: "Остаток", dataIndex: "Remnant", width: 85 },
                
                //{ text: "Цена в вал.", dataIndex: "PriceVAT", width: 85, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 85, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 85, hidden: true },
                //{ text: "Цена прих", dataIndex: "PriceCurrency", width: 85, hidden: true },
                
                { text: "Цена Роз.(в вал.)", dataIndex: "PriceRetailVAT", width: 85, hidden: true },
                { text: "Цена Роз.", dataIndex: "PriceRetailCurrency", width: 85 }, //, hidden: true

                //{ text: "Цена Опт (в вал.)", dataIndex: "PriceWholeRetailVAT", width: 85, hidden: true },
                //{ text: "Цена Опт", dataIndex: "PriceWholeRetailCurrency", width: 85, hidden: true }, //, hidden: true

                //{ text: "Цена ИМ (в вал.)", dataIndex: "PriceIMVAT", width: 85, hidden: true },
                //{ text: "Цена ИМ", dataIndex: "PriceIMCurrency", width: 85, hidden: true }, //, hidden: true

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", flex: 1 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                { text: "Поставщик", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true }
            ],

            //В Константах "нижнея панель" не нужна
            /*bbar: new Ext.PagingToolbar({
                store: this.storeRemPartiesGrid,                      // указано хранилище
                displayInfo: true,                          // вывести инфо обо общем числе записей
                displayMsg: lanShowing + "  {0} - {1} " + lanOf + " {2}"     // формат инфо
            }),*/

        });



        //Tab
        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

        var PanelDocumentDetails = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentDetails_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            width: "100%",
            autoScroll: true,

            items: [

                // *** *** *** Not Visible *** *** ***
                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },  //, hidden: true
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "№", name: "DocRetailID", id: "DocRetailID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirPriceTypeID", name: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: "checkbox", boxLabel: lanReserve, margin: "0 0 0 5", name: "Reserve", itemId: "Reserve", flex: 1, id: "Reserve" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirVatValue", name: "DirVatValue", id: "DirVatValue" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                // *** *** *** Not Visible *** *** ***


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'viewDateField', fieldLabel: "Дата", name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", allowBlank: false, editable: false },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanContractor, flex: 1, allowBlank: true, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirContractorsGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorID', itemId: "DirContractorID", id: "DirContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },

                    ]
                }
            ]
        });


        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 
        


        //2. Грид
        var rowEditing = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            //bodyStyle: 'background:transparent;',
            itemId: "grid",
            listeners: { selectionchange: 'onGrid_selectionchange', edit: 'onGrid_edit' }, //, itemclick: 'onGrid_itemclick', itemdblclick: 'onGrid_itemdblclick'

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeGrid, //storeDocRetailTabsGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 75 }, //, hidden: true
                //Товар
                { text: "№", dataIndex: "DirNomenID", width: 50 },
                { text: lanNomenclature, dataIndex: "DirNomenName", flex: 1 }, //flex: 1
                //К-во
                { text: lanCount, dataIndex: "Quantity", width: 75, editor: { xtype: 'textfield' } },
                //Суммы
                { text: "Тип цены", dataIndex: "DirPriceTypeName", width: 75, hidden: true },
                { text: lanPriceSale, dataIndex: "PriceCurrency", width: 100 },
                { text: lanSum, dataIndex: "SUMSalePriceVATCurrency", width: 100 },

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", flex: 1 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 70, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 70, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 70, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 70, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 70, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 70, hidden: true },
                { text: "Поставщик", dataIndex: "DirCharStyleName", width: 70, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 70, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 70, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 70, hidden: true },
            ],
            plugins: [rowEditing],

            tbar: [

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanAddPosition, tooltip: lanAddPosition,
                    itemId: "btnGridAddPosition",
                    listeners: { click: 'onGrid_BtnGridAddPosition' }
                },
                /*
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: lanEdit, tooltip: lanEdit, disabled: true,
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit",
                    listeners: { click: 'onGrid_BtnGridEdit' }
                },
                */
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete",
                    listeners: { click: 'onGrid_BtnGridDelete' }
                },
                /*
                '-',
                { // *** Создать на основании Расход.накл *** *** *** *** *** *** *** *** *** *** ***
                    //itemId: 'OnBasisOfInvoice' + Doc_ID,
                    icon: '../Scripts/sklad/images/table_relation.png', text: lanOnBasisOfDoc + "...", tooltip: lanOnBasisOfDoc,
                    menu: [
                        { icon: '../Scripts/sklad/images/table_relation.png', text: lanCreateDocInvoice, tooltip: lanCreateDocInvoice },
                        { icon: '../Scripts/sklad/images/table_relation.png', text: lanCreateDocReturnsCustomers, tooltip: lanCreateDocReturnsCustomers }
                    ]
                } // *** Создать на основании Расход.накл *** *** *** *** *** *** *** *** *** *** ***
                */
            ],
        });


        //3. Футер
        var PanelFooter = Ext.create('Ext.panel.Panel', {
            id: "PanelFooter_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            //region: "south",
            //bodyStyle: 'background:transparent;',

            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            //split: true,

            items: [
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //DirPaymentTypes
                        {
                            xtype: 'viewComboBox',
                            labelAlign: 'top',
                            fieldLabel: "Тип оплаты", allowBlank: false, flex: 1,
                            //margin: "0 0 0 5",
                            store: this.storeDirPaymentTypesGrid, // store getting items from server
                            valueField: 'DirPaymentTypeID',
                            hiddenName: 'DirPaymentTypeID',
                            displayField: 'DirPaymentTypeName',
                            name: 'DirPaymentTypeID', itemId: "DirPaymentTypeID", id: "DirPaymentTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        //Суммы
                        { xtype: 'textfield', fieldLabel: lanPayable, labelAlign: 'top', name: "SumOfVATCurrency", id: "SumOfVATCurrency" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
                        //Скидка
                        {
                            xtype: 'textfield', fieldLabel: lanDiscount, labelAlign: 'top', itemId: "Discount", name: "Discount", id: "Discount" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            listeners: { change: 'onChangeDiscount' },
                        },
                        //Внесено
                        {
                            xtype: 'textfield', fieldLabel: lanContributed, labelAlign: 'top', itemId: "Payment", name: "Payment", id: "Payment" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, style: 'font-weight:bold;', flex: 1, allowBlank: true,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            listeners: { change: 'onPaymentDiscount' },
                        },
                        //Сдача
                        { xtype: 'textfield', fieldLabel: lanSurrender, labelAlign: 'top', itemId: "Surrender", name: "Surrender", id: "Surrender" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, style: 'font-weight:bold;color:red;', flex: 1, readOnly: true, allowBlank: true },
                    ]
                }
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
            //bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [
                PanelDocumentDetails,
                PanelSearch, PanelParty,
                PanelGrid, PanelFooter
            ],

            buttons: [
                { xtype: "checkbox", boxLabel: lanOnCredit, labelAlign: 'top', inputValue: true, name: "OnCredit", id: "OnCredit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                {
                    id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", hidden: true,
                    UO_Action: "held_cancel",
                    text: lanHeldCancel, icon: '../Scripts/sklad/images/save_held.png',
                    listeners: { click: 'onBtnHeldCancelClick' }
                },
                {
                    id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", hidden: true,
                    UO_Action: "held",
                    text: lanCalculates, icon: '../Scripts/sklad/images/save_held.png',
                    listeners: { click: 'onBtnHeldsClick' }
                },

                "->",

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png',
                    listeners: { click: 'onBtnCancelClick' }
                },
                " ",
                {
                    id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint", hidden: true,
                    text: lanPrint, icon: '../Scripts/sklad/images/print.png',
                    menu:
                    [
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintHtml",
                            UO_Action: "html",
                            text: "Html", icon: '../Scripts/sklad/images/html.png',
                            listeners: { click: 'onBtnPrintHtmlClick' }
                        },
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintExcel",
                            UO_Action: "excel",
                            text: "MS Excel", icon: '../Scripts/sklad/images/excel.png',
                            listeners: { click: 'onBtnPrintHtmlClick' }
                        }
                    ]
                },
                "-",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png',
                    listeners: { click: 'onBtnHelpClick' }
                }
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

                title: "Товар",
                region: 'west',
                //width: 215,

                store: this.storeNomenTree,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirNomen"
                },


                columns: [
                    //{ text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    //{ text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: 'Наименование', flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'О', dataIndex: 'Remains', width: 35, hidden: true, tdCls: 'x-change-cell' },
                    //{ text: 'DirNomenPatchFull', dataIndex: 'DirNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],

                /*
                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDocRetailsEdit_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDocRetailsEdit_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDocRetailsEdit_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDocRetailsEdit_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDocRetailsEdit_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDocRetailsEdit_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDocRetailsEdit_onTree_addSub;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }
                */

            }),


            // *** *** *** *** *** *** *** *** ***


            //Шапка документа + табличная часть
            formPanel

        ],


        //this.buttons = ,


        this.callParent(arguments)
    },

});