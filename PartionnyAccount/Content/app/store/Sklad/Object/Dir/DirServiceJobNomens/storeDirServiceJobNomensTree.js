Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceJobNomens/storeDirServiceJobNomensTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceJobNomensTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceJobNomens/modelDirServiceJobNomensTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceJobNomens/modelDirServiceJobNomensTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceJobNomens,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});