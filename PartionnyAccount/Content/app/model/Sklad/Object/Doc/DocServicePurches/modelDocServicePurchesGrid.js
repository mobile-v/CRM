//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServicePurchesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DocID" },
        { name: "Del" },
        { name: "Held" },
        //{ name: "DirContractorNameOrg" },
        { name: "DirContractorName" },
        { name: "DirWarehouseName" },

        { name: "DocServicePurchID" }, //№ квитанции
        { name: "DocDate", type: "date" }, //Дата приёма
        { name: "DirServiceNomenName" }, //Аппарат
        { name: "SerialNumber" }, //Серийник
        { name: "ProblemClientWords" }, //Неисправность со слов клиента (большое поле для ввода)
        { name: "DirServiceContractorName" }, //Клиент
        { name: "DirServiceContractorPhone" }, //Клиент
        { name: "UrgentRepairs" }, //Срочный ремонт

        //{ name: "DateDone", type: "date" }, //Дата готовности
        { name: 'DateDone', mapping: 'DateDone', type: 'date' },// , dateFormat: 'Y-m-d', put this in your store} 

        { name: "DirServiceStatusID" }, { name: "DirServiceStatusName" }, //Статус
        { name: "PrepaymentSum" },
        { name: "Payment" },
        { name: "Description" }, //Описание - Наименование статуса (пред-последнего). Для того что бы знать что было с Телефоном: Готов или Отказ. Т.к. в конце ставится статус "Выдан"
        { name: "ServiceTypeRepair" },
        { name: "FromGuarantee" },

        { name: "DirEmployeeNameMaster" },

        //Дата, когда аппарат переместили на выдачу
        { name: "IssuanceDate", type: "date" },
        //Выдали клиенту
        { name: "DateVIDACHI", type: "date" },

        { name: "Sums" },

        //Перемещён в БУ
        { name: "InSecondHand" }, { name: "InSecondHandString" },
    ]
});