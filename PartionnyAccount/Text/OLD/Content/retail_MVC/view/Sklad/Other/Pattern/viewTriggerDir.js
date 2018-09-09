//Триггер для выбора справочника - Контроллер, тот в котором находится виджет
//Используется в Константах
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewTriggerDir", {
    extend: "Ext.form.field.Text",
    alias: "widget.viewTriggerDir",


    //x-form-clear-trigger     // the X icon
    //x-form-search-trigger    // the magnifying glass icon
    //x-form-trigger           // the down arrow (default for combobox) icon
    //x-form-date-trigger      // the calendar icon (just in case)
    triggers: {
        foo: {
            cls: 'my-foo-trigger',
            handler: function () {
                //console.log('Select Group or Direction!');
                this.fireEvent("ontriggerclick", this, event);
            },
        },
        /*foreign: {
            handler: function () {
                console.log('foreign');
                this.fireEvent("ontriggerclick", this, event);
            },
        },
        clear: {
            cls: 'x-form-clear-trigger',
            scope: "controller",
            handler: function () {
                console.log('clear');
                this.fireEvent("ontriggerclick", this, event);
            },
        }*/
    },
    //itemId: "TriggerDir",

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
    }

});