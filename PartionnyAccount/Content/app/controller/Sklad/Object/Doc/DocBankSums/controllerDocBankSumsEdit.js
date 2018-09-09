Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocBankSums/controllerDocBankSumsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocBankSumsEdit': { close: this.this_close },

            'viewDocBankSumsEdit button#btnMake': { click: this.onBtnMakeClick },
            'viewDocBankSumsEdit button#btnPay': { click: this.onBtnPayClick },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocBankSumsEdit button#btnSave': { click: this.onBtnSaveClick },
            'viewDocBankSumsEdit button#btnCancel': { click: this.onBtnCancelClick },
            'viewDocBankSumsEdit button#btnHelp': { click: this.onBtnHelpClick },
            'viewDocBankSumsEdit button#btnClose': { click: this.onBtnCloseClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Внести Наличку
    onBtnMakeClick: function (aButton, aEvent, aOptions) {
        controllerDocBankSumsEdit_container_Show(aButton.UO_id, "Вид операции - Внесение на счет", 1);
    },
    //Выплатить Наличку
    onBtnPayClick: function (aButton, aEvent, aOptions) {
        controllerDocBankSumsEdit_container_Show(aButton.UO_id, "Вид операции - снятие или перевод со счета", 2);
    },


    // === Кнопки: Сохранение, Отмена и Помощь === === ===
    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);
        
        //Сохранение - только INSERT
        widgetXForm.submit({
            method: "POST",
            url: HTTP_DocBankSums,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {

                //1. Обновить форму 
                widgetXForm.load({
                    method: "GET",
                    timeout: varTimeOutDefault,
                    waitMsg: lanLoading,
                    url: HTTP_DirBanks + Ext.getCmp("DirBankID" + aButton.UO_id).getValue() + "/", // + "/?DocID=" + IdcallModelData.DocID,
                    success: function (form, action) {
                    },
                    failure: function (form, action) {
                        funPanelSubmitFailure(form, action);
                    }
                });

                //2. Убрать поля
                controllerDocBankSumsEdit_container_Hide(aButton.UO_id);

                //3. Обновить грид
                if (Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        controllerDocBankSumsEdit_container_Hide(aButton.UO_id);
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-bank-sums/", '_blank');
    },
    onBtnCloseClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("viewDocBankSumsEdit" + aButton.UO_id).close();
    },

});



// === Функции === === ===
//1. Скрыть под-форму (сумма, примечание, от кого )
function controllerDocBankSumsEdit_container_Hide(id) {
    Ext.getCmp("form_" + id).setHeight(100);
    Ext.getCmp("viewDocBankSumsEdit" + id).setHeight(400);

    Ext.getCmp("grid_" + id).setVisible(true);
    Ext.getCmp("containerFooterX" + id).setVisible(false);
    Ext.getCmp("containerFooterY" + id).setVisible(false);
    Ext.getCmp("DocBankSumSum" + id).setValue("0.00");
    Ext.getCmp("btnSave" + id).setVisible(false);
    Ext.getCmp("btnCancel" + id).setVisible(false);
};
function controllerDocBankSumsEdit_container_Show(id, DocBankSumSum_label, DirBankSumTypeID) {
    Ext.getCmp("form_" + id).setHeight(300);
    Ext.getCmp("viewDocBankSumsEdit" + id).setHeight(300);

    Ext.getCmp("grid_" + id).setVisible(false);
    Ext.getCmp("containerFooterX" + id).setVisible(true);
    Ext.getCmp("containerFooterY" + id).setVisible(true);
    Ext.getCmp("DocBankSumSum" + id).labelEl.update(DocBankSumSum_label); //"Вид операции - Внесение"
    Ext.getCmp("DirBankSumTypeID" + id).setValue(DirBankSumTypeID); //1
    Ext.getCmp("btnSave" + id).setVisible(true);
    Ext.getCmp("btnCancel" + id).setVisible(true);

    Ext.getCmp("DocBankSumSum" + id).setValue("0.00");
    Ext.getCmp("DocBankSumSum" + id).setReadOnly(false);
};