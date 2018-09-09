Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/List/controllerListObjectPFs', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.controllerListObjectPFs',


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewListObjectPFsClose: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    onGridClick: function (model, records) {
    },
    onGridItemclick: function (view, record, item, index, eventObj) {
        //Если запись удалена, то выдать сообщение и выйти
        if (funGridRecordDel(record)) { return; }

        if (varSelectOneClick) {

            //window.open("../report/reportpf/")

            var mapForm = document.createElement("form");
            mapForm.target = "Map";
            mapForm.method = "GET"; // or "post" if appropriate
            mapForm.action = "../report/reportpf/";

            //Параметр "pID"
            /*
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "pID"; mapInput.value = "DocBankSumsReport"; mapForm.appendChild(mapInput);
            */

            //Параметр "ListObjectID"
            var IdcallModelData = Ext.getCmp(view.grid.id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "ListObjectPFID"; mapInput.value = IdcallModelData.ListObjectPFID; mapForm.appendChild(mapInput);

            //Параметр "ListObjectID"
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "ListObjectID"; mapInput.value = Ext.getCmp("viewListObjectPFs" + view.grid.UO_id).GridServerParam1; mapForm.appendChild(mapInput);

            //Параметр "DocID"
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "DocID"; mapInput.value = Ext.getCmp("viewListObjectPFs" + view.grid.UO_id).TreeServerParam1; mapForm.appendChild(mapInput);

            //Параметр "HtmlExcel" (в параметр "UO_Function_Tree" впихнул текст: Html или Excel)
            var mapInput = document.createElement("input"); mapInput.type = "text";
            mapInput.name = "HtmlExcel"; mapInput.value = Ext.getCmp("viewListObjectPFs" + view.grid.UO_id).UO_Function_Tree; mapForm.appendChild(mapInput);


            document.body.appendChild(mapForm);
            map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

            if (map) { mapForm.submit(); }
            else { alert('You must allow popups for this map to work.'); }
        };
    },
    onGridItemdblclick: function (view, record, item, index, e) {
    },



});