Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocDomesticExpenses/modelDocDomesticExpensesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocDomesticExpenseID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "NumberTT" },
        { name: "NumberTax" },
        { name: "DirContractorIDOrg" }, { name: "DirContractorNameOrg" },
        { name: "DirContractorID" }, { name: "DirContractorName" },
        { name: "DirWarehouseID" }, { name: "DirWarehouseName" },
        { name: "Base" },
        { name: "Description" },
        { name: "SumOfVATCurrency" },
        { name: "SumOfVATIncomeWholesale" },
        { name: "HavePay" }, //{ name: "Payment" },

        //Комментарий: причина возврата
        { name: "DirReturnTypeID" }, { name: "DirReturnTypeName" },
        { name: "DirDescriptionID" }, { name: "DirDescriptionName" },
    ]
});