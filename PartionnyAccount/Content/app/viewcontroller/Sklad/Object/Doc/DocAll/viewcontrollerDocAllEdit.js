Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocAll/viewcontrollerDocAllEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocAllEdit',


    //Поиск для документов
    onTriggerSearchTreeClick1: function (aButton, aEvent) {
        fun_onTriggerSearchTreeClick_Search(aButton, false);
    },

    //Поиск только для Сервисного Центра
    onTriggerSearchGridClick1: function (aButton, aEvent) {
        
        if (Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).getValue().length > 0) {

            //Анулировать даты
            if (Ext.getCmp("DateS" + aButton.UO_id)) { Ext.getCmp("DateS" + aButton.UO_id).setValue(null); }
            if (Ext.getCmp("DatePo" + aButton.UO_id)) { Ext.getCmp("DatePo" + aButton.UO_id).setValue(null); }

            if (Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).UO_DocX == "viewDocServiceWorkshops") {
                //funGridDoc(aButton.UO_id, HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID);
                funGridDoc(aButton.UO_id, HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=9&DirWarehouseIDOnly=" + Ext.getCmp("DirWarehouseIDPanelGrid9_" + aButton.UO_id).getValue());
            }
            else if (Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).UO_DocX == "viewDocSecondHandWorkshops") {
                funGridDoc(aButton.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID);
            }

        }
    },

});