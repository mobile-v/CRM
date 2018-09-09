Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetailReturns/controllerDocRetailReturnsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.controllerDocRetailReturnsEdit',


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewDocRetailReturnsEditClose: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Редактирование или добавление нового Поставщика
    onTriggerDocRetailNameClick: function (aButton, aEvent, aOptions) {

        var Params = [
            aButton.id,
            true, //UO_Center
            true, //UO_Modal
            undefined, //UO_Function_Tree
            this.fn_onTriggerDocRetailNameClick, //UO_Function_Grid
            true,  //TreeShow
            true, //GridShow
            undefined,     //TreeServerParam1
            undefined      //GridServerParam1
        ]
        ObjectConfig("viewDocRetails", Params);
    },
    //Заполнить 2-а поля
    fn_onTriggerDocRetailNameClick: function (id, rec) {
        Ext.getCmp("DocRetailName" + id).setValue("№ " + rec.get("DocRetailID") + " за " + rec.get("DocDate"));
        Ext.getCmp("DocRetailID" + id).setValue(rec.get("DocRetailID"));
        Ext.getCmp("DirContractorID" + id).setValue(rec.get("DirContractorID"));
        Ext.getCmp("DirWarehouseID" + id).setValue(rec.get("DirWarehouseID"));
        Ext.getCmp("DirContractorIDOrg" + id).setValue(rec.get("DirContractorIDOrg"));
        Ext.getCmp("DirVatValue" + id).setValue(rec.get("DirVatValue"));
        Ext.getCmp("Discount" + id).setValue(rec.get("Discount"));


        //Обновление "Списание партий"

        //Выбран документ Продажа
        if (Ext.getCmp("DocRetailID" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите документ Продажа (так как списанные партии привязаны к Продаже)!"); return; }

        //Получаем storeGrid и делаем load()
        var storeGrid = Ext.getCmp("gridPartyMinus_" + id).getStore();
        storeGrid.proxy.url = HTTP_RemPartyMinuses + "?DocRetailID=" + Ext.getCmp("DocRetailID" + id).getValue();
        storeGrid.load();
    },

    
    //PanelParty
    //'viewDocRetailReturnsEdit button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
    //Обновить список Товаров
    onBtnDirNomenReloadClick: function (aButton, aEvent, aOptions) {
        //var storeDirNomensTree = Ext.getCmp(aButton.UO_idMain).storeDirNomensTree;
        var storeDirNomensTree = Ext.getCmp("tree_" + aButton.UO_id).store;
        storeDirNomensTree.load();
    },

    
    //Грид (itemId=grid)
    //'viewDocRetailReturnsEdit button#btnGridAddPosition': { click: this.onGrid_BtnGridAddPosition },
    onGrid_BtnGridAddPosition: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;
        /*
        var node = funReturnNode(id);
        */

        //Выбран ли товар
        //if (node == null) { Ext.Msg.alert(lanOrgName, "Не выбран товар! Выберите товар в списке слева!"); return; }

        //Выбрана ли партия
        var IdcallModelData = Ext.getCmp("gridPartyMinus_" + id).getSelectionModel().getSelection();
        if (IdcallModelData.length == 0) {
            Ext.Msg.alert(lanOrgName, "Не выбрана партия! Выберите партию товара в списке сверху!");
            return;
        }

        //Есть ли Остаток
        //if (Ext.getCmp("gridParty_" + id).store.data.length == 0) { Ext.Msg.alert(lanOrgName, "Нет остатка товара (партий) на складе!"); return; }


        /*
        - Когда выбираем партию товара ПРОВЕРЯТЬ Склад. 
          Если не соответствует - вывести об этом сообщение и НЕ открытвать форму!
        Алгоритм:
        1. Кликнули на добавить товар
        2. Проверка в контролере на склад в документа и в партии
        3. Не соответствует - сообщение и закрыть форму
        4. Соответствует - всё ок!
        */
        if (IdcallModelData[0].data.DirWarehouseID != parseInt(Ext.getCmp("DirWarehouseID" + id).getValue())) {
            Ext.Msg.alert(lanOrgName, "Склад партии не соответствует складу документа!<br /> Поменяйте склад документа или кликните ещё раз на выбранном товаре!");
            return;
        }



        //Store Партий
        //var storeRemPartiesGrid = Ext.getCmp("viewDocRetailReturnsEdit" + aButton.UO_id).storeRemPartiesGrid;
        var storeRemPartiesGrid = new Ext.getCmp("gridPartyMinus_" + aButton.UO_id).getSelectionModel().getSelection()[0].data;
        storeRemPartiesGrid.Quantity = 1;
        storeRemPartiesGrid.DirPriceTypeID = 1;

        //Store Спецификации
        var storeGrid = Ext.getCmp("viewDocRetailReturnsEdit" + aButton.UO_id).storeGrid;

        //Вставляем запис в Спецификацию
        storeGrid.insert(storeGrid.data.items.length, storeRemPartiesGrid);

        //Пересчитываем сумму
        controllerDocRetailReturnsEdit_RecalculationSums(aButton.UO_id, false);
    },

    //'viewDocRetailReturnsEdit button#btnGridEdit': { click: this.onGrid_BtnGridEdit },
    onGrid_BtnGridEdit: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;

        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            2,    // 1 - Новое, 2 - Редактировать
            true,
            Ext.getCmp("grid_" + aButton.UO_id).store.indexOf(Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0]),       // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0], // Для загрузки данных в форму редактирования Табличной части
            id,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            false
        ]
        ObjectEditConfig("viewDocRetailReturnTabsEdit", Params);
    },

    //'viewDocRetailReturnsEdit button#btnGridDelete': { click: this.onGrid_BtnGridDelete },
    onGrid_BtnGridDelete: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName,
            msg: lanDelete + "?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    var selection = Ext.getCmp("grid_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
                    if (selection) { Ext.getCmp("grid_" + aButton.UO_id).store.remove(selection); }
                }
            }
        });
    },


    // Клик по Гриду
    /*'viewDocRetailReturnsEdit [itemId=grid]': {
        selectionchange: this.onGrid_selectionchange,
        itemclick: this.onGrid_itemclick,
        itemdblclick: this.onGrid_itemdblclick
    },*/
    
    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        //model.view.ownerGrid.down("#btnGridEdit").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnGridDelete").setDisabled(records.length === 0);
    },
    //Клик: Редактирования или выбор
    /*onGrid_itemclick: function (view, record, item, index, eventObj) {
        controllerDocRetailReturnsEdit_RecalculationSums(view.grid.UO_id, false);
    },
    //ДаблКлик: Редактирования или выбор
    onGrid_itemdblclick: function (view, record, item, index, e) {
        controllerDocRetailReturnsEdit_RecalculationSums(view.grid.UO_id, false);
    },*/
    //Редактирование
    onGrid_edit: function (view, record, item, index, e) { //, index, e

        record.record.data.SUMPriceCurrency = (parseFloat(record.record.data.Quantity) * parseFloat(record.record.data.PriceCurrency)).toFixed(varFractionalPartInSum);
        record.record.commit()

        //Пересчитываем сумму
        controllerDocRetailReturnsEdit_RecalculationSums(view.grid.UO_id, false);
    },
    

    onChangeDiscount: function (textfield, valueNew, valueOld, e) {
        controllerDocRetailReturnsEdit_RecalculationSums(textfield.UO_id, false);
    },
    onPaymentDiscount: function (textfield, valueNew, valueOld, e) {
        controllerDocRetailReturnsEdit_RecalculationSums(textfield.UO_id, false);
    },





    // === Кнопки: Сохранение, Отмена и Помощь === === ===
    //'viewDocRetailReturnsEdit button#btnHeldCancel': { click: this.onBtnHeldCancelClick },
    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { controllerDocRetailReturnsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

    //'viewDocRetailReturnsEdit button#btnHelds': { click: this.onBtnHeldsClick },
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        
        if (Ext.getCmp("OnCredit" + aButton.UO_id).getValue() != true && Ext.getCmp('Surrender' + aButton.UO_id).getValue() < 0) { Ext.Msg.alert(lanOrgName, txtMsg026_2); return; }
        controllerDocRetailReturnsEdit_onBtnSaveClick(aButton, aEvent, aOptions);

    },

    //'viewDocRetailReturnsEdit button#btnCancel': { "click": this.onBtnCancelClick },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },

    //'viewDocRetailReturnsEdit button#btnHelp': { "click": this.onBtnHelpClick },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-retail/", '_blank');
    },

    //***
    //'viewDocRetailReturnsEdit menuitem#btnPrintHtml': { click: this.onBtnPrintHtmlClick },
    //'viewDocRetailReturnsEdit menuitem#btnPrintExcel': { click: this.onBtnPrintHtmlClick }
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {
        //aButton.UO_Action: html, excel
        //alert(aButton.UO_Action);

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocRetailReturnID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

        //Открытие списка ПФ
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            undefined, //aButton.UO_Action, //UO_Function_Tree: Html или Excel
            undefined,
            undefined,
            undefined,
            Ext.getCmp("DocID" + aButton.UO_id).getValue(),
            56
        ]
        ObjectConfig("viewListObjectPFs", Params);
    },

});


