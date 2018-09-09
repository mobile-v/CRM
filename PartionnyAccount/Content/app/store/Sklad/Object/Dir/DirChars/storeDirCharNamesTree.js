Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharNamesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCharNamesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharNamesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharNamesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCharNames,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});