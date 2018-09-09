//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandInventoryTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandInventoryTabsGrid",

    storeId: 'storeDocSecondHandInventoryTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInventoryTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandInventoryTabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandInventoryTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});