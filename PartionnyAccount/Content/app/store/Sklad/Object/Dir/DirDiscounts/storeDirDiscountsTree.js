Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirDiscounts/storeDirDiscountsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirDiscountsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirDiscounts/modelDirDiscountsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirDiscounts/modelDirDiscountsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirDiscounts,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});