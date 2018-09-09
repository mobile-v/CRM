//ComboBox для выбора справочника, в которіх небольшое к-во записей и отсутствуют группы
//Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewComboBox", {
    extend: "Ext.form.ComboBox",
    alias: "widget.viewComboBox",


    //fieldLabel: lanCounterType,
    triggerAction: 'all', // query all records on trigger click
    //emptyText: "",
    minChars: 2, // minimum characters to start the search
    enableKeyEvents: true, // otherwise we will not receive key events 
    pageSize: 9990000,
    queryMode: 'local',
    resizable: false, // make the drop down list resizable
    minListWidth: 220, // we need wider list for paging toolbar
    allowBlank: false, // force user to fill something
    typeAhead: false,
    hideTrigger: false,
    editable: false,
    //queryMode: comboBoxDirContractorTypeStore,

    width: "95%",
    editable: false,
    allowBlank: false,

    conf: {},

    initComponent: function () {
        //body
        /*
        this.id = this.conf.id;       //Name + ObjectID
        this.UO_id = this.conf.UO_id; //ObjectID
        this.UO_idMain = this.conf.UO_idMain; // == this.id
        */

        this.callParent(arguments);

        //Глюк: 2-ды нельзя открыть один КомбоБокс, конфликт id.
        //Решение: https://www.sencha.com/forum/showthread.php?303101
        this.pickerId = this.getId() + "_Picker";
    }

});