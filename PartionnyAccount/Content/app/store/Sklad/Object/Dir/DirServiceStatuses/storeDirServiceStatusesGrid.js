Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceStatuses/storeDirServiceStatusesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceStatusesGrid",

    storeId: 'storeDirServiceStatusesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceStatuses/modelDirServiceStatusesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceStatuses,
        reader: {
            type: "json",
            rootProperty: "DirServiceStatus" //pID
        },
        timeout: varTimeOutDefault,
    }
});