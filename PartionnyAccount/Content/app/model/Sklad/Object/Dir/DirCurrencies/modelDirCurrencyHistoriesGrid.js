Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirCurrencies/modelDirCurrencyHistoriesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DirCurrencyHistoryID", type: 'int' },
        { name: "HistoryDate", type: 'date' },
        { name: "DirCurrencyName", type: 'string' },
        { name: "DirCurrencyRate", type: 'float' },
        { name: "DirCurrencyMultiplicity", type: 'int' }
    ]
});