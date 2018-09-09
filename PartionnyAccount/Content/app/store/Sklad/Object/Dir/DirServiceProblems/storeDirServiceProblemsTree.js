Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceProblems/storeDirServiceProblemsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceProblemsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceProblems/modelDirServiceProblemsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceProblems/modelDirServiceProblemsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceProblems,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});