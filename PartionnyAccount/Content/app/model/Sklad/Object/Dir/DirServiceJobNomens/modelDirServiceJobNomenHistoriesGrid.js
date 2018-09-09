Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirServiceJobNomens/modelDirServiceJobNomenHistoriesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        /*
        { name: "DirServiceJobNomenHistoryID" },
        { name: "DirServiceJobNomenID" },
        { name: "HistoryDate", type: "date" },

        { name: "HistoryDateTime", type: "date" },
        { name: "PriceVAT" },
        */
        { name: "DirServiceJobNomenID" },
        { name: "DirServiceJobNomenName" },

        { name: "PriceVAT" },

        { name: "DirCurrencyID" },
        { name: "DirCurrencyName" },
        { name: "DirCurrencyRate" },
        { name: "DirCurrencyMultiplicity" },

        { name: "PriceRetailVAT" },
        { name: "PriceRetailCurrency" },

        { name: "PriceWholesaleVAT" },
        { name: "PriceWholesaleCurrency" },

        { name: "PriceIMVAT" },
        { name: "PriceIMCurrency" },
    ]
});