Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirSecondHandStatuses/storeDirSecondHandStatusesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirSecondHandStatusesGrid",

    storeId: 'storeDirSecondHandStatusesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirSecondHandStatuses/modelDirSecondHandStatusesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirSecondHandStatuses,
        reader: {
            type: "json",
            rootProperty: "DirSecondHandStatus" //pID
        },
        timeout: varTimeOutDefault,
    }
});