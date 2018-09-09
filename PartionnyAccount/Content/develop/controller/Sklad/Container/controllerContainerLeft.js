Ext.define("PartionnyAccount.controller.Sklad/Container/controllerContainerLeft", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewContainerLeft': { close: this.this_close },

            //Группа (itemId=tree)
            // Клик по Группе
            'viewContainerLeft [itemId=viewPanelJournal]': {
                itemclick: this.onTree_itemclick
            },

        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        var Params = [
            "viewContainerLeft",
            false, //UO_Center
            false, //UO_Modal
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            rec.data.id //GridServerParam1 (ListObjectID)
        ]
        ObjectConfig("viewListObjectPFs", Params);
    },

});