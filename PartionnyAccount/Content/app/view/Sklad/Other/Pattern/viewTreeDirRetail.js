//ComboBox для выбора справочника, в которых небольшое к-во записей и отсутствуют группы
//Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewTreeDirRetail", {
    extend: "Ext.tree.Panel",
    alias: "widget.viewTreeDirRetail",

    region: "west",
    loadMask: true,
    rootVisible: false,
    width: 275,
    useArrows: true,
    itemId: "tree",
    hlColor: "FF0000",
    hlBaseColor: "00FF00",
    hideHeaders: true,
    region: 'west',
    split: true,

    //collapsible: true,
    //title: "Товар",

    listeners: {
        selectionchange: 'onTree_selectionchange', itemclick: 'onTree_itemclick', itemdblclick: 'onTree_itemdblclick',

        afteritemexpand: function (nodeX, index, item, eOpts) {

            var viewTreeDir = this;
            //!!! Работает !!!
            item.scrollIntoView();
            //item.scrollIntoView(viewTreeDirRetail.view.el, false, true);

            //var nodeEl = Ext.get(this.view.getNode(index));
            //nodeEl.scrollIntoView(this.view.el, true, true);

            //viewTreeDir.ensureVisible(viewTreeDir.store.first());

            //1.
            /*nodeX.eachChild(
                function (node) {
                    viewTreeDir.getView().focusRow(node);
                }
            );*/
            //2.
            //viewTreeDir.getView().getEl().scrollTo('top', nodeX, true);
            //viewTreeDir.scrollBy('top', nodeX, true);
            //3.
            //viewTreeDir.body.scrollTo('top', 1250);
            //4.
            //viewTreeDir.scrollTo('top', 67);
            //5.
            //viewTreeDir.scrollBy(500, 500, true);
            //6.
            //viewTreeDir.scrollTo('top', 15);
            //7.
            //Ext.getBody().scrollTo('top', 0);
        }
    },

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
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/collapsible16.png',
                tooltip: "Спрятать",
                itemId: "collapsible",
                id: "collapsible" + this.UO_id,
                listeners: { click: 'onTree_collapsible' }
            },

            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                xtype: "button",
                icon: '../Scripts/sklad/images/expand.png',
                tooltip: "Развернуть",
                itemId: "expandAll",
                id: "expandAll" + this.UO_id,
                hidden: true,
                listeners: { click: 'onTree_expandAll' }
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
                listeners: { click: 'onTree_collapseAll' }
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
        /*
        plugins: {
            ptype: 'treeviewdragdrop',
            appendOnly: true
        },
        */
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