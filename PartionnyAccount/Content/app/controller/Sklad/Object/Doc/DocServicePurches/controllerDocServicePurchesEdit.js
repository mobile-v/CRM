Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocServicePurches/controllerDocServicePurchesEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocServicePurchesEdit': { close: this.this_close },

            //Сисок (itemId=tree)
            // Меню Списка
            'viewDocServicePurchesEdit [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDocServicePurchesEdit [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDocServicePurchesEdit [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDocServicePurchesEdit [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDocServicePurchesEdit [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDocServicePurchesEdit [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDocServicePurchesEdit [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDocServicePurchesEdit [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick,

                //itemcontextmenu: this.onTree_contextMenuForTreePanel,
            },

            'viewDocServicePurchesEdit dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },


            'viewDocServicePurchesEdit button#btnDirServiceNomenReload': { click: this.onBtnDirServiceNomenReloadClick },
            'viewDocServicePurchesEdit #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },


            //*** Не видимо *** *** *** *** *** *** *** *** *** *** *** ***
            //Мастер - Перегрузить
            'viewDocServicePurchesEdit [itemId=DirEmployeeIDMaster]': { select: this.onDirEmployeeIDMasterSelect },
            'viewDocServicePurchesEdit button#btnDirEmployeeEdit': { click: this.onBtnDirEmployeeEditClick },
            'viewDocServicePurchesEdit button#btnDirEmployeeReload': { click: this.onBtnDirEmployeeReloadClick },
            //Контрагент - Перегрузить
            'viewDocServicePurchesEdit [itemId=DirServiceContractorID]': { select: this.onDirServiceContractorIDSelect },
            'viewDocServicePurchesEdit button#btnDirServiceContractorEdit': { click: this.onBtnDirServiceContractorEditClick },
            'viewDocServicePurchesEdit button#btnDirServiceContractorReload': { click: this.onBtnDirServiceContractorReloadClick },
            //Склад - Перегрузить
            'viewDocServicePurchesEdit [itemId=DirWarehouseID]': { select: this.onDirWarehouseIDSelect },
            'viewDocServicePurchesEdit button#btnDirWarehouseEdit': { click: this.onBtnDirWarehouseEditClick },
            'viewDocServicePurchesEdit button#btnDirWarehouseReload': { click: this.onBtnDirWarehouseReloadClick },
            //Currencies - Перегрузить
            'viewDocServicePurchesEdit [itemId=DirCurrencyID]': { select: this.onDirCurrencyIDSelect },
            'viewDocServicePurchesEdit button#btnCurrencyEdit': { "click": this.onBtnCurrencyEditClick },
            'viewDocServicePurchesEdit button#btnCurrencyReload': { "click": this.onBtnCurrencyReloadClick },
            //*** Не видимо *** *** *** *** *** *** *** *** *** *** *** ***


            'viewDocServicePurchesEdit [itemId=UrgentRepairs]': { change: this.onUrgentRepairsChecked },



            //'viewDocServicePurchesEdit [itemId=ComponentPass]': { change: this.onComponentPassChecked },
            //'viewDocServicePurchesEdit [itemId=ComponentOther]': { change: this.onComponentOtherChecked },
            'viewDocServicePurchesEdit button#btnDirServiceComplectAdd': { "click": this.onBtnDirServiceComplectAddClick },

            'viewDocServicePurchesEdit button#btnDirServiceProblemAdd': { "click": this.onBtnDirServiceProblemAddClick },
            'viewDocServicePurchesEdit button#btnDirServiceProblemEdit': { click: this.onBtnDirServiceProblemEditClick },
            'viewDocServicePurchesEdit button#btnDirServiceProblemReload': { click: this.onBtnDirServiceProblemReloadClick },

            'viewDocServicePurchesEdit [itemId=DirServiceContractorAddress]': { change: this.onDirServiceContractorAddressChecked },
            'viewDocServicePurchesEdit [itemId=Prepayment]': { change: this.onPrepaymentChecked },

            'viewDocServicePurchesEdit [itemId=DirServiceComplectID]': { select: this.onDirServiceComplectIDSelect },

            'viewDocServicePurchesEdit [itemId=DirServiceProblemID]': { select: this.onDirServiceProblemIDSelect },



            //Телефон: нажатие Enter и Tab
            'viewDocServicePurchesEdit [itemId=DirServiceContractorPhone]': {
                specialkey: this.onDirServiceContractorPhoneSpecialkey,
                select: this.onDirServiceContractorPhoneSelect
            },


            'viewDocServicePurchesEdit [itemId=SerialNumberNo]': { change: this.onSerialNumberNoChecked },
            'viewDocServicePurchesEdit [itemId=ComponentPasTextNo]': { change: this.onComponentPasTextNoChecked },



            //Типовые неисправности
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault1ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault2ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault3ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault4ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault5ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault6ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault7ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault8ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault9ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault10ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault11ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault12ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault13ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            'viewDocServicePurchesEdit [itemId=DirServiceNomenTypicalFault14ID]': { change: this.onDirServiceNomenTypicalFault1IDChecked },
            



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            //'viewDocServicePurchesEdit menuitem#btnSave': { click: this.onBtnSaveClick },
            //'viewDocServicePurchesEdit menuitem#btnSaveClose': { click: this.onBtnSaveClick },
            'viewDocServicePurchesEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocServicePurchesEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocServicePurchesEdit button#btnHelp': { "click": this.onBtnHelpClick },

            //***
            'viewDocServicePurchesEdit menuitem#btnPrintHtml': { click: this.onBtnPrintHtmlClick },
            'viewDocServicePurchesEdit menuitem#btnPrintExcel': { click: this.onBtnPrintHtmlClick },

            'viewDocServicePurchesEdit menuitem#btnPrint_barcode': { "click": this.onBtnPrintClick },
            'viewDocServicePurchesEdit menuitem#btnPrint_barcode_price': { "click": this.onBtnPrintClick },
            'viewDocServicePurchesEdit menuitem#btnPrint_barcode_name': { "click": this.onBtnPrintClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },

    
    
    //Обновить список Товаров
    onBtnDirServiceNomenReloadClick: function (aButton, aEvent, aOptions) {
        //var storeDirServiceNomensTree = Ext.getCmp(aButton.UO_idMain).storeDirServiceNomensTree;
        var storeDirServiceNomensTree = Ext.getCmp("tree_" + aButton.UO_id).store;
        storeDirServiceNomensTree.load();
    },

    //Поиск
    onTriggerSearchTreeClick1: function (aButton, aEvent) {
        controllerDirServiceNomens_onTriggerSearchTreeClick_Search(aButton, false);
    },
    onTriggerSearchTreeClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            controllerDirServiceNomens_onTriggerSearchTreeClick_Search(f, false);
        }
    },
    onTriggerSearchTreeClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) {
            //funGridDir(e.UO_id, textReal, HTTP_DirServiceNomens);
            //alert("В стадии разработки ...");
        }
    },



    // Группа (itemId=tree) === === === === === === === === === ===

    //Меню Группы
    onTree_expandAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).expandAll();
    },
    onTree_collapseAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).collapseAll();
    },

    onTree_folderNew: function (aButton, aEvent) {
        controllerDocServicePurchesEdit_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDocServicePurchesEdit_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDocServicePurchesEdit_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDocServicePurchesEdit_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDocServicePurchesEdit_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);

        if (records.length == 0) { return }

        //ID-шник
        var id = model.view.grid.UO_id;

        //Полный путь от Группы к выбранному объкту
        Ext.getCmp("DirServiceNomenPatchFull" + id).setText("<b style='color: red'>" + records[0].data.DirServiceNomenPatchFull + "<b>", false);
        Ext.getCmp("DirServiceNomenPatchFull" + id).UO_Text = records[0].data.DirServiceNomenPatchFull;
        //Аппарат
        Ext.getCmp("DirServiceNomenID" + id).setValue(records[0].data.id);
        Ext.getCmp("DirServiceNomenName" + id).setValue(records[0].data.text);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        //ID-шник
        var id = view.grid.UO_id;
        
        //Полный путь от Группы к выбранному объкту
        Ext.getCmp("DirServiceNomenPatchFull" + id).setText("<b style='color: red'>" + rec.get('DirServiceNomenPatchFull') + "<b>", false); //Ext.getCmp("DirServiceNomenPatchFull" + id).setValue(rec.get('DirServiceNomenPatchFull'));
        Ext.getCmp("DirServiceNomenPatchFull" + id).UO_Text = rec.get('DirServiceNomenPatchFull');
        //Аппарат
        //if (Ext.getCmp("DirServiceNomenID" + id).getValue() > 0) { Ext.Msg.alert(lanOrgName, "Вы сменили Аппарат с " + Ext.getCmp("DirServiceNomenName" + id).getValue() + " на " + rec.get('text')); }
        Ext.getCmp("DirServiceNomenID" + id).setValue(rec.get('id'));
        Ext.getCmp("DirServiceNomenName" + id).setValue(rec.get('text'));


        //1. === Типовые неисправности ===

        //Де-активировать
        
        for (var i = 1; i <= 14; i++) {
            Ext.getCmp("DirServiceNomenTypicalFault" + i + "ID" + id).setValue(false);
            Ext.getCmp("DirServiceNomenTypicalFault" + i + "ID" + id).setDisabled(true);
            Ext.getCmp("DirServiceNomenTypicalFault" + i + "ID" + id).setBoxLabel(" нет ");
        }

        //Запрос на сервер за списком цен
        /*
        var storeDirServiceNomenPricesGrid = Ext.create("store.storeDirServiceNomenPricesGrid");
        storeDirServiceNomenPricesGrid.setData([], false);
        storeDirServiceNomenPricesGrid.proxy.url = HTTP_DirServiceNomenPrices + "?type=Grid&DirServiceNomenID=" + rec.get('id');

        var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("form_" + id) }); loadingMask.show();
        storeDirServiceNomenPricesGrid.load({ waitMsg: lanLoading });
        storeDirServiceNomenPricesGrid.on('load', function () {
            loadingMask.hide();
            
            for (var i = 0; i < storeDirServiceNomenPricesGrid.data.length; i++) {
                Ext.getCmp("DirServiceNomenTypicalFaultID" + storeDirServiceNomenPricesGrid.data.items[i].data.DirServiceNomenTypicalFaultID + id).UO_Price = storeDirServiceNomenPricesGrid.data.items[i].data.PriceVAT;
                Ext.getCmp("DirServiceNomenTypicalFaultID" + storeDirServiceNomenPricesGrid.data.items[i].data.DirServiceNomenTypicalFaultID + id).setDisabled(false);
                Ext.getCmp("DirServiceNomenTypicalFaultID" + storeDirServiceNomenPricesGrid.data.items[i].data.DirServiceNomenTypicalFaultID + id).setBoxLabel(storeDirServiceNomenPricesGrid.data.items[i].data.DirServiceNomenTypicalFaultName + " - " + storeDirServiceNomenPricesGrid.data.items[i].data.PriceVAT + " " + varDirCurrencyNameShort);
            }

        });
        */
        
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DirServiceNomens + rec.get('id') + "/",
            method: 'GET',
            success: function (result) {

                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    let sErr = sData;
                    if (sData.data) sErr = sData.data;
                    funPanelSubmitFailure(sErr);
                }
                else {

                    if (sData.data.Faults1Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault1ID" + id).UO_Price = sData.data.Faults1Price;
                        Ext.getCmp("DirServiceNomenTypicalFault1ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault1ID" + id).setBoxLabel(Fault1Name + " - " + sData.data.Faults1Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults2Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault2ID" + id).UO_Price = sData.data.Faults2Price;
                        Ext.getCmp("DirServiceNomenTypicalFault2ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault2ID" + id).setBoxLabel(Fault2Name + " - " + sData.data.Faults2Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults3Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault3ID" + id).UO_Price = sData.data.Faults3Price;
                        Ext.getCmp("DirServiceNomenTypicalFault3ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault3ID" + id).setBoxLabel(Fault3Name + " - " + sData.data.Faults3Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults4Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault4ID" + id).UO_Price = sData.data.Faults4Price;
                        Ext.getCmp("DirServiceNomenTypicalFault4ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault4ID" + id).setBoxLabel(Fault4Name + " - " + sData.data.Faults4Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults5Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault5ID" + id).UO_Price = sData.data.Faults5Price;
                        Ext.getCmp("DirServiceNomenTypicalFault5ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault5ID" + id).setBoxLabel(Fault5Name + " - " + sData.data.Faults5Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults6Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault6ID" + id).UO_Price = sData.data.Faults6Price;
                        Ext.getCmp("DirServiceNomenTypicalFault6ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault6ID" + id).setBoxLabel(Fault6Name + " - " + sData.data.Faults61Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults7Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault7ID" + id).UO_Price = sData.data.Faults7Price;
                        Ext.getCmp("DirServiceNomenTypicalFault7ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault7ID" + id).setBoxLabel(Fault7Name + " - " + sData.data.Faults7Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults8Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault8ID" + id).UO_Price = sData.data.Faults8Price;
                        Ext.getCmp("DirServiceNomenTypicalFault8ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault8ID" + id).setBoxLabel(Fault8Name + " - " + sData.data.Faults8Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults9Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault9ID" + id).UO_Price = sData.data.Faults9Price;
                        Ext.getCmp("DirServiceNomenTypicalFault9ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault9ID" + id).setBoxLabel(Fault9Name + " - " + sData.data.Faults9Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults10Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault10ID" + id).UO_Price = sData.data.Faults10Price;
                        Ext.getCmp("DirServiceNomenTypicalFault10ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault10ID" + id).setBoxLabel(Fault10Name + " - " + sData.data.Faults10Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults11Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault11ID" + id).UO_Price = sData.data.Faults11Price;
                        Ext.getCmp("DirServiceNomenTypicalFault11ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault11ID" + id).setBoxLabel(Fault11Name + " - " + sData.data.Faults11Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults12Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault12ID" + id).UO_Price = sData.data.Faults12Price;
                        Ext.getCmp("DirServiceNomenTypicalFault12ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault12ID" + id).setBoxLabel(Fault12Name + " - " + sData.data.Faults12Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults13Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault13ID" + id).UO_Price = sData.data.Faults13Price;
                        Ext.getCmp("DirServiceNomenTypicalFault13ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault13ID" + id).setBoxLabel(Fault13Name + " - " + sData.data.Faults13Price + " " + varDirCurrencyNameShort);
                    }
                    if (sData.data.Faults14Check) {
                        Ext.getCmp("DirServiceNomenTypicalFault14ID" + id).UO_Price = sData.data.Faults14Price;
                        Ext.getCmp("DirServiceNomenTypicalFault14ID" + id).setDisabled(false);
                        Ext.getCmp("DirServiceNomenTypicalFault14ID" + id).setBoxLabel(Fault14Name + " - " + sData.data.Faults14Price + " " + varDirCurrencyNameShort);
                    }

                }

            },
            failure: function (result) {
                funPanelSubmitFailure(form, action);
            }
        });

    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
    },


    //beforedrop
    onTree_beforedrop: function (node, data, overModel, dropPosition, dropPosition1, dropPosition2, dropPosition3) {
        fun_Nods_Drop_Down(HTTP_DirServiceNomens, node, data, overModel, dropPosition, dropPosition1, dropPosition2, dropPosition3);
        return;
    },
    //drop
    onTree_drop: function (node, data, overModel, dropPosition) {
        //Ext.Msg.alert("Группа перемещена!");
    },



    // *** DirEmployee ***
    //Редактирование или добавление нового Склада
    onDirEmployeeIDMasterSelect: function (combo, records) {
        var tree_ = Ext.getCmp("tree_" + combo.UO_id);
        tree_.store.proxy.url = HTTP_DirServiceNomens + "?DirEmployeeID=" + records.data.DirEmployeeID;
    },
    onBtnDirEmployeeEditClick: function (combo, aEvent, aOptions) {
        var Params = [
            combo.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirEmployees", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirEmployeeReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirEmployeesGrid = Ext.getCmp(aButton.UO_idMain).storeDirEmployeesGrid;
        storeDirEmployeesGrid.load();
    },

    // *** DirServiceContractorName ***
    //Редактирование или добавление нового Поставщика
    onDirServiceContractorIDSelect: function (combo, records) {
        Ext.getCmp("DirServiceContractorName" + combo.UO_id).setValue(records.data.DirServiceContractorName);
        Ext.getCmp("DirServiceContractorAddress" + combo.UO_id).setValue(records.data.DirServiceContractorAddress);
        Ext.getCmp("DirServiceContractorPhone" + combo.UO_id).setValue(records.data.DirServiceContractorPhone);
        Ext.getCmp("DirServiceContractorEmail" + combo.UO_id).setValue(records.data.DirServiceContractorEmail);
    },
    onBtnDirServiceContractorEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirServiceContractors", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirServiceContractorReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirServiceContractorsGrid = Ext.getCmp(aButton.UO_idMain).storeDirServiceContractorsGrid;
        storeDirServiceContractorsGrid.load();
    },

    // *** DirWarehouse ***
    //Редактирование или добавление нового Склада
    onDirWarehouseIDSelect: function (combo, records) {
        var tree_ = Ext.getCmp("tree_" + combo.UO_id);
        tree_.store.proxy.url = HTTP_DirServiceNomens + "?DirWarehouseID=" + records.data.DirWarehouseID;
    },
    onBtnDirWarehouseEditClick: function (combo, aEvent, aOptions) {
        var Params = [
            combo.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirWarehouses", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirWarehouseReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirWarehousesGrid = Ext.getCmp(aButton.UO_idMain).storeDirWarehousesGrid;
        storeDirWarehousesGrid.load();
    },

    // *** DirCurrency ***
    onDirCurrencyIDSelect: function (combo, records) { //aButton, aEvent, aOptions
        //Запрос на сервер за курсом м кратностью
        Ext.Msg.show({
            title: lanOrgName,
            msg: "Изменить Курс и Кратность?",
            buttons: Ext.Msg.YESNO,
            fn: function (btn) {
                if (btn == "yes") {
                    Ext.getCmp("DirCurrencyRate" + combo.UO_id).setValue(records.data.DirCurrencyRate);
                    Ext.getCmp("DirCurrencyMultiplicity" + combo.UO_id).setValue(records.data.DirCurrencyMultiplicity);
                }
            },
            icon: Ext.window.MessageBox.QUESTION
        });
    },
    onBtnCurrencyEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCurrencies", Params);
    },
    onBtnCurrencyReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCurrenciesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCurrenciesGrid;
        storeDirCurrenciesGrid.load();
    },

    onUrgentRepairsChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("DirServiceContractorName" + ctl.UO_id).disable();
        }
        else {
            Ext.getCmp("DirServiceContractorName" + ctl.UO_id).enable();
        }
    },


    onBtnDirServiceComplectAddClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("ComponentOtherText" + aButton.UO_id).setValue(Ext.getCmp("DirServiceComplectID" + aButton.UO_id).getRawValue());
    },

    onBtnDirServiceProblemAddClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("ProblemClientWords" + aButton.UO_id).setValue(Ext.getCmp("DirServiceProblemID" + aButton.UO_id).getRawValue());
    },
    onBtnDirServiceProblemEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirServiceProblems", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirServiceProblemReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirServiceProblemsGrid = Ext.getCmp(aButton.UO_idMain).storeDirServiceProblemsGrid;
        storeDirServiceProblemsGrid.load();
    },

    onDirServiceContractorAddressChecked: function (field, newVal, oldVal) { //ctl.UO_id
        var myNumber = newVal.replace(/\D/g, '');
        Ext.getCmp("DirServiceContractorPhone" + field.UO_id).setValue(myNumber);
    },

    onPrepaymentChecked: function (ctl, val) { //ctl.UO_id
        if (val) {
            Ext.getCmp("PrepaymentSum" + ctl.UO_id).setVisible(true);
        }
        else {
            Ext.getCmp("PrepaymentSum" + ctl.UO_id).setVisible(false);
        }
    },

    onDirServiceComplectIDSelect: function (combo, records) {
        Ext.getCmp("ComponentOtherText" + combo.UO_id).setValue(records.data.DirServiceComplectName);
    },

    onDirServiceProblemIDSelect: function (combo, records) {
        
        if (Ext.getCmp("ProblemClientWords" + combo.UO_id).getValue().length > 0) {
            Ext.getCmp("ProblemClientWords" + combo.UO_id).setValue
            (
                Ext.getCmp("ProblemClientWords" + combo.UO_id).getValue() + ", "
            );
        }

        Ext.getCmp("ProblemClientWords" + combo.UO_id).setValue(Ext.getCmp("ProblemClientWords" + combo.UO_id).getValue() + " " + records.data.DirServiceProblemName);
    },




    //Телефон: нажатие Enter и Tab
    onDirServiceContractorPhoneSpecialkey: function (textfield, e) {

        if (e.getKey() == e.ENTER || e.getKey() == e.TAB) {

            /*
            if (!Ext.getCmp("DirServiceContractorPhone" + textfield.UO_id).value) return;

            //Запрос на сервер за: ФИО, К-во (успеш, не успеш и общее) ремонтов
            Ext.Ajax.request({
                timeout: varTimeOutDefault,
                waitMsg: lanUpload,
                url: HTTP_DirServiceContractors + "0/?parSearch=" + encodeURIComponent(Ext.getCmp("DirServiceContractorPhone" + textfield.UO_id).value),
                method: 'GET',
                success: function (result) {
                    var sData = Ext.decode(result.responseText);
                    if (sData.success == true) {
                        Ext.getCmp("DirServiceContractorID" + textfield.UO_id).setValue(parseInt(sData.data.DirServiceContractorID));
                        Ext.getCmp("DirServiceContractorName" + textfield.UO_id).setValue(sData.data.DirServiceContractorName);
                        Ext.getCmp("QuantityOk" + textfield.UO_id).setValue(sData.data.QuantityOk);
                        Ext.getCmp("QuantityFail" + textfield.UO_id).setValue(sData.data.QuantityFail);
                        Ext.getCmp("QuantityCount" + textfield.UO_id).setValue(sData.data.QuantityCount);
                    } else {
                        //Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
                    }
                },
                failure: function (form, action) {
                    funPanelSubmitFailure(form, action);
                }
            });
            */

            //controllerDocServicePurchesEdit_onDirServiceContractorPhoneX(textfield.UO_id);


            //Только если Ентер (при Таб перескочит через )
            //if (e.getKey() == e.ENTER) Ext.getCmp("DirServiceContractorName" + textfield.UO_id).focus();
        }
    },

    onDirServiceContractorPhoneSelect: function (combo, records, eOpts) {
        /*
        if (!Ext.getCmp("DirServiceContractorPhone" + combo.UO_id).value) return;

        //Запрос на сервер за: ФИО, К-во (успеш, не успеш и общее) ремонтов
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DirServiceContractors + "0/?parSearch=" + encodeURIComponent(Ext.getCmp("DirServiceContractorPhone" + combo.UO_id).value),
            method: 'GET',
            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == true) {
                    Ext.getCmp("DirServiceContractorID" + combo.UO_id).setValue(parseInt(sData.data.DirServiceContractorID));
                    Ext.getCmp("DirServiceContractorName" + combo.UO_id).setValue(sData.data.DirServiceContractorName);
                    Ext.getCmp("QuantityOk" + combo.UO_id).setValue(sData.data.QuantityOk);
                    Ext.getCmp("QuantityFail" + combo.UO_id).setValue(sData.data.QuantityFail);
                    Ext.getCmp("QuantityCount" + combo.UO_id).setValue(sData.data.QuantityCount);
                } else {
                    //Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
                }
            },
            failure: function (form, action) {
                funPanelSubmitFailure(form, action);
            }
        });
        */

        controllerDocServicePurchesEdit_onDirServiceContractorPhoneX(combo.UO_id);
    },



    onSerialNumberNoChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("SerialNumber" + ctl.UO_id).allowBlank = true;
            Ext.getCmp("SerialNumber" + ctl.UO_id).disable();
        }
        else {
            Ext.getCmp("SerialNumber" + ctl.UO_id).allowBlank = false;
            Ext.getCmp("SerialNumber" + ctl.UO_id).enable();
        }
    },

    onComponentPasTextNoChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.getCmp("ComponentPasText" + ctl.UO_id).allowBlank = true;
            Ext.getCmp("ComponentPasText" + ctl.UO_id).disable();
        }
        else {
            Ext.getCmp("ComponentPasText" + ctl.UO_id).allowBlank = false;
            Ext.getCmp("ComponentPasText" + ctl.UO_id).enable();
        }
    },



    //Типовые неисправности
    onDirServiceNomenTypicalFault1IDChecked: function (ctl, val) {
        //Чистим Неисправнсти со слов клиента
        Ext.getCmp("ProblemClientWords" + ctl.UO_id).setValue("");

        //
        var UO_Price = 0;
        for (var i = 0; i < varStoreDirServiceNomenTypicalFaultsGrid.data.length; i++) {
            var DirServiceNomenTypicalFaultIDX = Ext.getCmp("DirServiceNomenTypicalFault" + parseInt(i + 1) + "ID" + ctl.UO_id);
            if (DirServiceNomenTypicalFaultIDX.checked) {
                UO_Price += DirServiceNomenTypicalFaultIDX.UO_Price;
                Ext.getCmp("ProblemClientWords" + ctl.UO_id).setValue(Ext.getCmp("ProblemClientWords" + ctl.UO_id).getValue() + " " + DirServiceNomenTypicalFaultIDX.boxLabel);
            }
        }
        Ext.getCmp("PriceVAT" + ctl.UO_id).setValue(UO_Price);

    },




    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#btnGridEdit").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnGridDelete").setDisabled(records.length === 0);
    },
    //Клик: Редактирования или выбор
    onGrid_itemclick: function (view, record, item, index, eventObj) {
        //Если запись удалена, то выдать сообщение и выйти
        if (funGridRecordDel(record)) { return; }

        if (varSelectOneClick) {
            if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
                var Params = [
                    view.grid.id, //UO_idCallб //"grid_" + aButton.UO_id, //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    2,    // 1 - Новое, 2 - Редактировать
                    true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                    index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    record,       // Для загрузки данных в форму Б.С. и Договора,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    true
                ]
                ObjectEditConfig("viewDocServicePurchTabsEdit", Params);
            }
            else {
                Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
                Ext.getCmp(view.grid.UO_idMain).close();
            }
        }
    },
    //ДаблКлик: Редактирования или выбор
    onGrid_itemdblclick: function (view, record, item, index, e) {
        if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
            var Params = [
                view.grid.id, //UO_idCall
                true, //UO_Center
                true, //UO_Modal
                2,    // 1 - Новое, 2 - Редактировать
                true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                record,        // Для загрузки данных в форму Б.С. и Договора,
                undefined,
                undefined,
                undefined,
                undefined,
                undefined,
                undefined,
                true
            ]
            ObjectEditConfig("viewDocServicePurchTabsEdit", Params);
        }
        else {
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }
    },


    // === Кнопки === === ===
    //Сохранить или Сохранить и закрыть
    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //Алгоритм:
        //1. Если надо печатать на ККМ, 
        //   1.1. то печатаем сначала на ККМ, передавая 2 параметра aButton и controllerDocServicePurchesEdit_onBtnSaveClick_Union
        //   1.2. если улачная печать на ККМ, то функция вызывает функцию controllerDocServicePurchesEdit_onBtnSaveClick_Union (но, как передать в неё aButton ... ?)
        //2. Иначе просто сохраняем в БД
        
        //Проверка: "Срочный ремонт"
        if (varPhoneNumberBegin == parseInt(Ext.getCmp("DirServiceContractorPhone" + aButton.UO_id).getValue()) && !Ext.getCmp("UrgentRepairs" + aButton.UO_id).getValue()) { Ext.Msg.alert(lanOrgName, "Вы не ввели номер телефона!<br />Введите номер телефона или установите переключатель 'Срочный ремонт'!"); return; }
        if (!Ext.getCmp("PriceVAT" + aButton.UO_id).getValue()) { Ext.Msg.alert(lanOrgName, "Заполните поле 'Ориентир.стоимость'!"); return; }

        var PrepaymentSum = parseFloat(Ext.getCmp("PrepaymentSum" + aButton.UO_id).getValue());
        if (PrepaymentSum == 0 || isNaN(PrepaymentSum)) {

            Ext.MessageBox.show({
                icon: Ext.MessageBox.WARNING, //ERROR,INFO,QUESTION,WARNING
                width: 300,
                title: lanOrgName,
                msg: "Вы не заполнили поле <b style='color: red;'>предоплата</b>!",
                buttonText: { yes: "Заполнить", no: "Не заполнять", cancel: "Отмена" },
                fn: function (btn) {
                    if (btn == "yes") {
                        Ext.getCmp("PrepaymentSum" + aButton.UO_id).focus();
                        return;
                    }
                    else if (btn == "no") {

                        if (varPayType == 0) {
                            Ext.MessageBox.show({
                                icon: Ext.MessageBox.QUESTION,
                                width: 300,
                                title: lanOrgName,
                                msg: "Выбирите <b>Тип оплаты</b>!",
                                buttonText: { yes: "Наличная", no: "Безналичная", cancel: "Отмена" },
                                fn: function (btn) {
                                    if (btn == "yes") {
                                        Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                                        controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
                                    }
                                    else if (btn == "no") {
                                        Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                                        controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
                                    }
                                }
                            });
                        }
                        else if (varPayType == 1) {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                            controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
                        }
                        else if (varPayType == 2) {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                            controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
                        }

                    }
                }
            });
        }
        else {

            if (varPayType == 0) {
                Ext.MessageBox.show({
                    icon: Ext.MessageBox.QUESTION,
                    width: 300,
                    title: lanOrgName,
                    msg: 'Выбирите Тип оплаты!',
                    buttonText: { yes: "Наличная", no: "Безналичная", cancel: "Отмена" },
                    fn: function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                            controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
                        }
                        else if (btn == "no") {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                            controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
                        }
                    }
                });
            }
            else if (varPayType == 1) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
            }
            else if (varPayType == 2) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
            }

        }
    },
    //Отменить
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    //Help
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-servis/", '_blank');
    },
    //***
    //Распечатать
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

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
            40, //ПФ == СС
            undefined,
            31 //ListObjectPFID
        ]
        ObjectConfig("viewListObjectPFs", Params);

    },
    onBtnPrintClick: function (aButton, aEvent, aOptions) {

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

        var mapForm = document.createElement("form");
        mapForm.target = "Map";
        mapForm.method = "GET"; // or "post" if appropriate
        mapForm.action = "../report/report/";

        //var UO_id = Ext.getCmp(aButton.UO_idCall).UO_id;

        //Параметр "pID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pID"; mapInput.value = "DocPurchesPrintCode"; mapForm.appendChild(mapInput);

        //Параметр "DirNomenID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DirNomenID"; mapInput.value = Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "DirNomenName"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DirNomenName"; mapInput.value = ""; //Ext.getCmp("DirServiceNomenPatchFull" + aButton.UO_id).html; mapForm.appendChild(mapInput);

        //Параметр "PriceRetailCurrency"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "PriceRetailCurrency"; mapInput.value = Ext.getCmp("PriceVAT" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "UO_Action"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "UO_Action"; mapInput.value = aButton.UO_Action; mapForm.appendChild(mapInput);


        document.body.appendChild(mapForm);
        map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

        if (map) { mapForm.submit(); }
        else { alert('You must allow popups for this map to work.'); }

    },
});


