Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceProblems/storeDirServiceProblemsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceProblemsGrid",

    storeId: 'storeDirServiceProblemsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceProblems/modelDirServiceProblemsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceProblems, //"api/Dir/DirServiceProblems/",
        reader: {
            type: "json",
            rootProperty: "DirServiceProblem" //pID
        },
        timeout: varTimeOutDefault,
    }
});