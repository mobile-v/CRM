Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandRetailTabsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocSecondHandRetailTabsEdit',



    onChangeDiscount: function (textfield, valueNew, valueOld, e) {
        viewcontrollerDocSecondHandRetailTabsEdit_RecalculationSums(textfield.UO_id);
    },
    onPaymentDiscount: function (textfield, valueNew, valueOld, e) {
        viewcontrollerDocSecondHandRetailTabsEdit_RecalculationSums(textfield.UO_id);
    },

    //Расчет - не работает
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        
        if (!varRightDocDescriptionCheck && parseFloat(Ext.getCmp("Discount" + aButton.UO_id).getValue()) > 0) { Ext.Msg.alert(lanOrgName, "Скидки отключены! Обратитесь к Администратору для их включения!"); return; }

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
                            viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick_Union(aButton);
                        }
                        else if (btn == "no") {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                            viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick_Union(aButton);
                        }
                    }
                });
            }
            else if (varPayType == 1) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick_Union(aButton);
            }
            else if (varPayType == 2) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick_Union(aButton);
            }

        }
        else {
            viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick_Union(aButton);
        }
    },


    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

});


//Функия сохранения (тут много ньюансов)
//- ККМ
//- Сохранение

//Запускается при нажатии на кнопку "Провести"
function viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick_Union(aButton) {

    //KKM
    if (varKKMSActive) {
        Ext.Msg.confirm("Confirmation", "Печатать чек на ККМ?", function (btnText) {
            if (btnText === "no") {

                viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick(aButton);

                //Закрыть
                //Ext.getCmp(aButton.UO_idMain).close();
            }
            else if (btnText === "yes") {

                //Параметры формы
                var
                    SubReal = parseFloat(Ext.getCmp("Quantity" + aButton.UO_id).getValue()) * (parseFloat(parseFloat(Ext.getCmp("PriceCurrency" + aButton.UO_id).getValue()) - parseFloat(Ext.getCmp("Discount" + aButton.UO_id).getValue()))),
                    SumNal = 0,
                    SumBezNal = 0;
                if (parseInt(Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).getValue()) == 1) {
                    SumNal = SubReal;
                }
                else {
                    SumBezNal = SubReal;
                }

                //DirNomenName - Полное
                //var DirNomenName = Ext.getCmp("DirNomenName" + aButton.UO_id).getValue();

                var FormData =
                    {
                        DirNomenName: Ext.getCmp("DirServiceNomenName" + aButton.UO_id).getValue(),
                        PriceCurrency: parseFloat(Ext.getCmp("PriceCurrency" + aButton.UO_id).getValue()),
                        Amount: SubReal,
                        Quantity: parseFloat(Ext.getCmp("Quantity" + aButton.UO_id).getValue()),
                        SumNal: SumNal,
                        SumBezNal: SumBezNal,
                        KKMSPhone: Ext.getCmp("KKMSPhone" + aButton.UO_id).getValue(),
                        KKMSEMail: Ext.getCmp("KKMSEMail" + aButton.UO_id).getValue(),
                    };

                RegisterCheck(aButton, FormData, viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick_Kkm, 0, false);
            }
        }, this);
    }
    else {
        viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick(aButton);
    }

}

//Запускается после печати чека (надо получить CheckNumber)
function viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick_Kkm(Rezult, textStatus, jqXHR, aButton) {
    
    if (Rezult.Status == 0) {

        if (Ext.getCmp("KKMSCheckNumber" + aButton.UO_id)) {
            //Получаем "CheckNumber" и пишем его в поле
            Ext.getCmp("KKMSCheckNumber" + aButton.UO_id).setValue(Rezult.CheckNumber);
            Ext.getCmp("KKMSIdCommand" + aButton.UO_id).setValue(Rezult.IdCommand);

            //Сохраняем в БД
            viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick(aButton);
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
function viewcontrollerDocSecondHandRetailTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {
    
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);
    //Проверка
    if (!widgetXForm.UO_GridSave) { Ext.Msg.alert("", "UO_GridSave == undefined"); return; }
    //Форма
    var form = widgetXForm.getForm();
    //Валидация
    if (!form.isValid()) { Ext.Msg.alert(lanOrgName, "Пожалуйста, заполните все поля формы!"); return; }
    //Получаем данные полей с формы
    var rec = form.getFieldValues();

    //Получаем Store Грида
    var store = Ext.getCmp("grid_" + aButton.UO_id).getStore(); store.setData([], false);
    //Insert
    store.insert(store.data.items.length, rec);

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++




    //Спецификация (табличная часть)
    var recordsDocSecondHandRetailTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocSecondHandRetailTab.push(rec.data); });

    //Проверка
    //if (Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Контрагента!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    //if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }
    if (Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Тип оплаты!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocSecondHandRetails + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocSecondHandRetailID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocSecondHandRetails + "?id=" + parseInt(Ext.getCmp("DocSecondHandRetailID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }



    var map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocSecondHandRetailTab: Ext.encode(recordsDocSecondHandRetailTab) },

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {
            

            //Если новая накладная присваиваем полученные номера!
            if (!Ext.getCmp('DocID' + aButton.UO_id).getValue()) {
                var sData = action.result.data;
                Ext.getCmp('DocID' + aButton.UO_id).setValue(sData.DocID);
                Ext.getCmp('DocSecondHandRetailReturnID' + aButton.UO_id).setValue(sData.DocSecondHandPurchID);
                //Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocSecondHandPurchID);
                //Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocSecondHandPurchID);
            }


            //Печать
            Ext.MessageBox.show({
                icon: Ext.MessageBox.QUESTION,
                width: 300,
                title: lanOrgName,
                msg: 'Печатать чек?',
                buttonText: { yes: "Да", no: "Нет", cancel: "Отмена" },
                fn: function (btn) {
                    if (btn == "yes") {
                        controllerListObjectPFs_onGrid_itemclick(
                            map,
                            aButton.UO_id,
                            66,
                            42,
                            Ext.getCmp("DocID" + aButton.UO_id).getValue(),
                            "Html"
                        );
                        //Закрыть
                        Ext.getCmp(aButton.UO_idMain).close();
                    }
                    else {
                        //Закрыть
                        Ext.getCmp(aButton.UO_idMain).close();
                    }
                }
            });


            //Обновляем грид
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) {
                storeGrid = Ext.getCmp(aButton.UO_idCall).getStore();
                storeGrid.load();
                storeGrid.on('load', function () {
                    //viewcontrollerDocSecondHandRetailsEdit_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false)
                    var storeGridParty = Ext.getCmp("gridParty_" + Ext.getCmp(aButton.UO_idCall).UO_id).store;
                    storeGridParty.load();
                    storeGridParty.on('load', function () {
                        if (Ext.getCmp(aButton.UO_idCall) && Ext.getCmp("TriggerSearchTree" + Ext.getCmp(aButton.UO_idCall).UO_id)) {
                            Ext.getCmp("TriggerSearchTree" + Ext.getCmp(aButton.UO_idCall).UO_id).focus();
                        }
                    });
                });
            }


        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};



//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function viewcontrollerDocSecondHandRetailTabsEdit_RecalculationSums(id) {
    if (parseFloat(Ext.getCmp("PriceCurrency" + id).getValue()) - parseFloat(Ext.getCmp("Discount" + id).getValue()) < 0) {
        Ext.Msg.alert(lanOrgName, "Скидка больше цены товара!"); return;
    }
};

