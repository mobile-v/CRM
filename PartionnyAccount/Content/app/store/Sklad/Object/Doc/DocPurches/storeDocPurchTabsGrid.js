//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocPurches/storeDocPurchTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocPurchTabsGrid",

    storeId: 'storeDocPurchTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocPurches/modelDocPurchTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocPurchTabs,
        reader: {
            type: "json",
            rootProperty: "DocPurchTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});