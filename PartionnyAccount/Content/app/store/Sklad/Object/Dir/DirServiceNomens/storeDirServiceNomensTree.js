Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomensTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceNomensTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomensTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomensTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceNomens,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});