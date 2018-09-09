Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenTypicalFaultsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceNomenTypicalFaultsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenTypicalFaultsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenTypicalFaultsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceNomenTypicalFaults,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});