//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServicePurch2TabsGrid', {
    extend: 'Ext.data.Model',

    fields: [

        { name: 'IsZakaz', type: 'bool', useNull: true },

        { name: 'DocServicePurch2TabID', type: 'int', useNull: true },
        { name: 'DocServicePurchID', type: 'int', useNull: false },

        { name: 'DirEmployeeID', type: 'int', useNull: false },
        { name: 'DirEmployeeName', type: 'string', useNull: false },

        { name: 'DirNomenID', type: 'int', useNull: false },
        { name: 'DirNomenName', type: 'string', useNull: false },
        { name: 'RemPartyID', type: 'int', useNull: false },
        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },

        { name: 'TabDate' },
        { name: 'PayDate' },
        { name: 'RemontN' },
    ]
});