//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocServicePurches/storeDocServiceMovsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocServiceMovsGrid",

    storeId: 'storeDocServiceMovsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServiceMovsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocServiceMovs,
        reader: {
            type: "json",
            rootProperty: "DocServiceMov" //pID
        },
        timeout: varTimeOutDefault,
    }
});