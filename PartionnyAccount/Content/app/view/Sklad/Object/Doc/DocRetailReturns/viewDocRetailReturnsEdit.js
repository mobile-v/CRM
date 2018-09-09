Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocRetailReturns/viewDocRetailReturnsEdit", {
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocRetailReturnsEdit",

    layout: "border",
    region: "center",
    title: "Возврат -> ",
    width: 850, height: 525,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocRetailReturnsEdit',

    listeners: { close: 'onViewDocRetailReturnsEditClose' },

    bodyStyle: 'background:white;', //bodyStyle: 'opacity:0.5;',
    bodyPadding: 5,

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {

        //Создать 3-и панели:
        // Вверху      - Партии
        // По середине - Основное
        // Внизу       - Спецификация
        //И есть Сплитер


        //1.
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
                { xtype: 'textfield', fieldLabel: "№", name: "DocRetailReturnID", id: "DocRetailReturnID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirPriceTypeID", name: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: "checkbox", boxLabel: lanReserve, margin: "0 0 0 5", name: "Reserve", itemId: "Reserve", flex: 1, id: "Reserve" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirVatValue", name: "DirVatValue", id: "DirVatValue" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirContractorID", name: "DirContractorID", id: "DirContractorID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                // *** *** *** Not Visible *** *** ***


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        {
                            xtype: 'viewDateField', fieldLabel: "Дата", name: "DocDate", id: "DocDate" + this.UO_id, margin: "0 0 0 5", allowBlank: false,
                            id: 'DocDate' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            listeners: { change: 'onDocDateChange' },
                        }, //, readOnly: true

                        /*
                        {
                            xtype: 'viewTriggerDirRetail',
                            fieldLabel: "Продажа", emptyText: "...", allowBlank: false, flex: 1, margin: "0 0 0 5",
                            name: 'DocRetailName', itemId: "DocRetailName", id: "DocRetailName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                        {
                            xtype: 'viewTriggerDirFieldRetail',
                            allowBlank: false,
                            name: 'DocRetailID', itemId: "DocRetailID", id: "DocRetailID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                        */
                    ]
                }
            ]
        });


        //2. Грид
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
                { text: "Партия спис.", dataIndex: "RemPartyMinusID", width: 50, hidden: true },
                //Товар
                { text: "Код", dataIndex: "DirNomenID", width: 50, style: "height: 25px;" },
                { text: "Найм.", dataIndex: "DirNomenName", flex: 1 },
                { text: "Дата", dataIndex: "DocDate", width: 85 },

                { text: "Поставщик", dataIndex: "DirContractorName", flex: 1 },
                { text: "Склад", dataIndex: "DirWarehouseName", flex: 1 },
                { text: "Документ", dataIndex: "ListDocNameRu", flex: 1 },

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
                { text: "Характеристики", dataIndex: "DirChar", flex: 1, hidden: true },
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
            bbar: new Ext.PagingToolbar({
                store: this.storeRemPartyMinusesGrid,                      // указано хранилище
                displayInfo: true,                          // вывести инфо обо общем числе записей
                displayMsg: lanShowing + "  {0} - {1} " + lanOf + " {2}"     // формат инфо
            }),

        });


        //3. Грид
        var PanelGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: "viewDocRetailReturnsEdit", // this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            //bodyStyle: 'background:transparent;',
            itemId: "grid",
            listeners: { selectionchange: 'onGrid_selectionchange', edit: 'onGrid_edit' }, //, itemclick: 'onGrid_itemclick', itemdblclick: 'onGrid_itemdblclick'

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeGrid, //storeDocRetailReturnTabsGrid,

            columns: [
                { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                //Партия
                { text: "Партия", dataIndex: "RemPartyMinusID", width: 50, hidden: true },
                //Товар
                { text: "Код", dataIndex: "DirNomenID", width: 50, style: "height: 25px;" },
                { text: lanNomenclature, dataIndex: "DirNomenName", flex: 1 }, //flex: 1
                { text: "Дата док", dataIndex: "DocDate", width: 75 },
                { text: "Дата приём", dataIndex: "DocDatePurches", width: 75 },
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
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true },

                { text: "Сотрудник", dataIndex: "DirEmployeeName", flex: 1 },
            ],

            tbar: [
                //Новый Чек
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Вернуть</b></font>", tooltip: lanAddPosition, style: "width: 150px; height: 40px;",
                    itemId: "btnGridAddPosition",
                    listeners: { click: 'onGrid_BtnGridAddPosition' }
                },
                //Редактирование Чека
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanEdit + "</b></font>", tooltip: lanEdit, disabled: true, style: "width: 160px; height: 40px;",
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit",
                    listeners: { click: 'onGrid_BtnGridEdit' }
                },
                //Удаление Чека
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanDelete + "</b></font>", tooltip: lanDeletionFlag + "?", disabled: true, style: "width: 120px; height: 40px;",
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete",
                    listeners: { click: 'onGrid_BtnGridDelete' }
                },
            ],

            //В Константах "нижнея панель" не нужна
            bbar: new Ext.PagingToolbar({
                store: this.storeGrid,                      // указано хранилище
                displayInfo: true,                          // вывести инфо обо общем числе записей
                displayMsg: lanShowing + "  {0} - {1} " + lanOf + " {2}"     // формат инфо
            }),


            //Формат даты
            viewConfig: {
                getRowClass: function (record, index) {

                    // 1. === Исправляем формат даты: "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"  ===  ===  ===  ===  === 
                    for (var i = 0; i < record.store.model.fields.length; i++) {
                        //Если поле типа "Дата"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }
                    }

                    //2.  === Стили ===  ===  ===  ===  === 
                    //Удалённые
                    if (record.get('Del') == true) { return 'price-del'; }

                    if (this.grid.UO_View.indexOf("Doc") == -1 && this.grid.UO_View.indexOf("DirContractor") == -1) { return 'price-Dir'; }
                    else if (record.get('Held') == true) {
                        if (parseFloat(record.get('HavePay')) == 0) { record.data.DocDiscPay = "Оплачена"; return 'price-held_paid'; } //Оплачена
                        else if (parseFloat(record.get('HavePay')) < 0) { record.data.DocDiscPay = "Переплачане"; return 'price-held_overpaid'; }  //Переплачане
                        else if (parseFloat(record.get('HavePay')) > 0 && parseFloat(record.get('SumOfVATCurrency')) == parseFloat(record.get('HavePay'))) { record.data.DocDiscPay = "Не оплачена"; return 'price-held_unpaid'; } //Не оплачена
                        else if (parseFloat(record.get('HavePay')) > 0) { record.data.DocDiscPay = "Частично оплачена"; return 'price-held_partly_paid'; } //Частично оплачена
                        else { return 'price-held_paid'; }
                    }
                    else if (record.get('Reserve') == true) {
                        if (parseFloat(record.get('HavePay')) == 0) { record.data.DocDiscPay = "Оплачена"; return 'price-reserv_paid'; } //Оплачена
                        else if (parseFloat(record.get('HavePay')) < 0) { record.data.DocDiscPay = "Переплачане"; return 'price-reserv_overpaid'; }  //Переплачане
                        else if (parseFloat(record.get('HavePay')) > 0 && parseFloat(record.get('SumOfVATCurrency')) == parseFloat(record.get('HavePay'))) { record.data.DocDiscPay = "Не оплачена"; return 'price-reserv_unpaid'; } //Не оплачена
                        else if (parseFloat(record.get('HavePay')) > 0) { record.data.DocDiscPay = "Частично оплачена"; return 'price-reserv_partly_paid'; } //Частично оплачена
                        else { return 'price-reserv_paid'; }
                    }
                    else {
                        if (parseFloat(record.get('HavePay')) == 0) { record.data.DocDiscPay = "Оплачена"; return 'price-held_no_paid'; } //Оплачена
                        else if (parseFloat(record.get('HavePay')) < 0) { record.data.DocDiscPay = "Переплачане"; return 'price-held_no_overpaid'; }  //Переплачане
                        else if (parseFloat(record.get('HavePay')) > 0 && parseFloat(record.get('SumOfVATCurrency')) == parseFloat(record.get('HavePay'))) { record.data.DocDiscPay = "Не оплачена"; return 'price-held_no_unpaid'; } //Не оплачена
                        else if (parseFloat(record.get('HavePay')) > 0) { record.data.DocDiscPay = "Частично оплачена"; return 'price-held_no_partly_paid'; } //Частично оплачена
                        else { return 'price-held_no_paid'; }
                    }

                }, //getRowClass

                stripeRows: true,

            } //viewConfig

        });


        //4. Футер
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
                        //К-во продано
                        { xtype: 'textfield', fieldLabel: "К-во", labelAlign: 'top', itemId: "SumQuantity", name: "SumQuantity", id: "SumQuantity" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, style: 'font-weight:bold;color:red;', flex: 1, readOnly: true, allowBlank: true },
                        //Скидка
                        { xtype: 'textfield', fieldLabel: "Скидка", labelAlign: 'top', itemId: "SumDiscount", name: "SumDiscount", id: "SumDiscount" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, style: 'font-weight:bold;color:red;', flex: 1, readOnly: true, allowBlank: true },
                        //Сумма
                        { xtype: 'textfield', fieldLabel: "Сумма", labelAlign: 'top', name: "SumOfVATCurrency", id: "SumOfVATCurrency" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, readOnly: true, allowBlank: true },
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
                PanelDocumentDetails,
                PanelPartyMinus,
                PanelGrid, PanelFooter //tabPanelDetails
            ],

        });




        //body
        this.items = [

            //Шапка документа + табличная часть
            formPanel

        ],


        this.callParent(arguments);
    }

});