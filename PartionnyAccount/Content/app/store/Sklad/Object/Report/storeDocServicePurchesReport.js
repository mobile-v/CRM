Ext.define('PartionnyAccount.store.Sklad/Object/Report/storeDocServicePurchesReport', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocServicePurchesReport",

    storeId: 'storeDocServicePurchesReport',
    model: 'PartionnyAccount.model.Sklad/Object/Report/modelDocServicePurchesReport',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocServicePurchesReport, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "DocServicePurchesReport" //pID
        },
        timeout: varTimeOutDefault,
    }
});