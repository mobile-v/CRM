Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirOrdersStates/storeDirOrdersStatesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirOrdersStatesGrid",

    storeId: 'storeDirOrdersStatesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirOrdersStates/modelDirOrdersStatesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirOrdersStates,
        reader: {
            type: "json",
            rootProperty: "DirOrdersState" //pID
        },
        timeout: varTimeOutDefault,
    }
});