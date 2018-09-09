Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharSizesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCharSizesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSizesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSizesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCharSizes,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});