Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocSalaries/controllerDocSalaryTabsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],
    
    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocSalaryTabsEdit': { close: this.this_close },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocSalaryTabsEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocSalaryTabsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocSalaryTabsEdit button#btnDel': { "click": this.onBtnDelClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },





    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //Считаем сумму
        Ext.getCmp("Sums" + aButton.UO_id).setValue(
            parseFloat(Ext.getCmp("SumSalary" + aButton.UO_id).getValue()) + 
            parseFloat(Ext.getCmp("DirBonusIDSalary" + aButton.UO_id).getValue()) +
            parseFloat(Ext.getCmp("DirBonusID2Salary" + aButton.UO_id).getValue())
            )

        fun_SaveTabDocNoChar1(aButton, controllerDocSalariesEdit_RecalculationSums);
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnDelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName,
            msg: lanDelete + "?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    var selection = Ext.getCmp(aButton.UO_idCall).getView().getSelectionModel().getSelection()[0];
                    if (selection) {
                        Ext.getCmp(aButton.UO_idCall).store.remove(selection);

                        Ext.getCmp(aButton.UO_idMain).close();
                    }
                }
            }
        });
    },

});