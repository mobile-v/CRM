Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceDiagnosticRresults/storeDirServiceDiagnosticRresultsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceDiagnosticRresultsGrid",

    storeId: 'storeDirServiceDiagnosticRresultsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceDiagnosticRresults/modelDirServiceDiagnosticRresultsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceDiagnosticRresults,
        reader: {
            type: "json",
            rootProperty: "DirServiceDiagnosticRresult" //pID
        },
        timeout: varTimeOutDefault,
    }
});