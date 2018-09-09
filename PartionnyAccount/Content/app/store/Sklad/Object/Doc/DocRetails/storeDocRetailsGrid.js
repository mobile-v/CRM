Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocRetails/storeDocRetailsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocRetailsGrid",

    storeId: 'storeDocRetailsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocRetails/modelDocRetailsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocRetails,
        reader: {
            type: "json",
            rootProperty: "DocRetail" //pID
        },
        timeout: varTimeOutDefault,
    }
});