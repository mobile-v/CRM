Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceJobNomens/storeDirServiceJobNomenHistoriesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceJobNomenHistoriesGrid",

    storeId: 'storeDirServiceJobNomenHistoriesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceJobNomens/modelDirServiceJobNomenHistoriesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceJobNomenHistories,
        reader: {
            type: "json",
            rootProperty: "DirServiceJobNomenHistory" //pID
        },
        timeout: varTimeOutDefault,
    }
});