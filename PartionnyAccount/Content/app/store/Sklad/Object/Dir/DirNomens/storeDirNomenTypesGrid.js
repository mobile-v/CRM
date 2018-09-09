Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirNomens/storeDirNomenTypesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirNomenTypesGrid",

    storeId: 'storeDirNomenTypesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomenTypesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirNomenTypes,
        reader: {
            type: "json",
            rootProperty: "DirNomenType"
        },
        timeout: varTimeOutDefault,
    }
});