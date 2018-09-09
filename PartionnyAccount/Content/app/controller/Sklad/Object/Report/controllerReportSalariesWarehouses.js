Ext.define("PartionnyAccount.controller.Sklad/Object/Report/controllerReportSalariesWarehouses", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewReportSalariesWarehouses': { close: this.this_close },

            'viewReportSalariesWarehouses button#btnDirWarehousesClear': { "click": this.onBtnDirWarehousesClear },
            'viewReportSalariesWarehouses button#btnDirEmployeesClear': { "click": this.onBtnDirEmployeesClear },
            'viewReportSalariesWarehouses button#btnDirPriceTypeClear': { "click": this.onBtnDirPriceTypeClear },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewReportSalariesWarehouses menuitem#btnPrintRu': { "click": this.onBtnPrintClick },
            'viewReportSalariesWarehouses menuitem#btnPrintUa': { "click": this.onBtnPrintClick },
            'viewReportSalariesWarehouses button#btnCancel': { "click": this.onBtnCancelClick },

            'viewReportSalariesWarehouses button#btnReport': { "click": this.onBtnReportClick },
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
    onBtnDirPriceTypeClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirPriceTypeID" + aButton.UO_id).setValue("");
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
        mapInput.name = "pID"; mapInput.value = "DocSalaries"; mapForm.appendChild(mapInput);

        //Параметр "pLanguage"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pLanguage"; mapInput.value = aButton.UO_Language; mapForm.appendChild(mapInput);

        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DateS"; mapInput.value = Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);
        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DatePo"; mapInput.value = Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);


        //Параметр "DirContractorIDOrg"
        if (Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirContractorIDOrg"; mapInput.value = Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirContractorNameOrg"; mapInput.value = Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
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

    onBtnReportClick: function (aButton, aEvent, aOptions) {

        //0. Проверки
        if (isNaN(parseInt(Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue()))) {
            Ext.Msg.alert(lanOrgName, "Выбирите точку!");
            return;
        }

        //1. Грид
        var grid = Ext.getCmp("grid_" + aButton.UO_id);

        //3. Запрос на сервер
        var storeReportSalariesWarehouses = Ext.getCmp("grid_" + aButton.UO_id).getStore();
        storeReportSalariesWarehouses.proxy.url = HTTP_ReportSalariesWarehouses +
                "?pLanguage=" + aButton.UO_Language + "&" +
                "DateS=" + Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d') + "&" +
                "DatePo=" + Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d') + "&" +
                "DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue() + "&" +
                "DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue();
        storeReportSalariesWarehouses.load({ waitMsg: lanLoading });
        storeReportSalariesWarehouses.on('load', function () {
            //alert("!!!");
        });



        /*
        if (varDirEmployeeID != 1) {
            var col = grid.columns;
            col[0].setVisible(false);
            col[1].setVisible(false);
            col[2].setVisible(false);
            col[3].setVisible(false);
            col[4].setVisible(false);
            col[5].setVisible(false);
            col[6].setVisible(false);
            col[7].setVisible(false);
            col[8].setVisible(false);
            col[9].setVisible(false);
            col[10].setVisible(false);
            col[11].setVisible(false);
            col[12].setVisible(false);
            col[13].setVisible(false);
        }
        */


    },

});