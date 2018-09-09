Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirServiceNomens/viewDirServiceNomens", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirServiceNomens",

    layout: "border",
    region: "center",
    title: "Устройства",
    width: 775, height: 450,
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


        //Edit === === === === === === === === === === === === === ===
        var SearchType_values = [
            [1, 'В товаре (код, наименование)']
        ];
        var panelEdit = Ext.create('Ext.panel.Panel', {
            id: "panelEdit_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            title: "Редактирование",
            region: "center",
            layout: 'border', // тип лэйоута - трехколонник с подвалом и шапкой
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,

            items: [
                {
                    xtype: "panel",
                    region: "north", //!!! Важно для Ресайз-а !!!
                    bodyStyle: 'background:transparent;',
                    //title: lanGeneral,
                    frame: true,
                    monitorValid: true,
                    defaultType: 'textfield',
                    //width: "100%", height: "100%", //width: 500, height: 200,
                    bodyPadding: 5,
                    layout: 'anchor',
                    defaults: { anchor: '100%' },
                    autoScroll: true,

                    items: [
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', name: "DirServiceNomenPatchFull", id: "DirServiceNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true },
                                {
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload",
                                    id: "btnDirServiceNomenReload" + this.UO_id, itemId: "btnDirServiceNomenReload"
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
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },

                                //"->",
                                {
                                    id: "TriggerSearchTree" + this.UO_id,
                                    UO_id: this.UO_id,
                                    UO_idMain: this.UO_idMain,
                                    UO_idCall: this.UO_idCall,

                                    xtype: 'viewTriggerSearch',
                                    //fieldLabel: lanGroup,
                                    emptyText: "Поиск ...",
                                    name: 'TriggerSearchTree',
                                    id: "TriggerSearchTree" + this.UO_id, itemId: "TriggerSearchTree",
                                    allowBlank: true,
                                    flex: 1
                                }

                            ]
                        },
                    ]
                },


                {
                    xtype: "form",
                    id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    layout: "border",
                    region: "center",

                    items: [
                        {
                            xtype: 'viewDirServiceNomensEdit',
                            storeDirNomenTypesGrid: this.storeDirNomenTypesGrid,
                            storeDirServiceNomenCategoriesGrid: this.storeDirServiceNomenCategoriesGrid,
                            storeDirServiceNomenPricesGrid: this.storeDirServiceNomenPricesGrid,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },

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
                    UO_OnStop: this.UO_OnStop, //Что бы не было событий при перегрузке Дерева, то глючит (в контролере "controllerDirServiceNomens" в методе "onTree_beforedrop" врубается ждущее событие "storeNomenTree.on(...)" и происходит перемещение объектов)
                },

                store: this.storeGrid,

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
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Доступ', width: 50, dataIndex: 'Active', sortable: true },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'DirServiceNomenPatchFull', dataIndex: 'DirServiceNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],

                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirServiceNomens_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirServiceNomens_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDirServiceNomens_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDirServiceNomens_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirServiceNomens_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirServiceNomens_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDirServiceNomens_onTree_addSub;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }

            }),


            // *** *** *** *** *** *** *** *** ***


            panelEdit

        ],


        this.callParent(arguments);
    }

});