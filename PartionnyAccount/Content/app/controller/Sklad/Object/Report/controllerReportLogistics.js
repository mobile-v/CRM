Ext.define("PartionnyAccount.controller.Sklad/Object/Report/controllerReportLogistics", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewReportLogistics': { close: this.this_close },

            'viewReportLogistics button#btnDirWarehousesClear': { "click": this.onBtnDirWarehousesClear },
            'viewReportLogistics button#btnDirEmployeesClear': { "click": this.onBtnDirEmployeesClear },
            'viewReportLogistics button#btnDirPriceTypeClear': { "click": this.onBtnDirPriceTypeClear },

            'viewReportLogistics [itemId=ReportType]': { select: this.onReportTypeSelect },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewReportLogistics menuitem#btnPrintRu': { "click": this.onBtnPrintClick },
            'viewReportLogistics menuitem#btnPrintUa': { "click": this.onBtnPrintClick },
            'viewReportLogistics button#btnCancel': { "click": this.onBtnCancelClick },

            'viewReportLogistics button#btnDocMovementsEdit': { "click": this.onBtnDocMovementsEditClick },
            'viewReportLogistics button#btnReport': { "click": this.onBtnReportClick },
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


    //Тип отчеа, если "Брак", отобразить кнопку "Сформировать перемещение"
    onReportTypeSelect: function (combo, records, eOpts) {
        if (records.data.ReportType == 8) {
            Ext.getCmp("btnDocMovementsEdit" + combo.UO_id).setVisible(true);
        }
        else {
            Ext.getCmp("btnDocMovementsEdit" + combo.UO_id).setVisible(false);
        }

        Ext.getCmp("grid_" + combo.UO_id).store.setData([], false);
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
        mapInput.name = "pID"; mapInput.value = "ReportLogistics"; mapForm.appendChild(mapInput);

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
        
        //Параметр "DirMovementStatusID"
        if (Ext.getCmp("DirMovementStatusID" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirMovementStatusID"; mapInput.value = Ext.getCmp("DirMovementStatusID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirMovementStatusName"; mapInput.value = Ext.getCmp("DirMovementStatusID" + aButton.UO_id).getRawValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DocOrTab" (Отобразить документы или табличную часть)
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DocOrTab"; mapInput.value = Ext.getCmp("DocOrTab" + aButton.UO_id).getValue().DocOrTab; mapForm.appendChild(mapInput);


        document.body.appendChild(mapForm);
        map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

        if (map) { mapForm.submit(); }
        else { alert('You must allow popups for this map to work.'); }
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },

    onBtnDocMovementsEditClick: function (aButton, aEvent, aOptions) {
        var id = aButton.UO_id;

        if (Ext.getCmp("grid_" + id).store.data.length == 0) { Ext.Msg.alert(lanOrgName, "Нет данных для формирования перемещения!"); return; }

        Ext.MessageBox.show({
            title: lanOrgName, msg: "Создать документ на перемещение товара?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false, //'Удалить '
            fn: function (buttons) {
                if (buttons == "yes") {

                    //1. Данные формы
                    var modelDocMovementsGrid = Ext.create('PartionnyAccount.model.Sklad/Object/Doc/DocMovements/modelDocMovementsGrid');
                    modelDocMovementsGrid.data = Ext.getCmp("form_" + id).getForm().getFieldValues(); //form.loadRecord(formRec); //Вот так надо загружать данные в Форму
                    //2. Данные Грида (эти данные загружаются в модель или в сторе)
                    var gridRec = Ext.getCmp("grid_" + id).getStore(); //.getSelectionModel(); //.getSelection(); //store.insert(widgetXForm.UO_GridIndex, gridRec); //Вот так надо загружать данные в Грид
                    //3. Параметр содержащий данные Формы и Грида
                    var ArrList = [modelDocMovementsGrid, gridRec, Ext.getCmp("DirWarehouseID" + id).getValue()];
                    //4. Создание Р.Н.
                    var Params = [
                        "grid_" + id, //UO_idCall (в "ObjectEditConfig" UO_idCall = undefined)
                        false, //UO_Center
                        false, //UO_Modal
                        3,     // 1 - Новое, 2 - Редактировать, 3 - Копия
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        ArrList
                    ]
                    ObjectEditConfig("viewDocMovementsEdit", Params);

                }
            }
        });
    },
    onBtnReportClick: function (aButton, aEvent, aOptions) {

        //1. Грид
        var grid = Ext.getCmp("grid_" + aButton.UO_id);



        //2. Создаём коллекции полей для каждого отчета, которые надо - показать, все остальные - спрятать.

        //Проданный товар (Опт + Розница)
        var DocOrTab_1 = ["DocMovementID", "DocDate", "DirWarehouseNameFrom", "DirEmployeeNameCourier", "DirWarehouseNameTo", "DirMovementStatusName"]
        //Приходы
        var DocOrTab_2 = ["DocMovementID", "DocDate", , "DirWarehouseNameFrom", "DirEmployeeNameCourier", "DirWarehouseNameTo", "DirMovementStatusName", "DirNomenID", "DirNomenPatchFull", "DirNomenName", "Sale_Quantity", "DirChar"]


        //Показываем нужные поля
        var DocOrTab_X;
        var DocOrTab = parseInt(Ext.getCmp("DocOrTab" + aButton.UO_id).getValue().DocOrTab);
        switch (DocOrTab) {
            case 1: DocOrTab_X = DocOrTab_1; break;
            case 2: DocOrTab_X = DocOrTab_2; break;
        }


        //Спрятали поле "DirPriceTypeName"
        var gridColumns = grid.columns;
        for (var i = 0; i < gridColumns.length; i++) {
            //Колонка
            var gridColumn = gridColumns[i];

            //Прячем все поля
            gridColumn.hide();

            for (var j = 0; j < DocOrTab_X.length; j++) {
                if (gridColumn.dataIndex == DocOrTab_X[j]) {
                    gridColumn.show();
                }
            }
        }



        //3. Запрос на сервер
        var storeReportLogistics = Ext.getCmp("grid_" + aButton.UO_id).getStore();
        storeReportLogistics.proxy.url = HTTP_ReportLogistics +
                "?pLanguage=" + aButton.UO_Language + "&" +
                "DateS=" + Ext.Date.format(Ext.getCmp("DateS" + aButton.UO_id).getValue(), 'Y-m-d') + "&" + 
                "DatePo=" + Ext.Date.format(Ext.getCmp("DatePo" + aButton.UO_id).getValue(), 'Y-m-d') + "&" +
                "DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue() + "&" +
                "DirMovementStatusID=" + Ext.getCmp("DirMovementStatusID" + aButton.UO_id).getValue() + "&" +
                //"DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() + "&" +
                "DirEmployeeID=" + Ext.getCmp("DirEmployeeID" + aButton.UO_id).getValue() + "&" +
                "DocOrTab=" + DocOrTab,
        storeReportLogistics.load({ waitMsg: lanLoading });
        storeReportLogistics.on('load', function () {
            
        });

    },

});