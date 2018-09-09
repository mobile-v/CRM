Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocCashOfficeSums/controllerDocCashOfficeSumsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocCashOfficeSumsEdit': { close: this.this_close },

            'viewDocCashOfficeSumsEdit button#btnMake': { click: this.onBtnMakeClick },
            'viewDocCashOfficeSumsEdit button#btnPay': { click: this.onBtnPayClick },
            'viewDocCashOfficeSumsEdit button#btnZReport': { click: this.onBtnZReportClick },

            //Отменить выбраного сотрудника
            'viewDocCashOfficeSumsEdit button#btnEmployeeClearDMoney': { "click": this.onBtnEmployeeClearMoneyClick },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocCashOfficeSumsEdit button#btnSave': { click: this.onBtnSaveClick },
            'viewDocCashOfficeSumsEdit button#btnCancel': { click: this.onBtnCancelClick },
            'viewDocCashOfficeSumsEdit button#btnHelp': { click: this.onBtnHelpClick },
            'viewDocCashOfficeSumsEdit button#btnClose': { click: this.onBtnCloseClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Внести Наличку
    onBtnMakeClick: function (aButton, aEvent, aOptions) {
        controllerDocCashOfficeSumsEdit_container_Show(aButton.UO_id, "Вид операции - Внесение", "От кого", 1);
    },
    //Выплатить Наличку
    onBtnPayClick: function (aButton, aEvent, aOptions) {
        controllerDocCashOfficeSumsEdit_container_Show(aButton.UO_id, "Вид операции - Выплата", "Кому", 2);
    },
    //Z-отчет
    onBtnZReportClick: function (aButton, aEvent, aOptions) {
        controllerDocCashOfficeSumsEdit_container_Show(aButton.UO_id, "Вид операции - Z-отчет", "Кому", 3);
    },


    onBtnEmployeeClearMoneyClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirEmployeeIDMoney" + aButton.UO_id).setValue("");
    },


    // === Кнопки: Сохранение, Отмена и Помощь === === ===
    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);
        
        //Сумма должна быть больше нуля!
        if (parseFloat(Ext.getCmp("DocCashOfficeSumSum" + aButton.UO_id).getValue()) <= 0) return;

        //Сохранение - только INSERT
        widgetXForm.submit({
            method: "POST",
            url: HTTP_DocCashOfficeSums,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {

                //1. Обновить форму 
                widgetXForm.load({
                    method: "GET",
                    timeout: varTimeOutDefault,
                    waitMsg: lanLoading,
                    url: HTTP_DirCashOffices + Ext.getCmp("DirCashOfficeID" + aButton.UO_id).getValue() + "/", // + "/?DocID=" + IdcallModelData.DocID,
                    success: function (form, action) {
                    },
                    failure: function (form, action) {
                        funPanelSubmitFailure(form, action);
                    }
                });

                //2. Убрать поля
                controllerDocCashOfficeSumsEdit_container_Hide(aButton.UO_id);

                //3. Обновить грид
                if (Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        controllerDocCashOfficeSumsEdit_container_Hide(aButton.UO_id);
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-cash-office-sums/", '_blank');
    },
    onBtnCloseClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("viewDocCashOfficeSumsEdit" + aButton.UO_id).close();
    },

});



// === Функции === === ===
//1. Скрыть под-форму (сумма, примечание, от кого )
function controllerDocCashOfficeSumsEdit_container_Hide(id) {
    Ext.getCmp("form_" + id).setHeight(100);
    Ext.getCmp("viewDocCashOfficeSumsEdit" + id).setHeight(400);

    Ext.getCmp("grid_" + id).setVisible(true);
    Ext.getCmp("containerFooterX" + id).setVisible(false);
    Ext.getCmp("containerFooterY" + id).setVisible(false);
    Ext.getCmp("containerFooterZ" + id).setVisible(false);
    Ext.getCmp("DocCashOfficeSumSum" + id).setValue("0.00");
    Ext.getCmp("btnSave" + id).setVisible(false);
    Ext.getCmp("btnCancel" + id).setVisible(false);
};
function controllerDocCashOfficeSumsEdit_container_Show(id, DocCashOfficeSumSum_label, DirEmployeeID_label, DirCashOfficeSumTypeID) {
    Ext.getCmp("form_" + id).setHeight(300);
    Ext.getCmp("viewDocCashOfficeSumsEdit" + id).setHeight(300);

    Ext.getCmp("grid_" + id).setVisible(false);
    Ext.getCmp("containerFooterX" + id).setVisible(true);
    Ext.getCmp("containerFooterY" + id).setVisible(true);
    Ext.getCmp("containerFooterZ" + id).setVisible(true);
    Ext.getCmp("DocCashOfficeSumSum" + id).labelEl.update(DocCashOfficeSumSum_label); //"Вид операции - Внесение"
    Ext.getCmp("DirEmployeeIDMoney" + id).labelEl.update(DirEmployeeID_label); //"От кого"
    Ext.getCmp("DirCashOfficeSumTypeID" + id).setValue(DirCashOfficeSumTypeID); //1
    Ext.getCmp("btnSave" + id).setVisible(true);
    Ext.getCmp("btnCancel" + id).setVisible(true);

    //Если Z-отчет
    if (DirCashOfficeSumTypeID == 3) {
        Ext.getCmp("DocCashOfficeSumSum" + id).setValue(Ext.getCmp("DirCashOfficeSum" + id).getValue());
        Ext.getCmp("DocCashOfficeSumSum" + id).setReadOnly(true);
    }
    else {
        Ext.getCmp("DocCashOfficeSumSum" + id).setValue("0.00");
        Ext.getCmp("DocCashOfficeSumSum" + id).setReadOnly(false);
    }
};

