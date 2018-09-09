Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirServiceJobNomens/controllerDirServiceDiagnosticRresultsWin", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirServiceDiagnosticRresultsWin': { close: this.this_close },


            'viewDirServiceDiagnosticRresultsWin [itemId=DirServiceDiagnosticRresultName]': { select: this.onDirServiceDiagnosticRresultNameSelect },
            'viewDirServiceDiagnosticRresultsWin [itemId=PriceRetailVAT]': { specialkey: this.onPriceRetailVATSpecialkey },
            'viewDirServiceDiagnosticRresultsWin [itemId=DirServiceDiagnosticRresultName]': { specialkey: this.onDirServiceDiagnosticRresultNameSpecialkey },
            'viewDirServiceDiagnosticRresultsWin button#btnDirServiceDiagnosticRresultNameAdd': { click: this.onBtnDirServiceDiagnosticRresultNameAddClick },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirServiceDiagnosticRresultsWin button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirServiceDiagnosticRresultsWin button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirServiceDiagnosticRresultsWin button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    onDirServiceDiagnosticRresultNameSelect: function (combo, records) {
        /*
        if (!Ext.getCmp("PriceRetailVAT" + combo.UO_id).getValue()) { Ext.Msg.alert(lanOrgName, "Укажите пожалуйста цену и нажмите Ентер!"); return; return; }

        if (Ext.getCmp("DiagnosticRresultTxt" + combo.UO_id).getValue().length > 0) {
            Ext.getCmp("DiagnosticRresultTxt" + combo.UO_id).setValue
            (
                Ext.getCmp("DiagnosticRresultTxt" + combo.UO_id).getValue() + ", "
            );
        }

        Ext.getCmp("DiagnosticRresultTxt" + combo.UO_id).setValue
        (
            Ext.getCmp("DiagnosticRresultTxt" + combo.UO_id).getValue() + " " + records.data.DirServiceDiagnosticRresultName + " - " + Ext.getCmp("PriceRetailVAT" + combo.UO_id).getValue()
        );
        */

        controllerDirServiceDiagnosticRresultsWin_DiagnosticRresultTxt(combo.UO_id, records.data.DirServiceDiagnosticRresultName);
    },
    onPriceRetailVATSpecialkey: function (f, e) {
        if (e.getKey() == e.ENTER) {
            controllerDirServiceDiagnosticRresultsWin_DiagnosticRresultTxt(f.UO_id, Ext.getCmp("DirServiceDiagnosticRresultName" + f.UO_id).getValue());
        }
    },
    onDirServiceDiagnosticRresultNameSpecialkey: function (f, e) {
        if (e.getKey() == e.ENTER) {
            controllerDirServiceDiagnosticRresultsWin_DiagnosticRresultTxt(f.UO_id, Ext.getCmp("DirServiceDiagnosticRresultName" + f.UO_id).getValue());
        }
    },
    onBtnDirServiceDiagnosticRresultNameAddClick: function (aButton, aEvent, aOptions) {
        controllerDirServiceDiagnosticRresultsWin_DiagnosticRresultTxt(aButton.UO_id, Ext.getCmp("DirServiceDiagnosticRresultName" + aButton.UO_id).getValue());
    },



    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //1. Если поле "DiagnosticRresultTxt" пустое, а поля "PriceRetailVAT" и "DirServiceDiagnosticRresultName" заполнены, то пишем их в поле "DiagnosticRresultTxt"
        if (
            Ext.getCmp("DiagnosticRresultTxt" + aButton.UO_id).getValue().length == 0 &&
            (Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue().length > 0 && Ext.getCmp("DirServiceDiagnosticRresultName" + aButton.UO_id).getValue() == null) ||
            (Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue().length == 0 && Ext.getCmp("DirServiceDiagnosticRresultName" + aButton.UO_id).getValue() != null)
           ) {
            Ext.Msg.alert(lanOrgName, "Укажите цену или результат диагностики!"); return;
        }


        //1. Если поле "DiagnosticRresultTxt" пустое, а поля "PriceRetailVAT" и "DirServiceDiagnosticRresultName" заполнены, то пишем их в поле "DiagnosticRresultTxt"
        if (
            Ext.getCmp("DiagnosticRresultTxt" + aButton.UO_id).getValue().length == 0 &&
            Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue().length > 0 &&
            Ext.getCmp("DirServiceDiagnosticRresultName" + aButton.UO_id).getValue() != null
           ) {
            controllerDirServiceDiagnosticRresultsWin_DiagnosticRresultTxt(aButton.UO_id, Ext.getCmp("DirServiceDiagnosticRresultName" + aButton.UO_id).getValue());
        }


        //2. Сохраняем
        var widg = Ext.getCmp("viewDirServiceDiagnosticRresultsWin" + aButton.UO_id);
        widg.UO_Param_fn.data.DiagnosticRresults = Ext.getCmp("DiagnosticRresultTxt" + aButton.UO_id).getValue();

        widg.UO_GridIndex(widg.UO_GridRecord, widg.UO_Param_id, widg.UO_Param_fn, widg.UO_idTab, Ext.getCmp("DiagnosticRresultTxt" + aButton.UO_id).getValue()); //widg.UO_GridServerParam1
        widg.close();
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    /*onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-tovar/", '_blank');
    }*/
});


function controllerDirServiceDiagnosticRresultsWin_DiagnosticRresultTxt(id, DirServiceDiagnosticRresultName) {
    //1. Проверки
    if (!Ext.getCmp("PriceRetailVAT" + id).getValue()) { Ext.getCmp("PriceRetailVAT" + id).focus(); Ext.Msg.alert(lanOrgName, "Укажите пожалуйста цену и нажмите Ентер!"); return; }
    if (!Ext.getCmp("DirServiceDiagnosticRresultName" + id).getValue()) { Ext.getCmp("DirServiceDiagnosticRresultName" + id).focus(); Ext.Msg.alert(lanOrgName, "Укажите пожалуйста Результат и нажмите Ентер!"); return; }
    //2. Запятая
    if (Ext.getCmp("DiagnosticRresultTxt" + id).getValue().length > 0) {
        Ext.getCmp("DiagnosticRresultTxt" + id).setValue
        (
            Ext.getCmp("DiagnosticRresultTxt" + id).getValue() + ", "
        );
    }


    //3. Заполняем поле "DiagnosticRresultTxt"
    var DiagnosticRresultTxt = DiagnosticRresultTxt = Ext.getCmp("DiagnosticRresultTxt" + id).getValue() + " " + DirServiceDiagnosticRresultName + "; Цена:" + Ext.getCmp("PriceRetailVAT" + id).getValue() + "; Дней:" + Ext.getCmp("Quantity" + id).getValue();
    Ext.getCmp("DiagnosticRresultTxt" + id).setValue(DiagnosticRresultTxt);


    //4. Обнуляем
    Ext.getCmp("DirServiceDiagnosticRresultName" + id).setValue(null); Ext.getCmp("PriceRetailVAT" + id).setValue(null);
}