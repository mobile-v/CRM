Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocRetails/viewDocRetails", {
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocRetails",

    layout: "border",
    region: "center",
    title: "Чек -> Список",
    iconCls: 'fa fa-list-alt',
    width: 750, height: 350,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocRetails',

    listeners: { close: 'onViewDocRetailsClose' },

    conf: {},

    initComponent: function () {

        //body
        this.items = [

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
                    { text: "№", dataIndex: "DocRetailID", width: 50, tdCls: 'x-change-cell' },
                    { text: lanNumberInt, dataIndex: "NumberInt", hidden: true, tdCls: 'x-change-cell' },
                    { text: lanDate, dataIndex: "DocDate", sortable: true, width: 100, tdCls: 'x-change-cell' },
                    { text: lanOrg, dataIndex: "DirContractorNameOrg", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    { text: lanContractor, dataIndex: "DirContractorName", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    { text: lanWarehouse, dataIndex: "DirWarehouseName", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    { text: lanBase, dataIndex: "Base", hidden: true, tdCls: 'x-change-cell' },
                    { text: lanDisc, dataIndex: "Description", hidden: true, tdCls: 'x-change-cell' },

                    { text: lanSumOfVAT, dataIndex: "SumOfVATCurrency", sortable: true, width: 100, tdCls: 'x-change-cell' },
                    { text: lanSumVAT, dataIndex: "SumVATCurrency", hidden: true, tdCls: 'x-change-cell' },
                    { text: lanHavePay, dataIndex: "HavePay", width: 75, tdCls: 'x-change-cell' },
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