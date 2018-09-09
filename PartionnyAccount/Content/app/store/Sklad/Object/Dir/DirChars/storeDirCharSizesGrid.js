Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharSizesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCharSizesGrid",

    storeId: 'storeDirCharSizesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSizesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCharSizes,
        reader: {
            type: "json",
            rootProperty: "DirCharSize" //pID
        },
        timeout: varTimeOutDefault,
    }
});