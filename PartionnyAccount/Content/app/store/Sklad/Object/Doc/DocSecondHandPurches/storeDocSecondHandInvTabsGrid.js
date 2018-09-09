//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandInvTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandInvTabsGrid",

    storeId: 'storeDocSecondHandInvTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInvTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandInvTabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandInvTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});