﻿Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirContractors/controllerDirContractors", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirContractors': { close: this.this_close },

            //Группа (itemId=tree)
            // Меню Группы
            'viewDirContractors [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDirContractors [itemId=FolderEdit]': { click: this.onTree_folderEdit },
            'viewDirContractors [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDirContractors [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDirContractors [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },

            //Google Maps
            'viewDirContractors button#btnGoogleMaps': { "click": this.onBtnGoogleMapsClick },

            //Скидка
            'viewDirContractors #DirDiscountID': { "select": this.onDirDiscountIDSelect },
            //Анулирование скидки
            'viewDirContractors button#btnDiscountClear': { "click": this.onBtnDiscountClearClick },
             

            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirContractors button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirContractors button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirContractors button#btnHelp': { "click": this.onBtnHelpClick },
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
        controllerDirContractors_onTree_folderNew(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDirContractors_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDirContractors_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        //model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);
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


        widgetXForm.load({
            method: "GET",
            timeout: varTimeOutDefault,
            waitMsg: lanLoading,
            url: HTTP_DirContractors + rec.get('id') + "/",
            success: function (form, action) {
                widgetXForm.UO_Loaded = true;

                //Если выбрана градационная скидка, Фиксированную очистить
                if (Ext.getCmp("DirDiscountID" + view.grid.UO_id).getValue() > 0) {
                    Ext.getCmp("DirContractorDiscount" + view.grid.UO_id).setValue(0);
                    Ext.getCmp("DirContractorDiscount" + view.grid.UO_id).setReadOnly(true); Ext.getCmp("DirContractorDiscount" + view.grid.UO_id).disable();
                }
                else {
                    Ext.getCmp("DirContractorDiscount" + view.grid.UO_id).enable();
                }

                //Гугл Мапс
                if (Ext.getCmp("DirContractorAddress" + view.grid.UO_id).value.length > 2) {
                    controllerDirContractors_onBtnGoogleMapsClick(view.grid.UO_id);
                }
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


    //Google Maps
    onBtnGoogleMapsClick: function (aButton, aEvent, aOptions) {
        //1. Панель "GMap": задать Гео-Локация, только если есть адрес!
        if (Ext.getCmp("DirContractorAddress" + aButton.UO_id).value.length > 2) {
            controllerDirContractors_onBtnGoogleMapsClick(aButton.UO_id);
            //Переключится на панель "//GMap"
            Ext.getCmp("tab_" + aButton.UO_id).setActiveTab(Ext.getCmp("PanelGMap_" + aButton.UO_id));
        }
        else {
            Ext.MessageBox.show({ title: lanFailure, msg: "Введите адрес контрагента для гео-локации!", icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
        }
    },


    //Скидка
    onDirDiscountIDSelect: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirContractorDiscount" + aButton.UO_id).setValue(0);
        Ext.getCmp("DirContractorDiscount" + aButton.UO_id).setReadOnly(true); Ext.getCmp("DirContractorDiscount" + aButton.UO_id).disable();
    },
    //Очистить Скидку
    onBtnDiscountClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirDiscountID" + aButton.UO_id).setValue(0);
        //Ext.getCmp("DirContractorDiscount" + aButton.UO_id).setValue(0);
        Ext.getCmp("DirContractorDiscount" + aButton.UO_id).setReadOnly(false); Ext.getCmp("DirContractorDiscount" + aButton.UO_id).enable();
    },





    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirContractors;
        if (parseInt(Ext.getCmp("DirContractorID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirContractors + "?id=" + parseInt(Ext.getCmp("DirContractorID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                Ext.getCmp("tree_" + aButton.UO_id).getStore().load();
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
        window.open(HTTP_Help + "spravochnik-kontragenty/", '_blank');
    }
});



// === Функции === === ===

function controllerDirContractors_onTree_folderNew(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("tree_" + id).disable();

    Ext.getCmp("DirContractorDiscount" + id).setValue(0);
};
function controllerDirContractors_onTree_folderNewSub(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("AddSub" + id).setValue(true);
    var node = funReturnNode(id);
    Ext.getCmp("Sub" + id).setValue(node.data.id);

    Ext.getCmp("tree_" + id).disable();
};
function controllerDirContractors_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                Ext.getCmp("DirContractorID" + id).setValue(0);
                Ext.getCmp("DirContractorName" + id).setValue(Ext.getCmp("DirContractorName" + id).getValue() + " (копия)");

                Ext.getCmp("tree_" + id).disable();
            }
        }
    });
};
function controllerDirContractors_onTree_folderDel(id) {
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
                var DirContractorID = Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.id; //Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.DirContractorID;
                //Запрос на удаление
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirContractors + DirContractorID + "/",
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
function controllerDirContractors_onTree_folderSubNull(id) {
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
                    url: HTTP_DirContractors + "?id=" + Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.id + "&sub=0", // + overModel.data.id,
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

function controllerDirContractors_onBtnGoogleMapsClick(id) {
    //Удаляем "gmappanel" с старой геолокацией
    if (Ext.getCmp("gmappanel" + id) != undefined) {
        Ext.getCmp("PanelGMap_" + id).remove(Ext.getCmp("gmappanel" + id), true);
    };
    //Создаём виджет "gmappanel"
    Ext.getCmp("PanelGMap_" + id).add([
        {
            xtype: 'gmappanel',
            gmapType: 'map',
            id: "gmappanel" + id,
            center: {
                geoCodeAddr: Ext.getCmp("DirContractorAddress" + id).value,
                marker: {
                    title: 'Holmes Home'
                }
            },
            mapOptions: {
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }
        }
    ]);
};