//Функия выбора контрагента
function controllerDocServicePurchesEdit_onDirServiceContractorPhoneX(UO_id) {

    if (!Ext.getCmp("DirServiceContractorPhone" + UO_id).value) return;

    //Запрос на сервер за: ФИО, К-во (успеш, не успеш и общее) ремонтов
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DirServiceContractors + "0/?parSearch=" + encodeURIComponent(Ext.getCmp("DirServiceContractorPhone" + UO_id).value),
        method: 'GET',
        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == true) {
                Ext.getCmp("DirServiceContractorID" + UO_id).setValue(parseInt(sData.data.DirServiceContractorID));
                Ext.getCmp("DirServiceContractorName" + UO_id).setValue(sData.data.DirServiceContractorName);
                Ext.getCmp("QuantityOk" + UO_id).setValue(sData.data.QuantityOk);
                Ext.getCmp("QuantityFail" + UO_id).setValue(sData.data.QuantityFail);
                Ext.getCmp("QuantityCount" + UO_id).setValue(sData.data.QuantityCount);



                //Получаем все ремонты "QuantityOk"
                var DirServiceNomenID = 0;
                var node = Ext.getCmp("tree_" + UO_id).getSelectionModel().getSelection();
                if (node && node[0]) {
                    DirServiceNomenID = node[0].data.id;
                }
                /*else {
                    Ext.MessageBox.show({ title: lanOrgName, msg: "Чтобы получить список предыдущих ремонтов, нужно сначала выбрать аппарат, а после ввести номер клиента!", icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
                    return;
                }*/
                //if (DirServiceNomenID == 0) { return; }

                //Формируем поиск: 
                //"&DirServiceNomenID=" + DirServiceNomenID + "&parSearch="
                //
                
                if (sData.data.QuantityOk > 0) {

                    //Открытие списка ПФ
                    var Params = [
                        UO_id,
                        true, //UO_Center
                        true, //UO_Modal
                        undefined, //UO_Function_Tree: Html или Excel
                        undefined,
                        undefined,
                        undefined,
                        sData.data.DirServiceContractorID,
                        DirServiceNomenID
                    ]
                    ObjectConfig("viewDocServicePurchesSelect", Params);

                }

            } else {
                //Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
            }
        },
        failure: function (form, action) {
            funPanelSubmitFailure(form, action);
        }
    });

};



