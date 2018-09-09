Ext.define("PartionnyAccount.controller.Sklad/Object/Report/controllerReportBanksCashOffices", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewReportBanksCashOffices': { close: this.this_close },

            'viewReportBanksCashOffices button#btnDirWarehousesClear': { "click": this.onBtnDirWarehousesClear },
            'viewReportBanksCashOffices button#btnDirEmployeesClear': { "click": this.onBtnDirEmployeesClear },

            'viewReportBanksCashOffices #ReportType': { select: this.onReportType_Select },
            'viewReportBanksCashOffices #ReportGroup': { select: this.onReportGroup_Select },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewReportBanksCashOffices menuitem#btnPrintRu': { "click": this.onBtnPrintClick },
            'viewReportBanksCashOffices menuitem#btnPrintUa': { "click": this.onBtnPrintClick },
            'viewReportBanksCashOffices button#btnCancel': { "click": this.onBtnCancelClick },
            'viewReportBanksCashOffices button#btnReport': { "click": this.onBtnReportClick },
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

    onReportType_Select: function (combo, records, eOpts) {
        if (records.data.ReportType != 1) {
            Ext.getCmp("ReportGroup" + combo.UO_id).setValue(1);
            Ext.getCmp("ReportGroup" + combo.UO_id).setReadOnly(true);
        }
        else {
            Ext.getCmp("ReportGroup" + combo.UO_id).setReadOnly(false);
        }
    },
    onReportGroup_Select: function (combo, records, eOpts) {
        if (records.data.ReportGroup == 2) {
            //...
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
        mapInput.name = "pID"; mapInput.value = "ReportBanksCashOffices"; mapForm.appendChild(mapInput);

        //Параметр "pLanguage"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pLanguage"; mapInput.value = aButton.UO_Language; mapForm.appendChild(mapInput);

        //Параметр "CasheAndBank"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "CasheAndBank"; mapInput.value = Ext.getCmp("CasheAndBank" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);
        //Параметр "Cashe"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "Cashe"; mapInput.value = Ext.getCmp("Cashe" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);
        //Параметр "Bank"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "Bank"; mapInput.value = Ext.getCmp("Bank" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DateS"; mapInput.value = Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);
        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DatePo"; mapInput.value = Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);


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
        
        //Параметр "ReportType"
        if (Ext.getCmp("ReportType" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "ReportType"; mapInput.value = Ext.getCmp("ReportType" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "ReportTypeName"; mapInput.value = Ext.getCmp("ReportType" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "ReportGroup"
        if (Ext.getCmp("ReportGroup" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "ReportGroup"; mapInput.value = Ext.getCmp("ReportGroup" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "ReportGroupName"; mapInput.value = Ext.getCmp("ReportGroup" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DocXID"; mapInput.value = Ext.getCmp("DocXID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);


        document.body.appendChild(mapForm);
        map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

        if (map) { mapForm.submit(); }
        else { alert('You must allow popups for this map to work.'); }
    },

    onBtnReportClick: function (aButton, aEvent, aOptions) {

        //1. Грид
        var grid = Ext.getCmp("grid_" + aButton.UO_id);



        //2. Создаём коллекции полей для каждого отчета, которые надо - показать, все остальные - спрятать.

        /*
        //Банк
        var ReportType_Bank = ["DocBankSumDate", "DirBankSumTypeName", "DocBankSumSum", "DirEmployeeName", "Base", "Description", "DirBankName", "DirWarehouseName", "Discount"];
        //Касса
        var ReportType_Cash = ["DocCashOfficeSumDate", "DirCashOfficeSumTypeName", "DocCashOfficeSumSum", "DirEmployeeName", "Base", "Description", "DirCashOfficeName", "DirWarehouseName", "Discount"];


        //Показываем нужные поля
        var ReportType_X;
        if (Ext.getCmp("Bank" + aButton.UO_id).getValue()) ReportType_X = ReportType_Bank;
        else ReportType_X = ReportType_Cash;


        //Спрятали поле "DirPriceTypeName"
        var gridColumns = grid.columns;
        for (var i = 0; i < gridColumns.length; i++) {
            //Колонка
            var gridColumn = gridColumns[i];

            //Прячем все поля
            gridColumn.hide();

            for (var j = 0; j < ReportType_X.length; j++) {
                if (gridColumn.dataIndex == ReportType_X[j]) {
                    gridColumn.show();
                }
            }
        }*/



        //3. Запрос на сервер
        var storeReportBanksCashOffices = Ext.getCmp("grid_" + aButton.UO_id).getStore();
        storeReportBanksCashOffices.proxy.url =
            HTTP_ReportBanksCashOffices +
            "?pLanguage=" + aButton.UO_Language + "&" +
            "DateS=" + Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d') + "&" +
            "DatePo=" + Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d') + "&" +
            "DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue() + "&" +
            "DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() + "&" +
            "DirEmployeeID=" + Ext.getCmp("DirEmployeeID" + aButton.UO_id).getValue() + "&" +
            "CasheAndBank=" + Ext.getCmp("CasheAndBank" + aButton.UO_id).getValue() + "&" +
            "Cashe=" + Ext.getCmp("Cashe" + aButton.UO_id).getValue() + "&" +
            "Bank=" + Ext.getCmp("Bank" + aButton.UO_id).getValue() + "&" +
            "ReportType=" + Ext.getCmp("ReportType" + aButton.UO_id).getValue() + "&" +
            "ReportGroup=" + Ext.getCmp("ReportGroup" + aButton.UO_id).getValue() + "&" +
            "DocXID=" + Ext.getCmp("DocXID" + aButton.UO_id).getValue();
        
        storeReportBanksCashOffices.load({ waitMsg: lanLoading });
        storeReportBanksCashOffices.on('load', function () {

            Ext.getCmp("Quantity" + aButton.UO_id).setValue(storeReportBanksCashOffices.data.length);

        });

    }

});

