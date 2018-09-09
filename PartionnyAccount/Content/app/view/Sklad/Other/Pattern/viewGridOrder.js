Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridOrder", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridOrder",

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
            { text: "№ desk", dataIndex: "DocID", width: 85, hidden: true },
            { text: "№ ", dataIndex: "DocOrderIntID", width: 50 },
            { text: "", dataIndex: "Status", width: 22, tdCls: 'x-change-cell2' },
            { text: lanState, dataIndex: "DirOrderIntStatusName", width: 100 },
            { text: lanDocDate, dataIndex: "DocDate", width: 75 },

            { text: "Тип у-ва", dataIndex: "DirServiceNomenName", flex: 2, tdCls: 'x-change-cell' },
            //{ text: "Товар/Запчасть", dataIndex: "DirNomenXName6", flex: 2, tdCls: 'x-change-cell' },
            { text: "Марка", dataIndex: "DirNomen1Name", flex: 2, tdCls: 'x-change-cell' },
            { text: "Модель", dataIndex: "DirNomen2Name", flex: 2, tdCls: 'x-change-cell' },
            { text: "Товар", dataIndex: "DirNomenName", flex: 2, tdCls: 'x-change-cell' },

            { text: "Заказчик", dataIndex: "DirEmployeeName", flex: 1 },
            { text: lanWarehouse, dataIndex: "DirWarehouseName", flex: 1 },
            { text: lanType, dataIndex: "DirOrderIntTypeName", flex: 1 },
            { text: "Примечание", dataIndex: "Description", flex: 1, tdCls: 'x-change-cell' },
            { text: "", dataIndex: "NomenExist", width: 100, tdCls: 'x-change-cell' },
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



            

            //2.  === Стили ===  ===  ===  ===  === 
            //Удалённые
            //if (record.get('Del') == true) { return 'price-del'; }

            var StatusID = parseFloat(record.get('DirOrderIntStatusID'));

            /*
            //Если аппарат просрочен: DateDone >= new Date()
            if (new Date(record.get('DateDone')) <= new Date()) {
                //return 'status-DateDone';
                if (StatusID == 1) { return 'status-DateDone-1'; }
                else if (StatusID == 2) { return 'status-DateDone-2'; }
                else if (StatusID == 3) { return 'status-DateDone-3'; }
                else if (StatusID == 4) { return 'status-DateDone-4'; }
                else if (StatusID == 5) { return 'status-DateDone-5'; }
                else if (StatusID == 6) { return 'status-DateDone-6'; }
                else if (StatusID == 7) { return 'status-7'; }
                else if (StatusID == 8) { return 'status-8'; }
            }
            */

            if (StatusID == 1) { return 'status-1'; }
            else if (StatusID == 2) {

                if (record.get('FromGuarantee')) {//if (funParseBool(record.get('FromGuarantee'))) {
                    return 'status-FromGuarantee';
                }
                else {
                    return 'status-2';
                }
            }

            else if (StatusID == 3) { return 'status-3'; }
            else if (StatusID == 4) { return 'status-7'; }
            else if (StatusID == 5) { return 'status-8'; }
            //else if (StatusID == 6) { return 'status-6'; }
            //else if (StatusID == 7) { return 'status-7'; }
            //else if (StatusID == 8) { return 'status-8'; }

            


        }, //getRowClass

        stripeRows: true,
    }

});