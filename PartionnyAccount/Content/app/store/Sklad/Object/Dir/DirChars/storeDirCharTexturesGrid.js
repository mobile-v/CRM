Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharTexturesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCharTexturesGrid",

    storeId: 'storeDirCharTexturesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharTexturesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCharTextures,
        reader: {
            type: "json",
            rootProperty: "DirCharTexture" //pID
        },
        timeout: varTimeOutDefault,
    }
});