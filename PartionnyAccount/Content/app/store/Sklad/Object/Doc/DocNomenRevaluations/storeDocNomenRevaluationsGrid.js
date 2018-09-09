//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocNomenRevaluations/storeDocNomenRevaluationsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocNomenRevaluationsGrid",

    storeId: 'storeDocNomenRevaluationsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocNomenRevaluations/modelDocNomenRevaluationsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocNomenRevaluations,
        reader: {
            type: "json",
            rootProperty: "DocNomenRevaluation" //pID
        },
        timeout: varTimeOutDefault,
    }
});