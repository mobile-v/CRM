Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharColoursGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCharColoursGrid",

    storeId: 'storeDirCharColoursGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharColoursGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCharColours,
        reader: {
            type: "json",
            rootProperty: "DirCharColour" //pID
        },
        timeout: varTimeOutDefault,
    }
});