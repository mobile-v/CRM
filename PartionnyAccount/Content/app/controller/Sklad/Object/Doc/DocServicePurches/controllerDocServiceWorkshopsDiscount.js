Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocServicePurches/controllerDocServiceWorkshopsDiscount", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({

            //Изменение скидок
            'viewDocServiceWorkshopsDiscount [itemId=DiscountX]': { change: this.onDiscountYChange },
            'viewDocServiceWorkshopsDiscount [itemId=DiscountY]': { change: this.onDiscountYChange },

            // === Кнопки ===
            'viewDocServiceWorkshopsDiscount button#btnDirPaymentTypeID1': { "click": this.onBtnDirPaymentTypeIDXClick },
            'viewDocServiceWorkshopsDiscount button#btnDirPaymentTypeID2': { "click": this.onBtnDirPaymentTypeIDXClick },
        });
    },



    //Изменили скидку - меняем сумму
    onDiscountYChange: function (aTextfield, aText) {
        var UO_id = aTextfield.UO_id;
        var
            DiscountX = parseFloat(Ext.getCmp("DiscountX" + UO_id).getValue()),
            DiscountY = parseFloat(Ext.getCmp("DiscountY" + UO_id).getValue()),
            SumTotal2a = parseFloat(Ext.getCmp("SumTotal2a" + UO_id).getValue());

        //Пересчитываем сумму
        Ext.getCmp("labelSumTotal2a" + UO_id).setText(
            "К оплате " + (SumTotal2a - (DiscountX + DiscountY)) + " руб"
        );
    },


    // Кнопки === === === === === === === === === === ===

    onBtnDirPaymentTypeIDXClick: function (aButton, aEvent, aOptions) {

        var UO_id = aButton.UO_id;
        var viewDocServiceWorkshopsDiscount = Ext.getCmp("viewDocServiceWorkshopsDiscount" + UO_id);


        //1. Проверки:
        var
            SumTotal2a = parseFloat(Ext.getCmp("SumTotal2a" + UO_id).getValue()),
            SumDocServicePurch1Tabs = parseFloat(Ext.getCmp("SumDocServicePurch1Tabs" + UO_id).getValue()),
            SumDocServicePurch2Tabs = parseFloat(Ext.getCmp("SumDocServicePurch2Tabs" + UO_id).getValue()),
            DiscountSum = parseFloat(Ext.getCmp("DiscountX" + UO_id).getValue()) + parseFloat(Ext.getCmp("DiscountY" + UO_id).getValue()),
            DiscountX = parseFloat(Ext.getCmp("DiscountX" + UO_id).getValue()),
            DiscountY = parseFloat(Ext.getCmp("DiscountY" + UO_id).getValue());
        //1.1. что бы сумма дисконта была меньше суммы:
        if ((SumTotal2a > 0 || DiscountSum > 0) && (DiscountSum > SumTotal2a)) {
            Ext.Msg.alert(lanOrgName, "Скидка (" + DiscountSum + ") больше или равна сумме (" + SumTotal2a + ") Исправьте! (1)");
            return;
        }
        //1.2. что бы сумма дисконта DiscountX была меньше суммы Выполненных работ:
        if (DiscountX > SumDocServicePurch1Tabs) {
            Ext.Msg.alert(lanOrgName, "Скидка больше сумме выполненных работ! Исправьте! (2)");
            return;
        }
        //1.3. что бы сумма дисконта DiscountY была меньше суммы Запчастей:
        if (DiscountY > SumDocServicePurch2Tabs) {
            Ext.Msg.alert(lanOrgName, "Скидка больше сумме запчастей! Исправьте! (3)");
            return;
        }
        //1.4. что бы сумма не была больше "varDiscountPercentService" в процентах от суммы "SumTotal2a"
        var MaxDiscount = (SumTotal2a * varDiscountPercentService) / 100;
        if (DiscountX + DiscountY > MaxDiscount) {
            Ext.Msg.alert(lanOrgName, "Скидка больше допустмой! Максимальная: " + MaxDiscount + "руб (" + varDiscountPercentService + "%)!<br/>Исправьте или обратитесь к Администратору!  (1)");
            return;
        }


        //Тип оплаты:
        Ext.getCmp("DirPaymentTypeID" + viewDocServiceWorkshopsDiscount.UO_Param_id.UO_id).setValue(aButton.UO_Type);
        Ext.getCmp("DiscountX" + viewDocServiceWorkshopsDiscount.UO_Param_id.UO_id).setValue(DiscountX);
        Ext.getCmp("DiscountY" + viewDocServiceWorkshopsDiscount.UO_Param_id.UO_id).setValue(DiscountY);


        //Запускаем функцию: controllerDocServiceWorkshops_ChangeStatus_Request_Union(aButton)
        viewDocServiceWorkshopsDiscount.UO_Param_fn(viewDocServiceWorkshopsDiscount.UO_Param_id, DiscountX, DiscountY);
        viewDocServiceWorkshopsDiscount.close();

    },

});