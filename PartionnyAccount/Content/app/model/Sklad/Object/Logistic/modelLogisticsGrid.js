//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Logistic/modelLogisticsGrid', {
    extend: 'Ext.data.Model',

    fields: [

        //Тип документа
        { name: 'DocTypeXID', type: 'int', useNull: true }, //1 - DocMovementTabID || 2 - DocSecondHandMovementTabID
        { name: "DocTypeXName" },

        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "LogisticID" },
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