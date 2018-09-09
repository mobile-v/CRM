//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandRazborTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandRazborTabsGrid",

    storeId: 'storeDocSecondHandRazborTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRazborTabsGrid',
    pageSize: 1000000,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandRazborTabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandRazborTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});