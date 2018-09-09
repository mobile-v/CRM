Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirReturnTypes/storeDirReturnTypesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirReturnTypesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirReturnTypes/modelDirReturnTypesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirReturnTypes/modelDirReturnTypesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirReturnTypes,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});