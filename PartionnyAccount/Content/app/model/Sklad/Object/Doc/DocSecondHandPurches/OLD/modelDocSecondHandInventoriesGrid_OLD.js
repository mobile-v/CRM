//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInventoriesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocSecondHandInventoryID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorNameOrg" },
        { name: "DirContractorName" },
        { name: "DirWarehouseName" },
        { name: "Base" },
        { name: "Description" },

        //Списанные аппараты с ЗП
        { name: "SumOfVATCurrency1" },
        //Аппараты на разбор
        { name: "SumOfVATCurrency2" },

        { name: "DocDateHeld", type: "date" },
        { name: "DocDatePayment", type: "date" },

        { name: "SpisatS" },
        { name: "SpisatSDirEmployeeID" },
    ]
});