Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharSeasonsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCharSeasonsGrid",

    storeId: 'storeDirCharSeasonsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSeasonsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCharSeasons,
        reader: {
            type: "json",
            rootProperty: "DirCharSeason" //pID
        },
        timeout: varTimeOutDefault,
    }
});