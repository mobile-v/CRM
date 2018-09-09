Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirDomesticExpenses/storeDirDomesticExpensesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirDomesticExpensesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirDomesticExpenses/modelDirDomesticExpensesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirDomesticExpenses/modelDirDomesticExpensesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirDomesticExpenses,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});