//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Rem/RemParties/storeRem2PartiesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeRem2PartiesGrid",

    storeId: 'storeRem2PartiesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Rem/RemParties/modelRem2PartiesGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_Rem2Parties,
        reader: {
            type: "json",
            rootProperty: "Rem2Party" //pID
        },
        timeout: varTimeOutDefault,
    }
});