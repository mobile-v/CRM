Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirChars/storeDirCharMaterialsTree', {
    extend: 'Ext.data.TreeStore',
    alias: "store.storeDirCharMaterialsTree",

    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharMaterialsTree',
    requires: 'PartionnyAccount.model.Sklad/Object/Dir/DirChars/modelDirCharMaterialsTree',

    proxy: {
        type: 'ajax',
        url: HTTP_DirCharMaterials,
        timeout: varTimeOutDefault,
        actionMethods: "GET", //'POST',
        reader: {
            type: 'json',
            rootProperty: 'query'
        }
    }
});