Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridService", {
    extend: "Ext.grid.Panel", //"Ext.grid.Panel",
    alias: "widget.viewGridService",

    region: "center",
    loadMask: true,
    //autoScroll: true,
    //touchScroll: true,
    itemId: "grid",
    UI_DateFormatStr: false,

    conf: {},

    initComponent: function () {

        this.id = this.conf.id;
        this.UO_id = this.conf.UO_id;
        this.UO_idMain = this.conf.UO_idMain;
        this.UO_idCall = this.conf.UO_idCall;
        this.UO_View = this.conf.UO_View;

        //this.itemId = this.conf.itemId;
        //this.store = this.conf.store;

        this.columns = [
            { text: "№ desk", dataIndex: "DocID", width: 50, hidden: true },
            { text: "№", dataIndex: "DocServicePurchID", width: 50 },

            { text: lanState, dataIndex: "DirServiceStatusName", flex: 1, hidden: true },
            { text: "", dataIndex: "Status", width: 55, tdCls: 'x-change-cell2' },
            { text: "Аппарат", dataIndex: "DirServiceNomenName", flex: 2, tdCls: 'x-change-cell' },

            { text: "Серийный", dataIndex: "SerialNumber", width: 90 },
            { text: "Клиент", dataIndex: "DirServiceContractorName", flex: 1 }, 
            { text: lanDocDate, dataIndex: "DocDate", width: 75 },


            {
                text: "Точка", dataIndex: "DirWarehouseName", width: 85,
            },


            { text: "Неисправность", dataIndex: "ProblemClientWords", flex: 1, hidden: true },
            { text: lanPhone, dataIndex: "DirServiceContractorPhone", flex: 1, hidden: true },
            { text: "Срочный", dataIndex: "UrgentRepairs", flex: 1, hidden: true },

            //{ text: "Готовность", dataIndex: "DateDone", width: 75, hidden: true },
            //{ text: "Готовность", dataIndex: "DateDone", width: 75, sortable: false, editor: { xtype: 'textfield' } },
            //{ header: "Готовность", align: 'center', width: 120, sortable: true, dataIndex: 'DateDone', renderer: Ext.util.Format.dateRenderer('Y-m-d'), editor: { xtype: 'datefield' } }, // // put this in your column model} 

            {
                xtype: 'datecolumn',
                dataIndex: 'DateDone',
                header: 'Готовность',
                sortable: true,
                //id: 'depreciationStartPeriod',
                width: 75,
                format: 'Y-m-d', //format: 'Y-m-d H:i:s', // <------- this way
                editor: {
                    xtype: 'datefield',
                    format: 'Y-m-d', //format: 'Y-m-d H:i:s',
                    submitFormat: 'c'  // <-------------- this way
                }
            },


            { text: "Предоплата", dataIndex: "Prepayment", width: 50, hidden: true },

            //!!! Пиздец !!!
            //Вот тут я ЛОХАНУЛСЯ!!!
            { text: "На выдачу", dataIndex: "IssuanceDate", width: 85 }, //{ text: "Дата выдачи", dataIndex: "IssuanceDate", width: 85 },
            { text: "Выдали", dataIndex: "DateVIDACHI", width: 85 },

            { text: "Сумма", dataIndex: "Sums", width: 75, summaryType: 'sum' },

            { text: "Мастер", dataIndex: "DirEmployeeNameMaster", width: 75 },

            //{ text: lanOrg, dataIndex: "DirContractorNameOrg", flex: 1, hidden: true },
            //{ text: lanContractor, dataIndex: "DirContractorName", flex: 1 },

            //{ text: "Б/У", dataIndex: "InSecondHand", width: 75, hidden: true },
            { text: "БУ", dataIndex: "InSecondHandString", width: 50, hidden: true },
            { text: "№ БУ", dataIndex: "DocSecondHandPurchID", width: 50 },
        ],


        this.callParent(arguments);
    },



    viewConfig: {
        getRowClass: function (record, index) {


            // 1. === Исправляем формат даты: "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"  ===  ===  ===  ===  === 
            for (var i = 0; i < record.store.model.fields.length; i++) {
                //Если поле типа "Дата"
                if (record.store.model.fields[i].type == "date") {
                    //Если есть дата, может быть пустое значение
                    if (record.data[record.store.model.fields[i].name] != null) {

                        if (record.data[record.store.model.fields[i].name].length != 10) {
                            //Ext.Date.format
                            record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                        }
                    }
                }
            }



            // 2.  === Стили ===  ===  ===  ===  === 
            var StatusID = parseFloat(record.get('DirServiceStatusID'));
            
            //2.1. Если просрочка 3 и более месяцев
            var DateDoneMinis = (new Date() - new Date(record.get('DateDone'))) / (1000 * 3600 * 24); //DateDone
            if (DateDoneMinis > 91)
            {
                //return 'status-DateDone-0';
                if (StatusID == 1) { return 'status-DateDone-101'; }
                else if (StatusID == 2) { return 'status-DateDone-201'; }
                else if (StatusID == 3) { return 'status-DateDone-301'; }
                else if (StatusID == 4) { return 'status-DateDone-401'; } //{ return 'status-DateDone-4'; }
                else if (StatusID == 5) { return 'status-DateDone-501'; }
                else if (StatusID == 6) { return 'status-DateDone-601'; }
                else if (StatusID == 7) { return 'status-701'; }
                else if (StatusID == 8) { return 'status-801'; }
                else if (StatusID == 9) { return 'status-901'; }
            }
            else 
            //2.1. Если аппарат просрочен: DateDone <= new Date()
            if (new Date(record.get('DateDone')) <= new Date()) {
                //return 'status-DateDone';
                if (StatusID == 1) { return 'status-DateDone-1'; }
                else if (StatusID == 2) { return 'status-DateDone-2'; }
                else if (StatusID == 3) { return 'status-DateDone-3'; }
                else if (StatusID == 4) { return 'status-DateDone-4'; } //{ return 'status-DateDone-4'; }
                else if (StatusID == 5) { return 'status-DateDone-5'; }
                else if (StatusID == 6) { return 'status-DateDone-6'; }
                else if (StatusID == 7) { return 'status-7'; }
                else if (StatusID == 8) { return 'status-8'; }
                else if (StatusID == 9) { return 'status-9'; }
            }

            //2.2. Не просрочен
            if (StatusID == 1) { return 'status-1'; }
            else if (StatusID == 2) {
                if (record.get('FromGuarantee')) { return 'status-FromGuarantee'; }
                else { return 'status-2'; }
            }
            else if (StatusID == 3) { return 'status-3'; }
            else if (StatusID == 4) { return 'status-4'; }
            else if (StatusID == 5) { return 'status-5'; }
            else if (StatusID == 6) { return 'status-6'; }
            else if (StatusID == 7) { return 'status-7'; }
            else if (StatusID == 8) { return 'status-8'; }
            else if (StatusID == 9) { return 'status-9'; }

        }, //getRowClass

        stripeRows: true,
    }


});

/*
Ext.define('Fiddle.MyOverride', {
    override: 'Ext.grid.header.Container',

    getMenuItems: function () {
        var items = this.callParent();
        items.push({
            itemId: 'customItem',
            text: 'Hi there!',
            handler: function () {
                alert(111);
            }
        });
        return items;
    }
});
*/
