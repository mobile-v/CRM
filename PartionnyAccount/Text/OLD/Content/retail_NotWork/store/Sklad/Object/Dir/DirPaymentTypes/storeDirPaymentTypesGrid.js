Ext.define('PartionnyAccount.store.Sklad/Object/Dir/DirPaymentTypes/storeDirPaymentTypesGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDirPaymentTypesGrid",

    storeId: 'storeDirPaymentTypesGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Dir/DirPaymentTypes/modelDirPaymentTypesGrid',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DirPaymentTypes,
        reader: {
            type: "json",
            rootProperty: "DirPaymentType" //pID
        },
        timeout: varTimeOutDefault,
    }
});