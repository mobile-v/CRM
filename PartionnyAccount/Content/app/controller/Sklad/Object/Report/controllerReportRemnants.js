﻿Ext.define("PartionnyAccount.controller.Sklad/Object/Report/controllerReportRemnants", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewReportRemnants': { close: this.this_close },

            'viewReportRemnants button#btnDirWarehousesClear': { "click": this.onBtnDirWarehousesClear },

            'viewReportRemnants button#btnDirEmployeesClear': { "click": this.onBtnDirEmployeesClear },

            'viewReportRemnants [itemId=OperationalBalances]': { change: this.onOperationalBalancesChecked },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewReportRemnants menuitem#btnPrintRu': { "click": this.onBtnPrintClick },
            'viewReportRemnants menuitem#btnPrintUa': { "click": this.onBtnPrintClick },
            'viewReportRemnants button#btnCancel': { "click": this.onBtnCancelClick },
            'viewReportRemnants button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },

    onBtnDirWarehousesClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirWarehouseID" + aButton.UO_id).setValue("");
    },

    onBtnDirEmployeesClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirEmployeeID" + aButton.UO_id).setValue("");
    },

    onOperationalBalancesChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("DateS" + ctl.UO_id).disable();
            Ext.getCmp("DatePo" + ctl.UO_id).disable();
            Ext.getCmp("DirEmployeeID" + ctl.UO_id).disable();
        }
        else {
            Ext.getCmp("DateS" + ctl.UO_id).enable();
            Ext.getCmp("DatePo" + ctl.UO_id).enable();
            Ext.getCmp("DirEmployeeID" + ctl.UO_id).enable();
        }
    },



    // Кнопки === === === === === === === === === === ===

    onBtnPrintClick: function (aButton, aEvent, aOptions) {

        //window.open("../report/reportpf/")

        var mapForm = document.createElement("form");
        mapForm.target = "Map";
        mapForm.method = "GET"; // or "post" if appropriate
        mapForm.action = "../report/report/";

        //Параметр "pID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pID"; mapInput.value = "ReportRemnants"; mapForm.appendChild(mapInput);

        //Параметр "pLanguage"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pLanguage"; mapInput.value = aButton.UO_Language; mapForm.appendChild(mapInput);

        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DateS"; mapInput.value = Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);
        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DatePo"; mapInput.value = Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);

        //Параметр "OperationalBalances"
        if (Ext.getCmp("OperationalBalances" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "OperationalBalances"; mapInput.value = Ext.getCmp("OperationalBalances" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirContractorIDOrg"
        if (Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirContractorIDOrg"; mapInput.value = Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirContractorNameOrg"; mapInput.value = Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirWarehouseID"
        if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirWarehouseID"; mapInput.value = Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirWarehouseName"; mapInput.value = Ext.getCmp("DirWarehouseID" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirEmployeeID"
        if (Ext.getCmp("DirEmployeeID" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirEmployeeID"; mapInput.value = Ext.getCmp("DirEmployeeID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirEmployeeName"; mapInput.value = Ext.getCmp("DirEmployeeID" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }
        

        document.body.appendChild(mapForm);
        map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

        if (map) { mapForm.submit(); }
        else { alert('You must allow popups for this map to work.'); }
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        //window.open(HTTP_Help + "report-remnants/", '_blank');
        Ext.Msg.alert(lanOrgName, "Самый быстрый способ сформировать остатки - это установить переключатель Формальные остатки и выбрать организацию (Ваше юр.лицо) и склад. Иначе выбирите период за который будут сформированы остатки.");
    }
});