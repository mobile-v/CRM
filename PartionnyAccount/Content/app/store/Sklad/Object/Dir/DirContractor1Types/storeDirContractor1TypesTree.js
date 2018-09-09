Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirContractor1Types/storeDirContractor1TypesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirContractor1TypesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractor1Types/modelDirContractor1TypesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractor1Types/modelDirContractor1TypesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirContractor1Types,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});