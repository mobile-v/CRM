Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirBanks/viewDirBanksGrid", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirBanksGrid",

    layout: "border",
    region: "center",
    title: lanBanks,
    width: 550, height: 300,
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

            Ext.create('widget.viewGridDir', {

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
                    { text: "№", dataIndex: "DirBankID", width: 50, tdCls: 'x-change-cell' }, //lanNumberPurchDocument
                    { text: "Наименование", dataIndex: "DirBankName", sortable: true, flex: 1, tdCls: 'x-change-cell' }, //lanNumberPurchDocument
                    { text: "Валюта", dataIndex: "DirCurrencyName", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    { text: lanSum, dataIndex: "DirBankSum", sortable: true, flex: 1, tdCls: 'x-change-cell' }
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