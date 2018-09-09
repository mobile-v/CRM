Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirSmsTemplates/storeDirSmsTemplatesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirSmsTemplatesGrid",

    storeId: 'storeDirSmsTemplatesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirSmsTemplates/modelDirSmsTemplatesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirSmsTemplates,
        reader: {
            type: "json",
            rootProperty: "DirSmsTemplate" //pID
        },
        timeout: varTimeOutDefault,
    }
});