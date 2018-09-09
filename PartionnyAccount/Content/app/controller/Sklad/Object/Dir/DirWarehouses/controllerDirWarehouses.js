Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirWarehouses/controllerDirWarehouses", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirWarehouses': { close: this.this_close },

            //Группа (itemId=tree)
            // Меню Группы
            'viewDirWarehouses [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDirWarehouses [itemId=FolderEdit]': { click: this.onTree_folderEdit },
            'viewDirWarehouses [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDirWarehouses [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDirWarehouses [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },

            'viewDirWarehouses [itemId=SmenaClose]': { change: this.onSmenaCloseChecked },


            //Касса
            'viewDirWarehouses button#btnDirCashOfficeEdit': { click: this.onBtnDirCashOfficeEditClick },
            'viewDirWarehouses button#btnDirCashOfficeReload': { click: this.onBtnDirCashOfficeReloadClick },

            //Банк
            'viewDirWarehouses button#btnDirBankEdit': { click: this.onBtnDirBankEditClick },
            'viewDirWarehouses button#btnDirBankReload': { click: this.onBtnDirBankReloadClick },


            // Клик по Группе
            'viewDirWarehouses [itemId=KKMSActive]': { change: this.onKKMSActiveChange },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirWarehouses button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirWarehouses button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirWarehouses button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Группа (itemId=tree) === === === === ===

    //Меню Группы
    onTree_folderNew: function (aButton, aEvent) {
        controllerDirWarehouses_onTree_folderNew(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDirWarehouses_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDirWarehouses_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        //model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);

        //Ext.getCmp("btnHistory" + model.view.ownerGrid.UO_id).setDisabled(records.length === 0);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        //Если запись помечена на удаление, то сообщить об этом и выйти
        if (rec.Del == true) {
            Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
            return;
        };

        //Получаем форму редактирования
        var widgetXForm = Ext.getCmp("form_" + view.grid.UO_id);
        widgetXForm.UO_Loaded = false;

        var storeDirWarehousesGrid = Ext.getCmp("PanelGridDirWarehouse_" + view.grid.UO_id).getStore();
        storeDirWarehousesGrid.setData([], false);
        Ext.getCmp("PanelGridDirWarehouse_" + view.grid.UO_id).store = storeDirWarehousesGrid;
        storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid&Sub=" + rec.get('id') + "";

        widgetXForm.load({
            method: "GET",
            timeout: varTimeOutDefault,
            waitMsg: lanLoading,
            url: HTTP_DirWarehouses + rec.get('id') + "/",
            success: function (form, action) {
                widgetXForm.UO_Loaded = true;
                storeDirWarehousesGrid.load({ waitMsg: lanLoading });
            },
            failure: function (form, action) {
                funPanelSubmitFailure(form, action);
            }
        });
    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
    },

    onSmenaCloseChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("SmenaCloseTime" + ctl.UO_id).enable();
        }
        else {
            Ext.getCmp("SmenaCloseTime" + ctl.UO_id).disable();
        }
    },


    // *** Касса ***
    //Редактирование или добавление
    onBtnDirCashOfficeEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCashOffices", Params);
    },
    //РеЛоад - перегрузить Combo, что бы появились новые записи
    onBtnDirCashOfficeReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCashOfficesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCashOfficesGrid;
        storeDirCashOfficesGrid.load();
    },


    // *** Банк ***
    //Редактирование или добавление
    onBtnDirBankEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirBanks", Params);
    },
    //РеЛоад - перегрузить Combo, что бы появились новые записи
    onBtnDirBankReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirBanksGrid = Ext.getCmp(aButton.UO_idMain).storeDirBanksGrid;
        storeDirBanksGrid.load();
    },


    onKKMSActiveChange: function (checkbox, newVal, oldVal) {
        if (newVal && !varKKMSActive_FromSet) {
            alert("Запрещенно использование ККМ! Вначеле настройте ККМ в Настройках!");
        }
        else {
            //...
        }
    },


    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        
        //Проверка:
        if (
            parseFloat(Ext.getCmp("SalaryPercentTrade" + aButton.UO_id).getValue()) < 0 || parseFloat(Ext.getCmp("SalaryPercentTrade" + aButton.UO_id).getValue()) > 100 ||
            ((parseInt(Ext.getCmp("SalaryPercentService1TabsType" + aButton.UO_id).getValue()) == 1) && (parseFloat(Ext.getCmp("SalaryPercentService1Tabs" + aButton.UO_id).getValue()) < 0 || parseFloat(Ext.getCmp("SalaryPercentService1Tabs" + aButton.UO_id).getValue()) > 100)) ||
            ((parseInt(Ext.getCmp("SalaryPercentService2TabsType" + aButton.UO_id).getValue()) == 1) && (parseFloat(Ext.getCmp("SalaryPercentService2Tabs" + aButton.UO_id).getValue()) < 0 || parseFloat(Ext.getCmp("SalaryPercentService2Tabs" + aButton.UO_id).getValue()) > 100)) ||
            parseFloat(Ext.getCmp("SalaryPercentSecond" + aButton.UO_id).getValue()) < 0 || parseFloat(Ext.getCmp("SalaryPercentSecond" + aButton.UO_id).getValue()) > 100
          ) {
            Ext.Msg.alert(lanOrgName, "Не корректный процент от прибыли!");
            return;
        }

        if (Ext.getCmp("KKMSActive" + aButton.UO_id).getValue() && !varKKMSActive_FromSet) {
            Ext.getCmp("KKMSActive" + aButton.UO_id).setValue(false);
        }
        


        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirWarehouses;
        if (parseInt(Ext.getCmp("DirWarehouseID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirWarehouses + "?id=" + parseInt(Ext.getCmp("DirWarehouseID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                Ext.getCmp("tree_" + aButton.UO_id).getStore().load();

                var storeDirWarehousesGrid = Ext.getCmp("PanelGridDirWarehouse_" + aButton.UO_id).getStore();
                storeDirWarehousesGrid.setData([], false);
                Ext.getCmp("PanelGridDirWarehouse_" + aButton.UO_id).store = storeDirWarehousesGrid;
                storeDirWarehousesGrid.proxy.url = HTTP_DirWarehouses + "?type=Grid&Sub=" + Ext.getCmp("DirWarehouseID" + aButton.UO_id).value + "";

                storeDirWarehousesGrid.load({ waitMsg: lanLoading });
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });

        Ext.getCmp("tree_" + aButton.UO_id).enable();
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        //Ext.getCmp(aButton.UO_idMain).close();
        Ext.getCmp("tree_" + aButton.UO_id).enable();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-warehouses/", '_blank');
    }
});



