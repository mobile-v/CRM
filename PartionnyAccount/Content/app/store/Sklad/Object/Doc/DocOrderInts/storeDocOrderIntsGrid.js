//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocOrderInts/storeDocOrderIntsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocOrderIntsGrid",

    storeId: 'storeDocOrderIntsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocOrderInts/modelDocOrderIntsGrid',
    pageSize: 1000000,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocOrderInts,
        reader: {
            type: "json",
            rootProperty: "DocOrderInt" //pID
        },
        timeout: varTimeOutDefault,
    }
});