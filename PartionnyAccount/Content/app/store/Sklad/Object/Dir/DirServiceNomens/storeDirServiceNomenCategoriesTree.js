Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenCategoriesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceNomenCategoriesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenCategoriesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenCategoriesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceNomenCategories,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});