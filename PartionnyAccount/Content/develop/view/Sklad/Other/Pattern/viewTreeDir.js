//ComboBox для выбора справочника, в которых небольшое к-во записей и отсутствуют группы
//Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewTreeDir", {
    extend: "Ext.tree.Panel",
    alias: "widget.viewTreeDir",

    region: "west",
    loadMask: true,
    rootVisible: false,
    width: 180,
    split: true,
    collapsible: true,
    useArrows: true,
    itemId: "tree",


    //PartionnyAccount.view.Other/Plugin/treeFilter
    /*
    plugins: [{
        ptype: 'treefilter2',
        allowParentFolders: true
    }],
    dockedItems: [{
        xtype: 'toolbar',
        dock: 'top',
        items: [{
            xtype: 'trigger',
            triggerCls: 'x-form-clear-trigger',
            onTriggerClick: function () {
                this.reset();
                this.focus();
            },
            listeners: {
                change: function (field, newVal) {
                    var tree = field.up('treepanel');

                    tree.filter(newVal);
                },
                buffer: 250
            }
        }]
    }],
    */

    //PartionnyAccount.view.Other/Plugin/treeFilter2
    /*
    mixins: {
        treeFilter: 'PartionnyAccount.view.Other/Plugin/treeFilter2'
    },

    go: function (text) {
        this.filterByText(text);
    },
    */

    hlColor: "FF0000",
    hlBaseColor : "00FF00",

    /*
    dockedItems: [{
        xtype: 'toolbar',
        dock: 'bottom',
        items: [{
            text: "Search for 'пленка'",
            handler: function () {

                var me = this,
                    panel = me.up("panel"),
                    rn = panel.getRootNode(),
                    regex = new RegExp("пленка");

                rn.findChildBy(function (child) {
                    var text = child.data.text;
                    if (regex.test(text) === true) {

                        var pNode = child.parentNode;
                        while (pNode) {

                            pNode.expand();
                            pNode = pNode.parentNode;
                        }

                        child.expand();
                        //panel.getSelectionModel().select(child, true);
                    }
                }, this, true);

            }
        }]
    }],
    */

    conf: {},

    initComponent: function () {
        this.id = this.conf.id;
        this.UO_id = this.conf.UO_id;
        this.UO_idMain = this.conf.UO_idMain;
        this.UO_idCall = this.conf.UO_idCall;
        this.UO_View = this.conf.UO_View;
        this.UO_OnStop = this.conf.UO_OnStop;

        /*this.root = {
            nodeType: 'sync',
            text: 'Группа',
            draggable: false,
            id: "DirEmployee"
        };*/

        this.tbar = [
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/expand.png',
                tooltip: "Развернуть",
                itemId: "expandAll",
                id: "expandAll" + this.UO_id,
            },
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/collased.png',
                tooltip: "Свернуть",
                itemId: "collapseAll",
                id: "collapseAll" + this.UO_id,
            },

            "->",
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/folder_add.png',
                tooltip: "Новая категория",
                itemId: "FolderNew",
                id: "FolderNew" + this.UO_id,
            },
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,
                xtype: "button", disabled: true,
                icon: '../Scripts/sklad/images/folder_page.png',
                tooltip: "Новая подкатегория",
                itemId: "FolderNewSub",
                id: "FolderNewSub" + this.UO_id,
            },
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,
                xtype: "button", disabled: true,
                icon: '../Scripts/sklad/images/folder_link.png',
                tooltip: "Создать копию",
                itemId: "FolderCopy",
                id: "FolderCopy" + this.UO_id,
            },
            {
                UO_id: this.UO_id,
                UO_idMain: this.UO_idMain,
                UO_idCall: this.UO_idCall,
                xtype: "button", disabled: true,
                icon: '../Scripts/sklad/images/folder_delete.png',
                tooltip: "Удалить",
                itemId: "FolderDel",
                id: "FolderDel" + this.UO_id,
            }
        ];


        /*this.column = [
            { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
            { text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
            //this is so we know which column will show the tree
            { xtype: 'treecolumn', text: 'Наименование', flex: 1, sortable: true, dataIndex: 'text' },
            { text: 'Доступ', width: 25, dataIndex: 'Active', sortable: true }
        ]*/


        this.callParent(arguments);
    },



    viewConfig: {

        plugins: {
            ptype: 'treeviewdragdrop',
            //enableDrop: true,
            //enableDrag: true,
            //allowContainerDrop: true,
            //itemId: "treeviewdragdrop"

            //noAppend: true,
            appendOnly: true

            /*
            allowContainerDrops: true,
            dropZone: {
                // we want to always append child node to the hovered one
                // this behavior isn't supported out of the box by the plugin
                // so we override a template "onNodeDrop" method
                onNodeDrop: function (node) {
                    this.valid = true;
                    this.currentPosition = 'append';
                    this.overRecord = this.view.getRecord(node);
                    // call overridden method
                    return this.self.prototype.onNodeDrop.apply(this, arguments);
                }
            }
            */

        },
        getRowClass: function (record, index) {

            //2.  === Стили ===  ===  ===  ===  === 
            //Удалённые
            if (record.get('Del') == true) { return 'price-del'; }

            //Справочник "Контрагенты": красить зелёным, если есть договор
            if (this.grid.UO_View == "viewDirContractors") if (parseInt(record.get('DirContractID')) > 0) { return 'price-held_no_paid'; }


        }, //getRowClass

        stripeRows: true,

    }

});