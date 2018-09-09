//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocRetailReturns/storeDocRetailReturnTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocRetailReturnTabsGrid",

    storeId: 'storeDocRetailReturnTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocRetailReturns/modelDocRetailReturnTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocRetailReturnTabs,
        reader: {
            type: "json",
            rootProperty: "DocRetailReturnTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});