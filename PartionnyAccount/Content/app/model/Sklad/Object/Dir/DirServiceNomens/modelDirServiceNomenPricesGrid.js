Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenPricesGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "DirServiceNomenPriceID" },
        { name: "PriceVAT", type: 'float', useNull: false },

        { name: "DirServiceNomenTypicalFaultID" },
        { name: "DirServiceNomenTypicalFaultName" },
    ]
});