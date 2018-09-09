Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenPricesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirServiceNomenPricesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenPricesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenPricesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirServiceNomenPrices,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});