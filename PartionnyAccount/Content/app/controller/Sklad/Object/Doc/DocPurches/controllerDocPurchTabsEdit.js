Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocPurches/controllerDocPurchTabsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocPurchTabsEdit': { close: this.this_close },

            //'viewDocPurchTabsEdit #DirNomenName': { "ontriggerclick": this.onTriggerDirNomenNameClick },
            //'viewDocPurchTabsEdit button#btnDirNomenHistory': { "click": this.onbtnDirNomenHistoryClick },



            //Характеристики
            //Colour
            'viewDocPurchTabsEdit button#btnCharColourEdit': { "click": this.onBtnCharColourEditClick },
            'viewDocPurchTabsEdit button#btnCharColourReload': { "click": this.onBtnCharColourReloadClick },
            'viewDocPurchTabsEdit button#btnCharColourClear': { "click": this.onBtnCharColourClearClick },
            //Material
            'viewDocPurchTabsEdit button#btnCharMaterialEdit': { "click": this.onBtnCharMaterialEditClick },
            'viewDocPurchTabsEdit button#btnCharMaterialReload': { "click": this.onBtnCharMaterialReloadClick },
            'viewDocPurchTabsEdit button#btnCharMaterialClear': { "click": this.onBtnCharMaterialClearClick },
            //Name
            'viewDocPurchTabsEdit button#btnCharNameEdit': { "click": this.onBtnCharNameEditClick },
            'viewDocPurchTabsEdit button#btnCharNameReload': { "click": this.onBtnCharNameReloadClick },
            'viewDocPurchTabsEdit button#btnCharNameClear': { "click": this.onBtnCharNameClearClick },
            //Season
            'viewDocPurchTabsEdit button#btnCharSeasonEdit': { "click": this.onBtnCharSeasonEditClick },
            'viewDocPurchTabsEdit button#btnCharSeasonReload': { "click": this.onBtnCharSeasonReloadClick },
            'viewDocPurchTabsEdit button#btnCharSeasonClear': { "click": this.onBtnCharSeasonClearClick },
            //Sex
            'viewDocPurchTabsEdit button#btnCharSexEdit': { "click": this.onBtnCharSexEditClick },
            'viewDocPurchTabsEdit button#btnCharSexReload': { "click": this.onBtnCharSexReloadClick },
            'viewDocPurchTabsEdit button#btnCharSexClear': { "click": this.onBtnCharSexClearClick },
            //Size
            'viewDocPurchTabsEdit button#btnCharSizeEdit': { "click": this.onBtnCharSizeEditClick },
            'viewDocPurchTabsEdit button#btnCharSizeReload': { "click": this.onBtnCharSizeReloadClick },
            'viewDocPurchTabsEdit button#btnCharSizeClear': { "click": this.onBtnCharSizeClearClick },
            //Style
            'viewDocPurchTabsEdit button#btnCharStyleEdit': { "click": this.onBtnCharStyleEditClick },
            'viewDocPurchTabsEdit button#btnCharStyleReload': { "click": this.onBtnCharStyleReloadClick },
            'viewDocPurchTabsEdit button#btnCharStyleClear': { "click": this.onBtnCharStyleClearClick },
            //Texture
            'viewDocPurchTabsEdit button#btnCharTextureEdit': { "click": this.onBtnCharTextureEditClick },
            'viewDocPurchTabsEdit button#btnCharTextureReload': { "click": this.onBtnCharTextureReloadClick },
            'viewDocPurchTabsEdit button#btnCharTextureClear': { "click": this.onBtnCharTextureClearClick },


            //Currencies
            'viewDocPurchTabsEdit [itemId=DirCurrencyID]': { select: this.onDirCurrencyIDSelect },
            //'viewDocPurchTabsEdit [itemId=DirCurrencyID]': { change: this.onDirCurrencyIDSelect },

            'viewDocPurchTabsEdit button#btnCurrencyEdit': { "click": this.onBtnCurrencyEditClick },
            'viewDocPurchTabsEdit button#btnCurrencyReload': { "click": this.onBtnCurrencyReloadClick },
            'viewDocPurchTabsEdit button#btnCurrencyClear': { "click": this.onBtnCurrencyClearClick },


            'viewDocPurchTabsEdit [itemId=DirCurrencyRate]': { change: this.onDirCurrencyRateChange },
            'viewDocPurchTabsEdit [itemId=DirCurrencyMultiplicity]': { change: this.onDirCurrencyMultiplicityChange },


            'viewDocPurchTabsEdit [itemId=PriceVAT]': { change: this.onPriceVATChange },
            'viewDocPurchTabsEdit [itemId=PriceCurrency]': { change: this.onPriceCurrencyChange },

            'viewDocPurchTabsEdit [itemId=MarkupRetail]': { change: this.onMarkupRetailChange },
            'viewDocPurchTabsEdit [itemId=PriceRetailVAT]': { change: this.onPriceRetailVATChange },
            'viewDocPurchTabsEdit [itemId=PriceRetailCurrency]': { change: this.onPriceRetailCurrencyChange },

            'viewDocPurchTabsEdit [itemId=MarkupWholesale]': { change: this.onMarkupWholesaleChange },
            'viewDocPurchTabsEdit [itemId=PriceWholesaleVAT]': { change: this.onPriceWholesaleVATChange },
            'viewDocPurchTabsEdit [itemId=PriceWholesaleCurrency]': { change: this.onPriceWholesaleCurrencyChange },

            'viewDocPurchTabsEdit [itemId=MarkupIM]': { change: this.onMarkupIMChange },
            'viewDocPurchTabsEdit [itemId=PriceIMVAT]': { change: this.onPriceIMVATChange },
            'viewDocPurchTabsEdit [itemId=PriceIMCurrency]': { change: this.onPriceIMCurrencyChange },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocPurchTabsEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocPurchTabsEdit menuitem#btnPrint_barcode': { "click": this.onBtnPrintClick },
            'viewDocPurchTabsEdit menuitem#btnPrint_barcode_price': { "click": this.onBtnPrintClick },
            'viewDocPurchTabsEdit menuitem#btnPrint_barcode_name': { "click": this.onBtnPrintClick },
            'viewDocPurchTabsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocPurchTabsEdit button#btnDel': { "click": this.onBtnDelClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Характеристики
    //Colour
    onBtnCharColourEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharColours", Params);
    },
    onBtnCharColourReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCharColoursGrid = Ext.getCmp(aButton.UO_idMain).storeDirCharColoursGrid;
        storeDirCharColoursGrid.load();
    },
    onBtnCharColourClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCharColourID" + aButton.UO_id).setValue("");
    },
    //Material
    onBtnCharMaterialEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharMaterials", Params);
    },
    onBtnCharMaterialReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCharMaterialsGrid = Ext.getCmp(aButton.UO_idMain).storeDirCharMaterialsGrid;
        storeDirCharMaterialsGrid.load();
    },
    onBtnCharMaterialClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCharMaterialID" + aButton.UO_id).setValue("");
    },
    //Name
    onBtnCharNameEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharNames", Params);
    },
    onBtnCharNameReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCharNamesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCharNamesGrid;
        storeDirCharNamesGrid.load();
    },
    onBtnCharNameClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCharNameID" + aButton.UO_id).setValue("");
    },
    //Season
    onBtnCharSeasonEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharSeasons", Params);
    },
    onBtnCharSeasonReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCharSeasonsGrid = Ext.getCmp(aButton.UO_idMain).storeDirCharSeasonsGrid;
        storeDirCharSeasonsGrid.load();
    },
    onBtnCharSeasonClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCharSeasonID" + aButton.UO_id).setValue("");
    },
    //Sex
    onBtnCharSexEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharSexes", Params);
    },
    onBtnCharSexReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCharSexesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCharSexesGrid;
        storeDirCharSexesGrid.load();
    },
    onBtnCharSexClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCharSexID" + aButton.UO_id).setValue("");
    },
    //Size
    onBtnCharSizeEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharSizes", Params);
    },
    onBtnCharSizeReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCharSizesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCharSizesGrid;
        storeDirCharSizesGrid.load();
    },
    onBtnCharSizeClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCharSizeID" + aButton.UO_id).setValue("");
    },
    //Style
    onBtnCharStyleEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharStyles", Params);
    },
    onBtnCharStyleReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCharStylesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCharStylesGrid;
        storeDirCharStylesGrid.load();
    },
    onBtnCharStyleClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCharStyleID" + aButton.UO_id).setValue("");
    },
    //Texture
    onBtnCharTextureEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharTextures", Params);
    },
    onBtnCharTextureReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCharTexturesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCharTexturesGrid;
        storeDirCharTexturesGrid.load();
    },
    onBtnCharTextureClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCharTextureID" + aButton.UO_id).setValue("");
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
    onBtnCurrencyClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCurrencyID" + aButton.UO_id).setValue("");
    },



    //Поменяли "Курс" (DirCurrencyRate)
    onDirCurrencyRateChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Кратность" (DirCurrencyMultiplicity)
    onDirCurrencyMultiplicityChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },



    //Поменяли "Приходную цену"
    onPriceVATChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Приходную цену в текущей валюте" (PriceCurrency)
    onPriceCurrencyChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },

    //Поменяли "Наценку Розницы"
    onMarkupRetailChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupRetail_Change(aTextfield.UO_id, PrimaryFieldID); //fn_controllerDirNomensEdit_MarkupRetail_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Розницы"
    onPriceRetailVATChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceRetailVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Розницы"
    onPriceRetailCurrencyChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceRetailCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },

    //Поменяли "Наценку Опта"
    onMarkupWholesaleChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupWholesale_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Опта"
    onPriceWholesaleVATChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceWholesaleVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Опта"
    onPriceWholesaleCurrencyChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceWholesaleCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },

    //Поменяли "Наценку IM"
    onMarkupIMChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupIM_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену IM"
    onPriceIMVATChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceIMVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену IM"
    onPriceIMCurrencyChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceIMCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },





    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        
        controllerDocPurchTabsEdit_onBtnSaveClick(aButton);

    },
    onBtnPrintClick: function (aButton, aEvent, aOptions) {

        var mapForm = document.createElement("form");
        mapForm.target = "Map";
        mapForm.method = "GET"; // or "post" if appropriate
        mapForm.action = "../report/report/";

        //var UO_id = Ext.getCmp(aButton.UO_idCall).UO_id;

        //Параметр "pID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pID"; mapInput.value = "DocPurcheTabsPrintCode"; mapForm.appendChild(mapInput);

        //Параметр "DirNomenID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "Quantity"; mapInput.value = Ext.getCmp("Quantity" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "DirNomenID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DirNomenID"; mapInput.value = Ext.getCmp("DirNomenID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "DirNomenName"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DirNomenName"; mapInput.value = Ext.getCmp("DirNomenName" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "PriceRetailCurrency"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "PriceRetailCurrency"; mapInput.value = Ext.getCmp("PriceRetailCurrency" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "UO_Action"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "UO_Action"; mapInput.value = aButton.UO_Action; mapForm.appendChild(mapInput);


        document.body.appendChild(mapForm);
        map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

        if (map) { mapForm.submit(); }
        else { alert('You must allow popups for this map to work.'); }

    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnDelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName,
            msg: lanDelete + "?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    var selection = Ext.getCmp(aButton.UO_idCall).getView().getSelectionModel().getSelection()[0];
                    if (selection) {
                        Ext.getCmp(aButton.UO_idCall).store.remove(selection);

                        Ext.getCmp(aButton.UO_idMain).close();
                    }
                }
            }
        });
    },

});



