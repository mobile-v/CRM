Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandRetailsEdit", {

    extend: InterfaceSystemObjName,
    alias: "widget.viewDocSecondHandRetailsEdit",

    layout: "border",
    region: "center",
    title: "Б/У: Чек -> Новый",
    width: 850, height: 525,
    autoScroll: false,
    buttonAlign: 'left',
    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocSecondHandRetailsEdit',
    listeners: { close: 'onViewDocSecondHandRetailsEditClose' },

    conf: {},

    initComponent: function () {

        //1. Дата + Поиск
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

                // *** *** *** Not Visible *** *** *** *** *** *** *** ***

                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirPriceTypeID", name: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                // *** *** *** Not Visible *** *** *** *** *** *** *** ***



                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                    
                        { xtype: 'textfield', margin: "0 0 0 15", name: "DirNomenPatchFull", id: "DirNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true }, //, fieldLabel: ""
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
                            id: "TriggerSearchGrid" + this.UO_id,
                            UO_id: this.UO_id,
                            UO_idMain: this.UO_idMain,
                            UO_idCall: this.UO_idCall,

                            xtype: 'viewTriggerSearchGrid',
                            emptyText: "Поиск ...", allowBlank: true, flex: 1,
                            name: 'TriggerSearchGrid', id: "TriggerSearchGrid" + this.UO_id, itemId: "TriggerSearchGrid",
                            listeners: { ontriggerclick: "onTriggerSearchGridClick1", specialkey: "onTriggerSearchGridClick2", change: "onTriggerSearchGridClick3" }
                        },

                    ]
                },

            ]
        });

        //2. Партии
        var PanelParty = Ext.create('widget.viewGridRem22', {
            
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

            store: this.storeRem2PartiesGrid,

        });

        //3. Грид
        var PanelGrid = Ext.create('Ext.grid.Panel', { 
            
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: "viewDocSecondHandRetailsEdit", // this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            
            //bodyStyle: 'background:transparent;',
            itemId: "grid",
            listeners: { selectionchange: 'onGrid_selectionchange' }, //, itemclick: 'onGrid_itemclick', itemdblclick: 'onGrid_itemdblclick'

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeGrid, //storeDocRetailTabsGrid,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [
                { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                //Партия с которой списали
                { text: "Документ", dataIndex: "DocID", width: 75, hidden: true, tdCls: 'x-change-cell' },
                //Партия с которой списали
                { text: "Партия", dataIndex: "Rem2PartyID", width: 75, hidden: true, tdCls: 'x-change-cell' },
                //Партия списания
                { text: "Партия списания", dataIndex: "Rem2PartyMinusID", width: 75, hidden: true, tdCls: 'x-change-cell' },

                //Документ БУ
                { text: "Документ", dataIndex: "DocSecondHandPurchID", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },

                //Товар
                { text: "Код", dataIndex: "DirServiceNomenID", width: 50, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
                { text: "Товар", dataIndex: "DirServiceNomenName", flex: 1, tdCls: 'x-change-cell' }, //flex: 1

                //Характеристики
                /*
                { text: "Характеристики", dataIndex: "DirChar", flex: 1, hidden: true, tdCls: 'x-change-cell' },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 70, tdCls: 'x-change-cell' }, //, hidden: true
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 70, hidden: true, tdCls: 'x-change-cell' },
                { text: "Имя", dataIndex: "DirCharNameName", width: 70, tdCls: 'x-change-cell' }, //, hidden: true
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 70, hidden: true, tdCls: 'x-change-cell' },
                { text: "Пол", dataIndex: "DirCharSexName", width: 70, hidden: true, tdCls: 'x-change-cell' },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 70, hidden: true, tdCls: 'x-change-cell' },
                { text: "Поставщик", dataIndex: "DirCharStyleName", width: 70, tdCls: 'x-change-cell' }, //, hidden: true
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 70, hidden: true, tdCls: 'x-change-cell' },
                { text: "Серийный", dataIndex: "SerialNumber", width: 70, hidden: true, tdCls: 'x-change-cell' },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 70, hidden: true, tdCls: 'x-change-cell' },
                */

                //Приходная цена
                { text: "Закупка", dataIndex: "PriceCurrencyPurch", width: 100, tdCls: 'x-change-cell' }, //hidden: true, 

                //К-во
                { text: lanCount, dataIndex: "Quantity", width: 75, summaryType: 'sum', tdCls: 'x-change-cell', hidden: true },
                //Суммы
                { text: "Тип цены", dataIndex: "DirPriceTypeName", width: 75, hidden: true, tdCls: 'x-change-cell' },
                { text: lanPriceSale, dataIndex: "PriceCurrency", width: 100, hidden: true, tdCls: 'x-change-cell' },
                { text: lanSum, dataIndex: "SUMSalePriceVATCurrency", width: 100, summaryType: 'sum', tdCls: 'x-change-cell' },
                { text: "Скидка", dataIndex: "Discount", width: 50, style: "height: 25px;", summaryType: 'sum', tdCls: 'x-change-cell' },

                { text: "Дата", dataIndex: "DocDate", width: 85, tdCls: 'x-change-cell' },
                { text: "Сотрудник", dataIndex: "DirEmployeeName", flex: 1, tdCls: 'x-change-cell', hidden: true },

                //{ text: "№ причины", dataIndex: "DirDescriptionID", width: 100, hidden: true },
                { text: "Тип возвр.", dataIndex: "DirReturnTypeName", width: 85, hidden: true, tdCls: 'x-change-cell' },
                { text: "Причина", dataIndex: "DirDescriptionName", width: 85, hidden: true, tdCls: 'x-change-cell' },

                //ККМ
                { text: "Чек №", dataIndex: "KKMSCheckNumber", width: 60, tdCls: 'x-change-cell' },
                { text: "Чек Id", dataIndex: "KKMSIdCommand", width: 100, hidden: true, tdCls: 'x-change-cell' },
            ],

            tbar: [
                //Новый Чек
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Продать</b></font>", tooltip: lanAddPosition, style: "width: 120px; height: 40px;",
                    itemId: "btnGridAddPosition",
                    listeners: { click: 'onGrid_BtnGridAddPosition' }
                },
                //Редактирование Чека
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanEdit + "</b></font>", tooltip: lanEdit, disabled: true, style: "width: 160px; height: 40px;",
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit", hidden: true,
                    listeners: { click: 'onGrid_BtnGridEdit' }
                },
                //Удаление Чека
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanDelete + "</b></font>", tooltip: lanDeletionFlag + "?", disabled: true, style: "width: 110px; height: 40px;",
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete", hidden: true,
                    listeners: { click: 'onGrid_BtnGridDelete' }
                },
                //Возврат
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Возврат</b></font>", tooltip: lanAddPosition, disabled: true, style: "width: 120px; height: 40px;",
                    itemId: "btnGridAddReturnPosition",
                    listeners: { click: 'onGrid_BtnGridAddReturnPosition' }
                },
                //Списание
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Списать</b></font>", tooltip: lanDocumentActWriteOff, style: "width: 120px; height: 40px;",
                    itemId: "btnGridActWriteOff",
                    listeners: { click: 'onGrid_BtnGridActWriteOff' }
                },


                { xtype: 'label', text: ' С ', style: 'display:block; padding: 0px 10px 0px 10px' },
                {
                    xtype: 'viewDateFieldFix', name: "DocDateS", id: "DocDateS" + this.UO_id, allowBlank: false, width: 100, //, fieldLabel: "С"
                    id: 'DocDateS' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    listeners: { change: 'onDocDateSChange' },
                },
                { xtype: 'label', text: ' По ', style: 'display:block; padding: 0px 10px 0px 10px' },
                {
                    xtype: 'viewDateFieldFix', name: "DocDatePo", id: "DocDatePo" + this.UO_id, allowBlank: false, width: 100, //, fieldLabel: "По"
                    id: 'DocDatePo' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    listeners: { change: 'onDocDatePoChange' },
                }, 



                "->",
                //KKM
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/modem.png', tooltip: "KKM", disabled: true, style: "width: 40px; height: 40px;",
                    itemId: "btnGridKKM",
                    listeners: { click: 'onGrid_BtnGridKKM' }
                },
                //Обновить грид
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/refresh16.png', tooltip: "Обновить", style: "width: 40px; height: 40px;",
                    id: "btnGridRefresh" + this.UO_id, itemId: "btnGridRefresh",
                    listeners: { click: 'onGrid_BtnGridRefresh' }
                },

            ],

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

                    //Продажа
                    if (parseInt(record.get('ListObjectID')) == 66) { return 'price-held_paid'; }
                    //Возврат
                    if (parseInt(record.get('ListObjectID')) == 67) { return 'price-held_unpaid'; }
                    //Поступление (покупка аппарата)
                    if (parseInt(record.get('ListObjectID')) == 65) { return 'yellow'; }

                    //Продажа
                    //if (parseFloat(record.get('PriceCurrency')) > 0) { record.data.DocDiscPay = "Оплачена"; return 'price-held_paid'; }
                    //Возврат
                    //if (parseFloat(record.get('PriceCurrency')) < 0) { record.data.DocDiscPay = "Оплачена"; return 'price-held_unpaid'; }

                    //Не используется
                    /*
                    if (this.grid.UO_View.indexOf("Doc") == -1 && this.grid.UO_View.indexOf("DirContractor") == -1) { return 'price-Dir'; }
                    else if (record.get('Held') == true) {
                        if (parseFloat(record.get('PriceCurrency')) == 0) { record.data.DocDiscPay = "Оплачена"; return 'price-held_paid'; } //Оплачена
                        else if (parseFloat(record.get('PriceCurrency')) < 0) { record.data.DocDiscPay = "Переплачане"; return 'price-held_overpaid'; }  //Переплачане
                        else if (parseFloat(record.get('PriceCurrency')) > 0 && parseFloat(record.get('SumOfVATCurrency')) == parseFloat(record.get('PriceCurrency'))) { record.data.DocDiscPay = "Не оплачена"; return 'price-held_unpaid'; } //Не оплачена
                        else if (parseFloat(record.get('PriceCurrency')) > 0) { record.data.DocDiscPay = "Частично оплачена"; return 'price-held_partly_paid'; } //Частично оплачена
                        else { return 'price-held_paid'; }
                    }
                    else if (record.get('Reserve') == true) {
                        if (parseFloat(record.get('PriceCurrency')) == 0) { record.data.DocDiscPay = "Оплачена"; return 'price-reserv_paid'; } //Оплачена
                        else if (parseFloat(record.get('PriceCurrency')) < 0) { record.data.DocDiscPay = "Переплачане"; return 'price-reserv_overpaid'; }  //Переплачане
                        else if (parseFloat(record.get('PriceCurrency')) > 0 && parseFloat(record.get('SumOfVATCurrency')) == parseFloat(record.get('PriceCurrency'))) { record.data.DocDiscPay = "Не оплачена"; return 'price-reserv_unpaid'; } //Не оплачена
                        else if (parseFloat(record.get('PriceCurrency')) > 0) { record.data.DocDiscPay = "Частично оплачена"; return 'price-reserv_partly_paid'; } //Частично оплачена
                        else { return 'price-reserv_paid'; }
                    }
                    else {
                        if (parseFloat(record.get('PriceCurrency')) == 0) { record.data.DocDiscPay = "Оплачена"; return 'price-held_no_paid'; } //Оплачена
                        else if (parseFloat(record.get('PriceCurrency')) < 0) { record.data.DocDiscPay = "Переплачане"; return 'price-held_no_overpaid'; }  //Переплачане
                        else if (parseFloat(record.get('PriceCurrency')) > 0 && parseFloat(record.get('SumOfVATCurrency')) == parseFloat(record.get('PriceCurrency'))) { record.data.DocDiscPay = "Не оплачена"; return 'price-held_no_unpaid'; } //Не оплачена
                        else if (parseFloat(record.get('PriceCurrency')) > 0) { record.data.DocDiscPay = "Частично оплачена"; return 'price-held_no_partly_paid'; } //Частично оплачена
                        else { return 'price-held_no_paid'; }
                    }
                    */

                }, //getRowClass

                stripeRows: true,

            } //viewConfig

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
                PanelSearch, PanelParty,
                PanelGrid, //PanelFooter
            ],

        });


        //body
        this.items = [

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

                /*
                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDocSecondHandRetailsEdit_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDocSecondHandRetailsEdit_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDocSecondHandRetailsEdit_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDocSecondHandRetailsEdit_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDocSecondHandRetailsEdit_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDocSecondHandRetailsEdit_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDocSecondHandRetailsEdit_onTree_addSub;
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


        this.callParent(arguments)
    },

});