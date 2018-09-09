Ext.define("PartionnyAccount.view.Sklad/Object/Service/API/viewAPI10", {
    extend: "Ext.Window", //extend: "Ext.panel.Panel",
    alias: "widget.viewAPI10",

    layout: "border", //!!! Важно для Ресайз-а внутренней панели !!!
    width: 575, height: 485,
    region: "center",
    monitorValid: true,
    //autoScroll: false,
    defaultType: 'textfield',
    title: "API 1.0",

    frame: true,
    border: false,
    resizable: false,
    //modal: true,
    buttonAlign: 'left',

    timeout: varTimeOutDefault,
    waitMsg: lanLoading,

    UO_maximize: false, //Максимизировать во весь экран
    UO_Center: true,    //true - в центре экрана, false - окна каскадом

    autoHeight: true,

    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    conf: {},

    initComponent: function () {
        
        var btnGen = new Ext.Button({
            text: "V",
            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnGen",

            /*
            handler: function () {
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    waitMsg: lanUpload,
                    url: HTTP_Api10 + "1/?pID=KeyGen",
                    method: 'GET',
                    success: function (result) {
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == false) { Ext.Msg.alert(lanOrgName, lanError + "<BR>" + sData.data); }
                        else {
                            Ext.getCmp("API10Key" + this.UO_id).setValue(sData.data);
                        }
                    },
                    failure: function (form, action) { PanelSubmitFailure(form, action); }
                });
            }
            */

        });

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

                { xtype: 'container', height: 5 },

                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'textfield', fieldLabel: "№", name: "API10ID", id: "API10ID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                        { xtype: 'textareafield', fieldLabel: 'Код идентификации', labelWidth: 150, name: "API10Key", id: "API10Key" + this.UO_id, flex: 1, height: 35, allowBlank: false, readOnly: true },
                        btnGen

                        //{ UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnGen", text: "V" },
                    ]
                },



                {
                    xtype: 'container',
                    //height: 250,
                    width: 575,
                    layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        {
                            xtype: 'fieldset',
                            title: lanAccess + " Export",
                            width: 270,
                            layout: 'anchor',
                            items: [
                                {
                                    xtype: "checkbox", boxLabel: "Справочник Товар", name: "ExportDirNomens", id: "ExportDirNomens" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    width: 275, inputValue: true,
                                    handler: function (ctl, val) {
                                        //val==true - checked, val==false - No checked
                                        if (val) {
                                            Ext.getCmp("ExportDirNomen_DirNomenNameFull" + ctl.UO_id).enable();
                                            Ext.getCmp("ExportDirNomen_Description" + ctl.UO_id).enable();
                                            Ext.getCmp("ExportDirNomen_DescriptionFull" + ctl.UO_id).enable();
                                            Ext.getCmp("ExportDirNomen_ImageLink" + ctl.UO_id).enable();
                                        }
                                        else {
                                            Ext.getCmp("ExportDirNomen_DirNomenNameFull" + ctl.UO_id).disable();
                                            Ext.getCmp("ExportDirNomen_Description" + ctl.UO_id).disable();
                                            Ext.getCmp("ExportDirNomen_DescriptionFull" + ctl.UO_id).disable();
                                            Ext.getCmp("ExportDirNomen_ImageLink" + ctl.UO_id).disable();
                                        }
                                    }
                                },

                                {
                                    xtype: 'fieldset',
                                    title: "Дополнительные поля",
                                    width: 220,
                                    layout: 'anchor',
                                    items: [
                                        {
                                            xtype: "checkbox", boxLabel: " - Полное наименование", name: "ExportDirNomen_DirNomenNameFull", id: "ExportDirNomen_DirNomenNameFull" + this.UO_id,
                                            width: 275, inputValue: true,
                                            handler: function (ctl, val) {
                                                if (val) Ext.Msg.alert(lanOrgName, "Это дополнительное поле содержит большое к-во символов и может замедлить экспорт данных!");
                                            }
                                        },
                                        {
                                            xtype: "checkbox", boxLabel: " - Описание", name: "ExportDirNomen_Description", id: "ExportDirNomen_Description" + this.UO_id,
                                            width: 275, inputValue: true,
                                            handler: function (ctl, val) {
                                                if (val) Ext.Msg.alert(lanOrgName, "Это дополнительное поле содержит большое к-во символов и может замедлить экспорт данных!");
                                            }
                                        },
                                        {
                                            xtype: "checkbox", boxLabel: " - Полное описание", name: "ExportDirNomen_DescriptionFull", id: "ExportDirNomen_DescriptionFull" + this.UO_id,
                                            width: 275, inputValue: true,
                                            handler: function (ctl, val) {
                                                if (val) Ext.Msg.alert(lanOrgName, "Это дополнительное поле содержит большое к-во символов и может замедлить экспорт данных!");
                                            }
                                        },
                                        {
                                            xtype: "checkbox", boxLabel: " - Изображение", name: "ExportDirNomen_ImageLink", id: "ExportDirNomen_ImageLink" + this.UO_id,
                                            width: 275, inputValue: true,
                                            handler: function (ctl, val) {
                                                if (val) Ext.Msg.alert(lanOrgName, "Это дополнительное поле содержит большое к-во символов и может замедлить экспорт данных!");
                                            }
                                        },
                                    ]
                                },
                                { xtype: "checkbox", boxLabel: "Справочники Хар-ки товара", name: "ExportDirChars", id: "ExportDirChars" + this.UO_id, width: 275, inputValue: true },
                                { xtype: "checkbox", boxLabel: "Справочник Контрагенты", name: "ExportDirContractors", id: "ExportDirContractors" + this.UO_id, width: 275, inputValue: true },
                                { xtype: "checkbox", boxLabel: "Торговля: Остатки", name: "ExportRemRemnants", id: "ExportRemRemnants" + this.UO_id, width: 275, inputValue: true },
                                { xtype: "checkbox", boxLabel: "Торговля: Партии", name: "ExportRemParties", id: "ExportRemParties" + this.UO_id, width: 275, inputValue: true },

                                //СЦ
                                { xtype: "checkbox", boxLabel: "СЦ: Устройства", name: "ExportDirServiceNomens", id: "ExportDirServiceNomens" + this.UO_id, width: 275, inputValue: true },

                                //БУ
                                { xtype: "checkbox", boxLabel: "Б/У: Партии", name: "ExportRem2Parties", id: "ExportRem2Parties" + this.UO_id, width: 275, inputValue: true },
                            ]
                        },
                        {
                            xtype: 'fieldset',
                            title: lanAccess + " Import",
                            width: 270,
                            layout: 'anchor',
                            items: [
                                //{ xtype: "checkbox", boxLabel: "Справочник Товар", name: "ImportDirNomens", id: "ImportDirNomens" + this.UO_id, width: 275, inputValue: true },
                                //{ xtype: "checkbox", boxLabel: "Справочник Контрагенты", name: "ImportDirContractors", id: "ImportDirContractors" + this.UO_id, width: 275, inputValue: true },
                                { xtype: "checkbox", boxLabel: "Заказ покупателя", name: "ImportDocOrderInts", id: "ImportDocOrderInts" + this.UO_id, width: 275, inputValue: true },
                            ]
                        }
                    ]

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