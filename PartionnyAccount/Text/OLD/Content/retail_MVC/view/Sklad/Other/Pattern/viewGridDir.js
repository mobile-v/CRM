//ComboBox для выбора справочника, в которіх небольшое к-во записей и отсутствуют группы
//Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridDir", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridDir",

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
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/table_add.png', tooltip: lanNewM, //disabled: true,
                id: "btnNew" + this.UO_id, itemId: "btnNew",
            },
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/table_relation.png', tooltip: lanNewCopy, disabled: true,
                id: "btnNewCopy" + this.UO_id, itemId: "btnNewCopy"
            },
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/table_edit.png', tooltip: lanEdit, disabled: true,
                id: "btnEdit" + this.UO_id, itemId: "btnEdit"
            },
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/table_delete.png', tooltip: lanDeletionFlag + "?", disabled: true, // lanDeletionFlag
                id: "btnDelete" + this.UO_id, itemId: "btnDelete"
            },

            "->",
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,

                xtype: "button",
                icon: '../Scripts/sklad/images/help16.png', tooltip: "Help", //disabled: true,
                id: "btnHelp" + this.UO_id, itemId: "btnHelp"
            },


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
                allowBlank: true
            },

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


        }, //getRowClass

        stripeRows: true,

        listeners: {
            itemcontextmenu: function (view, rec, node, index, e) {
                e.stopEvent();
                contextMenuGrid.showAt(e.getXY());
                return false;
            }
        }

    } //viewConfig

});