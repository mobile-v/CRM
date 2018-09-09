//Хранилище только для Grid
                                 //Sklad/Object/Doc/DirCashOffices/storeDocCashOfficeSumMovementsGrid
Ext.define('PartionnyAccount.store.Sklad/Object/Doc/DocCashOfficeSums/storeDocCashOfficeSumMovementsGrid', {
    extend: 'Ext.data.Store',
    alias: "store.storeDocCashOfficeSumMovementsGrid",

    storeId: 'storeDocCashOfficeSumMovementsGrid',
    model: 'PartionnyAccount.model.Sklad/Object/Doc/DocCashOfficeSums/modelDocCashOfficeSumMovementsGrid',
    pageSize: varPageSizeJurn,
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: HTTP_DocCashOfficeSumMovements,
        reader: {
            type: "json",
            rootProperty: "DocCashOfficeSumMovement" //pID
        },
        timeout: varTimeOutDefault,
    }
});