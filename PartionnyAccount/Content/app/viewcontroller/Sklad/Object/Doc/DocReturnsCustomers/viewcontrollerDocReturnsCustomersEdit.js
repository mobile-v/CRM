Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocReturnsCustomers/viewcontrollerDocReturnsCustomersEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocReturnsCustomersEdit',


    //Редактирование или добавление нового Поставщика
    onTriggerDocSaleNameClick: function (aButton, aEvent, aOptions) {
        
        var Params = [
            aButton.id,
            true, //UO_Center
            true, //UO_Modal
            undefined, //UO_Function_Tree
            this.fn_onTriggerDocSaleNameClick, //UO_Function_Grid
            true,  //TreeShow
            true, //GridShow
            undefined,     //TreeServerParam1
            undefined      //GridServerParam1
        ]
        ObjectConfig("viewDocSales", Params);
    },
    //Заполнить 2-а поля
    fn_onTriggerDocSaleNameClick: function (id, rec) {
        Ext.getCmp("DocSaleName" + id).setValue("№ " + rec.get("DocSaleID") + " за " + rec.get("DocDate"));
        Ext.getCmp("DocSaleID" + id).setValue(rec.get("DocSaleID"));
        Ext.getCmp("DirContractorID" + id).setValue(rec.get("DirContractorID"));
        Ext.getCmp("DirWarehouseID" + id).setValue(rec.get("DirWarehouseID"));
        Ext.getCmp("DirContractorIDOrg" + id).setValue(rec.get("DirContractorIDOrg"));
        Ext.getCmp("DirVatValue" + id).setValue(rec.get("DirVatValue"));
        Ext.getCmp("Discount" + id).setValue(rec.get("Discount"));


        //Обновление "Списание партий"

        //Выбран документ Продажа
        if (Ext.getCmp("DocSaleID" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите документ Продажа (так как списанные партии привязаны к Продаже)!"); return; }

        //Получаем storeGrid и делаем load()
        var storeGrid = Ext.getCmp("gridPartyMinus_" + id).getStore();
        storeGrid.proxy.url = HTTP_RemPartyMinuses + "?DocSaleID=" + Ext.getCmp("DocSaleID" + id).getValue();
        storeGrid.load();
    },


});