Ext.define("PartionnyAccount.controller.Sklad/Object/Service/ExchangeData/controllerImportsDocPurchesExcel", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewImportsDocPurchesExcel': { close: this.this_close },

            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewImportsDocPurchesExcel button#btnSave': { "click": this.onBtnSaveClick },
            'viewImportsDocPurchesExcel button#btnCancel': { "click": this.onBtnCancelClick },
            'viewImportsDocPurchesExcel button#btnHelp': { "click": this.onBtnHelpClick }
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl =
            HTTP_ImportsDocPurchesExcel +
            "?sheetName=" + Ext.getCmp("sheetName" + aButton.UO_id).getValue() +
            "&DirContractorID=" + Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() +
            "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue();

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                Ext.getCmp(aButton.UO_idMain).close();
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "import-prikhodnoy-nakladnoy/", '_blank');
    }

});