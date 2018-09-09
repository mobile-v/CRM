Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirVats/storeDirVatsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirVatsGrid",

    storeId: 'storeDirVatsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirVats/modelDirVatsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirVats, //"api/Dir/DirVats/",
        reader: {
            type: "json",
            rootProperty: "DirVat" //pID
        },
        timeout: varTimeOutDefault,
    }
});