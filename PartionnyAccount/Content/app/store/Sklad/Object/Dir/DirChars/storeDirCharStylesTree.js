Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharStylesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCharStylesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharStylesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharStylesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCharStyles,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});