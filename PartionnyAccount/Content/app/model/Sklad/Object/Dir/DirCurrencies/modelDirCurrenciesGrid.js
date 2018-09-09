Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirCurrencies/modelDirCurrenciesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DirCurrencyID" },
        { name: "Del" },
        //{ name: "DirCurrencyCode" },
        //{ name: "DirCurrencyNameShort" },
        { name: "DirCurrencyName" },
        { name: "DirCurrencyRate" },
        { name: "DirCurrencyMultiplicity" }
    ]
});