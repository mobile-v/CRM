//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandPurch2TabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandPurch2TabsGrid",

    storeId: 'storeDocSecondHandPurch2TabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandPurch2TabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandPurch2Tabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandPurch2Tab" //pID
        },
        timeout: varTimeOutDefault,
    }
});