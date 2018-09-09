//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocInventories/storeDocInventoryTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocInventoryTabsGrid",

    storeId: 'storeDocInventoryTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocInventories/modelDocInventoryTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocInventoryTabs,
        reader: {
            type: "json",
            rootProperty: "DocInventoryTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});