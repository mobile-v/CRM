Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirNomens/storeDirNomensGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirNomensGrid",

    storeId: 'storeDirNomensGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomensGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirNomens,
        reader: {
            type: "json",
            rootProperty: "DirNomen" //pID
        },
        timeout: varTimeOutDefault,
    }
});