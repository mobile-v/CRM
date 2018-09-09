Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceStatuses/storeDirServiceStatusesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceStatusesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceStatuses/modelDirServiceStatusesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceStatuses/modelDirServiceStatusesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceStatuses,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});