Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirNomens/modelDirNomenHistoriesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DirNomenHistoryID" },
        { name: "DirNomenID" },
        { name: "HistoryDate", type: "date" },

        { name: "HistoryDateTime", type: "date" },
        { name: "PriceVAT" },
        { name: "DirCurrencyName" },
        { name: "MarkupRetail" },
        { name: "PriceRetailVAT" },
        { name: "MarkupWholesale" },
        { name: "PriceWholesaleVAT" },
        { name: "MarkupIM" },
        { name: "PriceIMVAT" }
    ]
});