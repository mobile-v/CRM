//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocInventories/storeDocInventoriesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocInventoriesGrid",

    storeId: 'storeDocInventoriesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocInventories/modelDocInventoriesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocInventories,
        reader: {
            type: "json",
            rootProperty: "DocInventory" //pID
        },
        timeout: varTimeOutDefault,
    }
});