Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirDiscounts/modelDirDiscountTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DirDiscountTabID', type: 'int', useNull: true },
        { name: 'DirDiscountID', type: 'int', useNull: true },
        { name: 'SumBegin', type: 'float', useNull: true },
        { name: 'Discount', type: 'float', useNull: true },
    ]
});