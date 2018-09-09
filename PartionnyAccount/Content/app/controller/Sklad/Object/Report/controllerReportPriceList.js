Ext.define("PartionnyAccount.controller.Sklad/Object/Report/controllerReportPriceList", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewReportPriceList': { close: this.this_close },


            // Клик по Группе "DirNomenName"
            'viewReportPriceList [itemId=DirNomenName]': {
                selectionchange: this.onCombotree_DirNomenName_selectionchange,
                itemclick: this.onCombotree_DirNomenName_itemclick,
                itemdblclick: this.onCombotree_DirNomenName_itemdblclick,

                //itemcontextmenu: this.onTree_contextMenuForTreePanel,
            },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewReportPriceList menuitem#btnPrintRu': { "click": this.onBtnPrintClick },
            'viewReportPriceList menuitem#btnPrintUa': { "click": this.onBtnPrintClick },
            'viewReportPriceList button#btnCancel': { "click": this.onBtnCancelClick },
            'viewReportPriceList button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    // *** DirNomenName ***
    // Селект
    onCombotree_DirNomenName_selectionchange: function (model, records) {
        //alert("111")
    },
    // Клик по Группе
    onCombotree_DirNomenName_itemclick: function (view, rec, item, index, eventObj) {
        //alert("222")
        Ext.getCmp("DirNomenID" + view.grid.UO_id).setValue(rec.get('id'));
        Ext.getCmp("DirNomenName" + view.grid.UO_id).setValue(rec.get('text'));
    },
    // Дабл клик по Группе - не используется
    onCombotree_DirNomenName_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("333")
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirNomenReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirNomensTree = Ext.getCmp(aButton.UO_idMain).storeDirNomensTree;
        storeDirNomensTree.load();
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
        mapInput.name = "pID"; mapInput.value = "ReportPriceList"; mapForm.appendChild(mapInput);

        //Параметр "pLanguage"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pLanguage"; mapInput.value = aButton.UO_Language; mapForm.appendChild(mapInput);

        //Параметр "PriceGreater0"
        if (Ext.getCmp("PriceGreater0" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "PriceGreater0"; mapInput.value = Ext.getCmp("PriceGreater0" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirPriceTypeID"
        if (Ext.getCmp("DirPriceTypeID" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirPriceTypeID"; mapInput.value = Ext.getCmp("DirPriceTypeID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);
        }

        //Параметр "DirNomenID"
        if (Ext.getCmp("DirNomenID" + aButton.UO_id).getValue()) {
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DirNomenID"; mapInput.value = Ext.getCmp("DirNomenID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);
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
        //window.open(HTTP_Help + "report-price-list/", '_blank');
        Ext.Msg.alert(lanOrgName, "Для формирования прибыли достаточно выбрать тип цены. Но можно сформировать отчет только по выбранной группе товара.");
    }
});