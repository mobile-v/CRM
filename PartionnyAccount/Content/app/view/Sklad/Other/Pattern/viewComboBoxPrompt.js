//ComboBox для выбора справочника, в которіх небольшое к-во записей и отсутствуют группы
//Используется в Справочник.Контрагент
Ext.define("PartionnyAccount.view.Sklad/Other/Pattern/viewComboBoxPrompt", {
    extend: "Ext.window.MessageBox",
    initComponent: function () {
        this.callParent();
        var index = this.promptContainer.items.indexOf(this.textField);
        this.promptContainer.remove(this.textField);
        this.textField = this._createComboBox();
        this.promptContainer.insert(index, this.textField);
    },

    _createComboBox: function () {
        //copy paste what is being done in the initComonent to create the textfield
        return new Ext.form.ComboBox({
            //id: this.id + "ComboBox",
            anchor: "100%",
            enableKeyEvents: true,
            listeners: {
                keydown: this.onPromptKey,
                scope: this
            },

            
            store: this.store, // store getting items from server
            valueField: this.valueField,
            hiddenName: this.hiddenName,
            displayField: this.displayField,
            name: this.name, itemId: this.itemId, //id: "DirCurrencyID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,


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
            allowBlank: true,
            ///Поиск
            editable: true, typeAhead: true, minChars: 2,

        });
    }

});