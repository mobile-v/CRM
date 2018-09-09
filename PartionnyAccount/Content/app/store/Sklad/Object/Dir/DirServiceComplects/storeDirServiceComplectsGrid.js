Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceComplects/storeDirServiceComplectsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceComplectsGrid",

    storeId: 'storeDirServiceComplectsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceComplects/modelDirServiceComplectsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceComplects, //"api/Dir/DirServiceComplects/",
        reader: {
            type: "json",
            rootProperty: "DirServiceComplect" //pID
        },
        timeout: varTimeOutDefault,
    }
});