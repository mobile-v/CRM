//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocMovements/storeDocMovementsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocMovementsGrid",

    storeId: 'storeDocMovementsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocMovements/modelDocMovementsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocMovements,
        reader: {
            type: "json",
            rootProperty: "DocMovement" //pID
        },
        timeout: varTimeOutDefault,
    }
});