//Функия сохранения (тут много ньюансов)
//- ККМ
//- Сохранение

//Запускается при нажатии на кнопку "Провести"
function controllerDocServicePurchesEdit_onBtnSaveClick_Union(aButton) {

    //Проверка
    if (Ext.getCmp("DirServiceNomenID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите аппарат!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите склад!"); return; }

    //KKM
    if (varKKMSActive) {
        Ext.Msg.confirm("Confirmation", "Печатать чек на ККМ?", function (btnText) {
            if (btnText === "no") {

                controllerDocServicePurchesEdit_onBtnSaveClick(aButton);

                //Закрыть
                //Ext.getCmp(aButton.UO_idMain).close();
            }
            else if (btnText === "yes") {
                
                //Параметры формы
                var
                    SubReal = 1 * (parseFloat(Ext.getCmp("PrepaymentSum" + aButton.UO_id).getValue() - 0)),
                    SumNal = 0,
                    SumBezNal = 0;
                if (parseInt(Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).getValue()) == 1) {
                    SumNal = SubReal;
                }
                else {
                    SumBezNal = SubReal;
                }

                var FormData =
                    {
                        DirNomenName: Ext.getCmp("DirServiceNomenPatchFull" + aButton.UO_id).UO_Text,
                        PriceCurrency: parseFloat(Ext.getCmp("PrepaymentSum" + aButton.UO_id).getValue()),
                        Amount: SubReal,
                        Quantity: 1, //parseFloat(Ext.getCmp("Quantity" + aButton.UO_id).getValue()),
                        SumNal: SumNal,
                        SumBezNal: SumBezNal,
                        KKMSPhone: Ext.getCmp("DirServiceContractorPhone" + aButton.UO_id).getValue(),
                        KKMSEMail: "",

                        SignMethodCalculation: 3,  //АВАНС
                        SignCalculationObject: 10, //ПЛАТЕЖ
                    };


                RegisterCheck(aButton, FormData, controllerDocServicePurchesEdit_onBtnSaveClick_Kkm, 0, false);
            }
        }, this);
    }
    else {
        controllerDocServicePurchesEdit_onBtnSaveClick(aButton);
    }
}

