Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirBonuses/storeDirBonusTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirBonusTabsGrid",

    storeId: 'storeDirBonusTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirBonuses/modelDirBonusTabsGrid',
    pageSize: 99999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirBonusTabs,
        reader: {
            type: "json",
            rootProperty: "DirBonusTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});