//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocSalaries/storeDocSalaryTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocSalaryTabsGrid",

    storeId: 'storeDocSalaryTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocSalaries/modelDocSalaryTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocSalaryTabs,
        reader: {
            type: "json",
            rootProperty: "DocSalaryTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});