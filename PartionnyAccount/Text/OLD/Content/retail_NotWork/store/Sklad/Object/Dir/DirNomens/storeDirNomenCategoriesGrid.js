Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirNomens/storeDirNomenCategoriesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirNomenCategoriesGrid",

    storeId: 'storeDirNomenCategoriesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomenCategoriesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirNomenCategories,
        reader: {
            type: "json",
            rootProperty: "DirNomenCategory"
        },
        timeout: varTimeOutDefault,
    }
});