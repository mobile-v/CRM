Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirServiceJobNomens/controllerDirServiceJobNomensWinEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirServiceJobNomensWinEdit': { close: this.this_close },


            //Фотка
            'viewDirServiceJobNomensWinEdit [itemId=ImageLink]': { "change": this.onPanelGeneral_ImageLink },


            //Категория товара
            'viewDirServiceJobNomensWinEdit button#btnDirServiceJobNomenCategoryEdit': { click: this.onBtnDirServiceJobNomenCategoryEditClick },
            'viewDirServiceJobNomensWinEdit button#btnDirServiceJobNomenCategoryReload': { click: this.onBtnDirServiceJobNomenCategoryReloadClick },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirServiceJobNomensWinEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirServiceJobNomensWinEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirServiceJobNomensWinEdit button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    // *** Фотка ***
    onPanelGeneral_ImageLink: function (aTextfield, aValueReal, aValuePrevious) {
        try {
            Ext.getCmp("imageShow" + aTextfield.UO_id).setSrc(aValueReal);
        } catch (ex) { alert(e.name); }

    },



    // *** DirServiceJobNomenCategoryName ***
    //Редактирование или добавление нового Поставщика
    onBtnDirServiceJobNomenCategoryEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirServiceJobNomenCategories", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirServiceJobNomenCategoryReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirServiceJobNomenCategoriesGrid = Ext.getCmp(aButton.UO_idMain).storeDirServiceJobNomenCategoriesGrid;
        storeDirServiceJobNomenCategoriesGrid.load();
    },



    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //Цены
        Ext.getCmp("PriceWholesaleVAT" + aButton.UO_id).setValue(Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue());
        Ext.getCmp("PriceIMVAT" + aButton.UO_id).setValue(Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue());

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirServiceJobNomens;
        if (parseInt(Ext.getCmp("DirServiceJobNomenID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirServiceJobNomens + "?id=" + parseInt(Ext.getCmp("DirServiceJobNomenID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                fun_ReopenTree_1(aButton.UO_id, undefined, aButton.UO_idCall, action.result.data);
                Ext.getCmp(aButton.UO_idMain).close();
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-tovar/", '_blank');
    }
});