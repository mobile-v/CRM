Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharNamesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCharNamesGrid",

    storeId: 'storeDirCharNamesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharNamesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCharNames,
        reader: {
            type: "json",
            rootProperty: "DirCharName" //pID
        },
        timeout: varTimeOutDefault,
    }
});