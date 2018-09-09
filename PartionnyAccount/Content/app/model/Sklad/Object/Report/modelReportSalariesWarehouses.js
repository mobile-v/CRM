Ext.define('PartionnyAccount.model.Sklad/Object/Report/modelReportSalariesWarehouses', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DocDate', type: 'date', useNull: true },
        { name: 'TradeCash', type: 'float', useNull: false },
        { name: 'TradeBank', type: 'float', useNull: false },
        { name: 'ServiceCash', type: 'float', useNull: false },
        { name: 'ServiceBank', type: 'float', useNull: false },
        { name: 'SecondCash', type: 'float', useNull: false },
        { name: 'SecondBank', type: 'float', useNull: false },
        { name: 'TradeSumCashBank', type: 'float', useNull: false },
        { name: 'X1', type: 'float', useNull: false },
        { name: 'PurchesSum', type: 'float', useNull: false },
        { name: 'DocPurchesCashSum', type: 'float', useNull: false },
        { name: 'DocPurchesBankSum', type: 'float', useNull: false },
        { name: 'SecondCashPurch', type: 'float', useNull: false },
        { name: 'SecondBankPurch', type: 'float', useNull: false },
        { name: 'DomesticExpenses', type: 'float', useNull: false },
        { name: 'DomesticExpensesSalary', type: 'float', useNull: false },
        { name: 'Encashment', type: 'float', useNull: false },
        { name: 'InventorySum1', type: 'float', useNull: false },

        { name: 'sumSalaryPercentTrade', type: 'float', useNull: false },
        { name: 'sumSalaryPercentService1Tabs', type: 'float', useNull: false }, { name: 'sumSalaryPercentService1TabsCount', type: 'float', useNull: false },
        { name: 'sumSalaryPercentService2Tabs', type: 'float', useNull: false },
        { name: 'sumSalaryPercentSecond', type: 'float', useNull: false },

        { name: 'sumSecondHandInventory', type: 'float', useNull: false },
        { name: 'sumSalaryPercent2Second', type: 'float', useNull: false },
        { name: 'sumSalaryPercent3Second', type: 'float', useNull: false },

        { name: 'SalatyProc', type: 'float', useNull: false },
    ]
});