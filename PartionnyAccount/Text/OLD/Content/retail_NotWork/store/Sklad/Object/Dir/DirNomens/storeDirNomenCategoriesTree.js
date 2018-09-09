Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirNomens/storeDirNomenCategoriesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirNomenCategoriesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomenCategoriesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomenCategoriesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirNomenCategories,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});