Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirContractor2Types/storeDirContractor2TypesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirContractor2TypesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractor2Types/modelDirContractor2TypesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractor2Types/modelDirContractor2TypesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirContractor2Types,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});