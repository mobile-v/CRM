Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirDescriptions/storeDirDescriptionsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirDescriptionsGrid",

    storeId: 'storeDirDescriptionsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirDescriptions/modelDirDescriptionsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirDescriptions, //"api/Dir/DirDescriptions/",
        reader: {
            type: "json",
            rootProperty: "DirDescription" //pID
        },
        timeout: varTimeOutDefault,
    }
});