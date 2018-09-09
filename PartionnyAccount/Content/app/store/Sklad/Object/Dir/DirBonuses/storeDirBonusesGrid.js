Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBonuses/storeDirBonusesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirBonusesGrid",

    storeId: 'storeDirBonusesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBonuses/modelDirBonusesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirBonuses, //"api/Dir/DirBonuses/",
        reader: {
            type: "json",
            rootProperty: "DirBonus" //pID
        },
        timeout: varTimeOutDefault,
    }
});