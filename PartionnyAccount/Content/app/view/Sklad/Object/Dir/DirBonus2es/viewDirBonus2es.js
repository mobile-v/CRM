Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirBonus2es/viewDirBonus2es", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirBonus2es",

    layout: "border",
    region: "center",
    title: lanBonus + " (" + lanEmployees + ")",
    width: 650, height: 350,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {

        //1. General-Panel
        var PanelGeneral = Ext.create('Ext.panel.Panel', {
            id: "PanelGeneral_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "north", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanGeneral,
            bodyPadding: 5,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "50", //width: 500, height: 200,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            //margin: "0 5 0 0",

            items: [
                //ID
                { xtype: 'textfield', fieldLabel: "DirBonus2ID", name: "DirBonus2ID", id: "DirBonus2ID" + this.UO_id, hidden: true }, //, readOnly: true
                //System record
                { xtype: "checkbox", boxLabel: "SysRecord", name: "SysRecord", id: "SysRecord" + this.UO_id, inputValue: true, hidden: true, readOnly: true },


                { xtype: 'textfield', fieldLabel: lanName, name: "DirBonus2Name", id: "DirBonus2Name" + this.UO_id, flex: 1, allowBlank: false },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: lanDesc, name: "Description", id: "Description" + this.UO_id, flex: 1, allowBlank: true },
            ]
        });


        //2. Tab
        var rowEditing = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGridBonusTab = Ext.create('Ext.grid.Panel', {
            id: "PanelGridBonusTab_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center",
            loadMask: true,
            //autoScroll: true,
            //touchScroll: true,

            title: lanGraduation,
            itemId: "PanelGridBonusTab_grid",

            store: this.storeDirBonus2TabsGrid,

            columns: [
                //{ text: "№", dataIndex: "DirBonus2TabID", hidden: true },
                //{ text: "№", dataIndex: "DirBonus2ID", hidden: true },
                { text: lanSumOf, dataIndex: "SumBegin", flex: 1, editor: { xtype: 'textfield' } },
                { text: lanBonus + " %", dataIndex: "Bonus", flex: 1, editor: { xtype: 'textfield' } }
            ],

            tbar: [
                {
                    UO_id: this.UO_id,
                    UO_idMain: this.UO_idMain,
                    UO_idCall: "PanelGridBonusTab_" + this.UO_id,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', tooltip: lanNewM,
                    itemId: "PanelGridBonusTab_btnNew"
                },
                {
                    UO_id: this.UO_id,
                    UO_idMain: this.UO_idMain,
                    UO_idCall: "PanelGridBonusTab_" + this.UO_id,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', tooltip: lanDelete, disabled: true,
                    itemId: "PanelGridBonusTab_btnDelete"
                }

            ],

            plugins: [rowEditing],
            rowEditing: rowEditing,

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

            layout: 'border',
            defaults: { anchor: '100%' },

            region: "center",
            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,


            items: [
                PanelGeneral, PanelGridBonusTab
            ],


            buttons: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                    text: lanSave, icon: '../Scripts/sklad/images/save.png'
                },
                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
                },

                "-",

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                }
            ]

        });



        //body
        this.items = [

            Ext.create('widget.viewTreeDir', {

                conf: {
                    id: "tree_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

                store: this.storeGrid,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirBonus2"
                },

                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    { text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Доступ', width: 50, dataIndex: 'Active', sortable: true },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                ],

                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirBonus2es_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirBonus2es_onTree_folderNewSub;
                        contextMenuTree.folderCopy = controllerDirBonus2es_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirBonus2es_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirBonus2es_onTree_folderSubNull;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }

            }),


            // *** *** *** *** *** *** *** *** ***


            formPanel

        ],


        this.callParent(arguments);
    }

});