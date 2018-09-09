Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirBanks/viewDirBanks", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirBanks",

    layout: "border",
    region: "center",
    title: lanBanks,
    width: 650, height: 350,
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
        
        //1. General-Panel
        var formPanelEdit = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanGeneral,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,
            autoHeight: true,

            items: [
                //ID
                { xtype: 'textfield', fieldLabel: "DirBankID", name: "DirBankID", id: "DirBankID" + this.UO_id, allowBlank: true, hidden: true }, //, readOnly: true
                //System record
                { xtype: "checkbox", boxLabel: "SysRecord", name: "SysRecord", id: "SysRecord" + this.UO_id, inputValue: true, hidden: true, readOnly: true },

                //Наименование
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanName, name: "DirBankName", id: "DirBankName" + this.UO_id, flex: 1, allowBlank: false }, //, readOnly: true
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //DirCurrency
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanCurrency, flex: 1, allowBlank: false, //, emptyText: "..."

                            store: this.storeDirCurrenciesGrid, // store getting items from server
                            valueField: 'DirCurrencyID',
                            hiddenName: 'DirCurrencyID',
                            displayField: 'DirCurrencyName',
                            name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirCurrencyEdit", id: "btnDirCurrencyEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-reload", itemId: "btnDirCurrencyReload", id: "btnDirCurrencyReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //Тип + Сумма покупок
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanMFO, name: "DirBankMFO", width: 210, allowBlank: true, flex: 1 },
                        { xtype: 'textfield', fieldLabel: lanPhone, margin: "0 0 0 10", name: "DirBankPhone", width: 210, allowBlank: true, flex: 1 }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //Тип + Сумма покупок
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanCurrAccount, name: "DirBankCurrAccount", width: 210, allowBlank: true, flex: 1 },
                        { xtype: 'textfield', fieldLabel: lanCorrAccount, margin: "0 0 0 10", name: "DirBankCorrAccount", width: 210, allowBlank: true, flex: 1 }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //Тип + Сумма покупок
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanTIN, name: "DirBankTIN", width: 210, allowBlank: true, flex: 1 },
                        { xtype: 'textfield', fieldLabel: lanSWIFT, margin: "0 0 0 10", name: "DirBankSWIFT", width: 210, allowBlank: true, flex: 1 }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: lanDescription, name: "Description", flex: 1, allowBlank: true },
            ],


            //buttonAlign: 'left',
            buttons: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                    text: lanSave, icon: '../Scripts/sklad/images/save.png'
                },
                " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
                },

                "-",

                {
                    id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHistory",
                    text: lanHistory, icon: '../Scripts/sklad/images/history.png',
                    disabled: true
                },

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                },


                /*
                { xtype: 'viewDateField', fieldLabel: "", width: 90, name: "HistoryDate", id: "HistoryDate" + this.UO_id, allowBlank: true, editable: false },

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnChange",
                    text: lanChange, icon: '../Scripts/sklad/images/change.png'
                },
                */
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
                },

                store: this.storeGrid,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirBank"
                },

                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    { text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Доступ', width: 50, dataIndex: 'Active', sortable: true },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                ],

                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirBanks_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirBanks_onTree_folderNewSub;
                        contextMenuTree.folderCopy = controllerDirBanks_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirBanks_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirBanks_onTree_folderSubNull;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }

            }),


            // *** *** *** *** *** *** *** *** ***


            formPanelEdit

        ],


        this.callParent(arguments);
    }

});