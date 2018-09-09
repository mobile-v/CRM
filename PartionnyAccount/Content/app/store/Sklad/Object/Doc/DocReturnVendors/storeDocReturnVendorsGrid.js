//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocReturnVendors/storeDocReturnVendorsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocReturnVendorsGrid",

    storeId: 'storeDocReturnVendorsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocReturnVendors/modelDocReturnVendorsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocReturnVendors,
        reader: {
            type: "json",
            rootProperty: "DocReturnVendor" //pID
        },
        timeout: varTimeOutDefault,
    }
});