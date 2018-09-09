Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewGridServiceLog", {
    extend: "Ext.grid.Panel",
    alias: "widget.viewGridServiceLog",

    region: "center",
    flex: 1,
    loadMask: true,
    //autoScroll: true,
    //touchScroll: true,
    //itemId: "grid",
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
            {
                dataIndex: "Field1", flex: 1,
                renderer: function (value, metaData, record, rowIndex, colIndex, view) {
                    metaData.style = "white-space: normal;";
                    return value;
                }
            },
        ],


        this.callParent(arguments);
    },



    viewConfig: {
        getRowClass: function (record, index) {


            for (var i = 0; i < record.store.model.fields.length; i++) {


                //1. Если поле типа "Дата": "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"
                if (record.store.model.fields[i].type == "date") {
                    //Если есть дата, может быть пустое значение
                    if (record.data[record.store.model.fields[i].name] != null) {

                        if (record.data[record.store.model.fields[i].name].length != 10) {
                            //Ext.Date.format
                            record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "Y-m-d H:i:sO");
                        }
                        else {
                            //Рабочий метод, но нет смысла использовать
                            //Ext.Date.parse and Ext.Date.format
                            //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                            //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                        }
                    }
                }


                
                //2.  === Формируем поле "Field1"  ===  ===  ===  ===  === 

                if (record.data["DirServiceLogTypeID"] != 9) {

                    //Все статусы != 9
                    
                    var FN = record.store.model.fields[i].name;
                    if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                        if (record.data[record.store.model.fields[i].name] != null) {
                            record.data["Field1"] += record.data[record.store.model.fields[i].name];

                            if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                            else record.data["Field1"] += " - ";
                        }
                    }
                    else if (FN == "LogServiceDate") {
                        record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                    }

                    if (record.data["DirServiceLogTypeID"] == 12 && i == record.store.model.fields.length - 1) {
                        record.data["Field1"] += " (с " + record.data["DirWarehouseNameFrom"] + " на " + record.data["DirWarehouseNameTo"] + ")";
                    }

                }

                else {

                    //Все статусы == 9

                    if (record.data["DirServiceLogTypeID"] == 9 && i == 0) { record.data["Field1"] += "<b style='color: red;'> *** "; }



                    var FN = record.store.model.fields[i].name;
                    /*if (FN == "DirServiceLogTypeName" || FN == "DirServiceStatusName" || FN == "Msg" || FN == "DirEmployeeName") {
                        if (record.data[record.store.model.fields[i].name] != null) {

                            if (FN == "DirServiceLogTypeName") {
                                record.data["Field1"] += "Повторный ремонт";
                            }
                            else {
                                record.data["Field1"] += record.data[record.store.model.fields[i].name];
                            }

                            if (FN == "DirEmployeeName" || (FN == "Msg" && record.data["Msg"] == "")) { }
                            else record.data["Field1"] += " - ";
                        }
                    }*/
                    if (FN == "DirServiceLogTypeName") {
                        record.data["Field1"] += "Повторный ремонт" + " - ";
                    }
                    else if (FN == "DirServiceStatusName") {
                        //record.data["Field1"] += record.data[record.store.model.fields[i].name] + " - ";
                    }
                    else if (FN == "Msg") {
                        //record.data["Field1"] += record.data[record.store.model.fields[i].name];
                        //if (record.data["Msg"] != "") record.data["Field1"] += " - ";
                    }
                    else if (FN == "DirEmployeeName") {
                        record.data["Field1"] += record.data[record.store.model.fields[i].name];
                    }
                    else if (FN == "LogServiceDate") {
                        record.data["Field1"] += Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), "y-m-d H:i") + " - ";
                    }



                    if (record.data["DirServiceLogTypeID"] == 9 && i == record.store.model.fields.length - 1) {

                        record.data["Field1"] += "<br />" + record.data["Msg"];

                        record.data["Field1"] += " *** </b>";
                    }

                    if (record.data["DirServiceLogTypeID"] == 12 && i == record.store.model.fields.length - 1) {
                        record.data["Field1"] += " (с " + record.data["DirWarehouseNameFrom"] + " на " + record.data["DirWarehouseNameTo"] + ")";
                    }
                }

            }


        }, //getRowClass

        stripeRows: true,
    }

});