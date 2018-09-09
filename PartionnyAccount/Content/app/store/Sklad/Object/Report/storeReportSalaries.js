Ext.define('PartionnyAccount.store.Sklad/Object/Report/storeReportSalaries', {
    extend: 'Ext.data.Store',
    alias: "store.storeReportSalaries",

    storeId: 'storeReportSalaries',
    model: 'PartionnyAccount.model.Sklad/Object/Report/modelReportSalaries',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ReportSalaries, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "ReportSalaries" //pID
        },
        timeout: varTimeOutDefault,
    }
});