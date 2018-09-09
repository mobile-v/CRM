Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharTexturesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCharTexturesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharTexturesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharTexturesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCharTextures,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});