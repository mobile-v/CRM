Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetails/viewcontrollerDocRetailTabsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocRetailTabsEdit',



    onChangeDiscount: function (textfield, valueNew, valueOld, e) {
        viewcontrollerDocRetailTabsEdit_RecalculationSums(textfield.UO_id);
    },
    onPaymentDiscount: function (textfield, valueNew, valueOld, e) {
        viewcontrollerDocRetailTabsEdit_RecalculationSums(textfield.UO_id);
    },

    //Расчет
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {

        //Алгоритм:
        //1. Если надо печатать на ККМ, 
        //   1.1. то печатаем сначала на ККМ, передавая 2 параметра aButton и viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Union
        //   1.2. если улачная печать на ККМ, то функция вызывает функцию viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Union (но, как передать в неё aButton ... ?)
        //2. Иначе просто сохраняем в БД


        //1. Проверки:
        var
            Discount = parseFloat(Ext.getCmp("Discount" + aButton.UO_id).getValue()),
            Quantity = parseFloat(Ext.getCmp("Quantity" + aButton.UO_id).getValue()),
            PriceCurrency = parseFloat(Ext.getCmp("PriceCurrency" + aButton.UO_id).getValue());
        //1.1. 
        if (!varRightDocDescriptionCheck && Discount > 0) { Ext.Msg.alert(lanOrgName, "Скидки отключены! Обратитесь к Администратору для их включения!"); return; }
        //1.4. что бы сумма не была больше "varDiscountPercentMarket" в процентах от суммы "Quantity * PriceCurrency"
        var MaxDiscount = (Quantity * PriceCurrency * varDiscountPercentMarket) / 100;
        if (Discount > MaxDiscount) {
            Ext.Msg.alert(lanOrgName, "Скидка больше допустмой! Максимальная: " + MaxDiscount + "руб (" + varDiscountPercentMarket + "%)!<br/>Исправьте или обратитесь к Администратору!");
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
                            viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
                        }
                        else if (btn == "no") {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                            viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
                        }
                    }
                });
            }
            else if (varPayType == 1) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
            }
            else if (varPayType == 2) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
            }
        }
        else {
            viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Union(aButton); //, aEvent, aOptions
        }

    },


    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { viewcontrollerDocRetailTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

});



//Функия сохранения (тут много ньюансов)
//- ККМ
//- Сохранение

//Запускается при нажатии на кнопку "Провести"
function viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Union(aButton) {

    //KKM
    if (varKKMSActive) {
        Ext.Msg.confirm("Confirmation", "Печатать чек на ККМ?", function (btnText) {
            if (btnText === "no") {

                viewcontrollerDocRetailTabsEdit_onBtnSaveClick(aButton);

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
                        DirNomenName: Ext.getCmp("DirNomenName" + aButton.UO_id).getValue(),
                        PriceCurrency: parseFloat(Ext.getCmp("PriceCurrency" + aButton.UO_id).getValue()),
                        Amount: SubReal,
                        Quantity: parseFloat(Ext.getCmp("Quantity" + aButton.UO_id).getValue()),
                        SumNal: SumNal,
                        SumBezNal: SumBezNal,
                        KKMSPhone: Ext.getCmp("KKMSPhone" + aButton.UO_id).getValue(),
                        KKMSEMail: Ext.getCmp("KKMSEMail" + aButton.UO_id).getValue(),
                    };

                RegisterCheck(aButton, FormData, viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Kkm, 0, false);
            }
        }, this);
    }
    else {
        viewcontrollerDocRetailTabsEdit_onBtnSaveClick(aButton);
    }

}

//Запускается после печати чека (надо получить CheckNumber)
function viewcontrollerDocRetailTabsEdit_onBtnSaveClick_Kkm(Rezult, textStatus, jqXHR, aButton) {
    
    if (Rezult.Status == 0) {

        if (Ext.getCmp("KKMSCheckNumber" + aButton.UO_id)) {
            //Получаем "CheckNumber" и пишем его в поле
            Ext.getCmp("KKMSCheckNumber" + aButton.UO_id).setValue(Rezult.CheckNumber);
            Ext.getCmp("KKMSIdCommand" + aButton.UO_id).setValue(Rezult.IdCommand);

            //Сохраняем в БД
            viewcontrollerDocRetailTabsEdit_onBtnSaveClick(aButton);
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

//Для сохранения в БД
function viewcontrollerDocRetailTabsEdit_onBtnSaveClick(aButton) { //, aEvent, aOptions

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
    var recordsDocRetailTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocRetailTab.push(rec.data); });

    //Проверка
    //if (Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Контрагента!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    //if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }
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
            //Обновляем грид
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) {
                storeGrid = Ext.getCmp(aButton.UO_idCall).getStore();
                storeGrid.load();
                storeGrid.on('load', function () {
                    //viewcontrollerDocRetailsEdit_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false)
                    var storeGridParty = Ext.getCmp("gridParty_" + Ext.getCmp(aButton.UO_idCall).UO_id).store;
                    storeGridParty.load();
                    storeGridParty.on('load', function () {
                        if (Ext.getCmp(aButton.UO_idCall) && Ext.getCmp("TriggerSearchTree" + Ext.getCmp(aButton.UO_idCall).UO_id)) {
                            Ext.getCmp("TriggerSearchTree" + Ext.getCmp(aButton.UO_idCall).UO_id).focus();
                        }
                    });
                });
            };



            //KKM
            /*
            if (varKKMSActive) {
                Ext.Msg.confirm("Confirmation", "Печатать чек на ККМ?", function (btnText) {
                    if (btnText === "no") {
                        //Закрыть
                        Ext.getCmp(aButton.UO_idMain).close();
                    }
                    else if (btnText === "yes") {
                        RegisterCheck(aButton, 0, false);
                    }
                }, this);
            }
            */

            //Закрыть
            Ext.getCmp(aButton.UO_idMain).close();
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};




//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function viewcontrollerDocRetailTabsEdit_RecalculationSums(id) {

    var Discount = parseFloat(Ext.getCmp("Discount" + id).getValue());

    /*
    //Тип скидки: от суммы или от цены (если продали более 1 аппарата)
    //varDiscountMarketType
    if (varDiscountMarketType == 1) {
        //от суммы
        //...
    }
    else {
        //от цены
        Discount = Discount * parseFloat(Ext.getCmp("Quantity" + id).getValue());
    }
    */

    if (parseFloat(Ext.getCmp("PriceCurrency" + id).getValue()) - Discount < 0) {
        Ext.Msg.alert(lanOrgName, "Скидка больше цены товара!"); return;
    }



};

