Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewDirNomensEdit", {
    extend: "Ext.panel.Panel",
    //extend: InterfaceSystemObjName,
    alias: "widget.viewDirNomensEdit",

    layout: "border",
    region: "center",
    //title: lanGoods,
    //width: 750, height: 350,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

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
            //title: lanGeneral,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%", //width: 500, height: 200,
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,
            //autoHeight: true,

            items: [
                //ID
                { xtype: 'textfield', fieldLabel: "DirNomeID", name: "DirNomenID", id: "DirNomenID" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Sub", name: "Sub", id: "Sub" + this.UO_id, allowBlank: true, hidden: true },


                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },


                { xtype: 'textfield', fieldLabel: lanName, name: "DirNomenName", id: "DirNomenName" + this.UO_id, flex: 1, allowBlank: false },
                { xtype: 'textfield', fieldLabel: lanNameFull, name: "DirNomenNameFull", id: "DirNomenNameFull" + this.UO_id, flex: 1, allowBlank: true },

                //Наименование
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: 'textfield', fieldLabel: lanArticle, name: "DirNomenArticle", id: "DirNomenArticle" + this.UO_id, flex: 1, allowBlank: true },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: lanNomenType, allowBlank: false, flex: 1, //, emptyText: "Тип"
                            margin: "0 0 0 10",
                            store: this.storeDirNomenTypesGrid, // store getting items from server
                            valueField: 'DirNomenTypeID',
                            hiddenName: 'DirNomenTypeID',
                            displayField: 'DirNomenTypeName',
                            name: 'DirNomenTypeID', itemId: "DirNomenTypeID", id: "DirNomenTypeID" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "Категория", allowBlank: true, flex: 1, //, emptyText: "Тип"
                    
                            store: this.storeDirNomenCategoriesGrid, // store getting items from server
                            valueField: 'DirNomenCategoryID',
                            hiddenName: 'DirNomenCategoryID',
                            displayField: 'DirNomenCategoryName',
                            name: 'DirNomenCategoryID', itemId: "DirNomenCategoryID", id: "DirNomenCategoryID" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

                            editable: true,
                            typeAhead: true,
                            minChars: 2,
                        },
                        { xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnDirNomenCategoryEdit", id: "btnDirNomenCategoryEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnDirNomenCategoryReload", id: "btnDirNomenCategoryReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: 'button', tooltip: "Clear", itemId: "btnDirNomenCategoryClear", text: "X", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
                    ]
                },


                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },


                { xtype: 'fieldset', width: "95%", layout: { align: 'stretch', type: 'column' }, title: "Описание",  items: [ ] },


                { xtype: 'label', text: lanNameShort },

                {
                    xtype: "htmleditor",
                    width: "100%", //height: "100%",
                    flex: 1,
                    name: "Description",
                    id: "Description" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
                },

                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                { xtype: 'label', text: lanNameFull },

                {
                    xtype: "htmleditor",
                    width: "100%", //height: "100%",
                    flex: 1,
                    name: "DescriptionFull",
                    id: "DescriptionFull" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
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

            ]

        });



        //body
        this.items = [
            
            formPanelEdit

        ],


        this.callParent(arguments);
    }

});