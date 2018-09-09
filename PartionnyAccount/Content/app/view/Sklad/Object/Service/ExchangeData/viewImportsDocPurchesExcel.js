Ext.define("PartionnyAccount.view.Sklad/Object/Service/ExchangeData/viewImportsDocPurchesExcel", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewImportsDocPurchesExcel",

    layout: "border",
    region: "center",
    title: "Импорт товара (Excel)",
    width: 550, height: 220,
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

        //DirContractor
        /*
        var TreeComboDirContractor = Ext.create('widget.viewTreeCombo', {
            fieldLabel: "Моя Организация", emptyText: "...", allowBlank: false, flex: 1,
            name: 'DirContractorName', itemId: "DirContractorName", id: "DirContractorName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            store: this.storeDirContractorsTree, //storeMenu,
            selectChildren: true,
            canSelectFolders: true,
            //itemId: "DirContractorName",
            root: {
                nodeType: 'sync',
                text: 'Группа',
                draggable: true,
                id: "DirContractor"
            }
        });
        */
        //body
        this.items = [

            Ext.create('Ext.form.Panel', {
                id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                UO_Loaded: this.UO_Loaded,

                //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
                //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
                UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)


                region: "center", //!!! Важно для Ресайз-а !!!
                bodyStyle: 'background:transparent;',
                frame: true,
                monitorValid: true,
                defaultType: 'textfield',
                width: "100%", height: "100%", //width: 500, height: 200,
                bodyPadding: 5,
                layout: 'anchor',
                defaults: { anchor: '100%' },
                autoScroll: true,


                items: [
                    /*
                    TreeComboDirContractor,
                    {
                        xtype: 'viewTriggerDirField',
                        allowBlank: true,
                        name: "DirContractorID", itemId: "DirContractorID", id: "DirContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                    },
                    */

                    //DirContractorOrg
                    {
                        xtype: 'viewComboBox',
                        fieldLabel: "Организация", flex: 2, allowBlank: false, //, emptyText: "..."
                        //margin: "0 0 0 5",
                        store: this.storeDirContractorsOrgGrid, // store getting items from server
                        valueField: 'DirContractorID',
                        hiddenName: 'DirContractorID',
                        displayField: 'DirContractorName',
                        name: 'DirContractorIDOrg', itemId: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        //disabled: true
                        //editable: false, typeAhead: false, minChars: 200,
                    },

                    { xtype: 'container', height: 5 },

                    //DirContractor
                    {
                        xtype: 'viewComboBox',
                        fieldLabel: lanContractor, flex: 1, allowBlank: false, //, emptyText: "..."

                        store: this.storeDirContractorsGrid, // store getting items from server
                        valueField: 'DirContractorID',
                        hiddenName: 'DirContractorID',
                        displayField: 'DirContractorName',
                        name: 'DirContractorID', itemId: "DirContractorID", id: "DirContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        //Поиск
                        editable: false, typeAhead: false, minChars: 200,
                    },

                    { xtype: 'container', height: 5 },

                    //DirWarehouse
                    /*
                    {
                        xtype: 'viewComboBox',
                        fieldLabel: lanWarehouse, flex: 1, allowBlank: false, //, emptyText: "..."
                        //margin: "0 0 0 5",
                        store: this.storeDirWarehousesGrid, // store getting items from server
                        valueField: 'DirWarehouseID',
                        hiddenName: 'DirWarehouseID',
                        displayField: 'DirWarehouseName',
                        name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        //Поиск
                        editable: false, typeAhead: false, minChars: 200,
                    },
                    */

                    { xtype: 'container', height: 5 },


                    {
                        xtype: 'filefield',
                        name: 'ExcelFile',
                        id: 'ExcelFile' + this.UO_id,
                        //fieldLabel: 'Excel',
                        labelWidth: 125,
                        msgTarget: 'side',
                        allowBlank: false,
                        anchor: '100%',
                        buttonText: lanISelectExcelFile
                    },
                    {
                        xtype: 'textfield',
                        id: 'sheetName' + this.UO_id,
                        name: 'sheetName',
                        fieldLabel: "Лист (напр. List1)", //'Наименование Листа (напр. Лист3)', //=  "Лист (напр. Лист1)"
                        labelWidth: 125,
                        allowBlank: false,
                        anchor: '100%'
                    }
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
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp",
                        text: lanHelp, icon: '../Scripts/sklad/images/help16.png',
                    }
                ]

            })

        ],


        this.callParent(arguments);
    }

});