Ext.define("PartionnyAccount.view.Sklad/Object/Report/viewReportPriceList", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewReportPriceList",

    layout: "border",
    region: "center",
    title: lanPriceList,
    width: 450, height: 160,
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
        
        //DirNomen
        var TreeComboDirNomen = Ext.create('widget.viewTreeCombo', {
            fieldLabel: lanGroup, emptyText: "...", allowBlank: true, flex: 1,
            name: 'DirNomenName', itemId: "DirNomenName", id: "DirNomenName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            store: this.storeDirNomensTree, //storeMenu,
            selectChildren: true,
            canSelectFolders: true,
            //readOnly: true,
            //itemId: "DirNomenName",
            root: {
                nodeType: 'sync',
                text: 'Группа',
                draggable: true,
                id: "DirNomen"
            }
        });

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
            region: "center", //!!! Важно для Ресайз-а !!!
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

            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [
                
                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: "checkbox", boxLabel: "Цены только > 0", name: "PriceGreater0", itemId: "PriceGreater0", flex: 1, id: "PriceGreater0" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, 
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Тип цены", flex: 2, allowBlank: false,
                            store: this.storeDirPriceTypesGrid, // store getting items from server
                            valueField: 'DirPriceTypeID',
                            hiddenName: 'DirPriceTypeID',
                            displayField: 'DirPriceTypeName',
                            name: 'DirPriceTypeID', itemId: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                        },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        TreeComboDirNomen,
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: true,
                            name: "DirNomenID", itemId: "DirNomenID", id: "DirNomenID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },
                
            ]
        });


        //body
        this.items = [

            formPanel

        ],


        this.buttons = [
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
                id: "btnHelp" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                text: lanHelp, icon: '../Scripts/sklad/images/help16.png', UO_Action: "cancel",
            },
        ],


        this.callParent(arguments);
    }

});