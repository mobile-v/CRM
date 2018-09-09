Ext.define('PartionnyAccount.model.Sklad/Object/Dir/DirBonus2es/modelDirBonus2TabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DirBonus2TabID', type: 'int', useNull: true },
        { name: 'DirBonus2ID', type: 'int', useNull: true },
        { name: 'SumBegin', type: 'float', useNull: true },
        { name: 'Bonus', type: 'float', useNull: true }
    ]
});