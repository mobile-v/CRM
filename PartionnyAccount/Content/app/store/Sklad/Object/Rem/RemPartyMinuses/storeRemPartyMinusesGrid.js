//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Rem/RemPartyMinuses/storeRemPartyMinusesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeRemPartyMinusesGrid",

    storeId: 'storeRemPartyMinusesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Rem/RemPartyMinuses/modelRemPartyMinusesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_RemPartyMinuses,
        reader: {
            type: "json",
            rootProperty: "RemPartyMinus" //pID
        },
        timeout: varTimeOutDefault,
    }
});