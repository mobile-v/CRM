//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocServicePurches/storeDocServicePurch1TabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocServicePurch1TabsGrid",

    storeId: 'storeDocServicePurch1TabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServicePurch1TabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocServicePurch1Tabs,
        reader: {
            type: "json",
            rootProperty: "DocServicePurch1Tab" //pID
        },
        timeout: varTimeOutDefault,
    }
});