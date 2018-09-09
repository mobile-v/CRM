Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirEmployees/storeDirEmployeeHistoriesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirEmployeeHistoriesGrid",

    storeId: 'storeDirEmployeeHistoriesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirEmployees/modelDirEmployeeHistoriesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirEmployeeHistories,
        reader: {
            type: "json",
            rootProperty: "DirEmployeeHistory"
        },
        timeout: varTimeOutDefault,
    }
});