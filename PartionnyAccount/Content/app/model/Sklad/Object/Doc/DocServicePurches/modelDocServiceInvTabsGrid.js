//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServiceInvTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [

        //DocServiceInvTabs
        { name: 'DocServiceInvTabID', type: 'int', useNull: true },
        { name: 'DocServiceInvID', type: 'int', useNull: true },
        { name: 'DirServiceNomenID', type: 'int', useNull: true },
        //{ name: 'Rem2PartyID', type: 'int', useNull: true },
        //{ name: 'Quantity', type: 'float', useNull: false },
        //{ name: 'DirPriceTypeID', type: 'int', useNull: true },
        
        { name: 'PriceVAT', type: 'string' },
        { name: 'PriceCurrency', type: 'string' },
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'int', useNull: false },

        { name: 'PrepaymentSum', type: 'float', useNull: false },
        { name: 'Sums1', type: 'float', useNull: false },
        { name: 'Sums2', type: 'float', useNull: false },


        { name: 'DirServiceStatusName' },

        { name: 'Exist' },
        { name: 'ExistName' },

        { name: 'IsAdmin' },
        { name: 'IsAdminNameRu' },


        //Партии
        { name: 'DocID', type: 'int', useNull: true },
        { name: 'DocDate', type: "date", useNull: true },

    ]
});