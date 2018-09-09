Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocMovements/viewDocMovementsLogistics", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDocMovementsLogistics",

    layout: "border",
    region: "center",
    title: "Логистика",
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

        // 0. Все
        var PanelGrid0 = Ext.create('widget.viewGridMovementsLogistics', {
            conf: {
                id: "PanelGrid0_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            itemId: "PanelGrid0_",
            store: this.storeGrid0,
            title: "Все",
        });
        // 2. В ожидании курьера
        var PanelGrid2 = Ext.create('widget.viewGridMovementsLogistics', {
            conf: {
                id: "PanelGrid2_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            itemId: "PanelGrid2_",
            store: this.storeGrid2,
            title: "В ожидании курьера",
        });
        // 3. Курьер принял
        var PanelGrid3 = Ext.create('widget.viewGridMovementsLogistics', {
            conf: {
                id: "PanelGrid3_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            itemId: "PanelGrid3_",
            store: this.storeGrid3,
            title: "Курьер принял",
        });
        // 4. Архив
        var PanelGrid9 = Ext.create('widget.viewGridMovementsLogistics', {
            conf: {
                id: "PanelGrid9_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            },

            tbar: [
                {
                    id: "TriggerSearchGrid" + this.UO_id,
                    UO_id: this.UO_id,
                    UO_idMain: this.UO_idMain,
                    UO_idCall: this.UO_idCall,

                    xtype: 'viewTriggerSearchGrid',
                    //fieldLabel: lanGroup,
                    emptyText: "Поиск ...",
                    name: 'TriggerSearchGrid',
                    id: "TriggerSearchGrid" + this.UO_id, itemId: "TriggerSearchGrid",
                    allowBlank: true,
                },

                { xtype: 'label', forId: "DateS" + this.UO_id, text: lanWith, margin: "0 0 0 15" },
                {
                    UO_id: this.UO_id,
                    UO_idMain: this.UO_idMain,
                    UO_idCall: this.UO_idCall,
                    margin: "0 0 0 5",
                    xtype: "viewDateFieldFix", name: "DateS", id: "DateS" + this.UO_id, itemId: "DateS", width: 100, editable: false,
                },

                { xtype: 'label', forId: "DatePo" + this.UO_id, text: lanTo, margin: "0 0 0 5" },
                {
                    UO_id: this.UO_id,
                    UO_idMain: this.UO_idMain,
                    UO_idCall: this.UO_idCall,
                    margin: "0 0 0 5",
                    xtype: "viewDateFieldFix", name: "DatePo", id: "DatePo" + this.UO_id, itemId: "DatePo", width: 100, editable: false,
                }

            ],

            itemId: "PanelGrid9_",
            store: this.storeGrid4,
            title: "Архив",
        });


        //Tab-Panel
        var tabPanel = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            width: "100%", height: "100%",
            autoHeight: true,
            split: true,

            items: [
                PanelGrid0, PanelGrid2, PanelGrid3, PanelGrid9
            ],

            listeners: {
                tabchange: function (tabPanel, newTab, oldTab, index) {
                    
                    var widgetXForm = Ext.getCmp("form_" + tabPanel.UO_id).setVisible(false);
                    Ext.getCmp("btnStatus1" + tabPanel.UO_id).setVisible(false);
                    Ext.getCmp("btnStatus2" + tabPanel.UO_id).setVisible(false);
                    Ext.getCmp("btnStatus3" + tabPanel.UO_id).setVisible(false);
                    Ext.getCmp("btnStatus4" + tabPanel.UO_id).setVisible(false);
                    Ext.getCmp("btnTabShow" + tabPanel.UO_id).setVisible(false);
                    Ext.getCmp("btnDocEdit" + tabPanel.UO_id).setVisible(false);
                    
                    if (newTab.itemId != "PanelGrid9_") { newTab.store.load({ waitMsg: lanLoading }); }
                    
                }
            },

        });



        //3. Таблицы

        //3.1. Табличная часть документа
        var PanelGrid1 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "grid1",
            id: "grid1_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            title: "Спецификация",
            region: "east",
            flex: 1,
            hideHeaders: true,
            hidden: true,
            store: this.storeDocMovementTabsGrid, //storeDocAccountTabsGrid,

            columns: [
                //Партия
                { text: "Партия", dataIndex: "RemPartyID", width: 50, hidden: true },
                //Товар
                { text: "№", dataIndex: "DirNomenID", width: 50, style: "height: 25px;" },
                { text: lanNomenclature, dataIndex: "DirNomenName", flex: 1 },
                //К-во
                { text: lanCount, dataIndex: "Quantity", width: 75 },
                //Цены
                //{ text: "Розница Наценка", dataIndex: "MarkupRetail", width: 100, hidden: true },
                //{ text: "Розница Цена", dataIndex: "PriceRetailCurrency", width: 100, hidden: true },
                //{ text: "Опт Наценка", dataIndex: "MarkupWholesale", width: 100, hidden: true },
                //{ text: "Опт Цена", dataIndex: "PriceWholesaleCurrency", width: 100, hidden: true },
                //{ text: "IM Наценка", dataIndex: "MarkupIM", width: 100, hidden: true },
                //{ text: "IM Цена", dataIndex: "PriceIMCurrency", width: 100, hidden: true },
                //Суммы
                //{ text: lanPriceVatFull, dataIndex: "PriceCurrency", width: 100 },
                //{ text: lanSum, dataIndex: "SUMPurchPriceVATCurrency", width: 100 },

                //Характеристики
                { text: "Характеристики", dataIndex: "DirChar", flex: 1 },
                { text: "Цвет", dataIndex: "DirCharColourName", width: 100, hidden: true },
                { text: "Производитель", dataIndex: "DirCharMaterialName", width: 100, hidden: true },
                { text: "Имя", dataIndex: "DirCharNameName", width: 100, hidden: true },
                { text: "Сезон", dataIndex: "DirCharSeasonName", width: 100, hidden: true },
                { text: "Пол", dataIndex: "DirCharSexName", width: 100, hidden: true },
                { text: "Размер", dataIndex: "DirCharSizeName", width: 100, hidden: true },
                { text: "Поставщик", dataIndex: "DirCharStyleName", width: 100, hidden: true },
                { text: "Текстура", dataIndex: "DirCharTextureName", width: 100, hidden: true },
                { text: "Серийный", dataIndex: "SerialNumber", width: 100, hidden: true },
                { text: "Штрих-Код", dataIndex: "Barcode", width: 100, hidden: true },

                { text: "Тип возвр.", dataIndex: "DirReturnTypeName", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },
                { text: "Причина", dataIndex: "DirDescriptionName", width: 85, style: "height: 25px;", tdCls: 'x-change-cell' },
            ],
        });

        //3.2. Лог
        var PanelGridLog0 = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            itemId: "gridLog0",
            id: "gridLog0_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            split: true,
            region: "center",
            flex: 1,
            //split: true,
            store: this.storeLogMovementsGrid0, //storeDocAccountTabsGrid,
            title: "Лог",

            columns: [
                {
                    dataIndex: "Field1", flex: 1,
                    renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                        metaData.style = "white-space: normal;";
                        return value;
                    }
                },
            ],

            viewConfig: {
                getRowClass: function (record, index) {


                    for (var i = 0; i < record.store.model.fields.length; i++) {


                        //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:sO");
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }




                        //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                        var FN = record.store.model.fields[i].name;
                        if (FN == "DirMovementLogTypeName" || FN == "DirMovementStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                            if (record.data[record.store.model.fields[i].name] != null) {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];

                                if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                                else record.data["Field1"] += " - ";
                            }
                        }
                        else if (FN == "LogMovementDate") {
                            record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                        }

                    }


                }, //getRowClass

                stripeRows: true,

            } //viewConfig

        });



        //4. Футер
        var PanelFooter = Ext.create('Ext.panel.Panel', {
            id: "PanelFooter_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            bodyStyle: 'background:transparent;',
            region: "south",
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            //split: true,

            items: [

                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocMovementID", name: "DocMovementID", id: "DocMovementID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirMovementStatusID", name: "DirMovementStatusID", id: "DirMovementStatusID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseNameFrom", name: "DirWarehouseNameFrom", id: "DirWarehouseNameFrom" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseNameTo", name: "DirWarehouseNameTo", id: "DirWarehouseNameTo" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        //Статусы

                    ]
                },
              

            ],

        });



        //Form-Panel
        var formPanel1 = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,
            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)

            //bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "south", //!!! Важно для Ресайз-а !!!
            monitorValid: true,
            defaultType: 'textfield',
            layout: 'border',
            split: true,
            hidden: true,
            width: "100%", height: 200,
            bodyPadding: 5,
            autoHeight: true,

            items: [
                PanelGrid1, PanelGridLog0,
                PanelFooter,
            ]

        });


        //body
        this.items = [

            {
                region: 'center',
                xtype: 'panel',
                layout: 'border', // тип лэйоута - трехколонник с подвалом и шапкой
                items: [
                    tabPanel,
                    formPanel1
                ]
            }

        ],


        this.buttons = [
            
            //Статусы
            {
                tooltip: "Вернуть в 'Перемещение'", icon: '../Scripts/sklad/images/Status/work24.png', style: "width: 50px; height: 35px;", scale: 'large',
                id: "btnStatus1" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus1",
                enableToggle: true, pressed: false
            },
            {
                tooltip: "В ожидании курьера", icon: '../Scripts/sklad/images/Status/question24.png', style: "width: 50px; height: 35px;", scale: 'large',
                id: "btnStatus2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus2",
                enableToggle: true, pressed: false
            },
            {
                tooltip: "Курьер принял", icon: '../Scripts/sklad/images/Status/onagreeing24.png', style: "width: 50px; height: 35px;", scale: 'large',
                id: "btnStatus3" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus3",
                enableToggle: true, pressed: false
            },
            {
                tooltip: "Курьер отдал", icon: '../Scripts/sklad/images/Status/renovated24.png', style: "width: 50px; height: 35px;", scale: 'large',
                id: "btnStatus4" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnStatus4",
                enableToggle: true, pressed: false
            },

            //Документы
            "->",
            {
                id: "btnTabShow" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnTabShow", style: "width: 120px; height: 40px;",
                text: "Спецификация", icon: '../Scripts/sklad/images/save_held.png'
            },
            {
                id: "btnDocEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDocEdit", style: "width: 120px; height: 40px;",
                text: "Редактировать", icon: '../Scripts/sklad/images/save_held.png'
            },

        ],



        this.callParent(arguments);
    }

});