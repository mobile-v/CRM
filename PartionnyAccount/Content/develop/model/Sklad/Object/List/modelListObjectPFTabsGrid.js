//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/List/modelListObjectPFTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'ListObjectPFTabID', type: 'int', useNull: true },
        { name: 'ListObjectPFTabName', type: 'string', useNull: false },
        { name: 'ListObjectFieldNameID', type: 'int', useNull: false },
        { name: 'ListObjectFieldNameRu', type: 'string', useNull: false },
        { name: 'PositionID', type: 'int', useNull: false },
        { name: 'TabNum', type: 'int', useNull: true },
        { name: 'Width', type: 'int', useNull: true },
    ]
});