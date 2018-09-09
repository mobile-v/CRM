Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBanks/storeDirBanksTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirBanksTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBanks/modelDirBanksTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirBanks/modelDirBanksTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirBanks,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});