Ext.define("PartionnyAccount.controller.Sklad/Object/Report/controllerDocCashOfficeReport", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocCashOfficeReport': { close: this.this_close },


            //Касса
            'viewDocCashOfficeReport button#btnCashOfficeClear': { "click": this.onBtnCashOfficeClearClick },

            //Операция
            'viewDocCashOfficeReport button#btnCashOfficeSumTypeClear': { "click": this.onBtnCashOfficeSumTypeClearClick },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocCashOfficeReport menuitem#btnPrintRu': { "click": this.onBtnPrintClick },
            'viewDocCashOfficeReport menuitem#btnPrintUa': { "click": this.onBtnPrintClick },
            'viewDocCashOfficeReport button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocCashOfficeReport button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    // *** Касса ***
    onBtnCashOfficeClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCashOfficeID" + aButton.UO_id).setValue("");
    },


    // *** Операция ***
    onBtnCashOfficeSumTypeClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCashOfficeSumTypeID" + aButton.UO_id).setValue("");
    },


    // Кнопки === === === === === === === === === === ===

    onBtnPrintClick: function (aButton, aEvent, aOptions) {

        //window.open("../report/reportpf/")

        var mapForm = document.createElement("form");
        mapForm.target = "Map";
        mapForm.method = "GET"; // or "post" if appropriate
        mapForm.action = "../report/report/";
        /*
            "?pID=DocCashOfficeReport" + 
            "&pLanguage=" + aButton.UO_Language + 
            "&DateS=" + Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d') + 
            "&DatePo=" + Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d') + 
            "&DirCashOfficeID=" + Ext.getCmp("DirCashOfficeID" + aButton.UO_id).getValue() + 
            "&DirCashOfficeSumTypeID=" + Ext.getCmp("DirCashOfficeSumTypeID" + aButton.UO_id).getValue();
        */
        

        //Параметр "pID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pID"; mapInput.value = "DocCashOfficeReport"; mapForm.appendChild(mapInput);

        //Параметр "pLanguage"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pLanguage"; mapInput.value = aButton.UO_Language; mapForm.appendChild(mapInput);

        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DateS"; mapInput.value = Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);
        //Параметр "DateS"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DatePo"; mapInput.value = Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d'); mapForm.appendChild(mapInput);

        //Параметр "DirCashOfficeID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DirCashOfficeID"; mapInput.value = Ext.getCmp("DirCashOfficeID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "DirCashOfficeSumTypeID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DirCashOfficeSumTypeID"; mapInput.value = Ext.getCmp("DirCashOfficeSumTypeID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);
        

        document.body.appendChild(mapForm);
        map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

        if (map) { mapForm.submit(); }
        else { alert('You must allow popups for this map to work.'); }
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "Warehouses/", '_blank');
    }
});