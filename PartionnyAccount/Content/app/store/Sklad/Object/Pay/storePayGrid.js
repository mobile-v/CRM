//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Pay/storePayGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storePayGrid",

    storeId: 'storePayGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Pay/modelPayGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_Pays,
        reader: {
            type: "json",
            rootProperty: "Pay" //pID
        },
        timeout: varTimeOutDefault,
    }
});