Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirDomesticExpenses/storeDirDomesticExpensesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirDomesticExpensesGrid",

    storeId: 'storeDirDomesticExpensesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirDomesticExpenses/modelDirDomesticExpensesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirDomesticExpenses,
        reader: {
            type: "json",
            rootProperty: "DirDomesticExpenses" //pID
        },
        timeout: varTimeOutDefault,
    }
});