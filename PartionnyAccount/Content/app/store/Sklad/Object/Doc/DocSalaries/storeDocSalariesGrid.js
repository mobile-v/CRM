//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSalaries/storeDocSalariesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSalariesGrid",

    storeId: 'storeDocSalariesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSalaries/modelDocSalariesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSalaries,
        reader: {
            type: "json",
            rootProperty: "DocSalary" //pID
        },
        timeout: varTimeOutDefault,
    }
});