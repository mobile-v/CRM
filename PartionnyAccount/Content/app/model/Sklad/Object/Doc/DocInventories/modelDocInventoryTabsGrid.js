//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocInventories/modelDocInventoryTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DocInventoryTabID', type: 'int', useNull: true },
        { name: 'DirNomenID', type: 'int', useNull: false },
        { name: 'DirNomenName', type: 'string', useNull: false },
        { name: 'RemPartyID', type: 'int', useNull: false },
        //{ name: 'DirPriceTypeID', type: 'int', useNull: false },

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

        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },

        { name: 'Quantity_WriteOff', type: 'float', useNull: false },
        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте

        { name: 'Quantity_Purch', type: 'float', useNull: false },
        { name: 'PriceRetailVAT', type: 'float', useNull: false }, { name: 'PriceRetailCurrency', type: 'float', useNull: false },
        { name: 'PriceWholesaleVAT', type: 'float', useNull: false }, { name: 'PriceWholesaleCurrency', type: 'float', useNull: false },
        { name: 'PriceIMVAT', type: 'float', useNull: false }, { name: 'PriceIMCurrency', type: 'float', useNull: false },

        { name: 'DirCurrencyName', type: 'string', useNull: false },
        { name: 'SUMPurchPriceVATCurrency', type: 'float', useNull: false }, //Стоимость Прихода С НДС в текущей валюте

        //Поставщика от которого пришла партия первоначально - этот параметр передаётся во все другие партии (напр. перемещение)
        { name: "DirContractorID" },
        { name: "DirNomenMinimumBalance" },
    ]
});