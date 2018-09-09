Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocRetails/viewDocRetailsEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocRetailsEdit",

    layout: "border",
    region: "center",
    title: lanDocumentRetails,
    width: 900, height: 575,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    conf: {},

    initComponent: function () {

        //Создать 3-и панели:
        // Вверху      - Партии
        // По середине - Основное
        // Внизу       - Спецификация
        //И есть Сплитер


        //Панель
        var SearchType_values = [
            [1, 'В товаре (код, наименование)'],
            [2, 'В партиях (серии, штрих-код)'],
        ];
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
                            id: "TriggerSearchGrid" + this.UO_id,
                            UO_id: this.UO_id,
                            UO_idMain: this.UO_idMain,
                            UO_idCall: this.UO_idCall,

                            xtype: 'viewTriggerSearch',
                            emptyText: "Поиск ...", allowBlank: true, flex: 1,
                            name: 'TriggerSearchGrid', id: "TriggerSearchGrid" + this.UO_id, itemId: "TriggerSearchGrid"
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
                { text: "Партия", dataIndex: "RemPartyID", width: 50 }, //, hidden: true
                //Товар
                { text: "Код", dataIndex: "DirNomenID", width: 50 },
                { text: "Найм.", dataIndex: "DirNomenName", width: 50 },
                { text: "Дата", dataIndex: "DocDate", width: 75 },

                { text: "Поставщик", dataIndex: "DirContractorName", width: 75 },
                { text: "Склад", dataIndex: "DirWarehouseName", width: 75 },
                { text: "Документ", dataIndex: "ListDocNameRu", width: 75 },

                { text: "Налог", dataIndex: "DirVatValue", width: 50, hidden: true },
                { text: "Поступило", dataIndex: "Quantity", width: 85 },
                { text: "Остаток", dataIndex: "Remnant", width: 85 },
                
                //{ text: "Цена в вал.", dataIndex: "PriceVAT", width: 85, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 50, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 50, hidden: true },
                //{ text: "Цена прих", dataIndex: "PriceCurrency", width: 85, hidden: true },
                
                { text: "Цена Розница (в вал.)", dataIndex: "PriceRetailVAT", width: 85, hidden: true },
                { text: "Цена Розница", dataIndex: "PriceRetailCurrency", width: 85 }, //, hidden: true

                { text: "Цена Опт (в вал.)", dataIndex: "PriceWholeRetailVAT", width: 85, hidden: true },
                { text: "Цена Опт", dataIndex: "PriceWholeRetailCurrency", width: 85 }, //, hidden: true

                { text: "Цена ИМ (в вал.)", dataIndex: "PriceIMVAT", width: 85, hidden: true },
                { text: "Цена ИМ", dataIndex: "PriceIMCurrency", width: 85 }, //, hidden: true

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", width: 125 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100, hidden: true },
                { text: "Материал", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                { text: "Стиль", dataIndex: "DirCharStyleName", width: 100, hidden: true },
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
            bodyStyle: 'background:transparent;',
            title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            
            defaults: { anchor: '100%' },
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%", height: 110,
            autoScroll: true,
            //split: true,

            items: [

                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },  //, hidden: true
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", name: "DocRetailID", id: "DocRetailID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true },
                        { xtype: 'viewDateField', fieldLabel: lanDateCounterparty, name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", allowBlank: false, editable: false },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        
                        //DirPriceTypes
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Тип цены", flex: 2, allowBlank: false,
                            store: this.storeDirPriceTypesGrid, // store getting items from server
                            valueField: 'DirPriceTypeID',
                            hiddenName: 'DirPriceTypeID',
                            displayField: 'DirPriceTypeName',
                            name: 'DirPriceTypeID', itemId: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                        },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", flex: 2, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
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

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //DirContractor
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanContractor, flex: 1, allowBlank: false, //, emptyText: "..."

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
            width: "100%", height: 110,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

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
                        { xtype: 'textfield', fieldLabel: lanBase, name: "DocBase", id: "DocBase" + this.UO_id, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanDisc, margin: "0 0 0 5", name: "DocDisc", id: "DocDisc" + this.UO_id, flex: 1, allowBlank: true }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //DirVat
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanVat, flex: 1, allowBlank: false, //, emptyText: "..."
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

            ]
        });

        var PanelDocumentPay = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentPay_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanDiscPay,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%", height: 110,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        //DirPaymentTypes
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Тип оплаты", flex: 1, allowBlank: false,
                            //margin: "0 0 0 5",
                            store: this.storeDirPaymentTypesGrid, // store getting items from server
                            valueField: 'DirPaymentTypeID',
                            hiddenName: 'DirPaymentTypeID',
                            displayField: 'DirPaymentTypeName',
                            name: 'DirPaymentTypeID', itemId: "DirPaymentTypeID", id: "DirPaymentTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                        },

                        { xtype: 'textfield', fieldLabel: lanPaid, regex: /^[+\-]?\d+(?:\.\d+)?$/, margin: "0 0 0 5", flex: 1, allowBlank: false, name: "Payment", itemId: "Payment", id: "Payment" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },

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
            width: "100%", height: 110,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

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
                PanelDocumentDetails, PanelDocumentAdditionally, PanelDocumentPay, PanelDocumentDiscount
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

            store: this.storeGrid, //storeDocRetailTabsGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 50 }, //, hidden: true
                //Товар
                { text: "№", dataIndex: "DirNomenID", width: 50 },
                { text: lanNomenclature, dataIndex: "DirNomenName", flex: 1 }, //flex: 1
                //К-во
                { text: lanCount, dataIndex: "Quantity", width: 75 },
                //Суммы
                { text: "Тип цены", dataIndex: "DirPriceTypeName", width: 75 },
                { text: lanPriceRetail, dataIndex: "PriceCurrency", width: 100 },
                { text: lanSum, dataIndex: "SUMRetailPriceVATCurrency", width: 100 },

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", flex: 1 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 70, hidden: true },
                { text: "Материал", dataIndex: "DirCharMaterialName", width: 70, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 70, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 70, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 70, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 70, hidden: true },
                { text: "Стиль", dataIndex: "DirCharStyleName", width: 70, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 70, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 70, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 70, hidden: true },
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
                PanelSearch, PanelParty,
                tabPanelDetails, PanelGrid, PanelFooter
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

            }),


            // *** *** *** *** *** *** *** *** ***


            //Шапка документа + табличная часть
            formPanel

        ],


        this.buttons = [
            {
                id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", hidden: true,
                UO_Action: "held_cancel",
                text: lanHeldCancel, icon: '../Scripts/sklad/images/save_held.png'
            },
            {
                id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", hidden: true,
                UO_Action: "held",
                text: lanHelds, icon: '../Scripts/sklad/images/save_held.png'
            },
            {
                id: "btnRecord" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true,
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
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", 
                text: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
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
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png',
            }

        ],


        this.callParent(arguments);
    }

});