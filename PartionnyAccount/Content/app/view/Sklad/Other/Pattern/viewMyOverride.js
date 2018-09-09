Ext.define('PartionnyAccount.view.Sklad/Other/Pattern/viewMyOverride', {
    override: 'widget.viewGrid', //'Ext.grid.header.Container',

    getMenuItems: function () {
        var items = this.callParent();
        items.push({
            itemId: 'customItem',
            text: 'Hi there!',
            handler: function () {
                alert(111);
            }
        });
        return items;
    }
});