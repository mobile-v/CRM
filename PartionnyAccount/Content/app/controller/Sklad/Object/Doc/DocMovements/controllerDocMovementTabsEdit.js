Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocMovements/controllerDocMovementTabsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocMovementTabsEdit': { close: this.this_close },

            //'viewDocMovementTabsEdit #DirNomenName': { "ontriggerclick": this.onTriggerDirNomenNameClick },
            //'viewDocMovementTabsEdit button#btnDirNomenHistory': { "click": this.onbtnDirNomenHistoryClick },



            //Характеристики
            //Colour
            'viewDocMovementTabsEdit button#btnCharColourEdit': { "click": this.onBtnCharColourEditClick },
            'viewDocMovementTabsEdit button#btnCharColourReload': { "click": this.onBtnCharColourReloadClick },
            'viewDocMovementTabsEdit button#btnCharColourClear': { "click": this.onBtnCharColourClearClick },
            //Material
            'viewDocMovementTabsEdit button#btnCharMaterialEdit': { "click": this.onBtnCharMaterialEditClick },
            'viewDocMovementTabsEdit button#btnCharMaterialReload': { "click": this.onBtnCharMaterialReloadClick },
            'viewDocMovementTabsEdit button#btnCharMaterialClear': { "click": this.onBtnCharMaterialClearClick },
            //Name
            'viewDocMovementTabsEdit button#btnCharNameEdit': { "click": this.onBtnCharNameEditClick },
            'viewDocMovementTabsEdit button#btnCharNameReload': { "click": this.onBtnCharNameReloadClick },
            'viewDocMovementTabsEdit button#btnCharNameClear': { "click": this.onBtnCharNameClearClick },
            //Season
            'viewDocMovementTabsEdit button#btnCharSeasonEdit': { "click": this.onBtnCharSeasonEditClick },
            'viewDocMovementTabsEdit button#btnCharSeasonReload': { "click": this.onBtnCharSeasonReloadClick },
            'viewDocMovementTabsEdit button#btnCharSeasonClear': { "click": this.onBtnCharSeasonClearClick },
            //Sex
            'viewDocMovementTabsEdit button#btnCharSexEdit': { "click": this.onBtnCharSexEditClick },
            'viewDocMovementTabsEdit button#btnCharSexReload': { "click": this.onBtnCharSexReloadClick },
            'viewDocMovementTabsEdit button#btnCharSexClear': { "click": this.onBtnCharSexClearClick },
            //Size
            'viewDocMovementTabsEdit button#btnCharSizeEdit': { "click": this.onBtnCharSizeEditClick },
            'viewDocMovementTabsEdit button#btnCharSizeReload': { "click": this.onBtnCharSizeReloadClick },
            'viewDocMovementTabsEdit button#btnCharSizeClear': { "click": this.onBtnCharSizeClearClick },
            //Style
            'viewDocMovementTabsEdit button#btnCharStyleEdit': { "click": this.onBtnCharStyleEditClick },
            'viewDocMovementTabsEdit button#btnCharStyleReload': { "click": this.onBtnCharStyleReloadClick },
            'viewDocMovementTabsEdit button#btnCharStyleClear': { "click": this.onBtnCharStyleClearClick },
            //Texture
            'viewDocMovementTabsEdit button#btnCharTextureEdit': { "click": this.onBtnCharTextureEditClick },
            'viewDocMovementTabsEdit button#btnCharTextureReload': { "click": this.onBtnCharTextureReloadClick },
            'viewDocMovementTabsEdit button#btnCharTextureClear': { "click": this.onBtnCharTextureClearClick },


            //Currencies
            'viewDocMovementTabsEdit [itemId=DirCurrencyID]': { select: this.onDirCurrencyIDSelect },
            //'viewDocMovementTabsEdit [itemId=DirCurrencyID]': { change: this.onDirCurrencyIDSelect },

            'viewDocMovementTabsEdit button#btnCurrencyEdit': { "click": this.onBtnCurrencyEditClick },
            'viewDocMovementTabsEdit button#btnCurrencyReload': { "click": this.onBtnCurrencyReloadClick },
            'viewDocMovementTabsEdit button#btnCurrencyClear': { "click": this.onBtnCurrencyClearClick },


            'viewDocMovementTabsEdit [itemId=DirCurrencyRate]': { change: this.onDirCurrencyRateChange },
            'viewDocMovementTabsEdit [itemId=DirCurrencyMultiplicity]': { change: this.onDirCurrencyMultiplicityChange },


            'viewDocMovementTabsEdit [itemId=PriceVAT]': { change: this.onPriceVATChange },
            'viewDocMovementTabsEdit [itemId=PriceCurrency]': { change: this.onPriceCurrencyChange },

            'viewDocMovementTabsEdit [itemId=MarkupRetail]': { change: this.onMarkupRetailChange },
            'viewDocMovementTabsEdit [itemId=PriceRetailVAT]': { change: this.onPriceRetailVATChange },
            'viewDocMovementTabsEdit [itemId=PriceRetailCurrency]': { change: this.onPriceRetailCurrencyChange },

            'viewDocMovementTabsEdit [itemId=MarkupWholesale]': { change: this.onMarkupWholesaleChange },
            'viewDocMovementTabsEdit [itemId=PriceWholesaleVAT]': { change: this.onPriceWholesaleVATChange },
            'viewDocMovementTabsEdit [itemId=PriceWholesaleCurrency]': { change: this.onPriceWholesaleCurrencyChange },

            'viewDocMovementTabsEdit [itemId=MarkupIM]': { change: this.onMarkupIMChange },
            'viewDocMovementTabsEdit [itemId=PriceIMVAT]': { change: this.onPriceIMVATChange },
            'viewDocMovementTabsEdit [itemId=PriceIMCurrency]': { change: this.onPriceIMCurrencyChange },

            'viewDocMovementTabsEdit [itemId=Quantity]': { change: this.onQuantityChange },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocMovementTabsEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocMovementTabsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocMovementTabsEdit button#btnDel': { "click": this.onBtnDelClick },
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


    //Поменяли "Приходную цену"
    onQuantityChange: function (aTextfield, aText) {
        var id = aTextfield.UO_id;
        Quantity = Ext.getCmp("Quantity" + id);

        //1. Quantity, если undefined, isNaN или "", то ставим её == 0
        if (isNaN(parseFloat(Ext.getCmp("Quantity" + id).getValue()))) { Ext.getCmp("Quantity" + id).setValue(0); }

        //2. Проверяем к-во
        if (parseFloat(Ext.getCmp("Quantity" + id).getValue()) > parseFloat(Ext.getCmp("Remnant" + id).getValue())) {
            Ext.Msg.alert(lanOrgName, "К-во товара на остатке меньше, чем Вы списываете! Исправте к-во!");
            return;
        }
    },




    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        fun_SaveTabDoc1(aButton, controllerDocMovementsEdit_RecalculationSums);
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