Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridSecondHandRazbor", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridSecondHandRazbor",

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


            //{ text: "Тип", dataIndex: "ListObjectNameRu", width: 150 },
            { text: "№", dataIndex: "DocSecondHandPurchID", width: 50 },
            //{ text: "№", dataIndex: "DocSecondHandRazborID", width: 50, hidden: true },

            { text: lanState, dataIndex: "DirSecondHandStatusName", flex: 1 },
            { text: "", dataIndex: "Status", width: 55, tdCls: 'x-change-cell2' },
            { text: "Аппарат", dataIndex: "DirServiceNomenName", flex: 2, tdCls: 'x-change-cell' },

            { text: lanDocDate, dataIndex: "DocDate", width: 75 },
            //{ text: lanWarehouse, dataIndex: "DirWarehouseName", width: 120 },
            
            /*
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
            */

            { text: "Сумма", dataIndex: "SumsDirNomen", width: 100, summaryType: 'sum' },

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