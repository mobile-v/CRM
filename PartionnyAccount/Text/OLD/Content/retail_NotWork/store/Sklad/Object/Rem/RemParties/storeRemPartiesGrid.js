//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Rem/RemParties/storeRemPartiesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeRemPartiesGrid",

    storeId: 'storeRemPartiesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Rem/RemParties/modelRemPartiesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_RemParties,
        reader: {
            type: "json",
            rootProperty: "RemParty" //pID
        },
        timeout: varTimeOutDefault,
    }
});