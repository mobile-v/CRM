Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirContractor2Types/storeDirContractor2TypesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirContractor2TypesGrid",

    storeId: 'storeDirContractor2TypesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirContractor2Types/modelDirContractor2TypesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirContractor2Types, //"api/Dir/DirContractor2Types/",
        reader: {
            type: "json",
            rootProperty: "DirContractor2Type" //pID
        },
        timeout: varTimeOutDefault,
    }
});