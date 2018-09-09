Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenTypicalFaultsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceNomenTypicalFaultsGrid",

    storeId: 'storeDirServiceNomenTypicalFaultsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenTypicalFaultsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceNomenTypicalFaults,
        reader: {
            type: "json",
            rootProperty: "DirServiceNomenTypicalFault"
        },
        timeout: varTimeOutDefault,
    }
});