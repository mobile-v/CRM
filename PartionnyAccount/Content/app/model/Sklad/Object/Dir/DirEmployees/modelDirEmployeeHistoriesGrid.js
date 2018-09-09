Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirEmployees/modelDirEmployeeHistoriesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DirEmployeeHistoryID", type: 'int' },
        { name: "HistoryDate", type: 'date' },
        { name: "DirCurrencyName", type: 'string' },
        { name: "Salary", type: 'float' },
        { name: "DirBonusName", type: 'string' },
        { name: "SalaryDayMonthly", type: 'string' }
    ]
});