//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocActWriteOffs/modelDocActWriteOffsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocActWriteOffID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorNameOrg" },
        //{ name: "DirContractorName" },
        { name: "DirWarehouseName" },
        { name: "Base" },
        { name: "Description" },
        { name: "SumOfVATCurrency" },
        { name: "SumOfVATIncomeWholesale" }
    ]
});