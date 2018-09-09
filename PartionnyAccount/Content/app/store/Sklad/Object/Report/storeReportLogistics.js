Ext.define('PartionnyAccount.store.Sklad/Object/Report/storeReportLogistics', {
    extend: 'Ext.data.Store',
    alias: "store.storeReportLogistics",

    storeId: 'storeReportLogistics',
    model: 'PartionnyAccount.model.Sklad/Object/Report/modelReportLogistics',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ReportLogistics, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "ReportLogistics" //pID
        },
        timeout: varTimeOutDefault,
    }
});