//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocActOnWorks/modelDocActOnWorkTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DocActOnWorkTabID', type: 'int', useNull: true },
        { name: 'DirNomenID', type: 'int', useNull: false },
        { name: 'DirNomenName', type: 'string', useNull: false },
        { name: 'Quantity', type: 'float', useNull: false },
        { name: 'DirPriceTypeID', type: 'int', useNull: false },
        { name: 'DirPriceTypeName', type: 'string', useNull: false },
        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },

        { name: 'DirCurrencyName', type: 'string', useNull: false },
        { name: 'SUMPurchPriceVATCurrency', type: 'float', useNull: false }, //Стоимость Прихода С НДС в текущей валюте
    ]
});