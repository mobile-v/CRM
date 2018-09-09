Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocReturnsCustomers/viewDocReturnsCustomersEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocReturnsCustomersEdit",

    layout: "border",
    region: "center",
    title: lanDocumentReturnsCustomers,
    width: 900, height: 550,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocReturnsCustomersEdit',

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {

        //Создать 3-и панели:
        // Вверху      - Партии
        // По середине - Основное
        // Внизу       - Спецификация
        //И есть Сплитер



        var PanelPartyMinus = Ext.create('widget.viewGridRem', {
            
            conf: {
                id: "gridPartyMinus_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View, //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },
            
            itemId: "gridPartyMinus",
            title: "Партии товара",
            collapsible: true,
            autoScroll: true,
            flex: 1,
            //split: true,
            store: this.storeRemPartyMinusesGrid,

            /*columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyMinusID", width: 50, hidden: true }, //, hidden: true
                //Товар
                { text: "Код", dataIndex: "DirNomenID", width: 50, style: "height: 25px;" },
                { text: "Дата док", dataIndex: "DocDate", width: 75 },
                //{ text: "Дата приём", dataIndex: "DocDatePurches", width: 75 },

                { text: "Поставщик", dataIndex: "DirContractorName", width: 75 },
                { text: "Склад", dataIndex: "DirWarehouseName", width: 75 },
                { text: "Документ", dataIndex: "ListDocNameRu", width: 75 },

                { text: "Налог", dataIndex: "DirVatValue", width: 50, hidden: true },
                { text: "Продано", dataIndex: "Quantity", width: 85 },

                { text: "Цена в вал.", dataIndex: "PriceVAT", width: 85, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 50, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 50, hidden: true },
                { text: "Цена", dataIndex: "PriceCurrency", width: 85 },

                
                //{ text: "Цена Розница (в вал.)", dataIndex: "PriceRetailVAT", width: 85, hidden: true },
                //{ text: "Цена", dataIndex: "PriceRetailCurrency", width: 85, hidden: true },

                //{ text: "Цена Опт (в вал.)", dataIndex: "PriceWholesaleVAT", width: 85, hidden: true },
                //{ text: "Цена", dataIndex: "PriceWholesaleCurrency", width: 85, hidden: true },

                //{ text: "Цена ИМ (в вал.)", dataIndex: "PriceIMVAT", width: 85, hidden: true },
                //{ text: "Цена", dataIndex: "PriceIMCurrency", width: 85, hidden: true },
                

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", width: 125, hidden: true },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100 },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                { text: "Поставщик", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true }
            ],*/

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
            width: "100%", height: 115 + varBodyPadding,
            autoScroll: true,
            //split: true,

            items: [

                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },  //, hidden: true
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", name: "DocReturnsCustomerID", id: "DocReturnsCustomerID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true },
                        { xtype: 'viewDateField', fieldLabel: lanDateDoc, name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", allowBlank: false, editable: false },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        {
                            xtype: 'viewTriggerDir',
                            fieldLabel: "Продажа", emptyText: "...", allowBlank: false, flex: 1,
                            name: 'DocSaleName', itemId: "DocSaleName", id: "DocSaleName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: false,
                            name: 'DocSaleID', itemId: "DocSaleID", id: "DocSaleID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                        
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirContractorsOrgGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true,
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
                        //DirContractor
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanContractor, flex: 1, allowBlank: false, //, emptyText: "..."

                            store: this.storeDirContractorsGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorID', itemId: "DirContractorID", id: "DirContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirContractorEdit", id: "btnDirContractorEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirContractorReload", id: "btnDirContractorReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanWarehouse, flex: 1, allowBlank: false, //, emptyText: "..."
                            margin: "0 0 0 5",
                            store: this.storeDirWarehousesGrid, // store getting items from server
                            valueField: 'DirWarehouseID',
                            hiddenName: 'DirWarehouseID',
                            displayField: 'DirWarehouseName',
                            name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },
                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirWarehouseEdit", id: "btnDirWarehouseEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirWarehouseReload", id: "btnDirWarehouseReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
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

            items: [
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
                            readOnly: true,
                            //Поиск
                            //editable: false, typeAhead: false, minChars: 200,
                        },

                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: lanBase, name: "Base", id: "Base" + this.UO_id, flex: 1, allowBlank: true },
                { xtype: 'textfield', fieldLabel: lanDisc, name: "Description", id: "Description" + this.UO_id, flex: 1, allowBlank: true }

            ]
        });

        /*
        var PanelDocumentPay = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentPay_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanDiscPay,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%", height: 115,
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
        */

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
                        { xtype: 'textfield', fieldLabel: lanDiscount, itemId: "Discount", name: "Discount", id: "Discount" + this.UO_id, readOnly: true, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        //{ xtype: 'button', itemId: "btnSpreadDiscount", tooltip: "Раскинуть скидку по спецификации", text: "Раскинуть скидку по спецификации", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                /*
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanSum, itemId: "SummOther", name: "SummOther", id: "SummOther" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', itemId: "btnSpreadSummOther", tooltip: "Изменить сумму документа", text: "Изменить сумму документа", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
                    ]
                },
                */
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
                PanelDocumentDetails, PanelDocumentAdditionally, PanelDocumentDiscount //, PanelDocumentPay
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

            store: this.storeGrid, //storeDocReturnsCustomerTabsGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyMinusID", width: 50, hidden: true },
                //Товар
                { text: "№", dataIndex: "DirNomenID", width: 50, style: "height: 25px;" },
                { text: lanNomenclature, dataIndex: "DirNomenName", flex: 1 }, //flex: 1
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
                { text: lanPriceSale, dataIndex: "PriceCurrency", width: 100 },
                { text: lanSum, dataIndex: "SUMPurchPriceVATCurrency", width: 100 },

                //Характеристики
                /*
                { text: "Характеристики", dataIndex: "DirChar", flex: 1 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                { text: "Поставщик", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                */
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true },
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
                PanelPartyMinus,
                tabPanelDetails, PanelGrid, PanelFooter
            ]
        });




        //body
        this.items = [

            //Шапка документа + табличная часть
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