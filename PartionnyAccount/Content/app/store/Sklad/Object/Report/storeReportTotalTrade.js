Ext.define('PartionnyAccount.store.Sklad/Object/Report/storeReportTotalTrade', {
    extend: 'Ext.data.Store',
    alias: "store.storeReportTotalTrade",

    storeId: 'storeReportTotalTrade',
    model: 'PartionnyAccount.model.Sklad/Object/Report/modelReportTotalTrade',
    pageSize: varPageSizeDir,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_ReportTotalTrade, //"api/Dir/DirWarehouses/",
        reader: {
            type: "json",
            rootProperty: "ReportTotalTrade" //pID
        },
        timeout: varTimeOutDefault,
    }
});