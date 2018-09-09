Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandInventoriesEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocSecondHandInventoriesEdit',


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewDocSecondHandInventoriesEditClose: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },

    //Не используется
    onSpisatS_select: function (combo, records, eOpts) {
        console.log(records.data.SpisatS);
    },

    onSpisatS_changet: function (combo, records, eOpts) {
        //console.log(records);
        if (records == 1) {
            Ext.getCmp("SpisatSDirEmployeeID" + combo.UO_id).setDisabled(true);
            Ext.getCmp("SpisatSDirEmployeeID" + combo.UO_id).setValue(undefined);
        }
        else {
            Ext.getCmp("SpisatSDirEmployeeID" + combo.UO_id).setDisabled(false);
        }
    },


    onGrid_BtnGridRefresh: function (aButton, aEvent, aOptions) {

        Ext.getCmp("btnGridRefresh" + aButton.UO_id).disable();

        var store = Ext.getCmp("grid_" + aButton.UO_id).store;

        store.proxy.url =
            HTTP_Rem2Parties +
            "?DirServiceNomenID=0" +
            "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue() +
            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() + 
            "&collectionWrapper=DocSecondHandInventoryTab";

        store.load({ waitMsg: lanLoading });
        store.on('load', function () {
            Ext.getCmp("btnGridRefresh" + aButton.UO_id).enable();
        });

    },



    // === Кнопки === === ===

    //Отменить проведение
    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { viewcontrollerDocSecondHandInventoriesEdit_onBtnSaveClick(aButton); } } //, aEvent, aOptions
        });
    },

    //Провести
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInventoriesEdit_onBtnSaveClick(aButton); //, aEvent, aOptions
    },

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInventoriesEdit_onBtnSaveClick(aButton);
    },

    onBtnSaveCloseClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInventoriesEdit_onBtnSaveClick(aButton);
    },

    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },

    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-second-hand-inventories/", '_blank');
    },

    //Распечатать
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {
        //aButton.UO_Action: html, excel
        //alert(aButton.UO_Action);

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocSecondHandInventoryID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

        //Открытие списка ПФ
        var Params = [
            aButton.id,
            true, //UO_Center
            true, //UO_Modal
            aButton.UO_Action, //UO_Function_Tree: Html или Excel
            undefined,
            undefined,
            undefined,
            Ext.getCmp("DocID" + aButton.UO_id).getValue(),
            76,
        ]
        ObjectConfig("viewListObjectPFs", Params);

    },
    
});




//Функия сохранения
function viewcontrollerDocSecondHandInventoriesEdit_onBtnSaveClick(aButton) { //, aEvent, aOptions

    //Спецификация (табличная часть)
    var recordsDocSecondHandInventoryTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocSecondHandInventoryTab.push(rec.data); });

    //Проверка
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Точку!"); return; }
    if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите аппараты для списания!"); return; }
    if (Ext.getCmp("SpisatS" + aButton.UO_id).getValue() == 2 && Ext.getCmp("SpisatSDirEmployeeID" + aButton.UO_id).getValue() == undefined) { Ext.Msg.alert(lanOrgName, "Выбирите сотрудника!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocSecondHandInventories + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocSecondHandInventoryID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocSecondHandInventories + "?id=" + parseInt(Ext.getCmp("DocSecondHandInventoryID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }
    
    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocSecondHandInventoryTab: Ext.encode(recordsDocSecondHandInventoryTab) },

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {

            if (aButton.UO_Action == "held_cancel") {
                Ext.getCmp("Held" + aButton.UO_id).setValue(false);
                Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(false);
                Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(true);
                Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(true);
            }
            else if (aButton.UO_Action == "held") {
                Ext.getCmp("Held" + aButton.UO_id).setValue(true);
                Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(true);
                Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(false);
                Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(false);
            }


            //Если новая накладная присваиваем полученные номера!
            if (!Ext.getCmp('DocID' + aButton.UO_id).getValue()) {
                var sData = action.result.data;
                Ext.getCmp('DocID' + aButton.UO_id).setValue(sData.DocID);
                Ext.getCmp('DocSecondHandInventoryID' + aButton.UO_id).setValue(sData.DocSecondHandInventoryID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocSecondHandInventoryID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocSecondHandInventoryID);

                Ext.getCmp('viewDocSecondHandInventoriesEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocSecondHandInventoriesEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocSecondHandInventoryID" + aButton.UO_id).getValue() + ")");
                Ext.getCmp("btnPrint" + aButton.UO_id).setVisible(true);
            }


            //Закрыть
            if (aButton.UO_Action == "save_close" || aButton.UO_Action == "held") { Ext.getCmp(aButton.UO_idMain).close(); }
            else {
                //Обновляем записи в гриде
                /*var locStore = Ext.getCmp(aButton.UO_idMain).storeGrid;
                locStore.load({ waitMsg: lanLoading });
                locStore.on('load', function () {
                    controllerDocSecondHandInventoriesEdit_RecalculationSums(Ext.getCmp(aButton.UO_idMain).UO_id, false);
                });*/
            }

            //Перегрузить грид, если грид открыт
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }

        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};

