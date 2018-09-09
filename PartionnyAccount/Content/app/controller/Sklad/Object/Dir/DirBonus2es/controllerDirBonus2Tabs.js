Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirBonus2es/controllerDirBonus2Tabs", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены другие виджеты ...)
            //Закрыте
            'viewDirBonus2es': { close: this.this_close },


            // === GridBarCode-Panel === === ===

            // Клик по Гриду
            'viewDirBonus2es [itemId=PanelGridBonusTab_grid]': { selectionchange: this.onPanelGridBankAccountGrid_selectionchange },
            'viewDirBonus2es button#PanelGridBonusTab_btnDelete': { "click": this.onBtnPanelGridBonusTab_btnDelete },
            'viewDirBonus2es button#PanelGridBonusTab_btnNew': { click: this.onBtnPanelGridBonusTab_btnNew },

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
        var storeDirBonus2TabsGrid = Ext.getCmp("PanelGridBonusTab_" + aButton.UO_id).store;
        storeDirBonus2TabsGrid.insert(0, new storeDirBonus2TabsGrid.model());

        var rowEditing = Ext.getCmp("PanelGridBonusTab_" + aButton.UO_id).rowEditing;
        rowEditing.startEdit(0, 0);
    },

});