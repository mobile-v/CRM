Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirMovementDescriptions/storeDirMovementDescriptionsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirMovementDescriptionsGrid",

    storeId: 'storeDirMovementDescriptionsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirMovementDescriptions/modelDirMovementDescriptionsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirMovementDescriptions, //"api/Dir/DirMovementDescriptions/",
        reader: {
            type: "json",
            rootProperty: "DirMovementDescription" //pID
        },
        timeout: varTimeOutDefault,
    }
});