//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRazborsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        //{ name: "DirContractorNameOrg" },
        //{ name: "DirContractorName" },
        { name: "DirWarehouseName" },

        { name: "ListObjectNameRu" },
        { name: "DocSecondHandPurchID" },
        { name: "DocSecondHandRazborID" },

        { name: "DocDate", type: "date" }, //Дата приёма
        { name: "DirSecondHandNomenName" }, //Аппарат
        { name: "SerialNumber" }, //Серийник
        { name: "ProblemClientWords" }, //Неисправность со слов клиента (большое поле для ввода)
        { name: "DirSecondHandContractorName" }, //Клиент
        { name: "DirSecondHandContractorPhone" }, //Клиент
        { name: "UrgentRepairs" }, //Срочный ремонт

        //{ name: "DateDone", type: "date" }, //Дата готовности
        { name: 'DateDone', mapping: 'DateDone', type: 'date' },// , dateFormat: 'Y-m-d', put this in your store} 

        { name: "DirSecondHandStatusID" }, { name: "DirSecondHandStatusName" }, //Статус
        { name: "PrepaymentSum" },
        { name: "Payment" },
        { name: "Description" }, //Описание - Наименование статуса (пред-последнего). Для того что бы знать что было с Телефоном: Готов или Отказ. Т.к. в конце ставится статус "Выдан"
        { name: "SecondHandTypeRepair" },
        { name: "FromGuarantee" },

        { name: "IssuanceDate", type: "date" },
        { name: "Sums" },

        //{ name: "FromService" },
        { name: "FromServiceString" },
        { name: "DocIDService" }, { name: "DocServicePurchID" },

    ]
});