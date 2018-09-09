//Docs:
//http://www.extjs-tutorial.com/extjs5/extjs5-viewmodel
//http://www.extjs-tutorial.com/extjs-examples/stores-in-viewmodel


Ext.application({
    name: "PartionnyAccount",
    appFolder: 'Content/retail',

    views: [
        'PartionnyAccount.view.Sklad/Container/TreeList',
    ],
    models: [

    ],
    stores: [

    ],
    controllers: [
        //'PartionnyAccount.controller.Sklad/Container/TreeListController',
    ],

    requires: ['PartionnyAccount.view.Sklad/Container/TreeListModel', 'PartionnyAccount.controller.Sklad/Container/TreeListController'],


    viewport: {
        autoMaximize: true
    },

    launch: function () {

        Ext.enableAriaButtons = false; Ext.enableAriaPanels = false;

        var Viewport = Ext.create("Ext.container.Viewport", {
            layout: "border",
            style: 'background: #fff; text-align:left;',
            frame: true,
            items: [
                {
                    xtype: "tree-list",
                    id: "tree-list",
                    layout: 'border',
                    
                }
            ]
        });
        
        //Загружаем настройки
        Variables_SettingsRequest();
        //Destroy the #appLoadingIndicator element
        Ext.get("loading").destroy(); Ext.get("loading-mask").destroy();
    }
});