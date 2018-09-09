Ext.define("PartionnyAccount.controller.Sklad/Object/Service/IM/controllerVitrinaInTrade", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewVitrinaInTrade': { close: this.this_close },

            //PanelDocumentDetails
            //Контрагент - Перегрузить
            'viewVitrinaInTrade #Slider_DirNomen1Name': { "mousedown": this.onTriggerSlider_DirNomen1NameClick },
            'viewVitrinaInTrade #Slider_DirNomen1Name': { "ontriggerclick": this.onTriggerSlider_DirNomen1NameClick },
            'viewVitrinaInTrade textfield#Slider_DirNomen1Name': {
                "ontriggerclick": this.onTriggerSlider_DirNomen1NameClick,
                ontriggerclick: this.onTriggerSlider_DirNomen1NameClick,
                keypress: this.onTriggerSlider_DirNomen1NameClick
            },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewVitrinaInTrade button#btnSave': { "click": this.onBtnSaveClick },
            'viewVitrinaInTrade button#btnCancel': { "click": this.onBtnCancelClick },
            'viewVitrinaInTrade button#btnHelp': { "click": this.onBtnHelpClick }

        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    // Кнопки === === === === === === === === === === ===
    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirWebShopUO;
        if (parseInt(Ext.getCmp("DirWebShopUOID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirWebShopUO + "?id=" + parseInt(Ext.getCmp("DirWebShopUOID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                Ext.getCmp(aButton.UO_idMain).close();
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "internet-magazin-vitrina/", '_blank');
    }


});