//Docs:
//http://www.extjs-tutorial.com/extjs5/extjs5-viewmodel
//http://www.extjs-tutorial.com/extjs-examples/stores-in-viewmodel

//MVVM - только для Контейнера
//MVC + MVVM - для всего остального

Ext.application({
    name: "PartionnyAccount",
    appFolder: 'Content/retail',

    views: [
        //Container
        'PartionnyAccount.view.Sklad/Container/viewContainerCentral', 'PartionnyAccount.view.Sklad/Container/viewContainerCentralPanel',
        //Doc
        'PartionnyAccount.view.Sklad/Object/Doc/DocRetails/viewDocRetails', 'PartionnyAccount.view.Sklad/Object/Doc/DocRetails/viewDocRetailsEdit',
        'PartionnyAccount.view.Sklad/Object/Doc/DocRetailReturns/viewDocRetailReturns', 'PartionnyAccount.view.Sklad/Object/Doc/DocRetailReturns/viewDocRetailReturnsEdit',
        //List
        'PartionnyAccount.view.Sklad/Object/List/viewListObjectPFs',
        //Report
        'PartionnyAccount.view.Sklad/Object/Report/viewReportRetailCash',

        //Pattern
        "Sklad/Other/Pattern/viewComboBox", "Sklad/Other/Pattern/viewDateField", "Sklad/Other/Pattern/viewGridDir", "Sklad/Other/Pattern/viewGridDirNomen", "Sklad/Other/Pattern/viewGridDoc", "Sklad/Other/Pattern/viewGridNoTBar", "Sklad/Other/Pattern/viewTriggerDir", "Sklad/Other/Pattern/viewTriggerDirField", "Sklad/Other/Pattern/viewTriggerSearch", "Sklad/Other/Pattern/viewTreeCombo", "Sklad/Other/Pattern/viewTreeDir", "Sklad/Other/Pattern/viewDirNomensEdit", "Sklad/Other/Pattern/viewDirServiceNomensEdit", "Sklad/Other/Pattern/viewDirServiceJobNomensEdit", "Sklad/Other/Pattern/viewGridRem",
        "Sklad/Other/Pattern/viewGridPF",
    ],
    models: [
        //Dir
        "Sklad/Object/Dir/DirNomens/modelDirNomenCategoriesGrid", "Sklad/Object/Dir/DirNomens/modelDirNomenCategoriesTree", "Sklad/Object/Dir/DirNomens/modelDirNomenHistoriesGrid", "Sklad/Object/Dir/DirNomens/modelDirNomensGrid", "Sklad/Object/Dir/DirNomens/modelDirNomensTree", "Sklad/Object/Dir/DirNomens/modelDirNomenTypesGrid", "Sklad/Object/Dir/DirNomens/modelDirNomenTypesTree",
        "Sklad/Object/Dir/DirContractors/modelDirContractorsGrid", "Sklad/Object/Dir/DirContractors/modelDirContractorsTree",
        "Sklad/Object/Dir/DirWarehouses/modelDirWarehousesGrid", "Sklad/Object/Dir/DirWarehouses/modelDirWarehousesTree",
        "Sklad/Object/Dir/DirPaymentTypes/modelDirPaymentTypesGrid",
        //Doc
        "PartionnyAccount.model.Sklad/Object/Doc/DocRetails/modelDocRetailsGrid", "PartionnyAccount.model.Sklad/Object/Doc/DocRetails/modelDocRetailTabsGrid",
        "PartionnyAccount.model.Sklad/Object/Doc/DocRetailReturns/modelDocRetailReturnsGrid", "PartionnyAccount.model.Sklad/Object/Doc/DocRetailReturns/modelDocRetailReturnTabsGrid",
        //Rem
        "PartionnyAccount.model.Sklad/Object/Rem/RemParties/modelRemPartiesGrid", "PartionnyAccount.model.Sklad/Object/Rem/RemPartyMinuses/modelRemPartyMinusesGrid",
        //List
        "PartionnyAccount.model.Sklad/Object/List/modelListObjectPFsGrid",
    ],
    stores: [
        //Doc
        "PartionnyAccount.store.Sklad/Object/Doc/DocRetails/storeDocRetailsGrid", "PartionnyAccount.store.Sklad/Object/Doc/DocRetails/storeDocRetailTabsGrid",
        "PartionnyAccount.store.Sklad/Object/Doc/DocRetailReturns/storeDocRetailReturnsGrid", "PartionnyAccount.store.Sklad/Object/Doc/DocRetailReturns/storeDocRetailReturnTabsGrid",
        //Dir
        "Sklad/Object/Dir/DirNomens/storeDirNomenCategoriesGrid", "Sklad/Object/Dir/DirNomens/storeDirNomenCategoriesTree", "Sklad/Object/Dir/DirNomens/storeDirNomenHistoriesGrid", "Sklad/Object/Dir/DirNomens/storeDirNomensGrid", "Sklad/Object/Dir/DirNomens/storeDirNomensTree", "Sklad/Object/Dir/DirNomens/storeDirNomenTypesGrid", "Sklad/Object/Dir/DirNomens/storeDirNomenTypesTree",
        "Sklad/Object/Dir/DirContractors/storeDirContractorsGrid", "Sklad/Object/Dir/DirContractors/storeDirContractorsTree",
        "Sklad/Object/Dir/DirWarehouses/storeDirWarehousesGrid", "Sklad/Object/Dir/DirWarehouses/storeDirWarehousesTree",
        "Sklad/Object/Dir/DirPaymentTypes/storeDirPaymentTypesGrid",
        //Rem
        "PartionnyAccount.store.Sklad/Object/Rem/RemParties/storeRemPartiesGrid", "PartionnyAccount.store.Sklad/Object/Rem/RemPartyMinuses/storeRemPartyMinusesGrid",
        //List
        "PartionnyAccount.store.Sklad/Object/List/storeListObjectPFsGrid",
    ],
    controllers: [ ],

    //MVVM для Контейнера и Объектов
    requires: [
        "PartionnyAccount.viewmodel.Sklad/Container/viewmodelContainerCentral", "PartionnyAccount.viewcontroller.Sklad/Container/controllerContainerCentral",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetails/viewcontrollerDocRetails", "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetails/viewcontrollerDocRetailsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetailReturns/viewcontrollerDocRetailReturns", "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetailReturns/viewcontrollerDocRetailReturnsEdit",
        //List
        "PartionnyAccount.viewcontroller.Sklad/Object/List/viewcontrollerListObjectPFs",
        //Report
        "PartionnyAccount.viewcontroller.Sklad/Object/Report/viewcontrollerReportRetailCash",
    ],


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
                    xtype: "viewContainerCentral",
                    id: "viewContainerCentralX",
                    region: "center",
                    layout: 'border',

                    /*
                    bbar: ['->', {
                        xtype: 'button',
                        text: 'Предыдущее',
                        handler: function (but) {
                            if (!funInterfaceSystem3_prev()) { Ext.Msg.alert(lanOrgName, "Нет объектов!");}
                        }
                    }, {
                        xtype: 'button',
                        text: 'Далее',
                        handler: function (but) {
                            if (!funInterfaceSystem3_next(false)) { Ext.Msg.alert(lanOrgName, "Нет объектов!");}
                        }
                    }],
                    */
                },

                /*
                {
                    xtype: 'panel',
                    height: 40,
                    region: "south",
                    title: "<center>" + varCopyrightSystem + lanOrgName + "</center>"
                },
                */
            ]
        });
        

        
        /*
        var treelist = Ext.getCmp('treelist');  //var treelist = this.lookupReference('treelist');
        treelist.setConfig("singleExpand", false);


        var treelist = Ext.getCmp('treelist'),
            ct = Ext.getCmp('treelistContainer');

        treelist.setExpanderFirst(!true);
        treelist.setUi(true ? 'nav' : null);
        treelist.setHighlightPath(true);
        ct[true ? 'addCls' : 'removeCls']('x-treelist-navigation');
        */
        


        //Загружаем настройки
        Variables_SettingsRequest();
        //Destroy the #appLoadingIndicator element
        Ext.get("loading").destroy(); Ext.get("loading-mask").destroy();
    }
});