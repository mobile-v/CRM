Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocDomesticExpenses/viewDocDomesticExpensesEdit", {
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocDomesticExpensesEdit",
    layout: "border",
    region: "center",
    title: "Выплаты",
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
    controller: 'viewcontrollerDocDomesticExpensesEdit',
    listeners: { close: 'onViewDocDomesticExpensesEditClose' },

    conf: {},

    initComponent: function () {
             
        //3. Грид
        var PanelGrid = Ext.create('Ext.grid.Panel', { 
            
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: "viewDocDomesticExpensesEdit", // this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            
            //bodyStyle: 'background:transparent;',
            itemId: "grid",
            listeners: { selectionchange: 'onGrid_selectionchange' }, //, itemclick: 'onGrid_itemclick', itemdblclick: 'onGrid_itemdblclick'

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeGrid, //storeDocDomesticExpenseTabsGrid,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [
                { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                //Партия с которой списали
                { text: "Документ", dataIndex: "DocID", width: 75, hidden: true, tdCls: 'x-change-cell' },

                //Товар
                { text: "Статья", dataIndex: "DirDomesticExpenseName", flex: 1, tdCls: 'x-change-cell' }, //flex: 1

                //Суммы
                { text: "Сумма", dataIndex: "PriceCurrency", width: 150, tdCls: 'x-change-cell' },
                { text: "Оплата", dataIndex: "DirPaymentTypeName", width: 150, tdCls: 'x-change-cell' },

                { text: "Дата", dataIndex: "DocDate", width: 150, tdCls: 'x-change-cell' },
                { text: "Создал", dataIndex: "DirEmployeeName", flex: 1, tdCls: 'x-change-cell' }, //, hidden: true
                { text: "Списать с", dataIndex: "DirEmployeeNameSpisat", flex: 1, tdCls: 'x-change-cell' }, //, hidden: true
            ],

            tbar: [
                //Новый Чек
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Оплатить</b></font>", tooltip: lanAddPosition, style: "width: 120px; height: 40px;",
                    itemId: "btnGridAddPosition",
                    listeners: { click: 'onGrid_BtnGridAddPosition' }
                },
                //Редактирование Чека
                /*{
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanEdit + "</b></font>", tooltip: lanEdit, disabled: true, style: "width: 160px; height: 40px;",
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit", hidden: true,
                    listeners: { click: 'onGrid_BtnGridEdit' }
                },*/
                //Удаление Чека
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanDelete + "</b></font>", tooltip: lanDeletionFlag + "?", style: "width: 110px; height: 40px;",
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete", //hidden: true, disabled: true, 
                    listeners: { click: 'onGrid_BtnGridDelete' }
                },
                //Возврат
                /*{
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Возврат</b></font>", tooltip: lanAddPosition, disabled: true, style: "width: 120px; height: 40px;",
                    itemId: "btnGridAddReturnPosition",
                    listeners: { click: 'onGrid_BtnGridAddReturnPosition' }
                },*/
                //Списание
                /*{
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "<font size=" + HeaderMenu_FontSize_1 + "><b>Списать</b></font>", tooltip: lanDocumentActWriteOff, style: "width: 120px; height: 40px;",
                    itemId: "btnGridActWriteOff",
                    listeners: { click: 'onGrid_BtnGridActWriteOff' }
                },*/


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
                    if (parseFloat(record.get('PriceCurrency')) > 0) { record.data.DocDiscPay = "Оплачена"; return 'price-held_paid'; }
                    //Возврат
                    if (parseFloat(record.get('PriceCurrency')) < 0) { record.data.DocDiscPay = "Оплачена"; return 'price-held_unpaid'; }

                    //Не используется
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
                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                PanelGrid, 
            ],

        });


        //body
        this.items = [

            //Статьи
            /*
            Ext.create('widget.viewTreeDirRetail', {

                conf: {
                    id: "tree_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

                store: this.storeDomesticExpenseTree,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirNomen"
                },


                columns: [
                    { xtype: 'treecolumn', text: 'Наименование', flex: 1, sortable: true, dataIndex: 'text', },
                ],

            }),
            */

            // *** *** *** *** *** *** *** *** ***


            //Шапка документа + табличная часть
            formPanel

        ],


        this.callParent(arguments)
    },

});