//Функия сохранения
function controllerDocPurchTabsEdit_onBtnSaveClick(aButton) { //, aEvent, aOptions


    //Алгоритм (СТАРЫЙ):
    //1. Сохраняем на Сервер
    //2. Сохраняем в Грид
    //fun_SaveTabDoc1(aButton, controllerDocPurchesEdit_RecalculationSums);


    // *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


    //Алгоритм (НОВЫЙ):
    //1. В "controllerDocPurchTabsEdit" при сохранении проверяем сохранён ли документ (DocID или DocPurchID != null)
    //if (parseFloat(Ext.getCmp("DocPurchID" + aButton.UO_id).getValue()) > 0) 
    //{
    //...
    //}
    //если не сохранён, то сохраняем без табличной части
    //2. Сохраняем отдельно каждую позицию спецификации при создании новой или редактировании
    //На сервере: 
    //    - INSERT
    //    - UPDATE

    //2. 
    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocPurchTabs + "?DocPurchID=" + parseInt(Ext.getCmp("DocPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value) + "&UO_Action=save";
    if (parseInt(Ext.getCmp("DocPurchTabID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocPurchTabs + "?id=" + parseInt(Ext.getCmp("DocPurchTabID" + aButton.UO_id).value) + "&DocPurchID=" + parseInt(Ext.getCmp("DocPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value) + "&UO_Action=save";
    }
    
    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {

            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) {
                var locStore = Ext.getCmp(aButton.UO_idCall).getStore();
                locStore.proxy.url = HTTP_DocPurchTabs + "?DocPurchID=" + Ext.getCmp("DocPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value;
                locStore.load({ waitMsg: lanLoading });
                locStore.on('load', function () {
                    if (Ext.getCmp(aButton.UO_idMain)) {
                        controllerDocPurchesEdit_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false);
                        Ext.getCmp(aButton.UO_idMain).close();
                    }
                });
            }
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });

};

