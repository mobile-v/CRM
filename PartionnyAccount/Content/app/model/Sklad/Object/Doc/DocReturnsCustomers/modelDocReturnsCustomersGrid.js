//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocReturnsCustomers/modelDocReturnsCustomersGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocReturnsCustomerID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "NumberTT" },
        { name: "NumberTax" },
        { name: "DirContractorNameOrg" },
        { name: "DirContractorName" },
        { name: "DirWarehouseName" },
        { name: "Base" },
        { name: "Description" },
        { name: "SumOfVATCurrency" },
        { name: "SumOfVATIncomeWholesale" },
        { name: "HavePay" }, //{ name: "Payment" },

        { name: "DocDateHeld", type: "date" },
        { name: "DocDatePayment", type: "date" },
    ]
});