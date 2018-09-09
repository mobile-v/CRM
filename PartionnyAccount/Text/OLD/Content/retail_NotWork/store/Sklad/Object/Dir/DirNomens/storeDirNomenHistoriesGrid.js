Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirNomens/storeDirNomenHistoriesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirNomenHistoriesGrid",

    storeId: 'storeDirNomenHistoriesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomenHistoriesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirNomenHistories,
        reader: {
            type: "json",
            rootProperty: "DirNomenHistory" //pID
        },
        timeout: varTimeOutDefault,
    }
});