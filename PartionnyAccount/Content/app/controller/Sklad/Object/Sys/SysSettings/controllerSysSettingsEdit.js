Ext.define("PartionnyAccount.controller.Sklad/Object/Sys/SysSettings/controllerSysSettingsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены другие виджеты ...)
            //Закрыте
            'viewSysSettingsEdit': { close: this.this_close },

            //Sms
            'viewSysSettingsEdit [itemId=SmsActive]': { change: this.onSmsActiveChecked },
            //Kkm
            'viewSysSettingsEdit [itemId=KKMSActive]': { change: this.onKKMSActiveChecked },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewSysSettingsEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewSysSettingsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewSysSettingsEdit button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },
    //Sms
    onSmsActiveChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("SmsServiceID" + ctl.UO_id).enable();
            Ext.getCmp("SmsLogin" + ctl.UO_id).enable();
            Ext.getCmp("SmsPassword" + ctl.UO_id).enable();
            Ext.getCmp("SmsTelFrom" + ctl.UO_id).enable();
            Ext.getCmp("DocServicePurchSmsAutoShow" + ctl.UO_id).enable();
            Ext.getCmp("SmsAutoShow" + ctl.UO_id).enable();
            Ext.getCmp("SmsAutoShow9" + ctl.UO_id).enable();
            Ext.getCmp("SmsAutoShowServiceFromArchiv" + ctl.UO_id).enable();
        }
        else {
            Ext.getCmp("SmsServiceID" + ctl.UO_id).disable();
            Ext.getCmp("SmsLogin" + ctl.UO_id).disable();
            Ext.getCmp("SmsPassword" + ctl.UO_id).disable();
            Ext.getCmp("SmsTelFrom" + ctl.UO_id).disable();
            Ext.getCmp("DocServicePurchSmsAutoShow" + ctl.UO_id).disable();
            Ext.getCmp("SmsAutoShow" + ctl.UO_id).disable();
            Ext.getCmp("SmsAutoShow9" + ctl.UO_id).disable();
            Ext.getCmp("SmsAutoShowServiceFromArchiv" + ctl.UO_id).disable();
        }
    },
    //Kkm
    onKKMSActiveChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("KKMSUrlServer" + ctl.UO_id).enable(); if (!Ext.getCmp("KKMSUrlServer" + ctl.UO_id).getValue()) { Ext.getCmp("KKMSUrlServer" + ctl.UO_id).setValue("http://localhost:5893/"); }
            Ext.getCmp("KKMSUser" + ctl.UO_id).enable(); if (!Ext.getCmp("KKMSUser" + ctl.UO_id).getValue()) { Ext.getCmp("KKMSUser" + ctl.UO_id).setValue("User"); }
            Ext.getCmp("KKMSPassword" + ctl.UO_id).enable(); if (!Ext.getCmp("KKMSPassword" + ctl.UO_id).getValue()) { Ext.getCmp("KKMSPassword" + ctl.UO_id).setValue("30"); }
            Ext.getCmp("KKMSNumDevice" + ctl.UO_id).enable();
            Ext.getCmp("KKMSCashierVATIN" + ctl.UO_id).enable();
            Ext.getCmp("KKMSTaxVariant" + ctl.UO_id).enable();
            Ext.getCmp("KKMSTax" + ctl.UO_id).enable();
        }
        else {
            Ext.getCmp("KKMSUrlServer" + ctl.UO_id).disable();
            Ext.getCmp("KKMSUser" + ctl.UO_id).disable();
            Ext.getCmp("KKMSPassword" + ctl.UO_id).disable();
            Ext.getCmp("KKMSNumDevice" + ctl.UO_id).disable();
            Ext.getCmp("KKMSCashierVATIN" + ctl.UO_id).disable();
            Ext.getCmp("KKMSTaxVariant" + ctl.UO_id).disable();
            Ext.getCmp("KKMSTax" + ctl.UO_id).disable();
        }
    },


    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Сохранение
        widgetXForm.submit({
            method: "PUT",
            url: HTTP_SysSettings + "/" + parseInt(Ext.getCmp("SysSettingsID" + aButton.UO_id).value) + "/",

            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                //Закрываем форму
                Ext.getCmp(aButton.UO_idMain).close();
                //Перегружаем "Настройки"
                Variables_SettingsRequest();
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "settings/", '_blank');
    },

});