Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirMovementStatuses/storeDirMovementStatusesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirMovementStatusesGrid",

    storeId: 'storeDirMovementStatusesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirMovementStatuses/modelDirMovementStatusesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirMovementStatuses,
        reader: {
            type: "json",
            rootProperty: "DirMovementStatus" //pID
        },
        timeout: varTimeOutDefault,
    }
});