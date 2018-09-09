Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharStylesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCharStylesGrid",

    storeId: 'storeDirCharStylesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharStylesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCharStyles,
        reader: {
            type: "json",
            rootProperty: "DirCharStyle" //pID
        },
        timeout: varTimeOutDefault,
    }
});