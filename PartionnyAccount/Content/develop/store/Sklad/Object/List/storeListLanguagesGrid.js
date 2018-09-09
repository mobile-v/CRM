//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/List/storeListLanguagesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeListLanguagesGrid",

    storeId: 'storeListLanguagesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/List/modelListLanguagesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ListLanguages,
        reader: {
            type: "json",
            rootProperty: "ListLanguage" //pID
        },
        timeout: varTimeOutDefault,
    }
});