//Запускается после печати чека (надо получить CheckNumber)
function controllerDocServicePurchesEdit_onBtnSaveClick_Kkm(Rezult, textStatus, jqXHR, aButton) {
    
    if (Rezult.Status == 0) {

        if (Ext.getCmp("KKMSCheckNumber" + aButton.UO_id)) {
            //Получаем "CheckNumber" и пишем его в поле
            Ext.getCmp("KKMSCheckNumber" + aButton.UO_id).setValue(Rezult.CheckNumber);
            Ext.getCmp("KKMSIdCommand" + aButton.UO_id).setValue(Rezult.IdCommand);

            //Сохраняем в БД
            controllerDocServicePurchesEdit_onBtnSaveClick(aButton);
        }

    }
    else {
        //----------------------------------------------------------------------
        // ОБЩЕЕ
        //----------------------------------------------------------------------
        var Responce = "";
        //document.getElementById('Slip').textContent = "";

        if (Rezult.Status == 0) {
            Responce = Responce + "Статус: " + "Ok" + "\r\n";
        } else if (Rezult.Status == 1) {
            Responce = Responce + "Статус: " + "Выполняется" + "\r\n";
        } else if (Rezult.Status == 2) {
            Responce = Responce + "Статус: " + "Ошибка!" + "\r\n";
        } else if (Rezult.Status == 3) {
            Responce = Responce + "Статус: " + "Данные не найдены!" + "\r\n";
        };

        // Текст ошибки
        if (Rezult.Error != undefined && Rezult.Error != "") {
            Responce = Responce + "Ошибка: " + Rezult.Error + "\r\n";
        }

        if (Rezult != undefined) {
            var JSon = JSON.stringify(Rezult, "", 4);
            Responce = Responce + "JSON ответа: \r\n" + JSon + "\r\n";
            if (Rezult.Slip != undefined) {
                //document.getElementById('Slip').textContent = Rezult.Slip;
                alert(Rezult.Slip);
            }
        }

        //document.getElementById('Responce').textContent = Responce;
        //$(".Responce").text(Responce);
        alert(Responce);

    }

}

