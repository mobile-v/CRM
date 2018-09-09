//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandRetailTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandRetailTabsGrid",

    storeId: 'storeDocSecondHandRetailTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRetailTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandRetailTabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandRetailTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});