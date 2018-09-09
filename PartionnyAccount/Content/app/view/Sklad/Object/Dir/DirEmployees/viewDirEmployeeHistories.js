//Строим класс со всеми виджетами: Sys.DirEmployee
Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirEmployees/viewDirEmployeeHistories", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirEmployeeHistories",

    layout: "border",
    region: "center",
    title: lanEmployees,
    width: 650, height: 320,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {
        //id: "viewDirEmployeeHistories_" + this.UO_id,
        //UO_id: this.UO_id,

        //body
        this.items = [

            Ext.create('widget.viewGridDir', {

                conf: {
                    id: "grid_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                },

                //hidden: true,
                /*region: "center",
                loadMask: true,
                autoScroll: true,
                touchScroll: true,
                itemId: "grid",*/

                store: this.storeGrid,

                columns: [
                    { text: "№", dataIndex: "DirEmployeeHistoryID", hidden: true },
                    { text: lanDate, dataIndex: "HistoryDate", sortable: true, flex: 1 },
                    { text: lanCurrency, dataIndex: "DirCurrencyName", sortable: true, flex: 1 },
                    { text: lanSalary, dataIndex: "Salary", sortable: true, width: 75 },
                    { text: lanBonus, dataIndex: "DirBonusName", sortable: true, flex: 1 },
                    { text: lanSalaryDayMonthly, dataIndex: "SalaryDayMonthly", sortable: true, width: 75 }
                ],


                //В Константах "нижнея панель" не нужна
                /*
                bbar: new Ext.PagingToolbar({
                    store: this.storeGrid,                          // указано хранилище
                    displayInfo: true,                          // вывести инфо обо общем числе записей
                    displayMsg: lanShowing + "  {0} - {1} " + lanOf + " {2}"     // формат инфо
                }),
                */

            }),

        ],


        this.callParent(arguments);
    }

});