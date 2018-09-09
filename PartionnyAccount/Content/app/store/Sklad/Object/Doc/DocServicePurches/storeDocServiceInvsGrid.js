//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocServicePurches/storeDocServiceInvsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocServiceInvsGrid",

    storeId: 'storeDocServiceInvsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServiceInvsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocServiceInvs,
        reader: {
            type: "json",
            rootProperty: "DocServiceInv" //pID
        },
        timeout: varTimeOutDefault,
    }
});