//Функия сохранения
function controllerDocRetailReturnsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    //Спецификация (табличная часть)
    var recordsDocRetailReturnTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocRetailReturnTab.push(rec.data); });

    //Проверка
    if (Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Контрагента!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocRetailReturns + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocRetailReturnID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocRetailReturns + "?id=" + parseInt(Ext.getCmp("DocRetailReturnID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocRetailReturnTab: Ext.encode(recordsDocRetailReturnTab) },

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {

            if (aButton.UO_Action == "held_cancel") {
                Ext.getCmp("Held" + aButton.UO_id).setValue(false);
                Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(false);
                Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(true);
                //Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(true);
            }
            else if (aButton.UO_Action == "held") {
                Ext.getCmp("Held" + aButton.UO_id).setValue(true);
                Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(true);
                Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(false);
                //Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(false);
            }


            //Если новая накладная присваиваем полученные номера!
            if (!Ext.getCmp('DocID' + aButton.UO_id).getValue()) {
                var sData = action.result.data;
                Ext.getCmp('DocID' + aButton.UO_id).setValue(sData.DocID);
                Ext.getCmp('DocRetailReturnID' + aButton.UO_id).setValue(sData.DocRetailReturnID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocRetailReturnID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocRetailReturnID);

                Ext.getCmp('viewDocRetailReturnsEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocRetailReturnsEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocRetailReturnID" + aButton.UO_id).getValue() + ")");
            }

            //Закрыть
            if (aButton.UO_Action == "save_close") Ext.getCmp(aButton.UO_idMain).close();
            //Перегрузить грид, если грид открыт
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) Ext.getCmp(aButton.UO_idCall).getStore().load();
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};



//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocRetailReturnsEdit_RecalculationSums(id, ShowMsg) {

    //fun_DocX_RecalculationSums(id, ShowMsg);
    //return;

    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocRetailReturnTabsGrid; //можно так: Ext.getCmp("viewDocRetailReturnsEdit" + id).storeDocRetailReturnTabsGrid;
    //Скидка, если undefined, isNaN или "", то ставим её == 0
    if (isNaN(parseFloat(Ext.getCmp("Discount" + id).getValue()))) { Ext.getCmp("Discount" + id).setValue(0); }
    //Ставка Налога
    var DirVatValue = 0;
    var Match = true; //совпадение ставок НДС для всех Номенклатур.
    var DirVatValue = parseFloat(Ext.getCmp("DirVatValue" + id).getValue());

    var SumOfVATCurrency = 0; //var SumVATCurrency = 0;
    for (var i = 0; i < storeGrid.data.items.length; i++) {
        //Номенклатура + проверка на совпадение ставок НДС для всех Номенклатур.
        //Сумма спецификации
        var pSumOfVATCurrency = parseFloat(storeGrid.data.items[i].data.Quantity * storeGrid.data.items[i].data.PriceCurrency);
        //Меняем сумму спецификации (если добавили новую позицию)
        storeGrid.data.items[i].data.SUMSalePriceVATCurrency = pSumOfVATCurrency.toFixed(varFractionalPartInSum);

        SumOfVATCurrency += parseFloat(pSumOfVATCurrency);
        //SumVATCurrency += parseFloat((parseFloat(pSumOfVATCurrency) - parseFloat(pSumOfVATCurrency) / (1 + (DirVatValue) / 100)));
    }

    //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)
    Ext.getCmp("grid_" + id).getView().refresh();

    //Сумма с НДС
    SumOfVATCurrency = parseFloat(parseFloat(SumOfVATCurrency) - parseFloat(parseFloat(SumOfVATCurrency) * parseFloat(Ext.getCmp("Discount" + id).getValue()) / 100));
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum)); //Подсчёт суммы SUM(Учётная цена * К-во)

    /*
    //Надо доплатить
    var HavePay = parseFloat(parseFloat(Ext.getCmp("SumOfVATCurrency" + id).getValue()) - parseFloat(Ext.getCmp("Payment" + id).getValue()));
    Ext.getCmp("HavePay" + id).setValue(HavePay.toFixed(varFractionalPartInSum));

    if (ShowMsg) {
        if (DirVatValue_Collection != "") {
            if (Match) Ext.Msg.alert(lanOrgName, txtVatChanges + "<b>" + DirVatValue_Collection + " %</b>" + txtVatRecalc); //Сообщение о пересчёте НДС
            else Ext.Msg.alert(lanOrgName, txtVatNotMatch + "<BR>" + txtVatChanges + "<b>" + DirVatValue_Collection + " %</b>" + txtVatRecalc); //Сообщение о пересчёте НДС
        }
    }
    */

    var Surrender = parseFloat(parseFloat(Ext.getCmp("Payment" + id).getValue()) - parseFloat(Ext.getCmp("SumOfVATCurrency" + id).getValue()));
    Ext.getCmp("Surrender" + id).setValue(Surrender.toFixed(varFractionalPartInSum));
};