Ext.define("PartionnyAccount.view.Sklad/Object/List/viewListObjectPFs", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewListObjectPFs",

    layout: "border",
    region: "center",
    title: lanPrintForms,
    width: 450, height: 250,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    conf: {},

    initComponent: function () {

        //body
        this.items = [

            Ext.create('widget.viewGridPF', {

                conf: {
                    id: "grid_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

                store: this.storeGrid,

                columns: [
                    { text: "№", dataIndex: "ListObjectID", hidden: true, tdCls: 'x-change-cell' },
                    //{ text: "№", dataIndex: "ListObjectPFID", hidden: true, tdCls: 'x-change-cell' },
                    //{ text: "№", dataIndex: "ListLanguageID", hidden: true, tdCls: 'x-change-cell' },
                    //{ text: "№", dataIndex: "ListLanguageNameSmall", hidden: true, tdCls: 'x-change-cell' },
                    //{ text: "", dataIndex: "SysRecord", hidden: true, tdCls: 'x-change-cell2' },
                    { text: "Del", dataIndex: "Del", hidden: true, tdCls: 'x-change-cell' }, { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    { text: lanName, dataIndex: "ListObjectPFName", sortable: true, flex: 1, tdCls: 'x-change-cell' }
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