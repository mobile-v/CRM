Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirCashOffices/controllerDirCashOfficesGrid", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirCashOfficesGrid': { close: this.this_close },

            //Грид (itemId=grid)
            'viewDirCashOfficesGrid button#btnEdit': { click: this.onGrid_BtnEdit },
            'viewDirCashOfficesGrid button#btnHelp': { click: this.onGrid_BtnHelp },
            // Клик по Гриду
            'viewDirCashOfficesGrid [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },
            'viewDirCashOfficesGrid #TriggerSearchGrid': {
                "ontriggerclick": this.onTriggerSearchGridClick1,
                "specialkey": this.onTriggerSearchGridClick2,
                "change": this.onTriggerSearchGridClick3
            },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Грид (itemId=grid) === === === === ===

    onGrid_BtnEdit: function (aButton, aEvent, aOptions) {
        if (Ext.getCmp(aButton.UO_idMain).UO_Function_Grid == undefined) {
            var Params = [
                "grid_" + aButton.UO_id, //UO_idCall
                true, //UO_Center
                true, //UO_Modal
                2     // 1 - Новое, 2 - Редактировать
            ];
            ObjectEditConfig("viewDocCashOfficeSumsEdit", Params);
        }
        else {
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }
        
    },

    onGrid_BtnHelp: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-kassa/", '_blank');
    },

    //Поиск
    onTriggerSearchGridClick1: function (aButton, aEvent) {
        funGridDoc(aButton.UO_id, HTTP_DocPurches); //, Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).value, HTTP_DocPurches
    },
    onTriggerSearchGridClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            funGridDoc(f.UO_id, HTTP_DocPurches); //, f.value, HTTP_DocPurches
        }
    },
    onTriggerSearchGridClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) funGridDoc(e.UO_id, HTTP_DocPurches); //, textReal, HTTP_DocPurches
    },


    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#btnEdit").setDisabled(records.length === 0);
    },
    //Клик: Редактирования или выбор
    onGrid_itemclick: function (view, record, item, index, eventObj) {
        //Если запись удалена, то выдать сообщение и выйти
        if (funGridRecordDel(record)) { return; }

        if (varSelectOneClick) {
            if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
                var Params = [
                    view.grid.id, //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    2     // 1 - Новое, 2 - Редактировать
                ];
                ObjectEditConfig("viewDocCashOfficeSumsEdit", Params);
            }
            else {
                Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
                Ext.getCmp(view.grid.UO_idMain).close();
            }
        }
    },
    //ДаблКлик: Редактирования или выбор
    onGrid_itemdblclick: function (view, record, item, index, e) {
        //Если запись удалена, то выдать сообщение и выйти
        if (funGridRecordDel(record)) { return; }

            if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
                var Params = [
                    view.grid.id, //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    2     // 1 - Новое, 2 - Редактировать
                ];
                ObjectEditConfig("viewDocCashOfficeSumsEdit", Params);
        }
    },
});