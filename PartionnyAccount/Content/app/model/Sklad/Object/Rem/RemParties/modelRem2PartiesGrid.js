//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Rem/RemParties/modelRem2PartiesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "Rem2PartyID" },
        { name: "DocSecondHandPurchID" },
        { name: "DirServiceNomenID" },
        { name: "DirServiceNomenName" },
        { name: "DocDate", type: "date" }, //Дата документа
        { name: "DocDatePurches", type: "date" }, //Дата прихода
        { name: "ListObjectID" },

        { name: "DirServiceContractorName" },
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

        { name: "DirSecondHandStatusID" }, { name: "DirSecondHandStatusName" }, //Статус

        //Характеристики
        /*
        { name: "DirCharColourID" }, { name: "DirCharColourName" },
        { name: "DirCharMaterialID" }, { name: "DirCharMaterialName" },
        { name: "DirCharNameID" }, { name: "DirCharNameName" },
        { name: "DirCharSeasonID" }, { name: "DirCharSeasonName" },
        { name: "DirCharSexID" }, { name: "DirCharSexName" },
        { name: "DirCharSizeID" }, { name: "DirCharSizeName" },
        { name: "DirCharStyleID" }, { name: "DirCharStyleName" },
        { name: "DirCharTextureID" }, { name: "DirCharTextureName" },
        { name: "DirChar" },
        */
        { name: "SerialNumber" },
        { name: "Barcode" },


        { name: "PriceWholesaleVAT" },
        { name: "PriceWholesaleCurrency" },

        { name: "PriceWholesaleVAT" },
        { name: "PriceWholesaleCurrency" },

        { name: "PriceIMVAT" },
        { name: "PriceIMCurrency" },

        { name: "DirNomenMinimumBalance" },

        { name: "DirEmployeeName" },

        //Комментарий: причина возврата
        { name: "DirReturnTypeID" }, { name: "DirReturnTypeName" },
        { name: "DirDescriptionID" }, { name: "DirDescriptionName" },
    ]
});