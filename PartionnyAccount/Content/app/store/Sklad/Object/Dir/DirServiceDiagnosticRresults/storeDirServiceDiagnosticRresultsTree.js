Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceDiagnosticRresults/storeDirServiceDiagnosticRresultsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceDiagnosticRresultsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceDiagnosticRresults/modelDirServiceDiagnosticRresultsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceDiagnosticRresults/modelDirServiceDiagnosticRresultsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceDiagnosticRresults,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});