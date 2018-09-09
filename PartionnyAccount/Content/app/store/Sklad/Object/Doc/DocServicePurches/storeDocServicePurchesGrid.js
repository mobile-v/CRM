//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocServicePurches/storeDocServicePurchesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocServicePurchesGrid",

    storeId: 'storeDocServicePurchesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServicePurchesGrid',
    pageSize: 1000000,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocServicePurches,
        reader: {
            type: "json",
            rootProperty: "DocServicePurch" //pID
        },
        timeout: varTimeOutDefault,
    }
});