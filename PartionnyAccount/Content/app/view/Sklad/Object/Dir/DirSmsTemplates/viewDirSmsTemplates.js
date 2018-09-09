Ext.define("PartionnyAccount.view.Sklad/Object/Dir/DirSmsTemplates/viewDirSmsTemplates", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewDirSmsTemplates",

    layout: "border",
    region: "center",
    title: "Шаблоны SMS",
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
                { xtype: 'textfield', fieldLabel: "DirSmsTemplateID", name: "DirSmsTemplateID", id: "DirSmsTemplateID" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "MenuID", name: "MenuID", id: "MenuID" + this.UO_id, allowBlank: true, hidden: true },

                {
                    xtype: 'viewComboBox',
                    allowBlank: true, flex: 1,
                    fieldLabel: lanType,
                    store: new Ext.data.SimpleStore({
                        fields: ['DirSmsTemplateType', 'DirSmsTemplateTypeName'],
                        data: this.DirSmsTemplateType_values
                    }),

                    valueField: 'DirSmsTemplateType',
                    hiddenName: 'DirSmsTemplateType',
                    displayField: 'DirSmsTemplateTypeName',
                    name: 'DirSmsTemplateType', itemId: "DirSmsTemplateType", id: "DirSmsTemplateType" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                },

                //Наименование + текст
                { xtype: 'textfield', fieldLabel: lanName, name: "DirSmsTemplateName", id: "DirSmsTemplateName" + this.UO_id, flex: 1, allowBlank: false },
                { xtype: 'textfield', fieldLabel: lanMessage, name: "DirSmsTemplateMsg", id: "DirSmsTemplateMsg" + this.UO_id, flex: 1, allowBlank: false },
                


                { xtype: 'container', height: 25 },





                {
                    xtype: 'fieldset', width: "95%", layout: { align: 'stretch', type: 'anchor' },
                    title: "Метаданные",
                    items: [

                        //1
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "Номер документа", labelWidth: 150, value: "[[[ДокументНомер]]]", name: "DirSmsTemplate1", id: "DirSmsTemplate1" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                                { xtype: 'textfield', fieldLabel: "Наименование товара", labelWidth: 150, value: "[[[ТоварНаименование]]]", name: "DirSmsTemplate2", id: "DirSmsTemplate2" + this.UO_id, flex: 1, allowBlank: true, readOnly: true, margin: "0 0 0 5" },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        //2
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "Ваша Организация", labelWidth: 150, value: "[[[Организация]]]", name: "DirSmsTemplate3", id: "DirSmsTemplate3" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                                { xtype: 'textfield', fieldLabel: "Точка", labelWidth: 150, value: "[[[ТочкаНаименование]]]", name: "DirSmsTemplate4", id: "DirSmsTemplate4" + this.UO_id, flex: 1, allowBlank: true, readOnly: true, margin: "0 0 0 5" },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        //3
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "Адрес точки", labelWidth: 150, value: "[[[ТочкаАдрес]]]", name: "DirSmsTemplate5", id: "DirSmsTemplate5" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                                { xtype: 'textfield', fieldLabel: "Телефон точки", labelWidth: 150, value: "[[[ТочкаТелефон]]]", name: "DirSmsTemplate6", id: "DirSmsTemplate6" + this.UO_id, flex: 1, allowBlank: true, readOnly: true, margin: "0 0 0 5" },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        //4
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "Точка с которой списываем", labelWidth: 150, value: "[[[ТочкаОт]]]", name: "DirSmsTemplate7", id: "DirSmsTemplate7" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                                { xtype: 'textfield', fieldLabel: "Точка на которую приходуем", labelWidth: 150, value: "[[[ТочкаНа]]]", name: "DirSmsTemplate8", id: "DirSmsTemplate8" + this.UO_id, flex: 1, allowBlank: true, readOnly: true, margin: "0 0 0 5" },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        //5
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "Сумма вся", labelWidth: 150, value: "[[[СуммаВся]]]", name: "DirSmsTemplate9", id: "DirSmsTemplate9" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                                { xtype: 'textfield', fieldLabel: "Сумма без предоплаты", labelWidth: 150, value: "[[[СуммаМинусПредоплата]]]", name: "DirSmsTemplate10", id: "DirSmsTemplate10" + this.UO_id, flex: 1, allowBlank: true, readOnly: true, margin: "0 0 0 5" },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        //6
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'textfield', fieldLabel: "Гарантия", labelWidth: 150, value: "[[[Гарантия]]]", name: "DirSmsTemplate11", id: "DirSmsTemplate11" + this.UO_id, flex: 1, allowBlank: true, readOnly: true },

                            ]
                        },

                        { xtype: 'container', height: 5 },

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

                /*{
                    id: "btnHistory" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHistory",
                    text: lanHistory, icon: '../Scripts/sklad/images/history.png',
                    disabled: true
                },*/

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
                    id: "DirSmsTemplate"
                },

                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    { text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: lanName, flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Доступ', width: 50, dataIndex: 'Active', sortable: true },
                    { text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                ],

                /*
                listeners: {
                    itemcontextmenu: function (view, rec, node, index, e) {
                        e.stopEvent();
                        //Присваиваем ID-шник
                        contextMenuTree.UO_id = this.UO_id;
                        //Присваиваем Функции обработки
                        contextMenuTree.folderNew = controllerDirSmsTemplates_onTree_folderNew;
                        contextMenuTree.folderNewSub = controllerDirSmsTemplates_onTree_folderNewSub;
                        contextMenuTree.folderCopy = controllerDirSmsTemplates_onTree_folderCopy;
                        contextMenuTree.folderDel = controllerDirSmsTemplates_onTree_folderDel;
                        contextMenuTree.folderSubNull = controllerDirSmsTemplates_onTree_folderSubNull;
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