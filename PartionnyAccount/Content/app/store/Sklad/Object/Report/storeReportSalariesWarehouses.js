Ext.define('PartionnyAccount.store.Sklad/Object/Report/storeReportSalariesWarehouses', {
    extend: 'Ext.data.Store',
    alias: "store.storeReportSalariesWarehouses",

    storeId: 'storeReportSalariesWarehouses',
    model: 'PartionnyAccount.model.Sklad/Object/Report/modelReportSalariesWarehouses',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ReportSalariesWarehouses, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "ReportSalariesWarehouses" //pID
        },
        timeout: varTimeOutDefault,
    }
});