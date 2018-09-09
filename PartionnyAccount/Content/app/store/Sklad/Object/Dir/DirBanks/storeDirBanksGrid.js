Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBanks/storeDirBanksGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirBanksGrid",

    storeId: 'storeDirBanksGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBanks/modelDirBanksGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirBanks, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "DirBank" //pID
        },
        timeout: varTimeOutDefault,
    }
});