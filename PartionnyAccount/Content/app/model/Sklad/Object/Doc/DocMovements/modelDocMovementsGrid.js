//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocMovements/modelDocMovementsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocMovementID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorNameOrg" },
        { name: "DirWarehouseNameFrom" },
        { name: "DirWarehouseNameTo" },
        { name: "Base" },
        { name: "Description" },

        { name: "DirEmployeeName" },

        //Курьер
        { name: "DirEmployeeIDCourier" },
        { name: "DirEmployeeNameCourier" },
        { name: "DirMovementStatusID" }, //1 - Курьер штрихнулся и забрал товар

        { name: "SumOfVATCurrency" },
        { name: "SumOfVATIncomeWholesale" },
    ]
});