Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirPriceTypes/storeDirPriceTypesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirPriceTypesGrid",

    storeId: 'storeDirPriceTypesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirPriceTypes/modelDirPriceTypesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirPriceTypes, //"api/Dir/DirPriceTypes/",
        reader: {
            type: "json",
            rootProperty: "DirPriceType" //pID
        },
        timeout: varTimeOutDefault,
    }
});