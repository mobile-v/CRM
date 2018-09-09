Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharSexesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCharSexesGrid",

    storeId: 'storeDirCharSexesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSexesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCharSexes,
        reader: {
            type: "json",
            rootProperty: "DirCharSex" //pID
        },
        timeout: varTimeOutDefault,
    }
});