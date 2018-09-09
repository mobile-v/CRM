Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceComplects/storeDirServiceComplectsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceComplectsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceComplects/modelDirServiceComplectsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceComplects/modelDirServiceComplectsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceComplects,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});