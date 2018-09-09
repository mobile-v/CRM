Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirSecondHandStatuses/storeDirSecondHandStatusesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirSecondHandStatusesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirSecondHandStatuses/modelDirSecondHandStatusesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirSecondHandStatuses/modelDirSecondHandStatusesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirSecondHandStatuses,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});