//Функия сохранения
function controllerDocServicePurchesEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    //Проверка
    //if (Ext.getCmp("DirServiceNomenID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите аппарат!"); return; }
    //if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите склад!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocServicePurches + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocServicePurchID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocServicePurches + "?id=" + parseInt(Ext.getCmp("DocServicePurchID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }



    var map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");


    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl + "&iTypeService=1",
        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {
           
            Ext.getCmp("btnSave" + aButton.UO_id).setVisible(false);
            Ext.getCmp("btnPrint" + aButton.UO_id).setVisible(true);

            //Если новая накладная присваиваем полученные номера!
            if (!Ext.getCmp('DocID' + aButton.UO_id).getValue()) {
                var sData = action.result.data;
                Ext.getCmp('DocID' + aButton.UO_id).setValue(sData.DocID);
                Ext.getCmp('DocServicePurchID' + aButton.UO_id).setValue(sData.DocServicePurchID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocServicePurchID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocServicePurchID);
            }

            //Закрыть
            if (aButton.UO_Action == "save_close") { Ext.getCmp(aButton.UO_idMain).close(); }
            //Перегрузить грид, если грид открыт
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }



            //Выводим ПФ: Квитанция
            controllerListObjectPFs_onGrid_itemclick(
                map,
                aButton.UO_id,
                40,
                31,
                Ext.getCmp("DocID" + aButton.UO_id).getValue(),
                "Html"
            );

            Ext.getCmp("viewDocServicePurchesEdit" + aButton.UO_id).close();

            //var activeTab = Ext.getCmp("viewContainerCentralTab").getActiveTab();
            //var activeTabIndex = tabPanel.items.findIndex('id', activeTab.id);
            //tabPanel.remove(activeTab);

        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};



