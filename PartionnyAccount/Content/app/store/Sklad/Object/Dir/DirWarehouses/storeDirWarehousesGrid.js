Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirWarehouses/storeDirWarehousesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirWarehousesGrid",

    storeId: 'storeDirWarehousesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirWarehouses/modelDirWarehousesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirWarehouses, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "DirWarehouse" //pID
        },
        timeout: varTimeOutDefault,
    }
});