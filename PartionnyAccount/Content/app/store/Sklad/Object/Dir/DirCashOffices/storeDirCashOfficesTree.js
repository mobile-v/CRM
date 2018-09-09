Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirCashOffices/storeDirCashOfficesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCashOfficesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirCashOffices/modelDirCashOfficesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirCashOffices/modelDirCashOfficesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCashOffices,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});