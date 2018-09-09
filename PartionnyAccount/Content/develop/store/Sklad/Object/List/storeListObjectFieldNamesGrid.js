//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/List/storeListObjectFieldNamesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeListObjectFieldNamesGrid",

    storeId: 'storeListObjectFieldNamesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/List/modelListObjectFieldNamesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ListObjectFieldNames,
        reader: {
            type: "json",
            rootProperty: "ListObjectFieldName" //pID
        },
        timeout: varTimeOutDefault,
    }
});