Ext.define('PartionnyAccount.model.Sklad/Object/Report/modelReportSalaries', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DocSalaryTabID', type: 'int', useNull: true },
        { name: 'DocSalaryID', type: 'int', useNull: false },
        { name: 'DirEmployeeID', type: 'int', useNull: false }, { name: 'DirEmployeeName', useNull: false },
        { name: 'DocDate', type: 'date', useNull: true },
        { name: 'DirCurrencyID', type: 'int', useNull: false }, { name: 'DirCurrencyName', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'int', useNull: false },

        { name: 'Salary', type: 'float', useNull: false },
        { name: 'SalaryDayMonthly', type: 'int', useNull: false }, { name: 'SalaryDayMonthlyName', useNull: false },
        { name: 'CountDay', type: 'int', useNull: false },
        { name: 'SumSalary', type: 'float', useNull: false },
        { name: 'SalaryFixedSalesMount', type: 'float', useNull: false }, //!!!!!!!

        { name: 'DirBonusID', type: 'int', useNull: true }, { name: 'DirBonusName' },
        { name: 'DirBonusIDSalary', type: 'float', useNull: false },

        { name: 'DirBonus2ID', type: 'int', useNull: true }, { name: 'DirBonus2Name' },
        { name: 'DirBonus2IDSalary', type: 'float', useNull: false },
        { name: 'SumSalaryFixedServiceOne', type: 'float', useNull: false }, //!!!!!!!

        //Б/У
        { name: 'DirBonus3ID', type: 'int', useNull: true }, { name: 'DirBonus3Name' },
        { name: 'DirBonus3IDSalary', type: 'float', useNull: false },
        { name: 'SumSalaryFixedSecondHandWorkshopOne', type: 'float', useNull: false }, //!!!!!!!

        { name: 'DirBonus4ID', type: 'int', useNull: true }, { name: 'DirBonus4Name' },
        { name: 'DirBonus4IDSalary', type: 'float', useNull: false },
        { name: 'SumSalaryFixedSecondHandRetailOne', type: 'float', useNull: false }, //!!!!!!!

        { name: 'sumSecondHandInventory', type: 'float', useNull: false },

        { name: 'Sums', type: 'float', useNull: false },
    ]
});