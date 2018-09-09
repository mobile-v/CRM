Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirDiscounts/storeDirDiscountTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirDiscountTabsGrid",

    storeId: 'storeDirDiscountTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirDiscounts/modelDirDiscountTabsGrid',
    pageSize: 99999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirDiscountTabs,
        reader: {
            type: "json",
            rootProperty: "DirDiscountTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});