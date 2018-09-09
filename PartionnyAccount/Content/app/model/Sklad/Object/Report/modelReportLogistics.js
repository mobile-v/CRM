Ext.define('PartionnyAccount.model.Sklad/Object/Report/modelReportLogistics', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocMovementID" },
        { name: "DirEmployeeName" },
        { name: "DocDate", type: "date" },
        { name: "DirWarehouseName" },
        { name: "DirMovementStatusID" },
        { name: "DirMovementStatusName" },

        { name: "DirNomenID" },
        { name: "DirNomenName" },
        { name: "DirNomenPatchFull" },
        { name: "Sale_Quantity" },
        { name: "DirChar" }
    ]
});