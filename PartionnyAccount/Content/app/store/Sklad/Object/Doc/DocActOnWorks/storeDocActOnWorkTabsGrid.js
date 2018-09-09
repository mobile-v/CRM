//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocActOnWorks/storeDocActOnWorkTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocActOnWorkTabsGrid",

    storeId: 'storeDocActOnWorkTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocActOnWorks/modelDocActOnWorkTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocActOnWorkTabs,
        reader: {
            type: "json",
            rootProperty: "DocActOnWorkTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});