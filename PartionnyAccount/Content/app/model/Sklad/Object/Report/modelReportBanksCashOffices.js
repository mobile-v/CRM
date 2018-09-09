Ext.define('PartionnyAccount.model.Sklad/Object/Report/modelReportBanksCashOffices', {
    extend: 'Ext.data.Model',

    fields: [
        //Примечание
        { name: "Base" },
        { name: "DirEmployeeName" },
        { name: "Description" },
        { name: "DirCurrencyName" },
        { name: "DirCurrencyMultiplicity" },
        { name: "DirCurrencyRate" },

        { name: "DocID" },
        { name: "DocXID" },

        /*
        { name: "DocCashOfficeSumDate", type: "date" },
        { name: "DirCashOfficeID" },
        { name: "DirCashOfficeName" },
        { name: "DirCashOfficeSumTypeName" },
        { name: "DocCashOfficeSumSum" },

        { name: "DocBankSumDate", type: "date" },
        { name: "DirBankID" },
        { name: "DirBankName" },
        { name: "DirBankSumTypeName" },
        { name: "DocBankSumSum" },
        */

        { name: "DocCashOfficeBankSumDate", type: "date" },
        { name: "DirCashOfficeBankID" },
        { name: "DirCashOfficeBankName" },
        { name: "DirCashOfficeBankSumTypeName" },
        { name: "DocCashOfficeBankSumSum" },
        { name: "DirWarehouseName" },
        { name: "Discount" },

        //KKMS
        { name: "KKMSCheckNumber" },
    ]
});