// === Функции === === ===

function controllerDirWarehouses_onTree_folderNew(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("tree_" + id).disable();

    //По-умолчанию
    Ext.getCmp("SalaryPercentTradeType" + id).setValue(1);
    Ext.getCmp("SalaryPercentTrade" + id).setValue(0);
    
    Ext.getCmp("SalaryPercentService1TabsType" + id).setValue(1);
    Ext.getCmp("SalaryPercentService1Tabs" + id).setValue(0);

    Ext.getCmp("SalaryPercentService2TabsType" + id).setValue(1);
    Ext.getCmp("SalaryPercentService2Tabs" + id).setValue(0);

    Ext.getCmp("SalaryPercentSecond" + id).setValue(0);
    Ext.getCmp("SalaryPercent2Second" + id).setValue(0);
    Ext.getCmp("SalaryPercent3Second" + id).setValue(0);

    //Сообщение!
    Ext.Msg.alert(
        lanOrgName,
        "Внимание!<br/>" +
        "Создайте новые записи для Кассы и Банка.<br/>" +
        "И выберите их в Точке."
    );

};
function controllerDirWarehouses_onTree_folderNewSub(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("AddSub" + id).setValue(true);
    var node = funReturnNode(id);
    Ext.getCmp("Sub" + id).setValue(node.data.id);

    Ext.getCmp("tree_" + id).disable();
};
function controllerDirWarehouses_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                Ext.getCmp("DirWarehouseID" + id).setValue(0);
                Ext.getCmp("DirWarehouseName" + id).setValue(Ext.getCmp("DirWarehouseName" + id).getValue() + " (копия)");

                Ext.getCmp("tree_" + id).disable();
            }
        }
    });
};
function controllerDirWarehouses_onTree_folderDel(id) {
    //Формируем сообщение: Удалить или снять пометку на удаление
    var sMsg = lanDelete;
    if (Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;

    //Процес Удаление или снятия пометки
    Ext.MessageBox.show({
        title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                //Лоадер
                var loadingMask = new Ext.LoadMask({
                    msg: 'Please wait...',
                    target: Ext.getCmp("tree_" + id)
                });
                loadingMask.show();
                //Выбранный ID-шник Грида
                var DirWarehouseID = Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.id; //Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.DirWarehouseID;
                //Запрос на удаление
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirWarehouses + DirWarehouseID + "/",
                    method: 'DELETE',
                    success: function (result) {
                        loadingMask.hide();
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            Ext.MessageBox.show({ title: lanOrgName, msg: sData.data.Msg, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.OK })
                            Ext.getCmp("tree_" + id).view.store.load();

                            //Чистим форму редактирования *** ***
                            //форма
                            Ext.getCmp("form_" + id).reset();
                        } else {
                            Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
                        }
                    },
                    failure: function (form, action) {
                        loadingMask.hide();
                        //Права.
                        /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                        Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                        funPanelSubmitFailure(form, action);
                    }
                });

            }
        }
    });
};
function controllerDirWarehouses_onTree_folderSubNull(id) {
    return;
    //Если это не узел, то выйти и сообщить об этом!
    /*if (!Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.leaf) {
        Ext.Msg.alert(lanOrgName, "Данная ветвь уже корневая!");
        return;
    }*/


    //Формируем сообщение: Удалить или снять пометку на удаление
    var sMsg = "Сделать корневой";
    if (Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;


    Ext.MessageBox.show({
        title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {

                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirWarehouses + "?id=" + Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.id + "&sub=0", // + overModel.data.id,
                    method: 'PUT',
                    success: function (result) {
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            //Ext.MessageBox.show({ title: lanOrgName, msg: sData.data.Msg, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.OK })
                            Ext.getCmp("tree_" + id).view.store.load();
                        } else {
                            Ext.getCmp("tree_" + id).view.store.load();
                            Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                        }
                    },
                    failure: function (form, action) {
                        //Права.
                        /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                        Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                        Ext.getCmp("tree_" + id).view.store.load();
                        funPanelSubmitFailure(form, action);
                    }
                });

            }
        }
    });

};

