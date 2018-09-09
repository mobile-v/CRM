//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocOrderInts/modelDocOrderIntsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "DocOrderIntID" },
        { name: "DocDate", type: "date" },
        { name: "DirEmployeeName" },
        { name: "DirWarehouseName" },


        { name: "DirOrderIntStatusID" }, { name: "DirOrderIntStatusName" }, //Статус
        { name: "DirOrderIntTypeID" }, { name: "DirOrderIntTypeName" }, //Статус

        { name: "DirServiceNomenName" },
        { name: "DirNomenID" },
        { name: "PriceVAT" }, { name: "PriceCurrency" },

        { name: "Description" },

        { name: "NomenExist" },

        { name: "DirNomenXName6" },
        { name: "DirNomen1Name" },
        { name: "DirNomen2Name" },
        { name: "DirNomenName" },

    ]
});