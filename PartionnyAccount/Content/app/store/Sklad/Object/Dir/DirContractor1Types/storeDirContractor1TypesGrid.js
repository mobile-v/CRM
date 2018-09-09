Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirContractor1Types/storeDirContractor1TypesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirContractor1TypesGrid",

    storeId: 'storeDirContractor1TypesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractor1Types/modelDirContractor1TypesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirContractor1Types, //"api/Dir/DirContractor1Types/",
        reader: {
            type: "json",
            rootProperty: "DirContractor1Type" //pID
        },
        timeout: varTimeOutDefault,
    }
});