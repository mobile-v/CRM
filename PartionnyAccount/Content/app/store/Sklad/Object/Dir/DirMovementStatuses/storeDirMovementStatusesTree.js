Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirMovementStatuses/storeDirMovementStatusesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirMovementStatusesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirMovementStatuses/modelDirMovementStatusesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirMovementStatuses/modelDirMovementStatusesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirMovementStatuses,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});