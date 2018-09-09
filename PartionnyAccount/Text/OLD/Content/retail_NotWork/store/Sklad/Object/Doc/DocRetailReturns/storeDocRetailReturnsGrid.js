//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocRetailReturns/storeDocRetailReturnsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocRetailReturnsGrid",

    storeId: 'storeDocRetailReturnsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocRetailReturns/modelDocRetailReturnsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocRetailReturns,
        reader: {
            type: "json",
            rootProperty: "DocRetailReturn" //pID
        },
        timeout: varTimeOutDefault,
    }
});