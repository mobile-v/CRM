Ext.application({
    name: "PartionnyAccount",
    appFolder: 'Content/retail',

    views: [
        "Sklad/Container/viewContainerCentralPanel",
        "Sklad/Container/viewContainerCentral",
    ],
    models: [ ],
    stores: [ ],
    controllers: ["Sklad/Container/controllerContainerCentral", ],

    viewport: {
        autoMaximize: true
    },

    launch: function () {

        //Отключить проверку на соответствие "WAI-ARIA 1.0" рекомендаций (а то выводит в консели ошибки, что не соответствуют кнопки рекомендациям "WAI-ARIA 1.0")
        //forum: https://www.sencha.com/forum/showthread.php?303936-Button-issue-with-WAI-ARIA
        //help: https://docs.sencha.com/extjs/6.0/whats_new/6.0.0/extjs_upgrade_guide.html#Button
        Ext.enableAriaButtons = false;
        Ext.enableAriaPanels = false;

        var Viewport = Ext.create("Ext.container.Viewport", {
            layout: "border",
            style: 'background: #fff; text-align:left;',
            frame: true,
            items: [
                {
                    xtype: "viewContainerCentral",
                    id: "viewContainerCentral",
                    layout: 'border',
                    /*
                    bbar: [
                        '->',
                        {
                            xtype: 'button',
                            text: 'Предыдущее',
                            handler: function (but) {
                                if (!funInterfaceSystem3_prev()) { Ext.Msg.alert(lanOrgName, "Нет объектов!"); }
                            }
                        },
                        {
                            xtype: 'button',
                            text: 'Далее',
                            handler: function (but) {
                                if (!funInterfaceSystem3_next(false)) { Ext.Msg.alert(lanOrgName, "Нет объектов!"); }
                            }
                        }
                    ],
                    */
                }
            ]
        });
        
        //Загружаем настройки
        Variables_SettingsRequest();
        //Destroy the #appLoadingIndicator element
        Ext.get("loading").destroy(); Ext.get("loading-mask").destroy();
    }
});