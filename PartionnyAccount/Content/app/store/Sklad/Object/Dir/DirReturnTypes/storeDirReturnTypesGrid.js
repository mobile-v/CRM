Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirReturnTypes/storeDirReturnTypesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirReturnTypesGrid",

    storeId: 'storeDirReturnTypesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirReturnTypes/modelDirReturnTypesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirReturnTypes, //"api/Dir/DirReturnTypes/",
        reader: {
            type: "json",
            rootProperty: "DirReturnType" //pID
        },
        timeout: varTimeOutDefault,
    }
});