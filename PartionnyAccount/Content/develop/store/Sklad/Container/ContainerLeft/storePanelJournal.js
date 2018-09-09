Ext.define('PartionnyAccount.store.Sklad/Container/ContainerLeft/storePanelJournal', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storePanelJournal",

    model: 'PartionnyAccount.model.Sklad/Container/ContainerLeft/modelPanelJournal',
    requires: 'PartionnyAccount.model.Sklad/Container/ContainerLeft/modelPanelJournal',

    proxy: {
        type: 'ajax',
        url: HTTP_ListObjects + "?ListObjectTypeID=3",
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});