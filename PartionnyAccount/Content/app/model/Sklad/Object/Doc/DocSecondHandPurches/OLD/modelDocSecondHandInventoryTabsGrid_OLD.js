//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInventoryTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [

        //DocSecondHandInventoryTabs
        { name: 'DocSecondHandInventoryTabID', type: 'int', useNull: true },
        { name: 'DocSecondHandInventoryID', type: 'int', useNull: true },
        { name: 'DirServiceNomenID', type: 'int', useNull: true },
        { name: 'Rem2PartyID', type: 'int', useNull: true },
        { name: 'Quantity', type: 'float', useNull: false },
        { name: 'DirPriceTypeID', type: 'int', useNull: true },
        { name: 'PriceVAT', type: 'float', useNull: false },
        { name: 'PriceCurrency', type: 'float', useNull: false },
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'int', useNull: false },

        { name: 'Exist' },
        { name: 'ExistName' },

        { name: 'IsAdmin' },
        { name: 'IsAdminNameRu' },


        //Партии
        { name: 'DocID', type: 'int', useNull: true },
        { name: 'DocDate', type: "date", useNull: true },

    ]
});