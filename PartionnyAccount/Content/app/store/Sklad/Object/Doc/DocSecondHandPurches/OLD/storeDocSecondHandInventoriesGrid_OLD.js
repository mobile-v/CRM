//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandInventoriesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandInventoriesGrid",

    storeId: 'storeDocSecondHandInventoriesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInventoriesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandInventories,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandInventory" //pID
        },
        timeout: varTimeOutDefault,
    }
});