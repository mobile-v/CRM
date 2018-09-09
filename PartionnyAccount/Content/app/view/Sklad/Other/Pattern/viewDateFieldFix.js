//Текстовое поле для тригера PartionnyAccount.view.Sklad/Other/Pattern/viewDateField
//Используется в Константах
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewDateFieldFix", {
    extend: "Ext.form.field.Date",
    alias: "widget.viewDateFieldFix",

    //format: "Y-m-d H:i:s",
    format: "c",

    //fieldLabel: lanDate,
    //maxValue: new Date(),
    editable: true,
    allowBlank: true,
    //flex: 1,

    conf: { },

    initComponent: function () {

        this.callParent(arguments);
    },

    //Это событие в Представлении, потому, что в Контролёре объявить его нельзя.
    listeners: {
        //С сервера приходит не корректный формат даты "yyyy-MM-dd HH-ii-ss", а нужно "yyyy-MM-dd"
        change: function () {
            if (this.format != DateFormatStr) {
                this.format = DateFormatStr;
                this.setValue(Ext.Date.format(new Date(this.value), DateFormatStr));
            }
        }
    }

});