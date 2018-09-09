Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocServicePurches/controllerDocServiceMasterSelect", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({

            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocServiceMasterSelect button#btnSave': { "click": this.onBtnSaveClick },
        });
    },



    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        var UO_id = aButton.UO_id;


        var viewDocServiceMasterSelect = Ext.getCmp("viewDocServiceMasterSelect" + UO_id);
        rec = viewDocServiceMasterSelect.UO_GridRecord;
        var DocServicePurchID = rec.data.DocServicePurchID;


        //На сервере создаём метод смены Мастера, по аналогии по смене гарантии
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocServicePurches + DocServicePurchID + "/" + Ext.getCmp("DirEmployeeID" + UO_id).getValue() + "/777/888/",
            method: 'PUT',

            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
                else {
                    //Ext.getCmp("gridLog0_" + UO_id).getStore().load();
                    Ext.getCmp("viewDocServiceMasterSelect" + UO_id).close();
                }
            },
            failure: function (result) {
                var sData = Ext.decode(result.responseText);
                Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
            }
        });


    },

});