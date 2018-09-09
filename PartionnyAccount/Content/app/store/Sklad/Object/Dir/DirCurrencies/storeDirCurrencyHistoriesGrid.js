Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirCurrencies/storeDirCurrencyHistoriesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCurrencyHistoriesGrid",

    storeId: 'storeDirCurrencyHistoriesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirCurrencies/modelDirCurrencyHistoriesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCurrencyHistories,
        reader: {
            type: "json",
            rootProperty: "DirCurrencyHistory"
        },
        timeout: varTimeOutDefault,
    }
});