Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocOrderInts/viewDocOrderIntsNomensEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocOrderIntsNomensEdit",

    layout: "border",
    region: "center",
    title: lanOrder + "(2)",
    width: 700, height: 475,
    autoScroll: false,

    UO_maximize: false, //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом
    UO_Modal: true,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {

        //body
        this.items = [

            //formPanelEdit

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
                    }
                }

            }),


            // *** *** *** *** *** *** *** *** ***


            {
                xtype: 'viewDocOrderIntsPattern',
                id: 'viewDocOrderIntsPattern' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                storeDirPriceTypesGrid: this.storeDirPriceTypesGrid,

                storeDirServiceNomensGrid: this.storeDirServiceNomensGrid,
                storeDirNomensGrid1: this.storeDirNomensGrid1,
                storeDirNomensGrid2: this.storeDirNomensGrid2,
                storeDirNomensGrid3: this.storeDirNomensGrid3,
                //storeDirNomensGrid4: this.storeDirNomensGrid4,
                //storeDirNomensGrid5: this.storeDirNomensGrid5,
                //storeDirNomensGrid6: this.storeDirNomensGrid6,

                storeDirContractorsGrid: this.storeDirContractorsGrid,

                //storeDirNomenCategoriesGrid: this.storeDirNomenCategoriesGrid,

                storeDirCharColoursGrid: this.storeDirCharColoursGrid,
                storeDirCharMaterialsGrid: this.storeDirCharMaterialsGrid,
                storeDirCharNamesGrid: this.storeDirCharNamesGrid,
                storeDirCharSeasonsGrid: this.storeDirCharSeasonsGrid,
                storeDirCharSexesGrid: this.storeDirCharSexesGrid,
                storeDirCharSizesGrid: this.storeDirCharSizesGrid,
                storeDirCharStylesGrid: this.storeDirCharStylesGrid,
                storeDirCharTexturesGrid: this.storeDirCharTexturesGrid,

                storeDirCurrenciesGrid: this.storeDirCurrenciesGrid,
            },

        ],


        this.callParent(arguments);
    }

});