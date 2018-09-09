Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirCurrencies/storeDirCurrenciesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCurrenciesGrid",

    storeId: 'storeDirCurrenciesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirCurrencies/modelDirCurrenciesGrid',
    pageSize: varPageSizeDir,
    
    /*
    fields: ['DirCurrencyID', 'DirCurrencyName', 'DirCurrencyRate', 'DirCurrencyMultiplicity', {
        name: 'display',
        convert: function (v, rec) { return rec[1] + ' - ' + rec[0] }
        // display looks like 'Texas - TX' 
    }],
    */

    proxy: {
        type: 'ajax',
        url: HTTP_DirCurrencies, //"api/Dir/DirCurrencies/",
        reader: {
            type: "json",
            rootProperty: "DirCurrency" //pID
        },
        timeout: varTimeOutDefault,
    }
});