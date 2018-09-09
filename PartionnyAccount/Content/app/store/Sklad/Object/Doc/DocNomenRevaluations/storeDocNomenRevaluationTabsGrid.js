//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocNomenRevaluations/storeDocNomenRevaluationTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocNomenRevaluationTabsGrid",

    storeId: 'storeDocNomenRevaluationTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocNomenRevaluations/modelDocNomenRevaluationTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocNomenRevaluationTabs,
        reader: {
            type: "json",
            rootProperty: "DocNomenRevaluationTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});