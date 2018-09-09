//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocAccounts/storeDocAccountTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocAccountTabsGrid",

    storeId: 'storeDocAccountTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocAccounts/modelDocAccountTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocAccountTabs,
        reader: {
            type: "json",
            rootProperty: "DocAccountTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});