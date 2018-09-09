Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBankSumTypes/storeDirBankSumTypesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirBankSumTypesGrid",

    storeId: 'storeDirBankSumTypesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBankSumTypes/modelDirBankSumTypesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirBankSumTypes, //"api/Dir/DirContractor1Types/",
        reader: {
            type: "json",
            rootProperty: "DirBankSumType" //pID
        },
        timeout: varTimeOutDefault,
    }
});