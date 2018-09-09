//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocServicePurches/storeDocServiceMovTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocServiceMovTabsGrid",

    storeId: 'storeDocServiceMovTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServiceMovTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocServiceMovTabs,
        reader: {
            type: "json",
            rootProperty: "DocServiceMovTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});