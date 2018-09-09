Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocMovements/viewDocMovementTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocMovementTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 675, height: 260,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: "Спецификация",
    autoHeight: true,

    frame: true,
    border: false,
    resizable: false,
    //modal: true,
    buttonAlign: 'left',

    timeout: varTimeOutDefault,
    waitMsg: lanLoading,

    UO_maximize: false, //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом

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

            UO_GridServerParam1: this.UO_GridServerParam1, //Параметры для Грида, например передать склад, что бы показать поле остаток!


            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            width: "100%", height: "100%",
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            autoScroll: true,

            items: [

                // === Не видимые поля === === ===
                { xtype: 'textfield', fieldLabel: "DirReturnTypeID", name: 'DirReturnTypeID', id: 'DirReturnTypeID' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirReturnTypeName", name: 'DirReturnTypeName', id: 'DirReturnTypeName' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirDescriptionID", name: 'DirDescriptionID', id: 'DirDescriptionID' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirDescriptionName", name: 'DirDescriptionName', id: 'DirDescriptionName' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },


                //DirNomen
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1,
                    items: [

                        //Остаток: максимум сколько можно списать
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: true,
                            name: 'Remnant', itemId: "Remnant", id: "Remnant" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },

                       {
                           xtype: 'textfield',
                           fieldLabel: "Артикул", labelAlign: "top", emptyText: "...", allowBlank: false, width: 100,
                           name: 'DirNomenID', itemId: "DirNomenID", id: "DirNomenID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                           readOnly: true
                       },
                        {
                            xtype: 'textfield',
                            fieldLabel: "Имя товара", labelAlign: "top", emptyText: "...", allowBlank: false, flex: 1, margin: "0 0 0 5",
                            name: 'DirNomenName', itemId: "DirNomenName", id: "DirNomenName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },

                        //К-во
                        {
                            xtype: 'numberfield',
                            //regex: /^[+\-]?\d+(?:\.\d+)?$/,
                            value: 1, minValue: 1, maxValue: 999999,
                            allowBlank: false, width: 125, fieldLabel: "<b>" + lanCount + "</b>", labelAlign: "top", margin: "0 0 0 5",
                            name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                         //Партия
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: false,
                            name: 'RemPartyID', itemId: "RemPartyID", id: "RemPartyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },

                               
                //Серийный номер + Штрих-код
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor', hidden: true,
                    title: "Серийный номер + Штрих-код",
                    autoHeight: true,
                    items: [
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                {
                                    xtype: 'textfield',
                                    allowBlank: true, flex: 1, fieldLabel: "Серийный номер",
                                    name: 'SerialNumber', itemId: "SerialNumber", id: "SerialNumber" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },
                                {
                                    xtype: 'textfield',
                                    allowBlank: true, flex: 1, fieldLabel: "Штрих-код", margin: "0 0 0 10", 
                                    name: 'Barcode', itemId: "Barcode", id: "Barcode" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },
                            ]
                        }
                    ]
                },

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
                                            //editable: true, typeAhead: true, minChars: 2
                                            readOnly: true
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharColourEdit", id: "btnCharColourEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharColourReload", id: "btnCharColourReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharColourClear", id: "btnCharColourClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        
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
                                            //editable: true, typeAhead: true, minChars: 2
                                            readOnly: true
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharMaterialEdit", id: "btnCharMaterialEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharMaterialReload", id: "btnCharMaterialReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharMaterialClear", id: "btnCharMaterialClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

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
                                            //editable: true, typeAhead: true, minChars: 2
                                            readOnly: true
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharNameEdit", id: "btnCharNameEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharNameReload", id: "btnCharNameReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharNameClear", id: "btnCharNameClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

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
                                            //editable: true, typeAhead: true, minChars: 2,
                                            readOnly: true
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharSeasonEdit", id: "btnCharSeasonEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharSeasonReload", id: "btnCharSeasonReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharSeasonClear", id: "btnCharSeasonClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

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
                                            //editable: true, typeAhead: true, minChars: 2
                                            readOnly: true
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharSexEdit", id: "btnCharSexEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharSexReload", id: "btnCharSexReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharSexClear", id: "btnCharSexClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

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
                                            //editable: true, typeAhead: true, minChars: 2
                                            readOnly: true
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharSizeEdit", id: "btnCharSizeEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharSizeReload", id: "btnCharSizeReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharSizeClear", id: "btnCharSizeClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

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

                                        {
                                            xtype: 'viewComboBox',
                                            fieldLabel: "Поставщик", flex: 1, allowBlank: true,

                                            store: this.storeDirCharStylesGrid, // store getting items from server
                                            valueField: 'DirCharStyleID',
                                            hiddenName: 'DirCharStyleID',
                                            displayField: 'DirCharStyleName',
                                            name: 'DirCharStyleID', itemId: "DirCharStyleID", id: "DirCharStyleID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            //Поиск
                                            //editable: true, typeAhead: true, minChars: 2
                                            readOnly: true
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharStyleEdit", id: "btnCharStyleEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharStyleReload", id: "btnCharStyleReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharStyleClear", id: "btnCharStyleClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

                                        {
                                            xtype: 'textfield',
                                            fieldLabel: "Цвет наименование", emptyText: "...", allowBlank: true, flex: 1, hidden: true,
                                            name: 'DirCharStyleName', itemId: "DirCharStyleName", id: "DirCharStyleName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
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
                                            //editable: true, typeAhead: true, minChars: 2
                                            readOnly: true
                                        },
                                        //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCharTextureEdit", id: "btnCharTextureEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCharTextureReload", id: "btnCharTextureReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                        //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCharTextureClear", id: "btnCharTextureClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },

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

                //Валюта + Цена в Валюте - !!! Не видимое !!!
                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor', hidden: true,
                    title: lanPurchase,
                    autoHeight: true,
                    items: [
                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            //hidden: true,
                            items: [
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: lanCurrency, flex: 2, allowBlank: true,
                                    
                                    store: this.storeDirCurrenciesGrid, // store getting items from server
                                    valueField: 'DirCurrencyID',
                                    hiddenName: 'DirCurrencyID',
                                    displayField: 'DirCurrencyName',
                                    name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    //editable: true, typeAhead: true, minChars: 2
                                    readOnly: true
                                },
                                //{ xtype: 'button', tooltip: "Edit", iconCls: "button-image-edit", itemId: "btnCurrencyEdit", id: "btnCurrencyEdit" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                //{ xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload", itemId: "btnCurrencyReload", id: "btnCurrencyReload" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },
                                //{ xtype: 'button', tooltip: "Clear", text: "X", itemId: "btnCurrencyClear", id: "btnCurrencyClear" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall },


                                //Приходная цена в валюте
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, margin: "0 0 0 10", fieldLabel: lanPriceInCurr, margin: "0 0 0 10",
                                    name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            //hidden: true,
                            items: [
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс", 
                                    name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", margin: "0 0 0 10",
                                    name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },
                            ]
                        },

                        { xtype: 'container', height: 5 },

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            //hidden: true,
                            items: [
                                //К-во
                                /*{
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>" + lanCount + "</b>",
                                    name: 'Quantity', itemId: 'Quantity', id: 'Quantity' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },*/

                                //Приходная цена
                                {
                                    xtype: 'textfield', 
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>" + lanPrice + "</b>", margin: "0 0 0 10",
                                    name: 'PriceCurrency', itemId: 'PriceCurrency', id: 'PriceCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    readOnly: true
                                },
                            ]
                        },
                    ]
                },
                                
                //Цены - Расход - !!! НЕ ВИДИМАЯ !!!
                {
                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'hbox' }, hidden: true,
                    items: [

                        {
                            name: 'ConsumableWholesale',
                            title: lanRetail,
                            autoHeight: true,

                            xtype: 'fieldset', flex: 1, layout: 'anchor',

                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [

                                        {
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupRetail', itemId: 'MarkupRetail', id: 'MarkupRetail' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },
                                        { //lanPriceVat
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceRetailVAT', itemId: 'PriceRetailVAT', id: 'PriceRetailVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true
                                        },
                                        //CurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceRetailCurrency', itemId: 'PriceRetailCurrency', id: 'PriceRetailCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        }

                                    ]
                                },

                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                        {
                            name: 'ConsumableWholesale',
                            title: lanWholesale,
                            autoHeight: true,

                            xtype: 'fieldset', flex: 1, layout: 'anchor',

                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [

                                        {
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupWholesale', itemId: 'MarkupWholesale', id: 'MarkupWholesale' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },
                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceWholesaleVAT', itemId: 'PriceWholesaleVAT', id: 'PriceWholesaleVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true
                                        },
                                        //CurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceWholesaleCurrency', itemId: 'PriceWholesaleCurrency', id: 'PriceWholesaleCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        }

                                    ]
                                },

                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                        {
                            name: 'ConsumableIM',
                            title: "Интернет-магазин",
                            autoHeight: true,

                            xtype: 'fieldset', flex: 1, layout: 'anchor',

                            items: [
                                {
                                    xtype: 'container', flex: 1, layout: { align: 'stretch', type: 'vbox' },
                                    items: [

                                        {
                                            xtype: 'textfield', fieldLabel: lanSurcharge + " %", name: 'MarkupIM', itemId: 'MarkupIM', id: 'MarkupIM' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        },
                                        {
                                            xtype: 'textfield', fieldLabel: lanPriceInCurr, name: 'PriceIMVAT', itemId: 'PriceIMVAT', id: 'PriceIMVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245, hidden: true
                                        },
                                        //CurrentExchange
                                        {
                                            xtype: 'textfield', fieldLabel: lanPrice, name: 'PriceIMCurrency', itemId: 'PriceIMCurrency', id: 'PriceIMCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                            regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, width: 245
                                        }

                                    ]
                                },

                            ]
                        },

                    ]
                }

            ]
        });



        this.items = [

            formPanel

        ];


        this.buttons = [
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                text: lanSave, tooltip: lanSave, icon: '../Scripts/sklad/images/save.png'
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel",
                text: lanCancel, tooltip: lanCancel, icon: '../Scripts/sklad/images/cancel.png'
            },

            "->",

            {
                id: "btnDel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnDel",
                text: lanDelete, tooltip: lanDelete, icon: '../Scripts/sklad/images/table_delete.png'
            },

        ],


        this.callParent(arguments);
    }

});