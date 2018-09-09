//Текстовое поле для тригера PartionnyAccount.view.Sklad/Other/Pattern/viewTriggerDir
//Используется в Константах
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewTriggerDirField", {
    extend: "Ext.form.field.Text",
    alias: "widget.viewTriggerDirField",


    //width: "95%",
    editable: false,
    //allowBlank: true,
    readOnly: true,
    hidden: true,
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
    }

});