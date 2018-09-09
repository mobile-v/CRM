Ext.define("PartionnyAccount.view.Sklad/Object/List/viewListObjectPFsEdit", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewListObjectPFsEdit",

    layout: "border",
    region: "center",
    title: lanPrintForms,
    width: 850, height: 500,
    autoScroll: false,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    buttonAlign: 'left',

    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    conf: {},

    initComponent: function () {

        //Создать 3-и панели:
        // Вверху      - Партии
        // По середине - Основное
        // Внизу       - Спецификация
        //И есть Сплитер


        //Панель
        var PanelHeader = Ext.create('Ext.panel.Panel', {
            bodyStyle: 'background:transparent;',
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            width: "100%",
            autoScroll: true,

            items: [

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //ID
                        { xtype: 'textfield', fieldLabel: "ListObjectPFID", name: "ListObjectPFID", id: "ListObjectPFID" + this.UO_id, allowBlank: true, hidden: true }, //, readOnly: true
                        { xtype: 'textfield', fieldLabel: "ListObjectID", name: "ListObjectID", id: "ListObjectID" + this.UO_id, allowBlank: true, hidden: true }, //, readOnly: true
                        {
                            xtype: 'textfield',
                            allowBlank: false, flex: 1, fieldLabel: lanName,
                            name: 'ListObjectPFName', itemId: "ListObjectPFName", id: "ListObjectPFName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                        {
                            xtype: 'textfield',
                            allowBlank: true, flex: 1, fieldLabel: lanDesc, margin: "0 0 0 10",
                            name: 'ListObjectPFDesc', itemId: "ListObjectPFDesc", id: "ListObjectPFDesc" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                        {
                            xtype: 'viewComboBox',
                            margin: "0 0 0 10",
                            fieldLabel: lanLanguage, flex: 1, allowBlank: false,
                            store: this.storeListLanguagesGrid, // store getting items from server
                            valueField: 'ListLanguageID',
                            hiddenName: 'ListLanguageID',
                            displayField: 'ListLanguageName',
                            name: 'ListLanguageID', itemId: "ListLanguageID", id: "ListLanguageID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            //Поиск
                            //editable: true, typeAhead: true, minChars: 2
                        }
                    ]
                }
            ]
        });


        //Tab
        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

        //1-я вкладка
        var PanelTabHeaderEast = new Ext.FormPanel({
            title: lanFieldDocument,
            region: 'east',
            width: 225,
            split: true, autoScroll: true,
            //autoLoad: "ASPX/ListObjectFieldShow.aspx?pID=" + pID + "&ListObjectField=ListObjectFieldHeaderShow&ListObjectPFID=" + ListObjectPFID,
            loader: {
                url: HTTP_ListObjectFieldNames + "Html1/Html2/" + "?ListObjectField=ListObjectFieldHeaderShow&ListObjectID=" + this.UO_Param_id,
                text: "Loading...",
                autoLoad: true,
                timeout: 60,
                scripts: true,
                renderer: 'html',
                success: function (result, result2, result3, result4) {
                    /*alert("1111");
                    var PanelTabHeaderEast = Ext.getCmp("PanelTabHeaderEast_" + result3.scope.target.UO_id)
                    var html = PanelTabHeaderEast.html.replace(/"/g, '');
                    PanelTabHeaderEast.update(html);*/
                },
                failure: function (loader, response, options) {
                    console.log(response)
                }
            }
        });
        var PanelTabHeaderCenter = new Ext.Panel({
            //title: lanFieldDocument,
            region: 'center',
            layout: 'border',
            //split: true, autoScroll: true,
            items: [
                {
                    xtype: "htmleditor",
                    region: "center",
                    flex: 1,

                    name: "ListObjectPFHtmlHeader",
                    id: "ListObjectPFHtmlHeader" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
                }
            ],
        });
        var PanelTabHeader = Ext.create('Ext.panel.Panel', {
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanCap,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            flex: 1,
            layout: 'border',

            items: [
                PanelTabHeaderCenter,
                PanelTabHeaderEast
            ],
            dockedItems: [
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        { xtype: "checkbox", boxLabel: lanUse, name: "ListObjectPFHtmlHeaderUse", id: "ListObjectPFHtmlHeaderUse" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                        { xtype: "checkbox", boxLabel: "Два экземпляра", name: "ListObjectPFHtmlDouble", id: "ListObjectPFHtmlDouble" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, margin: "0 0 0 15" },
                        
                        {
                            xtype: "numberfield", fieldLabel: "<b>Отступы по краям:</b> Верх", labelWidth: 160, width: 235,
                            name: "MarginTop", id: "MarginTop" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, margin: "0 0 0 15"
                        },
                        {
                            xtype: "numberfield", fieldLabel: "Низ", labelWidth: 35, width: 100,
                            name: "MarginBottom", id: "MarginBottom" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, margin: "0 0 0 15"
                        },
                        {
                            xtype: "numberfield", fieldLabel: "Лево", labelWidth: 35, width: 100,
                            name: "MarginLeft", id: "MarginLeft" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, margin: "0 0 0 15"
                        },
                        {
                            xtype: "numberfield", fieldLabel: "Право", labelWidth: 35, width: 100,
                            name: "MarginRight", id: "MarginRight" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, margin: "0 0 0 15"
                        },
                    ]
                },

            ]
        });


        //2-я вкладка (состоит из 4-х под-вкладок)
        //Шапка, Таблица, Итого, Текст
        //2.1.Шапка
        var PanelTabTableCap = new Ext.Panel({
            title: lanCap,
            region: 'center',
            layout: 'border',
            //split: true, autoScroll: true,
            items: [
                {
                    xtype: "textareafield",
                    region: "center",
                    flex: 1,

                    name: "ListObjectPFHtmlTabCap",
                    id: "ListObjectPFHtmlTabCap" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
                }
            ],
            dockedItems: [
                { xtype: "checkbox", boxLabel: lanUse, name: "ListObjectPFHtmlTabUseCap", id: "ListObjectPFHtmlTabUseCap" + this.UO_id, inputValue: true, itemId: "ListObjectPFHtmlTabUseCap", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
            ]
        });
        //2.1.Таблица
        var PanelTabTableGrid = Ext.create('Ext.grid.Panel', { //widget.viewGridDoc
            title: lanTable2,
            id: "grid_" + this.UO_id,  //WingetName + ObjectID
            UO_id: this.UO_id,         //ObjectID
            UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
            UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
            UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)

            itemId: "grid",

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            autoScroll: true,
            flex: 1,
            //split: true,

            store: this.storeListObjectPFTabsGrid,

            columns: [
                { text: lanName, dataIndex: "ListObjectPFTabName", flex: 1 },
                { text: lanField, dataIndex: "ListObjectFieldNameRu", flex: 1 },
                { text: "Таб №", dataIndex: "TabNum", width: 100 },
                { text: "Ширина", dataIndex: "Width", width: 100 },
            ],

            tbar: [

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    //xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: lanAddPosition, tooltip: lanAddPosition,
                    itemId: "btnGridNew",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_edit.png', text: lanEdit, tooltip: lanEdit, disabled: true,
                    id: "btnGridEdit" + this.UO_id, itemId: "btnGridEdit"
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDelete" + this.UO_id, itemId: "btnGridDelete"
                },


                ' ', '-', ' ',
                { xtype: "checkbox", boxLabel: lanUse, name: 'ListObjectPFHtmlTabUseTab', id: "ListObjectPFHtmlTabUseTab" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                ' ',
                { xtype: "checkbox", boxLabel: lanEnumerate, name: 'ListObjectPFHtmlTabEnumerate', id: "ListObjectPFHtmlTabEnumerate" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                ' ',
                { xtype: "checkbox", boxLabel: lanFontSmallBig, name: 'ListObjectPFHtmlTabFont', id: "ListObjectPFHtmlTabFont" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/help16.png', tooltip: "Help" + "?",
                    id: "btnGridTableHelp" + this.UO_id, itemId: "btnGridTableHelp"
                },
            ],
        });
        //2.3.Итого
        var PanelTabTableFooterEast = new Ext.FormPanel({
            title: lanFieldDocument,
            region: 'east',
            width: 225,
            split: true, autoScroll: true,
            //autoLoad: "ASPX/ListObjectFieldShow.aspx?pID=" + pID + "&ListObjectField=ListObjectFieldHeaderShow&ListObjectPFID=" + ListObjectPFID,
            loader: {
                url: HTTP_ListObjectFieldNames + "Html1/Html2/" + "?ListObjectField=ListObjectFieldFooterShow&ListObjectID=" + this.UO_Param_id,
                text: "Loading...",
                autoLoad: true,
                timeout: 60,
                scripts: true,
                renderer: 'html',
                success: function (result, result2, result3, result4) {
                    /*alert("1111");
                    var PanelTabHeaderEast = Ext.getCmp("PanelTabHeaderEast_" + result3.scope.target.UO_id)
                    var html = PanelTabHeaderEast.html.replace(/"/g, '');
                    PanelTabHeaderEast.update(html);*/
                },
                failure: function (loader, response, options) {
                    console.log(response)
                }
            }
        });
        var PanelTabTableFooterCenter = new Ext.Panel({
            //title: lanFieldDocument,
            region: 'center',
            layout: 'border',
            //split: true, autoScroll: true,
            items: [
                {
                    xtype: "textareafield",
                    region: "center",
                    flex: 1,

                    name: "ListObjectPFHtmlTabFooter",
                    id: "ListObjectPFHtmlTabFooter" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
                }
            ],
        });
        var PanelTabTableFooter = Ext.create('Ext.panel.Panel', {
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanFooter,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            flex: 1,
            layout: 'border',

            items: [
                PanelTabTableFooterCenter,
                PanelTabTableFooterEast
            ],
            dockedItems: [
                { xtype: "checkbox", boxLabel: lanUse, name: "ListObjectPFHtmlTabUseFooter", id: "ListObjectPFHtmlTabUseFooter" + this.UO_id, inputValue: true, itemId: "ListObjectPFHtmlTabUseFooter", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
            ]
        });
        //2.4.Текст
        var PanelTabTableTextEast = new Ext.FormPanel({
            title: lanFieldDocument,
            region: 'east',
            width: 225,
            split: true, autoScroll: true,
            //autoLoad: "ASPX/ListObjectFieldShow.aspx?pID=" + pID + "&ListObjectField=ListObjectFieldHeaderShow&ListObjectPFID=" + ListObjectPFID,
            loader: {
                url: HTTP_ListObjectFieldNames + "Html1/Html2/" + "?ListObjectField=ListObjectFieldTabShow&ListObjectID=" + this.UO_Param_id,
                text: "Loading...",
                autoLoad: true,
                timeout: 60,
                scripts: true,
                renderer: 'html',
                success: function (result, result2, result3, result4) {
                    /*alert("1111");
                    var PanelTabHeaderEast = Ext.getCmp("PanelTabHeaderEast_" + result3.scope.target.UO_id)
                    var html = PanelTabHeaderEast.html.replace(/"/g, '');
                    PanelTabHeaderEast.update(html);*/
                },
                failure: function (loader, response, options) {
                    console.log(response)
                }
            }
        });
        var PanelTabTableTextCenter = new Ext.Panel({
            //title: lanFieldDocument,
            region: 'center',
            layout: 'border',
            //split: true, autoScroll: true,
            items: [
                {
                    xtype: "htmleditor",
                    region: "center",
                    flex: 1,

                    name: "ListObjectPFHtmlTabText",
                    id: "ListObjectPFHtmlTabText" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
                }
            ],
        });
        var PanelTabTableText = Ext.create('Ext.panel.Panel', {
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: "Text",
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            flex: 1,
            layout: 'border',

            items: [
                PanelTabTableTextCenter,
                PanelTabTableTextEast
            ],
            dockedItems: [
                { xtype: "checkbox", boxLabel: lanUse, name: "ListObjectPFHtmlTabUseText", id: "ListObjectPFHtmlTabUseText" + this.UO_id, inputValue: true, itemId: "ListObjectPFHtmlTabUseText", UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
            ]
        });
        //Таб
        var PanelTabTable = Ext.create('Ext.tab.Panel', {
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanTable,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            flex: 1,
            layout: 'anchor',

            items: [
                PanelTabTableCap,
                PanelTabTableGrid,
                PanelTabTableFooter,
                PanelTabTableText
            ]
        });


        //3-я вкладка
        var PanelTabFooterEast = new Ext.FormPanel({
            title: lanFieldDocument,
            region: 'east',
            width: 225,
            split: true, autoScroll: true,
            //autoLoad: "ASPX/ListObjectFieldShow.aspx?pID=" + pID + "&ListObjectField=ListObjectFieldHeaderShow&ListObjectPFID=" + ListObjectPFID,
            loader: {
                url: HTTP_ListObjectFieldNames + "Html1/Html2/" + "?ListObjectField=ListObjectFieldFooterShow&ListObjectID=" + this.UO_Param_id,
                text: "Loading...",
                autoLoad: true,
                timeout: 60,
                scripts: true,
                renderer: 'html',
                success: function (result, result2, result3, result4) {
                    /*alert("1111");
                    var PanelTabHeaderEast = Ext.getCmp("PanelTabHeaderEast_" + result3.scope.target.UO_id)
                    var html = PanelTabHeaderEast.html.replace(/"/g, '');
                    PanelTabHeaderEast.update(html);*/
                },
                failure: function (loader, response, options) {
                    console.log(response)
                }
            }
        });
        var PanelTabFooterCenter = new Ext.Panel({
            //title: lanFieldDocument,
            region: 'center',
            layout: 'border',
            //split: true, autoScroll: true,
            items: [
                {
                    xtype: "htmleditor",
                    region: "center",
                    flex: 1,

                    name: "ListObjectPFHtmlFooter",
                    id: "ListObjectPFHtmlFooter" + this.UO_id,
                    hideLabel: false, labelSeparator: '111', //anchor: "100%" //, autoHeight: true
                }
            ],
        });
        var PanelTabFooter = Ext.create('Ext.panel.Panel', {
            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            title: lanFooter,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            flex: 1,
            layout: 'border',

            items: [
                PanelTabFooterEast,
                PanelTabFooterCenter
            ],
            dockedItems: [
                { xtype: "checkbox", boxLabel: lanUse, name: "ListObjectPFHtmlFooterUse", id: "ListObjectPFHtmlFooterUse" + this.UO_id, inputValue: true, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall }
            ]
        });



        //Tab-Panel
        var tabPanelDetails = Ext.create('Ext.tab.Panel', {
            id: "tab_" + this.UO_id,
                        
            UO_id: this.UO_id,
            UO_idMain: this.UO_idMain,
            UO_idCall: this.UO_idCall,
            region: "center",
            flex: 1,
           
            items: [
                PanelTabHeader, PanelTabTable, PanelTabFooter
            ]

        });

        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 


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
                PanelHeader,
                tabPanelDetails
            ]
        });




        //body
        this.items = [

            formPanel

        ],


        this.buttons = [
            {
                id: "btnRecord" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, 
                text: lanSave, icon: '../Scripts/sklad/images/save.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                        UO_Action: "save",
                        text: lanRecord, icon: '../Scripts/sklad/images/save.png',
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSaveClose",
                        UO_Action: "save_close",
                        text: lanRecordClose, icon: '../Scripts/sklad/images/save.png',
                    }
                ]
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

        ],


        this.callParent(arguments);
    }

});