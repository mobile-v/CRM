//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocDomesticExpenses/modelDocDomesticExpenseTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "DocDate", type: "date" },
        { name: "Discount" },
        { name: "DocDomesticExpenseID" },
        { name: "ListObjectID" },
        { name: "ListObjectNameRu" },
        { name: "DirEmployeeName" },
        { name: "DirEmployeeNameSpisat" },
        { name: "DirPaymentTypeName" },

        { name: 'DocDomesticExpenseTabID', type: 'int', useNull: true },
        { name: 'DirDomesticExpenseID', type: 'int', useNull: false },
        { name: 'DirDomesticExpenseName', type: 'string', useNull: false },

        //Сумма
        { name: 'PriceVAT', type: 'float', useNull: false }, //Приходная цена (в валюте или в рублях)
        { name: 'PriceCurrency', type: 'float', useNull: false }, //Приходная цена в текущей валюте
        { name: 'DirCurrencyID', type: 'int', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'float', useNull: false },

        { name: 'DirCurrencyName', type: 'string', useNull: false },
        { name: 'SUMSalePriceVATCurrency', type: 'float', useNull: false }, //Стоимость Прихода С НДС в текущей валюте


    ]
});