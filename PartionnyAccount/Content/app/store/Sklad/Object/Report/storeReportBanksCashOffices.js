Ext.define('PartionnyAccount.store.Sklad/Object/Report/storeReportBanksCashOffices', {
    extend: 'Ext.data.Store',
    alias: "store.storeReportBanksCashOffices",

    storeId: 'storeReportBanksCashOffices',
    model: 'PartionnyAccount.model.Sklad/Object/Report/modelReportBanksCashOffices',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ReportBanksCashOffices, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "ReportBanksCashOffices" //pID
        },
        timeout: varTimeOutDefault,
    }
});