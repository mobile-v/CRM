Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirVats/storeDirVatsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirVatsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirVats/modelDirVatsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirVats/modelDirVatsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirVats,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});