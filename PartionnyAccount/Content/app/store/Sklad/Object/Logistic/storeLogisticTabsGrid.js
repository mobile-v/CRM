//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Logistic/storeLogisticTabsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeLogisticTabsGrid",

    storeId: 'storeLogisticTabsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Logistic/modelLogisticTabsGrid',
    pageSize: 999999,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_LogisticTabs,
        reader: {
            type: "json",
            rootProperty: "LogisticTab" //pID
        },
        timeout: varTimeOutDefault,
    }
});