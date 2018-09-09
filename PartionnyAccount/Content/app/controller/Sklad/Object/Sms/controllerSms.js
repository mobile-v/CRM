Ext.define("PartionnyAccount.controller.Sklad/Object/Sms/controllerSms", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewSms': { close: this.this_close },

            // Клик по Гриду
            'viewSms [itemId=gridSms]': {
                selectionchange: this.ongridSms_selectionchange,
                itemclick: this.ongridSms_itemclick,
                itemdblclick: this.ongridSms_itemdblclick
            },


            'viewSms button#btnSend': { click: this.onBtnSendClick },

        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Кнопки редактирования Енеблед
    ongridSms_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    ongridSms_itemclick: function (view, record, item, index, eventObj) {
        
        //id
        var UO_id = view.grid.UO_id;
        //загрузка в форму из грида
        var widgetXForm = Ext.getCmp("form_" + UO_id);
        var form = widgetXForm.getForm();
        form.loadRecord(Ext.getCmp("gridSms_" + UO_id).getSelectionModel().getSelection()[0]);

        //ID-шник вызвавшей формы
        var UO_id_Call = Ext.getCmp(view.grid.UO_idCall).UO_id;

        var Msg = Ext.getCmp("DirSmsTemplateMsg" + UO_id).getValue();
        
        if (Ext.getCmp("DirServiceNomenNameLittle" + UO_id_Call)) {
            //Парсим текст: "[[[ТоварНаименование]]]" и "[[[Сумма]]]"
            Msg = Msg.replace("[[[ДокументНомер]]]", Ext.getCmp("DocServicePurchID" + UO_id).getValue());
            //Msg = Msg.replace("[[[ТоварНаименование]]]", Ext.getCmp("DirServiceNomenNameLittle" + UO_id_Call);
            Msg = Msg.replace("[[[ТоварНаименование]]]", Ext.getCmp("DirServiceNomenName" + UO_id_Call).getValue());
            Msg = Msg.replace("[[[Гарантия]]]", fun_addMonth(new Date(), Ext.getCmp("ServiceTypeRepair" + UO_id_Call).getValue()));

            //Сумма без предоплаты (если вернули на доработку, то сумма будет другая)
            if (parseFloat(Ext.getCmp("PrepaymentSum_1" + UO_id_Call).getValue()) > 0) {
                alert("Аппарат вернули на доработку!");
                Msg = Msg.replace("[[[СуммаВся]]]", Ext.getCmp("SumTotal2a" + UO_id_Call).getValue());
            }
            else {
                Msg = Msg.replace("[[[СуммаВся]]]", Ext.getCmp("SumTotal" + UO_id_Call).getValue());
            }

            //Сумма - Предоплата
            Msg = Msg.replace("[[[СуммаМинусПредоплата]]]", Ext.getCmp("SumTotal2a" + UO_id_Call).getValue());
        }
        else if (Ext.getCmp("DocMovementID" + UO_id_Call)) {
            //Парсим текст: "[[[ДокументНомер]]]" и "[[[ТочкаОт]]]"
            Msg = Msg.replace("[[[ДокументНомер]]]", Ext.getCmp("DocMovementID" + UO_id).getValue());
            Msg = Msg.replace("[[[ТочкаОт]]]", Ext.getCmp("DirWarehouseNameFrom" + UO_id).getValue());
        }
        else if (Ext.getCmp("DocSecondHandMovementID" + UO_id_Call)) {
            //Парсим текст: "[[[ДокументНомер]]]" и "[[[ТочкаОт]]]"
            Msg = Msg.replace("[[[ДокументНомер]]]", Ext.getCmp("DocSecondHandMovementID" + UO_id).getValue());
            Msg = Msg.replace("[[[ТочкаОт]]]", Ext.getCmp("DirWarehouseNameFrom" + UO_id).getValue());
        }
        else if (Ext.getCmp("DocOrderIntID" + UO_id_Call)) {
            Msg = Msg.replace("[[[ТоварНаименование]]]", Ext.getCmp("DirNomenXName6" + UO_id_Call).getValue());
            Msg = Msg.replace("[[[ТочкаНа]]]", Ext.getCmp("DirWarehouseNameFrom" + UO_id_Call).getValue());
        }


        //Общие
        Msg = Msg.replace("[[[Организация]]]", varDirContractorNameOrg);
        if (Ext.getCmp("DirWarehouseName" + UO_id_Call)) {
            Msg = Msg.replace("[[[ТочкаНа]]]", Ext.getCmp("DirWarehouseName" + UO_id_Call).getValue());
        }
        if (Ext.getCmp("DirWarehouseAddress" + UO_id_Call)) {
            Msg = Msg.replace("[[[ТочкаАдрес]]]", Ext.getCmp("DirWarehouseAddress" + UO_id_Call).getValue());
        }
        if (Ext.getCmp("Phone" + UO_id_Call)) {
            Msg = Msg.replace("[[[ТочкаТелефон]]]", Ext.getCmp("Phone" + UO_id_Call).getValue());
        }   


        Ext.getCmp("DirSmsTemplateMsg" + UO_id).setValue(Msg);
    },
    //ДаблКлик: Редактирования или выбор
    ongridSms_itemdblclick: function (view, record, item, index, e) {
        
    },



    onBtnSendClick: function (aButton, aEvent, aOptions) {

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        var _id = 0, ListObjectID = 0;
        if (Ext.getCmp("DocServicePurchID" + aButton.UO_id).value != "") { _id = parseInt(Ext.getCmp("DocServicePurchID" + aButton.UO_id).value); ListObjectID = 40; }
        else { _id = parseInt(Ext.getCmp("DocMovementID" + aButton.UO_id).value); ListObjectID = 33; }

        //Сохранение
        widgetXForm.submit({
            method: "PUT",
            url: HTTP_Sms + "?id=" + _id + "&ListObjectID=" + ListObjectID,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                if (action.result.success) {
                    //Ext.Msg.alert(lanOrgName, action.result.data.Msg);
                    Ext.getCmp("viewSms" + aButton.UO_id).close();
                }
                else {
                    Ext.Msg.alert(lanOrgName, action.result.data.Msg);
                }
            },
            failure: function (form, action) {
                funPanelSubmitFailure(form, action);
            }
        });

    },

});