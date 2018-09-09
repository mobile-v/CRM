Ext.define("PartionnyAccount.controller.Sklad/Object/Service/API/controllerAPI10", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewAPI10': { close: this.this_close },

            //Генерация ключа
            'viewAPI10 button#btnGen': { "click": this.onBtnGenClick },

            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewAPI10 button#btnSave': { "click": this.onBtnSaveClick },
            'viewAPI10 button#btnCancel': { "click": this.onBtnCancelClick },
            'viewAPI10 button#btnHelp': { "click": this.onBtnHelpClick }
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Генерация ключа
    onBtnGenClick: function (aButton, aEvent, aOptions) {

        Ext.MessageBox.show({
            title: lanOrgName, msg: "Сгенерировать новый код (старый будет не действительным, после сохранения)?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {

                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        waitMsg: lanUpload,
                        url: HTTP_Api10 + "1/?pID=KeyGen",
                        method: 'GET',
                        success: function (result) {
                            var sData = Ext.decode(result.responseText);
                            if (sData.success == false) { Ext.Msg.alert(lanOrgName, lanError + "<BR>" + sData); }
                            else {
                                Ext.getCmp("API10Key" + aButton.UO_id).setValue(sData);
                            }
                        },
                        failure: function (form, action) { PanelSubmitFailure(form, action); }
                    });

                }
            }
        });

    },


    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        //var sMethod = "POST";
        //var sUrl = HTTP_Api10;


        var sMethod = "POST";
        var sUrl = HTTP_Api10 + "?UO_Action=" + aButton.UO_Action;
        if (parseInt(Ext.getCmp("API10ID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_Api10 + "?id=" + parseInt(Ext.getCmp("API10ID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
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
        window.open(HTTP_Help + "api-1/", '_blank');
    }

});