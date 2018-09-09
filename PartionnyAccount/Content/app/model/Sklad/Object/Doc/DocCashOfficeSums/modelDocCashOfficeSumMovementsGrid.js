//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocCashOfficeSums/modelDocCashOfficeSumMovementsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        { name: "DocCashOfficeSumMovementID" },
        { name: "NumberInt" },
        { name: "DocDate", type: "date" },
        { name: "DirContractorNameOrg" },

        { name: "DirWarehouseNameFrom" },
        { name: "DirCashOfficeNameFrom" },

        { name: "DirWarehouseNameTo" },
        { name: "DirCashOfficeNameTo" },

        { name: "Reserve" },
        { name: "Sums" },
        { name: "SumsCurrency" },
        { name: "DirCurrencyID" },
        { name: "DirCurrencyRate" },
        { name: "DirCurrencyMultiplicity" },

        { name: "Base" },
        { name: "Description" },

        //Курьер
        { name: "DirEmployeeIDCourier" },
        { name: "DirEmployeeNameCourier" },
        { name: "DirMovementStatusID" }, //1 - Курьер штрихнулся и забрал товар
        
        { name: "DirEmployeeName" },
    ]
});