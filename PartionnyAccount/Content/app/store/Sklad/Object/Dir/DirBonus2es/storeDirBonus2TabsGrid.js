Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBonus2es/storeDirBonus2TabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirBonus2TabsGrid",

    storeId: 'storeDirBonus2TabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBonus2es/modelDirBonus2TabsGrid',
    pageSize: 99999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirBonus2Tabs,
        reader: {
            type: "json",
            rootProperty: "DirBonus2Tab" //pID
        },
        timeout: varTimeOutDefault,
    }
});