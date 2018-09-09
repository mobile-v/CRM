Ext.define("PartionnyAccount.view.Sklad/Object/Pay/viewPay", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewPay",

    layout: "border",
    region: "center",
    title: "Оплата",
    width: 550, height: 250,
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
                    //Общий номер документа
                    { text: "№", dataIndex: "DocID", sortable: true, width: 35, hidden: true, tdCls: 'x-change-cell' },
                    //DocXID - реальный номер документа
                    { text: "№", dataIndex: "DocXID", sortable: true, width: 35, hidden: true, tdCls: 'x-change-cell' },
                    //ID-шник: DocCashOfficeSumID or DocBankSumID
                    { text: "№", dataIndex: "DocCashBankID", width: 35, tdCls: 'x-change-cell' },
                    //ID-шник: Касса или Банка
                    { text: lanName, dataIndex: "DirPaymentTypeID", sortable: true, width: 35, hidden: true, tdCls: 'x-change-cell' },
                    //Наименование: Касса или Банка
                    { text: lanName, dataIndex: "DirXName", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    //Сотрудник внёсший запись
                    { text: lanEmployee, dataIndex: "DirEmployeeName", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    //Тип операции в Кассе или Банке
                    { text: lanOperation, dataIndex: "DirXSumTypeName", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    //Дата записи в Кассу или Банк
                    { text: lanDate, dataIndex: "DocXSumDate", sortable: true, width: 75, tdCls: 'x-change-cell' },
                    //Сумма
                    { text: lanSum, dataIndex: "DocXSumSum", sortable: true, flex: 1, tdCls: 'x-change-cell' },
                    //Валюта
                    { text: lanCurrency, dataIndex: "DirCurrencyName", sortable: true, flex: 1, hidden: true, tdCls: 'x-change-cell' },
                    //Курс
                    { text: lanRate, dataIndex: "DirCurrencyRate", hidden: true, tdCls: 'x-change-cell' },
                    //Кратность
                    { text: lanMultiplicity, dataIndex: "DirCurrencyMultiplicity", hidden: true, tdCls: 'x-change-cell' },
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