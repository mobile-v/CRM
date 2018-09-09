Ext.define('PartionnyAccount.model.Sklad/Object/Report/modelDocServicePurchesReport', {
    extend: 'Ext.data.Model',

    fields: [
        //Примечание
        { name: "DocServicePurchID" },
        { name: "DirServiceNomenName" },
        { name: "SerialNumber" },
        { name: "DirServiceContractorName" },
        { name: "DocDate", type: "date" },
        { name: "IssuanceDate", type: "date" },
        { name: "DateStatusChange", type: "date" },
        { name: "DirEmployeeName" },
        { name: "DirEmployeeNameMaster" },
        { name: "PrepaymentSum" },

        { name: "DiscountX" },
        { name: "SumDocServicePurch1Tabs" },

        { name: "DiscountY" },
        { name: "SumDocServicePurch2Tabs" },

        { name: "SumTotal" },
        { name: "SumTotal2" }
    ]
});