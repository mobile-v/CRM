Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirContractors/storeDirContractorsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirContractorsGrid",

    storeId: 'storeDirContractorsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractors/modelDirContractorsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirContractors, //"api/Dir/DirContractors/",
        reader: {
            type: "json",
            rootProperty: "DirContractor" //pID
        },
        timeout: varTimeOutDefault,
    }
});