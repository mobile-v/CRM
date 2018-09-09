Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirSmsTemplates/storeDirSmsTemplatesTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirSmsTemplatesTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirSmsTemplates/modelDirSmsTemplatesTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirSmsTemplates/modelDirSmsTemplatesTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirSmsTemplates,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});