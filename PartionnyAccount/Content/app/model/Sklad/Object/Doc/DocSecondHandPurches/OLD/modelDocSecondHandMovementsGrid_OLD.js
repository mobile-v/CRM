//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovementsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocSecondHandMovementID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorNameOrg" },
        { name: "DirWarehouseNameFrom" },
        { name: "DirWarehouseNameTo" },
        { name: "Base" },
        { name: "Description" },

        //Курьер
        { name: "DirEmployeeIDCourier" },
        { name: "DirEmployeeNameCourier" },
        { name: "DirMovementStatusID" }, //1 - Курьер штрихнулся и забрал товар

        { name: "SumOfVATCurrency" },
        { name: "SumOfVATIncomeWholesale" },

        { name: "DirEmployeeName" },
    ]
});