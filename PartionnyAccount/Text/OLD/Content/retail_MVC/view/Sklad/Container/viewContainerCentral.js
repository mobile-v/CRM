//Центральная панель
Ext.define("PartionnyAccount.view.Sklad/Container/viewContainerCentral", {
    extend: "Ext.panel.Panel",
    alias: "widget.viewContainerCentral",

    xtype: "tree-list",
    title: "InTradeCloud",
    region: "center",
    iconCls: "fa fa-barcode",
    layout: "border",
    region: "center",


    header: {
        items: [
            {
                xtype: "button",
                text: "Options",
                menu: [
                    {
                        text: "Expander Only",
                        checked: true,
                        handler: "onToggleConfig", itemId: "onToggleConfig1",
                        config: "expanderOnly"
                    }, {
                        text: "Single Expand",
                        checked: false,
                        handler: "onToggleConfig", itemId: "onToggleConfig2",
                        config: "singleExpand"
                    }
                ]
            },
            {
                xtype: "button",
                text: "Nav",
                enableToggle: true,
                reference: "navBtn",
                toggleHandler: "onToggleNav", itemId: "onToggleNav",
            },
            {
                xtype: "button",
                text: "Micro",
                enableToggle: true,
                toggleHandler: "onToggleMicro", itemId: "onToggleMicro",
            }
        ]
    },




    items: [

        // *** *** *** west *** *** *** ***

        Ext.create('Ext.tree.Panel', { //Ext.list.Tree

            xtype: 'treelist', reference: 'treelist', id: "treelist",
            cls: 'background-color: #32404e;',
            expanderOnly: false,

            region: "west",
            width: 225,
            split: true,
            useArrows: true,
            itemId: "tree",
            rootVisible: false,

            //bind: '{navItems}',


            root: {
                expanded: true,
                children: [
                    {
                        text: 'Чек',
                        iconCls: 'x-fa fa-barcode',
                        children: [
                            {
                                text: 'Список',
                                iconCls: 'x-fa fa-list-alt',
                                leaf: true
                            },
                            {
                                text: 'Новый',
                                iconCls: 'x-fa fa-newspaper-o',
                                leaf: true
                            }
                        ]
                    },
                {
                    text: 'Возврат',
                    iconCls: 'x-fa fa-external-link-square',
                    children: [
                        {
                            text: 'Список',
                            iconCls: 'x-fa fa-list-alt',
                            leaf: true
                        },
                        {
                            text: 'Новый',
                            iconCls: 'x-fa fa-newspaper-o',
                            leaf: true
                        }
                    ]
                },
                {
                    text: 'Отчеты',
                    iconCls: 'x-fa fa-bar-chart',
                    children: [
                        {
                            text: 'Денег в кассе',
                            iconCls: 'x-fa fa-money',
                            leaf: true
                        }
                    ]
                }
                ]
            }

        }),


        // *** *** *** center *** *** *** ***

        Ext.create('Ext.panel.Panel', {
                id: "viewContainerCentralPanel",
                layout: 'card',
                bodyPadding: 10,
                region: "center",
                //bodyStyle: "background-color: white", //bodyStyle: "background-color: transparent !important",
        }),


        // *** *** *** south *** *** *** ***

        Ext.create('Ext.panel.Panel', {
            height: 40,
            region: "south",
            title: "<center>" + varCopyrightSystem + lanOrgName + "</center>"
        }),
    ]


});