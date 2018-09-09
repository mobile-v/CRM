Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirCurrencies/storeDirCurrenciesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCurrenciesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirCurrencies/modelDirCurrenciesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirCurrencies/modelDirCurrenciesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCurrencies,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});