Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirBonuses/controllerDirBonusTabs", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены другие виджеты ...)
            //Закрыте
            'viewDirBonuses': { close: this.this_close },


            // === GridBarCode-Panel === === ===

            // Клик по Гриду
            'viewDirBonuses [itemId=PanelGridBonusTab_grid]': { selectionchange: this.onPanelGridBankAccountGrid_selectionchange },
            'viewDirBonuses button#PanelGridBonusTab_btnDelete': { "click": this.onBtnPanelGridBonusTab_btnDelete },
            'viewDirBonuses button#PanelGridBonusTab_btnNew': { click: this.onBtnPanelGridBonusTab_btnNew },

        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    // === GridBonusTabs-Panel === === ===

    //Кнопки редактирования Енеблед
    onPanelGridBankAccountGrid_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#PanelGridBonusTab_btnDelete").setDisabled(records.length === 0);
    },
    onBtnPanelGridBonusTab_btnDelete: function (aButton, aEvent, aOptions) {
        var selection = Ext.getCmp("PanelGridBonusTab_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
        if (selection) { Ext.getCmp("PanelGridBonusTab_" + aButton.UO_id).store.remove(selection); }
    },
    onBtnPanelGridBonusTab_btnNew: function (aButton, aEvent, aOptions) {
        var storeDirBonusTabsGrid = Ext.getCmp("PanelGridBonusTab_" + aButton.UO_id).store;
        storeDirBonusTabsGrid.insert(0, new storeDirBonusTabsGrid.model());

        var rowEditing = Ext.getCmp("PanelGridBonusTab_" + aButton.UO_id).rowEditing;
        rowEditing.startEdit(0, 0);
    },

});