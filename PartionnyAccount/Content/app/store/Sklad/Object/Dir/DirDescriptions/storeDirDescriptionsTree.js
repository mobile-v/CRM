Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirDescriptions/storeDirDescriptionsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirDescriptionsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirDescriptions/modelDirDescriptionsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirDescriptions/modelDirDescriptionsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirDescriptions,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});