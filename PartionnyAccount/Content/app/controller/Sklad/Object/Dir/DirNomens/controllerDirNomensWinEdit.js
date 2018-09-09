Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirNomens/controllerDirNomensWinEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirNomensWinEdit': { close: this.this_close },


            //Фотка
            'viewDirNomensWinEdit [itemId=ImageLink]': { "change": this.onPanelGeneral_ImageLink },


            //Категория товара
            'viewDirNomensWinEdit [itemId=DirNomenCategoryID]': { select: this.onDirNomenCategoryIDSelect },
            'viewDirNomensWinEdit button#btnDirNomenCategoryEdit': { click: this.onBtnDirNomenCategoryEditClick },
            'viewDirNomensWinEdit button#btnDirNomenCategoryReload': { click: this.onBtnDirNomenCategoryReloadClick },
            'viewDirNomensWinEdit button#btnDirNomenCategoryClear': { "click": this.onBtnDirNomenCategoryClearClick },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirNomensWinEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirNomensWinEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirNomensWinEdit button#btnHelp': { "click": this.onBtnHelpClick },
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



    // *** DirNomenCategoryName ***
    onDirNomenCategoryIDSelect: function (combo, records, eOpts) {
        controllerDirNomens_onDirNomenCategoryIDSelect(combo, records, eOpts)
    },
    //Редактирование или добавление нового Поставщика
    onBtnDirNomenCategoryEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirNomenCategories", Params);
    },
    //РеЛоад - перегрузить, что бы появились новые записи
    onBtnDirNomenCategoryReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirNomenCategoriesGrid = Ext.getCmp(aButton.UO_idMain).storeDirNomenCategoriesGrid;
        storeDirNomenCategoriesGrid.load();
    },
    //Очистить
    onBtnDirNomenCategoryClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirNomenCategoryID" + aButton.UO_id).setValue(null);
    },



    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirNomens;
        if (parseInt(Ext.getCmp("DirNomenID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirNomens + "?id=" + parseInt(Ext.getCmp("DirNomenID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                fun_ReopenTree_1(aButton.UO_id, aButton.UO_idMain, aButton.UO_idCall, action.result.data);
                //Ext.getCmp(aButton.UO_idMain).close();
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