Ext.define('PartionnyAccount.view.Sklad/Container/viewContainerCentral', {
    extend: 'Ext.panel.Panel',
    xtype: 'viewContainerCentral',

    layout: 'border',
    region: "center",
    title: 'ВТорговомОблаке',
    iconCls: 'fa fa-barcode',

    //Контроллер
    controller: 'controllerContainerCentral',
    //Модель
    viewModel: {
        type: 'viewmodelContainerCentral'
    },

    header: {
        items: [
            //Чек
            {
                xtype: 'button',
                text: 'Чек',
                menu: [
                    {
                        text: 'Список',
                        listeners: { click: 'onBtnDocRetailsClick' }
                    },
                    {
                        text: 'Новый',
                        listeners: { click: 'onBtnDocRetailsEditClick' }
                    }
                ]
            },
            //Возврат
            {
                xtype: 'button',
                text: 'Возврат',
                menu: [
                    {
                        text: 'Список',
                        listeners: { click: 'onBtnDocRetailReturnsClick' }
                    },
                    {
                        text: 'Новый',
                        listeners: { click: 'onBtnDocRetailReturnsEditClick' }
                    }
                ]
            },
            //Отчеты
            {
                xtype: 'button',
                text: 'Отчеты',
                menu: [
                    {
                        text: 'Денег в кассе',
                        listeners: { click: 'onBtnReportCashClick' }
                    }
                ]
            },

            //Выход
            {
                text: lanExit,
                tooltip: "<font size=" + HeaderMenu_FontSize_1 + ">" + lanExit + "</font>",
                handler:
                    function () {
                        Ext.MessageBox.show({
                            title: lanOrgName,
                            msg: lanExit2, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                            fn: function (buttons) { if (buttons == "yes") { Ext.util.Cookies.clear('CookieIPOL'); window.location.href = '/account/login/'; } }
                        });
                    }
            },
        ]
    },

    items: [
        {
            xtype: 'viewContainerCentralPanel',
            id: "viewContainerCentral",
            region: "center",
            bodyPadding: 5,
            layout: 'card'
        }
    ],

    /*
    bbar: ['->',
        {
            xtype: 'button',
            text: 'Назад',
            handler: function (but) {
                if (!funInterfaceSystem3_prev()) { Ext.Msg.alert(lanOrgName, "Нет объектов!");}
            }
        },
        {
            xtype: 'button',
            text: 'Вперёд',
            handler: function (but) {
                if (!funInterfaceSystem3_next(false)) { Ext.Msg.alert(lanOrgName, "Нет объектов!");}
            }
        }
    ]
    */

});