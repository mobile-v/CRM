Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirMovementDescriptions/storeDirMovementDescriptionsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirMovementDescriptionsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirMovementDescriptions/modelDirMovementDescriptionsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirMovementDescriptions/modelDirMovementDescriptionsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirMovementDescriptions,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});