Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirContractors/viewDirContractors", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirContractors",

    layout: "border",
    region: "center",
    title: lanContractors,
    width: 750, height: 435,
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
        var PanelGeneral = Ext.create('Ext.panel.Panel', {
            id: "PanelGeneral_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

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

            items: [
                //ID
                { xtype: 'textfield', fieldLabel: "DirContractorID", name: "DirContractorID", id: "DirContractorID" + this.UO_id, hidden: true }, //, readOnly: true
                //System record
                { xtype: "checkbox", boxLabel: "SysRecord", name: "SysRecord", id: "SysRecord" + this.UO_id, inputValue: true, hidden: true, readOnly: true },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanName, name: "DirContractorName", id: "DirContractorName" + this.UO_id, flex: 1, allowBlank: false },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //Тип + Сумма покупок
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //Тип-1
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanType, allowBlank: false, flex: 1, //, emptyText: "Тип"

                            store: this.storeDirContractor1TypesGrid, // store getting items from server
                            valueField: 'DirContractor1TypeID',
                            hiddenName: 'DirContractor1TypeID',
                            displayField: 'DirContractor1TypeName',
                            name: 'DirContractor1TypeID', itemId: "DirContractor1TypeID", id: "DirContractor1TypeID" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        //Тип-2
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanType, allowBlank: false, flex: 1, //, emptyText: "Тип"
                            margin: "0 0 0 10",
                            store: this.storeDirContractor2TypesGrid, // store getting items from server
                            valueField: 'DirContractor2TypeID',
                            hiddenName: 'DirContractor2TypeID',
                            displayField: 'DirContractor2TypeName',
                            name: 'DirContractor2TypeID', itemId: "DirContractor2TypeID", id: "DirContractor2TypeID" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanAddress, name: "DirContractorAddress", id: "DirContractorAddress" + this.UO_id, flex: 1, allowBlank: true },
                        { xtype: 'button', itemId: "btnGoogleMaps", tooltip: "Google Maps", text: "G", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: "Основной Счёт Организации", name: "DirBankAccountName", id: "DirBankAccountName" + this.UO_id, flex: 1, allowBlank: true }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //Скидка + Скидка
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                            items: [

                                //Скидка
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: lanDiscount, allowBlank: true, flex: 1, //, emptyText: "Тип"
                                    
                                    store: this.storeDirDiscountsGrid, // store getting items from server
                                    valueField: 'DirDiscountID',
                                    hiddenName: 'DirDiscountID',
                                    displayField: 'DirDiscountName',
                                    name: 'DirDiscountID', itemId: "DirDiscountID", id: "DirDiscountID" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },
                                { xtype: 'button', itemId: "btnDiscountClear", tooltip: "Clear", text: "X", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }

                            ]
                        },

                        //Скидка
                        { xtype: 'textfield', fieldLabel: lanFixedDiscount, margin: "0 0 0 10", name: "DirContractorDiscount", id: "DirContractorDiscount" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: false },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //Телефон + Факс
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanPhone, name: "DirContractorPhone", flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanFax, margin: "0 0 0 10", name: "DirContractorFax", flex: 1, allowBlank: true }
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                //EMail + WWW
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanEMail, name: "DirContractorEmail", flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanWWW, margin: "0 0 0 10", name: "DirContractorWWW", flex: 1, allowBlank: true },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'textfield',
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            fieldLabel: lanImageURL, name: "ImageLink", itemId: "ImageLink", id: "ImageLink" + this.UO_id, flex: 1, allowBlank: true
                        },
                    ]
                },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'image',
                            id: "imageShow" + this.UO_id,
                            src: '../Scripts/sklad/images/ru_default_no_foto.jpg',
                            style: {
                                'display': 'block',
                                'margin': 'auto'
                            },
                            alt: "Photo",
                            width: 320, height: 240,
                            region: "center"
                        }
                    ]
                },

            ]
        });


        //6. GMap-Panel
        var PanelGMap = Ext.create('Ext.panel.Panel', {
            id: "PanelGMap_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Карта",
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'fit', //anchor
            //defaults: { anchor: '100%' },
            autoScroll: true,

            /*items: [
                {
                    xtype: 'gmappanel',
                    gmapType: 'map',
                    id: "gmappanel" + this.UO_id,
                    center: {
                        geoCodeAddr: "1600 Amphitheatre Parkway Mountain View, CA 94043",
                        //geoCodeAddr: "221B Baker Street",
                        //geoCodeAddr: Ext.getCmp("DirContractorAddress" + aButton.UO_id).value,
                        marker: {
                            title: 'Holmes Home'
                        }
                    },
                    mapOptions: {
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    }
                },
            ]*/
        });



        //Tab-Panel
        var tabPanel = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,

            region: "center",
            bodyStyle: 'background:transparent;',
            //width: "100%", height: "100%",
            autoHeight: true,

            items: [
                PanelGeneral, PanelGMap
            ]

        });


        //Form-Panel
        var formPanelEdit = Ext.create('Ext.form.Panel', {
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

            region: "center",
            width: "100%", height: "100%",
            bodyPadding: 5,
            layout: 'fit',
            defaults: {
                anchor: '100%'
            },
            autoHeight: true,


            items: [
                tabPanel
            ],


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
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                }
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
                width: 220,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirContractor"
                },

                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    { text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'Тип', dataIndex: 'DirContractor2TypeName', width: 50, tdCls: 'x-change-cell' },
                ],

                /*
                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirContractors_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirContractors_onTree_folderNewSub;
                        contextMenuTree.folderCopy = controllerDirContractors_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirContractors_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirContractors_onTree_folderSubNull;
                        //Выводим
                        contextMenuTree.showAt(e.getXY());
                        return false;
                    }
                }
                */

            }),


            // *** *** *** *** *** *** *** *** ***


            formPanelEdit

        ],


        this.callParent(arguments);
    }

});