//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSalaries/modelDocSalariesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "DocDate", type: "date" },
        { name: "Held" },
        { name: "Base" },
        { name: "Del" },
        { name: "Description" },
        { name: "DocSalaryID" },
        { name: "DirContractorIDOrg" }, { name: "DirContractorNameOrg" },

        { name: "SumSalary" },
        { name: "DirBonusIDSalary" },
        { name: "DirBonusID2Salary" },
        { name: "Sums" },
    ]
});