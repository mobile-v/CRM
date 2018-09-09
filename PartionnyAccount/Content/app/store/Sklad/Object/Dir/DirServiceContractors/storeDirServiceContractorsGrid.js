Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceContractors/storeDirServiceContractorsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceContractorsGrid",

    storeId: 'storeDirServiceContractorsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceContractors/modelDirServiceContractorsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceContractors, //"api/Dir/DirServiceContractors/",
        reader: {
            type: "json",
            rootProperty: "DirServiceContractor" //pID
        },
        timeout: varTimeOutDefault,
    }
});