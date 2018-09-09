Ext.define("PartionnyAccount.controller.Sklad/Object/Pay/controllerPayEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],
    
    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewPayEdit': { close: this.this_close },


            //Currencies
            'viewPayEdit [itemId=DirCurrencyID]': { select: this.onDirCurrencyIDSelect },
            //'viewPayEdit [itemId=DirCurrencyID]': { change: this.onDirCurrencyIDSelect },

            'viewPayEdit button#btnCurrencyEdit': { "click": this.onBtnCurrencyEditClick },
            'viewPayEdit button#btnCurrencyReload': { "click": this.onBtnCurrencyReloadClick },
            'viewPayEdit button#btnCurrencyClear': { "click": this.onBtnCurrencyClearClick },


            'viewPayEdit [itemId=DirCurrencyRate]': { change: this.onDirCurrencyRateChange },
            'viewPayEdit [itemId=DirCurrencyMultiplicity]': { change: this.onDirCurrencyMultiplicityChange },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewPayEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewPayEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewPayEdit button#btnDel': { "click": this.onBtnDelClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    //Currency
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
            true, //UO_Center
            true, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCurrencies", Params);
    },
    onBtnCurrencyReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCurrenciesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCurrenciesGrid;
        storeDirCurrenciesGrid.load();
    },
    onBtnCurrencyClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCurrencyID" + aButton.UO_id).setValue("");
    },



    //Поменяли "Курс" (DirCurrencyRate)
    onDirCurrencyRateChange: function (aTextfield, aText) {
        //var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        //fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Кратность" (DirCurrencyMultiplicity)
    onDirCurrencyMultiplicityChange: function (aTextfield, aText) {
        //var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        //fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },




    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_Pays;
        if (parseInt(Ext.getCmp("DocCashBankID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_Pays + "?id=" + parseInt(Ext.getCmp("DocCashBankID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {

                //1. Обновить грид (список платежей)
                var Call = Ext.getCmp(aButton.UO_idCall);
                if (Call != undefined && Call.store != undefined) Call.getStore().load();


                //2. Изменить поле Доплата (Документ)
                var sData = action.result.data;

                var Doc_id = Ext.getCmp(Ext.getCmp(aButton.UO_idCall).UO_idCall).UO_id;

                var Payment = Ext.getCmp("Payment" + Doc_id);
                Payment.setValue(sData.Payment);

                var HavePay = Ext.getCmp("HavePay" + Doc_id);
                //HavePay.setValue(HavePay.getValue() - Ext.getCmp("DocXSumSum" + aButton.UO_id).getValue());
                HavePay.setValue(Ext.getCmp("SumOfVATCurrency" + Doc_id).getValue() - sData.Payment);


                //3. Закрыть форму
                Ext.getCmp(aButton.UO_idMain).close();

                //4. Закрыть Грид
                Ext.getCmp("viewPay" + Ext.getCmp(aButton.UO_idCall).UO_id).close();

            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },

    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },

    onBtnDelClick: function (aButton, aEvent, aOptions) {

        Ext.MessageBox.show({
            title: lanOrgName, msg: lanDelete + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    //Лоадер
                    var loadingMask = new Ext.LoadMask({
                        msg: 'Please wait...',
                        target: Ext.getCmp("form_" + aButton.UO_id)
                    });
                    loadingMask.show();
                    //Выбранный ID-шник Грида
                    var DocCashBankID = parseInt(Ext.getCmp("DocCashBankID" + aButton.UO_id).value);
                    var DirPaymentTypeID = parseInt(Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).value);
                    //Запрос на удаление
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_Pays + DocCashBankID + "/?DirPaymentTypeID=" + DirPaymentTypeID,
                        method: 'DELETE',
                        success: function (result) {
                            loadingMask.hide();
                            var sData = Ext.decode(result.responseText);
                            if (sData.success == true) {
                                Ext.MessageBox.show({ title: lanOrgName, msg: sData.data.Msg, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.OK })


                                //1. Обновить грид (список платежей)
                                var Call = Ext.getCmp(aButton.UO_idCall);
                                if (Call != undefined && Call.store != undefined) Call.getStore().load();


                                //2. Изменить поле Доплата (Документ)
                                var Doc_id = Ext.getCmp(Ext.getCmp(aButton.UO_idCall).UO_idCall).UO_id;

                                var Payment = Ext.getCmp("Payment" + Doc_id);
                                Payment.setValue(sData.data.Payment);

                                var HavePay = Ext.getCmp("HavePay" + Doc_id);
                                //HavePay.setValue(HavePay.getValue() - Ext.getCmp("DocXSumSum" + aButton.UO_id).getValue());
                                HavePay.setValue(Ext.getCmp("SumOfVATCurrency" + Doc_id).getValue() - sData.data.Payment);


                                //3. Закрыть форму
                                Ext.getCmp(aButton.UO_idMain).close();

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

    },

});