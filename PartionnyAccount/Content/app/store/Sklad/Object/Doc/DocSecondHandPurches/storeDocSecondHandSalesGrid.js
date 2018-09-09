//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandSalesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandSalesGrid",

    storeId: 'storeDocSecondHandSalesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandSalesGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandSales,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandSale" //pID
        },
        timeout: varTimeOutDefault,
    }
});