Ext.define("PartionnyAccount.controller.Sklad/Object/List/controllerListObjectPFsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewListObjectPFsEdit': { close: this.this_close },


            //Грид (itemId=grid)
            'viewListObjectPFsEdit button#btnGridNew': { click: this.onGrid_BtnNew },
            'viewListObjectPFsEdit button#btnGridEdit': { click: this.onGrid_BtnEdit },
            'viewListObjectPFsEdit button#btnGridDelete': { click: this.onGrid_BtnDelete },
            // Клик по Гриду
            'viewListObjectPFsEdit [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },



            //ListObjectPFHtmlTabUseCap
            'viewListObjectPFsEdit [itemId=ListObjectPFHtmlTabUseCap]': { change: this.onListObjectPFHtmlTabUseCapChecked },
            //ListObjectPFHtmlTabUseFooter
            'viewListObjectPFsEdit [itemId=ListObjectPFHtmlTabUseFooter]': { change: this.onListObjectPFHtmlTabUseFooterChecked },
            //ListObjectPFHtmlTabUseText
            'viewListObjectPFsEdit [itemId=ListObjectPFHtmlTabUseText]': { change: this.onListObjectPFHtmlTabUseTextChecked },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewListObjectPFsEdit menuitem#btnSave': { click: this.onBtnSaveClick },
            'viewListObjectPFsEdit menuitem#btnSaveClose': { click: this.onBtnSaveClick },
            'viewListObjectPFsEdit button#btnCancel': { "click": this.onBtnCancelClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    onGrid_BtnNew: function (aButton, aEvent, aOptions) {

        //Выбран ли Склад и Контрагнта
        if (!Ext.getCmp("ListObjectPFHtmlTabUseTab" + aButton.UO_id).getValue()) { Ext.Msg.alert(lanOrgName, "Установите переключатель 'Используется'!"); return; }

        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            undefined,
            undefined,

            undefined,
            undefined,
            undefined,

            "ListObjectID=" + Ext.getCmp("ListObjectID" + aButton.UO_id).getValue() + "&ListObjectField=ListObjectFieldTabShow",     //GridServerParam1
        ]
        ObjectEditConfig("viewListObjectPFTabsEdit", Params);
    },

    onGrid_BtnEdit: function (aButton, aEvent, aOptions) {

        var index = Ext.getCmp("grid_" + aButton.UO_id).store.indexOf(Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0]);
        var record = Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0];

        if (Ext.getCmp(aButton.UO_idMain).UO_Function_Grid == undefined) {
            var Params = [
                "grid_" + aButton.UO_id, //UO_idCall
                true, //UO_Center
                true, //UO_Modal
                2,    // 1 - Новое, 2 - Редактировать
                true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                record,       // Для загрузки данных в форму Б.С. и Договора,

                undefined,
                undefined,
                undefined,

                "ListObjectID=" + Ext.getCmp("ListObjectID" + aButton.UO_id).getValue() + "&ListObjectField=ListObjectFieldTabShow",     //GridServerParam1

            ]
            ObjectEditConfig("viewListObjectPFTabsEdit", Params);
        }
        else {
            Ext.getCmp(aButton.UO_idMain).UO_Function_Grid(Ext.getCmp(aButton.UO_idCall).UO_id, record);
            Ext.getCmp(aButton.UO_idMain).close();
        }
    },

    onGrid_BtnDelete: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName,
            msg: lanDelete + "?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    var selection = Ext.getCmp("grid_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
                    if (selection) { Ext.getCmp("grid_" + aButton.UO_id).store.remove(selection); }
                }
            }
        });
    },


    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#btnGridEdit").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnGridDelete").setDisabled(records.length === 0);
    },
    //Клик: Редактирования или выбор
    onGrid_itemclick: function (view, record, item, index, eventObj) {
        
        if (varSelectOneClick) {
            var index = Ext.getCmp("grid_" + view.grid.UO_id).store.indexOf(Ext.getCmp("grid_" + view.grid.UO_id).getSelectionModel().getSelection()[0]);
            var record = Ext.getCmp("grid_" + view.grid.UO_id).getSelectionModel().getSelection()[0];

            if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
                var Params = [
                    "grid_" + view.grid.UO_id, //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    2,    // 1 - Новое, 2 - Редактировать
                    true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                    index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    record,       // Для загрузки данных в форму Б.С. и Договора,

                    undefined,
                    undefined,
                    undefined,

                    "ListObjectID=" + Ext.getCmp("ListObjectID" + view.grid.UO_id).getValue() + "&ListObjectField=ListObjectFieldTabShow",     //GridServerParam1

                ]
                ObjectEditConfig("viewListObjectPFTabsEdit", Params);
            }
            else {
                Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
                Ext.getCmp(view.grid.UO_idMain).close();
            }
        }
    },
    //ДаблКлик: Редактирования или выбор
    onGrid_itemdblclick: function (view, record, item, index, e) {
        var index = Ext.getCmp("grid_" + view.grid.UO_id).store.indexOf(Ext.getCmp("grid_" + view.grid.UO_id).getSelectionModel().getSelection()[0]);
        var record = Ext.getCmp("grid_" + view.grid.UO_id).getSelectionModel().getSelection()[0];

        if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
            var Params = [
                "grid_" + view.grid.UO_id, //UO_idCall
                true, //UO_Center
                true, //UO_Modal
                2,    // 1 - Новое, 2 - Редактировать
                true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                record,       // Для загрузки данных в форму Б.С. и Договора,

                undefined,
                undefined,
                undefined,

                "ListObjectID=" + Ext.getCmp("ListObjectID" + view.grid.UO_id).getValue() + "&ListObjectField=ListObjectFieldTabShow",     //GridServerParam1

            ]
            ObjectEditConfig("viewListObjectPFTabsEdit", Params);
        }
        else {
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }
    },



    //ListObjectPFHtmlTabUseCap
    onListObjectPFHtmlTabUseCapChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("ListObjectPFHtmlTabUseText" + ctl.UO_id).setValue(false);
            Ext.Msg.alert(lanOrgName, txtMsg039_2);
        }
        else {
        }
    },

    //ListObjectPFHtmlTabUseFooter
    onListObjectPFHtmlTabUseFooterChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("ListObjectPFHtmlTabUseText" + ctl.UO_id).setValue(false);
            Ext.Msg.alert(lanOrgName, txtMsg039_2);
        }
        else {
        }
    },

    //ListObjectPFHtmlTabUseText
    onListObjectPFHtmlTabUseTextChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("ListObjectPFHtmlHeaderUse" + ctl.UO_id).setValue(false);
            Ext.getCmp("ListObjectPFHtmlTabUseTab" + ctl.UO_id).setValue(false);
            Ext.getCmp("ListObjectPFHtmlFooterUse" + ctl.UO_id).setValue(false);
            Ext.Msg.alert(lanOrgName, txtMsg039);
        }
        else {
        }
    },




    // === Кнопки === === ===

    //Сохранить или Сохранить и закрыть
    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        
        try {
            if (Boolean(Ext.getCmp("ListObjectPFHtmlHeaderUse" + aButton.UO_id).getValue()) == true) {
                if (Ext.getCmp("ListObjectPFHtmlHeader" + aButton.UO_id).getValue().length < 1) { Ext.Msg.alert(lanOrgName, txtMsg037_1 + txtMsg037); return; }
            }
        } catch (err) { }
        try {
            if (Boolean(Ext.getCmp("ListObjectPFHtmlTabUseTab" + aButton.UO_id).getValue()) == true) {
                if (storeGridListObjectPFTab.data.items.length == 0) { Ext.Msg.alert(lanOrgName, txtMsg037_2 + txtMsg037); return; }
            }
        } catch (err) { }
        try {
            if (Boolean(Ext.getCmp("ListObjectPFHtmlFooterUse" + aButton.UO_id).getValue()) == true) {
                if (Ext.getCmp("ListObjectPFHtmlFooter" + aButton.UO_id).getValue().length < 1) { Ext.Msg.alert(lanOrgName, txtMsg037_3 + txtMsg037); return; }
            }
        } catch (err) { }
        
        //Табличные данные
        var recordsListObjectPFTab = [];
        var storeGridListObjectPFTab = Ext.getCmp("grid_" + aButton.UO_id).store;
        storeGridListObjectPFTab.data.each(function (rec) { recordsListObjectPFTab.push(rec.data); });
        

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_ListObjectPFs + "?UO_Action=" + aButton.UO_Action;
        if (parseInt(Ext.getCmp("ListObjectPFID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_ListObjectPFs + "?id=" + parseInt(Ext.getCmp("ListObjectPFID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            params: { recordsListObjectPFTab: Ext.encode(recordsListObjectPFTab) },

            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {

                //Если новая накладная присваиваем полученные номера!
                var sData = action.result.data;
                Ext.getCmp('ListObjectPFID' + aButton.UO_id).setValue(sData.ListObjectPFID);

                //Закрыть
                if (aButton.UO_Action == "save_close") { Ext.getCmp(aButton.UO_idMain).close(); }
                //Перегрузить грид, если грид открыт
                if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },

    //Отменить
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
});