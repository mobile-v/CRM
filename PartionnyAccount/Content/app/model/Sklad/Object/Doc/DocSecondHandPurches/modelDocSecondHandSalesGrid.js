//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandSalesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "KKMSCheckNumber" },
        { name: "KKMSIdCommand" },
        { name: "DocDate", type: "date" },
        { name: "Discount" },
        { name: "DocSecondHandSaleID" },
        { name: "ListObjectID" },
        { name: "ListObjectNameRu" },
        { name: "DirEmployeeName" },

        //Приходная цена
        { name: 'PriceCurrencyPurch', type: 'float', useNull: false },

        //{ name: 'DocSecondHandSaleID', type: 'int', useNull: true },
        { name: 'DirServiceNomenID', type: 'int', useNull: false },
        { name: 'DirServiceNomenName', type: 'string', useNull: false },
        //Партия
        //{ name: 'Rem2PartyID', type: 'int', useNull: false },
        //партия списания
        //{ name: 'Rem2PartyMinusID', type: 'int', useNull: false },
        //{ name: 'Quantity', type: 'float', useNull: false },
        { name: 'DirPriceTypeID', type: 'int', useNull: false },
        { name: 'DirPriceTypeName', type: 'string', useNull: false },
        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },

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

        { name: 'DirCurrencyName', type: 'string', useNull: false },
        { name: 'SUMSalePriceVATCurrency', type: 'float', useNull: false }, //Стоимость Прихода С НДС в текущей валюте


    ]
});