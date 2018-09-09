//ComboBox для выбора справочника, в которіх небольшое к-во записей и отсутствуют группы
//Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridPF", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridPF",

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

            /*
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
            */
        ],

        this.callParent(arguments);
    },



    //Формат даты
    viewConfig: {
        getRowClass: function (record, index) {
            //Удалена          - Красным (Бакграунд)
            if (record.get('Del') == 'True') { return 'price-del'; }
            return "lang_" + record.get('ListLanguageNameSmall');

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