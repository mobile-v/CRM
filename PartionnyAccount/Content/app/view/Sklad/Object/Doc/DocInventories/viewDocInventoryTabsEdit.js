Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocInventories/viewDocInventoryTabsEdit", {
    extend: "Ext.Window", UO_Extend: "Window", //extend: InterfaceSystemObjName,
    alias: "widget.viewDocInventoryTabsEdit",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 625, height: 185,
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


                // !!! НЕ ВИДИМОЕ !!! === === === === === === === === === === === === === === === === ===
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' }, flex: 1, hidden: true,
                    items: [
                        { xtype: 'textfield', fieldLabel: "DirContractorID", name: 'DirContractorID', readOnly: true, },
                        {
                            xtype: 'textfield', fieldLabel: "DirNomenMinimumBalance",
                            name: 'DirNomenMinimumBalance', id: "DirNomenMinimumBalance" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true,
                        },

                        { xtype: 'textfield', fieldLabel: "DirCharColourID", name: 'DirCharColourID', readOnly: true, }, { xtype: 'textfield', fieldLabel: "DirCharColourName", name: 'DirCharColourName', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "DirCharMaterialID", name: 'DirCharMaterialID', readOnly: true, }, { xtype: 'textfield', fieldLabel: "DirCharMaterialName", name: 'DirCharMaterialName', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "DirCharNameID", name: 'DirCharNameID', readOnly: true, }, { xtype: 'textfield', fieldLabel: "DirCharNameName", name: 'DirCharNameName', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "DirCharSeasonID", name: 'DirCharSeasonID', readOnly: true, }, { xtype: 'textfield', fieldLabel: "DirCharSeasonName", name: 'DirCharSeasonName', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "DirCharSexID", name: 'DirCharSexID', readOnly: true, }, { xtype: 'textfield', fieldLabel: "DirCharSexName", name: 'DirCharSexName', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "DirCharSizeID", name: 'DirCharSizeID', readOnly: true, }, { xtype: 'textfield', fieldLabel: "DirCharSizeName", name: 'DirCharSizeName', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "DirCharStyleID", name: 'DirCharStyleID', readOnly: true, }, { xtype: 'textfield', fieldLabel: "DirCharStyleName", name: 'DirCharStyleName', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "DirCharTextureID", name: 'DirCharTextureID', readOnly: true, }, { xtype: 'textfield', fieldLabel: "DirCharTextureName", name: 'DirCharTextureName', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "DirChar", name: 'DirChar', readOnly: true, }, 
                        { xtype: 'textfield', fieldLabel: "SerialNumber", name: 'SerialNumber', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "Barcode", name: 'Barcode', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "PriceRetailVAT", name: 'PriceRetailVAT', readOnly: true, }, { xtype: 'textfield', fieldLabel: "PriceRetailCurrency", name: 'PriceRetailCurrency', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "PriceWholesaleVAT", name: 'PriceWholesaleVAT', readOnly: true, }, { xtype: 'textfield', fieldLabel: "PriceWholesaleCurrency", name: 'PriceWholesaleCurrency', readOnly: true, },
                        { xtype: 'textfield', fieldLabel: "PriceIMVAT", name: 'PriceIMVAT', readOnly: true, }, { xtype: 'textfield', fieldLabel: "PriceIMCurrency", name: 'PriceIMCurrency', readOnly: true, },


                        {
                            xtype: 'viewComboBox', fieldLabel: lanCurrency, flex: 2, allowBlank: true, store: this.storeDirCurrenciesGrid, valueField: 'DirCurrencyID', hiddenName: 'DirCurrencyID', displayField: 'DirCurrencyName',
                            name: 'DirCurrencyID', itemId: "DirCurrencyID", id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, 
                        },

                        {
                            xtype: 'textfield', regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Курс",
                            name: 'DirCurrencyRate', itemId: "DirCurrencyRate", id: "DirCurrencyRate" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true, 
                        },
                        {
                            xtype: 'textfield', regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "Кратность", margin: "0 0 0 10",
                            name: 'DirCurrencyMultiplicity', itemId: "DirCurrencyMultiplicity", id: "DirCurrencyMultiplicity" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, readOnly: true, 
                        },
                    ]
                },
                

                // !!! НЕ ВИДИМОЕ !!! === === === === === === === === === === === === === === === === ===





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

                        //Товар
                        {
                            xtype: 'textfield',
                            fieldLabel: "Имя товара", emptyText: "...", allowBlank: false, flex: 1,
                            name: 'DirNomenName', itemId: "DirNomenName", id: "DirNomenName" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            readOnly: true
                        },
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: false,
                            name: 'DirNomenID', itemId: "DirNomenID", id: "DirNomenID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                        //Партия
                        {
                            xtype: 'viewTriggerDirField',
                            allowBlank: true,
                            name: 'RemPartyID', itemId: "RemPartyID", id: "RemPartyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                        },
                    ]
                },
                

                { xtype: 'container', height: 5 },


                {
                    xtype: 'fieldset', width: "95%", layout: 'anchor',
                    title: "Расходная цена - количество",
                    autoHeight: true,
                    items: [

                        {
                            xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                //К-во Списания
                                /*{
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>" + lanCount + " (спис.)</b>",
                                    name: 'Quantity_WriteOff', itemId: 'Quantity_WriteOff', id: 'Quantity_WriteOff' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },*/
                                {
                                    xtype: 'numberfield',
                                    value: 1, maxValue: 999999, minValue: 1,
                                    allowBlank: false, flex: 1, fieldLabel: "<b>" + lanCount + " (спис.)</b>",
                                    name: 'Quantity_WriteOff', itemId: 'Quantity_WriteOff', id: 'Quantity_WriteOff' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },

                                //К-во Прих
                                /*{
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>" + lanCount + " (прих.)</b>", margin: "0 0 0 10",
                                    name: 'Quantity_Purch', itemId: 'Quantity_Purch', id: 'Quantity_Purch' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },*/
                                {
                                    xtype: 'numberfield',
                                    value: 1, maxValue: 999999, minValue: 1,
                                    allowBlank: false, flex: 1, fieldLabel: "<b>" + lanCount + " (прих.)</b>", margin: "0 0 0 10",
                                    name: 'Quantity_Purch', itemId: 'Quantity_Purch', id: 'Quantity_Purch' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                },


                                //Приходная цена
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: lanPriceInCurr, margin: "0 0 0 10",
                                    name: 'PriceVAT', itemId: 'PriceVAT', id: 'PriceVAT' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
                                },
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: "<b>" + lanPrice + "</b>", margin: "0 0 0 10",
                                    name: 'PriceCurrency', itemId: 'PriceCurrency', id: 'PriceCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    hidden: true
                                },

                                //Сумма
                                {
                                    xtype: 'textfield',
                                    regex: /^[+\-]?\d+(?:\.\d+)?$/, allowBlank: false, flex: 1, fieldLabel: lanPriceInCurr, margin: "0 0 0 10",
                                    name: 'SUMPurchPriceVATCurrency', itemId: 'SUMPurchPriceVATCurrency', id: 'SUMPurchPriceVATCurrency' + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    hidden: true
                                },
                            ]
                        },

                        //Для растояния между Контейнерами
                        { xtype: 'container', height: 5 },

                    ]
                },

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