Ext.define('PartionnyAccount.view.Sklad/Container/viewContainerCentral', {
    extend: 'Ext.panel.Panel',
    xtype: 'viewContainerCentral',

    layout: 'border',
    region: "center",
    title: 'ВТорговомОблаке',
    iconCls: 'fa fa-barcode',

    //Контроллер
    controller: 'controllerContainerCentral',
    //Модель
    viewModel: {
        type: 'viewmodelContainerCentral'
    },

    header: {
        items: [
            {
                xtype: 'button',
                iconCls: 'x-fa fa-bars ',
                enableToggle: true,
                reference: 'btnMenu',
                toggleHandler: 'onBtnMenu'
            },
            {
                xtype: 'button',
                text: 'Options',
                menu: [
                    {
                        text: 'Expander Only',
                        checked: true,
                        handler: 'onToggleConfig',
                        config: 'expanderOnly'
                    },
                    {
                        text: 'Single Expand',
                        checked: false,
                        handler: 'onToggleConfig',
                        config: 'singleExpand'
                    }
                ]
            },
            {
                xtype: 'button',
                text: 'Nav',
                enableToggle: true,
                reference: 'navBtn',
                toggleHandler: 'onToggleNav'
            },
            {
                xtype: 'button',
                text: 'Micro',
                enableToggle: true,
                toggleHandler: 'onToggleMicro'
            }
        ]
    },

    items: [
        {
            region: 'west',
            width: 175,
            split: true,
            reference: 'treelistContainer',
            //ui: 'navigation',
            layout: {
                type: 'vbox',
                align: 'stretch'
            },
            border: false,
            scrollable: 'y',
            items: [
                {
                    xtype: 'treelist',
                    reference: 'treelist', id: "treelist",
                    bind: '{navItems}',
                    //bodyStyle: { "background-color": "#32404e" },
                    //ui: 'navigation',
                    //expanderFirst: false,
                    expanderOnly: false,

                    //toggleHandler: 'onTreelisttemClick',
                    //itemId: "onTreelisttemClick",
                    listeners: { itemclick: 'onTreelisttemClick' }
                }
            ]
        },
        {
            xtype: 'viewContainerCentralPanel',
            id: "viewContainerCentral",
            region: "center",
            bodyPadding: 5,
            layout: 'card',

            //bind: { html: '{selectionText}' }
        },

    ]
});