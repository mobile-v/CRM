Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirNomens/viewDirNomenRemParties", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirNomenRemParties",

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

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {


        var PanelParty = Ext.create('widget.viewGridRem', {

            conf: {
                id: "gridParty_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
            },

            itemId: "gridParty",
            //title: "Партии товара",
            //collapsible: true,
            autoScroll: true,
            flex: 1,
            //split: true,
            store: this.storeRemPartiesGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 50 }, //, hidden: true
                //Товар
                { text: "Код", dataIndex: "DirNomenID", width: 50 },
                { text: "Найм.", dataIndex: "DirNomenName", width: 50 },
                { text: "Дата док", dataIndex: "DocDate", width: 75 },
                { text: "Дата приём", dataIndex: "DocDatePurches", width: 75 },

                { text: "Поставщик", dataIndex: "DirContractorName", width: 75 },
                { text: "Склад", dataIndex: "DirWarehouseName", width: 75 },
                { text: "Документ", dataIndex: "ListDocNameRu", width: 75 },

                { text: "Налог", dataIndex: "DirVatValue", width: 50, hidden: true },
                { text: "Поступило", dataIndex: "Quantity", width: 85 },
                { text: "Остаток", dataIndex: "Remnant", width: 85 },

                //{ text: "Цена в вал.", dataIndex: "PriceVAT", width: 85, hidden: true },
                { text: "Курс", dataIndex: "DirCurrencyRate", width: 50, hidden: true },
                { text: "Кратность", dataIndex: "DirCurrencyMultiplicity", width: 50, hidden: true },
                //{ text: "Цена прих", dataIndex: "PriceCurrency", width: 85, hidden: true },

                { text: "Цена Розница (в вал.)", dataIndex: "PriceRetailVAT", width: 85, hidden: true },
                { text: "Цена Розница", dataIndex: "PriceRetailCurrency", width: 85 }, //, hidden: true

                { text: "Цена Опт (в вал.)", dataIndex: "PriceWholesaleVAT", width: 85, hidden: true },
                { text: "Цена Опт", dataIndex: "PriceWholesaleCurrency", width: 85 }, //, hidden: true

                { text: "Цена ИМ (в вал.)", dataIndex: "PriceIMVAT", width: 85, hidden: true },
                { text: "Цена ИМ", dataIndex: "PriceIMCurrency", width: 85 }, //, hidden: true

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", width: 125 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                { text: "Поставщик", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true }
            ],

            //В Константах "нижнея панель" не нужна
            /*bbar: new Ext.PagingToolbar({
                store: this.storeRemPartiesGrid,                      // указано хранилище
                displayInfo: true,                          // вывести инфо обо общем числе записей
                displayMsg: lanShowing + "  {0} - {1} " + lanOf + " {2}"     // формат инфо
            }),*/

        });


        //Edit === === === === === === === === === === === === === ===
        var panelEdit = Ext.create('Ext.panel.Panel', {
            id: "panelEdit_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            //title: "Редактирование",
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

                        //*** Не видно *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                        { xtype: 'textfield', fieldLabel: "DirOrderIntTypeID", name: "DirOrderIntTypeID", readOnly: true, flex: 1, id: "DirOrderIntTypeID" + this.UO_id, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "Организация", name: "DirContractorIDOrg", readOnly: true, flex: 1, id: "DirContractorIDOrg" + this.UO_id, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "Склад", name: "DirWarehouseID", readOnly: true, flex: 1, id: "DirWarehouseID" + this.UO_id, allowBlank: true, hidden: true },
                        { xtype: 'viewDateField', fieldLabel: lanDate, name: "DocDate", id: "DocDate" + this.UO_id, width: 200, readOnly: true, allowBlank: false, editable: false, hidden: true },



                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                {
                                    xtype: "button",
                                    id: "btnOrder" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnOrder",
                                    text: lanOrder, icon: '../Scripts/sklad/images/add.png'
                                },

                                { xtype: 'textfield', margin: "0 0 0 15", name: "DirNomenPatchFull", id: "DirNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true },
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

                PanelParty

            ],

            buttons: [

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
                    UO_OnStop: this.UO_OnStop, //Что бы не было событий при перегрузке Дерева, то глючит (в контролере "controllerDirNomenRemParties" в методе "onTree_beforedrop" врубается ждущее событие "storeNomenTree.on(...)" и происходит перемещение объектов)
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

                /*listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirNomenRemParties_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirNomenRemParties_onTree_folderNewSub;
                        contextMenuTree.folderEdit = controllerDirNomenRemParties_onTree_folderEdit;
                        contextMenuTree.folderCopy = controllerDirNomenRemParties_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirNomenRemParties_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirNomenRemParties_onTree_folderSubNull;
                        contextMenuTree.addSub = controllerDirNomenRemParties_onTree_addSub;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }*/

            }),


            // *** *** *** *** *** *** *** *** ***

            panelEdit
            //tabPanel

        ],


        this.callParent(arguments);
    }

});