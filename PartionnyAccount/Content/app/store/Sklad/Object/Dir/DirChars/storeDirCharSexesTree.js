Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharSexesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCharSexesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSexesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSexesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCharSexes,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});