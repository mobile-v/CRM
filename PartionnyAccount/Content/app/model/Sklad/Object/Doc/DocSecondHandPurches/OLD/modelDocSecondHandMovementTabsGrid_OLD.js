//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovementTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'Rem2PartyID', type: 'int', useNull: false },
        { name: 'DocSecondHandMovementTabID', type: 'int', useNull: true },
        { name: 'DirServiceNomenID', type: 'int', useNull: false },
        { name: 'DirServiceNomenName', type: 'string', useNull: false },
        { name: 'Quantity', type: 'float', useNull: false },
        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },
        { name: 'DirCurrencyName', type: 'string', useNull: false },
        { name: 'SUMPurchPriceVATCurrency', type: 'float', useNull: false }, //Стоимость Прихода С НДС в текущей валюте


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