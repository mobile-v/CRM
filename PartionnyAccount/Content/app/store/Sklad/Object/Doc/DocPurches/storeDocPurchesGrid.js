//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocPurches/storeDocPurchesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocPurchesGrid",

    storeId: 'storeDocPurchesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocPurches/modelDocPurchesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocPurches,
        reader: {
            type: "json",
            rootProperty: "DocPurch" //pID
        },
        timeout: varTimeOutDefault,
    }
});