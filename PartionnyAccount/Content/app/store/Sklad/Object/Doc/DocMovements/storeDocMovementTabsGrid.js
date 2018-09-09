//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocMovements/storeDocMovementTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocMovementTabsGrid",

    storeId: 'storeDocMovementTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocMovements/modelDocMovementTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocMovementTabs,
        reader: {
            type: "json",
            rootProperty: "DocMovementTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});