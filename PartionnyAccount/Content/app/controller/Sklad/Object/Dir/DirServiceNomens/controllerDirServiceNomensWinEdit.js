Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirServiceNomens/controllerDirServiceNomensWinEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirServiceNomensWinEdit': { close: this.this_close },


            //Грид (itemId=grid)
            'viewDirServiceNomensWinEdit button#btnGridAddAll': { click: this.onGrid_BtnGridAddAll },
            'viewDirServiceNomensWinEdit button#btnGridDelete': { click: this.onGrid_BtnGridDelete },

            //Типичные неисправности
            'viewDirServiceNomensWinEdit [itemId=Faults1Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults2Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults3Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults4Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults5Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults6Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults7Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults8Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults9Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults10Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults11Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults12Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults13Check]': { change: this.onFaults1CheckChecked },
            'viewDirServiceNomensWinEdit [itemId=Faults14Check]': { change: this.onFaults1CheckChecked },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirServiceNomensWinEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirServiceNomensWinEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirServiceNomensWinEdit button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    //Грид (itemId=grid) === === === === ===

    //Добавить все
    onGrid_BtnGridAddAll: function (aButton, aEvent, aOptions) {
        //varStoreDirServiceNomenTypicalFaultsGrid
        
        //1.
        var store = Ext.getCmp("PanelGridDirServiceNomenPrice_" + aButton.UO_id).getStore();
        store.setData([], false);
        Ext.getCmp("PanelGridDirServiceNomenPrice_" + aButton.UO_id).store = store;

        //2.
        //store.insert(store.data.items.length, rec);
        for (var i = 0; i < varStoreDirServiceNomenTypicalFaultsGrid.data.items.length; i++) {
            varStoreDirServiceNomenTypicalFaultsGrid.data.items[i].data.PriceVAT = 0;
            store.insert(store.data.items.length, varStoreDirServiceNomenTypicalFaultsGrid.data.items[i].data);
        }

    },
    //Удалить все
    onGrid_BtnGridDelete: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName,
            msg: "Удалить все записи?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    /*
                    var selection = Ext.getCmp("grid_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
                    if (selection) { Ext.getCmp("grid_" + aButton.UO_id).store.remove(selection); }
                    */

                    var store = Ext.getCmp("PanelGridDirServiceNomenPrice_" + aButton.UO_id).getStore();
                    store.setData([], false);
                    Ext.getCmp("PanelGridDirServiceNomenPrice_" + aButton.UO_id).store = store;
                }
            }
        });
    },


    //Типичные неисправности
    onFaults1CheckChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("Faults" + ctl.UO_Numb + "Price" + ctl.UO_id).enable();
            Ext.getCmp("Faults" + ctl.UO_Numb + "Price" + ctl.UO_id).setValue(0);
        }
        else {
            Ext.getCmp("Faults" + ctl.UO_Numb + "Price" + ctl.UO_id).disable();
            Ext.getCmp("Faults" + ctl.UO_Numb + "Price" + ctl.UO_id).setValue(0);
        }
    },
    



    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Таблица
        /*
        var recordsDirServiceNomenPriceGrid = [];
        var store = Ext.getCmp("PanelGridDirServiceNomenPrice_" + aButton.UO_id).store;
        store.data.each(function (rec) { recordsDirServiceNomenPriceGrid.push(rec.data); });
        */

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirServiceNomens;
        if (parseInt(Ext.getCmp("DirServiceNomenID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirServiceNomens + "?id=" + parseInt(Ext.getCmp("DirServiceNomenID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            //params: { recordsDirServiceNomenPriceGrid: Ext.encode(recordsDirServiceNomenPriceGrid) },

            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                fun_ReopenTree_1(aButton.UO_id, undefined, aButton.UO_idCall, action.result.data);
                Ext.getCmp(aButton.UO_idMain).close();
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-tovar/", '_blank');
    }
});