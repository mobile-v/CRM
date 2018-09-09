//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSales/storeDocSalesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSalesGrid",

    storeId: 'storeDocSalesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSales/modelDocSalesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSales,
        reader: {
            type: "json",
            rootProperty: "DocSale" //pID
        },
        timeout: varTimeOutDefault,
    }
});