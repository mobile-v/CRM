//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocAccounts/storeDocAccountsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocAccountsGrid",

    storeId: 'storeDocAccountsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocAccounts/modelDocAccountsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocAccounts,
        reader: {
            type: "json",
            rootProperty: "DocAccount" //pID
        },
        timeout: varTimeOutDefault,
    }
});