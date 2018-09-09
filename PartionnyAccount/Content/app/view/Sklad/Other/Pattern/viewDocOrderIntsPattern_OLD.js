Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewDocOrderIntsPattern", {
    extend: "Ext.panel.Panel",
    //extend: InterfaceSystemObjName,
    alias: "widget.viewDocOrderIntsPattern",

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

    //Контроллер
    controller: 'viewcontrollerDirNomens',

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
            autoHeight: true,

            items: [

                //ID
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocOrderIntID", name: "DocOrderIntID", id: "DocOrderIntID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, allowBlank: false, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, allowBlank: false, hidden: true },
                { xtype: 'viewDateField', fieldLabel: "DocDate", name: "DocDate", id: "DocDate" + this.UO_id, allowBlank: false, hidden: true },

                // ??? Документ который создал этот Заказ ???
                { xtype: 'textfield', fieldLabel: "DocID2", name: "DocID2", id: "DocID2_" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                //Документ который создал этот Заказ
                { xtype: 'textfield', fieldLabel: "DocIDBase", name: "DocIDBase", id: "DocIDBase" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                //Реальный номер документа который создал этот Заказ
                { xtype: 'textfield', fieldLabel: "NumberReal", name: "NumberReal", id: "NumberReal" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                //Тип оплаты
                { xtype: 'textfield', fieldLabel: "DirPaymentTypeID", name: "DirPaymentTypeID", id: "DirPaymentTypeID" + this.UO_id, readOnly: true, allowBlank: false, hidden: true },


                //Тип документа + Наименование товара
                {
                    title: "Зап.часть",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [

                        {
                            xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                            items: [

                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                    items: [
                                        {
                                            xtype: 'viewComboBox', fieldLabel: "Тип заявки", allowBlank: false, flex: 1, readOnly: true,
                                            store: new Ext.data.SimpleStore({
                                                fields: ['DirOrderIntTypeID', 'DirOrderIntTypeName'],
                                                data: DirOrderIntType_values
                                            }),

                                            valueField: 'DirOrderIntTypeID',
                                            hiddenName: 'DirOrderIntTypeID',
                                            displayField: 'DirOrderIntTypeName',
                                            name: 'DirOrderIntTypeID', itemId: "DirOrderIntTypeID", id: "DirOrderIntTypeID" + this.UO_id,
                                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        },

                                        { xtype: 'textfield', fieldLabel: "Код", name: "DirNomenID", id: "DirNomenID" + this.UO_id, flex: 1, readOnly: true, allowBlank: true, hidden: true },
                                        { xtype: 'textfield', fieldLabel: lanName, name: "DirNomenName", id: "DirNomenName" + this.UO_id, flex: 2, readOnly: true, allowBlank: true, margin: "0 0 0 5" },
                                    ]
                                },

                                { xtype: 'container', height: 5 },

                            ]
                        },

                    ]
                },


                //Группы
                {
                    title: "Группа",
                    autoHeight: true,
                    xtype: 'fieldset', flex: 1, layout: 'anchor',
                    items: [
                        {
                            xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                            items: [


                                //Группы: 1 и 2
                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                    items: [
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                            items: [

                                                {
                                                    xtype: 'viewComboBox',
                                                    fieldLabel: "Марка", flex: 1, allowBlank: false, //Группа-1

                                                    store: this.storeDirNomensGrid1, // store getting items from server
                                                    valueField: 'DirNomenID',
                                                    hiddenName: 'DirNomenID',
                                                    displayField: 'DirNomenName',
                                                    name: "DirNomenID1", itemId: "DirNomenID1", id: "DirNomenID1" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    //Поиск
                                                    //editable: true, typeAhead: true, minChars: 2
                                                },
                                                { xtype: 'textfield', fieldLabel: "DirNomen1ID", name: "DirNomen1ID", id: "DirNomen1ID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                                { xtype: 'textfield', fieldLabel: "DirNomen1Name", name: "DirNomen1Name", id: "DirNomen1Name" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                                            ]
                                        },

                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                            items: [

                                                {
                                                    xtype: 'viewComboBox',
                                                    fieldLabel: "Модель", flex: 1, allowBlank: true, //Группа-2
                                                    margin: "0 0 0 10",
                                                    store: this.storeDirNomensGrid2, // store getting items from server
                                                    valueField: 'DirNomenID',
                                                    hiddenName: 'DirNomenID',
                                                    displayField: 'DirNomenName',
                                                    name: "DirNomenID2", itemId: "DirNomenID2", id: "DirNomenID2" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    readOnly: true,
                                                    //Поиск
                                                    editable: true, typeAhead: true, minChars: 2
                                                },
                                                { xtype: 'textfield', fieldLabel: "DirNomen2ID", name: "DirNomen2ID", id: "DirNomen2ID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                                { xtype: 'textfield', fieldLabel: "DirNomen2Name", name: "DirNomen2Name", id: "DirNomen2Name" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                            ]
                                        },
                                    ]
                                },



                                /*

                                //Группы: 3 и 4
                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                    items: [
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                            items: [

                                                {
                                                    xtype: 'viewComboBox',
                                                    fieldLabel: "Группа-3", flex: 1, allowBlank: true,

                                                    store: this.storeDirNomensGrid3, // store getting items from server
                                                    valueField: 'DirNomenID',
                                                    hiddenName: 'DirNomenID',
                                                    displayField: 'DirNomenName',
                                                    name: "DirNomenID3", itemId: "DirNomenID3", id: "DirNomenID3" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    readOnly: true,
                                                    //Поиск
                                                    editable: true, typeAhead: true, minChars: 2
                                                },
                                                { xtype: 'textfield', fieldLabel: "DirNomen3ID", name: "DirNomen3ID", id: "DirNomen3ID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                                { xtype: 'textfield', fieldLabel: "DirNomen3Name", name: "DirNomen3Name", id: "DirNomen3Name" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                                            ]
                                        },

                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                            items: [

                                                {
                                                    xtype: 'viewComboBox',
                                                    fieldLabel: "Группа-4", flex: 1, allowBlank: true,
                                                    margin: "0 0 0 10",
                                                    store: this.storeDirNomensGrid4, // store getting items from server
                                                    valueField: 'DirNomenID',
                                                    hiddenName: 'DirNomenID',
                                                    displayField: 'DirNomenName',
                                                    name: "DirNomenID4", itemId: "DirNomenID4", id: "DirNomenID4" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    readOnly: true,
                                                    //Поиск
                                                    editable: true, typeAhead: true, minChars: 2
                                                },
                                                { xtype: 'textfield', fieldLabel: "DirNomen4ID", name: "DirNomen4ID", id: "DirNomen4ID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                                { xtype: 'textfield', fieldLabel: "DirNomen4Name", name: "DirNomen4Name", id: "DirNomen4Name" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                            ]
                                        },
                                    ]
                                },



                                //Группы: 5 и 6
                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                                    items: [
                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                            items: [

                                                {
                                                    xtype: 'viewComboBox',
                                                    fieldLabel: "Группа-5", flex: 1, allowBlank: true,

                                                    store: this.storeDirNomensGrid5, // store getting items from server
                                                    valueField: 'DirNomenID',
                                                    hiddenName: 'DirNomenID',
                                                    displayField: 'DirNomenName',
                                                    name: "DirNomenID5", itemId: "DirNomenID5", id: "DirNomenID5" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    readOnly: true,
                                                    //Поиск
                                                    editable: true, typeAhead: true, minChars: 2
                                                },
                                                { xtype: 'textfield', fieldLabel: "DirNomen5ID", name: "DirNomen5ID", id: "DirNomen5ID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                                { xtype: 'textfield', fieldLabel: "DirNomen5Name", name: "DirNomen5Name", id: "DirNomen5Name" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                            ]
                                        },

                                        {
                                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                            items: [

                                                {
                                                    xtype: 'viewComboBox',
                                                    fieldLabel: "Группа-6", flex: 1, allowBlank: true,
                                                    margin: "0 0 0 10",
                                                    store: this.storeDirNomensGrid6, // store getting items from server
                                                    valueField: 'DirNomenID',
                                                    hiddenName: 'DirNomenID',
                                                    displayField: 'DirNomenName',
                                                    name: "DirNomenID6", itemId: "DirNomenID6", id: "DirNomenID6" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                                    readOnly: true,
                                                    //Поиск
                                                    editable: true, typeAhead: true, minChars: 2
                                                },
                                                { xtype: 'textfield', fieldLabel: "DirNomen6ID", name: "DirNomen6ID", id: "DirNomen6ID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                                                { xtype: 'textfield', fieldLabel: "DirNomen6Name", name: "DirNomen6Name", id: "DirNomen6Name" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                                            ]
                                        },
                                    ]
                                },

                                */



                                { xtype: 'container', height: 5 },

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Категория", allowBlank: false, flex: 1, //, emptyText: "Тип"

                                    store: this.storeDirNomenCategoriesGrid, // store getting items from server
                                    valueField: 'DirNomenCategoryID',
                                    hiddenName: 'DirNomenCategoryID',
                                    displayField: 'DirNomenCategoryName',
                                    name: 'DirNomenCategoryID', itemId: "DirNomenCategoryID", id: "DirNomenCategoryID" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    editable: true, typeAhead: true, minChars: 2,
                                },



                                { xtype: 'container', height: 5 },

                            ]
                        },

                    ]
                },


                { xtype: 'container', height: 5 },


                //Цена
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //К-во
                        {
                            xtype: 'numberfield',
                            value: 1, maxValue: 9999, minValue: 1,
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 160, fieldLabel: "<b>" + lanCount + "</b>",
                            name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                        {
                            xtype: 'viewComboBox',
                            fieldLabel: "<b>" + "Ориентир." + "</b>",
                            flex: 1, allowBlank: false,
                            margin: "0 0 0 15",
                            store: this.storeDirPriceTypesGrid, // store getting items from server
                            valueField: 'DirPriceTypeID',
                            hiddenName: 'DirPriceTypeID',
                            displayField: 'DirPriceTypeName',
                            name: 'DirPriceTypeID', itemId: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },

                        //Приходная цена
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, margin: "0 0 0 10", //, fieldLabel: "<b>" + lanPrice + "</b>"
                            name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            hidden: true
                        },
                        {
                            xtype: 'textfield',
                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 150, margin: "0 0 0 10", //, fieldLabel: "<b>" + lanPrice + "</b>"
                            name: 'PriceCurrency', itemId: 'PriceCurrency', id: 'PriceCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                        //Предоплата
                        {
                            xtype: 'textfield',
                            fieldLabel: "Предоплата", labelStyle: 'width:100px', regex: /^[+\-]?\d+(?:\.\d+)?$/, flex: 1,
                            margin: "0 0 0 25", name: "PrepaymentSum", id: "PrepaymentSum" + this.UO_id, allowBlank: false
                        },

                        //Валюты
                        { xtype: 'textfield', fieldLabel: "DirCurrencyID", name: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "DirCurrencyRate", name: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                        { xtype: 'textfield', fieldLabel: "DirCurrencyMultiplicity", name: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                    ]
                },


                { xtype: 'container', height: 5 },


                //Характеристики
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Характеристики",
                    autoHeight: true,
                    items: [
                        //DirCharColours, DirCharMaterials
                        {
                            xtype: 'textfield',
                            fieldLabel: "DirChar", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                            name: 'DirChar', itemId: "DirChar", id: "DirChar" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },


                        //DirCharColours, DirCharMaterials
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                    items: [

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Цвет", flex: 1, allowBlank: true,

                                            store: this.storeDirCharColoursGrid, // store getting items from server
                                            valueField: 'DirCharColourID',
                                            hiddenName: 'DirCharColourID',
                                            displayField: 'DirCharColourName',
                                            name: 'DirCharColourID', itemId: "DirCharColourID", id: "DirCharColourID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharColourEdit", id: "btnCharColourEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharColourReload", id: "btnCharColourReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharColourClear", id: "btnCharColourClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharColourName', itemId: "DirCharColourName", id: "DirCharColourName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }

                                    ]
                                },

                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                    items: [

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Производитель", flex: 1, allowBlank: true,
                                            margin: "0 0 0 10",
                                            store: this.storeDirCharMaterialsGrid, // store getting items from server
                                            valueField: 'DirCharMaterialID',
                                            hiddenName: 'DirCharMaterialID',
                                            displayField: 'DirCharMaterialName',
                                            name: 'DirCharMaterialID', itemId: "DirCharMaterialID", id: "DirCharMaterialID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharMaterialEdit", id: "btnCharMaterialEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharMaterialReload", id: "btnCharMaterialReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharMaterialClear", id: "btnCharMaterialClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharMaterialName', itemId: "DirCharMaterialName", id: "DirCharMaterialName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }

                                    ]
                                },
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                        //DirCharNames, DirCharSeasons
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                    items: [

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Имя", flex: 1, allowBlank: true,

                                            store: this.storeDirCharNamesGrid, // store getting items from server
                                            valueField: 'DirCharNameID',
                                            hiddenName: 'DirCharNameID',
                                            displayField: 'DirCharNameName',
                                            name: 'DirCharNameID', itemId: "DirCharNameID", id: "DirCharNameID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharNameEdit", id: "btnCharNameEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharNameReload", id: "btnCharNameReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharNameClear", id: "btnCharNameClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharNameName', itemId: "DirCharNameName", id: "DirCharNameName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }

                                    ]
                                },

                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                    items: [

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Сезон", flex: 1, allowBlank: true,
                                            margin: "0 0 0 10",
                                            store: this.storeDirCharSeasonsGrid, // store getting items from server
                                            valueField: 'DirCharSeasonID',
                                            hiddenName: 'DirCharSeasonID',
                                            displayField: 'DirCharSeasonName',
                                            name: 'DirCharSeasonID', itemId: "DirCharSeasonID", id: "DirCharSeasonID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2,
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharSeasonEdit", id: "btnCharSeasonEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharSeasonReload", id: "btnCharSeasonReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharSeasonClear", id: "btnCharSeasonClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharSeasonName', itemId: "DirCharSeasonName", id: "DirCharSeasonName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }

                                    ]
                                },
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                        //DirCharSexes, DirCharSizes
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                    items: [

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Пол", flex: 1, allowBlank: true,

                                            store: this.storeDirCharSexesGrid, // store getting items from server
                                            valueField: 'DirCharSexID',
                                            hiddenName: 'DirCharSexID',
                                            displayField: 'DirCharSexName',
                                            name: 'DirCharSexID', itemId: "DirCharSexID", id: "DirCharSexID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharSexEdit", id: "btnCharSexEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharSexReload", id: "btnCharSexReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharSexClear", id: "btnCharSexClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharSexName', itemId: "DirCharSexName", id: "DirCharSexName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }

                                    ]
                                },

                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                    items: [

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Размер", flex: 1, allowBlank: true,
                                            margin: "0 0 0 10",
                                            store: this.storeDirCharSizesGrid, // store getting items from server
                                            valueField: 'DirCharSizeID',
                                            hiddenName: 'DirCharSizeID',
                                            displayField: 'DirCharSizeName',
                                            name: 'DirCharSizeID', itemId: "DirCharSizeID", id: "DirCharSizeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharSizeEdit", id: "btnCharSizeEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharSizeReload", id: "btnCharSizeReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharSizeClear", id: "btnCharSizeClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharSizeName', itemId: "DirCharSizeName", id: "DirCharSizeName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }

                                    ]
                                },
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                        //DirCharStyles, DirCharTextures
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                    items: [
                                        /*
                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Поставщик", flex: 1, allowBlank: false,

                                            store: this.storeDirCharStylesGrid, // store getting items from server
                                            valueField: 'DirCharStyleID',
                                            hiddenName: 'DirCharStyleID',
                                            displayField: 'DirCharStyleName',
                                            name: 'DirCharStyleID', itemId: "DirCharStyleID", id: "DirCharStyleID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharStyleEdit", id: "btnCharStyleEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharStyleReload", id: "btnCharStyleReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharStyleClear", id: "btnCharStyleClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        
                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharStyleName', itemId: "DirCharStyleName", id: "DirCharStyleName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }
                                        */

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Поставщик", flex: 1, allowBlank: false,

                                            store: this.storeDirContractorsGrid, // store getting items from server
                                            valueField: 'DirContractorID',
                                            hiddenName: 'DirContractorID',
                                            displayField: 'DirContractorName',
                                            name: 'DirContractorID', itemId: "DirContractorID", id: "DirContractorID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharStyleEdit", id: "btnCharStyleEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharStyleReload", id: "btnCharStyleReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharStyleClear", id: "btnCharStyleClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Поставщик наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirContractorName', itemId: "DirContractorName", id: "DirContractorName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }
                                    ]
                                },

                                {
                                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                                    items: [

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Текстура", flex: 1, allowBlank: true,
                                            margin: "0 0 0 10",
                                            store: this.storeDirCharTexturesGrid, // store getting items from server
                                            valueField: 'DirCharTextureID',
                                            hiddenName: 'DirCharTextureID',
                                            displayField: 'DirCharTextureName',
                                            name: 'DirCharTextureID', itemId: "DirCharTextureID", id: "DirCharTextureID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            editable: true, typeAhead: true, minChars: 2
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharTextureEdit", id: "btnCharTextureEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharTextureReload", id: "btnCharTextureReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        { xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharTextureClear", id: "btnCharTextureClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharTextureName', itemId: "DirCharTextureName", id: "DirCharTextureName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                        }

                                    ]
                                },
                            ]
                        },
                    ]
                },

                { xtype: 'container', height: 5 },


                //Заказчик (Сотрудник)
                //{ xtype: 'textfield', fieldLabel: "DirEmployeeID", name: "DirEmployeeID", id: "DirEmployeeID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                //{ xtype: 'displayfield', fieldLabel: 'Заказчик', name: "DirEmployeeName", id: "DirEmployeeName" + this.UO_id, readOnly: true, allowBlank: false, flex: 2 },

                { xtype: 'container', height: 5 },

                // Телефон + ФИО
                {
                    xtype: 'fieldset', width: "95%", title: "Клиент", items: [

                        { xtype: 'container', height: 5 },

                        //Телефон + ФИО
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'textfield', fieldLabel: "Телефон", labelWidth: 50, name: "DirOrderIntContractorPhone", itemId: "DirOrderIntContractorPhone", id: "DirOrderIntContractorPhone" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    allowBlank: false, flex: 1,
                                },
                                { xtype: 'textfield', fieldLabel: "ФИО", labelWidth: 30, name: "DirOrderIntContractorName", id: "DirOrderIntContractorName" + this.UO_id, allowBlank: false, flex: 2, margin: "0 0 0 5" },

                                //{ xtype: 'displayfield', fieldLabel: lanCount, name: "QuantityCount", id: "QuantityCount" + this.UO_id, allowBlank: false, flex: 1, margin: "0 0 0 5" },

                                  {
                                      xtype: 'viewDateField', width: 190,
                                      fieldLabel: "Дата.исполнения", labelWidth: 95, //labelStyle: 'width:120px',
                                      margin: "0 0 0 25", name: "DateDone", id: "DateDone" + this.UO_id, allowBlank: false
                                  },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                    ]
                },

                { xtype: 'container', height: 5 },

                { xtype: 'textfield', fieldLabel: "Примечание", name: "Description", id: "Description" + this.UO_id, flex: 1, allowBlank: true },

            ],


            //buttonAlign: 'left',
            buttons: [
                {
                    id: "btnSave" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                    UO_Action: "save",
                    text: lanRecord, icon: '../Scripts/sklad/images/save.png',
                    style: "width: 120px; height: 40px;",
                },
                /*" ",
                {
                    id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint", hidden: true, style: "width: 120px; height: 40px;",
                    text: lanPrint, icon: '../Scripts/sklad/images/print.png',
                    menu:
                    [
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintHtml",
                            UO_Action: "html",
                            text: "Html", icon: '../Scripts/sklad/images/html.png',
                        },
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintExcel",
                            UO_Action: "excel",
                            text: "MS Excel", icon: '../Scripts/sklad/images/excel.png',
                        },

                        "-",

                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint_barcode",
                            UO_Action: "barcode",
                            text: "Штрих-Код", icon: '../Scripts/sklad/images/print.png',
                        },
                    ]
                },*/
            ]

        });


        //body
        this.items = [
            
            formPanelEdit,

        ],

        this.callParent(arguments);
    }

});