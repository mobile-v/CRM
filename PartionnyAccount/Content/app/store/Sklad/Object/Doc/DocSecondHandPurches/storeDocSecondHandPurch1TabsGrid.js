//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandPurch1TabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandPurch1TabsGrid",

    storeId: 'storeDocSecondHandPurch1TabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandPurch1TabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandPurch1Tabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandPurch1Tab" //pID
        },
        timeout: varTimeOutDefault,
    }
});