Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirEmployees/storeDirEmployeeWarehousesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirEmployeeWarehousesGrid",

    storeId: 'storeDirEmployeeWarehousesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirEmployees/modelDirEmployeeWarehousesGrid',
    pageSize: 99999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirEmployeeWarehouses,
        reader: {
            type: "json",
            rootProperty: "DirEmployeeWarehouses" //pID
        },
        timeout: varTimeOutDefault,
    }
});