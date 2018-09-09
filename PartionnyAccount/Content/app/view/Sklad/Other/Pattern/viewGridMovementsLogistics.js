Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridMovementsLogistics", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridMovementsLogistics",

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
            { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
            { text: "№ desk", dataIndex: "DocID", width: 50, hidden: true },
            { text: "№", dataIndex: "DocMovementID", width: 50 },
            { text: lanDate, dataIndex: "DocDate", sortable: true, width: 75 },
            { text: lanOrg, dataIndex: "DirContractorNameOrg", sortable: true, flex: 1, hidden: true },

            { text: lanWarehouseFrom, dataIndex: "DirWarehouseNameFrom", sortable: true, flex: 1, tdCls: 'x-change-cell-3' },
            { text: "Курьер", dataIndex: "DirEmployeeNameCourier", sortable: true, flex: 1, tdCls: 'x-change-cell-4' },
            { text: lanWarehouseTo, dataIndex: "DirWarehouseNameTo", sortable: true, flex: 1, tdCls: 'x-change-cell-5' },

            { text: lanBase, dataIndex: "Base", hidden: true },
            { text: lanDisc, dataIndex: "Description", hidden: true },
        ],


        this.callParent(arguments);
    },



    //Формат даты
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


                //Логистика: в ожидании курьера
            if (parseInt(record.get('DirMovementStatusID')) == 2) { return 'movements-2'; }
                //Логистика: курьер принял
            else if (parseInt(record.get('DirMovementStatusID')) == 3) { return 'movements-3'; }
                //Логистика: курьер отдал
            else if (parseInt(record.get('DirMovementStatusID')) == 4) { return 'movements-4'; }


        }, //getRowClass

        stripeRows: true,

    } //viewConfig

});