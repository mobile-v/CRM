Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewDirServiceNomensEdit", {
    extend: "Ext.panel.Panel",
    //extend: InterfaceSystemObjName,
    alias: "widget.viewDirServiceNomensEdit",

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
        
        
        var GeneralPanel = Ext.create('Ext.panel.Panel', {
            id: "PanelGeneral_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "north", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanGeneral,
            bodyPadding: 5,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "150", //width: 500, height: 200,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [
                //ID
                { xtype: 'textfield', fieldLabel: "DirNomeID", name: "DirServiceNomenID", id: "DirServiceNomenID" + this.UO_id, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "Sub", name: "Sub", id: "Sub" + this.UO_id, allowBlank: true, hidden: true },
                
                //Наименование
                { xtype: 'textfield', fieldLabel: lanName, name: "DirServiceNomenName", id: "DirServiceNomenName" + this.UO_id, flex: 1, allowBlank: false },
                { xtype: 'textfield', fieldLabel: lanNameFull, name: "DirServiceNomenNameFull", id: "DirServiceNomenNameFull" + this.UO_id, flex: 1, allowBlank: true },



                { xtype: "checkbox", boxLabel: "Использовать в И-М?", name: "ImportToIM", itemId: "ImportToIM", width: 5, id: "ImportToIM" + this.UO_id, inputValue: true, UO_Numb: 1, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                //Типичные неисправности
                {
                    title: "Типичные неисправности",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [

                        //1. Замена дисплейного модуля (экран+сенсор в сборе)
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Замена дисплейного модуля (экран+сенсор в сборе)", width: 300 },
                                { xtype: "checkbox", name: "Faults1Check", itemId: "Faults1Check", width: 5, id: "Faults1Check" + this.UO_id, inputValue: true, UO_Numb: 1, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults1Price", id: "Faults1Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //2. Замена сенсорного стекла (тачскрина)
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Замена сенсорного стекла (тачскрина)", width: 300 },
                                { xtype: "checkbox", name: "Faults2Check", itemId: "Faults2Check", width: 5, id: "Faults2Check" + this.UO_id, inputValue: true, UO_Numb: 2, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults2Price", id: "Faults2Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //3. Замена разъема зарядки
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Замена разъема зарядки", width: 300 },
                                { xtype: "checkbox", name: "Faults3Check", itemId: "Faults3Check", width: 5, id: "Faults3Check" + this.UO_id, inputValue: true, UO_Numb: 3, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults3Price", id: "Faults3Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //4. Замена разъема sim-карты
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Замена разъема sim-карты", width: 300 },
                                { xtype: "checkbox", name: "Faults4Check", itemId: "Faults4Check", width: 5, id: "Faults4Check" + this.UO_id, inputValue: true, UO_Numb: 4, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults4Price", id: "Faults4Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //5. Обновление ПО (прошивка)
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Обновление ПО (прошивка)", width: 300 },
                                { xtype: "checkbox", name: "Faults5Check", itemId: "Faults5Check", width: 5, id: "Faults51Check" + this.UO_id, inputValue: true, UO_Numb: 5, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults5Price", id: "Faults5Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //6. Замена динамика (слуховой)
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Замена динамика (слуховой)", width: 300 },
                                { xtype: "checkbox", name: "Faults6Check", itemId: "Faults6Check", width: 5, id: "Faults6Check" + this.UO_id, inputValue: true, UO_Numb: 6, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults6Price", id: "Faults6Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //7. Замена микрофона
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Замена микрофона", width: 300 },
                                { xtype: "checkbox", name: "Faults7Check", itemId: "Faults7Check", width: 5, id: "Faults7Check" + this.UO_id, inputValue: true, UO_Numb: 7, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults7Price", id: "Faults7Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //8. Замена динамика (звонок)
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Замена динамика (звонок)", width: 300 },
                                { xtype: "checkbox", name: "Faults8Check", itemId: "Faults8Check", width: 5, id: "Faults8Check" + this.UO_id, inputValue: true, UO_Numb: 8, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults8Price", id: "Faults8Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //9. Восстановление после попадания жидкости
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Восстановление после попадания жидкости", width: 300 },
                                { xtype: "checkbox", name: "Faults9Check", itemId: "Faults9Check", width: 5, id: "Faults9Check" + this.UO_id, inputValue: true, UO_Numb: 9, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults9Price", id: "Faults9Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //10. Восстановление цепи питания
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Восстановление цепи питания", width: 300 },
                                { xtype: "checkbox", name: "Faults10Check", itemId: "Faults10Check", width: 5, id: "Faults10Check" + this.UO_id, inputValue: true, UO_Numb: 10, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults10Price", id: "Faults10Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //11. Ремонт материнской платы
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Ремонт материнской платы", width: 300 },
                                { xtype: "checkbox", name: "Faults11Check", itemId: "Faults11Check", width: 5, id: "Faults11Check" + this.UO_id, inputValue: true, UO_Numb: 11, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults11Price", id: "Faults11Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //12. Резерв-5
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Резерв-5", width: 300 },
                                { xtype: "checkbox", name: "Faults12Check", itemId: "Faults12Check", width: 5, id: "Faults12Check" + this.UO_id, inputValue: true, UO_Numb: 12, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults12Price", id: "Faults12Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //13. Резерв-6
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Резерв-6", width: 300 },
                                { xtype: "checkbox", name: "Faults13Check", itemId: "Faults13Check", width: 5, id: "Faults13Check" + this.UO_id, inputValue: true, UO_Numb: 13, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults13Price", id: "Faults13Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                        { xtype: 'container', height: 2 },

                        //14. Резерв-7
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: "label", text: "Резерв-7", width: 300 },
                                { xtype: "checkbox", name: "Faults14Check", itemId: "Faults14Check", width: 5, id: "Faults14Check" + this.UO_id, inputValue: true, UO_Numb: 14, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                { xtype: 'textfield', name: "Faults14Price", id: "Faults14Price" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1, allowBlank: true, margin: "0 0 0 20", disabled: true },

                            ]
                        },

                    ]
                },


            ],
        });


        //2. Tab
        /*
        var rowEditing = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGridDirServiceNomenPrice = Ext.create('Ext.grid.Panel', {
            id: "PanelGridDirServiceNomenPrice_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center",
            loadMask: true,
            //autoScroll: true,
            //touchScroll: true,

            title: "Типовые неисправоности",
            itemId: "PanelGridDirServiceNomenPrice_grid",

            store: this.storeDirServiceNomenPricesGrid,

            columns: [
                { text: lanName, dataIndex: "DirServiceNomenTypicalFaultName", flex: 1 },
                { text: "Цена", dataIndex: "PriceVAT", width: 75, editor: { xtype: 'textfield' } }
            ],

            tbar: [
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_add.png', text: "Добавить все", tooltip: "Удалить и добавить все",
                    itemId: "btnGridAddAll",
                },
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: "Удалить все", tooltip: lanDeletionFlag + "?", 
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete"
                },
            ],

            plugins: [rowEditing],
            rowEditing: rowEditing,

        });
        */


        var groupPanel = Ext.create('Ext.panel.Panel', {
            id: "groupPanel_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
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

            layout: 'border',
            defaults: { anchor: '100%' },

            region: "center",
            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,


            items: [
                GeneralPanel, //PanelGridDirServiceNomenPrice
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
                    text: lanHelp, icon: '../Scripts/sklad/images/help16.png'
                },

            ]

        });



        //body
        this.items = [
            
            groupPanel

        ],


        this.callParent(arguments);
    }

});