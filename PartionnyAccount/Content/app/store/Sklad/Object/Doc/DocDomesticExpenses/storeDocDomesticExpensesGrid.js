Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocDomesticExpenses/storeDocDomesticExpensesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocDomesticExpensesGrid",

    storeId: 'storeDocDomesticExpensesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocDomesticExpenses/modelDocDomesticExpensesGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocDomesticExpenses,
        reader: {
            type: "json",
            rootProperty: "DocDomesticExpense" //pID
        },
        timeout: varTimeOutDefault,
    }
});