﻿Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSalaries/viewDocSalaries", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocSalaries",

    layout: "border",
    region: "center",
    title: lanJournalSales,
    width: 750, height: 350,
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

        //body
        this.items = [

            Ext.create('Ext.tree.Panel', {
                id: "tree_" + this.UO_id,  //WingetName + ObjectID
                UO_id: this.UO_id,         //ObjectID
                UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет

                title: "Фильтр",
                region: "west",
                loadMask: true,
                rootVisible: false,
                width: 150,
                split: true,
                collapsible: true,
                useArrows: true,
                itemId: "tree",

                root: {
                    text: 'Фильтр',
                    expanded: true,
                    children:
                    [
                        {
                            id: 0,
                            text: "Все",
                            leaf: true,
                            icon: '../Scripts/sklad/images/doc_all.png'
                        },
                        {
                            id: 1,
                            text: "Проведённые",
                            leaf: true,
                            icon: '../Scripts/sklad/images/doc_held.png'
                        },
                        {
                            id: 2,
                            text: "Не проведённые",
                            leaf: true,
                            icon: '../Scripts/sklad/images/doc_held_no.png'
                        },
                        {
                            id: 3,
                            text: "Импортированные",
                            leaf: true,
                            icon: '../Scripts/sklad/images/import.png'
                        }
                    ]
                }

            }),


            // *** *** *** *** *** *** *** *** ***


            Ext.create('widget.viewGridDoc', {

                conf: {
                    id: "grid_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

                store: this.storeGrid,

                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    { text: "№ desk", dataIndex: "DocID", width: 50, hidden: true, tdCls: 'x-change-cell' },
                    { text: lanDate, dataIndex: "DocDate", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    { text: lanBase, dataIndex: "Base", hidden: true, tdCls: 'x-change-cell' },
                    { text: lanDisc, dataIndex: "Description", hidden: true, tdCls: 'x-change-cell' },
                    { text: "№", dataIndex: "DocSalaryID", width: 50, tdCls: 'x-change-cell' }, //lanNumberSaleDocument
                    { text: lanOrg, dataIndex: "DirContractorNameOrg", hidden: true, sortable: true, flex: 1, tdCls: 'x-change-cell' },

                    { text: "Сумма ЗП", dataIndex: "SumSalary", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    { text: "Премия (продавца)", dataIndex: "DirBonusIDSalary", flex: 1, tdCls: 'x-change-cell' },
                    { text: "Премия (мастера)", dataIndex: "DirBonusID2Salary", flex: 1, tdCls: 'x-change-cell' },
                    { text: "Сумма", dataIndex: "Sums", flex: 1, tdCls: 'x-change-cell' },
                ],

                //В Константах "нижнея панель" не нужна
                bbar: new Ext.PagingToolbar({
                    store: this.storeGrid,                      // указано хранилище
                    displayInfo: true,                          // вывести инфо обо общем числе записей
                    displayMsg: lanShowing + "  {0} - {1} " + lanOf + " {2}"     // формат инфо
                }),

            }),

        ],


        this.callParent(arguments);
    }

});