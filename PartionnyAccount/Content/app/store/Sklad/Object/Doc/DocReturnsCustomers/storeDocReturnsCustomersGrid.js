//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocReturnsCustomers/storeDocReturnsCustomersGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocReturnsCustomersGrid",

    storeId: 'storeDocReturnsCustomersGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocReturnsCustomers/modelDocReturnsCustomersGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocReturnsCustomers,
        reader: {
            type: "json",
            rootProperty: "DocReturnsCustomer" //pID
        },
        timeout: varTimeOutDefault,
    }
});