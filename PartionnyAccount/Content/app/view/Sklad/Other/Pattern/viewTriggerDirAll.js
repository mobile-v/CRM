/*
Как использовать:

{
    xtype: 'viewTriggerDirAll',
    fieldLabel: "Товар №1", emptyText: "...", allowBlank: false, flex: 1,
    name: 'Slider_DirNomen1Name', itemId: "Slider_DirNomen1Name", id: "Slider_DirNomen1Name" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

    triggers: {
        foo: {
            cls: 'my-foo-trigger',
            handler: 'onTriggerSlider_DirNomen1NameClick'
        }
    },
},
{
    xtype: 'viewTriggerDirField',
    allowBlank: false,
    name: 'Slider_DirNomen1ID', itemId: "Slider_DirNomen1ID", id: "Slider_DirNomen1ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall
},

А так же использовать ВьюКонтроллер

*/


//Триггер для выбора справочника - Контроллер, тот в котором находится виджет
//Используется в Константах
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewTriggerDirAll", {
    extend: "Ext.form.field.Text",
    alias: "widget.viewTriggerDirAll",


    width: "95%",
    editable: false,
    allowBlank: false,

    conf: {},

    initComponent: function () {
        this.callParent(arguments);
    }

});