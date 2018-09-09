//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocReturnVendors/storeDocReturnVendorTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocReturnVendorTabsGrid",

    storeId: 'storeDocReturnVendorTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocReturnVendors/modelDocReturnVendorTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocReturnVendorTabs,
        reader: {
            type: "json",
            rootProperty: "DocReturnVendorTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});