//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/List/storeListObjectPFTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeListObjectPFTabsGrid",

    storeId: 'storeListObjectPFTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/List/modelListObjectPFTabsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ListObjectPFTabs,
        reader: {
            type: "json",
            rootProperty: "ListObjectPFTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});