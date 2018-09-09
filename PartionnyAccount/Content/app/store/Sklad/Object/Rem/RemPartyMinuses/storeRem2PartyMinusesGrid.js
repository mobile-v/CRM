//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Rem/RemPartyMinuses/storeRem2PartyMinusesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeRem2PartyMinusesGrid",

    storeId: 'storeRem2PartyMinusesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Rem/RemPartyMinuses/modelRem2PartyMinusesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_Rem2PartyMinuses,
        reader: {
            type: "json",
            rootProperty: "Rem2PartyMinus" //pID
        },
        timeout: varTimeOutDefault,
    }
});