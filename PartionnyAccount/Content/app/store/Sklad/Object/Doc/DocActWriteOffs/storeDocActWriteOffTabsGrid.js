//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocActWriteOffs/storeDocActWriteOffTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocActWriteOffTabsGrid",

    storeId: 'storeDocActWriteOffTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocActWriteOffs/modelDocActWriteOffTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocActWriteOffTabs,
        reader: {
            type: "json",
            rootProperty: "DocActWriteOffTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});