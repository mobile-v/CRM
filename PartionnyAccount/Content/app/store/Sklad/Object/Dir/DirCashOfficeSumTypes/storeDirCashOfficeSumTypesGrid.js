Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirCashOfficeSumTypes/storeDirCashOfficeSumTypesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCashOfficeSumTypesGrid",

    storeId: 'storeDirCashOfficeSumTypesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirCashOfficeSumTypes/modelDirCashOfficeSumTypesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCashOfficeSumTypes, //"api/Dir/DirContractor1Types/",
        reader: {
            type: "json",
            rootProperty: "DirCashOfficeSumType" //pID
        },
        timeout: varTimeOutDefault,
    }
});