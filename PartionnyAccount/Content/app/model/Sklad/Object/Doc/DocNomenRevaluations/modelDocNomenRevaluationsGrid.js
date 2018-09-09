//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocNomenRevaluations/modelDocNomenRevaluationsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocNomenRevaluationID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorIDOrg" }, { name: "DirContractorNameOrg" },
        { name: "DirWarehouseID" }, { name: "DirWarehouseName" },
        { name: "Base" },
    ]
});