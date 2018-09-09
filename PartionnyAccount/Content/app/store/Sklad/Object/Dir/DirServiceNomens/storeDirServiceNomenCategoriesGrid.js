Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenCategoriesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirServiceNomenCategoriesGrid",

    storeId: 'storeDirServiceNomenCategoriesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenCategoriesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceNomenCategories,
        reader: {
            type: "json",
            rootProperty: "DirServiceNomenCategory"
        },
        timeout: varTimeOutDefault,
    }
});