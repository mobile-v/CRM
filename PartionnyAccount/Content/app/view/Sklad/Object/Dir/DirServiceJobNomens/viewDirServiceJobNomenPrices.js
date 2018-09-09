Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirServiceJobNomens/viewDirServiceJobNomenPrices", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirServiceJobNomenPrices",

    layout: "border",
    region: "center",
    title: "Выполненные работы с ценами",
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


        var PanelDirServiceJobNomenHistories = Ext.create('widget.viewGridNoTBar', { //widget.viewGridDoc
            conf: {
                id: "grid_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },
            itemId: "grid",

            region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeDirServiceJobNomenHistoriesGrid,

            columns: [
                //{ text: "№", dataIndex: "DirNomenID", width: 50, hidden: true }, //flex: 1
                { text: lanCurrency, dataIndex: "DirCurrencyName", flex: 1 },
                //{ text: "Цена №1", dataIndex: "PriceVAT", width: 100 },
                { text: lanRetailPrice3, dataIndex: "PriceRetailCurrency", flex: 1 },
                //{ text: lanWholesalePrice3, dataIndex: "PriceWholesaleCurrency", flex: 1 },
                //{ text: lanOnlineShop, dataIndex: "PriceIMCurrency", flex: 1 }
            ],

        });

        
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

                                { xtype: 'textfield', name: "DirServiceJobNomenPatchFull", id: "DirServiceJobNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true },
                                {
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload",
                                    id: "btnDirJobNomenReload" + this.UO_id, itemId: "btnDirJobNomenReload"
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
                                },

                                {
                                    xtype: 'viewComboBox',
                                    //fieldLabel: "Тип цены",
                                    flex: 2, allowBlank: false,
                                    margin: "0 0 0 15",
                                    store: this.storeDirPriceTypesGrid, // store getting items from server
                                    valueField: 'DirPriceTypeID',
                                    hiddenName: 'DirPriceTypeID',
                                    displayField: 'DirPriceTypeName',
                                    name: 'DirPriceTypeID', itemId: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },

                            ]
                        },
                    ]
                },

                PanelDirServiceJobNomenHistories,

                //PanelFooter
            ],

            buttons: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSelect",
                    text: "Выбрать", icon: '../Scripts/sklad/images/add.png'
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
                    UO_OnStop: this.UO_OnStop, //Что бы не было событий при перегрузке Дерева, то глючит (в контролере "controllerDirServiceJobNomens" в методе "onTree_beforedrop" врубается ждущее событие "storeNomenTree.on(...)" и происходит перемещение объектов)
                },

                store: this.storeGrid,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirServiceJobNomen"
                },

                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    //{ text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Доступ', width: 50, dataIndex: 'Active', sortable: true },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'DirServiceJobNomenPatchFull', dataIndex: 'DirServiceJobNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],

                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirServiceJobNomenPrices_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirServiceJobNomenPrices_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDirServiceJobNomenPrices_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDirServiceJobNomenPrices_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirServiceJobNomenPrices_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirServiceJobNomenPrices_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDirServiceJobNomenPrices_onTree_addSub;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }

            }),


            // *** *** *** *** *** *** *** *** ***


            panelEdit,
            { xtype: 'textfield', fieldLabel: "DirServiceJobNomenType", name: "DirServiceJobNomenType", id: "DirServiceJobNomenType" + this.UO_id, allowBlank: true, hidden: true },

        ],


        this.callParent(arguments);
    }

});