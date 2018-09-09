//Триггер для поиска в объектах
//Используется в табличных частях
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewTriggerSearch", {
    extend: "Ext.form.field.Text",
    alias: "widget.viewTriggerSearch",


    //x-form-clear-trigger     // the X icon
    //x-form-search-trigger    // the magnifying glass icon
    //x-form-trigger           // the down arrow (default for combobox) icon
    //x-form-date-trigger      // the calendar icon (just in case)
    triggers: {
        /*foo: {
            cls: 'x-form-search-trigger',
            handler: function () {
                //console.log('Select Group or Direction!');
                //this.fireEvent("ontriggerclick", this, event);
            },
        },*/
        foo: {
            cls: 'x-form-search-trigger',
            handler: 'onTriggerSearchTreeClick1'
        }

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

    width: "25%",
    editable: true,
    //allowBlank: false,

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