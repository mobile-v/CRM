//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandInvsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandInvsGrid",

    storeId: 'storeDocSecondHandInvsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInvsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandInvs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandInv" //pID
        },
        timeout: varTimeOutDefault,
    }
});