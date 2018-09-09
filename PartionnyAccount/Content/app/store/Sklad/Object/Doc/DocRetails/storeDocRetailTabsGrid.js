//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocRetails/storeDocRetailTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocRetailTabsGrid",

    storeId: 'storeDocRetailTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocRetails/modelDocRetailTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocRetailTabs,
        reader: {
            type: "json",
            rootProperty: "DocRetailTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});