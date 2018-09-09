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
    width: 650, height: 525,

    UO_maximize: false,
    UO_Center: true,

    html:
        "<div style='margin-right: 10%; margin-left: 10%;'>" +

        "<center>" +
        "<span style='font-size: 14pt;'>В торговом облаке 1.2</span><br />" +
        "Система онлайн сервисов автоматизации учета и управления малым и средним бизнесом<br />" + 
        "<strong>Как начать работу:</strong><br />" +
        "</center>" + 

        //"<HR />" +
        "<img src='../Scripts/sklad/images/company32.png' align='left' /><br />" +
        "<b>1 шаг</b> Вносим все Ваши юр.лица: Верхнее меню -> Настройки -> Мои фирмы. При редактировании/создании новой, в выпадающем списке выбирите 'Моя фирма'<br /><br />" +

        "<img src='../Scripts/sklad/images/Dir/employees32.png' align='left' /><br />" +
        "<b>2 шаг</b> Вносим всех Ваших сотрудников с доступом в сервис: Верхнее меню -> Настройки -> Сотрудники. Обратите внимание на вкладку 'Доступ', а так же на выбор из выпадающих списков Склад и Организацию<br /><br />" +

        "<img src='../Scripts/sklad/images/settings32_2.png' align='left' /><br />" +
        "<b>3 шаг</b> Настраиваем сервис для работы: Верхнее меню -> Настройки. В Настройках содержится информация для работы со всеми объектами (справочники, документы, ...) сервиса, а так же некоторые значения по умолчанию.<br /><br />" +

        "<img src='../Scripts/sklad/images/doc32.png'align='left' /><br />" +
        "<b>4 шаг</b> Ввод начальных остатков:<br />" +
        " &nbsp; &nbsp; &nbsp; - Через документ 'Приходная накладная' (Меню -> Торговля). Можно создать фиктивного контрагента или оприходовать сразу по приходным накладным от поставщика." +
        //"<HR />" +

        "<center>" +
        "<a target='_blank' href='https://intradecloud.com/help/'><img src='../Scripts/sklad/images/Main/manual32.png' alt='Руководство пользователя Керівництво користувача User-s Guide' title='Руководство пользователя Керівництво користувача User-s Guide' /></a>" +
        " &nbsp; &nbsp; &nbsp; <a href='mailto:support@intradecloud.com'><img src='../Scripts/sklad/images/Main/message32.png' alt='Написать письмо в службу поддержки Написати листа до служби підтримки Write a letter to support InTradeCloud' title='Написать письмо в службу поддержки Написати листа до служби підтримки Write a letter to support InTradeCloud' /></a>" +
        //" &nbsp; &nbsp; &nbsp; <a target='_blank' href='https://intradecloud.com/consultations/'><img src='../Scripts/sklad/images/Main/question.png' alt='Консультации онлайн Консультації онлайн Online consultation' title='Консультации онлайн Консультації онлайн Online consultation' /></a>" +
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