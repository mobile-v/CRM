//ComboBox для выбора справочника, в которіх небольшое к-во записей и отсутствуют группы
//Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridDocRetail", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridDocRetail",

    region: "center",
    loadMask: true,
    autoScroll: true,
    touchScroll: true,
    itemId: "grid",
    UI_DateFormatStr: false,

    listeners: { selectionchange: 'onGridClick', itemclick: 'onGridItemclick', itemdblclick: 'onGridItemdblclick' },

    conf: {},

    initComponent: function () {
        this.id = this.conf.id;
        this.UO_id = this.conf.UO_id;
        this.UO_idMain = this.conf.UO_idMain;
        this.UO_idCall = this.conf.UO_idCall;

        this.UO_View = this.conf.UO_View;


        this.tbar = [
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/table_add.png', tooltip: lanNewM, //disabled: true,
                id: "btnNew" + this.UO_id, itemId: "btnNew",
                listeners: { click: 'onBtnNewClick' }
            },
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/table_relation.png', tooltip: lanNewCopy, disabled: true,
                id: "btnNewCopy" + this.UO_id, itemId: "btnNewCopy",
                listeners: { click: 'onBtnNewCopyClick' }
            },
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/table_edit.png', tooltip: lanEdit, disabled: true,
                id: "btnEdit" + this.UO_id, itemId: "btnEdit",
                listeners: { click: 'onBtnEditClick' }
            },
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/table_delete.png', tooltip: lanDeletionFlag + "?", disabled: true, // lanDeletionFlag
                id: "btnDelete" + this.UO_id, itemId: "btnDelete",
                listeners: { click: 'onBtnDeleteClick' }
            },

            "->",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, 
                xtype: "button",
                icon: '../Scripts/sklad/images/help16.png', tooltip: "Help", //disabled: true,
                id: "btnHelp" + this.UO_id, itemId: "btnHelp",
                listeners: { click: 'onBtnHelpClick' }
            },



            /*{
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/dataexchange_imports.png', tooltip: "Import", //disabled: true,
                id: "btnImport" + this.UO_id, itemId: "btnImport"
            },*/
            {
                id: "TriggerSearchGrid" + this.UO_id,
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,

                xtype: 'viewTriggerSearch',
                //fieldLabel: lanGroup,
                emptyText: "Поиск ...",
                name: 'TriggerSearchGrid',
                id: "TriggerSearchGrid" + this.UO_id, itemId: "TriggerSearchGrid",
                allowBlank: true,
                listeners: { ontriggerclick: "onTriggerClick", specialkey: "onTriggerSpecialkey", change: "onTriggerChange" }
            },

            { xtype: 'label', forId: "DateS" + this.UO_id, text: lanWith },
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,

                xtype: "viewDateField", name: "DateS", id: "DateS" + this.UO_id, itemId: "DateS", flex: 1, editable: false,
                listeners: { select: 'onDateSSelect' }
            },

            { xtype: 'label', forId: "DatePo" + this.UO_id, text: lanTo },
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,

                xtype: "viewDateField", name: "DatePo", id: "DatePo" + this.UO_id, itemId: "DatePo", flex: 1, editable: false,
                listeners: { select: 'onDatePoSelect' }
            }

        ],

        this.callParent(arguments);
    },



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
            //Справочник "Контрагенты": красить зелёным, если есть договор
            if (this.grid.UO_View == "viewDirContractors") if (parseInt(record.get('DirContractID')) > 0) { return 'price-held_no_paid'; }


            //Для документов:

            /*
            //Специально для Розницы
            if (this.grid.UO_View == "DocRetail" || this.grid.UO_View == "DocRetailReturns") {
                if (record.get('Held') == true) {
                    if (parseFloat(record.get('Surrender')) == 0) { return 'price-held_paid'; } //Оплачена
                    else if (parseFloat(record.get('Surrender')) < 0 && parseFloat(record.get('Contributed')) > 0) { return 'price-held_partly_paid'; } //Частично оплачена
                    else if (parseFloat(record.get('Surrender')) < 0 && parseFloat(record.get('Contributed')) == 0) { return 'price-held_unpaid'; } //Не оплачена
                    else { return 'price-held_paid'; }
                }
                else {
                    if (parseFloat(record.get('Surrender')) == 0) { return 'price-held_no_paid'; } //Оплачена
                    else if (parseFloat(record.get('Surrender')) < 0 && parseFloat(record.get('Contributed')) > 0) { return 'price-held_no_partly_paid'; } //Частично оплачена
                    else if (parseFloat(record.get('Surrender')) < 0 && parseFloat(record.get('Contributed')) == 0) { return 'price-held_no_unpaid'; } //Не оплачена
                    else { return 'price-held_no_paid'; }
                }
            }
            */


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

        /*listeners: {
            itemcontextmenu: function (view, rec, node, index, e) {
                e.stopEvent();
                contextMenuGrid.showAt(e.getXY());
                return false;
            }
        }*/

    } //viewConfig

});