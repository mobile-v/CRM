//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocDomesticExpenses/storeDocDomesticExpenseTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocDomesticExpenseTabsGrid",

    storeId: 'storeDocDomesticExpenseTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocDomesticExpenses/modelDocDomesticExpenseTabsGrid',
    pageSize: 999999,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: HTTP_DocDomesticExpenseTabs,
        reader: {
            type: "json",
            rootProperty: "DocDomesticExpenseTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});