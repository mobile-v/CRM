//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandPurchesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandPurchesGrid",

    storeId: 'storeDocSecondHandPurchesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandPurchesGrid',
    pageSize: 1000000,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandPurches,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandPurch" //pID
        },
        timeout: varTimeOutDefault,
    }
});