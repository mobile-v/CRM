//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocReturnsCustomers/storeDocReturnsCustomerTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocReturnsCustomerTabsGrid",

    storeId: 'storeDocReturnsCustomerTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocReturnsCustomers/modelDocReturnsCustomerTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocReturnsCustomerTabs,
        reader: {
            type: "json",
            rootProperty: "DocReturnsCustomerTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});