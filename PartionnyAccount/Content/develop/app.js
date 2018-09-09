Ext.application({
    name: "PartionnyAccount",
    appFolder: 'Content/develop',

    views: [
        //Container
        "Sklad/Container/viewContainerHeader", "Sklad/Container/viewContainerFooter", "Sklad/Container/viewContainerLeft",
        "Sklad/Container/viewContainerCentralPanel", "Sklad/Container/viewContainerCentralTab",
        "Sklad/Container/ContainerLeft/viewPanelJournal",
        //Object
        "Sklad/Object/Main/viewMain",
        //PF
        "Sklad/Object/List/viewListObjectPFs", "Sklad/Object/List/viewListObjectPFsEdit", "Sklad/Object/List/viewListObjectPFTabsEdit",

        //Pattern
        "Sklad/Other/Pattern/viewComboBox", "Sklad/Other/Pattern/viewDateField", "Sklad/Other/Pattern/viewGridDir", "Sklad/Other/Pattern/viewGridPF", "Sklad/Other/Pattern/viewGridDirNomen", "Sklad/Other/Pattern/viewGridDoc", "Sklad/Other/Pattern/viewTriggerDir", "Sklad/Other/Pattern/viewTriggerDirField", "Sklad/Other/Pattern/viewTriggerSearch", "Sklad/Other/Pattern/viewTreeCombo", "Sklad/Other/Pattern/viewTreeDir", "Sklad/Other/Pattern/viewDirNomensEdit", "Sklad/Other/Pattern/viewGridRem",
    ],
    models: [
        //Container
        "Sklad/Container/ContainerLeft/modelPanelJournal",
        //List
        "Sklad/Object/List/modelListObjectPFsGrid",
        "Sklad/Object/List/modelListLanguagesGrid",
        "Sklad/Object/List/modelListObjectPFTabsGrid",
        "Sklad/Object/List/modelListObjectFieldNamesGrid",
    ],
    stores: [
        //Container
        "Sklad/Container/ContainerLeft/storePanelJournal",
        //List
        "Sklad/Object/List/storeListObjectPFsGrid",
        "Sklad/Object/List/storeListLanguagesGrid",
        "Sklad/Object/List/storeListObjectPFTabsGrid",
        "Sklad/Object/List/storeListObjectFieldNamesGrid",
    ],
    controllers: [
        //Container
        "Sklad/Container/controllerContainerLeft",
        //List
        "Sklad/Object/List/controllerListObjectPFs",
        "Sklad/Object/List/controllerListObjectPFsEdit",
        "Sklad/Object/List/controllerListObjectPFTabsEdit",
    ],

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
                { xtype: "viewContainerHeader", id: "viewContainerHeader" },
                { xtype: "viewContainerFooter", id: "viewContainerFooter" },
                { xtype: "viewContainerLeft", id: "viewContainerLeft" },
                //{ xtype: "viewContainerRight", id: "viewContainerRight" },

                //Разные центральные панели, в зависимости от Интерфейса
                //{ xtype: "viewContainerCentral", id: "viewContainerCentral" },
            ]
        });

        //viewContainerHeader.defaultButtonUI = "default";

        if (InterfaceSystem == 1) {
            //Для оконного нужен скролин
            Viewport.add(
                Ext.create("widget.viewContainerCentralPanel", { id: "viewContainerCentral", autoScroll: true })
            );
        }
        else if (InterfaceSystem == 3) {
            //Интерфейс "Панель" => "Ext.layout.container.Card (Wizard)"
            //http://dev.sencha.com/deploy/ext-4.0.0/examples/layout-browser/layout-browser.html

            Viewport.add(
                Ext.create("widget.viewContainerCentralPanel", {
                    id: "viewContainerCentral",

                    layout: 'card',
                    bbar: ['->', {
                        xtype: 'button',
                        text: 'Предыдущее',
                        handler: function (but) {
                            if (!funInterfaceSystem3_prev()) { Ext.Msg.alert(lanOrgName, "Нет объектов!"); }
                        }
                    }, {
                        xtype: 'button',
                        text: 'Далее',
                        handler: function (but) {
                            if (!funInterfaceSystem3_next(false)) { Ext.Msg.alert(lanOrgName, "Нет объектов!"); }
                        }
                    }],
                })
            );
        }
        else {
            Viewport.add(
                Ext.create("widget.viewContainerCentralTab", { id: "viewContainerCentral" })
            );
        }


        //Загружаем настройки
        Variables_SettingsRequest();

        //Прячем правое меню сообщений: "MessageRightPanel"
        //Ext.getCmp("viewContainerRight").collapse(Ext.Component.DIRECTION_LEFT, true);

        //Destroy the #appLoadingIndicator element
        Ext.get("loading").destroy(); Ext.get("loading-mask").destroy();
    }
});