//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSales/storeDocSaleTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSaleTabsGrid",

    storeId: 'storeDocSaleTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSales/modelDocSaleTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSaleTabs,
        reader: {
            type: "json",
            rootProperty: "DocSaleTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});