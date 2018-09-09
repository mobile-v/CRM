//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocNomenRevaluations/modelDocNomenRevaluationTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DocNomenRevaluationTabID', type: 'int', useNull: true },
        { name: 'DirNomenID', type: 'int', useNull: false },
        { name: 'DirNomenName', type: 'string', useNull: false },

        { name: 'RemPartyID', type: 'int', useNull: false },

        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },

        //Цены
        { name: 'PriceRetailVAT_OLD', type: 'string', useNull: false },
        { name: 'PriceRetailVAT', type: 'float', useNull: false },
        { name: 'PriceRetailCurrency_OLD', type: 'float', useNull: false },
        { name: 'PriceRetailCurrency', type: 'float', useNull: false },

        { name: 'PriceWholesaleVAT_OLD', type: 'float', useNull: false },
        { name: 'PriceWholesaleVAT', type: 'float', useNull: false },
        { name: 'PriceWholesaleCurrency_OLD', type: 'float', useNull: false },
        { name: 'PriceWholesaleCurrency', type: 'float', useNull: false },

        { name: 'PriceIMVAT_OLD', type: 'float', useNull: false },
        { name: 'PriceIMVAT', type: 'float', useNull: false },
        { name: 'PriceIMCurrency_OLD', type: 'float', useNull: false },
        { name: 'PriceIMCurrency', type: 'float', useNull: false },
    ]
});