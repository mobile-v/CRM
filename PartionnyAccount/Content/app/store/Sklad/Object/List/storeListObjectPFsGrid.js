//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/List/storeListObjectPFsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeListObjectPFsGrid",

    storeId: 'storeListObjectPFsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/List/modelListObjectPFsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ListObjectPFs,
        reader: {
            type: "json",
            rootProperty: "ListObjectPF" //pID
        },
        timeout: varTimeOutDefault,
    }
});