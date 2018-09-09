Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirNomens/viewDirNomens", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirNomens",

    layout: "border",
    region: "center",
    title: lanGoods,
    width: 775, height: 450,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;', //bodyStyle: 'opacity:0.5;',
    bodyPadding: varBodyPadding,

    //Контроллер
    controller: 'viewcontrollerDocAllEdit',

    conf: {},

    initComponent: function () {


        //Edit === === === === === === === === === === === === === ===
        var formEdit = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id,
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
                                { xtype: 'textfield', name: "DirNomenPatchFull", id: "DirNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true }, //, fieldLabel: ""
                                {
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload",
                                    id: "btnDirNomenReload" + this.UO_id, itemId: "btnDirNomenReload"
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
                                    emptyText: "Поиск ...", allowBlank: true, flex: 1,
                                    name: 'TriggerSearchTree', id: "TriggerSearchTree" + this.UO_id, itemId: "TriggerSearchTree"
                                }
                            ]
                        },

                    ]
                },

                {
                    xtype: 'viewDirNomensEdit',
                    storeDirNomenTypesGrid: this.storeDirNomenTypesGrid,
                    storeDirNomenCategoriesGrid: this.storeDirNomenCategoriesGrid,
                    storeDirCurrenciesGrid: this.storeDirCurrenciesGrid,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                },
            ]
        });



        //Info === === === === === === === === === === === === === ===

        var panelInfoRefresh = Ext.create('Ext.panel.Panel', {
            id: "panelInfoRefresh_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            bodyStyle: 'background:transparent;',
            //title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',

            defaults: { anchor: '100%' },
            //width: "100%", height: "100%", //width: 500, height: 200,
            width: "100%",
            autoScroll: true,

            items: [
                {
                    xtype: "button", icon: '../Scripts/sklad/images/refresh16.png', text: "Обновить данные", tooltip: "Обновить данные",
                    itemId: "btnGridsRefresh", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, UO_View: this.UO_View
                }
            ]
        });

        var PanelDirNomenHistories = Ext.create('widget.viewGridNoTBar', { //widget.viewGridDoc
            conf: {
                id: "PanelDirNomenHistories_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },
            itemId: "PanelDirNomenHistories",

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeDirNomenHistoriesGrid,

            columns: [
                //{ text: "№", dataIndex: "DirNomenHistoryID", width: 50 },
                { text: "№", dataIndex: "DirNomenID", width: 50, hidden: true }, //flex: 1
                { text: lanDate, dataIndex: "HistoryDate", flex: 1 },
                //{ text: lanDate, dataIndex: "HistoryDateTime", width: 100 },
                { text: lanPriceVat, dataIndex: "PriceVAT", width: 100 },
                { text: lanCurrency, dataIndex: "DirCurrencyName", width: 100 },
                { text: lanSurcharge + " %", dataIndex: "MarkupRetail", width: 100 },
                { text: lanRetailPrice, dataIndex: "PriceRetailVAT", width: 100 },
                { text: lanSurcharge + " %", dataIndex: "MarkupWholesale", width: 100 },
                { text: lanWholesalePrice, dataIndex: "PriceWholesaleVAT", width: 100 },
                { text: lanSurcharge + " %", dataIndex: "MarkupIM", width: 100 },
                { text: "IM", dataIndex: "PriceIMVAT", width: 100 }
            ],

            /*
            tbar: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanAddPosition, tooltip: lanAddPosition,
                    itemId: "btnGridAddPosition",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: lanEdit, tooltip: lanEdit, disabled: true,
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit"
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete"
                },
            ]
            */
        });

        var PanelRemParties = Ext.create('widget.viewGridNoTBar', { //widget.viewGridDoc
            conf: {
                id: "PanelRemParties_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },
            itemId: "PanelRemParties",

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 2,
            split: true,

            store: this.storeRemPartiesGrid,

            columns: [
                { text: "Партия", dataIndex: "RemPartyID", width: 85, hidden: true, tdCls: 'x-change-cell' },
                { text: "Док.", dataIndex: "ListDocNameRu", flex: 1, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
                { text: "Док.№", dataIndex: "NumberReal", width: 50, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },

                { text: "Код", dataIndex: "DirNomenID", width: 50, hidden: true },
                { text: "Дата док", dataIndex: "DocDate", width: 75 },
                { text: "Дата приём", dataIndex: "DocDatePurches", width: 75 },

                { text: "Поставщик", dataIndex: "DirContractorName", flex: 1 },
                { text: "Склад", dataIndex: "DirWarehouseName", flex: 1 },

                { text: "Налог", dataIndex: "DirVatValue", width: 50, hidden: true },
                { text: "Поступило", dataIndex: "Quantity", width: 85 },
                { text: "Остаток", dataIndex: "Remnant", width: 85 },

                { text: "Цена в вал.", dataIndex: "PriceVAT", width: 85, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 50, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 50, hidden: true },
                { text: "Цена прих", dataIndex: "PriceCurrency", width: 85 },

                { text: "Цена Розница (в вал.)", dataIndex: "PriceRetailVAT", width: 85, hidden: true },
                { text: "Цена", dataIndex: "PriceRetailCurrency", width: 85, hidden: true },

                { text: "Цена Опт (в вал.)", dataIndex: "PriceWholesaleVAT", width: 85, hidden: true },
                { text: "Цена", dataIndex: "PriceWholesaleCurrency", width: 85, hidden: true },

                { text: "Цена ИМ (в вал.)", dataIndex: "PriceIMVAT", width: 85, hidden: true },
                { text: "Цена", dataIndex: "PriceIMCurrency", width: 85, hidden: true },

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", flex: 1 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                { text: "Стиль", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true }
            ],

            /*
            tbar: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanAddPosition, tooltip: lanAddPosition,
                    itemId: "btnGridAddPosition",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: lanEdit, tooltip: lanEdit, disabled: true,
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit"
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete"
                },
            ]
            */
        });

        var PanelRemPartyMinuses = Ext.create('widget.viewGridNoTBar', { //widget.viewGridDoc
            conf: {
                id: "RemPartyMinuses_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },
            itemId: "RemPartyMinuses",

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 2,
            split: true,

            store: this.storeRemPartyMinusesGrid,

            columns: [
                //{ text: "Код", dataIndex: "DirNomenID", width: 50 },
                { text: "Дата", dataIndex: "DocDate", flex: 1 },

                { text: "Поставщик", dataIndex: "DirContractorName", flex: 1 },
                { text: "Склад", dataIndex: "DirWarehouseName", flex: 1 },
                { text: "Документ", dataIndex: "ListDocNameRu", flex: 1 },

                { text: "Налог", dataIndex: "DirVatValue", width: 50, hidden: true },
                { text: "Поступило", dataIndex: "Quantity", width: 85 },

                { text: "Цена в вал.", dataIndex: "PriceVAT", width: 85, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 50, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 50, hidden: true },
                { text: "Цена прод", dataIndex: "PriceCurrency", width: 85 },

                { text: "Резерв", dataIndex: "Reserve", width: 85, hidden: true },
            ],

            /*
            tbar: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanAddPosition, tooltip: lanAddPosition,
                    itemId: "btnGridAddPosition",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: lanEdit, tooltip: lanEdit, disabled: true,
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit"
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete"
                },
            ]
            */
        });


        var panelInfo = Ext.create('Ext.panel.Panel', {
            id: "panelInfo_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            disabled: true,
            title: "Информация",
            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "center", //!!! Важно для Ресайз-а !!!
            monitorValid: true,
            defaultType: 'textfield',
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
                split: true,
            },
            split: true,
            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,


            items: [
                //1. История цен отсортирован: ORDER BY id DESC
                //2. Партии
                //2.1. Список партий с остатком
                //2.2. Когда кликаем а партию внизу в таблице отображается список спианий с этой партии.

                panelInfoRefresh,

                PanelDirNomenHistories, 
                PanelRemParties,
                PanelRemPartyMinuses

            ]
        });



        //Tab-Panel
        var tabPanel = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,

            items: [
                formEdit, panelInfo
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
                    UO_OnStop: this.UO_OnStop, //Что бы не было событий при перегрузке Дерева, то глючит (в контролере "controllerDirNomens" в методе "onTree_beforedrop" врубается ждущее событие "storeNomenTree.on(...)" и происходит перемещение объектов)
                },

                store: this.storeGrid,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirNomen"
                },
                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    //{ text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Доступ', width: 50, dataIndex: 'Active', sortable: true },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'DirNomenPatchFull', dataIndex: 'DirNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],

                listeners: {

                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirNomens_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirNomens_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDirNomens_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDirNomens_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirNomens_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirNomens_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDirNomens_onTree_addSub;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    },

                    expand: function (theParentNode) {
                        theParentNode.eachChild(
                            function (node) {
                                if(nodeIWantSelected == node) {
                                    this.getView().focusRow(node);
                                }
                            }
                    )}

                }

            }),


            // *** *** *** *** *** *** *** *** ***


            tabPanel

        ],


        this.callParent(arguments);
    }

});