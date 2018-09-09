Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirEmployees/modelDirEmployeeWarehousesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DirWarehouseID', type: 'int', useNull: false },
        { name: 'DirWarehouseName' },

        { name: 'IsAdmin' },
        { name: 'IsAdminNameRu' },

        { name: 'WarehouseAll' },
        { name: 'WarehouseAllNameRu' },
    ]
});