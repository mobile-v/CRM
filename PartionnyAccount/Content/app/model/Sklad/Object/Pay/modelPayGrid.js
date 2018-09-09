//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Pay/modelPayGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "DocXID" },
        { name: "DocCashBankID" },
        { name: "DirPaymentTypeID" },
        { name: "DirXName" },
        { name: "DirEmployeeName" },
        { name: "DirXSumTypeName" },
        { name: "DocXSumDate", type: "date" },
        { name: "DocXSumSum" },
        { name: "DirCurrencyName" },
        { name: "DirCurrencyRate" },
        { name: "DirCurrencyMultiplicity" }
    ]
});