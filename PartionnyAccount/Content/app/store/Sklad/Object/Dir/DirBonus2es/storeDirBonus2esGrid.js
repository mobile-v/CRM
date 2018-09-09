Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBonus2es/storeDirBonus2esGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirBonus2esGrid",

    storeId: 'storeDirBonus2esGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBonus2es/modelDirBonus2esGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirBonus2es, //"api/Dir/DirBonus2es/",
        reader: {
            type: "json",
            rootProperty: "DirBonus2" //pID
        },
        timeout: varTimeOutDefault,
    }
});