//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSalaries/modelDocSalaryTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DocSalaryTabID', type: 'int', useNull: true },
        { name: 'DocSalaryID', type: 'int', useNull: false },
        { name: 'DirEmployeeID', type: 'int', useNull: false }, { name: 'DirEmployeeName', useNull: false },
        { name: 'DirCurrencyID', type: 'int', useNull: false }, { name: 'DirCurrencyName', useNull: false },
        { name: 'DirCurrencyRate', type: 'float', useNull: false },
        { name: 'DirCurrencyMultiplicity', type: 'int', useNull: false },

        { name: 'Salary', type: 'float', useNull: false },
        { name: 'SalaryDayMonthly', type: 'int', useNull: false }, { name: 'SalaryDayMonthlyName', useNull: false },
        { name: 'CountDay', type: 'int', useNull: false },
        { name: 'SumSalary', type: 'float', useNull: false },

        { name: 'DirBonusID', type: 'int', useNull: true }, { name: 'DirBonusName' },
        { name: 'DirBonusIDSalary', type: 'float', useNull: false },

        { name: 'DirBonusID2', type: 'int', useNull: true }, { name: 'DirBonus2Name' },
        { name: 'DirBonusID2Salary', type: 'float', useNull: false },

        { name: 'Sums', type: 'float', useNull: false },
    ]
});