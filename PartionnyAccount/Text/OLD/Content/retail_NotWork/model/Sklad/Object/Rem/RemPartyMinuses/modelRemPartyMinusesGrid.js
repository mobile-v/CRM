//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Rem/RemPartyMinuses/modelRemPartyMinusesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "RemPartyMinusID" },
        { name: "RemPartyID" },
        { name: "DirNomenID" },
        { name: "DirNomenName" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorName" },
        { name: "DirVatValue" },
        { name: "DirWarehouseID" }, { name: "DirWarehouseName" },
        { name: "ListDocNameRu" },
        { name: "PriceVAT" },
        { name: "PriceCurrency" },
        { name: "Quantity" },
        { name: "DirCurrencyID" },
        { name: "DirCurrencyName" },
        { name: "DirCurrencyRate" },
        { name: "DirCurrencyMultiplicity" }
    ]
});