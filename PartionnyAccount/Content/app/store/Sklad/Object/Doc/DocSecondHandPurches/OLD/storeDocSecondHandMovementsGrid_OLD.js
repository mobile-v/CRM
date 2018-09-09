//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandMovementsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandMovementsGrid",

    storeId: 'storeDocSecondHandMovementsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovementsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandMovements,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandMovement" //pID
        },
        timeout: varTimeOutDefault,
    }
});