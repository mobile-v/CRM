Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirNomens/storeDirNomenTypesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirNomenTypesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomenTypesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomenTypesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirNomenTypes,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});