Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirDiscounts/controllerDirDiscountTabs", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены другие виджеты ...)
            //Закрыте
            'viewDirDiscounts': { close: this.this_close },


            // === GridBarCode-Panel === === ===

            // Клик по Гриду
            'viewDirDiscounts [itemId=PanelGridDiscountTab_grid]': { selectionchange: this.onPanelGridDiscountTab_grid_selectionchange },
            'viewDirDiscounts button#PanelGridDiscountTab_btnDelete': { "click": this.onBtnPanelGridDiscountTab_btnDelete },
            'viewDirDiscounts button#PanelGridDiscountTab_btnNew': { click: this.onBtnPanelGridDiscountTab_btnNew },

        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    // === GridDiscountTabs-Panel === === ===

    //Кнопки редактирования Енеблед
    onPanelGridDiscountTab_grid_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#PanelGridDiscountTab_btnDelete").setDisabled(records.length === 0);
    },
    onBtnPanelGridDiscountTab_btnDelete: function (aButton, aEvent, aOptions) {
        var selection = Ext.getCmp("PanelGridDiscountTab_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
        if (selection) { Ext.getCmp("PanelGridDiscountTab_" + aButton.UO_id).store.remove(selection); }
    },
    onBtnPanelGridDiscountTab_btnNew: function (aButton, aEvent, aOptions) {
        var storeDirDiscountTabsGrid = Ext.getCmp("PanelGridDiscountTab_" + aButton.UO_id).store;
        storeDirDiscountTabsGrid.insert(0, new storeDirDiscountTabsGrid.model());

        var rowEditing = Ext.getCmp("PanelGridDiscountTab_" + aButton.UO_id).rowEditing;
        rowEditing.startEdit(0, 0);
    },

});