Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirEmployees/controllerDirEmployees", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirEmployees': { close: this.this_close },

            //Группа (itemId=tree)
            'viewDirEmployees [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDirEmployees [itemId=FolderEdit]': { click: this.onTree_folderEdit },
            'viewDirEmployees [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDirEmployees [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDirEmployees [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },

            'viewDirEmployees dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },
            


            //Фотка
            'viewDirEmployees [itemId=ImageLink]': { "change": this.onPanelGeneral_ImageLink },

            // === Account-Panel === === ===
            //Доступ в сервис
            'viewDirEmployees [itemId=DirEmployeeActive]': { change: this.onDirEmployeeActiveChecked },
            //Склад
            //'viewDirEmployees button#btnClearWarehouse': { "click": this.onBtnClearWarehouseClick },
            //'viewDirEmployees button#btnDirWarehouseEdit': { click: this.onBtnDirWarehouseEditClick },
            //'viewDirEmployees button#btnDirWarehouseReload': { click: this.onBtnDirWarehouseReloadClick },
            //ТТ
            //'viewDirEmployees [itemId=RetailOnly]': { change: this.onRetailOnlyChecked },
            //Организация
            'viewDirEmployees [itemId=DirContractorIDOrg]': { select: this.onDirContractorIDOrgSelect }, //Не используется
            'viewDirEmployees button#btnClearContractorOrg': { "click": this.onBtnClearContractorOrgClick },
            'viewDirEmployees button#btnDirContractorOrgEdit': { click: this.onBtnDirContractorOrgEditClick },
            'viewDirEmployees button#btnDirContractorOrgReload': { click: this.onBtnDirContractorOrgReloadClick },
            //Точки
            'viewDirEmployees [itemId=PanelGridEmployeeWarehouses_grid]': { selectionchange: this.onPanelGridEmployeeWarehouses_selectionchange },
            'viewDirEmployees button#PanelGridEmployeeWarehouses_btnDelete': { "click": this.onBtnPanelGridEmployeeWarehouses_btnDelete },
            'viewDirEmployees button#btnAddWarehouse': { click: this.onBtnAddWarehouse },
            //Видит все точки
            'viewDirEmployees [itemId=RightDocServicePurchesWarehouseAllCheck]': { change: this.onRightDocServicePurchesWarehouseAllCheckChange },


            // === Salary-Panel === === ===
            //DirCurrency
            //Clear Bonus
            'viewDirEmployees button#btnClearBonus': { "click": this.onBtnClearBonusClick },
            'viewDirEmployees button#btnClearBonus2': { "click": this.onBtnClearBonus2Click },
            'viewDirEmployees button#btnClearBonus3': { "click": this.onBtnClearBonus3Click },
            'viewDirEmployees button#btnClearBonus4': { "click": this.onBtnClearBonus4Click },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirEmployees button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirEmployees button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirEmployees button#btnHelp': { "click": this.onBtnHelpClick },

            'viewDirEmployees button#btnHistory': { "click": this.onBtnHistoryClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Группа (itemId=tree) === === === === ===

    //Меню Группы
    onTree_expandAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).expandAll();
    },
    onTree_collapseAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).collapseAll();
    },

    onTree_folderNew: function (aButton, aEvent) {
        controllerDirEmployees_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDirEmployees_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDirEmployees_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDirEmployees_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        //model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);

        Ext.getCmp("btnHistory" + model.view.ownerGrid.UO_id).setDisabled(records.length === 0);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        //Если запись помечена на удаление, то сообщить об этом и выйти
        if (rec.Del == true) {
            Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
            return;
        };

        varBlock = true;

        //Получаем форму редактирования
        var widgetXForm = Ext.getCmp("form_" + view.grid.UO_id);
        widgetXForm.UO_Loaded = false;

        var storeDirEmployeeWarehousesGrid = Ext.getCmp("PanelGridEmployeeWarehouses_" + view.grid.UO_id).getStore();
        storeDirEmployeeWarehousesGrid.setData([], false);
        Ext.getCmp("PanelGridEmployeeWarehouses_" + view.grid.UO_id).store = storeDirEmployeeWarehousesGrid;
        storeDirEmployeeWarehousesGrid.proxy.url = HTTP_DirEmployeeWarehouses + rec.get('id') + "/";
        
        widgetXForm.load({
            method: "GET",
            timeout: varTimeOutDefault,
            waitMsg: lanLoading,
            url: HTTP_DirEmployees + rec.get('id') + "/",
            success: function (form, action) {
                widgetXForm.UO_Loaded = true;

                storeDirEmployeeWarehousesGrid.load({ waitMsg: lanLoading }); //storeDirEmployeeWarehousesGrid.on('load', function () { });

                //Системная запись, если да то выдать сообщение
                if (Ext.getCmp("SysRecord" + widgetXForm.UO_id).getValue() == true) {
                    Ext.getCmp("DirEmployeeActive" + widgetXForm.UO_id).disable();
                    Ext.getCmp("DirWarehouseID" + widgetXForm.UO_id).disable();
                    Ext.getCmp("btnAddWarehouse" + widgetXForm.UO_id).disable();
                    //Ext.getCmp("btnClearWarehouse" + widgetXForm.UO_id).disable();
                    //Ext.getCmp("RetailOnly" + widgetXForm.UO_id).disable();
                    Ext.getCmp("DirContractorIDOrg" + widgetXForm.UO_id).disable();

                    //Вкладка "Точка" - заблокировать
                    Ext.getCmp("PanelWarehouses_" + widgetXForm.UO_id).disable();

                    //Вкладка "Права" - только доя чтения
                    //Ext.getCmp("PanelRightsAccess_" + widgetXForm.UO_id).readOnly(true);

                    Ext.Msg.alert(lanOrgName, txtMsg040);
                }
                else {
                    //Т.к. форма не закрывается!
                    Ext.getCmp("DirEmployeeActive" + widgetXForm.UO_id).enable();

                    if (Ext.getCmp("DirEmployeeActive" + widgetXForm.UO_id).getValue()) {
                        Ext.getCmp("DirWarehouseID" + widgetXForm.UO_id).enable();
                        Ext.getCmp("btnAddWarehouse" + widgetXForm.UO_id).enable();
                        //Ext.getCmp("btnClearWarehouse" + widgetXForm.UO_id).enable();
                        //Ext.getCmp("RetailOnly" + widgetXForm.UO_id).enable();
                        Ext.getCmp("DirContractorIDOrg" + widgetXForm.UO_id).enable();
                        Ext.getCmp("btnClearContractorOrg" + widgetXForm.UO_id).enable();

                        //Вкладка "Точка" - раз-заблокировать
                        Ext.getCmp("PanelWarehouses_" + widgetXForm.UO_id).enable();
                    }
                }

                //===
                Ext.getCmp("DirEmployeeLink" + widgetXForm.UO_id).setValue(
                    "https://sklad.intradecloud.com/account/login/?" +
                    "username=" + Ext.getCmp("DirEmployeeLogin" + widgetXForm.UO_id).getValue() + "@" + varLoginMS + 
                    "&" +
                    "password=" + Ext.getCmp("DirEmployeePswd" + widgetXForm.UO_id).getValue() + "" +
                    "&" +
                    "language=1"
                );

                varBlock = false;

            },
            failure: function (form, action) {
                funPanelSubmitFailure(form, action);
                varBlock = false;
            }
        });
    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
    },

    //beforedrop
    onTree_beforedrop: function (node, data, overModel, dropPosition, dropPosition1, dropPosition2, dropPosition3) {

        //Если это не узел, то выйти и сообщить об этом!
        if (overModel.data.leaf) {
            Ext.Msg.alert(lanOrgName, "В данную ветвь перемещать запрещено!");
            //overModel.data.leaf = false;
            return;

            /*overModel.data.leaf = false;
            Ext.getCmp("tree_" + data.view.panel.UO_id).getView().refresh();*/
        }


        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            url: HTTP_DirEmployees + "?id=" + data.records[0].data.id + "&sub=" + overModel.data.id,
            method: 'PUT',
            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == true) {
                    
                } else {
                    Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                    Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                }
            },
            failure: function (form, action) {
                //Права.
                /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                funPanelSubmitFailure(form, action);
            }
        });
    },
    //drop
    onTree_drop: function (node, data, overModel, dropPosition) {
        //Ext.Msg.alert("Группа перемещена!");
    },

    // *** *** *** *** *** *** *** *** ***


    // Редактирование === === === === === === === === === === ===

    //Поиск
    onTriggerSearchGridClick1: function (aButton, aEvent) {
        funGridDir(aButton.UO_id, Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).value, HTTP_DirEmployees);
    },
    onTriggerSearchGridClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            funGridDir(f.UO_id, f.value, HTTP_DirEmployees);
        }
    },
    onTriggerSearchGridClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) funGridDir(e.UO_id, textReal, HTTP_DirEmployees);
    },

    //Разрешить Под-Группы? (AddSub)
    onAddSubChecked: function (ctl, val) { //ctl.UO_id

        //Если форма ещё не загружена - выйти!
        var widgetXForm = Ext.getCmp("form_" + ctl.UO_id);
        if (!widgetXForm.UO_Loaded) return;

        var node = funReturnNode(ctl.UO_id);
        if (node != undefined) {
            //val==true - checked, val==false - No checked
            if (val) {
                node.data.leaf = false;
                Ext.getCmp("tree_" + ctl.UO_id).getView().refresh();
                node.expand();
            }
            else {
                node.data.leaf = true;
                Ext.getCmp("tree_" + ctl.UO_id).getView().refresh();
            }
        }

        // *** *** *** *** *** *** ***

    },



    // *** Фотка ***
    onPanelGeneral_ImageLink: function (aTextfield, aValueReal, aValuePrevious) {
        try {
            Ext.getCmp("imageShow" + aTextfield.UO_id).setSrc(aValueReal);
        } catch (ex) { alert(e.name); }

    },



    // === Account-Panel === === ===

    onDirEmployeeActiveChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            //Ext.getCmp("SysDirRightID" + ctl.UO_id).enable();
            //Ext.getCmp("DirWarehouseName" + ctl.UO_id).enable();
            Ext.getCmp("DirEmployeeLogin" + ctl.UO_id).enable();
            Ext.getCmp("DirEmployeePswd" + ctl.UO_id).enable();
            Ext.getCmp("DirEmployeeLink" + ctl.UO_id).enable();
            Ext.getCmp("DirWarehouseID" + ctl.UO_id).enable();
            //Ext.getCmp("RetailOnly" + ctl.UO_id).enable();
            Ext.getCmp("DirContractorIDOrg" + ctl.UO_id).enable();

            Ext.Msg.alert(lanOrgName, txtMsg107);
        }
        else {
            Ext.getCmp("DirEmployeeLogin" + ctl.UO_id).disable();
            Ext.getCmp("DirEmployeePswd" + ctl.UO_id).disable();
            Ext.getCmp("DirEmployeeLink" + ctl.UO_id).disable();
            Ext.getCmp("DirWarehouseID" + ctl.UO_id).disable();
            //Ext.getCmp("RetailOnly" + ctl.UO_id).disable();
            Ext.getCmp("DirContractorIDOrg" + ctl.UO_id).disable();
        }
    },
    //Склад ***
    /*
    //Clear Bonus
    onBtnClearWarehouseClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirWarehouseID" + aButton.UO_id).setValue(0);
        //Ext.getCmp("RetailOnly" + aButton.UO_id).setValue(false);
        Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).setValue(0);
    },
    //Редактирование или добавление
    onBtnDirWarehouseEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirWarehouses", Params);
    },
    //РеЛоад - перегрузить Combo, что бы появились новые записи
    onBtnDirWarehouseReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirWarehousesGrid = Ext.getCmp(aButton.UO_idMain).storeDirWarehousesGrid;
        storeDirWarehousesGrid.load();
    },
    */

    onRetailOnlyChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            if (!Ext.getCmp("DirWarehouseID" + ctl.UO_id).getValue() || !Ext.getCmp("DirContractorIDOrg" + ctl.UO_id).getValue()) {
                //Ext.getCmp("RetailOnly" + ctl.UO_id).setValue(0);
                Ext.Msg.alert(lanOrgName, "Для доступа в интерфейс 'Розница' необходими выбрать Cклад и Организацию!");
            }
        }
        else {
            
        }
    },

    onDirContractorIDOrgSelect: function (combo, record) { 
    },
    onBtnClearContractorOrgClick: function (aButton, aEvent, aOptions) {
        //Ext.getCmp("RetailOnly" + aButton.UO_id).setValue(false);
        Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).setValue(0);
    },
    //Редактирование или добавление
    onBtnDirContractorOrgEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            undefined,
            undefined,
            undefined,
            undefined,
            "DirContractor2TypeID1=1"
        ]
        ObjectConfig("viewDirContractors", Params);
    },
    //РеЛоад - перегрузить Combo, что бы появились новые записи
    onBtnDirContractorOrgReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirContractorsOrgGrid = Ext.getCmp(aButton.UO_idMain).storeDirContractorsOrgGrid;
        storeDirContractorsOrgGrid.load();
    },
    onPanelGridEmployeeWarehouses_selectionchange: function (model, records) {
        /*
        //model.view.ownerGrid.down("#PanelGridEmployeeWarehouses_btnDelete").setDisabled(records.length === 0);
        

        //model.view.ownerGrid
        //Ext.getCmp("" + model.view.grid);
        if (records.length > 0) {
            Ext.getCmp("btnDelWarehouse" + model.view.ownerGrid.UO_id).setDisabled(false);
        }
        else {
            Ext.getCmp("btnDelWarehouse" + model.view.ownerGrid.UO_id).setDisabled(true);
        }
        */

        Ext.getCmp("btnDelWarehouse" + model.view.ownerGrid.UO_id).setDisabled(records.length === 0);
    },
    onBtnPanelGridEmployeeWarehouses_btnDelete: function (aButton, aEvent, aOptions) {
        var selection = Ext.getCmp("PanelGridEmployeeWarehouses_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
        if (selection) { Ext.getCmp("PanelGridEmployeeWarehouses_" + aButton.UO_id).store.remove(selection); }
    },
    onBtnAddWarehouse: function (aButton, aEvent, aOptions) {

        var store = Ext.getCmp("PanelGridEmployeeWarehouses_" + aButton.UO_id).store;

        //1. Проверка на задвоенность Склада
        //1.
        if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == null) { return; }
        //2.
        for (var i = 0; i < store.data.items.length; i++) {
            if (parseInt(store.data.items[i].data.DirWarehouseID) == parseInt(Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue())) {
                Ext.Msg.alert(lanOrgName, "Такая точка уже имеется в списке!");
                return;
            }
        }

        //Что бы отображать на русском
        //var IsAdminNameRu = "Нет";
        //if (Ext.getCmp("IsAdmin" + aButton.UO_id).getValue()) IsAdminNameRu = "Да";

        //2. Сохранение
        store.add(new store.model({
            DirWarehouseID: Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue(),
            DirWarehouseName: Ext.getCmp("DirWarehouseID" + aButton.UO_id).rawValue,

            IsAdmin: false, //IsAdmin: Ext.getCmp("IsAdmin" + aButton.UO_id).getValue(),
            IsAdminNameRu: "Не Администратор", //IsAdminNameRu: IsAdminNameRu

            WarehouseAll: false, //WarehouseAll: Ext.getCmp("WarehouseAll" + aButton.UO_id).getValue(),
            WarehouseAllNameRu: "Не Виден", //WarehouseAllNameRu: IsAdminNameRu

        }));

    },

    onRightDocServicePurchesWarehouseAllCheckChange: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            var storeGrid = Ext.getCmp("PanelGridEmployeeWarehouses_" + ctl.UO_id).store;
            for (var i = 0; i < storeGrid.data.length; i++) {
                storeGrid.data.items[i].data.WarehouseAll = true;
                storeGrid.data.items[i].data.WarehouseAllNameRu = "Виден";
            }
            Ext.getCmp("PanelGridEmployeeWarehouses_" + ctl.UO_id).getView().refresh();

            //Ext.Msg.alert(lanOrgName, "Сотрудник будет видить все отмеченные аппараты (в СЦ.Мастерская)!");
        }
        else {
            var storeGrid = Ext.getCmp("PanelGridEmployeeWarehouses_" + ctl.UO_id).store;
            for (var i = 0; i < storeGrid.data.length; i++) {
                storeGrid.data.items[i].data.WarehouseAll = false;
                storeGrid.data.items[i].data.WarehouseAllNameRu = "Не Виден";
            }
            Ext.getCmp("PanelGridEmployeeWarehouses_" + ctl.UO_id).getView().refresh();

            //Ext.Msg.alert(lanOrgName, "Сотрудник будет видить только ту точку на которую зашёл (в СЦ.Мастерская)!");
        }
    },



    // *** DirWarehouse ***
    // Селект Группы
    onCombotree_DirWarehouseName_selectionchange: function (model, records) {
        //alert("111")
    },
    // Клик по Группе
    onCombotree_DirWarehouseName_itemclick: function (view, rec, item, index, eventObj) {
        //alert("222")
        Ext.getCmp("DirWarehouseID" + view.grid.UO_id).setValue(rec.get('id'));
        Ext.getCmp("DirWarehouseName" + view.grid.UO_id).setValue(rec.get('text'));
    },
    // Дабл клик по Группе - не используется
    onCombotree_DirWarehouseName_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("333")
    },


    // === Salary-Panel === === ===

    // *** DirCurrency ***
    // Селект Группы
    onCombotree_DirCurrencyName_selectionchange: function (model, records) {
        //alert("111")
    },
    // Клик по Группе
    onCombotree_DirCurrencyName_itemclick: function (view, rec, item, index, eventObj) {
        //alert("222")
        Ext.getCmp("DirCurrencyID" + view.grid.UO_id).setValue(rec.get('id'));
        Ext.getCmp("DirCurrencyName" + view.grid.UO_id).setValue(rec.get('text'));
    },
    // Дабл клик по Группе - не используется
    onCombotree_DirCurrencyName_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("333")
    },


    // *** DirBonus ***
    // Селект Группы
    onCombotree_DirBonusName_selectionchange: function (model, records) {
        //alert("111")
    },
    // Клик по Группе
    onCombotree_DirBonusName_itemclick: function (view, rec, item, index, eventObj) {
        //alert("222")
        Ext.getCmp("DirBonusID" + view.grid.UO_id).setValue(rec.get('id'));
        Ext.getCmp("DirBonusName" + view.grid.UO_id).setValue(rec.get('text'));
    },
    // Дабл клик по Группе - не используется
    onCombotree_DirBonusName_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("333")
    },

    //Clear Bonus
    onBtnClearBonusClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirBonusID" + aButton.UO_id).setValue(0);
    },
    onBtnClearBonus2Click: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirBonus2ID" + aButton.UO_id).setValue(0);
    },
    onBtnClearBonus3Click: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirBonus3ID" + aButton.UO_id).setValue(0);
    },
    onBtnClearBonus4Click: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirBonus4ID" + aButton.UO_id).setValue(0);
    },


    // === Passport-Panel === === ===





    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Точки
        var recordsDirEmployeeWarehouses = [];
        if (Ext.getCmp("SysRecord" + aButton.UO_id).getValue() != true) {
            var storeEmployeeWarehouses = Ext.getCmp("PanelGridEmployeeWarehouses_" + aButton.UO_id).store;
            storeEmployeeWarehouses.data.each(function (rec) { recordsDirEmployeeWarehouses.push(rec.data); });
            if (storeEmployeeWarehouses.data.length == 0) { Ext.Msg.alert(lanOrgName, "Пожалусте, заполните вкладку Точки!"); return; }
        }
        
        //ЗП: Если выбрана сумма, то выбрать и Валюту
        if (Ext.getCmp("Salary" + aButton.UO_id).getValue() && !Ext.getCmp("DirCurrencyID" + aButton.UO_id).getValue()) {
            Ext.Msg.alert(lanOrgName, "Пожалусте, заполните вкладку Зарплата 'Валюта'! Валюта = " + Ext.getCmp("DirCurrencyID" + aButton.UO_id).getValue()); return;
        }

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirEmployees;
        if (parseInt(Ext.getCmp("DirEmployeeID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirEmployees + "?id=" + parseInt(Ext.getCmp("DirEmployeeID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            params: { recordsDirEmployeeWarehouses: Ext.encode(recordsDirEmployeeWarehouses) },

            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                //Ext.getCmp(aButton.UO_idMain).close();
                Ext.getCmp("tree_" + aButton.UO_id).getStore().load();
                //action.result.data.ID
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
        window.open(HTTP_Help + "spravochnik-sotrudniki/", '_blank');
    },
    onBtnHistoryClick: function (aButton, aEvent, aOptions) {
        var Params = [
            "viewDirEmployees" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            undefined, //UO_Function_Tree
            undefined, //UO_Function_Grid
            false,  //TreeShow
            true, //GridShow
            undefined,     //TreeServerParam1
            "DirEmployeeID=" + Ext.getCmp("DirEmployeeID" + aButton.UO_id).getValue()      //GridServerParam1
        ]
        ObjectConfig("viewDirEmployeeHistories", Params);
    },
});



