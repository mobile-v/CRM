Ext.define("PartionnyAccount.controller.Sklad/Object/List/controllerListObjectPFs", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewListObjectPFs': { close: this.this_close },


            // Клик по Гриду
            'viewListObjectPFs [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },




    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGrid_itemclick: function (view, record, item, index, eventObj) {
        //Если запись удалена, то выдать сообщение и выйти
        if (funGridRecordDel(record)) { return; }

        var map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");
        var IdcallModelData = Ext.getCmp(view.grid.id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
        controllerListObjectPFs_onGrid_itemclick(
            map,
            view.grid.UO_id,
            Ext.getCmp("viewListObjectPFs" + view.grid.UO_id).GridServerParam1,
            IdcallModelData.ListObjectPFID,
            Ext.getCmp("viewListObjectPFs" + view.grid.UO_id).TreeServerParam1,
            Ext.getCmp("viewListObjectPFs" + view.grid.UO_id).UO_Function_Tree
        );
    },
    //ДаблКлик: Редактирования или выбор
    onGrid_itemdblclick: function (view, record, item, index, e) {
        /*if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
            var Params = [
                view.grid.id, //dv.grid.id, //UO_idCall
                false, //UO_Center
                false, //UO_Modal
                2,     // 1 - Новое, 2 - Редактировать
                undefined,
                undefined,
                undefined,
                Ext.getCmp("viewListObjectPFs" + view.grid.UO_id).GridServerParam1 //(ListObjectID)
            ]
            ObjectEditConfig("viewListObjectPFsEdit", Params);
        }
        else {
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }*/
    },

});


function controllerListObjectPFs_onGrid_itemclick(map, UO_id, ListObjectID, ListObjectPFID, DocID, HtmlExcel) {
    
    var mapForm = document.createElement("form");
    mapForm.target = "Map";
    mapForm.method = "GET"; // or "post" if appropriate
    mapForm.action = "../report/reportpf/";


    var mapInput = document.createElement("input"); mapInput.type = "text";

    //Параметр "ListObjectID"
    if (ListObjectPFID == undefined) ListObjectPFID = 0;
    mapInput.name = "ListObjectPFID"; mapInput.value = ListObjectPFID; mapForm.appendChild(mapInput);

    //Параметр "ListObjectID"
    var mapInput = document.createElement("input"); mapInput.type = "text";
    mapInput.name = "ListObjectID"; mapInput.value = ListObjectID; mapForm.appendChild(mapInput);

    //Параметр "DocID"
    var mapInput = document.createElement("input"); mapInput.type = "text";
    mapInput.name = "DocID"; mapInput.value = DocID; mapForm.appendChild(mapInput);

    //Параметр "HtmlExcel" (в параметр "UO_Function_Tree" впихнул текст: Html или Excel)
    var mapInput = document.createElement("input"); mapInput.type = "text";
    mapInput.name = "HtmlExcel"; mapInput.value = HtmlExcel; mapForm.appendChild(mapInput);


    document.body.appendChild(mapForm);
    //var map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");


    if (map) { mapForm.submit(); }
    else { alert('You must allow popups for this map to work.'); }

};