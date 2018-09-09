//view=>Main (Главное окно)
Ext.define("PartionnyAccount.view.Sklad/Object/Main/viewMain", {
    //extend: "Ext.panel.Panel",
    extend: InterfaceSystemObjName,
    alias: "widget.viewMain",

    layout: 'fit',
    autoScroll: true,
    region: "center",
    bodyStyle: { "background-color": "white" },
    title: lanHome,
    width: 600, height: 300,

    UO_maximize: false,
    UO_Center: true,

    html:
        "<div style='margin-right: 10%; margin-left: 10%;'>" +

        "<center>" +
        "<span style='font-size: 14pt;'>В торговом облаке 1.2</span><br />" +
        "Система онлайн сервисов автоматизации учета и управления малым и средним бизнесом<br />" + 
        "<strong>Разработчик Печатных Форм</strong><br />" +
        "</center>" + 

        "<center>" +
        "<a target='_blank' href='https://intradecloud.com/help/'><img src='../Scripts/sklad/images/Main/manual32.png' alt='Руководство пользователя Керівництво користувача User-s Guide' title='Руководство пользователя Керівництво користувача User-s Guide' /></a>" +
        " &nbsp; &nbsp; &nbsp; <a href='mailto:support@intradecloud.com'><img src='../Scripts/sklad/images/Main/message32.png' alt='Написать письмо в службу поддержки Написати листа до служби підтримки Write a letter to support InTradeCloud' title='Написать письмо в службу поддержки Написати листа до служби підтримки Write a letter to support InTradeCloud' /></a>" +
        " &nbsp; &nbsp; &nbsp; <a target='_blank' href='https://intradecloud.com/consultations/'><img src='../Scripts/sklad/images/Main/question.png' alt='Консультации онлайн Консультації онлайн Online consultation' title='Консультации онлайн Консультації онлайн Online consultation' /></a>" +
        " &nbsp; &nbsp; &nbsp; <a target='_blank' href='https://intradecloud.com/prices/'><img src='../Scripts/sklad/images/Main/discount_1.png' alt='Цены Оплатить Ціни Оплатити Prices Pay InTradeCloud' title='Цены Оплатить Ціни Оплатити Prices Pay InTradeCloud' /></a><br />" +
        "</center>" + 

        "</div>",


    conf: {},

    initComponent: function () {
        //body
        /*
        this.id = this.conf.id;       //Name + ObjectID
        this.UO_id = this.conf.UO_id; //ObjectID
        this.UO_idMain = this.conf.UO_idMain; // == this.id
        */

        /*this.buttons = [
            {
                UO_id: this.UO_id,
                //UO_idMain: this.id,
                UO_idMain: this.UO_idMain,
                text: "Close",
                icon: "../Scripts/sklad/images/exit.png",
                itemId: "btnviewMainClose"
            }
        ],*/


        this.callParent(arguments);
    }

});