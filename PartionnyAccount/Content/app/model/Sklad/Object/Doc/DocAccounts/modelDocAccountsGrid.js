//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocAccounts/modelDocAccountsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocAccountID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorIDOrg" }, { name: "DirContractorNameOrg" },
        { name: "DirContractorID" }, { name: "DirContractorName" },
        { name: "DirWarehouseID" }, { name: "DirWarehouseName" },
        { name: "Base" },
        { name: "Description" },
        { name: "SumOfVATCurrency" },
        { name: "SumOfVATIncomeWholesale" },
        { name: "HavePay" }, //{ name: "Payment" },

        { name: "DocDateHeld", type: "date" },
        { name: "DocDatePayment", type: "date" },
    ]
});