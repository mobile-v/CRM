Ext.define("PartionnyAccount.controller.Sklad/Object/Report/controllerReportMovementNomen", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewReportMovementNomen': { close: this.this_close },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewReportMovementNomen menuitem#btnPrintRu': { "click": this.onBtnPrintClick },
            'viewReportMovementNomen menuitem#btnPrintUa': { "click": this.onBtnPrintClick },
            'viewReportMovementNomen button#btnCancel': { "click": this.onBtnCancelClick },
            'viewReportMovementNomen button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
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
        mapInput.name = "pID"; mapInput.value = "ReportMovementNomen"; mapForm.appendChild(mapInput);

        //Параметр "pLanguage"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pLanguage"; mapInput.value = aButton.UO_Language; mapForm.appendChild(mapInput);


        //Параметр "DirContractorIDOrg"
        if (Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirContractorIDOrg"; mapInput.value = Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirContractorNameOrg"; mapInput.value = Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirWarehouseIDFrom"
        if (Ext.getCmp("DirWarehouseIDFrom" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirWarehouseIDFrom"; mapInput.value = Ext.getCmp("DirWarehouseIDFrom" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirWarehouseNameFrom"; mapInput.value = Ext.getCmp("DirWarehouseIDFrom" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirWarehouseIDTo"
        if (Ext.getCmp("DirWarehouseIDTo" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirWarehouseIDTo"; mapInput.value = Ext.getCmp("DirWarehouseIDTo" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirWarehouseNameTo"; mapInput.value = Ext.getCmp("DirWarehouseIDTo" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
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
        Ext.Msg.alert(lanOrgName, "!!!");
    }
});