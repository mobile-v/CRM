//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServicePurch1TabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DocServicePurch1TabID', type: 'int', useNull: true },
        { name: 'DocServicePurchID', type: 'int', useNull: false },

        { name: 'DirEmployeeID', type: 'int', useNull: false },
        { name: 'DirEmployeeName', type: 'string', useNull: false },

        { name: 'DirServiceJobNomenID', type: 'int', useNull: false },
        { name: 'DirServiceJobNomenName', type: 'string', useNull: false },
        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },
        { name: 'DiagnosticRresults' },

        { name: 'TabDate' },
        { name: 'DirServiceStatusID' },
        { name: 'PayDate' },
        { name: 'RemontN' },
    ]
});