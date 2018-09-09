Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirEmployees/storeDirEmployeesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirEmployeesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirEmployees/modelDirEmployeesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirEmployees/modelDirEmployeesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirEmployees,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});