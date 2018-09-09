//Модель только для Grid
Ext.define('PartionnyAccount.model.Sklad/Object/Doc/DocServicePurches/modelDocServiceMovTabsGrid', {
    extend: 'Ext.data.Model',

    fields: [
        { name: 'DocServiceMovTabID', type: 'int', useNull: true },
        { name: 'DocServicePurchID', type: 'int', useNull: false },

        { name: 'DirServiceNomenID', type: 'int', useNull: false },
        { name: 'DirServiceNomenName', type: 'string', useNull: false },

        { name: 'DirServiceStatusID', type: 'int', useNull: false },
        { name: 'DirServiceStatusName', type: 'string', useNull: false },

        { name: 'DirServiceStatusID_789', type: 'int', useNull: true },
        { name: 'DirServiceStatusName_789', type: 'string', useNull: true },

        { name: 'SerialNumber', type: 'string', useNull: true },
        { name: 'DirServiceContractorName', type: 'string', useNull: true },
        { name: 'DirServiceContractorPhone', type: 'string', useNull: true },
        { name: 'PriceVAT', type: 'string', useNull: true },
        { name: 'PrepaymentSum', type: 'float', useNull: true },
        
    ]
});