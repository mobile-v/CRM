Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceContractors/storeDirServiceContractorsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceContractorsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceContractors/modelDirServiceContractorsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceContractors/modelDirServiceContractorsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceContractors,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});