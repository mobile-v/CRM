Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirOrdersStates/storeDirOrdersStatesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirOrdersStatesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirOrdersStates/modelDirOrdersStatesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirOrdersStates/modelDirOrdersStatesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirOrdersStates,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});