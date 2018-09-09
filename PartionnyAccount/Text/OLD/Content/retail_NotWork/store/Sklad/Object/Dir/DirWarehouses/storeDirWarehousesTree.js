Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirWarehouses/storeDirWarehousesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirWarehousesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirWarehouses/modelDirWarehousesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirWarehouses/modelDirWarehousesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirWarehouses,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});