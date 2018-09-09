Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomensGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceNomensGrid",

    storeId: 'storeDirServiceNomensGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomensGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceNomens,
        reader: {
            type: "json",
            rootProperty: "DirServiceNomen" //pID
        },
        timeout: varTimeOutDefault,
    }
});