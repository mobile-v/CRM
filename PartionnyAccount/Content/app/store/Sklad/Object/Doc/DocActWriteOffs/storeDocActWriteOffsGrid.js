//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocActWriteOffs/storeDocActWriteOffsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocActWriteOffsGrid",

    storeId: 'storeDocActWriteOffsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocActWriteOffs/modelDocActWriteOffsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocActWriteOffs,
        reader: {
            type: "json",
            rootProperty: "DocActWriteOff" //pID
        },
        timeout: varTimeOutDefault,
    }
});