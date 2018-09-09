Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirCashOffices/storeDirCashOfficesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCashOfficesGrid",

    storeId: 'storeDirCashOfficesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirCashOffices/modelDirCashOfficesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCashOffices, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "DirCashOffice" //pID
        },
        timeout: varTimeOutDefault,
    }
});