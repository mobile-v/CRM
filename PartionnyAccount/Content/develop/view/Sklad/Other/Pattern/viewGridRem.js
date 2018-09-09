//ComboBox для выбора справочника, в которых небольшое к-во записей и отсутствуют группы //Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridRem", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridRem",

    region: "center",
    loadMask: true,
    autoScroll: true,
    touchScroll: true,
    itemId: "grid",
    UI_DateFormatStr: false,

    conf: {},

    initComponent: function () {
        this.id = this.conf.id;
        this.UO_id = this.conf.UO_id;
        this.UO_idMain = this.conf.UO_idMain;
        this.UO_idCall = this.conf.UO_idCall;

        this.UO_View = this.conf.UO_View;


        this.tbar = [

            /*
            { xtype: 'textfield', name: "DirNomenPatchFull", id: "DirNomenPatchFull" + this.UO_id, readOnly: true, flex: 1, allowBlank: true }, //, fieldLabel: ""
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload",
                id: "btnDirNomenReload" + this.UO_id, itemId: "btnDirNomenReload"
            },

            //"->",
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
                flex: 1
            }
            */


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

            /*
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
            */

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