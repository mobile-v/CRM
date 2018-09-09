//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandMovementTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandMovementTabsGrid",

    storeId: 'storeDocSecondHandMovementTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovementTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandMovementTabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandMovementTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});