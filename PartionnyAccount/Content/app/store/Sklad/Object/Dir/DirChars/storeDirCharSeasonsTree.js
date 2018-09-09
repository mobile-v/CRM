Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharSeasonsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCharSeasonsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSeasonsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharSeasonsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCharSeasons,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});