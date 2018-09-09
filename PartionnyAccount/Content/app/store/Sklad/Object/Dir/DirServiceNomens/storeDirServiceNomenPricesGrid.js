Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenPricesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceNomenPricesGrid",

    storeId: 'storeDirServiceNomenPricesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenPricesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceNomenPrices,
        reader: {
            type: "json",
            rootProperty: "DirServiceNomenPrice"
        },
        timeout: varTimeOutDefault,
    }
});