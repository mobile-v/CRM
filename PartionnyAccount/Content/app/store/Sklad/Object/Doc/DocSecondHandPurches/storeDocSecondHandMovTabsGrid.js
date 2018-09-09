//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandMovTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandMovTabsGrid",

    storeId: 'storeDocSecondHandMovTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandMovTabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandMovTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});