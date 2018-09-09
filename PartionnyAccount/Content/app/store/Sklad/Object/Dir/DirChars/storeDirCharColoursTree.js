Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharColoursTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCharColoursTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharColoursTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharColoursTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCharColours,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});