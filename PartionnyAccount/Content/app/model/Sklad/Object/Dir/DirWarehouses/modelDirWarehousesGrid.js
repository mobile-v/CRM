Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirWarehouses/modelDirWarehousesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DirWarehouseID" },
        { name: "Del" },
        { name: "DirWarehouseName" },

        { name: "DirCashOfficeID" },
        { name: "DirCurrencyID" },
        { name: "DirCurrencyRate" },
        { name: "DirCurrencyMultiplicity" },
        { name: "DirCashOfficeSum" },

        { name: "IsAdmin" },
        { name: "KKMSActive" },
    ]
});