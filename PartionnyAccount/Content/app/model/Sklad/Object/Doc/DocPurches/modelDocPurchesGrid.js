﻿//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocPurches/modelDocPurchesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocPurchID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "NumberTT" },
        { name: "NumberTax" },
        { name: "DirContractorNameOrg" },
        { name: "DirContractorName" },
        { name: "DirWarehouseName" },
        { name: "Base" },
        { name: "Description" },

        { name: "DirEmployeeName" },

        { name: "SumOfVATCurrency" },
        { name: "SumOfVATIncomeWholesale" },
        { name: "HavePay" }, //{ name: "Payment" },
        { name: "DocDateHeld", type: "date" },
        { name: "DocDatePayment", type: "date" },
    ]
});