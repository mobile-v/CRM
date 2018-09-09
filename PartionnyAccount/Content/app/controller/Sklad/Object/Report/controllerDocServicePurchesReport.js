Ext.define("PartionnyAccount.controller.Sklad/Object/Report/controllerDocServicePurchesReport", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocServicePurchesReport': { close: this.this_close },

            'viewDocServicePurchesReport button#btnDirWarehousesClear': { "click": this.onBtnDirWarehousesClear },

            'viewDocServicePurchesReport button#btnDirServiceStatusesClear': { "click": this.onBtnbtnDirServiceStatusesClear },

            'viewDocServicePurchesReport button#btnDirEmployeesClear': { "click": this.onBtnDirEmployeesClear },

            'viewDocServicePurchesReport button#btnDirEmployeesMasterClear': { "click": this.onBtnDirEmployeesMasterClear },

            'viewDocServicePurchesReport button#btnDirServiceContractorsClear': { "click": this.onBtnDirServiceContractorsClear },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocServicePurchesReport menuitem#btnPrintRu': { "click": this.onBtnPrintClick },
            'viewDocServicePurchesReport menuitem#btnPrintUa': { "click": this.onBtnPrintClick },
            'viewDocServicePurchesReport button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocServicePurchesReport button#btnHelp': { "click": this.onBtnHelpClick },

            'viewDocServicePurchesReport button#btnReport': { "click": this.onBtnReportClick },
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

    onBtnbtnDirServiceStatusesClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirServiceStatusID" + aButton.UO_id).setValue("");
    },

    onBtnDirEmployeesClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirEmployeeID" + aButton.UO_id).setValue("");
    },

    onBtnDirEmployeesMasterClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirEmployeeIDMaster" + aButton.UO_id).setValue("");
    },

    onBtnDirServiceContractorsClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirServiceContractorID" + aButton.UO_id).setValue("");
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
        mapInput.name = "pID"; mapInput.value = "DocServicePurchesReport"; mapForm.appendChild(mapInput);

        //Параметр "pLanguage"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pLanguage"; mapInput.value = aButton.UO_Language; mapForm.appendChild(mapInput);

        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DateS"; mapInput.value = Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);
        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DatePo"; mapInput.value = Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);

        //Параметр "TypeRepair"
        if (Ext.getCmp("TypeRepair" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "TypeRepair"; mapInput.value = Ext.getCmp("TypeRepair" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);
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

        //Параметр "DirServiceStatusID"
        if (Ext.getCmp("DirServiceStatusID" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirServiceStatusID"; mapInput.value = Ext.getCmp("DirServiceStatusID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirServiceStatusName"; mapInput.value = Ext.getCmp("DirServiceStatusID" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirEmployeeID"
        if (Ext.getCmp("DirEmployeeID" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirEmployeeID"; mapInput.value = Ext.getCmp("DirEmployeeID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirEmployeeName"; mapInput.value = Ext.getCmp("DirEmployeeID" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirEmployeeID"
        if (Ext.getCmp("DirEmployeeIDMaster" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirEmployeeIDMaster"; mapInput.value = Ext.getCmp("DirEmployeeIDMaster" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirEmployeeNameMaster"; mapInput.value = Ext.getCmp("DirEmployeeIDMaster" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirServiceContractorID"
        if (Ext.getCmp("DirServiceContractorID" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirServiceContractorID"; mapInput.value = Ext.getCmp("DirServiceContractorID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirServiceContractorName"; mapInput.value = Ext.getCmp("DirServiceContractorID" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "ReportType"
        if (Ext.getCmp("ReportType" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "ReportType"; mapInput.value = Ext.getCmp("ReportType" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "ReportTyperName"; mapInput.value = Ext.getCmp("ReportType" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
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
        //window.open(HTTP_Help + "report-service-purches/", '_blank');
        Ext.Msg.alert(lanOrgName, "Для формирования отчета достаточно выбрать период. Но можно сформировать отчет выбрав дополнительные свойства.");
    },

    onBtnReportClick: function (aButton, aEvent, aOptions) {
        
        //1. Грид
        var grid = Ext.getCmp("grid_" + aButton.UO_id);

        //2. Запрос на сервер
        var storeDocServicePurchesReport = Ext.getCmp("grid_" + aButton.UO_id).getStore();
        storeDocServicePurchesReport.proxy.url = HTTP_DocServicePurchesReport +
                "?pLanguage=" + 1 + "&" + //aButton.UO_Language
                "DateS=" + Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d') + "&" +
                "DatePo=" + Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d') + "&" +
                "TypeRepair=" + Ext.getCmp("TypeRepair" + aButton.UO_id).getValue() + "&" +
                "DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue() + "&" +
                "DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() + "&" +
                "DirServiceStatusID=" + Ext.getCmp("DirServiceStatusID" + aButton.UO_id).getValue() + "&" +
                "DirEmployeeID=" + 0 + "&" + //Ext.getCmp("DirEmployeeID" + aButton.UO_id).getValue()
                "DirEmployeeIDMaster=" + Ext.getCmp("DirEmployeeIDMaster" + aButton.UO_id).getValue() + "&" +
                "DirServiceContractorID=" + 0 + "&" + 
                "ReportType=" + Ext.getCmp("ReportType" + aButton.UO_id).getValue() + "&"; //Ext.getCmp("DirServiceContractorID" + aButton.UO_id).getValue()
        storeDocServicePurchesReport.load({ waitMsg: lanLoading });
        storeDocServicePurchesReport.on('load', function () {
            //Считаем к-во поле: Приёмка, Готов/Отказ, Выдача
            //Если есть дата - "+1", нет - "+0"
            var Count_DocDate = 0, Count_IssuanceDate = 0, Count_DateStatusChange = 0;
            for (var i = 0; i < storeDocServicePurchesReport.data.items.length; i++) {
                var item = storeDocServicePurchesReport.data.items[i].data;
                if (item.DocDate) { Count_DocDate++; }
                if (item.IssuanceDate) { Count_IssuanceDate++; }
                if (item.DateStatusChange) { Count_DateStatusChange++; }
            }
            Ext.getCmp("labelCount_DocDate" + aButton.UO_id).setText("Приёмка: " + Count_DocDate); Ext.getCmp("DocDate" + aButton.UO_id).setText("Приёмка (<b style='color:red;'>" + Count_DocDate + "</b>)");
            Ext.getCmp("labelCount_IssuanceDate" + aButton.UO_id).setText("Готов/Отказ: " + Count_IssuanceDate); Ext.getCmp("IssuanceDate" + aButton.UO_id).setText("Готов/Отказ (<b style='color:red;'>" + Count_IssuanceDate + "</b>)");
            Ext.getCmp("labelCount_DateStatusChange" + aButton.UO_id).setText("Выдача: " + Count_DateStatusChange); Ext.getCmp("DateStatusChange" + aButton.UO_id).setText("Выдача (<b style='color:red;'>" + Count_DateStatusChange + "</b>)");


        });


    }
});