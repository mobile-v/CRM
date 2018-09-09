Ext.define("PartionnyAccount.controller.Sklad/Object/KKM/controllerKKMAdding", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({

            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewKKMAdding button#btnSave': { "click": this.onBtnSaveClick },
        });
    },



    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        
        if (Ext.getCmp("viewKKMAdding" + aButton.UO_id).UO_Type == 1) {
            DepositingCash(parseFloat(Ext.getCmp("DocCashOfficeSumSum" + aButton.UO_id).getValue()), Ext.getCmp("bPrint" + aButton.UO_id).getValue());
        }
        else {
            PaymentCash(parseFloat(Ext.getCmp("DocCashOfficeSumSum" + aButton.UO_id).getValue()), Ext.getCmp("bPrint" + aButton.UO_id).getValue());
        }

        //Закрыть форму!    
        Ext.getCmp("viewKKMAdding" + aButton.UO_id).close();
        
    },

});