Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocRetails/modelDocRetailsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocRetailID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "NumberTT" },
        { name: "NumberTax" },
        { name: "DirContractorIDOrg" }, { name: "DirContractorNameOrg" },
        { name: "DirContractorID" }, { name: "DirContractorName" },
        { name: "DirWarehouseID" }, { name: "DirWarehouseName" },
        { name: "Base" },
        { name: "Description" },

        { name: "DirPaymentTypeID" },
        { name: "SumOfVATCurrency" },
        { name: "SumOfVATIncomeWholesale" },
        { name: "HavePay" }, //{ name: "Payment" },

        //Комментарий: причина возврата
        { name: "DirReturnTypeID" }, { name: "DirReturnTypeName" },
        { name: "DirDescriptionID" }, { name: "DirDescriptionName" },

        { name: "DocDateHeld", type: "date" },
        { name: "DocDatePayment", type: "date" },
    ]
});