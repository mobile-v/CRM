//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Rem/RemParties/modelRemPartiesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "RemPartyID" },
        { name: "DirNomenID" },
        { name: "DirNomenName" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorName" },
        { name: "DirVatValue" },
        { name: "DirWarehouseID" },
        { name: "DirWarehouseName" },
        { name: "ListDocNameRu" },
        { name: "PriceVAT" },
        { name: "PriceCurrency" },
        { name: "Quantity" },
        { name: "Remnant" },
        { name: "DirCurrencyID" },
        { name: "DirCurrencyName" },
        { name: "DirCurrencyRate" },
        { name: "DirCurrencyMultiplicity" },


        //Характеристики
        { name: "DirCharColourID" }, { name: "DirCharColourName" },
        { name: "DirCharMaterialID" }, { name: "DirCharMaterialName" },
        { name: "DirCharNameID" }, { name: "DirCharNameName" },
        { name: "DirCharSeasonID" }, { name: "DirCharSeasonName" },
        { name: "DirCharSexID" }, { name: "DirCharSexName" },
        { name: "DirCharSizeID" }, { name: "DirCharSizeName" },
        { name: "DirCharStyleID" }, { name: "DirCharStyleName" },
        { name: "DirCharTextureID" }, { name: "DirCharTextureName" },
        { name: "DirChar" },
        { name: "SerialNumber" },
        { name: "Barcode" },


        { name: "PriceWholesaleVAT" },
        { name: "PriceWholesaleCurrency" },

        { name: "PriceWholesaleVAT" },
        { name: "PriceWholesaleCurrency" },

        { name: "PriceIMVAT" },
        { name: "PriceIMCurrency" }
    ]
});