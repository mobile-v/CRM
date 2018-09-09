//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocServicePurches/storeDocServiceInvTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocServiceInvTabsGrid",

    storeId: 'storeDocServiceInvTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServiceInvTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocServiceInvTabs,
        reader: {
            type: "json",
            rootProperty: "DocServiceInvTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});