Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirContractors/storeDirContractorsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirContractorsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractors/modelDirContractorsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractors/modelDirContractorsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirContractors,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});