Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirBonuses/modelDirBonusTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DirBonusTabID', type: 'int', useNull: true },
        { name: 'DirBonusID', type: 'int', useNull: true },
        { name: 'SumBegin', type: 'float', useNull: true },
        { name: 'Bonus', type: 'float', useNull: true }
    ]
});