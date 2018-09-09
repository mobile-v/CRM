Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirDiscounts/storeDirDiscountsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirDiscountsGrid",

    storeId: 'storeDirDiscountsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirDiscounts/modelDirDiscountsGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirDiscounts, //"api/Dir/DirDiscounts/",
        reader: {
            type: "json",
            rootProperty: "DirDiscount" //pID
        },
        timeout: varTimeOutDefault,
    }
});