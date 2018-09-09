Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharMaterialsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirCharMaterialsGrid",

    storeId: 'storeDirCharMaterialsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharMaterialsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirCharMaterials,
        reader: {
            type: "json",
            rootProperty: "DirCharMaterial" //pID
        },
        timeout: varTimeOutDefault,
    }
});