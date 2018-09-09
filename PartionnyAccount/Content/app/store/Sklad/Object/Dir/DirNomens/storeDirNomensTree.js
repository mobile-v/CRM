Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirNomens/storeDirNomensTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirNomensTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomensTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomensTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirNomens,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});