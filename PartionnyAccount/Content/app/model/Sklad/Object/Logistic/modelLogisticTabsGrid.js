//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Logistic/modelLogisticTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [


        { name: 'DocTypeXID', type: 'int', useNull: true }, //1 - DocMovementTabID || 2 - DocSecondHandMovementTabID


        //Торговля
        { name: 'LogisticTabID', type: 'int', useNull: true }, //DocMovementTabID || DocSecondHandMovementTabID


        //Торговля
        { name: 'RemXPartyID', type: 'int', useNull: false },

        //Торговля
        { name: 'DirNomenXID', type: 'int', useNull: false }, { name: 'DirNomenXName', type: 'string', useNull: false },



        { name: 'Quantity', type: 'float', useNull: false },
        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },
        { name: 'DirCurrencyName', type: 'string', useNull: false },
        { name: 'SUMPurchPriceVATCurrency', type: 'float', useNull: false }, //Стоимость Прихода С НДС в текущей валюте


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


        //Розничная цена
        { name: 'MarkupRetail', type: 'float', useNull: false },
        //Розничная цена
        { name: 'PriceRetailVAT', type: 'float', useNull: false },
        //Розничная цена в текущей валюте
        { name: 'PriceRetailCurrency', type: 'float', useNull: false },

        //Розничная цена
        { name: 'MarkupWholesale', type: 'float', useNull: false },
        //Оптовая цена
        { name: 'PriceWholesaleVAT', type: 'float', useNull: false },
        //Оптовая цена в текущей валюте
        { name: 'PriceWholesaleCurrency', type: 'float', useNull: false },

        //Розничная цена
        { name: 'MarkupIM', type: 'float', useNull: false },
        //Интернет-Магазин
        { name: 'PriceIMVAT', type: 'float', useNull: false },
        //Интернет-Магазин в текущей валюте
        { name: 'PriceIMCurrency', type: 'float', useNull: false },


        //Комментарий: причина возврата
        { name: "DirReturnTypeID" }, { name: "DirReturnTypeName" },
        { name: "DirDescriptionID" }, { name: "DirDescriptionName" },

    ]
});