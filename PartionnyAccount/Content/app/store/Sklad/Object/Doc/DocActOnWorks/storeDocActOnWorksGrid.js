//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocActOnWorks/storeDocActOnWorksGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocActOnWorksGrid",

    storeId: 'storeDocActOnWorksGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocActOnWorks/modelDocActOnWorksGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocActOnWorks,
        reader: {
            type: "json",
            rootProperty: "DocActOnWork" //pID
        },
        timeout: varTimeOutDefault,
    }
});