// === Функции === === ===

function controllerDirEmployees_onTree_folderNew(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("tree_" + id).disable();
    //Значения по умолчанию
    Ext.getCmp("DirEmployeeActive" + id).enable();
    Ext.getCmp("Salary" + id).setValue(0);
    Ext.getCmp("SalaryDayMonthly" + id).setValue(1);
    Ext.getCmp("DirCurrencyID" + id).setValue(varDirCurrencyID);
    Ext.getCmp("SalaryPercentService1Tabs" + id).setValue(0);
    Ext.getCmp("SalaryPercentService2Tabs" + id).setValue(0);
    //Разблокировать Точки
    Ext.getCmp("PanelWarehouses_" + id).enable();
    Ext.getCmp("DirWarehouseID" + id).enable();
    Ext.getCmp("btnAddWarehouse" + id).enable();
};
function controllerDirEmployees_onTree_folderNewSub(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("AddSub" + id).setValue(true);
    var node = funReturnNode(id);
    Ext.getCmp("Sub" + id).setValue(node.data.id);

    Ext.getCmp("tree_" + id).disable();
};
function controllerDirEmployees_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                Ext.getCmp("DirEmployeeID" + id).setValue(0);
                Ext.getCmp("DirEmployeeName" + id).setValue(Ext.getCmp("DirEmployeeName" + id).getValue() + " (копия)");
                Ext.getCmp("tree_" + id).disable();
                //Разблокировать Точки
                Ext.getCmp("PanelWarehouses_" + id).enable();
                Ext.getCmp("DirWarehouseID" + id).enable();
                Ext.getCmp("btnAddWarehouse" + id).enable();
            }
        }
    });
};
function controllerDirEmployees_onTree_folderDel(id) {
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
                var DirEmployeeID = Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.id; //Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.DirEmployeeID;
                //Запрос на удаление
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirEmployees + DirEmployeeID + "/",
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
function controllerDirEmployees_onTree_folderSubNull(id) {
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
                    url: HTTP_DirEmployees + "?id=" + Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.id + "&sub=0", // + overModel.data.id,
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

