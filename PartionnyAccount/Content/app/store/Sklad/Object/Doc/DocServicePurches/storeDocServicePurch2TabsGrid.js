//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocServicePurches/storeDocServicePurch2TabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocServicePurch2TabsGrid",

    storeId: 'storeDocServicePurch2TabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServicePurch2TabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocServicePurch2Tabs,
        reader: {
            type: "json",
            rootProperty: "DocServicePurch2Tab" //pID
        },
        timeout: varTimeOutDefault,
    }
});