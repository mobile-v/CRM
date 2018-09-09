Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceJobNomens/storeDirServiceJobNomensGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceJobNomensGrid",

    storeId: 'storeDirServiceJobNomensGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceJobNomens/modelDirServiceJobNomensGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceJobNomens,
        reader: {
            type: "json",
            rootProperty: "DirServiceJobNomen" //pID
        },
        timeout: varTimeOutDefault,
    }
});