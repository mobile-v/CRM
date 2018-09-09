//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandRazborsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandRazborsGrid",

    storeId: 'storeDocSecondHandRazborsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRazborsGrid',
    pageSize: 1000000,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandRazbors,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandRazbor" //pID
        },
        timeout: varTimeOutDefault,
    }
});