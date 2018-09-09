Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetails/viewcontrollerDocRetailsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocRetailsEdit',


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewDocRetailsEditClose: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    // Группа (itemId=tree) === === === === === === === === === ===

    //Меню Группы
    onTree_expandAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).expandAll();
    },
    onTree_collapseAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).collapseAll();
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        var id = view.grid.UO_id;
        fun_DirNomenPatchFull_RemParties2(id, rec);

        /*
        //Полный путь от Группы к выбранному объкту
        Ext.getCmp("DirNomenPatchFull" + id).setValue(rec.get('DirNomenPatchFull'));

        var storeGrid = Ext.getCmp("gridParty_" + id).getStore();
        //Если панель скрыта, то не показывать "партии товара"
        if (Ext.getCmp("gridParty_" + id).collapsed) { storeGrid.setData([], false); return; }
        //Выбрана ли Организация (если новая накладная, то может быть и не выбрана Организация)
        if (Ext.getCmp("DirContractorIDOrg" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); return; }
        //Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
        if (Ext.getCmp("DirWarehouseID" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад (так как партии привязаны к Складу)!"); return; }

        //Получаем storeGrid и делаем load()
        Ext.getCmp("tree_" + id).setDisabled(true);
        storeGrid.proxy.url = HTTP_RemParties + "?DirNomenID=" + rec.get('id') + "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + id).getValue() + "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d");
        storeGrid.load();
        Ext.getCmp("tree_" + id).setDisabled(false);
        */
    },
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
    },


    btnDirContractorClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirContractorID" + aButton.UO_id).setValue(null);
    },


    //PanelParty
    //'viewDocRetailsEdit button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
    //Обновить список Товаров
    onBtnDirNomenReloadClick: function (aButton, aEvent, aOptions) {
        //var storeDirNomensTree = Ext.getCmp(aButton.UO_idMain).storeDirNomensTree;
        var storeDirNomensTree = Ext.getCmp("tree_" + aButton.UO_id).store;
        storeDirNomensTree.load();
    },

    /*'viewDocRetailsEdit #TriggerSearchTree': {
        "ontriggerclick": this.onTriggerSearchTreeClick1,
        "specialkey": this.onTriggerSearchTreeClick2,
        "change": this.onTriggerSearchTreeClick3
    },*/
    //Поиск
    onTriggerSearchTreeClick1: function (aButton, aEvent) {
        fun_onTriggerSearchTreeClick_Search(aButton, false);
    },
    onTriggerSearchTreeClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            fun_onTriggerSearchTreeClick_Search(f, false);
        }
    },
    onTriggerSearchTreeClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) {
            //funGridDir(e.UO_id, textReal, HTTP_DirNomens);
            //alert("В стадии разработки ...");
        }
    },



    //Грид (itemId=grid)
    //'viewDocRetailsEdit button#btnGridAddPosition': { click: this.onGrid_BtnGridAddPosition },
    onGrid_BtnGridAddPosition: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;
        /*
        var node = funReturnNode(id);
        */

        //Выбран ли товар
        //if (node == null) { Ext.Msg.alert(lanOrgName, "Не выбран товар! Выберите товар в списке слева!"); return; }

        //Выбрана ли партия
        var IdcallModelData = Ext.getCmp("gridParty_" + id).getSelectionModel().getSelection();
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
        //var storeRemPartiesGrid = Ext.getCmp("viewDocRetailsEdit" + aButton.UO_id).storeRemPartiesGrid;
        var storeRemPartiesGrid = new Ext.getCmp("gridParty_" + aButton.UO_id).getSelectionModel().getSelection()[0].data;
        storeRemPartiesGrid.Quantity = 1;
        storeRemPartiesGrid.DirPriceTypeID = 1;
        storeRemPartiesGrid.PriceCurrency = storeRemPartiesGrid.PriceRetailCurrency;

        //Store Спецификации
        var storeGrid = Ext.getCmp("viewDocRetailsEdit" + aButton.UO_id).storeGrid;

        //Вставляем запис в Спецификацию
        storeGrid.insert(storeGrid.data.items.length, storeRemPartiesGrid);

        //Пересчитываем сумму
        controllerDocRetailsEdit_RecalculationSums(aButton.UO_id, false);
    },

    //'viewDocRetailsEdit button#btnGridEdit': { click: this.onGrid_BtnGridEdit },
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
        ObjectEditConfig("viewDocRetailTabsEdit", Params);
    },

    //'viewDocRetailsEdit button#btnGridDelete': { click: this.onGrid_BtnGridDelete },
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
    /*'viewDocRetailsEdit [itemId=grid]': {
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
        controllerDocRetailsEdit_RecalculationSums(view.grid.UO_id, false);
    },
    //ДаблКлик: Редактирования или выбор
    onGrid_itemdblclick: function (view, record, item, index, e) {
        controllerDocRetailsEdit_RecalculationSums(view.grid.UO_id, false);
    },*/
    //Редактирование
    onGrid_edit: function (view, record, item, index, e) { //, index, e

        record.record.data.SUMPriceCurrency = (parseFloat(record.record.data.Quantity) * parseFloat(record.record.data.PriceCurrency)).toFixed(varFractionalPartInSum);
        record.record.commit()

        //Пересчитываем сумму
        controllerDocRetailsEdit_RecalculationSums(view.grid.UO_id, false);
    },
    

    onChangeDiscount: function (textfield, valueNew, valueOld, e) {
        controllerDocRetailsEdit_RecalculationSums(textfield.UO_id, false);
    },
    onPaymentDiscount: function (textfield, valueNew, valueOld, e) {
        controllerDocRetailsEdit_RecalculationSums(textfield.UO_id, false);
    },





    // === Кнопки: Сохранение, Отмена и Помощь === === ===
    //'viewDocRetailsEdit button#btnHeldCancel': { click: this.onBtnHeldCancelClick },
    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { controllerDocRetailsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

    //'viewDocRetailsEdit button#btnHelds': { click: this.onBtnHeldsClick },
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        
        if (Ext.getCmp("OnCredit" + aButton.UO_id).getValue() != true && Ext.getCmp('Surrender' + aButton.UO_id).getValue() < 0) {
            //Ext.getCmp('Payment' + aButton.UO_id).focus(true);

            //Ext.Msg.alert(lanOrgName, txtMsg026_2);
            Ext.MessageBox.show({
                buttons: Ext.MessageBox.OK,
                width: 300,
                title: lanOrgName,
                msg: txtMsg026_2,
                //buttonText: { yes: "Наличная", no: "Безналичная", cancel: "Отмена" },
                fn: function (btn) {
                    if (btn == "ok") {
                        Ext.getCmp('Payment' + aButton.UO_id).focus(true);
                    }
                }
            });

            return;
        }

        if (Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).getValue() == null) {

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
                            controllerDocRetailsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                        }
                        else if (btn == "no") {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                            controllerDocRetailsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                        }
                    }
                });
            }
            else if (varPayType == 1) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                controllerDocRetailsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
            }
            else if (varPayType == 2) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                controllerDocRetailsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
            }

        }
        else {
            controllerDocRetailsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
        }
    },

    //'viewDocRetailsEdit button#btnCancel': { "click": this.onBtnCancelClick },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },

    //'viewDocRetailsEdit button#btnHelp': { "click": this.onBtnHelpClick },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-retail/", '_blank');
    },

    //***
    //'viewDocRetailsEdit menuitem#btnPrintHtml': { click: this.onBtnPrintHtmlClick },
    //'viewDocRetailsEdit menuitem#btnPrintExcel': { click: this.onBtnPrintHtmlClick }
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {
        //aButton.UO_Action: html, excel
        //alert(aButton.UO_Action);

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocRetailID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

        //Открытие списка ПФ
        var Params = [
            aButton.id,
            true, //UO_Center
            true, //UO_Modal
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
function controllerDocRetailsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    //Спецификация (табличная часть)
    var recordsDocRetailTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocRetailTab.push(rec.data); });

    //Проверка
    //if (Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Контрагента!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }
    if (Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Тип оплаты!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocRetails + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocRetailID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocRetails + "?id=" + parseInt(Ext.getCmp("DocRetailID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocRetailTab: Ext.encode(recordsDocRetailTab) },

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
                if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(true); }
                Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(false);
                //Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(false);
            }


            //Если новая накладная присваиваем полученные номера!
            if (!Ext.getCmp('DocID' + aButton.UO_id).getValue()) {
                var sData = action.result.data;
                Ext.getCmp('DocID' + aButton.UO_id).setValue(sData.DocID);
                Ext.getCmp('DocRetailID' + aButton.UO_id).setValue(sData.DocRetailID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocRetailID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocRetailID);

                Ext.getCmp('viewDocRetailsEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocRetailsEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocRetailID" + aButton.UO_id).getValue() + ")");
                Ext.getCmp("btnPrint" + aButton.UO_id).setVisible(true);
            }

            //Закрыть
            if (aButton.UO_Action == "save_close") { Ext.getCmp(aButton.UO_idMain).close(); }
            //Перегрузить грид, если грид открыт
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};



//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocRetailsEdit_RecalculationSums(id, ShowMsg) {

    //fun_DocX_RecalculationSums(id, ShowMsg);
    //return;

    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocRetailTabsGrid; //можно так: Ext.getCmp("viewDocRetailsEdit" + id).storeDocRetailTabsGrid;
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
    }

    //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)
    Ext.getCmp("grid_" + id).getView().refresh();

    //Сумма с НДС
    //Discount %
    //SumOfVATCurrency = parseFloat(parseFloat(SumOfVATCurrency) - parseFloat(parseFloat(SumOfVATCurrency) * parseFloat(Ext.getCmp("Discount" + id).getValue()) / 100));
    //Discount Fix
    SumOfVATCurrency = parseFloat(parseFloat(SumOfVATCurrency) - parseFloat(Ext.getCmp("Discount" + id).getValue()));
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum)); //Подсчёт суммы SUM(Учётная цена * К-во)

    /*
    //Надо доплатить
    var HavePay = parseFloat(parseFloat(Ext.getCmp("SumOfVATCurrency" + id).getValue()) - parseFloat(Ext.getCmp("Payment" + id).getValue()));
    Ext.getCmp("HavePay" + id).setValue(HavePay.toFixed(varFractionalPartInSum));

    if (ShowMsg) {
        if (DirVatValue_Collection != "") {
            if (Match) { Ext.Msg.alert(lanOrgName, txtVatChanges + "<b>" + DirVatValue_Collection + " %</b>" + txtVatRecalc);} //Сообщение о пересчёте НДС
            else { Ext.Msg.alert(lanOrgName, txtVatNotMatch + "<BR>" + txtVatChanges + "<b>" + DirVatValue_Collection + " %</b>" + txtVatRecalc); } //Сообщение о пересчёте НДС
        }
    }
    */

    var Surrender = parseFloat(parseFloat(Ext.getCmp("Payment" + id).getValue()) - parseFloat(Ext.getCmp("SumOfVATCurrency" + id).getValue()));
    Ext.getCmp("Surrender" + id).setValue(Surrender.toFixed(varFractionalPartInSum));
};