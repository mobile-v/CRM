//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSales/modelDocSalesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocSaleID" },
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

        { name: "DocDateHeld", type: "date" },
        { name: "DocDatePayment", type: "date" },
    ]
});