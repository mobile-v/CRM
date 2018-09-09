Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridSecondHand", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridSecondHand",

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
            { text: "№", dataIndex: "DocSecondHandPurchID", width: 50 },

            { text: lanState, dataIndex: "DirSecondHandStatusName", flex: 1, hidden: true },
            { text: "", dataIndex: "Status", width: 55, tdCls: 'x-change-cell2' },
            { text: "Аппарат", dataIndex: "DirServiceNomenName", flex: 2, tdCls: 'x-change-cell' },

            { text: "Серийный", dataIndex: "SerialNumber", width: 120 },
            { text: "Клиент", dataIndex: "DirServiceContractorName", flex: 1 },
            { text: lanDocDate, dataIndex: "DocDate", width: 75 },
            { text: lanWarehouse, dataIndex: "DirWarehouseName", width: 120 },

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


            //{ text: "Предоплата", dataIndex: "Prepayment", width: 50, hidden: true },
            //{ text: "Дата выдачи", dataIndex: "IssuanceDate", width: 100 },
            { text: "Сумма", dataIndex: "Sums", width: 100, summaryType: 'sum' },


            //{ text: lanOrg, dataIndex: "DirContractorNameOrg", flex: 1, hidden: true },
            //{ text: lanContractor, dataIndex: "DirContractorName", flex: 1 },


            //{ text: "Из СЦ", dataIndex: "FromService", width: 100, hidden: true },
            //{ text: "Из СЦ", dataIndex: "FromServiceString", width: 100, hidden: true },
            { text: "№ СЦ", dataIndex: "DocServicePurchID", width: 50 },

            { text: "Продажа", dataIndex: "DateRetail", width: 75, hidden: true },
            { text: "Возврат", dataIndex: "DateReturn", width: 75, hidden: true },
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
            var StatusID = parseFloat(record.get('DirSecondHandStatusID'));
            
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
                else if (StatusID == 10) { return 'status-10'; }
                else if (StatusID == 11) { return 'status-11'; }
                else if (StatusID == 12) { return 'status-12'; }
                else if (StatusID == 13) { return 'status-13'; }
                else if (StatusID == 14) { return 'status-14'; }
                else if (StatusID == 15) { return 'status-15'; }
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
            else if (StatusID == 10) { return 'status-10'; }
            else if (StatusID == 11) { return 'status-11'; }
            else if (StatusID == 12) { return 'status-12'; }
            else if (StatusID == 13) { return 'status-13'; }
            else if (StatusID == 14) { return 'status-14'; }
            else if (StatusID == 15) { return 'status-15'; }

        }, //getRowClass

        stripeRows: true,
    }

});