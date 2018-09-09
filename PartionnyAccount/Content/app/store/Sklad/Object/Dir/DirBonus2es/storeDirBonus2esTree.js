Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBonus2es/storeDirBonus2esTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirBonus2esTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBonus2es/modelDirBonus2esTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirBonus2es/modelDirBonus2esTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirBonus2es,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});