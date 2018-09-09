//Хранилище только для Grid
Ext.define('PartionnyAccount.store.Sklad/Object/Logistic/storeLogisticsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeLogisticsGrid",

    storeId: 'storeLogisticsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Logistic/modelLogisticsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_Logistics,
        reader: {
            type: "json",
            rootProperty: "Logistic" //pID
        },
        timeout: varTimeOutDefault,
    }
});