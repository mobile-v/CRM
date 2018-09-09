Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBonuses/storeDirBonusesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirBonusesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBonuses/modelDirBonusesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirBonuses/modelDirBonusesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirBonuses,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});