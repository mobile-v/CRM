Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirEmployees/storeDirEmployeesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirEmployeesGrid",

    storeId: 'storeDirEmployeesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirEmployees/modelDirEmployeesGrid',
    pageSize: 9999999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirEmployees,
        reader: {
            type: "json",
            rootProperty: "DirEmployee" //pID
        },
        timeout: varTimeOutDefault,
    }
});