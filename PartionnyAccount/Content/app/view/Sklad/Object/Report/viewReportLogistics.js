Ext.define("PartionnyAccount.view.Sklad/Object/Report/viewReportLogistics", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewReportLogistics",

    layout: "border",
    region: "center",
    title: "Отчет по Логистике",
    width: 650, height: 240,
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

        //Form-Panel
        var formPanel = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)


            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "north", //!!! Важно для Ресайз-а !!!
            monitorValid: true,
            defaultType: 'textfield',

            //layout: 'border',
            //defaults: { anchor: '100%' },
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
                split: true,
            },
            split: true,

            width: "100%", height: 90,
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'viewDateField', fieldLabel: "С", labelAlign: 'top', name: "DateS", id: "DateS" + this.UO_id, allowBlank: false },
                        { xtype: 'viewDateField', fieldLabel: "по", labelAlign: 'top', name: "DatePo", id: "DatePo" + this.UO_id, margin: "0 0 0 25", allowBlank: false },

                        //Не видимые!!!
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Организация", labelAlign: 'top', flex: 2, allowBlank: false, //, emptyText: "..."

                            store: this.storeDirContractorsOrgGrid, // store getting items from server
                            valueField: 'DirContractorID',
                            hiddenName: 'DirContractorID',
                            displayField: 'DirContractorName',
                            name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //disabled: true
                            //editable: false, typeAhead: false, minChars: 200,
                            hidden: true
                        },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Курьер", labelAlign: 'top', flex: 1, allowBlank: true, //, emptyText: "..."
                            margin: "0 0 0 25",
                            store: this.storeDirEmployeesGrid, // store getting items from server
                            valueField: 'DirEmployeeID',
                            hiddenName: 'DirEmployeeID',
                            displayField: 'DirEmployeeName',
                            name: 'DirEmployeeID', itemId: "DirEmployeeID", id: "DirEmployeeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Статус", labelAlign: 'top', flex: 1, allowBlank: true, //, emptyText: "..."
                            margin: "0 0 0 25",
                            store: this.storeDirMovementStatusesGrid, // store getting items from server
                            valueField: 'DirMovementStatusID',
                            hiddenName: 'DirMovementStatusID',
                            displayField: 'DirMovementStatusName',
                            name: 'DirMovementStatusID', itemId: "DirMovementStatusID", id: "DirMovementStatusID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            editable: false, typeAhead: false, minChars: 200,
                        },


                        {
                            xtype: 'radiogroup',
                            itemId: "DocOrTab", id: "DocOrTab" + this.UO_id, iUO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //fieldLabel: 'Single Column',
                            columns: 1,
                            items: [
                                { boxLabel: 'Документы', name: 'DocOrTab', inputValue: 1, checked: true },
                                { boxLabel: 'Товар', name: 'DocOrTab', inputValue: 2 }
                            ]
                        }

                        //{ xtype: "checkbox", boxLabel: "Документы / Товар", name: "DocOrTab", itemId: "DocOrTab", width: 100, id: "DocOrTab" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                    ]
                },


            ],

            buttons: [
                {
                    text: lanPrint, icon: '../Scripts/sklad/images/print.png',
                    menu: [
                        {
                            id: "btnPrintRu" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintRu",
                            text: lanLanguageRu, UO_Language: 0,
                            icon: '../Scripts/sklad/images/Flag/ru.png'
                        },
                        {
                            id: "btnPrintUa" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintUa",
                            text: lanLanguageUa, UO_Language: 1,
                            icon: '../Scripts/sklad/images/Flag/ua.png'
                        }
                    ]
                },
                " ",
                {
                    id: "btnCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                    text: lanCancel, icon: '../Scripts/sklad/images/cancel.png', UO_Action: "cancel",
                },


                "->",

                {
                    id: "btnDocMovementsEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDocMovementsEdit",
                    text: "Сформировать перемещение", icon: '../Scripts/sklad/images/doc_of.png', hidden: true
                },
                " ",
                {
                    id: "btnReport" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnReport",
                    text: lanReport, icon: '../Scripts/sklad/images/reports.png', UO_Action: "report",
                },
            ],

        });


        //2. Грид
        var PanelGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            itemId: "grid",

            conf: {},

            region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeReportLogistics,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [
                { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },

                { text: "№", dataIndex: "DocMovementID", width: 50, hidden: true },
                { text: "Курьер", dataIndex: "DirEmployeeName", flex: 1, hidden: true },
                { text: "Дата", dataIndex: "DocDate", width: 85, hidden: true },
                { text: "СтатусID", dataIndex: "DirMovementStatusID", width: 50, hidden: true },
                { text: "Статус", dataIndex: "DirMovementStatusName", flex: 1, hidden: true },

                //{ text: "Точка от", dataIndex: "DirWarehouseName", flex: 1, hidden: true },
                { text: lanWarehouseFrom, dataIndex: "DirWarehouseNameFrom", sortable: true, flex: 1, tdCls: 'x-change-cell-3' },
                { text: "Курьер", dataIndex: "DirEmployeeNameCourier", sortable: true, flex: 1, tdCls: 'x-change-cell-4' },
                { text: lanWarehouseTo, dataIndex: "DirWarehouseNameTo", sortable: true, flex: 1, tdCls: 'x-change-cell-5' },

                { text: "Код", dataIndex: "DirNomenID", width: 75, hidden: true },
                { text: "Категория", dataIndex: "DirNomenPatchFull", flex: 1, hidden: true },
                { text: "Наименование", dataIndex: "DirNomenName", flex: 1, hidden: true },
                { text: "К-во", dataIndex: "Sale_Quantity", width: 100, hidden: true },
                { text: "Х-ки", dataIndex: "DirChar", flex: 1, hidden: true },
            ],



            
            viewConfig: {
                getRowClass: function (record, index) {

                    // 1. === Исправляем формат даты: "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"  ===  ===  ===  ===  === 
                    for (var i = 0; i < record.store.model.fields.length; i++) {
                        //Если поле типа "Дата"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }
                    }


                    //Логистика: в ожидании курьера
                    if (parseInt(record.get('DirMovementStatusID')) == 2) { return 'movements-2'; }
                        //Логистика: курьер принял
                    else if (parseInt(record.get('DirMovementStatusID')) == 3) { return 'movements-3'; }
                        //Логистика: курьер отдал
                    else if (parseInt(record.get('DirMovementStatusID')) == 4) { return 'movements-4'; }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig

        });


        //body
        this.items = [
            {
                xtype: "panel",
                layout: 'border',
                region: "center",
                items: [
                    formPanel,
                    PanelGrid
                ]
            },

        ],


        this.callParent(arguments);
    }

});