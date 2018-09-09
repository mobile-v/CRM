Ext.define('PartionnyAccount.view.Sklad/Container/TreeList', {
    extend: 'Ext.panel.Panel',
    xtype: 'tree-list',
    region: "center", //width: 500, height: 450,
    title: 'TreeList',
    controller: 'tree-list',

    iconCls: 'fa fa-gears',
    layout: 'border',

    viewModel: {
        type: 'tree-list'
    },

    header: {
        items: [{
            xtype: 'button',
            text: 'Options',
            menu: [{
                text: 'Expander Only',
                checked: true,
                handler: 'onToggleConfig',
                config: 'expanderOnly'
            }, {
                text: 'Single Expand',
                checked: false,
                handler: 'onToggleConfig',
                config: 'singleExpand'
            }]
        }, {
            xtype: 'button',
            text: 'Nav',
            enableToggle: true,
            reference: 'navBtn',
            toggleHandler: 'onToggleNav'
        }, {
            xtype: 'button',
            text: 'Micro',
            enableToggle: true,
            toggleHandler: 'onToggleMicro'
        }]
    },

    items: [{
        region: 'west',
        width: 250,
        split: true,
        reference: 'treelistContainer',

        //cls: "background-color: #32404e;",
        //bodyStyle: { "background-color": "#32404e" },

        layout: {
            type: 'vbox',
            align: 'stretch'
        },
        border: false,
        scrollable: 'y',
        items: [{
            xtype: 'treelist',
            reference: 'treelist',
            bind: '{navItems}',
            //bodyStyle: { "background-color": "#32404e" },
        }]
    }, {
        region: 'center',
        bodyPadding: 10,
        bind: {
            html: '{selectionText}'
        }
    }]
});