//Модель только для Grid
                             
Ext.define('PartionnyAccount.model.Sklad/Object/List/modelListObjectPFsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: "ListObjectID" },
        { name: "ListObjectPFID" },
        { name: "ListLanguageID" },
        { name: "ListLanguageNameSmall" },
        { name: "SysRecord" },
        { name: "Del" },
        { name: "ListObjectPFName" }
    ]
});