// === Функции === === ===
//1. Для Товара - КонтекстМеню
function controllerDocServicePurchesEdit_onTree_folderNew(id) {
    var Params = [
        "tree_" + id,
        true, //UO_Center
        true, //UO_Modal
        1,    // 1 - Новое, 2 - Редактировать
        false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
        undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
        undefined, // Для загрузки данных в форму Б.С. и Договора,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        false      //GridTree
    ]
    ObjectEditConfig("viewDirServiceNomensWinEdit", Params);
};
function controllerDocServicePurchesEdit_onTree_folderEdit(id) {
    var Params = [
        "tree_" + id,
        true, //UO_Center
        true, //UO_Modal
        2,    // 1 - Новое, 2 - Редактировать
        false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
        undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
        undefined, // Для загрузки данных в форму Б.С. и Договора,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        false      //GridTree
    ]
    ObjectEditConfig("viewDirServiceNomensWinEdit", Params);
};
function controllerDocServicePurchesEdit_onTree_folderNewSub(id) {
    var node = funReturnNode(id);
    //Ext.getCmp("Sub" + id).setValue(node.data.id);

    var Sub = node.data.id;

    var Params = [
        "tree_" + id,
        true,  //UO_Center
        true,  //UO_Modal
        1,     // 1 - Новое, 2 - Редактировать
        false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
        Sub,   //Sub,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
        undefined, // Для загрузки данных в форму Б.С. и Договора,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        false      //GridTree
    ]
    ObjectEditConfig("viewDirServiceNomensWinEdit", Params);
};
function controllerDocServicePurchesEdit_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {

                var Params = [
                    "tree_" + id,
                    true, //UO_Center
                    true, //UO_Modal
                    3,    // 1 - Новое, 2 - Редактировать
                    false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                    undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    undefined, // Для загрузки данных в форму Б.С. и Договора,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    false      //GridTree
                ]
                ObjectEditConfig("viewDirServiceNomensWinEdit", Params);

            }
        }
    });
};
function controllerDocServicePurchesEdit_onTree_folderDel(id) {
    //Формируем сообщение: Удалить или снять пометку на удаление
    var sMsg = lanDelete;
    if (Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;

    //Процес Удаление или снятия пометки
    Ext.MessageBox.show({
        title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("tree_" + id) });
                loadingMask.show();
                //Выбранный ID-шник Грида
                var DirServiceNomenID = Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.id; //Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.DirServiceNomenID;
                //Запрос на удаление
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirServiceNomens + DirServiceNomenID + "/",
                    method: 'DELETE',
                    success: function (result) {
                        loadingMask.hide();
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            //Очистить форму
                            //Ext.getCmp("DirServiceNomenPatchFull" + id).setValue("");
                            //Ext.getCmp("form_" + id).reset(true);
                            //Открыть ветки
                            fun_ReopenTree_1(id, undefined, "tree_" + id, sData.data);
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
function controllerDocServicePurchesEdit_onTree_folderSubNull(id) {
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
                    url: HTTP_DirServiceNomens + "?id=" + Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.id + "&sub=0", // + overModel.data.id,
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
function controllerDocServicePurchesEdit_onTree_addSub(id) {
    
    //Если форма ещё не загружена - выйти!
    var widgetXForm = Ext.getCmp("form_" + id);
    //if (!widgetXForm.UO_Loaded) return;

    var node = funReturnNode(id);
    if (node != undefined) {
        node.data.leaf = false;
        Ext.getCmp("tree_" + id).getView().refresh();
        node.expand();
    }

};

