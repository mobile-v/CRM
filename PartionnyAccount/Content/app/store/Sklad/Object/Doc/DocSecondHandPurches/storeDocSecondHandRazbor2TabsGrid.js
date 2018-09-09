//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandRazbor2TabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandRazbor2TabsGrid",

    storeId: 'storeDocSecondHandRazbor2TabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRazbor2TabsGrid',
    pageSize: 1000000,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandRazbor2Tabs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandRazbor2Tab" //pID
        },
        timeout: varTimeOutDefault,
    }
});