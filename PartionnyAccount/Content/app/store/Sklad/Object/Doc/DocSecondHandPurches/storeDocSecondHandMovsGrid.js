//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandMovsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSecondHandMovsGrid",

    storeId: 'storeDocSecondHandMovsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSecondHandMovs,
        reader: {
            type: "json",
            rootProperty: "DocSecondHandMov" //pID
        },
        timeout: varTimeOutDefault,
    }
});